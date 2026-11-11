/**
 * 《末日列车》v0.2 经营核心：选线 → 到站点车厢经营 / 探索出击 → 继续或收车 → 日末结算 → 偿债。
 * Pure, deterministic (seeded RNG stored in state) and JSON-serialisable: the whole game is one save object.
 * 规则出处见 design/v0.2-plan.md；【补充案】在 game.exp = false 时全部关闭。
 */
import {
  CARS,
  DEVICES,
  DRIVER_CHANCE,
  DRIVER_ODDS,
  DRIVERS,
  EXP,
  FARE,
  COMP,
  GRADE_FACTOR,
  ITEM_IDS,
  ITEMS,
  isCommerce,
  PASSENGER_POOL,
  PLACE_COUNT,
  PLANT,
  RECIPES,
  RIDER_WANT_TAGS,
  ROUTE_TEMPLATES,
  RULES,
  SEAT_ODDS,
  SERVICES,
  STORAGE_H,
  TAG_WEIGHTS,
  unitOf,
  type CarKind,
  type DeviceKind,
  type Grade,
  type ItemId,
  type NodeKind,
  type Region,
  type Tag,
  type Theme,
} from './data.ts';
import { clone, countIn, findStack, fits, moveStack, placeLots, splitStack, takeItems, type Hold, type Lot, type Stack } from './grid.ts';
import { emptyMapState, type MapState } from './explore-map.ts';

export const SAVE_VERSION = 2;

export type Phase = 'station' | 'travel' | 'explore' | 'dayEnd' | 'settlement' | 'won' | 'lost';

export interface Goods {
  readonly item: ItemId;
  /** 交易单位数量（弹药为 30 发一份） */
  readonly qty: number;
  /** 单拿单价（按标称品质锁定；验货揭穿后按真实品质重算） */
  unit: number;
  readonly label?: Grade;
  readonly real?: Grade;
}
export interface Need {
  readonly item: ItemId;
  readonly qty: number;
  /** 本地零售牌价（良品基准，锁定） */
  price: number;
}
export interface Customer {
  readonly id: number;
  readonly kind: 'supplier' | 'buyer';
  /** 接待车厢：0 = 火车头货舱，其余为存储车厢 uid */
  readonly car: number;
  readonly arms?: boolean;
  readonly goods?: Goods[];
  readonly discount?: number;
  batchPrice?: number;
  readonly needs?: Need[];
  budget?: number;
  outcome?: 'deal' | 'pass' | 'walk' | 'caught';
  amount?: number;
  /** 【补充案】议价结果 */
  haggle?: { readonly pct: number; readonly result: 'ok' | 'stay' | 'walk' };
  /** 【补充案】已验货 */
  inspected?: boolean;
}
export interface Candidate {
  readonly id: number;
  readonly noble: boolean;
  dest?: number;
  seg?: number;
  fare?: number;
  deadline?: number;
  comp?: number;
  /** 一次性登车判定结果 */
  status?: 'boarded' | 'declined';
}
export interface Rider {
  readonly id: number;
  readonly noble: boolean;
  readonly dest: number;
  readonly fare: number;
  readonly deadline: number;
  readonly comp: number;
  readonly seat: number;
  want?: { readonly item: ItemId; readonly price: number; sold?: boolean };
  carry: number;
}
export interface RouteNode {
  readonly kind: NodeKind;
  readonly theme?: Theme;
}
export interface Route {
  readonly nodes: readonly RouteNode[];
  arrived: number;
}
export interface LostBag {
  readonly gun: 'pistol' | 'smg' | null;
  readonly ammo: number;
  readonly items: Lot[];
  readonly x: number;
  readonly y: number;
}
export interface Site {
  readonly seed: number;
  readonly area: 'resource' | 'hunt';
  readonly theme: Theme;
  sorties: number;
  closed: boolean;
  map: MapState;
  lost?: LostBag;
  recovery: boolean;
  /** 出击进行中（读档恢复同一现场） */
  active?: { recovery: boolean; bandages: Lot[] };
}
export interface Station {
  readonly id: number;
  readonly kind: NodeKind;
  readonly place: number;
  customers: Customer[];
  passengers: Candidate[];
  driver?: { readonly grade: 1 | 2 | 3; hired?: boolean };
  inspections: number;
  site?: Site;
  /** 【补充案】稽查员到访记录 */
  inspector?: { readonly fined: number; readonly seized: number };
}
export interface Plot {
  progress: number;
  growing: boolean;
  ripe: boolean;
}
export interface Device {
  readonly uid: number;
  readonly kind: DeviceKind;
  job?: { readonly recipe: string; progress: number; readonly inputs: Lot[] };
  out?: Lot;
}
export interface Car {
  readonly uid: number;
  readonly kind: CarKind;
  level: 0 | 1 | 2;
  /** 已付购建现金（回收按 60%） */
  paid: number;
  plots?: Plot[];
  devices?: Device[];
}
export interface DayLedger {
  segments: number;
  fuelRaw: number;
  retail: number;
  tickets: number;
  purchases: number;
  invest: number;
  repaid: number;
  delivered: number;
  refunds: number;
  fines: number;
  /** 当日开过枪 */
  fired: ('pistol' | 'smg')[];
  /** 当日运转过的设备 uid */
  worked: number[];
  wageRate: number;
}
export interface Bill {
  readonly day: number;
  readonly segments: number;
  readonly fuel: number;
  readonly wages: number;
  readonly living: number;
  readonly upkeep: number;
  readonly refunds: number;
  readonly total: number;
  readonly cashBefore: number;
  readonly cashAfter: number;
  readonly ledger: DayLedger;
  readonly heads: { readonly normal: number; readonly noble: number };
  readonly overdue: number;
}
export interface Settlement {
  readonly day: number;
  readonly required: number;
  readonly due: number;
}
export interface Game {
  readonly version: number;
  readonly seed: number;
  /** 【补充案】实验玩法开关 */
  readonly exp: boolean;
  rng: number;
  day: number;
  phase: Phase;
  resume?: Phase;
  cash: number;
  debt: number;
  repaid: number;
  holds: Hold[];
  cars: Car[];
  /** 自用：主武器 + 专用弹药栏（≤60 发） */
  kit: { gun: 'pistol' | 'smg' | null; ammo: number };
  hp: number;
  driver: 0 | 1 | 2 | 3;
  pendingDriver?: 1 | 2 | 3;
  nodesToday: number;
  linesToday: number;
  station: Station;
  nextId: number;
  routeOptions?: RouteNode[][];
  route?: Route;
  riders: Rider[];
  /** 探索带回但还没放进货舱的物品（必须整理后才能发车） */
  pending: Lot[];
  today: DayLedger;
  bill?: Bill;
  settlement?: Settlement;
  cleared: boolean;
  lostReason?: 'debt';
  /** 缺供计数〔07·04〕 */
  short: { water: number; ration: number };
  /** 【补充案】地区声望、坑客计数、待到访稽查 */
  rep: Record<Region, number>;
  cheats: Record<Region, number>;
  audit: Record<Region, boolean>;
  stats: { revenue: number; tickets: number; passengers: number; stations: number; sorties: number; extracted: number; deaths: number };
}

export type ActionError =
  | 'phase' | 'cash' | 'negative-cash' | 'space' | 'seats' | 'no-stock' | 'budget' | 'done' | 'unbound'
  | 'max-cars' | 'amount' | 'not-found' | 'quota' | 'lines' | 'route-left' | 'pending' | 'no-depot' | 'no-hire'
  | 'materials' | 'not-empty' | 'slots' | 'busy' | 'max-level' | 'declined' | 'rep' | 'explore' | 'no-gun' | 'closed'
  | 'inspections' | 'not-commerce' | 'recovery';
export type ActionResult =
  | { readonly ok: true; readonly amount?: number; readonly note?: string }
  | { readonly ok: false; readonly code: ActionError };
const fail = (code: ActionError): ActionResult => ({ ok: false, code });
const success = (amount?: number, note?: string): ActionResult =>
  ({ ok: true, ...(amount !== undefined ? { amount } : {}), ...(note ? { note } : {}) });

