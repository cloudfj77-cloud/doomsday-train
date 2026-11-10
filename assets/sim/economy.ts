/**
 * 《末日列车》经营核心：跑线 → 买卖 → 日结 → 还款。
 * Pure, deterministic (seeded RNG stored in state) and JSON-serialisable, so the
 * whole game is one autosave object. No DOM, no engine imports.
 */
import { TRAIN_EXIT, walkSite, canExtract } from '../shared/station-map.ts';
import {
  CARS,
  ITEM_IDS,
  ITEMS,
  PLACE_COUNT,
  RIDER_WANTS,
  RULES,
  TAG_WEIGHTS,
  type CarKind,
  type ItemId,
  type Region,
  type Tag,
} from './data.ts';

export const SAVE_VERSION = 1;

export type Phase = 'exploration' | 'depot' | 'travel' | 'station' | 'dayEnd' | 'settlement' | 'won' | 'lost';

export interface Goods {
  readonly item: ItemId;
  readonly qty: number;
  /** 单拿单价（锁定） */
  readonly unit: number;
}
export interface Need {
  readonly item: ItemId;
  readonly qty: number;
  /** 本地零售牌价（锁定） */
  readonly price: number;
}
export interface Customer {
  readonly id: number;
  readonly kind: 'supplier' | 'buyer';
  readonly goods?: readonly Goods[];
  /** 整批折扣（0.1/0.15/0.2），无整批资格为 undefined */
  readonly discount?: number;
  readonly batchPrice?: number;
  readonly needs?: readonly Need[];
  readonly budget?: number;
  /** 'deal' 成交离开；'pass' 被拒绝；undefined 仍在等待 */
  outcome?: 'deal' | 'pass';
  /** 成交金额（买入为负，卖出为正） */
  amount?: number;
}
export interface PassengerCandidate {
  readonly id: number;
  /** 绑定后的目的节点（路线节点下标，1 起）；未绑定为 undefined */
  dest?: number;
  fare?: number;
  /** 出发站在路线上的节点下标（0＝当天起点站） */
  from?: number;
  boarded?: boolean;
  /** S0 前 2 名保证去下一站 */
  readonly guaranteedNext?: boolean;
}
export interface Rider {
  readonly id: number;
  readonly dest: number;
  readonly fare: number;
  /** 当天随机购物需求（锁定价格） */
  want?: { readonly item: ItemId; readonly price: number; sold?: boolean };
  /** 已售给乘客的私有物（计载荷） */
  carryLoad: number;
}
export interface Station {
  readonly id: number;
  readonly region: Region;
  readonly place: number;
  /** 行情：当地参考价倍率（仅单一物品） */
  readonly event?: MarketEvent;
  customers: Customer[];
  passengers: PassengerCandidate[];
}
export interface MarketEvent {
  readonly kind: 'up' | 'down';
  readonly item: ItemId;
}
export interface RouteOption {
  readonly nodes: readonly Region[];
}
export interface Route {
  readonly nodes: readonly Region[];
  /** 已抵达节点数（0＝仍在起点站） */
  arrived: number;
}
export interface DayLedger {
  segments: number;
  fuelRaw: number;
  retail: number;
  tickets: number;
  purchases: number;
  cars: number;
  repaid: number;
  delivered: number;
}
export interface Bill {
  readonly day: number;
  readonly segments: number;
  readonly fuel: number;
  readonly wages: number;
  readonly living: number;
  readonly total: number;
  readonly cashBefore: number;
  readonly cashAfter: number;
  readonly ledger: DayLedger;
}
export interface Settlement {
  readonly day: number;
  /** 本期累计至少已还 */
  readonly required: number;
  /** 应补本金 D */
  readonly due: number;
}

