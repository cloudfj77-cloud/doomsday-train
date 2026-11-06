/**
 * Frontend root: owns the economy (assets/sim), every player-visible word, the
 * autosave and the HUD. It tells the Engine what to stage over the port.
 */
import type { GameHost } from '@forgeax/engine/app';
import { mountUi, type UiAsset } from '@forgeax/engine/ui';
import { bindUiLocalization, createUiLocalization } from '@forgeax/engine/ui/localization';
import { CARS, ITEM_IDS, ITEMS, RULES, type CarKind, type ItemId, type Region } from '../sim/data.ts';
import {
  arrive,
  boardPassenger,
  buyBatch,
  buyCar,
  buySingle,
  capacity,
  cellsFor,
  cellsUsed,
  chooseRoute,
  closeBill,
  continueAfterWin,
  debtPreview,
  depart,
  dueAmount,
  endDay,
  forfeit,
  inventoryValue,
  isGame,
  load,
  newGame,
  passCustomer,
  paySettlement,
  previewSale,
  repay,
  retailPrice,
  routeDone,
  seats,
  segmentFuel,
  sellToBuyer,
  sellToRider,
  storageBonus,
  type ActionResult,
  type Customer,
  type Game,
} from '../sim/economy.ts';
import { isEngineMessage, VIEW_CHANNEL, type EngineMessage, type ViewMessage } from '../shared/protocol.ts';
import { initialLanguage, LANG_KEY, type GameLanguage } from './language.ts';

type Host = Pick<GameHost, 'uiRoot' | 'port' | 'presentationPort' | 'setPointerLockAllowed'>;
type Tab = 'customers' | 'passengers' | 'depot' | 'cargo';

export const SAVE_KEY = 'doomsday.save.v1';
export const TRAVEL_MS = 2600;