// --- RNG (mulberry32; state kept in the save) ------------------------------------------

function next(game: Game): number {
  let t = (game.rng = (game.rng + 0x6d2b79f5) | 0);
  t = Math.imul(t ^ (t >>> 15), t | 1);
  t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
  return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}
const int = (game: Game, min: number, max: number) => min + Math.floor(next(game) * (max - min + 1));
const pick = <T>(game: Game, list: readonly T[]): T => list[Math.floor(next(game) * list.length)]!;
function weighted<T>(game: Game, entries: readonly (readonly [T, number])[]): T {
  const total = entries.reduce((sum, [, w]) => sum + w, 0);
  let roll = next(game) * total;
  for (const [value, w] of entries) {
    roll -= w;
    if (roll < 0) return value;
  }
  return entries[entries.length - 1]![0];
}
const idRef = (game: Game) => {
  const ref = { value: game.nextId };
  return { ref, commit: () => (game.nextId = ref.value) };
};

// --- Derived values ----------------------------------------------------------------------

export const carLevel = (car: Car) => CARS[car.kind][car.level];
export const findCar = (game: Game, uid: number) => game.cars.find((c) => c.uid === uid);
export const holdOf = (game: Game, id: number) => game.holds.find((h) => h.id === id);
export const count = (game: Game, item: ItemId) => countIn(game.holds, item);
export const commerce = (game: Game) => isCommerce(game.station.kind);
export const region = (game: Game): Region | undefined => (isCommerce(game.station.kind) ? game.station.kind : undefined);

export const seatsByTier = (game: Game): number[] => {
  const tiers = [RULES.headSeats, 0, 0, 0];
  for (const car of game.cars) if (car.kind === 'passenger') tiers[car.level + 1]! += carLevel(car).cap;
  return tiers;
};
export const seats = (game: Game) => seatsByTier(game).reduce((a, b) => a + b, 0);

export const storageCustomers = (game: Game) =>
  game.cars.filter((c) => c.kind === 'storage').reduce((sum, c) => sum + (carLevel(c).customers ?? 0), 0);

/** 当段载荷 Q〔11·01〕：公共物资、自用枪弹、乘客私有物、设备本体、在制品与成品缓存 */
export function load(game: Game): number {
  let q = game.holds.reduce((sum, h) => sum + h.stacks.reduce((a, s) => a + s.qty * ITEMS[s.item].load, 0), 0);
  q += game.pending.reduce((sum, l) => sum + l.qty * ITEMS[l.item].load, 0);
  if (game.kit.gun) q += ITEMS[game.kit.gun].load;
  q += game.kit.ammo * ITEMS.ammo.load;
  q += game.riders.reduce((sum, r) => sum + r.carry, 0);
  for (const car of game.cars) {
    for (const plot of car.plots ?? []) {
      if (plot.ripe) q += PLANT.out * ITEMS.veg.load;
      else if (plot.growing) q += PLANT.seed * ITEMS.seed.load + PLANT.water * ITEMS.water.load;
    }
    for (const d of car.devices ?? []) {
      q += DEVICES[d.kind].load;
      for (const l of d.job?.inputs ?? []) q += l.qty * ITEMS[l.item].load;
      if (d.out) q += d.out.qty * ITEMS[d.out.item].load;
    }
  }
  return Math.round(q * 100) / 100;
}
export const segmentFuel = (game: Game): number => RULES.fuelBase + RULES.fuelPerCar * game.cars.length + RULES.fuelPerLoad * load(game);

const gradeFactor = (game: Game, g: Grade | undefined) => (game.exp && g !== undefined ? GRADE_FACTOR[g] : 1);
/** 零售牌价（每交易单位）〔07·03〕 */
export const retailPrice = (game: Game, item: ItemId, label?: Grade): number =>
  Math.floor(ITEMS[item].value * RULES.retailMarkup * gradeFactor(game, label) + 1e-9);

export const isDueDay = (day: number): boolean => day % RULES.dueEvery === 0 && day / RULES.dueEvery <= RULES.dueCumulative.length;
export function nextDue(day: number): { readonly day: number; readonly required: number } | undefined {
  const k = Math.ceil(day / RULES.dueEvery);
  if (k < 1 || k > RULES.dueCumulative.length) return undefined;
  return { day: k * RULES.dueEvery, required: RULES.dueCumulative[k - 1]! };
}
export const dueAmount = (game: Game, required: number): number => Math.min(game.debt, Math.max(required - game.repaid, 0));
export function debtPreview(game: Game) {
  const due = nextDue(game.day);
  return due ? { ...due, due: dueAmount(game, due.required) } : undefined;
}
export const inventoryValue = (game: Game): number =>
  game.holds.reduce((sum, h) => sum + h.stacks.reduce((a, s) => a + (s.qty / unitOf(s.item)) * ITEMS[s.item].value * gradeFactor(game, s.real), 0), 0);

export const quota = (game: Game) => DRIVERS[game.driver];
export const nodesLeft = (game: Game) => quota(game).nodes - game.nodesToday;
export const routeDone = (game: Game) => !!game.route && game.route.arrived >= game.route.nodes.length;
export const routeActive = (game: Game) => !!game.route && !routeDone(game);
export const canEndDay = (game: Game) => game.phase === 'station' && commerce(game) && game.pending.length === 0;

const newLedger = (wageRate: number): DayLedger => ({
  segments: 0, fuelRaw: 0, retail: 0, tickets: 0, purchases: 0, invest: 0, repaid: 0, delivered: 0, refunds: 0, fines: 0,
  fired: [], worked: [], wageRate,
});

// --- Reputation【补充案】 -----------------------------------------------------------------

const clampRep = (v: number) => Math.max(-100, Math.min(100, Math.round(v * 100) / 100));
function addRep(game: Game, r: Region | undefined, delta: number): void {
  if (!game.exp || !r) return;
  game.rep[r] = clampRep(game.rep[r] + delta);
}
/** 议价成功率（公开）：接受 / 留下 / 走人 */
export function haggleOdds(game: Game, pct: number): { readonly accept: number; readonly stay: number; readonly walk: number } {
  const rep = game.rep[region(game) ?? 'rich'];
  const bonus = rep >= 0 ? rep / 6 : (rep / 6) * 2;
  const a = Math.max(0, Math.min(100, Math.round(100 - 2 * pct + bonus)));
  const b = Math.max(a, Math.min(100, Math.round(100 - 1.2 * pct + bonus)));
  return { accept: a, stay: b - a, walk: 100 - b };
}

// --- Generation --------------------------------------------------------------------------

function pickItem(game: Game, tag: Tag, exclude: readonly ItemId[]): ItemId {
  const pool = ITEM_IDS.filter((id) => ITEMS[id].tag === tag && !exclude.includes(id)).map((id) => [id, ITEMS[id].weight] as const);
  return pool.length ? weighted(game, pool) : pick(game, ITEM_IDS.filter((id) => !exclude.includes(id)));
}
const pickTag = (game: Game, r: Region, side: 'buy' | 'supply'): Tag =>
  weighted(game, (Object.keys(TAG_WEIGHTS[r][side]) as Tag[]).map((tag) => [tag, TAG_WEIGHTS[r][side][tag]] as const));

function grades(game: Game, r: Region, item: ItemId): { label?: Grade; real?: Grade } {
  if (!game.exp || !ITEMS[item].graded) return {};
  const label = (next(game) < 0.2 ? 2 : 1) as Grade;
  if (next(game) >= EXP.fakeChance[r]) return { label, real: label };
  const real = Math.max(0, label - int(game, 1, 2)) as Grade;
  return { label, real };
}
const supplyUnit = (game: Game, item: ItemId, g: Grade | undefined) =>
  Math.ceil(ITEMS[item].value * gradeFactor(game, g) * pick(game, RULES.supplyFactors) - 1e-9);

