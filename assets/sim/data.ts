/**
 * 《末日列车》v0.2 数据表。数值全部取自飞书团队文档（00 功能文档总表及 11 个分支表，0 Tag、无特性首测基准），
 * 出处见 design/v0.2-plan.md 与 demo_spec.md。【补充案】为拆解玩法补充，可在新游戏的「实验玩法」开关里关闭。
 * Player-visible names live in the UI language table, never here.
 */

export type ItemId =
  | 'ration'
  | 'water'
  | 'veg'
  | 'sewage'
  | 'scrap'
  | 'ingot'
  | 'parts'
  | 'seed'
  | 'research'
  | 'bandage'
  | 'pistol'
  | 'smg'
  | 'ammo';

export type Tag = 'daily' | 'industry' | 'arms' | 'medical';
export type Region = 'rich' | 'slum';
export type NodeKind = Region | 'resource' | 'hunt';
export type Theme = 'yard' | 'waterworks' | 'supply' | 'hunt';
export type CarKind = 'storage' | 'passenger' | 'planting' | 'processing';
export type DeviceKind = 'purifier' | 'furnace' | 'fabricator';
/** 品质【补充案】：2 优 / 1 良 / 0 差 */
export type Grade = 0 | 1 | 2;

export interface ItemDef {
  readonly id: ItemId;
  /** 参考价值 V（弹药为每 30 发） */
  readonly value: number;
  readonly w: number;
  readonly h: number;
  /** 单堆上限（弹药按发数） */
  readonly stack: number;
  /** 单件载荷（弹药为每发） */
  readonly load: number;
  readonly tag: Tag;
  /** 原料类：供货每类 2—6 件 */
  readonly raw?: boolean;
  /** 稀有：供货只带 1 件 */
  readonly rare?: boolean;
  /** 供货池相对权重（同标签内） */
  readonly weight: number;
  /** 交易计价单位（弹药 30 发） */
  readonly unit?: number;
  /** 带品质【补充案】 */
  readonly graded?: boolean;
  readonly weapon?: boolean;
}

export const ITEMS: Readonly<Record<ItemId, ItemDef>> = {
  ration: { id: 'ration', value: 32, w: 1, h: 1, stack: 1, load: 1, tag: 'daily', weight: 3 },
  water: { id: 'water', value: 12, w: 2, h: 1, stack: 1, load: 1, tag: 'daily', weight: 4, graded: true },
  veg: { id: 'veg', value: 18, w: 2, h: 1, stack: 1, load: 1, tag: 'daily', weight: 3 },
  sewage: { id: 'sewage', value: 4, w: 2, h: 1, stack: 2, load: 1, tag: 'industry', raw: true, weight: 2 },
  scrap: { id: 'scrap', value: 12, w: 2, h: 1, stack: 4, load: 1, tag: 'industry', raw: true, weight: 4 },
  ingot: { id: 'ingot', value: 28, w: 2, h: 1, stack: 4, load: 1, tag: 'industry', weight: 2 },
  parts: { id: 'parts', value: 20, w: 1, h: 1, stack: 2, load: 1, tag: 'industry', weight: 4, graded: true },
  seed: { id: 'seed', value: 8, w: 1, h: 1, stack: 4, load: 0.25, tag: 'industry', raw: true, weight: 2 },
  research: { id: 'research', value: 60, w: 1, h: 1, stack: 2, load: 1, tag: 'industry', rare: true, weight: 0.4 },
  bandage: { id: 'bandage', value: 18, w: 1, h: 1, stack: 4, load: 0.25, tag: 'medical', weight: 1, graded: true },
  pistol: { id: 'pistol', value: 120, w: 2, h: 1, stack: 1, load: 1, tag: 'arms', rare: true, weight: 1, weapon: true },
  smg: { id: 'smg', value: 300, w: 3, h: 2, stack: 1, load: 2, tag: 'arms', rare: true, weight: 0.25, weapon: true },
  ammo: { id: 'ammo', value: 10, w: 1, h: 1, stack: 60, load: 1 / 120, tag: 'arms', weight: 3, unit: 30 },
};
export const ITEM_IDS = Object.keys(ITEMS) as ItemId[];
export const unitOf = (id: ItemId): number => ITEMS[id].unit ?? 1;