export interface Game {
  readonly version: number;
  readonly seed: number;
  rng: number;
  day: number;
  phase: Phase;
  /** won 弹窗关闭后回到的阶段 */
  resume?: Phase;
  cash: number;
  /** 剩余本金 */
  debt: number;
  /** 累计已还本金 */
  repaid: number;
  cars: CarKind[];
  inventory: Record<ItemId, number>;
  station: Station;
  expedition?: { stationId: number; status: 'active' | 'returned' | 'skipped'; x: number; y: number; mapVersion?: number; surveyed?: string[] };
  nextId: number;
  event?: MarketEvent;
  routeOptions?: RouteOption[];
  route?: Route;
  riders: Rider[];
  today: DayLedger;
  bill?: Bill;
  settlement?: Settlement;
  /** 清债软胜利已出现过 */
  cleared: boolean;
  /** 失败原因 */
  lostReason?: 'debt';
  stats: { revenue: number; tickets: number; passengers: number; stations: number };
}

export type ActionError =
  | 'phase'
  | 'cash'
  | 'negative-cash'
  | 'space'
  | 'seats'
  | 'no-stock'
  | 'budget'
  | 'done'
  | 'unbound'
  | 'max-cars'
  | 'amount'
  | 'not-found';
export type ActionResult = { readonly ok: true; readonly amount?: number } | { readonly ok: false; readonly code: ActionError };
const fail = (code: ActionError): ActionResult => ({ ok: false, code });
const success = (amount?: number): ActionResult => (amount === undefined ? { ok: true } : { ok: true, amount });

// --- RNG (mulberry32, state kept in the save) ------------------------------------------

function next(game: Game): number {
  let t = (game.rng = (game.rng + 0x6d2b79f5) | 0);
  t = Math.imul(t ^ (t >>> 15), t | 1);
  t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
  return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}
const int = (game: Game, min: number, max: number) => min + Math.floor(next(game) * (max - min + 1));
function pick<T>(game: Game, list: readonly T[]): T {
  return list[Math.floor(next(game) * list.length)]!;
}
function weighted<T>(game: Game, entries: readonly (readonly [T, number])[]): T {
  const total = entries.reduce((sum, [, w]) => sum + w, 0);
  let roll = next(game) * total;
  for (const [value, w] of entries) {
    roll -= w;
    if (roll < 0) return value;
  }
  return entries[entries.length - 1]![0];
}

// --- Derived values ----------------------------------------------------------------------

export const emptyInventory = (): Record<ItemId, number> =>
  Object.fromEntries(ITEM_IDS.map((id) => [id, 0])) as Record<ItemId, number>;

export const cellsFor = (item: ItemId, qty: number): number =>
  qty <= 0 ? 0 : Math.ceil(qty / ITEMS[item].stack) * ITEMS[item].w * ITEMS[item].h;

export function cellsUsed(inventory: Readonly<Record<ItemId, number>>): number {
  return ITEM_IDS.reduce((sum, id) => sum + cellsFor(id, inventory[id]), 0);
}
export const capacity = (game: Game): number =>
  RULES.headCells + game.cars.reduce((sum, car) => sum + CARS[car].cells, 0);
export const seats = (game: Game): number => RULES.headSeats + game.cars.reduce((sum, car) => sum + CARS[car].seats, 0);
export const storageBonus = (game: Game): number => game.cars.reduce((sum, car) => sum + CARS[car].customers, 0);

/** 实际运输载荷 Q：公共货物 + 随身枪弹 1.5 + 乘客私有购买物 */
export function load(game: Game): number {
  const goods = ITEM_IDS.reduce((sum, id) => sum + game.inventory[id] * ITEMS[id].load, 0);
  return goods + RULES.personalLoad + game.riders.reduce((sum, rider) => sum + rider.carryLoad, 0);
}
export const segmentFuel = (game: Game): number =>
  RULES.fuelBase + RULES.fuelPerCar * game.cars.length + RULES.fuelPerLoad * load(game);

/** 当地参考值 M＝V×行情 */
export function marketValue(item: ItemId, event: MarketEvent | undefined): number {
  const v = ITEMS[item].value;
  if (!event || event.item !== item) return v;
  return v * (event.kind === 'up' ? RULES.eventUp : RULES.eventDown);
}
export const retailPrice = (item: ItemId, event: MarketEvent | undefined): number =>
  Math.floor(marketValue(item, event) * RULES.retailMarkup + 1e-9);