function supplier(game: Game, r: Region, car: number, forced?: readonly (readonly [ItemId, number])[]): Customer {
  const goods: Goods[] = [];
  const add = (item: ItemId, qty: number) => {
    const g = grades(game, r, item);
    goods.push({ item, qty, unit: supplyUnit(game, item, g.label), ...g });
  };
  let arms = false;
  if (forced) for (const [item, qty] of forced) add(item, qty);
  else {
    const tag = pickTag(game, r, 'supply');
    if (tag === 'arms') {
      arms = true;
      if (next(game) < 0.6) add(next(game) < 0.85 ? 'pistol' : 'smg', 1);
      const packs = int(game, goods.length ? 0 : 1, 3);
      if (packs > 0) add('ammo', packs);
    } else {
      const kinds = next(game) < 0.7 ? 1 : 2;
      for (let k = 0; k < kinds; k += 1) {
        const item = pickItem(game, k === 0 ? tag : pickTag(game, r, 'supply'), goods.map((x) => x.item));
        const def = ITEMS[item];
        add(item, def.rare ? 1 : item === 'ammo' ? int(game, 1, 3) : def.raw ? int(game, 2, 6) : int(game, 1, 4));
      }
    }
  }
  const batch = next(game) < RULES.batchChance;
  const discount = batch ? weighted(game, [[0.1, 25], [0.15, 50], [0.2, 25]] as const) : undefined;
  const c: Customer = { id: game.nextId++, kind: 'supplier', car, goods, ...(arms ? { arms } : {}), ...(discount !== undefined ? { discount } : {}) };
  repriceBatch(c);
  return c;
}
function repriceBatch(c: Customer): void {
  if (c.discount === undefined || !c.goods) return;
  const total = c.goods.reduce((sum, g) => sum + g.unit * g.qty, 0);
  c.batchPrice = Math.ceil(total * (1 - c.discount) - 1e-9);
}

function buyer(game: Game, r: Region, car: number): Customer {
  const needs: Need[] = [];
  const tag = pickTag(game, r, 'buy');
  const arms = tag === 'arms';
  const kinds = next(game) < 0.65 ? 1 : 2;
  for (let k = 0; k < kinds; k += 1) {
    const item = pickItem(game, k === 0 ? tag : pickTag(game, r, 'buy'), needs.map((n) => n.item));
    needs.push({ item, qty: ITEMS[item].rare ? 1 : int(game, 1, 4), price: retailPrice(game, item, ITEMS[item].graded ? 1 : undefined) });
  }
  const [lo, hi] = arms ? RULES.armsBudget : RULES.buyerBudget;
  let budget = Math.round(int(game, lo, hi) / 5) * 5;
  if (game.exp && game.rep[r] >= EXP.repBudget) budget = Math.round(budget * 1.1);
  return { id: game.nextId++, kind: 'buyer', car, needs, budget, ...(arms ? { arms } : {}) };
}

function customerCount(game: Game, r: Region): number {
  let base = RULES.baseCustomers;
  if (game.exp) {
    if (game.rep[r] >= EXP.repExtraCustomer) base += 1;
    if (game.rep[r] <= EXP.repFewerCustomer) base -= 1;
  }
  return Math.max(2, base + storageCustomers(game));
}

function makeCommerce(game: Game, r: Region, first: boolean): Station {
  const total = customerCount(game, r);
  // 基础名额归火车头货舱，各存储车厢按等级接待新增名额
  const slots: number[] = [];
  const storages = game.cars.filter((c) => c.kind === 'storage');
  const extra = storages.reduce((sum, c) => sum + (carLevel(c).customers ?? 0), 0);
  for (let i = 0; i < total - extra; i += 1) slots.push(0);
  for (const car of storages) for (let i = 0; i < (carLevel(car).customers ?? 0); i += 1) slots.push(car.uid);
  const customers: Customer[] = [];
  const shortage: [ItemId, number][] = [];
  if (game.short.water >= RULES.shortageAfter) shortage.push(['water', 2]);
  if (game.short.ration >= RULES.shortageAfter) shortage.push(['ration', 1]);
  slots.forEach((car, i) => {
    if (i === 0) customers.push(supplier(game, r, car, first ? [['water', 2], ['ration', 1]] : shortage.length ? shortage : undefined));
    else if (i === 1) customers.push(buyer(game, r, car));
    else customers.push(next(game) < 0.5 ? supplier(game, r, car) : buyer(game, r, car));
  });
  const has = (item: ItemId) => customers.some((c) => c.goods?.some((g) => g.item === item));
  game.short.water = has('water') ? 0 : game.short.water + 1;
  game.short.ration = has('ration') ? 0 : game.short.ration + 1;
  const pool = PASSENGER_POOL[r];
  const passengers: Candidate[] = [];
  for (let i = 0, n = int(game, pool.n[0], pool.n[1]); i < n; i += 1) passengers.push({ id: game.nextId++, noble: next(game) < pool.noble });
  const driver = SERVICES[r].hire && (first || next(game) < DRIVER_CHANCE)
    ? { grade: (first ? 1 : weighted(game, DRIVER_ODDS)) as 1 | 2 | 3 }
    : undefined;
  game.stats.stations += 1;
  return {
    id: game.nextId++,
    kind: r,
    place: int(game, 0, PLACE_COUNT[r] - 1),
    customers,
    passengers,
    ...(driver ? { driver } : {}),
    inspections: EXP.inspections,
  };
}

function makeSite(game: Game, node: RouteNode): Station {
  const area = node.kind === 'hunt' ? 'hunt' : 'resource';
  return {
    id: game.nextId++,
    kind: node.kind,
    place: int(game, 0, PLACE_COUNT[node.kind] - 1),
    customers: [],
    passengers: [],
    inspections: 0,
    site: { seed: (next(game) * 2 ** 31) | 0, area, theme: node.theme ?? 'hunt', sorties: 0, closed: false, map: emptyMapState(), recovery: false },
  };
}

function makeRouteOptions(game: Game): RouteNode[][] | undefined {
  const left = nodesLeft(game);
  if (left < 2 || game.linesToday >= quota(game).lines) return undefined;
  const themes: Theme[] = ['yard', 'waterworks', 'supply'];
  const node = (k: NodeKind): RouteNode => (k === 'resource' ? { kind: k, theme: pick(game, themes) } : k === 'hunt' ? { kind: k, theme: 'hunt' } : { kind: k });
  const pure = (t: readonly NodeKind[]) => t.every(isCommerce);
  const two = ROUTE_TEMPLATES.filter((t) => t.length === 2);
  const three = ROUTE_TEMPLATES.filter((t) => t.length === 3);
  const chosen: (readonly NodeKind[])[] = [];
  const take = (list: readonly (readonly NodeKind[])[], pred: (t: readonly NodeKind[]) => boolean = () => true) => {
    const cands = list.filter((t) => pred(t) && !chosen.includes(t));
    if (cands.length) chosen.push(pick(game, cands));
  };
  if (left >= 3) {
    take(two);
    take(three);
    if (!chosen.some(pure)) take([...two, ...three], pure);
    else take([...two, ...three]);
  } else {
    take(two, pure);
    take(two);
    take(two);
  }
  return chosen.sort((a, b) => a.length - b.length).map((t) => t.map(node));
}

/** 为当前站候车乘客绑定目的地〔05·03/03.01〕：只选锁定路线中未到达的商业站 */
function bindPassengers(game: Game): void {
  const route = game.route;
  if (!route || routeDone(game) || !commerce(game)) return;
  const here = route.arrived;
  const targets: number[] = [];
  route.nodes.forEach((n, i) => {
    if (i + 1 > here && isCommerce(n.kind)) targets.push(i + 1);
  });
  if (!targets.length) return;
  for (const p of game.station.passengers) {
    if (p.dest !== undefined || p.status) continue;
    const dest = pick(game, targets);
    const seg = dest - here;
    const [lo, hi] = FARE[p.noble ? 'noble' : 'normal'];
    const [clo, chi] = COMP[p.noble ? 'noble' : 'normal'];
    p.dest = dest;
    p.seg = seg;
    p.fare = int(game, lo, hi) * seg;
    p.deadline = game.day + (next(game) < 0.5 ? 0 : 1);
    p.comp = Math.round((clo + next(game) * (chi - clo)) * 100) / 100;
  }
}

