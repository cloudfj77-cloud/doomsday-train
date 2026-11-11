/**
 * 探索现场（二期·轻量野外，〔08〕〔09〕）：俯视射击 + 搜刮 + 撤离。Canvas 2D 叠在 3D 场景上，
 * 跑在前端根（DOM），结束时把结果交还经营核心 endSortie()。数值全部来自 data.ts。
 */
import {
  BACKPACK,
  BOXES,
  ENEMIES,
  EXPLORE,
  ITEMS,
  RULES,
  TIER_REVEAL,
  WEAPONS,
  type EnemyKind,
  type ItemId,
} from '../sim/data.ts';
import { generateMap, isWall, lineOfSight, type Loot, type MapLayout, type MapState } from '../sim/explore-map.ts';
import { clone, placeLots, takeItems, type Hold, type Lot } from '../sim/grid.ts';
import type { LostBag, SortieInit, SortieResult } from '../sim/economy.ts';

export interface ExploreOptions {
  readonly root: HTMLElement;
  readonly init: SortieInit;
  readonly t: (key: string, o?: Record<string, unknown>) => string;
  readonly itemName: (id: ItemId) => string;
  readonly chip: Readonly<Record<ItemId, string>>;
  readonly onEnd: (result: SortieResult, summary: ExploreSummary) => void;
  /** QA：暴露现场状态 */
  readonly expose?: (state: unknown) => void;
}
export interface ExploreSummary {
  readonly kills: number;
  readonly seconds: number;
  readonly shots: number;
  readonly taken: number;
}

interface Enemy {
  readonly id: number;
  readonly kind: EnemyKind;
  x: number;
  y: number;
  hp: number;
  face: number;
  state: 'patrol' | 'investigate' | 'chase';
  target?: [number, number];
  patrolIdx: number;
  patrol: readonly (readonly [number, number])[];
  lastSeen: number;
  cd: number;
  windup: number;
  dash: number;
  dashDir: [number, number];
  dashHit: boolean;
  path: [number, number][];
  pathAt: number;
  hit: number;
}
interface Bullet {
  x: number;
  y: number;
  vx: number;
  vy: number;
  left: number;
  dmg: number;
  enemy: boolean;
}
type Focus =
  | { kind: 'box'; id: number }
  | { kind: 'ground'; id: number }
  | { kind: 'corpse'; id: number }
  | { kind: 'bag' }
  | { kind: 'extract' };

const PX = 22;
const R_PLAYER = 0.35;
const R_ENEMY = 0.4;
const ENEMY_COLOR: Readonly<Record<EnemyKind, string>> = { melee: '#b9533a', charger: '#d89a2a', ranged: '#8a5ad0' };
const TIER_COLOR = ['#d8d0c0', '#5aa8e8', '#f0b830'];