/** 第 k 次偿债日 */
export const isDueDay = (day: number): boolean =>
  day % RULES.dueEvery === 0 && day / RULES.dueEvery <= RULES.dueCumulative.length;
export function nextDue(day: number): { readonly day: number; readonly required: number } | undefined {
  const k = Math.ceil(day / RULES.dueEvery);
  if (k < 1 || k > RULES.dueCumulative.length) return undefined;
  return { day: k * RULES.dueEvery, required: RULES.dueCumulative[k - 1]! };
}
export const dueAmount = (game: Game, required: number): number =>
  Math.min(game.debt, Math.max(required - game.repaid, 0));

export const inventoryValue = (game: Game): number =>
  ITEM_IDS.reduce((sum, id) => sum + game.inventory[id] * ITEMS[id].value, 0);

const newLedger = (): DayLedger => ({ segments: 0, fuelRaw: 0, retail: 0, tickets: 0, purchases: 0, cars: 0, repaid: 0, delivered: 0 });

// --- Generation --------------------------------------------------------------------------

function pickItem(game: Game, tag: Tag, exclude: readonly ItemId[]): ItemId {
  const pool = ITEM_IDS.filter((id) => ITEMS[id].tag === tag && !exclude.includes(id)).map(
    (id) => [id, ITEMS[id].weight] as const,
  );
  if (pool.length === 0) return pick(game, ITEM_IDS.filter((id) => !exclude.includes(id)));
  return weighted(game, pool);
}
function pickTag(game: Game, region: Region, side: 'buy' | 'supply'): Tag {
  const weights = TAG_WEIGHTS[region][side];
  return weighted(game, (Object.keys(weights) as Tag[]).map((tag) => [tag, weights[tag]] as const));
}

function supplier(game: Game, region: Region, event: MarketEvent | undefined, forced?: readonly Goods[]): Customer {
  const goods: Goods[] = [];
  const priceOf = (item: ItemId) => Math.ceil(marketValue(item, event) * pick(game, RULES.supplyFactors) - 1e-9);
  if (forced) {
    for (const entry of forced) goods.push({ ...entry, unit: priceOf(entry.item) });
  } else {
    const kinds = next(game) < 0.7 ? 1 : 2;
    for (let k = 0; k < kinds; k += 1) {
      const item = pickItem(game, pickTag(game, region, 'supply'), goods.map((g) => g.item));
      const def = ITEMS[item];
      const qty = def.rare ? 1 : def.raw ? int(game, 2, 6) : int(game, 1, 4);
      goods.push({ item, qty, unit: priceOf(item) });
    }
  }
  const batch = next(game) < RULES.batchChance;
  const discount = batch ? weighted(game, [[0.1, 25], [0.15, 50], [0.2, 25]] as const) : undefined;
  const total = goods.reduce((sum, g) => sum + g.unit * g.qty, 0);
  return {
    id: game.nextId++,
    kind: 'supplier',
    goods,
    ...(discount !== undefined ? { discount, batchPrice: Math.ceil(total * (1 - discount) - 1e-9) } : {}),
  };
}

function buyer(game: Game, region: Region, event: MarketEvent | undefined): Customer {
  const needs: Need[] = [];
  const kinds = next(game) < 0.65 ? 1 : 2;
  for (let k = 0; k < kinds; k += 1) {
    const item = pickItem(game, pickTag(game, region, 'buy'), needs.map((n) => n.item));
    needs.push({ item, qty: ITEMS[item].rare ? 1 : int(game, 1, 4), price: retailPrice(item, event) });
  }
  const [lo, hi] = RULES.buyerBudget;
  const budget = Math.round(int(game, lo, hi) / 5) * 5;
  return { id: game.nextId++, kind: 'buyer', needs, budget };
}