// --- Lifecycle ---------------------------------------------------------------------------

export function newGame(seed: number, experimental = true): Game {
  const game: Game = {
    version: SAVE_VERSION,
    seed,
    exp: experimental,
    rng: seed | 0,
    day: 1,
    phase: 'station',
    cash: RULES.startCash,
    debt: RULES.principal,
    repaid: 0,
    holds: [{ id: 0, w: RULES.headW, h: RULES.headH, stacks: [] }],
    cars: [],
    kit: { gun: 'pistol', ammo: RULES.startAmmo },
    hp: RULES.hpMax,
    driver: 0,
    nodesToday: 0,
    linesToday: 0,
    station: undefined as unknown as Station,
    nextId: 1,
    riders: [],
    pending: [],
    today: newLedger(DRIVERS[0].wage),
    cleared: false,
    short: { water: 0, ration: 0 },
    rep: { rich: 0, slum: 0 },
    cheats: { rich: 0, slum: 0 },
    audit: { rich: false, slum: false },
    stats: { revenue: 0, tickets: 0, passengers: 0, stations: 0, sorties: 0, extracted: 0, deaths: 0 },
  };
  game.station = makeCommerce(game, 'rich', true);
  game.routeOptions = makeRouteOptions(game);
  return game;
}

export function isGame(value: unknown): value is Game {
  const g = value as Game | null;
  return !!g && typeof g === 'object' && g.version === SAVE_VERSION && typeof g.cash === 'number' && !!g.station && Array.isArray(g.holds);
}

const atStation = (game: Game) => game.phase === 'station';
const trading = (game: Game) => game.phase === 'station' || game.phase === 'settlement';
const canPay = (game: Game, amount: number): ActionError | undefined =>
  game.cash < 0 ? 'negative-cash' : game.cash < amount ? 'cash' : undefined;

/** 选线：从候选中锁定 1 条，整条须在今日剩余额度内〔06·02〕 */
export function chooseRoute(game: Game, index: number): ActionResult {
  if (!atStation(game) || routeActive(game)) return fail(routeActive(game) ? 'route-left' : 'phase');
  if (game.pending.length) return fail('pending');
  const option = game.routeOptions?.[index];
  if (!option) return fail('not-found');
  if (option.length > nodesLeft(game)) return fail('quota');
  if (game.linesToday >= quota(game).lines) return fail('lines');
  game.route = { nodes: option, arrived: 0 };
  game.linesToday += 1;
  delete game.routeOptions;
  bindPassengers(game);
  return success();
}

// --- Trade -------------------------------------------------------------------------------

export const findCustomer = (game: Game, id: number) => game.station.customers.find((c) => c.id === id);

/** 进货入库顺序：接待车厢的仓优先 */
const intakeOrder = (game: Game, car: number) => [car, 0, ...game.holds.map((h) => h.id)];

function commitPurchase(game: Game, c: Customer, goods: readonly Goods[], total: number): ActionResult {
  const err = canPay(game, total);
  if (err) return fail(err);
  const ids = idRef(game);
  const lots: Lot[] = goods.filter((g) => g.qty > 0).map((g) => ({ item: g.item, qty: g.qty * unitOf(g.item), ...(g.label !== undefined ? { label: g.label, real: g.real ?? g.label } : {}) }));
  const holds = placeLots(game.holds, lots, ids.ref, intakeOrder(game, c.car));
  if (!holds) return fail('space');
  ids.commit();
  game.holds = holds;
  game.cash -= total;
  game.today.purchases += total;
  c.outcome = 'deal';
  c.amount = -total;
  const value = goods.reduce((sum, g) => sum + g.qty * ITEMS[g.item].value, 0);
  addRep(game, region(game), c.haggle?.result === 'ok' ? EXP.greedRep[c.haggle.pct] ?? 0 : value / 100);
  return success(total);
}

export function buySingle(game: Game, id: number, quantities: readonly number[]): ActionResult {
  if (game.phase !== 'station') return fail('phase');
  const c = findCustomer(game, id);
  if (!c || c.kind !== 'supplier' || !c.goods) return fail('not-found');
  if (c.outcome) return fail('done');
  const picked = c.goods.map((g, i) => ({ ...g, qty: Math.max(0, Math.min(g.qty, Math.floor(quantities[i] ?? 0))) }));
  const total = picked.reduce((sum, g) => sum + g.qty * g.unit, 0);
  if (total <= 0) return fail('amount');
  return commitPurchase(game, c, picked, total);
}

export function buyBatch(game: Game, id: number): ActionResult {
  if (game.phase !== 'station') return fail('phase');
  const c = findCustomer(game, id);
  if (!c || c.kind !== 'supplier' || !c.goods || c.batchPrice === undefined) return fail('not-found');
  if (c.outcome) return fail('done');
  return commitPurchase(game, c, c.goods, c.batchPrice);
}

export interface SaleLine {
  readonly item: ItemId;
  /** 基础单位数量 */
  readonly qty: number;
  readonly label?: Grade;
  readonly real?: Grade;
  readonly price: number;
}
/** 预览卖给采购客：按需求顺序逐类，min(需求、库存、预算能买的量)〔07·03〕；同类先卖标称品质高的 */
export function previewSale(game: Game, c: Customer): { readonly lines: readonly SaleLine[]; readonly total: number; readonly fake: boolean } {
  let budget = c.budget ?? 0;
  const lines: SaleLine[] = [];
  const markup = c.haggle?.result === 'ok' && c.kind === 'buyer' ? 1 + c.haggle.pct / 100 : 1;
  for (const need of c.needs ?? []) {
    const unit = unitOf(need.item);
    let want = need.qty * unit;
    const stacks = game.holds.flatMap((h) => h.stacks).filter((s) => s.item === need.item).sort((a, b) => (b.label ?? 1) - (a.label ?? 1));
    for (const s of stacks) {
      if (want <= 0 || budget <= 0) break;
      const price = Math.floor(retailPrice(game, need.item, ITEMS[need.item].graded ? s.label ?? 1 : undefined) * markup + 1e-9);
      const prev = lines.filter((l) => l.item === s.item && l.label === s.label && l.real === s.real).reduce((a, l) => a + l.qty, 0);
      const free = s.qty - prev;
      const affordable = Math.floor((budget * unit) / price);
      const n = Math.max(0, Math.min(want, free, affordable));
      if (n <= 0) continue;
      const cost = Math.max(1, Math.floor((n * price) / unit + 1e-9));
      lines.push({ item: s.item, qty: n, ...(s.label !== undefined ? { label: s.label, real: s.real } : {}), price: cost });
      budget -= cost;
      want -= n;
    }
  }
  const fake = lines.some((l) => (l.label ?? 1) > (l.real ?? 1));
  return { lines, total: lines.reduce((sum, l) => sum + l.price, 0), fake };
}

/** 卖给采购客（负现金也允许卖货）。【补充案】虚标货可能被识破。 */
export function sellToBuyer(game: Game, id: number): ActionResult {
  if (!trading(game)) return fail('phase');
  const c = findCustomer(game, id);
  if (!c || c.kind !== 'buyer') return fail('not-found');
  if (c.outcome) return fail('done');
  const sale = previewSale(game, c);
  if (sale.total <= 0) return fail((c.needs ?? []).some((n) => count(game, n.item) > 0) ? 'budget' : 'no-stock');
  const r = region(game);
  if (game.exp && sale.fake && r && next(game) < EXP.detect[r]) {
    c.outcome = 'caught';
    addRep(game, r, EXP.detectRep);
    return success(0, 'caught');
  }
  let holds = game.holds;
  for (const line of sale.lines) {
    const taken = takeItems(holds, line.item, line.qty, 'high', (s) => s.label === line.label && s.real === line.real);
    if (!taken) return fail('no-stock');
    holds = taken.holds;
  }
  game.holds = holds;
  game.cash += sale.total;
  game.today.retail += sale.total;
  game.stats.revenue += sale.total;
  c.outcome = 'deal';
  c.amount = sale.total;
  const value = sale.lines.reduce((sum, l) => sum + (l.qty / unitOf(l.item)) * ITEMS[l.item].value, 0);
  addRep(game, r, c.haggle?.result === 'ok' ? EXP.greedRep[c.haggle.pct] ?? 0 : value / 100);
  if (game.exp && sale.fake && r) {
    game.cheats[r] += 1;
    if (game.cheats[r] % EXP.cheatsPerInspector === 0) game.audit[r] = true;
    return success(sale.total, 'cheated');
  }
  return success(sale.total);
}