/** 品质系数【补充案】 */
export const GRADE_FACTOR: Readonly<Record<Grade, number>> = { 0: 0.5, 1: 1, 2: 1.15 };

/** 商业站顾客类别权重〔09·02〕 */
export const TAG_WEIGHTS: Readonly<Record<Region, Readonly<Record<'buy' | 'supply', Readonly<Record<Tag, number>>>>>> = {
  rich: { buy: { daily: 30, industry: 20, arms: 25, medical: 25 }, supply: { daily: 35, industry: 25, arms: 25, medical: 15 } },
  slum: { buy: { daily: 50, industry: 15, arms: 10, medical: 25 }, supply: { daily: 45, industry: 35, arms: 15, medical: 5 } },
};

/** 乘客车上购物需求池〔05·03/03.03〕（奢侈品未接入，按剩余比例归一） */
export const RIDER_WANT_TAGS: Readonly<Record<'normal' | 'noble', readonly (readonly [Tag, number])[]>> = {
  normal: [['daily', 60], ['medical', 25], ['industry', 10], ['arms', 5]],
  noble: [['daily', 15], ['medical', 15], ['arms', 10]],
};

export const RULES = Object.freeze({
  startCash: 500,
  principal: 6000,
  headW: 4,
  headH: 2,
  headSeats: 2,
  baseCustomers: 3,
  livingPerHead: 2,
  livingNoble: 7,
  personalGunLoad: 1,
  personalAmmoMax: 60,
  startAmmo: 60,
  fuelBase: 6,
  fuelPerCar: 4,
  fuelPerLoad: 0.5,
  retailMarkup: 1.2,
  supplyFactors: [0.8, 0.85, 0.9, 0.95, 1.0, 1.05, 1.1],
  batchChance: 0.6,
  buyerBudget: [40, 180] as const,
  armsBudget: [100, 500] as const,
  maxCars: 10,
  carRefund: 0.6,
  dueEvery: 5,
  dueCumulative: [600, 1500, 2500, 3600, 4800, 6000],
  /** 乘客购物基准 B（文档缺失，暂取） */
  riderBase: 120,
  riderShopChance: { normal: 0.3, noble: 0.45 },
  workPerSegment: 100,
  hpMax: 100,
  hpAfterDeath: 30,
  hpPerSegment: 20,
  gunUpkeep: { pistol: 6, smg: 14 } as Readonly<Record<'pistol' | 'smg', number>>,
  /** 缺供保障：连续 2 个商业站无对应供货 */
  shortageAfter: 2,
});

/** 司机〔05·01/01.04〕：每日节点、线路上限、招募费、每段工资 */
export const DRIVERS = [
  { nodes: 3, lines: 1, hire: 0, wage: 3 },
  { nodes: 4, lines: 2, hire: 180, wage: 5 },
  { nodes: 5, lines: 2, hire: 360, wage: 7 },
  { nodes: 6, lines: 2, hire: 720, wage: 10 },
] as const;
export const DRIVER_ODDS: readonly (readonly [1 | 2 | 3, number])[] = [[1, 65], [2, 28], [3, 7]];
export const DRIVER_CHANCE = 0.35;