export function runExplore(o: ExploreOptions): { dispose(): void } {
  const { root, init, t } = o;
  const doc = root.ownerDocument;
  const view = doc.defaultView!;
  const map: MapLayout = generateMap(init.seed, init.area, init.theme, init.exp);
  const ms: MapState = clone(init.map);
  let lost: LostBag | undefined = init.lost ? clone(init.lost) : undefined;

  // --- DOM -------------------------------------------------------------------------------
  root.innerHTML = `
    <canvas class="ex-canvas"></canvas>
    <div class="ex-top">
      <div class="ex-hp"><small>${t('ex.hp')}</small><i class="bar"><b data-x="hp"></b></i><span data-x="hpv"></span></div>
      <div class="ex-st"><small>${t('ex.stamina')}</small><i class="bar"><b data-x="st"></b></i></div>
      <div class="ex-ammo"><small>${t('ex.ammo')}</small><span data-x="ammo"></span></div>
      <div class="ex-load"><small>${t('ex.load')}</small><span data-x="load"></span></div>
      <div class="ex-time"><small>${t('ex.time')}</small><span data-x="time"></span></div>
    </div>
    <div class="ex-prompt" data-x="prompt"></div>
    <div class="ex-progress" data-x="progress"><i data-x="progress-fill"></i><span data-x="progress-label"></span></div>
    <div class="ex-help">${t('ex.help')}</div>
    <div class="ex-side">
      <div class="ex-panel"><div class="ex-title">${t('ex.backpack')}</div><div class="ex-grid" data-x="pack"></div></div>
      <div class="ex-panel ex-loot" data-x="loot"></div>
    </div>
    <div class="ex-pause" data-x="pause">
      <div class="ex-pause-box">
        <h3>${t('ex.paused')}</h3>
        <button type="button" class="btn go" data-ex="resume">${t('ex.resume')}</button>
        <button type="button" class="btn danger" data-ex="abandon">${t('ex.abandon')}</button>
        <p class="hint">${t('ex.abandonHint')}</p>
      </div>
    </div>`;
  const $ = <T extends HTMLElement = HTMLElement>(k: string) => root.querySelector<T>(`[data-x="${k}"]`)!;
  const canvas = root.querySelector<HTMLCanvasElement>('.ex-canvas')!;
  const ctx = canvas.getContext('2d')!;

  // --- State -----------------------------------------------------------------------------
  const p = {
    x: map.entry[0],
    y: map.entry[1],
    hp: Math.max(1, init.hp),
    stamina: EXPLORE.stamina as number,
    staminaWait: 0,
    aim: 0,
    guard: 0,
    gun: init.gun,
    mag: 0,
    reserve: init.ammo,
    reload: 0,
    fireCd: 0,
    pushCd: 0,
    fired: false,
    shots: 0,
    kills: 0,
    taken: 0,
  };
  if (p.gun) {
    p.mag = Math.min(WEAPONS[p.gun].mag, p.reserve);
    p.reserve -= p.mag;
  }
  let pack: Hold = { id: -1, w: BACKPACK.w, h: BACKPACK.h, stacks: [] };
  const ids = { value: 1 };
  if (init.backpack.length) pack = placeLots([pack], init.backpack, ids)?.[0] ?? pack;
  const enemies: Enemy[] = map.enemies
    .filter((e) => !ms.dead.includes(e.id))
    .map((e) => ({
      id: e.id, kind: e.kind, x: e.x, y: e.y, hp: ENEMIES[e.kind].hp, face: Math.PI, state: 'patrol', patrolIdx: 0, patrol: e.patrol,
      lastSeen: 0, cd: 0, windup: 0, dash: 0, dashDir: [0, 0], dashHit: false, path: [], pathAt: 0, hit: 0,
    }));
  const bullets: Bullet[] = [];
  const keys = new Set<string>();
  const mouse = { x: 0, y: 0, down: false };
  let elapsed = 0;
  let paused = false;
  let ended = false;
  let focus: Focus | undefined;
  let open: Focus | undefined; // 已打开（显示内容）的搜点
  /** 当前读条：搜刮 / 撤离 / 治疗 */
  let task: { kind: 'search' | 'extract' | 'heal' | 'pick'; t: number; need: number; focus?: Focus } | undefined;
  let lootDirty = true;
  let packDirty = true;
  const noises: { x: number; y: number; r: number }[] = [];

  // --- Helpers ---------------------------------------------------------------------------
  const packLoad = () => pack.stacks.reduce((s, x) => s + x.qty * ITEMS[x.item].load, 0);
  const carry = () => (p.gun ? ITEMS[p.gun].load : 0) + (p.mag + p.reserve) * ITEMS.ammo.load + packLoad();
  const collide = (x: number, y: number, r: number) =>
    isWall(map, x - r, y - r) || isWall(map, x + r, y - r) || isWall(map, x - r, y + r) || isWall(map, x + r, y + r);
  const moveCircle = (o: { x: number; y: number }, dx: number, dy: number, r: number) => {
    if (!collide(o.x + dx, o.y, r)) o.x += dx;
    if (!collide(o.x, o.y + dy, r)) o.y += dy;
  };
  const noise = (r: number) => noises.push({ x: p.x, y: p.y, r });
  const box = (id: number) => map.boxes.find((b) => b.id === id)!;
  const revealedOf = (id: number) => ms.revealed[id] ?? 0;
  const lootOf = (f: Focus): readonly Loot[] => {
    if (f.kind === 'box') return box(f.id).loot.slice(0, revealedOf(f.id));
    if (f.kind === 'ground') return map.ground.find((g) => g.id === f.id)!.loot;
    if (f.kind === 'corpse') return ms.searched.includes(f.id) ? map.enemies.find((e) => e.id === f.id)!.corpse : [];
    return [];
  };
  const searchNeed = (f: Focus): number => {
    if (f.kind === 'box') {
      const b = box(f.id);
      const opened = ms.opened.includes(b.id);
      if (!opened) return BOXES[b.kind].open;
      return TIER_REVEAL[b.tier];
    }
    if (f.kind === 'ground') return EXPLORE.groundPick;
    if (f.kind === 'corpse') return EXPLORE.corpseSearch + 1;
    if (f.kind === 'bag') return EXPLORE.lostBagOpen;
    return EXPLORE.extractHold;
  };
  const searchDone = (f: Focus): boolean => {
    if (f.kind === 'box') return ms.opened.includes(f.id) && revealedOf(f.id) >= box(f.id).loot.length;
    if (f.kind === 'corpse') return ms.searched.includes(f.id);
    if (f.kind === 'bag') return open?.kind === 'bag';
    return true;
  };

  // --- Path finding (BFS on the 1 m grid) --------------------------------------------------
  const bfs = (sx: number, sy: number, tx: number, ty: number): [number, number][] => {
    const W = map.w;
    const start = Math.floor(sy) * W + Math.floor(sx);
    const goal = Math.floor(ty) * W + Math.floor(tx);
    if (start === goal) return [];
    const prev = new Int32Array(W * map.h).fill(-1);
    prev[start] = start;
    const q = [start];
    for (let head = 0; head < q.length && head < 2400; head += 1) {
      const i = q[head]!;
      if (i === goal) break;
      const x = i % W;
      const y = (i / W) | 0;
      for (const [nx, ny] of [[x + 1, y], [x - 1, y], [x, y + 1], [x, y - 1]] as const) {
        if (isWall(map, nx + 0.5, ny + 0.5)) continue;
        const j = ny * W + nx;
        if (prev[j] !== -1) continue;
        prev[j] = i;
        q.push(j);
      }
    }
    if (prev[goal] === -1) return [];
    const path: [number, number][] = [];
    for (let i = goal; i !== start; i = prev[i]!) path.push([(i % W) + 0.5, ((i / W) | 0) + 0.5]);
    return path.reverse();
  };

  // --- Combat ----------------------------------------------------------------------------
  const hurtPlayer = (dmg: number) => {
    if (p.guard > 0 || ended) return;
    p.hp -= dmg;
    p.guard = EXPLORE.hurtGuard;
    if (task && task.kind !== 'pick') task = undefined;
    if (p.hp <= 0) finish('death');
  };
  const fire = () => {
    if (p.reload > 0 || p.fireCd > 0) return;
    if (!p.gun) {
      // 无枪：推开 1 m，无伤害〔08·03〕
      if (p.pushCd > 0) return;
      p.pushCd = EXPLORE.pushCooldown;
      for (const e of enemies) {
        const d = Math.hypot(e.x - p.x, e.y - p.y);
        if (d < EXPLORE.pushRange + 0.6) moveCircle(e, ((e.x - p.x) / d) * 1, ((e.y - p.y) / d) * 1, R_ENEMY);
      }
      return;
    }
    if (p.mag <= 0) {
      startReload();
      return;
    }
    const w = WEAPONS[p.gun];
    p.mag -= 1;
    p.shots += 1;
    p.fired = true;
    p.fireCd = 1 / w.rate;
    const spread = p.gun === 'smg' ? (Math.random() - 0.5) * 0.08 : 0;
    const a = p.aim + spread;
    bullets.push({ x: p.x + Math.cos(a) * 0.5, y: p.y + Math.sin(a) * 0.5, vx: Math.cos(a) * w.speed, vy: Math.sin(a) * w.speed, left: w.range, dmg: w.dmg, enemy: false });
    noise(EXPLORE.soundGun);
    if (task) task = undefined;
  };
  const backpackAmmo = () => pack.stacks.filter((s) => s.item === 'ammo').reduce((a, s) => a + s.qty, 0);
  const startReload = () => {
    if (!p.gun || p.reload > 0) return;
    const w = WEAPONS[p.gun];
    if (p.mag >= w.mag) return;
    if (p.reserve <= 0 && backpackAmmo() <= 0) return;
    p.reload = w.reload;
  };
  const finishReload = () => {
    if (!p.gun) return;
    const w = WEAPONS[p.gun];
    let need = w.mag - p.mag;
    const fromReserve = Math.min(need, p.reserve);
    p.reserve -= fromReserve;
    p.mag += fromReserve;
    need -= fromReserve;
    if (need > 0) {
      const n = Math.min(need, backpackAmmo());
      const taken = takeItems([pack], 'ammo', n);
      if (taken) {
        pack = taken.holds[0]!;
        p.mag += n;
        packDirty = true;
      }
    }
  };
  const killEnemy = (e: Enemy) => {
    enemies.splice(enemies.indexOf(e), 1);
    ms.dead.push(e.id);
    p.kills += 1;
  };

  // --- Interaction -----------------------------------------------------------------------
  const nearestFocus = (): Focus | undefined => {
    let best: { f: Focus; d: number } | undefined;
    const consider = (f: Focus, x: number, y: number, r = 1.6) => {
      const d = Math.hypot(x - p.x, y - p.y);
      if (d <= r && (!best || d < best.d)) best = { f, d };
    };
    for (const b of map.boxes) consider({ kind: 'box', id: b.id }, b.x, b.y);
    for (const g of map.ground) if (g.loot.some((l) => !ms.taken.includes(l.id))) consider({ kind: 'ground', id: g.id }, g.x, g.y);
    for (const e of map.enemies) {
      if (!ms.dead.includes(e.id)) continue;
      const pos = deadPos.get(e.id) ?? [e.x, e.y];
      if (ms.searched.includes(e.id) && !e.corpse.some((l) => !ms.taken.includes(l.id))) continue;
      consider({ kind: 'corpse', id: e.id }, pos[0], pos[1]);
    }
    if (lost && (lost.items.length || lost.gun || lost.ammo > 0)) consider({ kind: 'bag' }, lost.x, lost.y);
    // 身边 1.2 m 内有搜点时优先搜点；否则在撤离区内才是撤离
    if (best && best.d <= 1.2) return best.f;
    if (Math.hypot(map.extract[0] - p.x, map.extract[1] - p.y) <= 2.2) return { kind: 'extract' };
    return best?.f;
  };
  const deadPos = new Map<number, [number, number]>();
  const sameFocus = (a?: Focus, b?: Focus) => !!a && !!b && a.kind === b.kind && (a as { id?: number }).id === (b as { id?: number }).id;
  const interact = () => {
    if (!focus || task) return;
    if (focus.kind === 'extract') {
      task = { kind: 'extract', t: 0, need: EXPLORE.extractHold };
      return;
    }
    if (searchDone(focus)) {
      open = focus;
      lootDirty = true;
      return;
    }
    task = { kind: focus.kind === 'ground' ? 'pick' : 'search', t: 0, need: searchNeed(focus), focus };
  };
  const completeTask = () => {
    const tk = task!;
    task = undefined;
    if (tk.kind === 'extract') {
      finish('extract');
      return;
    }
    if (tk.kind === 'heal') {
      const taken = takeItems([pack], 'bandage', 1);
      if (taken) {
        pack = taken.holds[0]!;
        p.hp = Math.min(RULES.hpMax, p.hp + EXPLORE.bandageHeal);
        packDirty = true;
      }
      return;
    }
    const f = tk.focus!;
    if (f.kind === 'box') {
      if (!ms.opened.includes(f.id)) ms.opened.push(f.id);
      else ms.revealed[f.id] = revealedOf(f.id) + 1;
      if (!searchDone(f)) task = { kind: 'search', t: 0, need: searchNeed(f), focus: f };
    } else if (f.kind === 'corpse') ms.searched.push(f.id);
    open = f;
    lootDirty = true;
  };
  const takeLoot = (lootId: number) => {
    if (!open) return;
    if (open.kind === 'bag' && lost) {
      if (lootId === -1 && lost.gun) {
        if (!p.gun) {
          p.gun = lost.gun;
        } else {
          const placed = placeLots([pack], [{ item: lost.gun, qty: 1 }], ids);
          if (!placed) return toast(t('ex.full'));
          pack = placed[0]!;
        }
        lost = { ...lost, gun: null };
      } else if (lootId === -2 && lost.ammo > 0) {
        p.reserve += lost.ammo;
        lost = { ...lost, ammo: 0 };
      } else {
        const lot = lost.items[lootId];
        if (!lot) return;
        const placed = placeLots([pack], [lot], ids);
        if (!placed) return toast(t('ex.full'));
        pack = placed[0]!;
        lost = { ...lost, items: lost.items.filter((_, i) => i !== lootId) };
      }
      p.taken += 1;
      packDirty = lootDirty = true;
      return;
    }
    const loot = lootOf(open).find((l) => l.id === lootId);
    if (!loot || ms.taken.includes(loot.id)) return;
    if (loot.item === 'ammo' && p.gun) {
      p.reserve += loot.qty;
    } else if (ITEMS[loot.item].weapon && !p.gun) {
      p.gun = loot.item as 'pistol' | 'smg';
    } else {
      const placed = placeLots([pack], [{ item: loot.item, qty: loot.qty, ...(loot.label !== undefined ? { label: loot.label, real: loot.real } : {}) }], ids);
      if (!placed) return toast(t('ex.full'));
      pack = placed[0]!;
    }
    ms.taken.push(loot.id);
    p.taken += 1;
    packDirty = lootDirty = true;
  };
  const dropStack = (id: number) => {
    pack = { ...pack, stacks: pack.stacks.filter((s) => s.id !== id) };
    packDirty = true;
  };
  let toastTimer = 0;
  const toast = (text: string) => {
    $('prompt').textContent = text;
    toastTimer = 1.6;
  };

  // --- End -------------------------------------------------------------------------------
  const finish = (outcome: SortieResult['outcome']) => {
    if (ended) return;
    ended = true;
    const backpack: Lot[] = pack.stacks.map((s) => ({ item: s.item, qty: s.qty, ...(s.label !== undefined ? { label: s.label, real: s.real } : {}) }));
    if (lost && init.recovery) {
      // 回收局：遗失包里没取回的，按结局处理（撤离后节点关闭，未取回即失去）
    }
    const result: SortieResult = {
      outcome,
      hp: Math.max(0, p.hp),
      gun: p.gun,
      ammo: p.mag + p.reserve,
      backpack,
      map: ms,
      fired: p.fired,
      x: p.x,
      y: p.y,
    };
    cleanup();
    o.onEnd(result, { kills: p.kills, seconds: Math.round(elapsed), shots: p.shots, taken: p.taken });
  };

  // --- Input -----------------------------------------------------------------------------
  const onKey = (e: KeyboardEvent) => {
    const k = e.key.toLowerCase();
    if (e.type === 'keydown') {
      if (k === 'escape') {
        paused = !paused;
        $('pause').classList.toggle('on', paused);
      }
      if (paused) return;
      keys.add(k);
      if (k === 'e') interact();
      if (k === 'r') startReload();
      if (k === 'h' && !task && pack.stacks.some((s) => s.item === 'bandage') && p.hp < RULES.hpMax) task = { kind: 'heal', t: 0, need: EXPLORE.bandageTime };
      if (k === 'f' && open) for (const l of open.kind === 'bag' ? [] : lootOf(open)) if (!ms.taken.includes(l.id)) takeLoot(l.id);
    } else {
      keys.delete(k);
      if (k === 'e' && task?.kind === 'extract') task = undefined;
    }
    if (['w', 'a', 's', 'd', ' ', 'tab', 'shift', 'control', 'e', 'r', 'h', 'f'].includes(k)) e.preventDefault();
  };
  const toCanvas = (e: PointerEvent) => {
    const r = canvas.getBoundingClientRect();
    mouse.x = ((e.clientX - r.left) * canvas.width) / r.width;
    mouse.y = ((e.clientY - r.top) * canvas.height) / r.height;
  };
  const onMove = (e: PointerEvent) => toCanvas(e);
  const onDown = (e: PointerEvent) => {
    if (paused || e.button !== 0) return;
    toCanvas(e);
    mouse.down = true;
    fire();
  };
  const onUp = () => (mouse.down = false);
  const onClick = (e: Event) => {
    const el = (e.target as HTMLElement).closest<HTMLElement>('[data-ex]');
    if (!el) return;
    e.stopPropagation();
    const act = el.dataset.ex!;
    if (act === 'resume') {
      paused = false;
      $('pause').classList.remove('on');
    } else if (act === 'abandon') finish('abandon');
    else if (act === 'take') takeLoot(Number(el.dataset.id));
    else if (act === 'drop') dropStack(Number(el.dataset.id));
    else if (act === 'close') {
      open = undefined;
      lootDirty = true;
    }
  };
  view.addEventListener('keydown', onKey, true);
  view.addEventListener('keyup', onKey, true);
  canvas.addEventListener('pointermove', onMove);
  canvas.addEventListener('pointerdown', onDown);
  view.addEventListener('pointerup', onUp);
  root.addEventListener('click', onClick);
  const blurKeys = () => keys.clear();
  view.addEventListener('blur', blurKeys);

  // --- Simulation ------------------------------------------------------------------------
  const visible = (e: { x: number; y: number }) =>
    Math.hypot(e.x - p.x, e.y - p.y) <= EXPLORE.viewPlayer && lineOfSight(map, p.x, p.y, e.x, e.y);
  const enemySees = (e: Enemy) => {
    const dx = p.x - e.x;
    const dy = p.y - e.y;
    const d = Math.hypot(dx, dy);
    if (d > EXPLORE.viewEnemy) return false;
    let diff = Math.abs(Math.atan2(dy, dx) - e.face);
    if (diff > Math.PI) diff = Math.PI * 2 - diff;
    if (d > 2 && diff > ((EXPLORE.fovEnemy / 2) * Math.PI) / 180) return false;
    return lineOfSight(map, e.x, e.y, p.x, p.y);
  };
  const steer = (e: Enemy, tx: number, ty: number, speed: number, dt: number) => {
    if (e.pathAt <= 0 || !e.path.length) {
      e.path = lineOfSight(map, e.x, e.y, tx, ty) ? [[tx, ty]] : bfs(e.x, e.y, tx, ty);
      e.pathAt = 0.5;
    }
    const nextP = e.path[0];
    if (!nextP) return;
    const dx = nextP[0] - e.x;
    const dy = nextP[1] - e.y;
    const d = Math.hypot(dx, dy);
    if (d < 0.3) {
      e.path.shift();
      return;
    }
    e.face = Math.atan2(dy, dx);
    moveCircle(e, (dx / d) * speed * dt, (dy / d) * speed * dt, R_ENEMY);
  };
  const updateEnemy = (e: Enemy, dt: number, chasers: number) => {
    const def = ENEMIES[e.kind];
    e.cd = Math.max(0, e.cd - dt);
    e.pathAt -= dt;
    e.hit = Math.max(0, e.hit - dt);
    const d = Math.hypot(p.x - e.x, p.y - e.y);
    const sees = enemySees(e);
    if (sees && (e.state === 'chase' || chasers < EXPLORE.maxChasers[map.area])) {
      e.state = 'chase';
      e.lastSeen = 0;
      e.target = [p.x, p.y];
    } else if (e.state === 'chase') {
      e.lastSeen += dt;
      if (e.lastSeen > EXPLORE.loseTarget) e.state = 'patrol';
    }
    // 冲刺中
    if (e.dash > 0) {
      e.dash -= dt;
      moveCircle(e, e.dashDir[0] * def.dashSpeed! * dt, e.dashDir[1] * def.dashSpeed! * dt, R_ENEMY);
      if (!e.dashHit && Math.hypot(p.x - e.x, p.y - e.y) < R_PLAYER + R_ENEMY + 0.2) {
        e.dashHit = true;
        hurtPlayer(def.dmg);
      }
      return;
    }
    if (e.windup > 0) {
      e.windup -= dt;
      e.face = Math.atan2(p.y - e.y, p.x - e.x);
      if (e.windup <= 0) {
        if (e.kind === 'melee') {
          if (d < 1.6) hurtPlayer(def.dmg);
        } else if (e.kind === 'charger') {
          e.dash = def.dashTime!;
          e.dashHit = false;
          e.dashDir = [(p.x - e.x) / (d || 1), (p.y - e.y) / (d || 1)];
        } else if (e.kind === 'ranged') {
          const a = Math.atan2(p.y - e.y, p.x - e.x);
          bullets.push({ x: e.x, y: e.y, vx: Math.cos(a) * def.bulletSpeed!, vy: Math.sin(a) * def.bulletSpeed!, left: def.range! + 2, dmg: def.dmg, enemy: true });
        }
        e.cd = def.cd;
      }
      return;
    }
    if (e.state === 'chase') {
      const canSee = sees || lineOfSight(map, e.x, e.y, p.x, p.y);
      if (e.kind === 'melee' && d < 1.3 && e.cd <= 0) e.windup = def.windup;
      else if (e.kind === 'charger' && d < 6 && canSee && e.cd <= 0) e.windup = def.windup;
      else if (e.kind === 'ranged' && d < def.range! && canSee && e.cd <= 0) e.windup = def.windup;
      else if (e.kind === 'ranged' && d < 6 && canSee) {
        // 拉开距离
        moveCircle(e, ((e.x - p.x) / d) * def.speed * dt, ((e.y - p.y) / d) * def.speed * dt, R_ENEMY);
      } else steer(e, e.target![0], e.target![1], def.speed * 1.15, dt);
      return;
    }
    if (e.state === 'investigate' && e.target) {
      steer(e, e.target[0], e.target[1], def.speed, dt);
      if (Math.hypot(e.target[0] - e.x, e.target[1] - e.y) < 0.8) {
        e.lastSeen += dt;
        if (e.lastSeen > EXPLORE.loseTarget) {
          e.state = 'patrol';
          e.lastSeen = 0;
        }
      }
      return;
    }
    const wp = e.patrol[e.patrolIdx % e.patrol.length]!;
    steer(e, wp[0], wp[1], def.speed * 0.7, dt);
    if (Math.hypot(wp[0] - e.x, wp[1] - e.y) < 0.8) {
      e.patrolIdx += 1;
      e.path = [];
    }
  };

  const step = (dt: number) => {
    elapsed += dt;
    p.guard = Math.max(0, p.guard - dt);
    p.fireCd = Math.max(0, p.fireCd - dt);
    p.pushCd = Math.max(0, p.pushCd - dt);
    if (p.reload > 0) {
      p.reload -= dt;
      if (p.reload <= 0) {
        p.reload = 0;
        finishReload();
      }
    }
    // Aim
    const cam = camera();
    p.aim = Math.atan2(mouse.y / PX + cam.y - p.y, mouse.x / PX + cam.x - p.x);
    // Move
    let mx = (keys.has('d') ? 1 : 0) - (keys.has('a') ? 1 : 0);
    let my = (keys.has('s') ? 1 : 0) - (keys.has('w') ? 1 : 0);
    const moving = mx !== 0 || my !== 0;
    const heavy = carry() > EXPLORE.heavyLoad;
    const sneak = keys.has('control') || keys.has('c');
    const sprinting = moving && keys.has('shift') && p.stamina > 0 && !sneak;
    let speed: number = sneak ? EXPLORE.sneak : mouse.down && p.gun ? EXPLORE.aimWalk : heavy ? EXPLORE.heavyWalk : EXPLORE.walk;
    if (sprinting) {
      speed = heavy ? EXPLORE.heavySprint : EXPLORE.sprint;
      p.stamina = Math.max(0, p.stamina - EXPLORE.staminaDrain * dt);
      p.staminaWait = EXPLORE.staminaDelay;
    } else {
      p.staminaWait = Math.max(0, p.staminaWait - dt);
      if (p.staminaWait <= 0) p.stamina = Math.min(EXPLORE.stamina, p.stamina + EXPLORE.staminaRegen * dt);
    }
    if (moving) {
      const len = Math.hypot(mx, my);
      mx /= len;
      my /= len;
      moveCircle(p, mx * speed * dt, my * speed * dt, R_PLAYER);
      if (task && task.kind !== 'extract') task = undefined;
      if (task?.kind === 'extract' && Math.hypot(map.extract[0] - p.x, map.extract[1] - p.y) > 2.2) task = undefined;
      if (open && Math.hypot(...focusPos(open).map((v, i) => v - (i ? p.y : p.x)) as [number, number]) > 2.2) {
        open = undefined;
        lootDirty = true;
      }
      if (Math.random() < dt * 4) noise(sprinting ? EXPLORE.soundSprint : sneak ? EXPLORE.soundSneak : EXPLORE.soundWalk);
    }
    if (mouse.down && !sprinting && p.gun === 'smg') fire();
    // Tasks
    if (task) {
      if (task.kind === 'extract' && !keys.has('e')) task = undefined;
      else {
        task.t += dt;
        if (task.kind === 'search' && Math.random() < dt) noise(EXPLORE.soundSearch);
        if (task.t >= task.need) completeTask();
      }
    }
    // Sounds → investigate
    for (const n of noises) {
      for (const e of enemies) {
        if (e.state === 'chase') continue;
        if (Math.hypot(e.x - n.x, e.y - n.y) <= n.r) {
          e.state = 'investigate';
          e.target = [n.x + (Math.random() - 0.5) * 3, n.y + (Math.random() - 0.5) * 3];
          e.path = [];
          e.lastSeen = 0;
        }
      }
    }
    noises.length = 0;
    // Enemies
    const chasers = enemies.filter((e) => e.state === 'chase').length;
    for (const e of [...enemies]) updateEnemy(e, dt, chasers);
    // Bullets
    for (const b of [...bullets]) {
      const stepLen = Math.hypot(b.vx, b.vy) * dt;
      b.x += b.vx * dt;
      b.y += b.vy * dt;
      b.left -= stepLen;
      let gone = b.left <= 0 || isWall(map, b.x, b.y);
      if (!gone && b.enemy && Math.hypot(b.x - p.x, b.y - p.y) < R_PLAYER + 0.1) {
        hurtPlayer(b.dmg);
        gone = true;
      }
      if (!gone && !b.enemy) {
        for (const e of enemies) {
          if (Math.hypot(b.x - e.x, b.y - e.y) < R_ENEMY + 0.1) {
            e.hp -= b.dmg;
            e.hit = 0.12;
            e.state = 'chase';
            e.target = [p.x, p.y];
            e.lastSeen = 0;
            if (e.hp <= 0) {
              deadPos.set(e.id, [e.x, e.y]);
              killEnemy(e);
            }
            gone = true;
            break;
          }
        }
      }
      if (gone) bullets.splice(bullets.indexOf(b), 1);
    }
    focus = nearestFocus();
    toastTimer = Math.max(0, toastTimer - dt);
  };
  const focusPos = (f: Focus): [number, number] => {
    if (f.kind === 'box') {
      const b = box(f.id);
      return [b.x, b.y];
    }
    if (f.kind === 'ground') {
      const g = map.ground.find((x) => x.id === f.id)!;
      return [g.x, g.y];
    }
    if (f.kind === 'corpse') return deadPos.get(f.id) ?? [map.enemies.find((e) => e.id === f.id)!.x, map.enemies.find((e) => e.id === f.id)!.y];
    if (f.kind === 'bag') return [lost?.x ?? 0, lost?.y ?? 0];
    return [map.extract[0], map.extract[1]];
  };
  // Dead enemies from a previous sortie keep their recorded spawn (corpse) position.
  for (const e of map.enemies) if (ms.dead.includes(e.id)) deadPos.set(e.id, [e.x, e.y]);

  // --- Rendering -------------------------------------------------------------------------
  const camera = () => {
    const vw = canvas.width / PX;
    const vh = canvas.height / PX;
    return {
      x: Math.max(0, Math.min(map.w - vw, p.x - vw / 2)),
      y: Math.max(0, Math.min(map.h - vh, p.y - vh / 2)),
    };
  };
  const draw = () => {
    const W = canvas.width;
    const H = canvas.height;
    const cam = camera();
    const sx = (x: number) => (x - cam.x) * PX;
    const sy = (y: number) => (y - cam.y) * PX;
    ctx.fillStyle = '#2a2622';
    ctx.fillRect(0, 0, W, H);
    // floor speckle
    const x0 = Math.floor(cam.x);
    const y0 = Math.floor(cam.y);
    for (let y = y0; y < y0 + H / PX + 2; y += 1) {
      for (let x = x0; x < x0 + W / PX + 2; x += 1) {
        if (x < 0 || y < 0 || x >= map.w || y >= map.h) continue;
        const wall = map.walls[y * map.w + x] === 1;
        const n = ((x * 73856093) ^ (y * 19349663)) & 7;
        ctx.fillStyle = wall ? (n < 3 ? '#5b4b3c' : '#544537') : n === 0 ? '#332e29' : n === 1 ? '#2e2a25' : '#302b26';
        ctx.fillRect(sx(x), sy(y), PX + 0.5, PX + 0.5);
        if (wall) {
          ctx.fillStyle = '#6e5b48';
          ctx.fillRect(sx(x), sy(y), PX, 3);
        }
      }
    }
    // extraction zone
    const pulse = 0.5 + 0.5 * Math.sin(elapsed * 3);
    ctx.strokeStyle = `rgba(110, 220, 140, ${0.5 + 0.4 * pulse})`;
    ctx.fillStyle = 'rgba(80, 200, 120, 0.12)';
    ctx.lineWidth = 2;
    ctx.beginPath();
    ctx.arc(sx(map.extract[0]), sy(map.extract[1]), 2.2 * PX, 0, Math.PI * 2);
    ctx.fill();
    ctx.stroke();
    ctx.fillStyle = '#9ff0b4';
    ctx.font = '700 12px sans-serif';
    ctx.textAlign = 'center';
    ctx.fillText(t('ex.extractZone'), sx(map.extract[0]), sy(map.extract[1]) + 4);
    // entry / train
    ctx.fillStyle = '#3d3a36';
    ctx.fillRect(sx(0.2), sy(map.entry[1] - 4), PX * 1.4, PX * 8);
    // boxes
    for (const b of map.boxes) {
      const done = ms.opened.includes(b.id) && b.loot.every((l, i) => i >= revealedOf(b.id) || ms.taken.includes(l.id)) && revealedOf(b.id) >= b.loot.length;
      ctx.fillStyle = done ? '#3a332c' : '#7a5a38';
      ctx.fillRect(sx(b.x) - PX * 0.45, sy(b.y) - PX * 0.35, PX * 0.9, PX * 0.7);
      ctx.strokeStyle = TIER_COLOR[b.tier]!;
      ctx.lineWidth = b.tier ? 3 : 1.5;
      ctx.strokeRect(sx(b.x) - PX * 0.45, sy(b.y) - PX * 0.35, PX * 0.9, PX * 0.7);
    }
    for (const g of map.ground) {
      if (!g.loot.some((l) => !ms.taken.includes(l.id))) continue;
      ctx.fillStyle = '#e0c890';
      ctx.beginPath();
      ctx.arc(sx(g.x), sy(g.y), 4, 0, Math.PI * 2);
      ctx.fill();
    }
    for (const [id, [x, y]] of deadPos) {
      const e = map.enemies.find((m) => m.id === id)!;
      const hasLoot = !ms.searched.includes(id) || e.corpse.some((l) => !ms.taken.includes(l.id));
      ctx.fillStyle = hasLoot ? '#5a2a22' : '#3a2420';
      ctx.fillRect(sx(x) - 7, sy(y) - 4, 14, 8);
    }
    if (lost && (lost.items.length || lost.gun || lost.ammo > 0)) {
      ctx.fillStyle = '#e8d35a';
      ctx.fillRect(sx(lost.x) - 7, sy(lost.y) - 7, 14, 14);
      ctx.fillStyle = '#1b1208';
      ctx.fillText('!', sx(lost.x), sy(lost.y) + 4);
    }
    // enemies (visible only)
    for (const e of enemies) {
      if (!visible(e)) continue;
      const ex = sx(e.x);
      const ey = sy(e.y);
      ctx.fillStyle = e.hit > 0 ? '#ffffff' : ENEMY_COLOR[e.kind];
      ctx.beginPath();
      ctx.arc(ex, ey, R_ENEMY * PX, 0, Math.PI * 2);
      ctx.fill();
      ctx.strokeStyle = '#1b1208';
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.moveTo(ex, ey);
      ctx.lineTo(ex + Math.cos(e.face) * PX * 0.6, ey + Math.sin(e.face) * PX * 0.6);
      ctx.stroke();
      if (e.windup > 0) {
        ctx.strokeStyle = '#ff4030';
        ctx.lineWidth = 3;
        ctx.beginPath();
        ctx.arc(ex, ey, R_ENEMY * PX + 5, 0, Math.PI * 2);
        ctx.stroke();
      }
      if (e.state !== 'patrol') {
        ctx.fillStyle = e.state === 'chase' ? '#ff5a40' : '#f0c040';
        ctx.fillText(e.state === 'chase' ? '!' : '?', ex, ey - 14);
      }
      // hp
      ctx.fillStyle = '#000a';
      ctx.fillRect(ex - 10, ey + 12, 20, 3);
      ctx.fillStyle = '#e05a3a';
      ctx.fillRect(ex - 10, ey + 12, (20 * e.hp) / ENEMIES[e.kind].hp, 3);
    }
    // bullets
    for (const b of bullets) {
      ctx.fillStyle = b.enemy ? '#c070ff' : '#ffe08a';
      ctx.beginPath();
      ctx.arc(sx(b.x), sy(b.y), b.enemy ? 4 : 2.5, 0, Math.PI * 2);
      ctx.fill();
    }
    // player
    const px = sx(p.x);
    const py = sy(p.y);
    ctx.strokeStyle = 'rgba(255, 220, 140, 0.25)';
    ctx.lineWidth = 1;
    ctx.beginPath();
    ctx.moveTo(px, py);
    ctx.lineTo(px + Math.cos(p.aim) * PX * 6, py + Math.sin(p.aim) * PX * 6);
    ctx.stroke();
    ctx.fillStyle = p.guard > 0 ? '#ffffff' : '#e8a33c';
    ctx.beginPath();
    ctx.arc(px, py, R_PLAYER * PX, 0, Math.PI * 2);
    ctx.fill();
    ctx.strokeStyle = '#1b1208';
    ctx.lineWidth = 3;
    ctx.beginPath();
    ctx.moveTo(px, py);
    ctx.lineTo(px + Math.cos(p.aim) * PX * 0.7, py + Math.sin(p.aim) * PX * 0.7);
    ctx.stroke();
    // vision fog
    const g = ctx.createRadialGradient(px, py, PX * 6, px, py, EXPLORE.viewPlayer * PX);
    g.addColorStop(0, 'rgba(10,8,6,0)');
    g.addColorStop(1, 'rgba(10,8,6,0.6)');
    ctx.fillStyle = g;
    ctx.fillRect(0, 0, W, H);
    // focus highlight
    if (focus) {
      const [fx, fy] = focusPos(focus);
      ctx.strokeStyle = '#ffe08a';
      ctx.lineWidth = 2;
      ctx.setLineDash([4, 3]);
      ctx.strokeRect(sx(fx) - PX * 0.7, sy(fy) - PX * 0.6, PX * 1.4, PX * 1.2);
      ctx.setLineDash([]);
    }
    drawMinimap();
  };
  const drawMinimap = () => {
    const s = 2.4;
    const ox = canvas.width - map.w * s - 16;
    const oy = 70;
    ctx.fillStyle = 'rgba(12,10,8,0.8)';
    ctx.fillRect(ox - 4, oy - 4, map.w * s + 8, map.h * s + 8);
    ctx.fillStyle = '#5b4b3c';
    for (let y = 0; y < map.h; y += 1) for (let x = 0; x < map.w; x += 1) if (map.walls[y * map.w + x]) ctx.fillRect(ox + x * s, oy + y * s, s, s);
    for (const b of map.boxes) {
      ctx.fillStyle = TIER_COLOR[b.tier]!;
      ctx.fillRect(ox + b.x * s - 2, oy + b.y * s - 2, 4, 4);
    }
    ctx.fillStyle = '#6ee08c';
    ctx.fillRect(ox + map.extract[0] * s - 3, oy + map.extract[1] * s - 3, 6, 6);
    if (lost) {
      ctx.fillStyle = '#e8d35a';
      ctx.fillRect(ox + lost.x * s - 3, oy + lost.y * s - 3, 6, 6);
    }
    ctx.fillStyle = '#e8a33c';
    ctx.beginPath();
    ctx.arc(ox + p.x * s, oy + p.y * s, 3, 0, Math.PI * 2);
    ctx.fill();
  };

  // --- HUD -------------------------------------------------------------------------------
  const wallsText = map.walls.join('');
  const hud = () => {
    $('hp').style.width = `${Math.max(0, (p.hp / RULES.hpMax) * 100)}%`;
    $('hpv').textContent = String(Math.max(0, Math.ceil(p.hp)));
    $('st').style.width = `${(p.stamina / EXPLORE.stamina) * 100}%`;
    $('ammo').textContent = p.gun
      ? `${t(`item.${p.gun}`)} ${p.reload > 0 ? t('ex.reloading') : `${p.mag}/${p.reserve + backpackAmmo()}`}`
      : t('ex.unarmed');
    const c = carry();
    $('load').textContent = `${c.toFixed(1)}${c > EXPLORE.heavyLoad ? ` · ${t('ex.heavy')}` : ''}`;
    const m = Math.floor(elapsed / 60);
    const s = Math.floor(elapsed % 60);
    $('time').textContent = `${m}:${String(s).padStart(2, '0')} / 10:00`;
    const prog = $('progress');
    if (task) {
      prog.classList.add('on');
      $('progress-fill').style.width = `${Math.min(100, (task.t / task.need) * 100)}%`;
      $('progress-label').textContent = t(`ex.task.${task.kind}`);
    } else prog.classList.remove('on');
    if (toastTimer <= 0) {
      $('prompt').textContent = focus
        ? t(`ex.prompt.${focus.kind}`) + (focus.kind === 'box' ? ` · ${t(`ex.box.${box(focus.id).kind}`)}${init.exp ? ` · ${t(`ex.tier.${box(focus.id).tier}`)}` : ''}` : '')
        : '';
    }
    if (packDirty) renderPack();
    if (lootDirty) renderLoot();
    o.expose?.({
      hp: p.hp, x: p.x, y: p.y, mag: p.mag, reserve: p.reserve, enemies: enemies.length, kills: p.kills, focus: focus?.kind ?? null,
      task: task?.kind ?? null, open: open?.kind ?? null, pack: pack.stacks.map((s) => [s.item, s.qty]), elapsed, extract: map.extract,
      entry: map.entry, w: map.w, h: map.h, walls: wallsText, boxes: map.boxes.map((b) => [b.id, b.x, b.y, b.tier, ms.opened.includes(b.id), revealedOf(b.id), b.loot.length]),
    });
  };
  const cellPx = 30;
  const renderPack = () => {
    packDirty = false;
    const cells = pack.stacks
      .map((s) => {
        const [w, h] = s.rot ? [ITEMS[s.item].h, ITEMS[s.item].w] : [ITEMS[s.item].w, ITEMS[s.item].h];
        return `<button type="button" class="ex-item" data-ex="drop" data-id="${s.id}" title="${t('ex.drop')}" style="left:${s.x * cellPx}px;top:${s.y * cellPx}px;width:${w * cellPx - 2}px;height:${h * cellPx - 2}px;background:${o.chip[s.item]}"><span>${o.itemName(s.item)}</span><b>${s.qty}</b></button>`;
      })
      .join('');
    $('pack').innerHTML = `<div class="ex-cells" style="width:${BACKPACK.w * cellPx}px;height:${BACKPACK.h * cellPx}px">${cells}</div><small class="hint">${t('ex.dropHint')}</small>`;
  };
  const renderLoot = () => {
    lootDirty = false;
    const box$ = $('loot');
    if (!open) {
      box$.classList.remove('on');
      box$.innerHTML = '';
      return;
    }
    box$.classList.add('on');
    let rows = '';
    if (open.kind === 'bag' && lost) {
      if (lost.gun) rows += `<button type="button" class="ex-row" data-ex="take" data-id="-1"><span class="chip" style="background:${o.chip[lost.gun]}"></span>${o.itemName(lost.gun)}</button>`;
      if (lost.ammo > 0) rows += `<button type="button" class="ex-row" data-ex="take" data-id="-2"><span class="chip" style="background:${o.chip.ammo}"></span>${o.itemName('ammo')} ×${lost.ammo}</button>`;
      lost.items.forEach((l, i) => {
        rows += `<button type="button" class="ex-row" data-ex="take" data-id="${i}"><span class="chip" style="background:${o.chip[l.item]}"></span>${o.itemName(l.item)} ×${l.qty}</button>`;
      });
    } else {
      for (const l of lootOf(open)) {
        const took = ms.taken.includes(l.id);
        const grade = l.label !== undefined && init.exp ? ` · ${t(`grade.${l.label}`)}` : '';
        rows += `<button type="button" class="ex-row ${took ? 'took' : ''}" data-ex="take" data-id="${l.id}" ${took ? 'disabled' : ''}><span class="chip" style="background:${o.chip[l.item]}"></span>${o.itemName(l.item)} ×${l.qty}${grade}${took ? ` · ${t('ex.took')}` : ''}</button>`;
      }
      if (open.kind === 'box' && revealedOf(open.id) < box(open.id).loot.length) rows += `<p class="hint">${t('ex.moreGroups')}</p>`;
    }
    box$.innerHTML = `<div class="ex-title">${t(`ex.prompt.${open.kind}`)}<button type="button" class="ex-x" data-ex="close">×</button></div>${rows || `<p class="hint">${t('ex.empty')}</p>`}<small class="hint">${t('ex.takeHint')}</small>`;
  };

  // --- Loop ------------------------------------------------------------------------------
  let raf = 0;
  let last = view.performance.now();
  const resize = () => {
    const r = root.getBoundingClientRect();
    canvas.width = Math.max(320, Math.round(r.width));
    canvas.height = Math.max(240, Math.round(r.height));
  };
  resize();
  view.addEventListener('resize', resize);
  const frame = (now: number) => {
    if (ended) return;
    // 固定步长子步：低帧率时把一帧拆成多个 ≤0.05 s 的模拟步，避免慢动作与子弹穿透
    let rest = Math.min(0.25, (now - last) / 1000);
    last = now;
    while (!paused && !ended && rest > 1e-4) {
      const dt = Math.min(0.05, rest);
      step(dt);
      rest -= dt;
    }
    if (ended) return;
    draw();
    hud();
    raf = view.requestAnimationFrame(frame);
  };
  raf = view.requestAnimationFrame(frame);

  const cleanup = () => {
    view.cancelAnimationFrame(raf);
    view.removeEventListener('keydown', onKey, true);
    view.removeEventListener('keyup', onKey, true);
    view.removeEventListener('pointerup', onUp);
    view.removeEventListener('resize', resize);
    view.removeEventListener('blur', blurKeys);
    canvas.removeEventListener('pointermove', onMove);
    canvas.removeEventListener('pointerdown', onDown);
    root.removeEventListener('click', onClick);
    root.innerHTML = '';
  };
  return {
    dispose() {
      if (!ended) {
        ended = true;
        cleanup();
      }
    },
  };
}