export function passCustomer(game: Game, id: number): ActionResult {
  if (!trading(game)) return fail('phase');
  const c = findCustomer(game, id);
  if (!c) return fail('not-found');
  if (c.outcome) return fail('done');
  c.outcome = 'pass';
  return success();
}

/** 【补充案】议价：对供货客压价、对采购客抬价，每客一次，成功率公开 */
export function haggle(game: Game, id: number, pct: number): ActionResult {
  if (!game.exp) return fail('phase');
  if (game.phase !== 'station') return fail('phase');
  const c = findCustomer(game, id);
  if (!c) return fail('not-found');
  if (c.outcome || c.haggle) return fail('done');
  const odds = haggleOdds(game, pct);
  const roll = next(game) * 100;
  const result = roll < odds.accept ? 'ok' : roll < odds.accept + odds.stay ? 'stay' : 'walk';
  c.haggle = { pct, result };
  if (result === 'walk') {
    c.outcome = 'walk';
    addRep(game, region(game), EXP.walkRep);
  } else if (result === 'ok' && c.kind === 'supplier' && c.goods) {
    for (const g of c.goods) g.unit = Math.max(1, Math.ceil(g.unit * (1 - pct / 100) - 1e-9));
    repriceBatch(c);
  }
  return success(undefined, result);
}

/** 【补充案】验货：揭示该供货客全部货物真实品质，虚标的按真实品质重算单拿价 */
export function inspect(game: Game, id: number): ActionResult {
  if (!game.exp || game.phase !== 'station') return fail('phase');
  const c = findCustomer(game, id);
  if (!c || c.kind !== 'supplier' || !c.goods) return fail('not-found');
  if (c.outcome || c.inspected) return fail('done');
  if (game.station.inspections <= 0) return fail('inspections');
  game.station.inspections -= 1;
  c.inspected = true;
  let caught = 0;
  for (const g of c.goods) {
    if ((g.label ?? 1) > (g.real ?? 1)) {
      caught += 1;
      g.unit = Math.max(1, Math.ceil((g.unit * GRADE_FACTOR[g.real ?? 1]) / GRADE_FACTOR[g.label ?? 1] - 1e-9));
    }
  }
  repriceBatch(c);
  return success(caught, caught ? 'fake' : 'clean');
}

/** 【补充案】兑水：1 净水 + 1 污水 → 2 份「标称良、真实差」的净水 */
export function dilute(game: Game): ActionResult {
  if (!game.exp || !atStation(game)) return fail('phase');
  const w = takeItems(game.holds, 'water', 1, 'low');
  if (!w) return fail('no-stock');
  const s = takeItems(w.holds, 'sewage', 1, 'low');
  if (!s) return fail('no-stock');
  const ids = idRef(game);
  const holds = placeLots(s.holds, [{ item: 'water', qty: 2, label: 1, real: 0 }], ids.ref);
  if (!holds) return fail('space');
  ids.commit();
  game.holds = holds;
  return success(2);
}

/** 【补充案】翻新：把一堆标称「差」的货改贴成「良」（真实品质不变） */
export function relabel(game: Game, stackId: number): ActionResult {
  if (!game.exp || !atStation(game)) return fail('phase');
  const holds = clone(game.holds);
  const found = findStack(holds, stackId);
  if (!found || !ITEMS[found.stack.item].graded) return fail('not-found');
  if ((found.stack.label ?? 1) >= 1) return fail('done');
  found.stack.label = 1;
  found.stack.real = found.stack.real ?? 0;
  game.holds = holds;
  return success();
}

// --- Inventory ---------------------------------------------------------------------------

export function moveItem(game: Game, stackId: number, to: number, x: number, y: number, rot: boolean): ActionResult {
  if (game.phase === 'travel' || game.phase === 'explore') return fail('phase');
  const holds = moveStack(game.holds, stackId, to, x, y, rot);
  if (!holds) return fail('space');
  game.holds = holds;
  return success();
}
export function splitItem(game: Game, stackId: number): ActionResult {
  const ids = idRef(game);
  const holds = splitStack(game.holds, stackId, ids.ref);
  if (!holds) return fail('space');
  ids.commit();
  game.holds = holds;
  return success();
}
/** 自动整理：清空后按物品大小重新摆放并合堆 */
export function tidy(game: Game): ActionResult {
  if (game.phase === 'travel' || game.phase === 'explore') return fail('phase');
  const lots: Lot[] = game.holds.flatMap((h) => h.stacks.map((s) => ({ item: s.item, qty: s.qty, ...(s.label !== undefined ? { label: s.label, real: s.real } : {}) })));
  lots.sort((a, b) => ITEMS[b.item].w * ITEMS[b.item].h - ITEMS[a.item].w * ITEMS[a.item].h || a.item.localeCompare(b.item));
  const empty = game.holds.map((h) => ({ ...h, stacks: [] as Stack[] }));
  const ids = idRef(game);
  const holds = placeLots(empty, lots, ids.ref);
  if (!holds) return fail('space');
  ids.commit();
  game.holds = holds;
  return success();
}
/** 待整理物品放进货舱（指定位置或自动） */
export function placePending(game: Game, index: number, to?: { hold: number; x: number; y: number; rot: boolean }): ActionResult {
  const lot = game.pending[index];
  if (!lot) return fail('not-found');
  const ids = idRef(game);
  let holds: Hold[] | null;
  if (to && lot.qty <= ITEMS[lot.item].stack) {
    holds = clone(game.holds);
    const hold = holds.find((h) => h.id === to.hold);
    if (!hold) return fail('not-found');
    const [w, h] = to.rot ? [ITEMS[lot.item].h, ITEMS[lot.item].w] : [ITEMS[lot.item].w, ITEMS[lot.item].h];
    if (!fits(hold, to.x, to.y, w, h)) return fail('space');
    hold.stacks.push({ id: ids.ref.value++, item: lot.item, qty: lot.qty, x: to.x, y: to.y, rot: to.rot, ...(lot.label !== undefined ? { label: lot.label, real: lot.real ?? lot.label } : {}) });
  } else holds = placeLots(game.holds, [lot], ids.ref);
  if (!holds) return fail('space');
  ids.commit();
  game.holds = holds;
  game.pending.splice(index, 1);
  return success();
}
export function discardPending(game: Game, index: number): ActionResult {
  if (!game.pending[index]) return fail('not-found');
  game.pending.splice(index, 1);
  return success();
}
export function discardStack(game: Game, stackId: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  const holds = clone(game.holds);
  const found = findStack(holds, stackId);
  if (!found) return fail('not-found');
  found.hold.stacks = found.hold.stacks.filter((s) => s.id !== stackId);
  game.holds = holds;
  return success();
}