export interface CarLevel {
  readonly cash: number;
  readonly ingot: number;
  readonly parts: number;
  /** 存储：仓宽；载客：席位；种植：槽；加工：设备槽 */
  readonly cap: number;
  /** 存储车厢来客增量 */
  readonly customers?: number;
}
/** 车厢购建与升级〔05·02—05·06〕 */
export const CARS: Readonly<Record<CarKind, readonly [CarLevel, CarLevel, CarLevel]>> = {
  storage: [
    { cash: 600, ingot: 0, parts: 0, cap: 4, customers: 1 },
    { cash: 400, ingot: 2, parts: 0, cap: 6, customers: 2 },
    { cash: 700, ingot: 4, parts: 2, cap: 9, customers: 3 },
  ],
  passenger: [
    { cash: 800, ingot: 0, parts: 0, cap: 6 },
    { cash: 500, ingot: 0, parts: 2, cap: 8 },
    { cash: 800, ingot: 2, parts: 4, cap: 10 },
  ],
  planting: [
    { cash: 850, ingot: 0, parts: 0, cap: 2 },
    { cash: 550, ingot: 0, parts: 2, cap: 3 },
    { cash: 900, ingot: 2, parts: 4, cap: 4 },
  ],
  processing: [
    { cash: 1000, ingot: 0, parts: 0, cap: 4 },
    { cash: 700, ingot: 2, parts: 2, cap: 6 },
    { cash: 1100, ingot: 4, parts: 4, cap: 8 },
  ],
};
export const CAR_KINDS = Object.keys(CARS) as CarKind[];
export const STORAGE_H = 4;

/** 座位等级：0 车头 / 1—3 载客等级〔05·03/03.01〕 */
export const SEAT_ODDS: readonly { readonly normal: number; readonly noble: number }[] = [
  { normal: 0.5, noble: 0.05 },
  { normal: 0.7, noble: 0.25 },
  { normal: 0.85, noble: 0.5 },
  { normal: 0.95, noble: 0.75 },
];
export const FARE = { normal: [20, 28], noble: [60, 90] } as const;
export const COMP = { normal: [0.4, 0.6], noble: [1.0, 1.5] } as const;
export const PASSENGER_POOL: Readonly<Record<Region, { readonly n: readonly [number, number]; readonly noble: number }>> = {
  rich: { n: [2, 4], noble: 0.7 },
  slum: { n: [3, 6], noble: 0.1 },
};

/** 种植〔05·04〕：2 种子 + 2 净水 → 4 蔬菜，300 工作量 */
export const PLANT = { seed: 2, water: 2, out: 4, work: 300 } as const;

export interface Recipe {
  readonly id: string;
  readonly device: DeviceKind;
  readonly inputs: readonly (readonly [ItemId, number])[];
  readonly out: ItemId;
  readonly qty: number;
  readonly work: number;
}
/** 设备〔05·06/06.01〕：安装现金、占槽、载荷、条件维护 */
export const DEVICES: Readonly<Record<DeviceKind, { readonly cash: number; readonly slots: number; readonly load: number; readonly upkeep: number }>> = {
  purifier: { cash: 160, slots: 1, load: 2, upkeep: 3 },
  furnace: { cash: 260, slots: 2, load: 4, upkeep: 6 },
  fabricator: { cash: 420, slots: 2, load: 4, upkeep: 8 },
};
export const DEVICE_KINDS = Object.keys(DEVICES) as DeviceKind[];
/** 基础配方〔05·06/06.02〕 */
export const RECIPES: readonly Recipe[] = [
  { id: 'water', device: 'purifier', inputs: [['sewage', 6]], out: 'water', qty: 4, work: 100 },
  { id: 'ingot', device: 'furnace', inputs: [['scrap', 6]], out: 'ingot', qty: 4, work: 100 },
  { id: 'ammo', device: 'fabricator', inputs: [['ingot', 1]], out: 'ammo', qty: 120, work: 100 },
];

/** 路线模板〔06·02〕：起点不计，末站商业 */
export const ROUTE_TEMPLATES: readonly (readonly NodeKind[])[] = [
  ['rich', 'slum'],
  ['slum', 'rich'],
  ['resource', 'rich'],
  ['hunt', 'rich'],
  ['rich', 'slum', 'rich'],
  ['resource', 'slum', 'rich'],
  ['slum', 'hunt', 'rich'],
  ['resource', 'hunt', 'rich'],
];
export const isCommerce = (k: NodeKind): k is Region => k === 'rich' || k === 'slum';

