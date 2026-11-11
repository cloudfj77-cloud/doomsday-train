/**
 * 探索地图生成〔08〕〔09〕：按节点种子确定性生成紧凑小图、容器、地面点、敌人与全部掉落。
 * 重开、读档、回收都用同一种子，不重抽〔03·02〕。坐标单位：米；墙体为 1 m 网格。
 */
import {
  BOX_KINDS,
  BOXES,
  CORPSE,
  ENCOUNTER,
  POOLS,
  THEMES,
  TIER_WEIGHTS,
  type BoxKind,
  type EnemyKind,
  type Grade,
  type ItemId,
  type Pool,
  type Theme,
} from './data.ts';

export interface Loot {
  readonly id: number;
  readonly item: ItemId;
  readonly qty: number;
  readonly label?: Grade;
  readonly real?: Grade;
}
export interface Box {
  readonly id: number;
  readonly kind: BoxKind;
  /** 【补充案】0 普通 / 1 精良 / 2 稀有 */
  readonly tier: 0 | 1 | 2;
  readonly x: number;
  readonly y: number;
  readonly loot: readonly Loot[];
}
export interface GroundSpot {
  readonly id: number;
  readonly x: number;
  readonly y: number;
  readonly loot: readonly Loot[];
}
export interface EnemySpawn {
  readonly id: number;
  readonly kind: EnemyKind;
  readonly x: number;
  readonly y: number;
  readonly patrol: readonly (readonly [number, number])[];
  readonly corpse: readonly Loot[];
}
export interface MapLayout {
  readonly seed: number;
  readonly theme: Theme;
  readonly area: 'resource' | 'hunt';
  readonly w: number;
  readonly h: number;
  /** 行优先，1 = 墙 */
  readonly walls: readonly number[];
  readonly entry: readonly [number, number];
  readonly extract: readonly [number, number];
  readonly boxes: readonly Box[];
  readonly ground: readonly GroundSpot[];
  readonly enemies: readonly EnemySpawn[];
}
/** 现场改动（随存档保存，回收局沿用） */
export interface MapState {
  opened: number[];
  /** 已揭示组数：boxId → n */
  revealed: Record<number, number>;
  taken: number[];
  dead: number[];
  /** 已搜过的尸体 */
  searched: number[];
}
export const emptyMapState = (): MapState => ({ opened: [], revealed: {}, taken: [], dead: [], searched: [] });