/** 装备武器：从货舱取一把枪换到自用栏 */
export function equipGun(game: Game, item: 'pistol' | 'smg'): ActionResult {
  if (!atStation(game)) return fail('phase');
  const taken = takeItems(game.holds, item, 1);
  if (!taken) return fail('no-stock');
  let holds = taken.holds;
  if (game.kit.gun) {
    const ids = idRef(game);
    const placed = placeLots(holds, [{ item: game.kit.gun, qty: 1 }], ids.ref);
    if (!placed) return fail('space');
    ids.commit();
    holds = placed;
  }
  game.holds = holds;
  game.kit.gun = item;
  return success();
}
/** 补充弹药：货舱弹药装进自用栏（≤60 发） */
export function loadAmmo(game: Game): ActionResult {
  if (!atStation(game)) return fail('phase');
  const room = RULES.personalAmmoMax - game.kit.ammo;
  const have = count(game, 'ammo');
  const n = Math.min(room, have);
  if (n <= 0) return fail(room <= 0 ? 'done' : 'no-stock');
  const taken = takeItems(game.holds, 'ammo', n)!;
  game.holds = taken.holds;
  game.kit.ammo += n;
  return success(n);
}

// --- Passengers ----------------------------------------------------------------------------

export function boardPassenger(game: Game, id: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  const p = game.station.passengers.find((x) => x.id === id);
  if (!p) return fail('not-found');
  if (p.status) return fail('done');
  if (p.dest === undefined || p.fare === undefined) return fail('unbound');
  const r = region(game);
  if (game.exp && r && game.rep[r] <= EXP.repNoBoard) return fail('rep');
  const tiers = seatsByTier(game);
  const used = [0, 0, 0, 0];
  for (const rider of game.riders) used[rider.seat]! += 1;
  let seat = -1;
  for (let t = 3; t >= 0; t -= 1) if (tiers[t]! > used[t]!) {
    seat = t;
    break;
  }
  if (seat < 0) return fail('seats');
  const odds = SEAT_ODDS[seat]![p.noble ? 'noble' : 'normal'];
  if (next(game) >= odds) {
    p.status = 'declined';
    return fail('declined');
  }
  p.status = 'boarded';
  game.riders.push({ id: p.id, noble: p.noble, dest: p.dest, fare: p.fare, deadline: p.deadline!, comp: p.comp!, seat, carry: 0 });
  game.cash += p.fare;
  game.today.tickets += p.fare;
  game.stats.tickets += p.fare;
  game.stats.passengers += 1;
  return success(p.fare);
}

/** 卖给在车乘客当天的随机需求（1 件，单价 ≤ 50%×B） */
export function sellToRider(game: Game, id: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  const rider = game.riders.find((r) => r.id === id);
  if (!rider?.want || rider.want.sold) return fail('not-found');
  const unit = unitOf(rider.want.item);
  const taken = takeItems(game.holds, rider.want.item, unit, 'high');
  if (!taken) return fail('no-stock');
  game.holds = taken.holds;
  rider.carry += unit * ITEMS[rider.want.item].load;
  rider.want.sold = true;
  game.cash += rider.want.price;
  game.today.retail += rider.want.price;
  game.stats.revenue += rider.want.price;
  return success(rider.want.price);
}

// --- Drivers -------------------------------------------------------------------------------

/** 招聘司机：付费后登记次日上岗〔05·01/01.04〕 */
export function hireDriver(game: Game): ActionResult {
  if (!atStation(game)) return fail('phase');
  const cand = game.station.driver;
  if (!cand || cand.hired) return fail('no-hire');
  if (game.pendingDriver) return fail('done');
  const fee = DRIVERS[cand.grade].hire;
  const err = canPay(game, fee);
  if (err) return fail(err);
  game.cash -= fee;
  game.today.invest += fee;
  game.pendingDriver = cand.grade;
  cand.hired = true;
  return success(fee);
}

// --- Cars ----------------------------------------------------------------------------------

const depotHere = (game: Game) => commerce(game) && SERVICES[game.station.kind as Region].depot;

function payMaterials(game: Game, ingot: number, parts: number): Hold[] | null {
  let holds = game.holds;
  if (ingot) {
    const t = takeItems(holds, 'ingot', ingot);
    if (!t) return null;
    holds = t.holds;
  }
  if (parts) {
    const t = takeItems(holds, 'parts', parts);
    if (!t) return null;
    holds = t.holds;
  }
  return holds;
}

export function buyCar(game: Game, kind: CarKind): ActionResult {
  if (!atStation(game)) return fail('phase');
  if (!depotHere(game)) return fail('no-depot');
  if (game.cars.length >= RULES.maxCars) return fail('max-cars');
  const lv = CARS[kind][0];
  const err = canPay(game, lv.cash);
  if (err) return fail(err);
  const uid = game.nextId++;
  const car: Car = { uid, kind, level: 0, paid: lv.cash };
  if (kind === 'storage') game.holds.push({ id: uid, w: lv.cap, h: STORAGE_H, stacks: [] });
  if (kind === 'planting') car.plots = Array.from({ length: lv.cap }, () => ({ progress: 0, growing: false, ripe: false }));
  if (kind === 'processing') car.devices = [];
  game.cars.push(car);
  game.cash -= lv.cash;
  game.today.invest += lv.cash;
  return success(lv.cash);
}

export function upgradeCar(game: Game, uid: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  if (!depotHere(game)) return fail('no-depot');
  const car = findCar(game, uid);
  if (!car) return fail('not-found');
  if (car.level >= 2) return fail('max-level');
  const lv = CARS[car.kind][car.level + 1]!;
  const err = canPay(game, lv.cash);
  if (err) return fail(err);
  const holds = payMaterials(game, lv.ingot, lv.parts);
  if (!holds) return fail('materials');
  game.holds = holds;
  car.level = (car.level + 1) as 1 | 2;
  car.paid += lv.cash;
  game.cash -= lv.cash;
  game.today.invest += lv.cash;
  if (car.kind === 'storage') holdOf(game, uid)!.w = lv.cap; // 向右扩列，保留原坐标
  if (car.kind === 'planting') while (car.plots!.length < lv.cap) car.plots!.push({ progress: 0, growing: false, ripe: false });
  return success(lv.cash);
}

/** 回收车厢：返还实际已付购建现金的 60%，须先清空〔05·01/01.02〕 */
export function sellCar(game: Game, uid: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  if (!depotHere(game)) return fail('no-depot');
  const car = findCar(game, uid);
  if (!car) return fail('not-found');
  if (car.kind === 'storage' && holdOf(game, uid)!.stacks.length) return fail('not-empty');
  if (car.kind === 'planting' && car.plots!.some((p) => p.growing || p.ripe)) return fail('not-empty');
  if (car.kind === 'processing' && car.devices!.length) return fail('not-empty');
  if (car.kind === 'passenger') {
    const tiers = seatsByTier(game);
    tiers[car.level + 1]! -= carLevel(car).cap;
    const used = game.riders.filter((r) => r.seat === car.level + 1).length;
    if (used > tiers[car.level + 1]!) return fail('not-empty');
  }
  if (car.kind === 'storage' && game.station.customers.some((c) => c.car === uid && !c.outcome)) return fail('busy');
  const refund = Math.floor(car.paid * RULES.carRefund);
  game.cars = game.cars.filter((c) => c.uid !== uid);
  game.holds = game.holds.filter((h) => h.id !== uid);
  game.cash += refund;
  game.today.invest -= refund;
  return success(refund);
}

// --- Production ----------------------------------------------------------------------------

export function plant(game: Game, uid: number, index: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  const plot = findCar(game, uid)?.plots?.[index];
  if (!plot) return fail('not-found');
  if (plot.growing || plot.ripe) return fail('busy');
  const seeds = takeItems(game.holds, 'seed', PLANT.seed);
  if (!seeds) return fail('materials');
  const water = takeItems(seeds.holds, 'water', PLANT.water);
  if (!water) return fail('materials');
  game.holds = water.holds;
  plot.growing = true;
  plot.progress = 0;
  return success();
}

export function harvest(game: Game, uid: number, index: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  const plot = findCar(game, uid)?.plots?.[index];
  if (!plot) return fail('not-found');
  if (!plot.ripe) return fail('busy');
  const ids = idRef(game);
  const holds = placeLots(game.holds, [{ item: 'veg', qty: PLANT.out }], ids.ref);
  if (!holds) return fail('space');
  ids.commit();
  game.holds = holds;
  plot.ripe = false;
  plot.growing = false;
  plot.progress = 0;
  return success(PLANT.out);
}

