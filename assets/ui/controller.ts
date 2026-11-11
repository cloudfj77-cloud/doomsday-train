/**
 * Frontend root (v0.2): owns the economy (assets/sim), every player-visible word, the autosave and the HUD.
 * 到站后点列车上的车厢进入该节功能〔01·05〕；货舱是可拖放的二维格子；探索站出击进入 Canvas 俯视现场。
 */
import type { GameHost } from '@forgeax/engine/app';
import { mountUi, type UiAsset } from '@forgeax/engine/ui';
import { bindUiLocalization, createUiLocalization } from '@forgeax/engine/ui/localization';
import {
  CAR_KINDS,
  CARS,
  DEVICE_KINDS,
  DEVICES,
  DRIVERS,
  EXP,
  ITEMS,
  isCommerce,
  PLANT,
  RECIPES,
  RULES,
  SERVICES,
  unitOf,
  type CarKind,
  type Grade,
  type ItemId,
  type NodeKind,
} from '../sim/data.ts';
import { dims, type Stack } from '../sim/grid.ts';
import {
  arrive,
  boardPassenger,
  buyBatch,
  buyCar,
  buySingle,
  canEndDay,
  carLevel,
  chooseRoute,
  closeBill,
  collectOutput,
  commerce,
  continueAfterWin,
  count,
  debtPreview,
  depart,
  dilute,
  discardPending,
  discardStack,
  dueAmount,
  endDay,
  endSortie,
  equipGun,
  findCar,
  forfeit,
  haggle,
  haggleOdds,
  harvest,
  hireDriver,
  inspect,
  installDevice,
  inventoryValue,
  isGame,
  load,
  loadAmmo,
  moveItem,
  newGame,
  nodesLeft,
  passCustomer,
  paySettlement,
  placePending,
  plant,
  previewSale,
  quota,
  relabel,
  repay,
  retailPrice,
  routeActive,
  routeDone,
  seats,
  segmentFuel,
  sellCar,
  sellToBuyer,
  sellToRider,
  skipSite,
  slotsUsed,
  sortieInit,
  splitItem,
  startJob,
  startSortie,
  storageCustomers,
  tidy,
  upgradeCar,
  wantsRoute,
  type ActionResult,
  type Car,
  type Customer,
  type Game,
} from '../sim/economy.ts';
import { isEngineMessage, VIEW_CHANNEL, type EngineMessage, type ViewMessage } from '../shared/protocol.ts';
import { initialLanguage, LANG_KEY, type GameLanguage } from './language.ts';
import { runExplore, type ExploreSummary } from './explore.ts';

type Host = Pick<GameHost, 'uiRoot' | 'port' | 'presentationPort' | 'setPointerLockAllowed'>;
type Tab = 'car' | 'cargo' | 'depot';

export const SAVE_KEY = 'doomsday.save.v2';
const EXP_KEY = 'doomsday.exp';
export const TRAVEL_MS = 2600;
const CELL = 42;

export const CHIP: Readonly<Record<ItemId, string>> = {
  ration: '#c9a36a',
  water: '#6fb7d8',
  veg: '#7fbf5a',
  sewage: '#6b6a4a',
  scrap: '#8a7f78',
  ingot: '#b9b2a8',
  parts: '#d68a3a',
  seed: '#a0c060',
  research: '#9a7fd0',
  bandage: '#efe6d6',
  pistol: '#5a6470',
  smg: '#3f4a55',
  ammo: '#c8a24a',
};
const GRADE_CLASS = ['poor', 'fair', 'fine'];