function makeStation(game: Game, region: Region, event: MarketEvent | undefined, first = false): Station {
  const count = RULES.baseCustomers + storageBonus(game);
  const customers: Customer[] = [];
  for (let i = 0; i < count; i += 1) {
    if (i === 0) {
      // 初始站首位供货客保障至少 2 净水 + 1 口粮
      customers.push(
        first
          ? supplier(game, region, event, [
              { item: 'water', qty: 2, unit: 0 },
              { item: 'ration', qty: 1, unit: 0 },
            ])
          : supplier(game, region, event),
      );
    } else if (i === 1) customers.push(buyer(game, region, event));
    else customers.push(next(game) < 0.5 ? supplier(game, region, event) : buyer(game, region, event));
  }
  // 第 3 名起随机顺序出现，首两名固定在前
  const passengers: PassengerCandidate[] = [];
  const pc = int(game, 2, 4);
  for (let i = 0; i < pc; i += 1) passengers.push({ id: game.nextId++, ...(first && i < 2 ? { guaranteedNext: true } : {}) });
  game.stats.stations += 1;
  return {
    id: game.nextId++,
    region,
    place: int(game, 0, PLACE_COUNT[region] - 1),
    ...(event ? { event } : {}),
    customers,
    passengers,
  };
}

function rollEvent(game: Game): MarketEvent | undefined {
  if (game.day <= 1) return undefined;
  const roll = next(game);
  if (roll < 0.7) return undefined;
  return { kind: roll < 0.85 ? 'up' : 'down', item: pick(game, ITEM_IDS) };
}

function makeRouteOptions(game: Game): RouteOption[] {
  const region = (): Region => (next(game) < 0.5 ? 'rich' : 'slum');
  const alternating = (length: number): Region[] => {
    const start = region();
    return Array.from({ length }, (_, i) => (i % 2 === 0 ? start : start === 'rich' ? 'slum' : 'rich'));
  };
  const free = (length: number): Region[] => Array.from({ length }, region);
  const options: RouteOption[] = [
    { nodes: alternating(2) },
    { nodes: alternating(RULES.driverNodes) },
    { nodes: free(next(game) < 0.5 ? 2 : RULES.driverNodes) },
  ];
  // 候选生成后固定；按节点数排序，短线在前
  return options.sort((a, b) => a.nodes.length - b.nodes.length);
}

/** 为当前站点的乘客候选绑定目的地（路线已锁定后） */
function bindPassengers(game: Game): void {
  const route = game.route;
  if (!route) return;
  const here = route.arrived;
  const remaining = route.nodes.length - here;
  for (const p of game.station.passengers) {
    if (p.dest !== undefined || p.boarded) continue;
    if (remaining <= 0) continue;
    const dest = p.guaranteedNext ? here + 1 : here + int(game, 1, remaining);
    p.dest = dest;
    p.from = here;
    p.fare = RULES.ticketPerSegment * (dest - here);
  }
}

// --- Lifecycle ---------------------------------------------------------------------------

export function newGame(seed: number): Game {
  const game: Game = {
    version: SAVE_VERSION,
    seed,
    rng: seed | 0,
    day: 1,
    phase: 'depot',
    cash: RULES.startCash,
    debt: RULES.principal,
    repaid: 0,
    cars: [],
    inventory: emptyInventory(),
    station: undefined as unknown as Station,
    nextId: 1,
    riders: [],
    today: newLedger(),
    cleared: false,
    stats: { revenue: 0, tickets: 0, passengers: 0, stations: 0 },
  };
  game.station = makeStation(game, 'rich', undefined, true);
  game.routeOptions = makeRouteOptions(game);
  return game;
}

export function isGame(value: unknown): value is Game {
  const g = value as Game | null;
  return !!g && typeof g === 'object' && g.version === SAVE_VERSION && typeof g.cash === 'number' && !!g.station;
}