export const slotsUsed = (car: Car) => (car.devices ?? []).reduce((sum, d) => sum + DEVICES[d.kind].slots, 0);

export function installDevice(game: Game, uid: number, kind: DeviceKind): ActionResult {
  if (!atStation(game)) return fail('phase');
  const car = findCar(game, uid);
  if (!car || car.kind !== 'processing') return fail('not-found');
  if (slotsUsed(car) + DEVICES[kind].slots > carLevel(car).cap) return fail('slots');
  const err = canPay(game, DEVICES[kind].cash);
  if (err) return fail(err);
  game.cash -= DEVICES[kind].cash;
  game.today.invest += DEVICES[kind].cash;
  car.devices!.push({ uid: game.nextId++, kind });
  return success(DEVICES[kind].cash);
}

const findDevice = (game: Game, uid: number) => {
  for (const car of game.cars) {
    const d = car.devices?.find((x) => x.uid === uid);
    if (d) return d;
  }
  return undefined;
};

export function startJob(game: Game, deviceUid: number, recipeId: string): ActionResult {
  if (!atStation(game)) return fail('phase');
  const d = findDevice(game, deviceUid);
  const recipe = RECIPES.find((r) => r.id === recipeId);
  if (!d || !recipe || recipe.device !== d.kind) return fail('not-found');
  if (d.job || d.out) return fail('busy');
  let holds = game.holds;
  const inputs: Lot[] = [];
  for (const [item, qty] of recipe.inputs) {
    const t = takeItems(holds, item, qty);
    if (!t) return fail('materials');
    holds = t.holds;
    inputs.push(...t.taken);
  }
  game.holds = holds;
  d.job = { recipe: recipe.id, progress: 0, inputs };
  return success();
}

export function collectOutput(game: Game, deviceUid: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  const d = findDevice(game, deviceUid);
  if (!d?.out) return fail('not-found');
  const ids = idRef(game);
  const holds = placeLots(game.holds, [d.out], ids.ref);
  if (!holds) return fail('space');
  ids.commit();
  game.holds = holds;
  const qty = d.out.qty;
  delete d.out;
  return success(qty);
}

/** 每段真实车程推进一次工作量〔05·01/01.01〕 */
function advanceProduction(game: Game): void {
  for (const car of game.cars) {
    for (const plot of car.plots ?? []) {
      if (!plot.growing || plot.ripe) continue;
      plot.progress += RULES.workPerSegment;
      if (plot.progress >= PLANT.work) plot.ripe = true;
    }
    for (const d of car.devices ?? []) {
      if (!d.job) continue;
      const recipe = RECIPES.find((r) => r.id === d.job!.recipe)!;
      d.job.progress += RULES.workPerSegment;
      if (!game.today.worked.includes(d.uid)) game.today.worked.push(d.uid);
      if (d.job.progress >= recipe.work) {
        d.out = { item: recipe.out, qty: recipe.qty, ...(ITEMS[recipe.out].graded ? { label: 1 as Grade, real: 1 as Grade } : {}) };
        delete d.job;
      }
    }
  }
}

// --- Debt ----------------------------------------------------------------------------------

export function repay(game: Game, amount: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  const value = Math.floor(Math.min(amount, game.debt));
  if (value <= 0) return fail('amount');
  const err = canPay(game, value);
  if (err) return fail(err);
  payPrincipal(game, value);
  checkCleared(game);
  return success(value);
}
function payPrincipal(game: Game, value: number): void {
  game.cash -= value;
  game.debt -= value;
  game.repaid += value;
  game.today.repaid += value;
}
function checkCleared(game: Game): void {
  if (game.debt > 0 || game.cleared || game.riders.length > 0 || game.cash < 0) return;
  game.cleared = true;
  game.resume = game.phase;
  game.phase = 'won';
}

// --- Travel --------------------------------------------------------------------------------

/** 发车：锁定本段 N、Q，记一段燃油〔11·01〕 */
export function depart(game: Game): ActionResult {
  if (!atStation(game)) return fail('phase');
  if (!routeActive(game)) return fail('unbound');
  if (game.pending.length) return fail('pending');
  if (game.station.site?.active) return fail('explore');
  if (nodesLeft(game) <= 0) return fail('quota');
  game.today.segments += 1;
  game.today.fuelRaw += segmentFuel(game);
  game.phase = 'travel';
  return success();
}

/** 抵达：推进生产、下客、生成新站点、乘客随机购物、稽查【补充案】 */
export function arrive(game: Game): ActionResult {
  if (game.phase !== 'travel' || !game.route) return fail('phase');
  const route = game.route;
  route.arrived += 1;
  game.nodesToday += 1;
  game.hp = Math.min(RULES.hpMax, game.hp + RULES.hpPerSegment);
  advanceProduction(game);
  const here = route.arrived;
  const node = route.nodes[here - 1]!;
  const delivered = isCommerce(node.kind) ? game.riders.filter((r) => r.dest === here) : [];
  game.riders = game.riders.filter((r) => !delivered.includes(r));
  game.today.delivered += delivered.length;
  game.station = isCommerce(node.kind) ? makeCommerce(game, node.kind, false) : makeSite(game, node);
  for (const rider of game.riders) {
    if (rider.want) continue;
    if (next(game) < RULES.riderShopChance[rider.noble ? 'noble' : 'normal']) {
      const tag = weighted(game, RIDER_WANT_TAGS[rider.noble ? 'noble' : 'normal']);
      const items = ITEM_IDS.filter((id) => ITEMS[id].tag === tag && retailPrice(game, id, ITEMS[id].graded ? 1 : undefined) <= RULES.riderBase / 2);
      if (items.length) {
        const item = pick(game, items);
        rider.want = { item, price: retailPrice(game, item, ITEMS[item].graded ? 1 : undefined) };
      }
    }
  }
  // 【补充案】坑客累计后，下一个该地区商业站有稽查员上门
  if (game.exp && isCommerce(node.kind) && game.audit[node.kind]) {
    game.audit[node.kind] = false;
    let seized = 0;
    const holds = clone(game.holds);
    for (const h of holds) {
      const bad = h.stacks.filter((s) => (s.label ?? 1) > (s.real ?? 1));
      seized += bad.reduce((sum, s) => sum + s.qty, 0);
      h.stacks = h.stacks.filter((s) => !bad.includes(s));
    }
    const fined = seized > 0 ? EXP.inspectorFine : 0;
    game.holds = holds;
    game.cash -= fined;
    game.today.fines += fined;
    game.station.inspector = { fined, seized };
  }
  bindPassengers(game);
  if (routeDone(game)) game.routeOptions = makeRouteOptions(game);
  game.phase = 'station';
  return success(delivered.length);
}

// --- Exploration ---------------------------------------------------------------------------

export interface SortieInit {
  readonly seed: number;
  readonly area: 'resource' | 'hunt';
  readonly theme: Theme;
  readonly map: MapState;
  readonly recovery: boolean;
  readonly lost?: LostBag;
  readonly hp: number;
  readonly gun: 'pistol' | 'smg' | null;
  readonly ammo: number;
  readonly backpack: Lot[];
  readonly exp: boolean;
}

/** 出击：从真实库存带绷带进背包；自用枪弹随身〔08·01〕 */
export function startSortie(game: Game, bandages: number): ActionResult {
  if (!atStation(game)) return fail('phase');
  const site = game.station.site;
  if (!site) return fail('not-commerce');
  if (site.closed) return fail('closed');
  const recovery = site.sorties > 0;
  if (recovery && (!site.recovery || !site.lost)) return fail('recovery');
  if (game.pending.length) return fail('pending');
  let taken: Lot[] = [];
  if (bandages > 0) {
    const t = takeItems(game.holds, 'bandage', bandages);
    if (!t) return fail('no-stock');
    game.holds = t.holds;
    taken = t.taken;
  }
  site.sorties += 1;
  if (recovery) site.recovery = false;
  site.active = { recovery, bandages: taken };
  game.phase = 'explore';
  game.stats.sorties += 1;
  return success();
}