function rng(seed: number) {
  let s = seed | 0;
  const next = () => {
    let t = (s = (s + 0x6d2b79f5) | 0);
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
  const int = (a: number, b: number) => a + Math.floor(next() * (b - a + 1));
  const weighted = <T>(entries: readonly (readonly [T, number])[]): T => {
    const total = entries.reduce((sum, [, w]) => sum + w, 0);
    let roll = next() * total;
    for (const [v, w] of entries) {
      roll -= w;
      if (roll < 0) return v;
    }
    return entries[entries.length - 1]![0];
  };
  return { next, int, weighted };
}
type Rng = ReturnType<typeof rng>;

export function rollGroup(r: Rng, pool: Pool, id: { value: number }, tierBonus = 0): Loot {
  const entries = POOLS[pool];
  const [item, , lo, hi] = r.weighted(entries.map((e) => [e, e[1]] as const));
  let qty: number;
  if (item === 'ammo') qty = r.weighted([[15, 50], [30, 35], [45, 15]] as const);
  else qty = r.int(lo, hi);
  const graded = item === 'water' || item === 'parts' || item === 'bandage';
  const grade: Grade | undefined = graded ? (tierBonus >= 2 ? 2 : (r.weighted([[0, 15], [1, 70], [2, 15]] as const) as Grade)) : undefined;
  return { id: id.value++, item, qty, ...(grade !== undefined ? { label: grade, real: grade } : {}) };
}

export function generateMap(seed: number, area: 'resource' | 'hunt', theme: Theme, experimental: boolean): MapLayout {
  const r = rng(seed ^ 0x5eed);
  const enc = ENCOUNTER[area];
  const W = enc.w;
  const H = enc.h;
  const walls = new Array<number>(W * H).fill(0);
  const set = (x: number, y: number, v = 1) => {
    if (x >= 0 && y >= 0 && x < W && y < H) walls[y * W + x] = v;
  };
  const wall = (x: number, y: number) => x < 0 || y < 0 || x >= W || y >= H || walls[y * W + x] === 1;
  for (let x = 0; x < W; x += 1) {
    set(x, 0);
    set(x, H - 1);
  }
  for (let y = 0; y < H; y += 1) {
    set(0, y);
    set(W - 1, y);
  }
  const entry: [number, number] = [3, Math.floor(H / 2)];
  const extract: [number, number] = [4.5, Math.floor(H / 2) + 6];
  // Buildings: rectangles with two doorways, kept away from the entry strip.
  const rooms: { x: number; y: number; w: number; h: number }[] = [];
  const roomCount = area === 'hunt' ? 7 : 6;
  for (let tries = 0; rooms.length < roomCount && tries < 200; tries += 1) {
    const w = r.int(7, 12);
    const h = r.int(6, 10);
    const x = r.int(12, W - w - 3);
    const y = r.int(3, H - h - 3);
    if (rooms.some((o) => x < o.x + o.w + 3 && o.x < x + w + 3 && y < o.y + o.h + 3 && o.y < y + h + 3)) continue;
    rooms.push({ x, y, w, h });
  }
  for (const room of rooms) {
    for (let x = room.x; x < room.x + room.w; x += 1) {
      set(x, room.y);
      set(x, room.y + room.h - 1);
    }
    for (let y = room.y; y < room.y + room.h; y += 1) {
      set(room.x, y);
      set(room.x + room.w - 1, y);
    }
    // doorways (2 m wide) on two random sides
    const sides = [0, 1, 2, 3].sort(() => r.next() - 0.5).slice(0, 2);
    for (const side of sides) {
      if (side === 0 || side === 2) {
        const dx = r.int(room.x + 2, room.x + room.w - 4);
        const y = side === 0 ? room.y : room.y + room.h - 1;
        set(dx, y, 0);
        set(dx + 1, y, 0);
      } else {
        const dy = r.int(room.y + 2, room.y + room.h - 4);
        const x = side === 1 ? room.x + room.w - 1 : room.x;
        set(x, dy, 0);
        set(x, dy + 1, 0);
      }
    }
  }
  // Scattered cover (wrecks, crates)
  const coverCount = area === 'hunt' ? 26 : 20;
  for (let i = 0; i < coverCount; i += 1) {
    const x = r.int(9, W - 4);
    const y = r.int(2, H - 4);
    if (rooms.some((o) => x >= o.x - 1 && x <= o.x + o.w && y >= o.y - 1 && y <= o.y + o.h)) continue;
    const horizontal = r.next() < 0.5;
    for (let k = 0; k < r.int(2, 3); k += 1) set(horizontal ? x + k : x, horizontal ? y : y + k);
  }
  // Reachability from the entry
  const reach = new Uint8Array(W * H);
  const queue: number[] = [entry[1] * W + entry[0]];
  reach[queue[0]!] = 1;
  while (queue.length) {
    const i = queue.pop()!;
    const x = i % W;
    const y = (i / W) | 0;
    for (const [nx, ny] of [[x + 1, y], [x - 1, y], [x, y + 1], [x, y - 1]] as const) {
      if (wall(nx, ny)) continue;
      const j = ny * W + nx;
      if (reach[j]) continue;
      reach[j] = 1;
      queue.push(j);
    }
  }
  const open = (x: number, y: number) => !wall(x, y) && reach[y * W + x] === 1;
  const freeSpot = (minX: number, pred?: (x: number, y: number) => boolean): [number, number] => {
    for (let tries = 0; tries < 400; tries += 1) {
      const x = r.int(minX, W - 3);
      const y = r.int(2, H - 3);
      if (!open(x, y) || !open(x + 1, y) || !open(x, y + 1)) continue;
      if (pred && !pred(x, y)) continue;
      return [x + 0.5, y + 0.5];
    }
    return [W / 2, H / 2];
  };
  const inRoom = (x: number, y: number) => rooms.some((o) => x > o.x && x < o.x + o.w - 1 && y > o.y && y < o.y + o.h - 1);
  const id = { value: 1 };
  const used: [number, number][] = [entry, extract];
  const far = (x: number, y: number, d: number) => used.every(([ux, uy]) => Math.hypot(ux - x, uy - y) >= d);

  // Enemies: patrol groups of 2–3, spawned ≥ 8 m from the entry〔09·03〕
  const kinds: EnemyKind[] = [
    ...Array<EnemyKind>(enc.melee).fill('melee'),
    ...Array<EnemyKind>(enc.charger).fill('charger'),
    ...Array<EnemyKind>(enc.ranged).fill('ranged'),
  ].sort(() => r.next() - 0.5);
  const enemies: EnemySpawn[] = [];
  const guardPoints: [number, number][] = [];
  while (kinds.length) {
    const size = Math.min(kinds.length, r.int(2, 3));
    const [gx, gy] = freeSpot(16, (x, y) => far(x, y, 10));
    used.push([gx, gy]);
    guardPoints.push([gx, gy]);
    const patrol: [number, number][] = [[gx, gy]];
    for (let k = 0; k < 2; k += 1) {
      const p = freeSpot(14, (x, y) => Math.hypot(x - gx, y - gy) < 14 && Math.hypot(x - gx, y - gy) > 5);
      patrol.push(p);
    }
    for (let k = 0; k < size; k += 1) {
      const kind = kinds.pop()!;
      const corpse: Loot[] = r.next() < CORPSE.empty ? [] : [rollGroup(r, r.weighted(CORPSE.pools), id)];
      enemies.push({ id: id.value++, kind, x: gx + (k - 1) * 1.2, y: gy + ((k % 2) - 0.5), patrol, corpse });
    }
  }

  // Containers〔09·03〕〔09·04〕 + tier【补充案】
  const [cMin, cMax] = enc.containers;
  const count = r.int(cMin, cMax);
  const boxWeights = THEMES[theme].boxes;
  const boxes: Box[] = [];
  for (let i = 0; i < count; i += 1) {
    const kind = r.weighted(BOX_KINDS.map((k, n) => [k, boxWeights[n]!] as const));
    const tier = (experimental ? r.weighted(TIER_WEIGHTS[area].map((w, n) => [n as 0 | 1 | 2, w] as const)) : 0) as 0 | 1 | 2;
    // 高风险箱只放在守卫点附近；其余偏向室内
    const near = kind === 'risk' ? guardPoints[r.int(0, guardPoints.length - 1)] : undefined;
    const [x, y] = freeSpot(10, (px, py) =>
      far(px, py, 4) && (near ? Math.hypot(px - near[0], py - near[1]) < 7 : r.next() < 0.75 ? inRoom(px, py) : true),
    );
    used.push([x, y]);
    const def = BOXES[kind];
    const empty = r.next() < (tier === 1 ? def.empty / 2 : tier === 2 ? 0 : def.empty);
    const groups = tier === 2 ? 2 : empty ? 0 : (r.next() < def.two ? 2 : 1) + (tier === 1 ? 1 : 0);
    const loot: Loot[] = [];
    for (let g = 0; g < groups; g += 1) {
      const pool = theme === 'waterworks' && kind === 'tools' ? 'waterpool' : r.weighted(def.pools);
      loot.push(rollGroup(r, tier === 2 && g === 0 ? 'premium' : pool, id, tier));
    }
    boxes.push({ id: id.value++, kind, tier, x, y, loot });
  }

  // Ground spots: 50% one group〔09·04〕
  const [gMin, gMax] = enc.ground;
  const ground: GroundSpot[] = [];
  for (let i = 0, n = r.int(gMin, gMax); i < n; i += 1) {
    const [x, y] = freeSpot(8, (px, py) => far(px, py, 4));
    used.push([x, y]);
    const loot = r.next() < 0.5 ? [rollGroup(r, r.weighted(THEMES[theme].ground), id)] : [];
    ground.push({ id: id.value++, x, y, loot });
  }

  return { seed, theme, area, w: W, h: H, walls, entry, extract, boxes, ground, enemies };
}

export const isWall = (map: MapLayout, x: number, y: number): boolean => {
  const ix = Math.floor(x);
  const iy = Math.floor(y);
  return ix < 0 || iy < 0 || ix >= map.w || iy >= map.h || map.walls[iy * map.w + ix] === 1;
};

/** 视线：沿线段每 0.25 m 采样墙体 */
export function lineOfSight(map: MapLayout, ax: number, ay: number, bx: number, by: number): boolean {
  const d = Math.hypot(bx - ax, by - ay);
  const steps = Math.ceil(d / 0.25);
  for (let i = 1; i < steps; i += 1) {
    const t = i / steps;
    if (isWall(map, ax + (bx - ax) * t, ay + (by - ay) * t)) return false;
  }
  return true;
}