/** 出发前：从 3 条候选中锁定 1 条，绑定起点站乘客 */
export function chooseRoute(game: Game, index: number): ActionResult {
  if (game.phase !== 'depot' || game.route) return fail('phase');
  const option = game.routeOptions?.[index];
  if (!option) return fail('not-found');
  game.event = rollEvent(game);
  game.route = { nodes: option.nodes, arrived: 0 };
  bindPassengers(game);
  return success();
}

const trading = (game: Game) => game.phase === 'depot' || game.phase === 'station' || game.phase === 'settlement';

export function findCustomer(game: Game, id: number): Customer | undefined {
  return game.station.customers.find((c) => c.id === id);
}

/** 单拿：按所选数量买入（每客最多成交 1 次） */
export function buySingle(game: Game, id: number, quantities: readonly number[]): ActionResult {
  if (!trading(game) || game.phase === 'settlement') return fail('phase');
  const c = findCustomer(game, id);
  if (!c || c.kind !== 'supplier' || !c.goods) return fail('not-found');
  if (c.outcome) return fail('done');
  const picked = c.goods.map((g, i) => ({ ...g, qty: Math.max(0, Math.min(g.qty, Math.floor(quantities[i] ?? 0))) }));
  const total = picked.reduce((sum, g) => sum + g.qty * g.unit, 0);
  if (total <= 0) return fail('amount');
  return commitPurchase(game, c, picked, total);
}

/** 整批：接下完整货单，按折扣价一次付清 */
export function buyBatch(game: Game, id: number): ActionResult {
  if (!trading(game) || game.phase === 'settlement') return fail('phase');
  const c = findCustomer(game, id);
  if (!c || c.kind !== 'supplier' || !c.goods || c.batchPrice === undefined) return fail('not-found');
  if (c.outcome) return fail('done');
  return commitPurchase(game, c, c.goods, c.batchPrice);
}

function commitPurchase(game: Game, c: Customer, goods: readonly Goods[], total: number): ActionResult {
  if (game.cash < 0) return fail('negative-cash');
  if (game.cash < total) return fail('cash');
  const after = { ...game.inventory };
  for (const g of goods) after[g.item] += g.qty;
  if (cellsUsed(after) > capacity(game)) return fail('space');
  game.inventory = after;
  game.cash -= total;
  game.today.purchases += total;
  c.outcome = 'deal';
  c.amount = -total;
  return success(total);
}

/** 预览卖给采购客：min(需求、库存、预算能买的量)，按需求顺序逐类 */
export function previewSale(game: Game, c: Customer): { readonly lines: readonly { item: ItemId; qty: number; price: number }[]; readonly total: number } {
  let budget = c.budget ?? 0;
  const lines: { item: ItemId; qty: number; price: number }[] = [];
  for (const need of c.needs ?? []) {
    const qty = Math.max(0, Math.min(need.qty, game.inventory[need.item], Math.floor(budget / need.price)));
    if (qty > 0) {
      lines.push({ item: need.item, qty, price: need.price });
      budget -= qty * need.price;
    }
  }
  return { lines, total: lines.reduce((sum, l) => sum + l.qty * l.price, 0) };
}