/** 站点服务（文档未定义开放规则，暂定）：富人区全开，贫民区无整备 */
export const SERVICES: Readonly<Record<Region, { readonly depot: boolean; readonly hire: boolean }>> = {
  rich: { depot: true, hire: true },
  slum: { depot: false, hire: true },
};

export const PLACE_COUNT: Readonly<Record<NodeKind, number>> = { rich: 8, slum: 8, resource: 6, hunt: 4 };

// --- 【补充案】鉴定 / 掺假、议价 / 声望 ---------------------------------------------------

export const EXP = Object.freeze({
  /** 供货虚标概率 */
  fakeChance: { rich: 0.15, slum: 0.35 } as Readonly<Record<Region, number>>,
  /** 采购客识破虚标 */
  detect: { rich: 0.2, slum: 0.5 } as Readonly<Record<Region, number>>,
  detectRep: -8,
  /** 每站验货次数 */
  inspections: 2,
  /** 每 N 次坑客引来稽查员 */
  cheatsPerInspector: 3,
  inspectorFine: 50,
  haggle: [10, 25] as const,
  walkRep: -3,
  /** 议价成功时本笔声望改为扣 */
  greedRep: { 10: -0.5, 25: -1.5 } as Readonly<Record<number, number>>,
  repBudget: 20,
  repExtraCustomer: 50,
  repFewerCustomer: -30,
  repNoBoard: -60,
});

// --- 探索〔08〕〔09〕 ---------------------------------------------------------------------