const CHIP: Readonly<Record<ItemId, string>> = {
  ration: '#c9a36a',
  water: '#6fb7d8',
  veg: '#7fbf5a',
  sewage: '#6b6a4a',
  scrap: '#8a7f78',
  ingot: '#b9b2a8',
  parts: '#d68a3a',
  repair: '#d9473a',
  seed: '#a0c060',
  research: '#9a7fd0',
  bandage: '#efe6d6',
};

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
  let tab: Tab = 'customers';
  let routeDeferred = false;
  let settleSelling = false;
  let travelling = false;
  let travelSerial = 0;
  let travelTimer: number | undefined;
  let travelStart = 0;
  let engine: EngineMessage | undefined;
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
      region: g?.station.region ?? 'rich',
      stationSeed: g?.station.id ?? 0,
      cars: g?.cars ?? [],
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
  const money = (n: number) => (n < 0 ? `−${Math.abs(n)}` : String(n));
  const itemName = (id: ItemId) => t(`item.${id}`);
  const chip = (id: ItemId) => `<span class="chip" style="background:${CHIP[id]}"></span>`;
  const itemLabel = (id: ItemId) => `<span class="item">${chip(id)}${itemName(id)}</span>`;
  const placeName = (region: Region, place: number) => t(`place.${region}.p${place}`);
  const stationName = () => (game ? placeName(game.station.region, game.station.place) : '');
  const regionBadge = (region: Region) => `<span class="region ${region}">${t(`region.${region}`)}</span>`;
  const routeCost = (stops: number) =>
    Math.ceil(stops * (RULES.fuelBase + RULES.fuelPerCar * (game?.cars.length ?? 0) + RULES.fuelPerLoad * (game ? load(game) : 1.5))) +
    stops * RULES.driverWage +
    RULES.livingPerHead * RULES.crew;

  const toast = (text: string, kind = '') => {
    const id = ++toastId;
    toasts.push({ id, text, kind });
    while (toasts.length > 4) toasts.shift();
    renderToasts();
    view.setTimeout(() => {
      const index = toasts.findIndex((x) => x.id === id);
      if (index >= 0) toasts.splice(index, 1);
      renderToasts();
    }, 2800);
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
  const startNew = () => {
    game = newGame((Date.now() ^ Math.floor(Math.random() * 1e9)) | 0);
    tab = 'customers';
    routeDeferred = false;
    settleSelling = false;
    picks.clear();
    save();
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
      if (game.today.delivered > 0 && lastDelivered !== game.today.delivered) {
        toast(t('toast.delivered', { n: game.today.delivered - lastDelivered }), '');
      }
      lastDelivered = game.today.delivered;
      tab = 'customers';
      render();
      postView();
    }, TRAVEL_MS);
  };
  let lastDelivered = 0;

  const handle = (act: string, el: HTMLElement) => {
    const id = Number(el.dataset.id ?? -1);
    const g = game;
    switch (act) {
      case 'new-game':
        startNew();
        return;
      case 'continue': {
        const loaded = loadSave();
        if (loaded && loaded.phase !== 'lost') {
          game = loaded;
          lastDelivered = loaded.today.delivered;
          routeDeferred = false;
          settleSelling = false;
          commit();
        }
        return;
      }
      case 'lang':
        void setLanguage(el.dataset.lang as GameLanguage);
        return;
      case 'to-title':
        if (travelTimer) view.clearTimeout(travelTimer);
        travelling = false;
        game = null;
        render();
        postView();
        return;
    }
    if (!g) return;
    switch (act) {
      case 'tab':
        tab = el.dataset.tab as Tab;
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
      case 'choose-route': {
        if (!report(chooseRoute(g, Number(el.dataset.index)))) return;
        const ev = g.event;
        toast(ev ? t(ev.kind === 'up' ? 'toast.eventUp' : 'toast.eventDown', { item: itemName(ev.item) }) : t('toast.eventNone'), ev ? 'money' : '');
        tab = g.station.passengers.some((p) => p.dest !== undefined) ? 'passengers' : 'customers';
        commit();
        return;
      }
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
        report(buySingle(g, id, picks.get(id) ?? c.goods.map((x) => x.qty)), 'toast.bought', 'money');
        commit();
        return;
      }
      case 'buy-batch':
        report(buyBatch(g, id), 'toast.bought', 'money');
        commit();
        return;
      case 'sell':
        report(sellToBuyer(g, id), 'toast.sold', 'money');
        commit();
        return;
      case 'pass':
        report(passCustomer(g, id));
        commit();
        return;
      case 'board':
        report(boardPassenger(g, id), 'toast.boarded', 'money');
        commit();
        return;
      case 'sell-rider':
        report(sellToRider(g, id), 'toast.riderSold', 'money');
        commit();
        return;
      case 'buy-car':
        report(buyCar(g, el.dataset.kind as CarKind), 'toast.car', 'money');
        commit();
        return;
      case 'repay':
        report(repay(g, Number(el.dataset.amount)), 'toast.repaid', 'money');
        commit();
        return;
      case 'depart':
        doDepart();
        return;
      case 'end-day':
        report(endDay(g));
        commit();
        return;
      case 'close-bill':
        report(closeBill(g));
        lastDelivered = 0;
        routeDeferred = false;
        settleSelling = false;
        tab = 'customers';
        picks.clear();
        commit();
        return;
      case 'settle-sell':
        settleSelling = true;
        tab = 'customers';
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
          settleSelling = false;
          lastDelivered = 0;
          routeDeferred = false;
          tab = 'customers';
          picks.clear();
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

  const setLanguage = async (lang: GameLanguage) => {
    storage?.setItem(LANG_KEY, lang);
    await i18n.changeLanguage(lang);
  };

  // --- Rendering -------------------------------------------------------------------------
  const renderTitle = () => {
    const saved = loadSave();
    const canContinue = !!saved && saved.phase !== 'lost';
    const button = part<HTMLButtonElement>('continue-btn');
    button.disabled = !canContinue;
    part('save-info').textContent = canContinue
      ? t('cover.saveInfo', { day: saved!.day, cash: money(saved!.cash), debt: saved!.debt })
      : t('cover.noSave');
  };

  const customerCard = (g: Game, c: Customer) => {
    const done = !!c.outcome;
    const settle = g.phase === 'settlement';
    if (c.kind === 'supplier') {
      const goods = c.goods ?? [];
      const q = picks.get(c.id) ?? goods.map((x) => x.qty);
      const total = goods.reduce((sum, x, i) => sum + x.unit * (q[i] ?? 0), 0);
      const lines = goods
        .map(
          (x, i) => `<div class="line">${itemLabel(x.item)}<span class="muted">${t('cust.unit', { price: x.unit })} · ${t('cust.retail', { price: retailPrice(x.item, g.station.event) })}</span>
          ${done ? `<span class="price">×${x.qty}</span>` : `<span class="stepper"><button type="button" data-act="step" data-id="${c.id}" data-row="${i}" data-delta="-1">−</button><span>${q[i]}/${x.qty}</span><button type="button" data-act="step" data-id="${c.id}" data-row="${i}" data-delta="1">+</button></span>`}</div>`,
        )
        .join('');
      const batchTag = c.discount !== undefined ? `<span class="tag good">${t('cust.batchTag', { pct: Math.round(c.discount * 100) })}</span>` : `<span class="tag">${t('cust.noBatch')}</span>`;
      const status = c.outcome === 'deal' ? `<span class="tag good">${t('cust.deal', { amount: money(c.amount ?? 0) })}</span>` : c.outcome === 'pass' ? `<span class="tag">${t('cust.passed')}</span>` : batchTag;
      const actions = done || settle
        ? ''
        : `<div class="actions">
            <button type="button" class="btn teal" data-act="buy-single" data-id="${c.id}" ${total <= 0 ? 'disabled' : ''}>${t('cust.buySingle', { price: total })}</button>
            ${c.batchPrice !== undefined ? `<button type="button" class="btn go" data-act="buy-batch" data-id="${c.id}">${t('cust.buyBatch', { price: c.batchPrice })}</button>` : ''}
            <button type="button" class="btn" data-act="pass" data-id="${c.id}">${t('cust.pass')}</button>
          </div>`;
      return `<div class="card ${done ? 'done' : ''}" data-customer="${c.id}" data-kind="supplier"><div class="card-head"><span class="who supplier">${t('cust.supplier')}</span>${status}</div>${lines}${actions}</div>`;
    }
    const sale = previewSale(g, c);
    const lines = (c.needs ?? [])
      .map((n) => `<div class="line">${itemLabel(n.item)}<span class="muted">×${n.qty} · ${t('cust.retail', { price: n.price })}</span><span class="tag ${g.inventory[n.item] > 0 ? 'good' : ''}">${t('cust.have', { n: g.inventory[n.item] })}</span></div>`)
      .join('');
    const status = c.outcome === 'deal' ? `<span class="tag good">${t('cust.deal', { amount: c.amount ?? 0 })}</span>` : c.outcome === 'pass' ? `<span class="tag">${t('cust.passed')}</span>` : `<span class="tag">${t('cust.budget', { budget: c.budget ?? 0 })}</span>`;
    const anyStock = (c.needs ?? []).some((n) => g.inventory[n.item] > 0);
    const sellLabel = sale.total > 0 ? t('cust.sell', { amount: sale.total }) : anyStock ? t('cust.cantAfford') : t('cust.cantSell');
    const actions = done
      ? ''
      : `<div class="actions"><button type="button" class="btn go" data-act="sell" data-id="${c.id}" ${sale.total > 0 ? '' : 'disabled'}>${sellLabel}</button>${settle ? '' : `<button type="button" class="btn" data-act="pass" data-id="${c.id}">${t('cust.pass')}</button>`}</div>`;
    return `<div class="card ${done ? 'done' : ''}" data-customer="${c.id}" data-kind="buyer"><div class="card-head"><span class="who buyer">${t('cust.buyer')}</span>${status}</div>${lines}${actions}</div>`;
  };

  const renderTabs = (g: Game) => {
    const open = g.station.customers.filter((c) => !c.outcome).length;
    const waiting = g.station.passengers.filter((p) => !p.boarded && p.dest !== undefined).length + g.riders.filter((r) => r.want && !r.want.sold).length;
    const tabs: [Tab, number][] = [
      ['customers', open],
      ['passengers', waiting],
      ['depot', 0],
      ['cargo', 0],
    ];
    part('tabs').innerHTML = tabs
      .map(([key, count]) => `<button type="button" data-act="tab" data-tab="${key}" class="${tab === key ? 'active' : ''}">${t(`tab.${key}`)}${count > 0 ? `<span class="count">${count}</span>` : ''}</button>`)
      .join('');
    const body = part('tab-body');
    if (tab === 'customers') {
      const list = g.phase === 'settlement' ? g.station.customers.filter((c) => c.kind === 'buyer') : g.station.customers;
      const cards = list.map((c) => customerCard(g, c)).join('');
      body.innerHTML = `<p class="hint">${g.phase === 'settlement' ? t('cust.settleHint') : t('cust.hint')}</p>${cards}${open === 0 ? `<p class="empty">${t('cust.allDone')}</p>` : ''}`;
    } else if (tab === 'passengers') {
      const route = g.route;
      const waitingCards = g.station.passengers
        .filter((p) => !p.boarded)
        .map((p) => {
          if (p.dest === undefined || !route) {
            const note = route && routeDone(g) ? t('pas.lastStop') : t('pas.unbound');
            return `<div class="card"><div class="card-head"><span class="who">${t('pas.waiting')}</span></div><span class="muted">${note}</span></div>`;
          }
          const destRegion = route.nodes[p.dest - 1]!;
          const seg = p.dest - (p.from ?? 0);
          return `<div class="card" data-passenger="${p.id}"><div class="card-head"><span class="who">${t('pas.to', { place: t(`region.${destRegion}`) + ` #${p.dest}`, seg })}</span><span class="tag good">${p.fare}</span></div>
            <div class="actions"><button type="button" class="btn go" data-act="board" data-id="${p.id}">${t('pas.board', { fare: p.fare })}</button></div></div>`;
        })
        .join('');
      const riders = g.riders
        .map((r) => {
          const want = r.want
            ? `<div class="line">${t('pas.want', { item: itemName(r.want.item), price: r.want.price })}<span class="tag ${g.inventory[r.want.item] > 0 ? 'good' : ''}">${t('cust.have', { n: g.inventory[r.want.item] })}</span>${r.want.sold ? `<span class="tag good">${t('pas.sold')}</span>` : `<button type="button" class="btn go" data-act="sell-rider" data-id="${r.id}" ${g.inventory[r.want.item] > 0 ? '' : 'disabled'}>${t('pas.sellRider', { price: r.want.price })}</button>`}</div>`
            : '';
          return `<div class="card" data-rider="${r.id}"><div class="card-head"><span class="who">${t('pas.riderTo', { n: r.dest })}</span><span class="tag">${r.fare}</span></div>${want}</div>`;
        })
        .join('');
      body.innerHTML = `<p class="hint">${t('pas.hint')}</p><p class="section-title">${t('pas.seats', { used: g.riders.length, total: seats(g) })} · ${t('pas.waiting')}</p>${waitingCards || `<p class="empty">${t('pas.none')}</p>`}<p class="section-title">${t('pas.riders')}</p>${riders || `<p class="empty">${t('pas.none')}</p>`}`;
    } else if (tab === 'depot') {
      const owned = (kind: CarKind) => g.cars.filter((c) => c === kind).length;
      const car = (kind: CarKind) => `<div class="card"><div class="card-head"><span class="who">${t(`depot.${kind}`)}</span><span class="tag">${t('depot.owned', { n: owned(kind) })}</span></div>
        <span class="muted">${t(`depot.${kind}Desc`)}</span>
        <div class="actions"><button type="button" class="btn go" data-act="buy-car" data-kind="${kind}" ${g.cash < CARS[kind].price || g.cars.length >= RULES.maxCars || g.phase === 'settlement' ? 'disabled' : ''}>${t('depot.buy', { price: CARS[kind].price })}</button></div></div>`;
      const steps = [100, 300, 600].filter((a) => a < g.debt);
      const repayCard = `<div class="card"><div class="card-head"><span class="who">${t('depot.repay')}</span><span class="tag">${t('hud.debt')} ${g.debt}</span></div><span class="muted">${t('depot.repayDesc')}</span>
        <div class="actions">${steps.map((a) => `<button type="button" class="btn" data-act="repay" data-amount="${a}" ${g.cash < a || g.phase === 'settlement' ? 'disabled' : ''}>${t('depot.repayBtn', { amount: a })}</button>`).join('')}
        <button type="button" class="btn danger" data-act="repay" data-amount="${g.debt}" ${g.cash < g.debt || g.debt <= 0 || g.phase === 'settlement' ? 'disabled' : ''}>${t('depot.repayAll', { amount: g.debt })}</button></div></div>`;
      body.innerHTML = `${g.cash < 0 ? `<p class="hint" style="color:#ff8a72">${t('depot.negative')}</p>` : ''}<p class="section-title">${t('depot.cars', { n: g.cars.length })}</p>${car('storage')}${car('passenger')}${repayCard}`;
    } else {
      const used = cellsUsed(g.inventory);
      const cap = capacity(g);
      const rows = ITEM_IDS.filter((id) => g.inventory[id] > 0)
        .map((id) => `<div class="line">${itemLabel(id)}<span class="muted">×${g.inventory[id]} · ${t('cargo.itemCells', { n: cellsFor(id, g.inventory[id]) })}</span><span class="price">${ITEMS[id].value}</span></div>`)
        .join('');
      const cells: string[] = [];
      for (const id of ITEM_IDS) for (let k = 0; k < cellsFor(id, g.inventory[id]); k += 1) cells.push(`<span class="cell" style="background:${CHIP[id]}"></span>`);
      while (cells.length < cap) cells.push('<span class="cell"></span>');
      body.innerHTML = `<p class="section-title">${t('cargo.cells', { used, cap })} · ${t('cargo.load', { q: load(g) })}</p>
        <div class="grid-cells">${cells.join('')}</div><p class="hint">${t('cargo.grid')}</p>
        ${rows ? `<div class="card">${rows}</div>` : `<p class="empty">${t('cargo.empty')}</p>`}
        <p class="hint">${t('cargo.value', { v: inventoryValue(g) })}</p>
        <p class="hint">${t('cargo.fuel', { f: segmentFuel(g) })} · ${t('cargo.formula')}</p>`;
    }
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
  };

  const renderPlate = (g: Game) => {
    const route = g.route;
    const dots = route
      ? `<span class="dot past"></span>${route.nodes.map((r, i) => `<span class="dot ${r} ${i + 1 === route.arrived ? 'here' : i + 1 < route.arrived ? 'past' : ''}"></span>`).join('')}<span>${route.arrived === 0 ? t('plate.origin') : t('plate.stop', { k: route.arrived, n: route.nodes.length })}</span>`
      : `<span>${t('plate.origin')}</span>`;
    part('station-plate').innerHTML = travelling
      ? `<div>${regionBadge(g.station.region)}<span class="place">${stationName()}</span></div><div class="route-dots">${dots}</div>`
      : `<div>${regionBadge(g.station.region)}<span class="place">${stationName()}</span></div><div class="route-dots">${dots}</div>`;
    const stats: [string, string][] = [
      ['cells', `${cellsUsed(g.inventory)}/${capacity(g)}`],
      ['seats', `${g.riders.length}/${seats(g)}`],
      ['load', String(load(g))],
      ['fuel', `≈${Math.ceil(g.today.fuelRaw)}`],
      ['cars', String(g.cars.length)],
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
    if (g.phase === 'depot' && !g.route) {
      box.innerHTML = `<button type="button" class="big" data-act="open-routes">${t('action.chooseRoute')}</button>`;
    } else if ((g.phase === 'depot' || g.phase === 'station') && g.route && !routeDone(g)) {
      const nextRegion = g.route.nodes[g.route.arrived]!;
      box.innerHTML = `<button type="button" class="big" data-act="depart" ${travelling ? 'disabled' : ''}>${t('action.depart', { place: t(`region.${nextRegion}`) })}</button>`;
    } else if (g.phase === 'station' && routeDone(g)) {
      box.innerHTML = `<button type="button" class="big alt" data-act="end-day" ${travelling ? 'disabled' : ''}>${t('action.endDay')}</button>`;
    } else box.innerHTML = '';
  };

  const renderModal = (g: Game): boolean => {
    const modal = part('modal');
    if (travelling) return false;
    if (g.phase === 'depot' && !g.route && !routeDeferred) {
      const options = g.routeOptions ?? [];
      const perStop = RULES.baseCustomers + storageBonus(g);
      modal.innerHTML = `<h2>${t('route.title', { day: g.day })}</h2><p>${t('route.desc')}</p>
        <div class="routes">${options
          .map(
            (o, i) => `<div class="route" data-route="${i}"><b>${t('route.stops', { n: o.nodes.length })}</b>
            <div class="nodes"><div class="node">${regionBadge(g.station.region)}<span class="muted">${stationName()}</span></div>${o.nodes.map((r, k) => `<div class="node">↓ ${regionBadge(r)}<span class="muted">#${k + 1}</span></div>`).join('')}</div>
            <span class="muted">${t('route.customers', { n: o.nodes.length * perStop })}</span>
            <span class="muted">${t('route.fuel', { f: routeCost(o.nodes.length) })}</span>
            <button type="button" class="btn go" data-act="choose-route" data-index="${i}">${t('route.choose')}</button></div>`,
          )
          .join('')}</div>
        <div class="actions"><button type="button" class="btn" data-act="defer-routes">${t('route.later')}</button></div>`;
      return true;
    }
    if (g.phase === 'dayEnd' && g.bill) {
      const b = g.bill;
      const l = b.ledger;
      const dueNext = b.day % RULES.dueEvery === 0 && b.day / RULES.dueEvery <= RULES.dueCumulative.length && !(g.debt === 0 && g.cash >= 0);
      modal.innerHTML = `<h2>${t('bill.title', { day: b.day })}</h2>
        <div class="bill">
          <span>${t('bill.retail')}</span><span class="plus">+${l.retail}</span>
          <span>${t('bill.tickets')}</span><span class="plus">+${l.tickets}</span>
          <span>${t('bill.purchases')}</span><span class="minus">−${l.purchases}</span>
          ${l.cars ? `<span>${t('bill.cars')}</span><span class="minus">−${l.cars}</span>` : ''}
          ${l.repaid ? `<span>${t('bill.repaid')}</span><span class="minus">−${l.repaid}</span>` : ''}
          <div class="sep"></div>
          <span>${t('bill.fuel', { seg: b.segments })}</span><span class="minus">−${b.fuel}</span>
          <span>${t('bill.wages', { seg: b.segments })}</span><span class="minus">−${b.wages}</span>
          <span>${t('bill.living', { n: b.living / RULES.livingPerHead })}</span><span class="minus">−${b.living}</span>
          <span class="total">${t('bill.total')}</span><span class="total">−${b.total}</span>
        </div>
        <p class="big-number ${b.cashAfter < 0 ? 'minus' : ''}">${t('bill.cash', { before: money(b.cashBefore), after: money(b.cashAfter) })}</p>
        ${b.cashAfter < 0 ? `<p class="minus">${t('bill.negative')}</p>` : ''}
        <div class="actions"><button type="button" class="big ${dueNext ? 'alt' : ''}" data-act="close-bill">${dueNext ? t('bill.toSettle') : t('bill.next', { day: b.day + 1 })}</button></div>`;
      return true;
    }
    if (g.phase === 'settlement' && g.settlement && !settleSelling) {
      const due = dueAmount(g, g.settlement.required);
      const short = Math.max(0, due - g.cash, -g.cash);
      modal.innerHTML = `<h2 class="${short > 0 ? 'bad' : ''}">${t('settle.title', { day: g.settlement.day })}</h2>
        <div class="bill">
          <span>${t('settle.required')}</span><span>${g.settlement.required}</span>
          <span>${t('settle.repaid')}</span><span>${g.repaid}</span>
          <span class="total">${t('settle.due')}</span><span class="total">${due}</span>
          <span>${t('settle.cash')}</span><span class="${g.cash < 0 ? 'minus' : ''}">${money(g.cash)}</span>
        </div>
        <p class="${short > 0 ? 'minus' : 'plus'}">${short > 0 ? t('settle.short', { amount: short }) : t('settle.ok')}</p>
        <div class="actions">
          ${short > 0 ? `<button type="button" class="btn danger" data-act="forfeit">${t('settle.forfeit')}</button><button type="button" class="btn teal" data-act="settle-sell">${t('settle.sell')}</button>` : ''}
          <button type="button" class="big" data-act="settle-pay" ${short > 0 ? 'disabled' : ''}>${t('settle.pay', { amount: due })}</button>
        </div>`;
      return true;
    }
    if (g.phase === 'won') {
      modal.innerHTML = `<h2 class="good">${t('won.title')}</h2><p>${t('won.desc', { day: g.day })}</p><p class="muted">${t('won.stats', { revenue: g.stats.revenue, tickets: g.stats.tickets, passengers: g.stats.passengers, cars: g.cars.length })}</p>
        <div class="actions"><button type="button" class="btn" data-act="to-title">${t('won.title2')}</button><button type="button" class="big" data-act="win-continue">${t('won.continue')}</button></div>`;
      return true;
    }
    if (g.phase === 'lost') {
      const due = g.settlement ? dueAmount(g, g.settlement.required) : 0;
      modal.innerHTML = `<h2 class="bad">${t('lost.title')}</h2><p>${t('lost.desc', { day: g.day, due, repaid: g.repaid })}</p><p class="muted">${t('lost.stats', { day: g.day, revenue: g.stats.revenue, tickets: g.stats.tickets })}</p>
        <div class="actions"><button type="button" class="btn" data-act="to-title">${t('lost.title2')}</button><button type="button" class="big" data-act="retry">${t('lost.retry')}</button></div>`;
      return true;
    }
    return false;
  };

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
      renderTitle();
      mirror();
      return;
    }
    renderTop(g);
    renderTabs(g);
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
      settleSelling,
      lang: i18n.resolvedLanguage ?? i18n.language,
      engine,
      cells: game ? cellsUsed(game.inventory) : 0,
      capacity: game ? capacity(game) : 0,
    };
  }

  localize();
  postView();
  // Re-announce the view once the Engine side has had time to subscribe.
  const hello = view.setInterval(() => {
    if (engine?.ready) view.clearInterval(hello);
    postView();
  }, 500);

  return {
    dispose() {
      view.clearInterval(hello);
      if (travelTimer) view.clearTimeout(travelTimer);
      shadow.removeEventListener('click', onClick);
      port?.removeEventListener('message', onPort);
      unbind();
      ui.dispose();
    },
  };
}