/** 卖给采购客（负现金也允许卖货） */
export function sellToBuyer(game: Game, id: number): ActionResult {
  if (!trading(game)) return fail('phase');
  const c = findCustomer(game, id);
  if (!c || c.kind !== 'buyer') return fail('not-found');
  if (c.outcome) return fail('done');
  const sale = previewSale(game, c);
  if (sale.total <= 0) {
    const anyStock = (c.needs ?? []).some((n) => game.inventory[n.item] > 0);
    return fail(anyStock ? 'budget' : 'no-stock');
  }
  for (const line of sale.lines) game.inventory[line.item] -= line.qty;
  game.cash += sale.total;
  game.today.retail += sale.total;
  game.stats.revenue += sale.total;
  c.outcome = 'deal';
  c.amount = sale.total;
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

/** 普通乘客上车：上车即收全款 */
export function boardPassenger(game: Game, id: number): ActionResult {
  if (game.phase !== 'depot' && game.phase !== 'station') return fail('phase');
  const p = game.station.passengers.find((x) => x.id === id);
  if (!p) return fail('not-found');
  if (p.boarded) return fail('done');
  if (p.dest === undefined || p.fare === undefined) return fail('unbound');
  if (game.riders.length >= seats(game)) return fail('seats');
  p.boarded = true;
  game.riders.push({ id: p.id, dest: p.dest, fare: p.fare, carryLoad: 0 });
  game.cash += p.fare;
  game.today.tickets += p.fare;
  game.stats.tickets += p.fare;
  game.stats.passengers += 1;
  return success(p.fare);
}

/** 卖给在车乘客当天的随机需求（1 件，预算 60） */
export function sellToRider(game: Game, id: number): ActionResult {
  if (game.phase !== 'station' && game.phase !== 'depot') return fail('phase');
  const rider = game.riders.find((r) => r.id === id);
  if (!rider?.want || rider.want.sold) return fail('not-found');
  if (rider.want.price > RULES.riderBudget) return fail('budget');
  if (game.inventory[rider.want.item] <= 0) return fail('no-stock');
  game.inventory[rider.want.item] -= 1;
  rider.carryLoad += ITEMS[rider.want.item].load;
  rider.want.sold = true;
  game.cash += rider.want.price;
  game.today.retail += rider.want.price;
  game.stats.revenue += rider.want.price;
  return success(rider.want.price);
}

/** 整备：购买车厢（需现金非负且足额） */
export function buyCar(game: Game, kind: CarKind): ActionResult {
  if (game.phase !== 'depot' && game.phase !== 'station') return fail('phase');
  if (game.cars.length >= RULES.maxCars) return fail('max-cars');
  const price = CARS[kind].price;
  if (game.cash < 0) return fail('negative-cash');
  if (game.cash < price) return fail('cash');
  game.cash -= price;
  game.cars.push(kind);
  game.today.cars += price;
  return success(price);
}

/** 提前还款 */
export function repay(game: Game, amount: number): ActionResult {
  if (game.phase !== 'depot' && game.phase !== 'station') return fail('phase');
  const value = Math.floor(Math.min(amount, game.debt));
  if (value <= 0) return fail('amount');
  if (game.cash < value) return fail('cash');
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
  if (game.debt > 0 || game.cleared || game.riders.length > 0) return;
  game.cleared = true;
  game.resume = game.phase;
  game.phase = 'won';
}

/** 发车：锁定本段配置与载荷，记一段燃油 */
export function depart(game: Game): ActionResult {
  if (game.phase !== 'depot' && game.phase !== 'station') return fail('phase');
  const route = game.route;
  if (!route) return fail('unbound');
  if (route.arrived >= route.nodes.length) return fail('phase');
  game.today.segments += 1;
  game.today.fuelRaw += segmentFuel(game);
  game.phase = 'travel';
  return success();
}

/** 抵达：下客、新站客流、乘客随机购物 */
export function arrive(game: Game): ActionResult {
  if (game.phase !== 'travel' || !game.route) return fail('phase');
  const route = game.route;
  route.arrived += 1;
  const here = route.arrived;
  const delivered = game.riders.filter((r) => r.dest === here);
  game.riders = game.riders.filter((r) => r.dest !== here);
  game.today.delivered += delivered.length;
  game.station = makeStation(game, route.nodes[here - 1]!, game.event);
  for (const rider of game.riders) {
    if (rider.want) continue;
    if (next(game) < RULES.riderShopChance) {
      const item = weighted(game, RIDER_WANTS);
      rider.want = { item, price: retailPrice(item, game.event) };
    }
  }
  bindPassengers(game);
  game.phase = 'station';
  return success(delivered.length);
}

export const routeDone = (game: Game): boolean => !!game.route && game.route.arrived >= game.route.nodes.length;

/** 收车：日末费用入账（燃油、工资、生活维护），普通日可为负 */
export function endDay(game: Game): ActionResult {
  if (game.phase !== 'station' || !routeDone(game)) return fail('phase');
  const t = game.today;
  const fuel = Math.ceil(t.fuelRaw - 1e-9);
  const wages = RULES.driverWage * t.segments;
  const living = RULES.livingPerHead * (RULES.crew + game.riders.length);
  const total = fuel + wages + living;
  const before = game.cash;
  game.cash -= total;
  for (const rider of game.riders) delete rider.want;
  game.bill = { day: game.day, segments: t.segments, fuel, wages, living, total, cashBefore: before, cashAfter: game.cash, ledger: { ...t } };
  game.phase = 'dayEnd';
  return success(total);
}

/** 日结确认后：偿债日进入大结算，否则进入下一天 */
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

/** 大结算：付应补本金 D，需 C≥D 且付后非负 */
export function paySettlement(game: Game): ActionResult {
  if (game.phase !== 'settlement' || !game.settlement) return fail('phase');
  const due = dueAmount(game, game.settlement.required);
  if (game.cash < due || game.cash < 0) return fail('cash');
  if (due > 0) payPrincipal(game, due);
  delete game.settlement;
  if (game.debt === 0 && !game.cleared) {
    game.cleared = true;
    game.resume = 'depot';
    startNextDay(game);
    game.phase = 'won';
    return success(due);
  }
  startNextDay(game);
  return success(due);
}

/** 大结算现金不足且放弃补缺：本局失败 */
export function forfeit(game: Game): ActionResult {
  if (game.phase !== 'settlement') return fail('phase');
  game.phase = 'lost';
  game.lostReason = 'debt';
  return success();
}

/** 关闭清债胜利弹窗，继续经营 */
export function continueAfterWin(game: Game): ActionResult {
  if (game.phase !== 'won') return fail('phase');
  game.phase = game.resume ?? 'depot';
  delete game.resume;
  return success();
}

function startNextDay(game: Game): void {
  game.day += 1;
  game.today = newLedger();
  delete game.route;
  delete game.event;
  delete game.bill;
  // 过夜不刷新同站顾客与乘客候选；未登车候选等待新路线绑定
  for (const p of game.station.passengers) {
    if (!p.boarded) {
      delete p.dest;
      delete p.fare;
      delete p.from;
    }
  }
  game.station.passengers = game.station.passengers.filter((p) => !p.boarded);
  game.routeOptions = makeRouteOptions(game);
  game.phase = 'depot';
}

/** 下一次偿债预览（HUD 顶栏） */
export function debtPreview(game: Game): { readonly day: number; readonly required: number; readonly due: number } | undefined {
  const due = nextDue(game.day);
  if (!due) return undefined;
  return { ...due, due: dueAmount(game, due.required) };
}

export function enterStationMap(game: Game): ActionResult {
  if (game.phase !== 'station' || game.expedition?.stationId === game.station.id) return fail('phase');
  game.expedition = {stationId: game.station.id, status:'active', x:TRAIN_EXIT.x, y:TRAIN_EXIT.z, mapVersion:2, surveyed:[]};
  game.phase='exploration'; return {ok:true};
}
export function moveExplorer(game:Game,dx:number,dy:number):void {
  const e=game.expedition;
  if(game.phase!=='exploration'||!e||Math.abs(dx)+Math.abs(dy)!==1) return;
  const next=walkSite(game.station.id,e.x,e.y,dx,dy);
  e.x=next.x;e.y=next.z;
}
export function returnToTrain(game:Game):ActionResult {
  const e=game.expedition;
  if(game.phase!=='exploration'||!e||!canExtract(e.x,e.y)) return fail('phase');
  e.status='returned';game.phase='station';return {ok:true};
}
export function skipStationMap(game:Game):ActionResult {
  if(game.phase!=='station'||game.expedition?.stationId===game.station.id) return fail('phase');
  game.expedition={stationId:game.station.id,status:'skipped',x:1,y:5};return {ok:true};
}