export async function createGameUi(asset: UiAsset, host: Host): Promise<{ dispose(): void }> {
  const root = host.uiRoot;
  if (!root) throw new Error('Doomsday Train UI requires GameHost.uiRoot');
  host.setPointerLockAllowed?.(false);
  const document = root.ownerDocument;
  const view = document.defaultView!;
  const params = new view.URLSearchParams(view.location.search);
  const qa = params.get('qa') === '1';
  const storage = (() => {
    try {
      return view.localStorage;
    } catch {
      return undefined;
    }
  })();
  const first = initialLanguage({
    saved: storage?.getItem(LANG_KEY) ?? null,
    browser: [...(view.navigator.languages ?? []), view.navigator.language],
    override: params.get('lang'),
  });
  const localized = await createUiLocalization(asset, { lng: first.lang });
  if (!localized.ok) throw localized.error;
  const i18n = localized.value;
  const t = (key: string, options?: Record<string, unknown>) => i18n.t(key, options as never) as unknown as string;

  // --- State -----------------------------------------------------------------------------
  let game: Game | null = null;
  let tab: Tab = 'car';
  let selCar = 0;
  let routeDeferred = false;
  let settleSelling = false;
  let travelling = false;
  let travelSerial = 0;
  let travelTimer: number | undefined;
  let travelStart = 0;
  let engine: EngineMessage | undefined;
  let experimental = storage?.getItem(EXP_KEY) !== '0';
  let bandagePick = 1;
  let selStack: number | undefined;
  let summary: { readonly outcome: string; readonly s: ExploreSummary; readonly note?: string } | undefined;
  let explore: { dispose(): void } | undefined;
  let exploreState: unknown = null;
  const picks = new Map<number, number[]>();
  const toasts: { id: number; text: string; kind: string }[] = [];
  let toastId = 0;

  const loadSave = (): Game | null => {
    try {
      const raw = storage?.getItem(SAVE_KEY);
      if (!raw) return null;
      const parsed = JSON.parse(raw) as unknown;
      return isGame(parsed) ? parsed : null;
    } catch {
      return null;
    }
  };
  const save = () => {
    if (game) storage?.setItem(SAVE_KEY, JSON.stringify(game));
  };

  const port = host.presentationPort ?? host.port;
  const postView = () => {
    const g = game;
    const message: ViewMessage = {
      channel: VIEW_CHANNEL,
      mode: !g ? 'title' : travelling ? 'travel' : 'station',
      region: g?.station.kind ?? 'rich',
      stationSeed: g?.station.id ?? 0,
      cars: g?.cars.map((c) => c.kind) ?? [],
      travelMs: TRAVEL_MS,
      travelSerial,
      riders: g?.riders.length ?? 0,
    };
    port?.postMessage(message);
  };
  const onPort = (event: MessageEvent) => {
    if (isEngineMessage(event.data)) {
      engine = event.data;
      mirror();
    }
  };
  port?.addEventListener('message', onPort);
  port?.start?.();

  // --- Mount -----------------------------------------------------------------------------
  const mounted = mountUi(asset, { root, layer: 50 });
  if (!mounted.ok) throw mounted.error;
  const ui = mounted.value;
  const shadowRoot = ui.host.shadowRoot;
  if (!shadowRoot) throw new Error('Doomsday Train UI requires a shadow root');
  const shadow: ShadowRoot = shadowRoot;
  const part = <T extends HTMLElement = HTMLElement>(name: string) => shadow.querySelector<T>(`[data-ui-part="${name}"]`)!;
  const flag = (name: string, on: boolean) => ui.host.classList.toggle(name, on);

  // --- Formatting helpers ---------------------------------------------------------------
  const money = (n: number) => (n < 0 ? `−${Math.abs(Math.round(n))}` : String(Math.round(n)));
  const itemName = (id: ItemId) => t(`item.${id}`);
  const chip = (id: ItemId) => `<span class="chip" style="background:${CHIP[id]}"></span>`;
  const gradeTag = (g: Game, label?: Grade, real?: Grade, revealed = false) => {
    if (!g.exp || label === undefined) return '';
    const shown = revealed ? real ?? label : label;
    const fake = revealed && (real ?? label) < label;
    return `<span class="grade ${GRADE_CLASS[shown]}">${t(`grade.${shown}`)}${fake ? ` · ${t('grade.fake', { label: t(`grade.${label}`) })}` : ''}</span>`;
  };
  const itemLabel = (id: ItemId) => `<span class="item">${chip(id)}${itemName(id)}</span>`;
  const qtyText = (id: ItemId, baseQty: number) => (id === 'ammo' ? t('unit.rounds', { n: baseQty }) : `×${baseQty}`);
  const kindName = (k: NodeKind) => t(`region.${k}`);
  const placeName = (k: NodeKind, place: number) => t(`place.${k}.p${place}`);
  const stationName = () => (game ? placeName(game.station.kind, game.station.place) : '');
  const regionBadge = (k: NodeKind) => `<span class="region ${k}">${kindName(k)}</span>`;
  const carName = (g: Game, uid: number) => {
    if (uid === 0) return t('car.loco');
    const car = findCar(g, uid);
    if (!car) return '?';
    const same = g.cars.filter((c) => c.kind === car.kind);
    return `${t(`car.${car.kind}`)}${same.length > 1 ? ` ${same.indexOf(car) + 1}` : ''}`;
  };
  const routeCost = (g: Game, stops: number) =>
    Math.ceil(stops * segmentFuel(g)) + stops * quota(g).wage + RULES.livingPerHead * 2;

  const toast = (text: string, kind = '') => {
    const id = ++toastId;
    toasts.push({ id, text, kind });
    while (toasts.length > 4) toasts.shift();
    renderToasts();
    view.setTimeout(() => {
      const index = toasts.findIndex((x) => x.id === id);
      if (index >= 0) toasts.splice(index, 1);
      renderToasts();
    }, 3000);
  };
  const renderToasts = () => {
    part('toasts').innerHTML = toasts.map((x) => `<div class="toast ${x.kind}">${x.text}</div>`).join('');
  };
  const report = (result: ActionResult, okKey?: string, kind = 'money') => {
    if (!result.ok) {
      toast(t(`err.${result.code.replace(/-([a-z])/g, (_m, c: string) => c.toUpperCase())}`), 'bad');
      return false;
    }
    if (okKey) toast(t(okKey, { amount: result.amount ?? 0 }), kind);
    return true;
  };

  // --- Actions ---------------------------------------------------------------------------
  const resetUi = () => {
    tab = 'car';
    selCar = 0;
    routeDeferred = false;
    settleSelling = false;
    selStack = undefined;
    summary = undefined;
    picks.clear();
  };
  const startNew = () => {
    const seedParam = qa ? Number(params.get('seed')) : NaN;
    game = newGame(Number.isFinite(seedParam) && seedParam !== 0 ? seedParam : (Date.now() ^ Math.floor(Math.random() * 1e9)) | 0, experimental);
    resetUi();
    commit();
  };
  const commit = () => {
    save();
    render();
    postView();
  };

  const doDepart = () => {
    if (!game || travelling) return;
    const result = depart(game);
    if (!report(result)) return;
    arrive(game);
    travelling = true;
    travelSerial += 1;
    travelStart = view.performance.now();
    picks.clear();
    selStack = undefined;
    save();
    render();
    postView();
    const tick = () => {
      if (!travelling) return;
      const p = Math.min(1, (view.performance.now() - travelStart) / TRAVEL_MS);
      part('travel-fill').style.width = `${Math.round(p * 396)}px`;
      if (p < 1) view.requestAnimationFrame(tick);
    };
    view.requestAnimationFrame(tick);
    travelTimer = view.setTimeout(() => {
      travelling = false;
      if (!game) return;
      toast(t('toast.arrived', { place: stationName() }), '');
      if (game.today.delivered > lastDelivered) toast(t('toast.delivered', { n: game.today.delivered - lastDelivered }), '');
      lastDelivered = game.today.delivered;
      const insp = game.station.inspector;
      if (insp) toast(insp.seized > 0 ? t('toast.inspector', { n: insp.seized, fine: insp.fined }) : t('toast.inspectorClean'), insp.seized > 0 ? 'bad' : '');
      tab = 'car';
      selCar = 0;
      render();
      postView();
    }, TRAVEL_MS);
  };
  let lastDelivered = 0;

  const beginExplore = () => {
    const g = game;
    if (!g || explore) return;
    const init = sortieInit(g);
    if (!init) return;
    flag('exploring', true);
    explore = runExplore({
      root: part('explore-layer'),
      init,
      t,
      itemName,
      chip: CHIP,
      expose: qa ? (s) => (exploreState = s) : undefined,
      onEnd: (result, s) => {
        explore = undefined;
        exploreState = null;
        flag('exploring', false);
        if (!game) return;
        const r = endSortie(game, result);
        summary = { outcome: result.outcome, s, ...(r.ok && r.note ? { note: r.note } : {}) };
        tab = 'car';
        selCar = 0;
        commit();
      },
    });
  };

  const handle = (act: string, el: HTMLElement) => {
    const id = Number(el.dataset.id ?? -1);
    const g = game;
    switch (act) {
      case 'new-game':
        startNew();
        return;
      case 'toggle-exp':
        experimental = !experimental;
        storage?.setItem(EXP_KEY, experimental ? '1' : '0');
        render();
        return;
      case 'continue': {
        const loaded = loadSave();
        if (loaded && loaded.phase !== 'lost') {
          game = loaded;
          lastDelivered = loaded.today.delivered;
          resetUi();
          commit();
          if (game.phase === 'explore') beginExplore();
        }
        return;
      }
      case 'lang':
        void setLanguage(el.dataset.lang as GameLanguage);
        return;
      case 'to-title':
        if (travelTimer) view.clearTimeout(travelTimer);
        explore?.dispose();
        explore = undefined;
        flag('exploring', false);
        travelling = false;
        game = null;
        render();
        postView();
        return;
    }
    if (!g) return;
    const act1 = (r: ActionResult, key?: string, kind?: string) => {
      report(r, key, kind);
      commit();
    };
    switch (act) {
      case 'tab':
        tab = el.dataset.tab as Tab;
        selStack = undefined;
        render();
        return;
      case 'car':
        selCar = id;
        tab = 'car';
        render();
        return;
      case 'open-routes':
        routeDeferred = false;
        render();
        return;
      case 'defer-routes':
        routeDeferred = true;
        render();
        return;
      case 'choose-route':
        if (!report(chooseRoute(g, Number(el.dataset.index)))) return;
        toast(t('toast.routeLocked'), '');
        commit();
        return;
      case 'step': {
        const c = g.station.customers.find((x) => x.id === id);
        if (!c?.goods) return;
        const row = Number(el.dataset.row);
        const q = picks.get(id) ?? c.goods.map((x) => x.qty);
        q[row] = Math.max(0, Math.min(c.goods[row]!.qty, (q[row] ?? 0) + Number(el.dataset.delta)));
        picks.set(id, q);
        render();
        return;
      }
      case 'buy-single': {
        const c = g.station.customers.find((x) => x.id === id);
        if (!c?.goods) return;
        return act1(buySingle(g, id, picks.get(id) ?? c.goods.map((x) => x.qty)), 'toast.bought');
      }
      case 'buy-batch':
        return act1(buyBatch(g, id), 'toast.bought');
      case 'sell': {
        const r = sellToBuyer(g, id);
        if (r.ok && r.note === 'caught') toast(t('toast.caught', { rep: EXP.detectRep }), 'bad');
        else if (r.ok && r.note === 'cheated') toast(t('toast.cheated', { amount: r.amount }), 'money');
        else report(r, 'toast.sold');
        commit();
        return;
      }
      case 'pass':
        return act1(passCustomer(g, id));
      case 'haggle': {
        const r = haggle(g, id, Number(el.dataset.pct));
        if (r.ok) toast(t(`toast.haggle.${r.note}`), r.note === 'ok' ? 'money' : r.note === 'walk' ? 'bad' : '');
        else report(r);
        commit();
        return;
      }
      case 'inspect': {
        const r = inspect(g, id);
        if (r.ok) toast(r.note === 'fake' ? t('toast.inspectFake', { n: r.amount }) : t('toast.inspectClean'), r.note === 'fake' ? 'money' : '');
        else report(r);
        commit();
        return;
      }
      case 'board': {
        const r = boardPassenger(g, id);
        if (!r.ok && r.code === 'declined') toast(t('toast.declined'), 'bad');
        else report(r, 'toast.boarded');
        commit();
        return;
      }
      case 'sell-rider':
        return act1(sellToRider(g, id), 'toast.riderSold');
      case 'hire':
        return act1(hireDriver(g), 'toast.hired');
      case 'equip':
        return act1(equipGun(g, el.dataset.item as 'pistol' | 'smg'), 'toast.equipped', '');
      case 'load-ammo':
        return act1(loadAmmo(g), 'toast.ammo', '');
      case 'buy-car':
        return act1(buyCar(g, el.dataset.kind as CarKind), 'toast.car');
      case 'upgrade-car':
        return act1(upgradeCar(g, id), 'toast.upgraded');
      case 'sell-car':
        if (report(sellCar(g, id), 'toast.carSold')) selCar = 0;
        commit();
        return;
      case 'plant':
        return act1(plant(g, id, Number(el.dataset.index)), 'toast.planted', '');
      case 'harvest':
        return act1(harvest(g, id, Number(el.dataset.index)), 'toast.harvested', '');
      case 'install':
        return act1(installDevice(g, id, el.dataset.kind as never), 'toast.installed');
      case 'job':
        return act1(startJob(g, id, el.dataset.recipe!), 'toast.jobStarted', '');
      case 'collect':
        return act1(collectOutput(g, id), 'toast.collected', '');
      case 'repay':
        return act1(repay(g, Number(el.dataset.amount)), 'toast.repaid');
      case 'tidy':
        return act1(tidy(g), 'toast.tidied', '');
      case 'dilute':
        return act1(dilute(g), 'toast.diluted', '');
      case 'relabel':
        return act1(relabel(g, selStack ?? -1), 'toast.relabelled', '');
      case 'split':
        return act1(splitItem(g, selStack ?? -1));
      case 'discard':
        if (report(discardStack(g, selStack ?? -1))) selStack = undefined;
        commit();
        return;
      case 'rotate':
        if (selStack !== undefined) rotateStack(g, selStack);
        return;
      case 'place-pending':
        return act1(placePending(g, Number(el.dataset.index)));
      case 'drop-pending':
        return act1(discardPending(g, Number(el.dataset.index)));
      case 'band':
        bandagePick = Math.max(0, Math.min(count(g, 'bandage'), bandagePick + Number(el.dataset.delta)));
        render();
        return;
      case 'sortie':
        if (report(startSortie(g, Math.min(bandagePick, count(g, 'bandage'))))) {
          commit();
          beginExplore();
        }
        return;
      case 'skip-site':
        return act1(skipSite(g));
      case 'close-summary':
        summary = undefined;
        render();
        return;
      case 'depart':
        doDepart();
        return;
      case 'end-day':
        return act1(endDay(g));
      case 'close-bill':
        report(closeBill(g));
        lastDelivered = 0;
        resetUi();
        commit();
        return;
      case 'settle-sell':
        settleSelling = true;
        tab = 'car';
        render();
        return;
      case 'settle-back':
        settleSelling = false;
        render();
        return;
      case 'settle-pay': {
        const due = g.settlement ? dueAmount(g, g.settlement.required) : 0;
        if (report(paySettlement(g))) {
          toast(t('toast.settled', { amount: due }), 'money');
          lastDelivered = 0;
          resetUi();
        }
        commit();
        return;
      }
      case 'forfeit':
        forfeit(g);
        settleSelling = false;
        commit();
        return;
      case 'win-continue':
        continueAfterWin(g);
        commit();
        return;
      case 'retry':
        startNew();
        return;
    }
  };

  const onClick = (event: Event) => {
    const target = (event.target as HTMLElement | null)?.closest<HTMLElement>('[data-act]');
    if (!target || (target as HTMLButtonElement).disabled) return;
    event.preventDefault();
    handle(target.dataset.act!, target);
    const active = shadow.activeElement;
    if (active instanceof view.HTMLElement) active.blur();
  };
  shadow.addEventListener('click', onClick);

  // --- Grid drag & drop --------------------------------------------------------------------
  let drag: { id: number; rot: boolean; ghost: HTMLElement; ox: number; oy: number; moved: boolean; sx: number; sy: number } | undefined;
  const findStackIn = (g: Game, id: number): Stack | undefined => g.holds.flatMap((h) => h.stacks).find((s) => s.id === id);
  const rotateStack = (g: Game, id: number) => {
    const found = g.holds.find((h) => h.stacks.some((s) => s.id === id));
    const s = findStackIn(g, id);
    if (!found || !s) return;
    if (!report(moveItem(g, id, found.id, s.x, s.y, !s.rot))) return;
    commit();
  };
  const onPointerDown = (event: PointerEvent) => {
    const el = (event.target as HTMLElement | null)?.closest<HTMLElement>('[data-stack]');
    if (!el || !game || event.button !== 0) return;
    const s = findStackIn(game, Number(el.dataset.stack));
    if (!s) return;
    const rect = el.getBoundingClientRect();
    const ghost = el.cloneNode(true) as HTMLElement;
    ghost.classList.add('ghost');
    ghost.style.left = `${rect.left}px`;
    ghost.style.top = `${rect.top}px`;
    drag = { id: s.id, rot: s.rot, ghost, ox: event.clientX - rect.left, oy: event.clientY - rect.top, moved: false, sx: event.clientX, sy: event.clientY };
    event.preventDefault();
  };
  const onPointerMove = (event: PointerEvent) => {
    if (!drag || !game) return;
    if (!drag.moved && Math.hypot(event.clientX - drag.sx, event.clientY - drag.sy) < 5) return;
    if (!drag.moved) {
      drag.moved = true;
      part('drag-layer').appendChild(drag.ghost);
    }
    drag.ghost.style.left = `${event.clientX - drag.ox}px`;
    drag.ghost.style.top = `${event.clientY - drag.oy}px`;
  };
  const dropTarget = (x: number, y: number) => {
    for (const el of shadow.querySelectorAll<HTMLElement>('[data-hold-grid]')) {
      const r = el.getBoundingClientRect();
      if (x >= r.left && x < r.right && y >= r.top && y < r.bottom) {
        return { hold: Number(el.dataset.holdGrid), x: Math.floor((x - r.left) / CELL), y: Math.floor((y - r.top) / CELL) };
      }
    }
    return undefined;
  };
  const onPointerUp = (event: PointerEvent) => {
    if (!drag || !game) return;
    const d = drag;
    drag = undefined;
    d.ghost.remove();
    if (!d.moved) {
      selStack = selStack === d.id ? undefined : d.id;
      render();
      return;
    }
    const target = dropTarget(event.clientX - d.ox + CELL / 2, event.clientY - d.oy + CELL / 2);
    if (!target) return;
    report(moveItem(game, d.id, target.hold, target.x, target.y, d.rot));
    commit();
  };
  const onContext = (event: MouseEvent) => {
    const el = (event.target as HTMLElement | null)?.closest<HTMLElement>('[data-stack]');
    if (!el || !game) return;
    event.preventDefault();
    rotateStack(game, Number(el.dataset.stack));
  };
  const onKey = (event: KeyboardEvent) => {
    if (!game || explore) return;
    if (event.key.toLowerCase() === 'r') {
      if (drag) {
        drag.rot = !drag.rot;
        const s = findStackIn(game, drag.id);
        if (s) {
          const [w, h] = dims(s.item, drag.rot);
          drag.ghost.style.width = `${w * CELL - 2}px`;
          drag.ghost.style.height = `${h * CELL - 2}px`;
        }
      } else if (selStack !== undefined && tab === 'cargo') rotateStack(game, selStack);
    }
  };
  shadow.addEventListener('pointerdown', onPointerDown as EventListener);
  view.addEventListener('pointermove', onPointerMove);
  view.addEventListener('pointerup', onPointerUp);
  shadow.addEventListener('contextmenu', onContext as EventListener);
  view.addEventListener('keydown', onKey);

  const setLanguage = async (lang: GameLanguage) => {
    storage?.setItem(LANG_KEY, lang);
    await i18n.changeLanguage(lang);
  };

  // --- Rendering -------------------------------------------------------------------------
  const renderTitle = () => {
    const saved = loadSave();
    const canContinue = !!saved && saved.phase !== 'lost';
    part<HTMLButtonElement>('continue-btn').disabled = !canContinue;
    part('save-info').textContent = canContinue ? t('cover.saveInfo', { day: saved!.day, cash: money(saved!.cash), debt: saved!.debt }) : t('cover.noSave');
    part('exp-toggle').innerHTML = `<button type="button" class="exp-btn ${experimental ? 'on' : ''}" data-act="toggle-exp"><b class="box">${experimental ? '✓' : ''}</b>${t('cover.exp')}</button><small>${t('cover.expDesc')}</small>`;
  };

  const customerCard = (g: Game, c: Customer) => {
    const done = !!c.outcome;
    const settle = g.phase === 'settlement';
    const statusTag = () => {
      if (c.outcome === 'deal') return `<span class="tag good">${t('cust.deal', { amount: money(c.amount ?? 0) })}</span>`;
      if (c.outcome === 'pass') return `<span class="tag">${t('cust.passed')}</span>`;
      if (c.outcome === 'walk') return `<span class="tag bad">${t('cust.walked')}</span>`;
      if (c.outcome === 'caught') return `<span class="tag bad">${t('cust.caught')}</span>`;
      return '';
    };
    const haggleRow = (kind: 'supplier' | 'buyer') => {
      if (!g.exp || done || settle || c.haggle) return c.haggle ? `<span class="tag ${c.haggle.result === 'ok' ? 'good' : ''}">${t(`cust.haggle.${c.haggle.result}`, { pct: c.haggle.pct })}</span>` : '';
      return `<div class="haggle"><span class="muted">${t(kind === 'supplier' ? 'cust.askDiscount' : 'cust.askMarkup')}</span>${EXP.haggle
        .map((pct) => {
          const o = haggleOdds(g, pct);
          return `<button type="button" class="btn small" data-act="haggle" data-id="${c.id}" data-pct="${pct}" title="${t('cust.odds', o)}">${kind === 'supplier' ? '−' : '+'}${pct}% <small>${o.accept}%</small></button>`;
        })
        .join('')}</div>`;
    };
    if (c.kind === 'supplier') {
      const goods = c.goods ?? [];
      const q = picks.get(c.id) ?? goods.map((x) => x.qty);
      const total = goods.reduce((sum, x, i) => sum + x.unit * (q[i] ?? 0), 0);
      const lines = goods
        .map(
          (x, i) => `<div class="line">${itemLabel(x.item)}${gradeTag(g, x.label, x.real, c.inspected)}<span class="muted">${t('cust.unit', { price: x.unit })}${x.item === 'ammo' ? t('unit.per30') : ''} · ${t('cust.retail', { price: retailPrice(g, x.item, x.label) })}</span>
          ${done ? `<span class="price">×${x.qty}</span>` : `<span class="stepper"><button type="button" data-act="step" data-id="${c.id}" data-row="${i}" data-delta="-1">−</button><span>${q[i]}/${x.qty}</span><button type="button" data-act="step" data-id="${c.id}" data-row="${i}" data-delta="1">+</button></span>`}</div>`,
        )
        .join('');
      const batchTag = c.discount !== undefined ? `<span class="tag good">${t('cust.batchTag', { pct: Math.round(c.discount * 100) })}</span>` : `<span class="tag">${t('cust.noBatch')}</span>`;
      const graded = goods.some((x) => x.label !== undefined);
      const inspectBtn = g.exp && graded && !done && !c.inspected
        ? `<button type="button" class="btn small" data-act="inspect" data-id="${c.id}" ${g.station.inspections > 0 ? '' : 'disabled'}>${t('cust.inspect', { n: g.station.inspections })}</button>`
        : c.inspected ? `<span class="tag">${t('cust.inspected')}</span>` : '';
      const actions = done || settle
        ? ''
        : `<div class="actions">
            <button type="button" class="btn teal" data-act="buy-single" data-id="${c.id}" ${total <= 0 ? 'disabled' : ''}>${t('cust.buySingle', { price: total })}</button>
            ${c.batchPrice !== undefined ? `<button type="button" class="btn go" data-act="buy-batch" data-id="${c.id}">${t('cust.buyBatch', { price: c.batchPrice })}</button>` : ''}
            <button type="button" class="btn" data-act="pass" data-id="${c.id}">${t('cust.pass')}</button>
          </div>`;
      return `<div class="card ${done ? 'done' : ''}" data-customer="${c.id}" data-kind="supplier"><div class="card-head"><span class="who supplier">${t(c.arms ? 'cust.armsSupplier' : 'cust.supplier')}</span>${statusTag() || batchTag}</div>${lines}${done ? '' : `<div class="tools">${inspectBtn}${haggleRow('supplier')}</div>`}${actions}</div>`;
    }
    const sale = previewSale(g, c);
    const lines = (c.needs ?? [])
      .map((n) => {
        const have = count(g, n.item);
        return `<div class="line">${itemLabel(n.item)}<span class="muted">×${n.qty}${n.item === 'ammo' ? t('unit.per30') : ''} · ${t('cust.retail', { price: n.price })}</span><span class="tag ${have > 0 ? 'good' : ''}">${t('cust.have', { n: n.item === 'ammo' ? t('unit.rounds', { n: have }) : have })}</span></div>`;
      })
      .join('');
    const anyStock = (c.needs ?? []).some((n) => count(g, n.item) > 0);
    const sellLabel = sale.total > 0 ? t('cust.sell', { amount: sale.total }) : anyStock ? t('cust.cantAfford') : t('cust.cantSell');
    const warn = g.exp && sale.fake && !done ? `<p class="warn">${t('cust.fakeWarn', { pct: Math.round(EXP.detect[g.station.kind as 'rich' | 'slum'] * 100) })}</p>` : '';
    const actions = done
      ? ''
      : `<div class="actions"><button type="button" class="btn go" data-act="sell" data-id="${c.id}" ${sale.total > 0 ? '' : 'disabled'}>${sellLabel}</button>${settle ? '' : `<button type="button" class="btn" data-act="pass" data-id="${c.id}">${t('cust.pass')}</button>`}</div>`;
    return `<div class="card ${done ? 'done' : ''}" data-customer="${c.id}" data-kind="buyer"><div class="card-head"><span class="who buyer">${t(c.arms ? 'cust.armsBuyer' : 'cust.buyer')}</span>${statusTag() || `<span class="tag">${t('cust.budget', { budget: c.budget ?? 0 })}</span>`}</div>${lines}${warn}${done || settle ? '' : `<div class="tools">${haggleRow('buyer')}</div>`}${actions}</div>`;
  };

  const customersOf = (g: Game, car: number) => {
    const list = g.station.customers.filter((c) => c.car === car && (g.phase !== 'settlement' || c.kind === 'buyer'));
    if (!commerce(g)) return '';
    if (!list.length) return `<p class="empty">${t('cust.none')}</p>`;
    return `<p class="hint">${g.phase === 'settlement' ? t('cust.settleHint') : t('cust.hint')}</p>${list.map((c) => customerCard(g, c)).join('')}`;
  };

  const miniGrid = (g: Game, holdId: number) => {
    const hold = g.holds.find((h) => h.id === holdId);
    if (!hold) return '';
    const s = 16;
    const items = hold.stacks
      .map((st) => {
        const [w, h] = dims(st.item, st.rot);
        return `<i style="left:${st.x * s}px;top:${st.y * s}px;width:${w * s - 1}px;height:${h * s - 1}px;background:${CHIP[st.item]}"></i>`;
      })
      .join('');
    return `<div class="mini-grid" style="width:${hold.w * s}px;height:${hold.h * s}px">${items}</div>`;
  };

  const locoPanel = (g: Game) => {
    const q = quota(g);
    const lines: string[] = [];
    lines.push(`<div class="card"><div class="card-head"><span class="who">${t('loco.driver')}</span><span class="tag">${t(`driver.${g.driver}`)}</span></div>
      <div class="line"><span class="muted">${t('loco.quota', { left: nodesLeft(g), nodes: q.nodes, lines: g.linesToday, maxLines: q.lines, wage: q.wage })}</span></div>
      ${g.pendingDriver ? `<div class="line"><span class="tag good">${t('loco.pending', { grade: t(`driver.${g.pendingDriver}`) })}</span></div>` : ''}
      ${g.station.driver && !g.station.driver.hired && !g.pendingDriver
        ? `<div class="line"><span>${t('loco.candidate', { grade: t(`driver.${g.station.driver.grade}`), nodes: DRIVERS[g.station.driver.grade].nodes, wage: DRIVERS[g.station.driver.grade].wage })}</span>
           <button type="button" class="btn go" data-act="hire" ${g.cash < DRIVERS[g.station.driver.grade].hire ? 'disabled' : ''}>${t('loco.hire', { fee: DRIVERS[g.station.driver.grade].hire })}</button></div><p class="hint">${t('loco.hireHint')}</p>`
        : ''}</div>`);
    const guns = (['pistol', 'smg'] as const).filter((x) => count(g, x) > 0);
    lines.push(`<div class="card"><div class="card-head"><span class="who">${t('loco.kit')}</span><span class="tag">${t('loco.hp', { hp: g.hp })}</span></div>
      <div class="line">${g.kit.gun ? itemLabel(g.kit.gun) : `<span class="muted">${t('loco.noGun')}</span>`}<span class="muted">${t('unit.rounds', { n: g.kit.ammo })} / ${RULES.personalAmmoMax}</span>
      <button type="button" class="btn small" data-act="load-ammo" ${count(g, 'ammo') > 0 && g.kit.ammo < RULES.personalAmmoMax ? '' : 'disabled'}>${t('loco.loadAmmo')}</button></div>
      ${guns.map((x) => `<div class="line"><span class="muted">${t('loco.spareGun', { gun: itemName(x) })}</span><button type="button" class="btn small" data-act="equip" data-item="${x}">${t('loco.equip')}</button></div>`).join('')}</div>`);
    const site = g.station.site;
    if (site) {
      const bands = count(g, 'bandage');
      const pick = Math.min(bandagePick, bands);
      const enc = site.area === 'hunt' ? t('site.huntDesc') : t('site.resourceDesc');
      if (!site.closed) {
        const recovery = site.sorties > 0 && site.recovery && site.lost;
        lines.unshift(`<div class="card site"><div class="card-head"><span class="who">${t(`theme.${site.theme}`)}</span><span class="tag bad">${t(`region.${g.station.kind}`)}</span></div>
          <p class="muted">${enc}</p>
          ${recovery ? `<p class="warn">${t('site.recoveryDesc', { n: (site.lost!.items.length) + (site.lost!.gun ? 1 : 0) })}</p>` : ''}
          ${site.sorties === 0 || recovery ? `<div class="line"><span>${t('site.bandages')}</span><span class="stepper"><button type="button" data-act="band" data-delta="-1">−</button><span>${pick}/${bands}</span><button type="button" data-act="band" data-delta="1">+</button></span></div>
          <div class="actions"><button type="button" class="big" data-act="sortie" ${g.pending.length ? 'disabled' : ''}>${t(recovery ? 'site.recover' : 'site.sortie')}</button><button type="button" class="btn" data-act="skip-site">${t(recovery ? 'site.giveUp' : 'site.skip')}</button></div>
          <p class="hint">${t('site.rules')}</p>` : ''}</div>`);
      } else lines.unshift(`<div class="card"><div class="card-head"><span class="who">${t(`theme.${site.theme}`)}</span><span class="tag">${t('site.closed')}</span></div><p class="muted">${t('site.closedDesc')}</p></div>`);
    }
    return `${lines.join('')}${commerce(g) ? `<p class="section-title">${t('car.receiving', { n: g.station.customers.filter((c) => c.car === 0).length })}</p>${customersOf(g, 0)}` : ''}`;
  };

  const passengerPanel = (g: Game) => {
    const route = g.route;
    const cands = commerce(g)
      ? g.station.passengers
          .filter((p) => p.status !== 'boarded')
          .map((p) => {
            const kind = `<span class="tag ${p.noble ? 'noble' : ''}">${t(p.noble ? 'pas.noble' : 'pas.normal')}</span>`;
            if (p.status === 'declined') return `<div class="card done"><div class="card-head"><span class="who">${t('pas.waiting')}</span>${kind}<span class="tag bad">${t('pas.declined')}</span></div></div>`;
            if (p.dest === undefined || !route) {
              const note = route && routeDone(g) ? t('pas.lastStop') : t('pas.unbound');
              return `<div class="card"><div class="card-head"><span class="who">${t('pas.waiting')}</span>${kind}</div><span class="muted">${note}</span></div>`;
            }
            const destKind = route.nodes[p.dest - 1]!.kind;
            return `<div class="card" data-passenger="${p.id}"><div class="card-head"><span class="who">${t('pas.to', { place: `${kindName(destKind)} #${p.dest}`, seg: p.seg })}</span>${kind}<span class="tag good">${p.fare}</span></div>
              <div class="line"><span class="muted">${t(p.deadline === g.day ? 'pas.dueToday' : 'pas.dueTomorrow')} · ${t('pas.comp', { pct: Math.round(p.comp! * 100) })}</span></div>
              <div class="actions"><button type="button" class="btn go" data-act="board" data-id="${p.id}">${t('pas.board', { fare: p.fare })}</button></div></div>`;
          })
          .join('')
      : '';
    const riders = g.riders
      .map((r) => {
        const have = r.want ? count(g, r.want.item) : 0;
        const want = r.want
          ? `<div class="line">${t('pas.want', { item: itemName(r.want.item), price: r.want.price })}<span class="tag ${have > 0 ? 'good' : ''}">${t('cust.have', { n: have })}</span>${r.want.sold ? `<span class="tag good">${t('pas.sold')}</span>` : `<button type="button" class="btn go" data-act="sell-rider" data-id="${r.id}" ${have >= unitOf(r.want.item) ? '' : 'disabled'}>${t('pas.sellRider', { price: r.want.price })}</button>`}</div>`
          : '';
        const destKind = route?.nodes[r.dest - 1]?.kind;
        return `<div class="card" data-rider="${r.id}"><div class="card-head"><span class="who">${t('pas.riderTo', { n: r.dest, place: destKind ? kindName(destKind) : '' })}</span><span class="tag ${r.noble ? 'noble' : ''}">${t(r.noble ? 'pas.noble' : 'pas.normal')}</span><span class="tag ${r.deadline <= g.day ? 'bad' : ''}">${t(r.deadline <= g.day ? 'pas.dueToday' : 'pas.dueTomorrow')}</span></div>${want}</div>`;
      })
      .join('');
    return `<p class="hint">${t('pas.hint')}</p><p class="section-title">${t('pas.seats', { used: g.riders.length, total: seats(g) })}</p>
      ${commerce(g) ? `<p class="section-title">${t('pas.waiting')}</p>${cands || `<p class="empty">${t('pas.none')}</p>`}` : `<p class="hint">${t('pas.noneHere')}</p>`}
      <p class="section-title">${t('pas.riders')}</p>${riders || `<p class="empty">${t('pas.none')}</p>`}`;
  };

  const plantingPanel = (g: Game, car: Car) => {
    const plots = (car.plots ?? [])
      .map((p, i) => {
        const state = p.ripe ? t('plant.ripe') : p.growing ? t('plant.growing', { p: p.progress, w: PLANT.work }) : t('plant.empty');
        const action = p.ripe
          ? `<button type="button" class="btn go" data-act="harvest" data-id="${car.uid}" data-index="${i}">${t('plant.harvest')}</button>`
          : !p.growing
            ? `<button type="button" class="btn teal" data-act="plant" data-id="${car.uid}" data-index="${i}" ${count(g, 'seed') >= PLANT.seed && count(g, 'water') >= PLANT.water ? '' : 'disabled'}>${t('plant.sow')}</button>`
            : `<span class="bar"><i style="width:${(p.progress / PLANT.work) * 100}%"></i></span>`;
        return `<div class="line"><b>${t('plant.slot', { n: i + 1 })}</b><span class="muted">${state}</span>${action}</div>`;
      })
      .join('');
    return `<div class="card"><div class="card-head"><span class="who">${carName(g, car.uid)}</span><span class="tag">Lv${car.level + 1}</span></div>${plots}
      <p class="hint">${t('plant.recipe', { seed: PLANT.seed, water: PLANT.water, out: PLANT.out, work: PLANT.work })}</p>
      <p class="hint">${t('plant.stock', { seed: count(g, 'seed'), water: count(g, 'water') })}</p></div>`;
  };

  const processingPanel = (g: Game, car: Car) => {
    const devices = (car.devices ?? [])
      .map((d) => {
        const recipe = d.job ? RECIPES.find((r) => r.id === d.job!.recipe)! : undefined;
        const status = d.out
          ? `<span class="tag good">${t('proc.ready', { item: itemName(d.out.item), n: d.out.qty })}</span><button type="button" class="btn go" data-act="collect" data-id="${d.uid}">${t('proc.collect')}</button>`
          : recipe
            ? `<span class="muted">${t('proc.working', { item: itemName(recipe.out), p: d.job!.progress, w: recipe.work })}</span>`
            : RECIPES.filter((r) => r.device === d.kind)
                .map((r) => {
                  const ok = r.inputs.every(([item, n]) => count(g, item) >= n);
                  return `<button type="button" class="btn teal" data-act="job" data-id="${d.uid}" data-recipe="${r.id}" ${ok ? '' : 'disabled'}>${t('proc.recipe', { in: r.inputs.map(([item, n]) => `${n} ${itemName(item)}`).join(' + '), out: `${r.qty} ${itemName(r.out)}` })}</button>`;
                })
                .join('');
        return `<div class="card"><div class="card-head"><span class="who">${t(`device.${d.kind}`)}</span><span class="tag">${t('proc.upkeep', { n: DEVICES[d.kind].upkeep })}</span></div><div class="line">${status}</div></div>`;
      })
      .join('');
    const free = carLevel(car).cap - slotsUsed(car);
    const install = DEVICE_KINDS.map(
      (k) => `<button type="button" class="btn" data-act="install" data-id="${car.uid}" data-kind="${k}" ${free >= DEVICES[k].slots && g.cash >= DEVICES[k].cash ? '' : 'disabled'}>${t('proc.install', { name: t(`device.${k}`), price: DEVICES[k].cash, slots: DEVICES[k].slots })}</button>`,
    ).join('');
    return `<div class="card"><div class="card-head"><span class="who">${carName(g, car.uid)}</span><span class="tag">${t('proc.slots', { used: slotsUsed(car), cap: carLevel(car).cap })}</span></div><div class="actions wrap">${install}</div><p class="hint">${t('proc.hint')}</p></div>${devices}`;
  };

  const depotPanel = (g: Game) => {
    const here = commerce(g) && SERVICES[g.station.kind as 'rich' | 'slum'].depot;
    const lines: string[] = [];
    if (g.exp) {
      lines.push(`<div class="card"><div class="card-head"><span class="who">${t('rep.title')}</span></div>${(['rich', 'slum'] as const)
        .map((r) => `<div class="line">${regionBadge(r)}<span class="rep ${g.rep[r] < 0 ? 'neg' : ''}">${g.rep[r] >= 0 ? '+' : ''}${Math.round(g.rep[r] * 10) / 10}</span><span class="muted">${t('rep.cheats', { n: g.cheats[r] })}${g.audit[r] ? ` · ${t('rep.audit')}` : ''}</span></div>`)
        .join('')}<p class="hint">${t('rep.hint')}</p></div>`);
    }
    if (!here) lines.push(`<p class="hint warn">${t('depot.noService')}</p>`);
    lines.push(`<p class="section-title">${t('depot.cars', { n: g.cars.length })}</p>`);
    for (const kind of CAR_KINDS) {
      const lv = CARS[kind][0];
      lines.push(`<div class="card"><div class="card-head"><span class="who">${t(`car.${kind}`)}</span><span class="tag">${t('depot.owned', { n: g.cars.filter((c) => c.kind === kind).length })}</span></div>
        <span class="muted">${t(`depot.${kind}Desc`)}</span>
        <div class="actions"><button type="button" class="btn go" data-act="buy-car" data-kind="${kind}" ${!here || g.cash < lv.cash || g.cars.length >= RULES.maxCars || g.phase === 'settlement' ? 'disabled' : ''}>${t('depot.buy', { price: lv.cash })}</button></div></div>`);
    }
    if (g.cars.length) {
      lines.push(`<p class="section-title">${t('depot.owned2')}</p>`);
      for (const car of g.cars) {
        const nextLv = car.level < 2 ? CARS[car.kind][car.level + 1]! : undefined;
        lines.push(`<div class="card"><div class="card-head"><span class="who">${carName(g, car.uid)}</span><span class="tag">Lv${car.level + 1}</span></div>
          <div class="actions">${nextLv ? `<button type="button" class="btn teal" data-act="upgrade-car" data-id="${car.uid}" ${here ? '' : 'disabled'}>${t('depot.upgrade', { cash: nextLv.cash, ingot: nextLv.ingot, parts: nextLv.parts })}</button>` : `<span class="tag">${t('depot.maxLevel')}</span>`}
          <button type="button" class="btn danger" data-act="sell-car" data-id="${car.uid}" ${here ? '' : 'disabled'}>${t('depot.sell', { amount: Math.floor(car.paid * RULES.carRefund) })}</button></div></div>`);
      }
    }
    const steps = [100, 300, 600].filter((a) => a < g.debt);
    lines.push(`<div class="card"><div class="card-head"><span class="who">${t('depot.repay')}</span><span class="tag">${t('hud.debt')} ${g.debt}</span></div><span class="muted">${t('depot.repayDesc')}</span>
      <div class="actions">${steps.map((a) => `<button type="button" class="btn" data-act="repay" data-amount="${a}" ${g.cash < a || g.phase === 'settlement' ? 'disabled' : ''}>${t('depot.repayBtn', { amount: a })}</button>`).join('')}
      <button type="button" class="btn danger" data-act="repay" data-amount="${g.debt}" ${g.cash < g.debt || g.debt <= 0 || g.phase === 'settlement' ? 'disabled' : ''}>${t('depot.repayAll', { amount: g.debt })}</button></div></div>`);
    return `${g.cash < 0 ? `<p class="hint warn">${t('depot.negative')}</p>` : ''}${lines.join('')}`;
  };

  const cargoView = (g: Game) => {
    const grids = g.holds
      .map((hold) => {
        const cells = hold.stacks
          .map((s) => {
            const [w, h] = dims(s.item, s.rot);
            const fake = g.exp && (s.label ?? 1) > (s.real ?? 1);
            return `<div class="stk ${selStack === s.id ? 'sel' : ''} ${fake ? 'fake' : ''}" data-stack="${s.id}" style="left:${s.x * CELL}px;top:${s.y * CELL}px;width:${w * CELL - 2}px;height:${h * CELL - 2}px;background:${CHIP[s.item]}"><span>${itemName(s.item)}</span><b>${s.item === 'ammo' ? s.qty : s.qty > 1 ? s.qty : ''}</b>${g.exp && s.label !== undefined ? `<em class="g${s.label}"></em>` : ''}</div>`;
          })
          .join('');
        const used = hold.stacks.reduce((sum, s) => sum + ITEMS[s.item].w * ITEMS[s.item].h, 0);
        return `<div class="hold"><div class="hold-head"><b>${carName(g, hold.id)}</b><span class="muted">${t('cargo.cells', { used, cap: hold.w * hold.h })}</span></div>
          <div class="hold-grid" data-hold-grid="${hold.id}" style="width:${hold.w * CELL}px;height:${hold.h * CELL}px;background-size:${CELL}px ${CELL}px">${cells}</div></div>`;
      })
      .join('');
    const sel = selStack !== undefined ? g.holds.flatMap((h) => h.stacks).find((s) => s.id === selStack) : undefined;
    const detail = sel
      ? `<div class="card"><div class="card-head">${itemLabel(sel.item)}${gradeTag(g, sel.label, sel.real, true)}<span class="tag">${qtyText(sel.item, sel.qty)}</span></div>
        <div class="line"><span class="muted">${t('cargo.detail', { w: ITEMS[sel.item].w, h: ITEMS[sel.item].h, stack: ITEMS[sel.item].stack, load: Math.round(sel.qty * ITEMS[sel.item].load * 100) / 100, value: Math.round((sel.qty / unitOf(sel.item)) * ITEMS[sel.item].value) })}</span></div>
        <div class="actions wrap"><button type="button" class="btn" data-act="rotate">${t('cargo.rotate')}</button><button type="button" class="btn" data-act="split" ${sel.qty > 1 ? '' : 'disabled'}>${t('cargo.split')}</button>
        ${g.exp && ITEMS[sel.item].graded && (sel.label ?? 1) < 1 ? `<button type="button" class="btn teal" data-act="relabel">${t('cargo.relabel')}</button>` : ''}
        <button type="button" class="btn danger" data-act="discard">${t('cargo.discard')}</button></div></div>`
      : `<p class="hint">${t('cargo.selectHint')}</p>`;
    const pending = g.pending.length
      ? `<div class="card pending"><div class="card-head"><span class="who">${t('cargo.pending')}</span><span class="tag bad">${t('cargo.pendingBlock')}</span></div>${g.pending
          .map((l, i) => `<div class="line">${itemLabel(l.item)}<span class="muted">${qtyText(l.item, l.qty)}</span><button type="button" class="btn small go" data-act="place-pending" data-index="${i}">${t('cargo.place')}</button><button type="button" class="btn small danger" data-act="drop-pending" data-index="${i}">${t('cargo.drop')}</button></div>`)
          .join('')}</div>`
      : '';
    const exp = g.exp
      ? `<div class="card"><div class="card-head"><span class="who">${t('cargo.shady')}</span></div><p class="hint">${t('cargo.shadyHint')}</p>
        <div class="actions"><button type="button" class="btn teal" data-act="dilute" ${count(g, 'water') && count(g, 'sewage') ? '' : 'disabled'}>${t('cargo.dilute')}</button></div></div>`
      : '';
    return `<div class="cargo-head"><b>${t('cargo.title')}</b><span class="muted">${t('cargo.load', { q: load(g) })} · ${t('cargo.fuel', { f: Math.round(segmentFuel(g) * 100) / 100 })} · ${t('cargo.value', { v: Math.round(inventoryValue(g)) })}</span>
      <button type="button" class="btn small" data-act="tidy">${t('cargo.tidy')}</button><button type="button" class="btn small" data-act="tab" data-tab="car">${t('cargo.close')}</button></div>
      <p class="hint">${t('cargo.dragHint')}</p>
      <div class="cargo-body"><div class="holds">${grids}</div><div class="cargo-side">${pending}${detail}${exp}</div></div>`;
  };

  const renderStrip = (g: Game) => {
    const badge = (n: number) => (n > 0 ? `<span class="count">${n}</span>` : '');
    const open = (car: number) => g.station.customers.filter((c) => c.car === car && !c.outcome).length;
    const items = [
      `<button type="button" class="car-btn loco ${selCar === 0 && tab === 'car' ? 'active' : ''}" data-act="car" data-id="0"><b>${t('car.loco')}</b><small>${t('car.locoSub')}</small>${badge(open(0) + (g.station.site && !g.station.site.closed ? 1 : 0))}</button>`,
      ...g.cars.map((car) => {
        let n = 0;
        if (car.kind === 'storage') n = open(car.uid);
        if (car.kind === 'passenger') n = g.station.passengers.filter((p) => !p.status && p.dest !== undefined).length + g.riders.filter((r) => r.want && !r.want.sold).length;
        if (car.kind === 'planting') n = car.plots!.filter((p) => p.ripe || !p.growing).length;
        if (car.kind === 'processing') n = car.devices!.filter((d) => d.out || !d.job).length;
        return `<button type="button" class="car-btn ${car.kind} ${selCar === car.uid && tab === 'car' ? 'active' : ''}" data-act="car" data-id="${car.uid}"><b>${carName(g, car.uid)}</b><small>Lv${car.level + 1}</small>${badge(n)}</button>`;
      }),
    ];
    if (!g.cars.some((c) => c.kind === 'passenger')) {
      const n = g.station.passengers.filter((p) => !p.status && p.dest !== undefined).length + g.riders.filter((r) => r.want && !r.want.sold).length;
      items.splice(1, 0, `<button type="button" class="car-btn seats ${selCar === -1 && tab === 'car' ? 'active' : ''}" data-act="car" data-id="-1"><b>${t('car.headSeats')}</b><small>${t('pas.seats', { used: g.riders.length, total: seats(g) })}</small>${badge(n)}</button>`);
    }
    part('train-strip').innerHTML = items.join('');
  };

  const renderPanel = (g: Game) => {
    const tabs: [Tab, string][] = [['car', t('tab.car')], ['cargo', t('tab.cargo')], ['depot', t('tab.depot')]];
    part('tabs').innerHTML = tabs
      .map(([key, label]) => `<button type="button" data-act="tab" data-tab="${key}" class="${tab === key ? 'active' : ''}">${label}${key === 'cargo' && g.pending.length ? `<span class="count">${g.pending.length}</span>` : ''}</button>`)
      .join('');
    const body = part('tab-body');
    const cargo = part('cargo-view');
    flag('cargo-open', tab === 'cargo');
    if (tab === 'cargo') {
      cargo.innerHTML = cargoView(g);
      body.innerHTML = `<p class="hint">${t('cargo.openHint')}</p>`;
      return;
    }
    cargo.innerHTML = '';
    if (tab === 'depot') {
      body.innerHTML = depotPanel(g);
      return;
    }
    if (selCar === 0) {
      body.innerHTML = `<p class="section-title">${t('car.loco')}</p>${locoPanel(g)}`;
      return;
    }
    if (selCar === -1) {
      body.innerHTML = passengerPanel(g);
      return;
    }
    const car = findCar(g, selCar);
    if (!car) {
      selCar = 0;
      body.innerHTML = locoPanel(g);
      return;
    }
    if (car.kind === 'storage')
      body.innerHTML = `<div class="card"><div class="card-head"><span class="who">${carName(g, car.uid)}</span><span class="tag">Lv${car.level + 1} · +${carLevel(car).customers}</span></div>${miniGrid(g, car.uid)}<p class="hint">${t('car.storageHint')}</p></div>
        <p class="section-title">${t('car.receiving', { n: g.station.customers.filter((c) => c.car === car.uid).length })}</p>${commerce(g) ? customersOf(g, car.uid) : `<p class="hint">${t('car.noTradeHere')}</p>`}`;
    else if (car.kind === 'passenger') body.innerHTML = passengerPanel(g);
    else if (car.kind === 'planting') body.innerHTML = plantingPanel(g, car);
    else body.innerHTML = processingPanel(g, car);
  };

  const renderTop = (g: Game) => {
    part('tb-day').textContent = t('hud.day', { day: g.day });
    part('tb-phase').textContent = t(`phase.${travelling ? 'travel' : g.phase}`);
    const cash = part('tb-cash');
    cash.textContent = money(g.cash);
    cash.classList.toggle('neg', g.cash < 0);
    part('tb-debt').textContent = String(g.debt);
    const due = debtPreview(g);
    const dueLabel = part('tb-due-label');
    const dueValue = part('tb-due');
    if (!due || g.debt === 0) {
      dueLabel.textContent = t('hud.dueNone');
      dueValue.textContent = '—';
      dueValue.classList.add('ok');
    } else {
      dueLabel.textContent = t('hud.due', { day: due.day });
      dueValue.textContent = due.due > 0 ? t('hud.dueNeed', { amount: due.due }) : t('hud.dueOk');
      dueValue.classList.toggle('ok', due.due === 0);
    }
    part('tb-quota').textContent = t('hud.quota', { left: nodesLeft(g), nodes: quota(g).nodes });
  };

  const renderPlate = (g: Game) => {
    const route = g.route;
    const dots = route
      ? `<span class="dot past"></span>${route.nodes.map((n, i) => `<span class="dot ${n.kind} ${i + 1 === route.arrived ? 'here' : i + 1 < route.arrived ? 'past' : ''}" title="${kindName(n.kind)}"></span>`).join('')}<span>${route.arrived === 0 ? t('plate.origin') : t('plate.stop', { k: route.arrived, n: route.nodes.length })}</span>`
      : `<span>${t('plate.noRoute')}</span>`;
    part('station-plate').innerHTML = `<div>${regionBadge(g.station.kind)}<span class="place">${stationName()}</span></div><div class="route-dots">${dots}</div>`;
    const stats: [string, string][] = [
      ['seats', `${g.riders.length}/${seats(g)}`],
      ['load', String(load(g))],
      ['fuel', `≈${Math.ceil(g.today.fuelRaw)}`],
      ['hp', String(g.hp)],
      ['customers', String(storageCustomers(g) + 3)],
    ];
    part('train-stats').innerHTML = stats.map(([key, value]) => `<span>${t(`stats.${key}`)} <b>${value}</b></span>`).join('');
    part('travel-label').textContent = t('travel.to', { place: stationName() });
  };

  const renderAction = (g: Game) => {
    const box = part('main-action');
    if (settleSelling && g.phase === 'settlement') {
      box.innerHTML = `<button type="button" class="big" data-act="settle-back">${t('settle.back')}</button>`;
      return;
    }
    if (g.phase !== 'station' || travelling) {
      box.innerHTML = '';
      return;
    }
    const buttons: string[] = [];
    if (routeActive(g)) {
      const nextNode = g.route!.nodes[g.route!.arrived]!;
      const blocked = g.pending.length > 0 || nodesLeft(g) <= 0;
      buttons.push(`<button type="button" class="big" data-act="depart" ${blocked ? 'disabled' : ''}>${t('action.depart', { place: kindName(nextNode.kind) })}</button>`);
      if (nodesLeft(g) <= 0) buttons.push(`<small class="warn">${t('action.quotaOut')}</small>`);
    } else if (wantsRoute(g)) buttons.push(`<button type="button" class="big" data-act="open-routes">${t('action.chooseRoute')}</button>`);
    if (commerce(g)) buttons.push(`<button type="button" class="${routeActive(g) || wantsRoute(g) ? 'btn' : 'big'}" data-act="end-day" ${canEndDay(g) ? '' : 'disabled'}>${t(routeActive(g) && g.route!.arrived > 0 ? 'action.earlyStop' : 'action.endDay')}</button>`);
    else if (!routeActive(g)) buttons.push(`<small class="warn">${t('action.noStopHere')}</small>`);
    if (g.pending.length) buttons.push(`<small class="warn">${t('action.pending')}</small>`);
    box.innerHTML = buttons.join('');
  };

  const renderModal = (g: Game): boolean => {
    const modal = part('modal');
    if (summary) {
      const s = summary;
      modal.innerHTML = `<h2 class="${s.outcome === 'extract' ? '' : 'bad'}">${t(`sum.${s.outcome}`)}</h2>
        <p>${t('sum.stats', { kills: s.s.kills, shots: s.s.shots, taken: s.s.taken, m: Math.floor(s.s.seconds / 60), s: s.s.seconds % 60 })}</p>
        ${s.note ? `<p class="${s.note === 'lost-bag' ? 'warn' : 'muted'}">${t(`sum.${s.note}`)}</p>` : ''}
        ${g.pending.length ? `<p class="warn">${t('sum.pending', { n: g.pending.length })}</p>` : ''}
        <p class="muted">${t('sum.hp', { hp: g.hp })}</p>
        <div class="actions"><button type="button" class="big" data-act="close-summary">${t('sum.ok')}</button></div>`;
      return true;
    }
    if (g.phase === 'station' && wantsRoute(g) && !routeDeferred && !travelling && !settleSelling) {
      const opts = g.routeOptions!;
      const cards = opts
        .map((nodes, i) => {
          const commerceStops = nodes.filter((n) => isCommerce(n.kind)).length;
          const fits = nodes.length <= nodesLeft(g);
          return `<div class="route-card ${fits ? '' : 'off'}">
            <div class="route-nodes">${nodes.map((n) => `<span class="region ${n.kind}">${kindName(n.kind)}${n.theme && n.kind === 'resource' ? `·${t(`theme.${n.theme}`)}` : ''}</span>`).join('<i>→</i>')}</div>
            <div class="route-meta"><span>${t('route.stops', { n: nodes.length })}</span><span>${t('route.customers', { n: commerceStops * (3 + storageCustomers(g)) })}</span><span>${t('route.cost', { f: routeCost(g, nodes.length) })}</span></div>
            <button type="button" class="btn go" data-act="choose-route" data-index="${i}" ${fits ? '' : 'disabled'}>${t('route.choose')}</button></div>`;
        })
        .join('');
      modal.innerHTML = `<h2>${t('route.title', { day: g.day })}</h2><p class="muted">${t('route.desc', { left: nodesLeft(g), lines: quota(g).lines - g.linesToday })}</p><div class="routes">${cards}</div>
        <div class="actions"><button type="button" class="btn" data-act="defer-routes">${t('route.later')}</button></div>`;
      return true;
    }
    if (g.phase === 'dayEnd' && g.bill) {
      const b = g.bill;
      const L = b.ledger;
      modal.innerHTML = `<h2>${t('bill.title', { day: b.day })}</h2>
        <div class="bill"><div><h4>${t('bill.income')}</h4>
          <p>${t('bill.retail')}<b>+${L.retail}</b></p><p>${t('bill.tickets')}<b>+${L.tickets}</b></p>
          <p>${t('bill.purchases')}<b>−${L.purchases}</b></p><p>${t('bill.invest')}<b>${L.invest >= 0 ? '−' : '+'}${Math.abs(L.invest)}</b></p>${L.repaid ? `<p>${t('bill.repaid')}<b>−${L.repaid}</b></p>` : ''}${L.fines ? `<p>${t('bill.fines')}<b>−${L.fines}</b></p>` : ''}</div>
        <div><h4>${t('bill.total')}</h4>
          <p>${t('bill.fuel', { seg: b.segments })}<b>−${b.fuel}</b></p><p>${t('bill.wages', { seg: b.segments })}<b>−${b.wages}</b></p>
          <p>${t('bill.living', { n: b.heads.normal, noble: b.heads.noble })}<b>−${b.living}</b></p>${b.upkeep ? `<p>${t('bill.upkeep')}<b>−${b.upkeep}</b></p>` : ''}
          ${b.refunds ? `<p class="warn">${t('bill.refunds', { n: b.overdue })}<b>−${b.refunds}</b></p>` : ''}
          <p class="sum">${t('bill.sum')}<b>−${b.total}</b></p></div></div>
        <p class="cashline">${t('bill.cash', { before: money(b.cashBefore), after: money(b.cashAfter) })}</p>
        ${b.cashAfter < 0 ? `<p class="warn">${t('bill.negative')}</p>` : ''}
        ${routeActive(g) ? `<p class="muted">${t('bill.continueRoute')}</p>` : ''}
        <div class="actions"><button type="button" class="big" data-act="close-bill">${t(isDueDayNext(g) ? 'bill.toSettle' : 'bill.next', { day: g.day + 1 })}</button></div>`;
      return true;
    }
    if (g.phase === 'settlement' && g.settlement && !settleSelling) {
      const due = dueAmount(g, g.settlement.required);
      const short = Math.max(0, due - g.cash, -g.cash);
      const sellable = g.station.customers.some((c) => c.kind === 'buyer' && !c.outcome && previewSale(g, c).total > 0);
      modal.innerHTML = `<h2>${t('settle.title', { day: g.settlement.day })}</h2>
        <p>${t('settle.desc', { required: g.settlement.required, repaid: g.repaid, due })}</p>
        <p class="cashline">${t('settle.cash', { cash: money(g.cash) })}</p>
        ${short > 0 ? `<p class="warn">${t('settle.short', { amount: short })}</p>` : `<p>${t('settle.ok')}</p>`}
        <div class="actions">
          <button type="button" class="big" data-act="settle-pay" ${short > 0 ? 'disabled' : ''}>${t('settle.pay', { amount: due })}</button>
          ${short > 0 ? `<button type="button" class="btn teal" data-act="settle-sell" ${sellable ? '' : 'disabled'}>${t('settle.sell')}</button><button type="button" class="btn danger" data-act="forfeit">${t('settle.forfeit')}</button>` : ''}
        </div>`;
      return true;
    }
    if (g.phase === 'won') {
      modal.innerHTML = `<h2 class="good">${t('won.title')}</h2><p>${t('won.desc', { day: g.day })}</p><p class="muted">${t('won.stats', { revenue: g.stats.revenue, tickets: g.stats.tickets, passengers: g.stats.passengers, sorties: g.stats.sorties })}</p>
        <div class="actions"><button type="button" class="big" data-act="win-continue">${t('won.continue')}</button><button type="button" class="btn" data-act="to-title">${t('won.title2')}</button></div>`;
      return true;
    }
    if (g.phase === 'lost') {
      modal.innerHTML = `<h2 class="bad">${t('lost.title')}</h2><p>${t('lost.desc', { day: g.day, repaid: g.repaid })}</p><p class="muted">${t('lost.stats', { revenue: g.stats.revenue, tickets: g.stats.tickets, sorties: g.stats.sorties })}</p>
        <div class="actions"><button type="button" class="btn" data-act="to-title">${t('lost.title2')}</button><button type="button" class="big" data-act="retry">${t('lost.retry')}</button></div>`;
      return true;
    }
    return false;
  };
  const isDueDayNext = (g: Game) => g.day % RULES.dueEvery === 0 && g.day / RULES.dueEvery <= RULES.dueCumulative.length && !(g.debt === 0 && g.cash >= 0);

  const renderBanner = (g: Game) => {
    const banner = part('settle-banner');
    if (!(settleSelling && g.phase === 'settlement' && g.settlement)) {
      banner.innerHTML = '';
      return;
    }
    const due = dueAmount(g, g.settlement.required);
    const short = Math.max(0, due - g.cash, -g.cash);
    banner.innerHTML = `<b>${short > 0 ? t('settle.banner', { amount: short }) : t('settle.bannerOk', { amount: due })}</b>
      <div class="actions"><button type="button" class="btn go" data-act="settle-pay" ${short > 0 ? 'disabled' : ''}>${t('settle.pay', { amount: due })}</button><button type="button" class="btn danger" data-act="forfeit">${t('settle.forfeit')}</button></div>`;
  };

  function render() {
    const g = game;
    flag('at-title', !g);
    flag('in-game', !!g);
    flag('travelling', travelling);
    flag('settle-selling', !!g && settleSelling && g.phase === 'settlement');
    for (const button of shadow.querySelectorAll<HTMLButtonElement>('[data-act="lang"]')) {
      button.classList.toggle('active', button.dataset.lang === (i18n.resolvedLanguage ?? i18n.language));
    }
    if (!g) {
      flag('modal-open', false);
      flag('cargo-open', false);
      renderTitle();
      mirror();
      return;
    }
    renderTop(g);
    renderStrip(g);
    renderPanel(g);
    renderPlate(g);
    renderAction(g);
    renderBanner(g);
    flag('modal-open', renderModal(g));
    mirror();
  }

  const localize = () => {
    for (const node of shadow.querySelectorAll<HTMLElement>('[data-i18n]')) node.textContent = t(node.dataset.i18n!);
    const lang = (i18n.resolvedLanguage ?? i18n.language ?? 'en') as GameLanguage;
    ui.host.lang = lang === 'zh' ? 'zh-CN' : 'en';
    ui.host.dataset.lang = lang;
    document.title = t('game.title');
    document.documentElement.lang = ui.host.lang;
    render();
  };
  const unbind = bindUiLocalization(ui, i18n, localize);

  /** QA read projection (?qa=1): state mirrored on the window for scripted acceptance. */
  function mirror() {
    if (!qa) return;
    (view as unknown as { __doomsday?: unknown }).__doomsday = {
      game: game ? JSON.parse(JSON.stringify(game)) : null,
      travelling,
      tab,
      selCar,
      settleSelling,
      summary: summary ?? null,
      lang: i18n.resolvedLanguage ?? i18n.language,
      engine,
      explore: exploreState,
    };
  }
  if (qa) view.setInterval(mirror, 250);

  localize();
  postView();
  const hello = view.setInterval(() => {
    if (engine?.ready) view.clearInterval(hello);
    postView();
  }, 500);

  return {
    dispose() {
      view.clearInterval(hello);
      if (travelTimer) view.clearTimeout(travelTimer);
      explore?.dispose();
      shadow.removeEventListener('click', onClick);
      shadow.removeEventListener('pointerdown', onPointerDown as EventListener);
      shadow.removeEventListener('contextmenu', onContext as EventListener);
      view.removeEventListener('pointermove', onPointerMove);
      view.removeEventListener('pointerup', onPointerUp);
      view.removeEventListener('keydown', onKey);
      port?.removeEventListener('message', onPort);
      unbind();
      ui.dispose();
    },
  };
}