export type EnemyKind = 'melee' | 'charger' | 'ranged';
export const ENEMIES: Readonly<Record<EnemyKind, {
  readonly hp: number; readonly speed: number; readonly dmg: number; readonly cd: number; readonly windup: number;
  readonly dashSpeed?: number; readonly dashTime?: number; readonly range?: number; readonly bulletSpeed?: number;
}>> = {
  melee: { hp: 20, speed: 2, dmg: 8, cd: 1.2, windup: 0.5 },
  charger: { hp: 16, speed: 2.8, dmg: 12, cd: 4.5, windup: 0.65, dashSpeed: 7.5, dashTime: 0.45 },
  ranged: { hp: 28, speed: 1.5, dmg: 10, cd: 2.8, windup: 0.6, range: 10, bulletSpeed: 6 },
};
export const WEAPONS: Readonly<Record<'pistol' | 'smg', { readonly dmg: number; readonly rate: number; readonly mag: number; readonly reload: number; readonly range: number; readonly speed: number }>> = {
  pistol: { dmg: 12, rate: 3, mag: 12, reload: 1.5, range: 16, speed: 30 },
  smg: { dmg: 7, rate: 7, mag: 24, reload: 1.6, range: 14, speed: 30 },
};
export const ENCOUNTER: Readonly<Record<'resource' | 'hunt', { readonly melee: number; readonly charger: number; readonly ranged: number; readonly w: number; readonly h: number; readonly containers: readonly [number, number]; readonly ground: readonly [number, number] }>> = {
  resource: { melee: 6, charger: 1, ranged: 1, w: 60, h: 45, containers: [4, 6], ground: [2, 3] },
  hunt: { melee: 8, charger: 2, ranged: 2, w: 70, h: 50, containers: [6, 8], ground: [3, 4] },
};
export type Pool = 'daily' | 'industry' | 'waterpool' | 'medical' | 'arms' | 'premium';
export type BoxKind = 'junk' | 'tools' | 'medical' | 'weapons' | 'risk';
/** 容器〔09·04〕：空置率、2 组概率、开箱秒、物品池 */
export const BOXES: Readonly<Record<BoxKind, { readonly empty: number; readonly two: number; readonly open: number; readonly pools: readonly (readonly [Pool, number])[] }>> = {
  junk: { empty: 0.25, two: 0.3, open: 2, pools: [['daily', 60], ['industry', 40]] },
  tools: { empty: 0.2, two: 0.5, open: 3, pools: [['industry', 85], ['daily', 15]] },
  medical: { empty: 0.3, two: 0.2, open: 2.5, pools: [['medical', 100]] },
  weapons: { empty: 0.45, two: 0, open: 4, pools: [['arms', 100]] },
  risk: { empty: 0.2, two: 0.4, open: 5, pools: [['industry', 50], ['arms', 30], ['medical', 20]] },
};
export const BOX_KINDS = Object.keys(BOXES) as BoxKind[];
/** 主题容器权重（杂物 / 工具 / 医疗 / 武器 / 高风险）与地面物池〔09·03〕 */
export const THEMES: Readonly<Record<Theme, { readonly boxes: readonly [number, number, number, number, number]; readonly ground: readonly (readonly [Pool, number])[] }>> = {
  yard: { boxes: [40, 45, 5, 5, 5], ground: [['industry', 80], ['daily', 20]] },
  waterworks: { boxes: [20, 55, 10, 5, 10], ground: [['waterpool', 70], ['industry', 30]] },
  supply: { boxes: [55, 15, 20, 5, 5], ground: [['daily', 80], ['industry', 20]] },
  hunt: { boxes: [30, 25, 10, 20, 15], ground: [['industry', 40], ['daily', 40], ['medical', 20]] },
};
/** 掉落池〔09·05〕：物品、权重、数量范围（弹药按发数） */
export const POOLS: Readonly<Record<Pool, readonly (readonly [ItemId, number, number, number])[]>> = {
  daily: [['ration', 20, 1, 1], ['water', 30, 1, 1], ['veg', 15, 1, 1], ['seed', 15, 2, 4], ['sewage', 20, 1, 3]],
  industry: [['scrap', 40, 1, 3], ['sewage', 20, 1, 3], ['parts', 25, 1, 2], ['ingot', 10, 1, 1], ['research', 2, 1, 1]],
  waterpool: [['sewage', 55, 1, 3], ['water', 25, 1, 1], ['parts', 15, 1, 1], ['bandage', 5, 1, 2]],
  medical: [['bandage', 100, 1, 3]],
  arms: [['ammo', 85, 15, 45], ['pistol', 12, 1, 1], ['smg', 3, 1, 1]],
  /** 【补充案】稀有箱高价池 */
  premium: [['research', 25, 1, 1], ['parts', 35, 2, 2], ['ingot', 25, 1, 2], ['smg', 5, 1, 1], ['bandage', 10, 2, 3]],
};
export const CORPSE = { empty: 0.75, pools: [['industry', 45], ['daily', 35], ['medical', 20]] as readonly (readonly [Pool, number])[] };
/** 【补充案】容器品质：普通 / 精良 / 稀有 */
export const TIER_WEIGHTS: Readonly<Record<'resource' | 'hunt', readonly [number, number, number]>> = { resource: [70, 25, 5], hunt: [55, 33, 12] };
export const TIER_REVEAL = [1, 1.5, 2] as const;
export const BACKPACK = { w: 4, h: 2 } as const;
export const EXPLORE = Object.freeze({
  walk: 4.5, aimWalk: 2.8, sneak: 2.2, sprint: 6, stamina: 100, staminaDrain: 25, staminaRegen: 20, staminaDelay: 1.5,
  heavyLoad: 6, heavyWalk: 3.8, heavySprint: 5.2, hurtGuard: 0.35,
  viewPlayer: 16, viewEnemy: 10, fovEnemy: 120,
  soundGun: 18, soundSprint: 8, soundWalk: 4, soundSneak: 2, soundSearch: 3, loseTarget: 6,
  extractHold: 5, bandageTime: 3, bandageHeal: 20, groundPick: 0.5, corpseSearch: 2, lostBagOpen: 2,
  spawnAway: 8, maxChasers: { resource: 4, hunt: 6 } as Readonly<Record<'resource' | 'hunt', number>>,
  pushRange: 1, pushCooldown: 1.2,
});
