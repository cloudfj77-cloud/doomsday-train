/**
 * 《末日列车》原型数据表。全部数值取自飞书策划案《二 玩法功能说明》0 Tag、无特性首测基准。
 * Player-visible names live in the UI language table (`item.<id>`, `region.<id>`), never here.
 */

export type ItemId =
  | 'ration'
  | 'water'
  | 'veg'
  | 'sewage'
  | 'scrap'
  | 'ingot'
  | 'parts'
  | 'repair'
  | 'seed'
  | 'research'
  | 'bandage';

export type Tag = 'daily' | 'industry' | 'medical';
export type Region = 'rich' | 'slum';
export type CarKind = 'storage' | 'passenger';

export interface ItemDef {
  readonly id: ItemId;
  /** 参考价值 V */
  readonly value: number;
  /** 格型宽×高（原型按面积计占格，不做二维摆放） */
  readonly w: number;
  readonly h: number;
  /** 单堆上限 */
  readonly stack: number;
  /** 单件载荷 */
  readonly load: number;
  readonly tag: Tag;
  /** 原料类：供货数量 2—6，其余 1—4 */
  readonly raw?: boolean;
  /** 高价稀有：供货只带 1 件 */
  readonly rare?: boolean;
  /** 供货池相对权重（同标签内） */
  readonly weight: number;
}

export const ITEMS: Readonly<Record<ItemId, ItemDef>> = {
  ration: { id: 'ration', value: 32, w: 1, h: 1, stack: 1, load: 1, tag: 'daily', weight: 3 },
  water: { id: 'water', value: 12, w: 2, h: 1, stack: 1, load: 1, tag: 'daily', weight: 4 },
  veg: { id: 'veg', value: 18, w: 2, h: 1, stack: 1, load: 1, tag: 'daily', weight: 3 },
  sewage: { id: 'sewage', value: 4, w: 2, h: 1, stack: 2, load: 1, tag: 'industry', raw: true, weight: 2 },
  scrap: { id: 'scrap', value: 12, w: 2, h: 1, stack: 4, load: 1, tag: 'industry', raw: true, weight: 4 },
  ingot: { id: 'ingot', value: 28, w: 2, h: 1, stack: 4, load: 1, tag: 'industry', weight: 2 },
  parts: { id: 'parts', value: 20, w: 1, h: 1, stack: 2, load: 1, tag: 'industry', weight: 4 },
  repair: { id: 'repair', value: 90, w: 2, h: 2, stack: 1, load: 1, tag: 'industry', rare: true, weight: 0.4 },
  seed: { id: 'seed', value: 8, w: 1, h: 1, stack: 4, load: 0.25, tag: 'industry', weight: 2 },
  research: { id: 'research', value: 60, w: 1, h: 1, stack: 2, load: 1, tag: 'industry', rare: true, weight: 0.3 },
  bandage: { id: 'bandage', value: 18, w: 1, h: 1, stack: 4, load: 0.25, tag: 'medical', weight: 1 },
};

export const ITEM_IDS = Object.keys(ITEMS) as ItemId[];

/** 商业站顾客标签权重（策划案表；首测原型无武器弹药，剩余标签按原比例归一）。 */
export const TAG_WEIGHTS: Readonly<Record<Region, Readonly<Record<'buy' | 'supply', Readonly<Record<Tag, number>>>>>> = {
  rich: { buy: { daily: 30, industry: 20, medical: 25 }, supply: { daily: 35, industry: 25, medical: 15 } },
  slum: { buy: { daily: 50, industry: 15, medical: 25 }, supply: { daily: 45, industry: 35, medical: 5 } },
};

/** 乘客在车随机购物需求池 */
export const RIDER_WANTS: readonly (readonly [ItemId, number])[] = [
  ['water', 30],
  ['ration', 20],
  ['veg', 20],
  ['bandage', 20],
  ['parts', 10],
];

export const RULES = Object.freeze({
  startCash: 500,
  principal: 6000,
  headCells: 8,
  headSeats: 2,
  baseCustomers: 3,
  driverNodes: 3,
  driverWage: 3,
  livingPerHead: 2,
  /** 玩家 + 现任司机 */
  crew: 2,
  personalLoad: 1.5,
  fuelBase: 6,
  fuelPerCar: 4,
  fuelPerLoad: 0.5,
  ticketPerSegment: 24,
  riderShopChance: 0.3,
  riderBudget: 60,
  retailMarkup: 1.2,
  supplyFactors: [0.8, 0.85, 0.9, 0.95, 1.0, 1.05, 1.1],
  batchChance: 0.6,
  buyerBudget: [40, 180] as const,
  maxCars: 10,
  /** 偿债大结算：第 5k 天累计至少已还 */
  dueEvery: 5,
  dueCumulative: [600, 1500, 2500, 3600, 4800, 6000],
  eventUp: 1.25,
  eventDown: 0.8,
});

export const CARS: Readonly<Record<CarKind, { readonly price: number; readonly cells: number; readonly seats: number; readonly customers: number }>> = {
  storage: { price: 600, cells: 16, seats: 0, customers: 1 },
  passenger: { price: 800, cells: 0, seats: 6, customers: 0 },
};

/** Station place-name pools (indices into `place.<region>.<n>` of the language table). */
export const PLACE_COUNT: Readonly<Record<Region, number>> = { rich: 8, slum: 8 };