export function sortieInit(game: Game): SortieInit | undefined {
  const site = game.station.site;
  if (!site?.active) return undefined;
  return {
    seed: site.seed,
    area: site.area,
    theme: site.theme,
    map: clone(site.map),
    recovery: site.active.recovery,
    ...(site.active.recovery && site.lost ? { lost: clone(site.lost) } : {}),
    hp: game.hp,
    gun: game.kit.gun,
    ammo: game.kit.ammo,
    backpack: clone(site.active.bandages),
    exp: game.exp,
  };
}

export interface SortieResult {
  readonly outcome: 'extract' | 'death' | 'abandon';
  readonly hp: number;
  readonly gun: 'pistol' | 'smg' | null;
  readonly ammo: number;
  readonly backpack: readonly Lot[];
  readonly map: MapState;
  readonly fired: boolean;
  /** 死亡位置 */
  readonly x?: number;
  readonly y?: number;
  /** 回收局中已从遗失包取回（遗失包剩余） */
  readonly lostLeft?: LostBag;
}

/** 结束出击：撤离入库 / 死亡留下遗失包 / 主动放弃〔08·01〕〔08·02〕 */
export function endSortie(game: Game, result: SortieResult): ActionResult {
  if (game.phase !== 'explore') return fail('phase');
  const site = game.station.site!;
  const wasRecovery = site.active?.recovery ?? false;
  delete site.active;
  site.map = clone(result.map);
  if (result.fired && result.gun && !game.today.fired.includes(result.gun)) game.today.fired.push(result.gun);
  game.phase = 'station';
  if (result.outcome === 'extract') {
    game.stats.extracted += 1;
    game.hp = Math.max(1, Math.round(result.hp));
    game.kit.gun = result.gun;
    game.kit.ammo = Math.min(RULES.personalAmmoMax, result.ammo);
    const lots: Lot[] = result.backpack.map((l) => clone(l));
    const extraAmmo = result.ammo - game.kit.ammo;
    if (extraAmmo > 0) lots.push({ item: 'ammo', qty: extraAmmo });
    if (wasRecovery) delete site.lost;
    site.closed = true;
    const ids = idRef(game);
    // 能放的放，放不下的进「待整理」
    for (const lot of lots) {
      const placed = placeLots(game.holds, [lot], ids.ref);
      if (placed) game.holds = placed;
      else game.pending.push(lot);
    }
    ids.commit();
    return success(lots.length);
  }
  game.hp = RULES.hpAfterDeath;
  if (result.outcome === 'death') {
    game.stats.deaths += 1;
    game.kit.gun = null;
    game.kit.ammo = 0;
    if (wasRecovery) {
      // 第二次失败：立即失去两次未带出的物品，无第三次
      delete site.lost;
      site.closed = true;
      return success(0, 'lost-all');
    }
    site.lost = { gun: result.gun, ammo: result.ammo, items: result.backpack.map((l) => clone(l)), x: result.x ?? 0, y: result.y ?? 0 };
    site.recovery = true;
    return success(0, 'lost-bag');
  }
  // 主动放弃：全部携物失去，无回收
  game.kit.gun = null;
  game.kit.ammo = 0;
  delete site.lost;
  site.closed = true;
  return success(0, 'abandon');
}

/** 不出击（或放弃回收）：关闭本节点 */
export function skipSite(game: Game): ActionResult {
  const site = game.station.site;
  if (!site || site.active) return fail('phase');
  site.closed = true;
  site.recovery = false;
  delete site.lost;
  return success();
}

// --- Day end -------------------------------------------------------------------------------

/** 收车：先下客与逾期退赔，再冻结在车人数、扣日末费用〔11·04〕 */
export function endDay(game: Game): ActionResult {
  if (!canEndDay(game)) return fail(game.pending.length ? 'pending' : commerce(game) ? 'phase' : 'not-commerce');
  const t = game.today;
  // 逾期：截止日为今天且未送达 → 退全款 + 赔偿
  let overdue = 0;
  const kept: Rider[] = [];
  for (const r of game.riders) {
    if (r.deadline <= game.day) {
      const refund = r.fare + Math.ceil(r.fare * r.comp - 1e-9);
      game.cash -= refund;
      t.refunds += refund;
      overdue += 1;
    } else kept.push(r);
  }
  game.riders = kept;
  const normalRiders = game.riders.filter((r) => !r.noble).length;
  const nobles = game.riders.filter((r) => r.noble).length;
  const crew = 2 + (game.pendingDriver ? 1 : 0);
  const living = RULES.livingPerHead * (crew + normalRiders) + RULES.livingNoble * nobles;
  const fuel = Math.ceil(t.fuelRaw - 1e-9);
  const wages = t.wageRate * t.segments;
  let upkeep = 0;
  for (const car of game.cars) for (const d of car.devices ?? []) if (t.worked.includes(d.uid)) upkeep += DEVICES[d.kind].upkeep;
  for (const gun of t.fired) upkeep += RULES.gunUpkeep[gun];
  const total = fuel + wages + living + upkeep;
  const before = game.cash + t.refunds;
  game.cash -= total;
  for (const rider of game.riders) delete rider.want;
  game.bill = {
    day: game.day, segments: t.segments, fuel, wages, living, upkeep, refunds: t.refunds, total: total + t.refunds,
    cashBefore: before, cashAfter: game.cash, ledger: clone(t), heads: { normal: crew + normalRiders, noble: nobles }, overdue,
  };
  game.phase = 'dayEnd';
  return success(total);
}

export function closeBill(game: Game): ActionResult {
  if (game.phase !== 'dayEnd') return fail('phase');
  if (isDueDay(game.day) && !(game.debt === 0 && game.cash >= 0)) {
    const required = RULES.dueCumulative[game.day / RULES.dueEvery - 1]!;
    game.settlement = { day: game.day, required, due: dueAmount(game, required) };
    game.phase = 'settlement';
    return success();
  }
  startNextDay(game);
  return success();
}

/** 大结算：付 D，需 C≥D 且付后非负 */
export function paySettlement(game: Game): ActionResult {
  if (game.phase !== 'settlement' || !game.settlement) return fail('phase');
  const due = dueAmount(game, game.settlement.required);
  if (game.cash < due || game.cash < 0) return fail('cash');
  if (due > 0) payPrincipal(game, due);
  delete game.settlement;
  startNextDay(game);
  if (game.debt === 0 && !game.cleared && game.riders.length === 0) {
    game.cleared = true;
    game.resume = 'station';
    game.phase = 'won';
  }
  return success(due);
}

export function forfeit(game: Game): ActionResult {
  if (game.phase !== 'settlement') return fail('phase');
  game.phase = 'lost';
  game.lostReason = 'debt';
  return success();
}

export function continueAfterWin(game: Game): ActionResult {
  if (game.phase !== 'won') return fail('phase');
  game.phase = game.resume ?? 'station';
  delete game.resume;
  return success();
}

function startNextDay(game: Game): void {
  game.day += 1;
  if (game.pendingDriver) {
    game.driver = game.pendingDriver;
    delete game.pendingDriver;
  }
  game.nodesToday = 0;
  game.linesToday = 0;
  game.today = newLedger(DRIVERS[game.driver].wage);
  delete game.bill;
  // 过夜不刷新同站顾客与乘客；未登车候选等待新路线绑定
  for (const p of game.station.passengers) {
    if (p.status) continue;
    delete p.dest;
    delete p.seg;
    delete p.fare;
    delete p.deadline;
    delete p.comp;
  }
  if (routeActive(game)) {
    // 次日优先续线：占 1 条线路，只扣实际剩余段
    game.linesToday = 1;
    delete game.routeOptions;
  } else {
    delete game.route;
    game.routeOptions = makeRouteOptions(game);
  }
  game.phase = 'station';
}

export const wantsRoute = (game: Game) => game.phase === 'station' && !routeActive(game) && !!game.routeOptions?.length;
