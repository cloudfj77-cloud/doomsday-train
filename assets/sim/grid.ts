/**
 * 二维格子仓库〔05·02/02.01〕〔10·07〕：每个仓独立网格，物品 / 堆占完整矩形，可 90° 旋转，
 * 同物品且同品质才合堆到单堆上限；整批入库先在副本上模拟，全部摆得下才一次提交。
 * Pure functions over JSON data; callers commit the returned clone.
 */
import { ITEMS, type Grade, type ItemId } from './data.ts';

export interface Stack {
  readonly id: number;
  readonly item: ItemId;
  /** 基础单位数量（弹药为发数） */
  qty: number;
  x: number;
  y: number;
  rot: boolean;
  /** 【补充案】标称品质 / 真实品质 */
  label?: Grade;
  real?: Grade;
}
export interface Hold {
  /** 0 = 火车头货舱；其余为存储车厢 uid；-1 = 探索背包 */
  readonly id: number;
  w: number;
  h: number;
  stacks: Stack[];
}
/** 待入库的一份货（未定位） */
export interface Lot {
  readonly item: ItemId;
  qty: number;
  label?: Grade;
  real?: Grade;
}

export const dims = (item: ItemId, rot: boolean): readonly [number, number] =>
  rot ? [ITEMS[item].h, ITEMS[item].w] : [ITEMS[item].w, ITEMS[item].h];

export const sameKind = (a: { item: ItemId; label?: Grade; real?: Grade }, b: { item: ItemId; label?: Grade; real?: Grade }): boolean =>
  a.item === b.item && (a.label ?? 1) === (b.label ?? 1) && (a.real ?? 1) === (b.real ?? 1);

export const clone = <T>(value: T): T => JSON.parse(JSON.stringify(value)) as T;

/** 矩形是否全部落在开放空格（可忽略某一堆，用于移动自身） */
export function fits(hold: Hold, x: number, y: number, w: number, h: number, ignore?: number): boolean {
  if (x < 0 || y < 0 || x + w > hold.w || y + h > hold.h) return false;
  for (const s of hold.stacks) {
    if (s.id === ignore) continue;
    const [sw, sh] = dims(s.item, s.rot);
    if (x < s.x + sw && s.x < x + w && y < s.y + sh && s.y < y + h) return false;
  }
  return true;
}

/** 找第一个能放下的位置：先横放，再旋转 */
export function findSpot(hold: Hold, item: ItemId): { x: number; y: number; rot: boolean } | null {
  for (const rot of ITEMS[item].w === ITEMS[item].h ? [false] : [false, true]) {
    const [w, h] = dims(item, rot);
    for (let y = 0; y + h <= hold.h; y += 1) for (let x = 0; x + w <= hold.w; x += 1) if (fits(hold, x, y, w, h)) return { x, y, rot };
  }
  return null;
}

export const usedCells = (hold: Hold): number =>
  hold.stacks.reduce((sum, s) => sum + ITEMS[s.item].w * ITEMS[s.item].h, 0);

export const countIn = (holds: readonly Hold[], item: ItemId, pred?: (s: Stack) => boolean): number =>
  holds.reduce((sum, h) => sum + h.stacks.filter((s) => s.item === item && (!pred || pred(s))).reduce((a, s) => a + s.qty, 0), 0);

/**
 * 把若干份货放进仓（先合堆，再开新堆）。`order` 是尝试顺序（仓 id）。
 * 返回新的仓副本；任何一份放不下返回 null（原子性）。
 */
export function placeLots(holds: readonly Hold[], lots: readonly Lot[], nextId: { value: number }, order?: readonly number[]): Hold[] | null {
  const out = clone(holds) as Hold[];
  const ordered = order ? [...order.map((id) => out.find((h) => h.id === id)!).filter(Boolean), ...out.filter((h) => !order.includes(h.id))] : out;
  for (const lot of lots) {
    let left = lot.qty;
    const max = ITEMS[lot.item].stack;
    for (const hold of ordered) {
      for (const s of hold.stacks) {
        if (left <= 0) break;
        if (!sameKind(s, lot) || s.qty >= max) continue;
        const add = Math.min(max - s.qty, left);
        s.qty += add;
        left -= add;
      }
    }
    while (left > 0) {
      let placed = false;
      for (const hold of ordered) {
        const spot = findSpot(hold, lot.item);
        if (!spot) continue;
        const add = Math.min(max, left);
        hold.stacks.push({
          id: nextId.value++,
          item: lot.item,
          qty: add,
          ...spot,
          ...(lot.label !== undefined ? { label: lot.label, real: lot.real ?? lot.label } : {}),
        });
        left -= add;
        placed = true;
        break;
      }
      if (!placed) return null;
    }
  }
  return out;
}

/**
 * 从仓里取走 qty 份某物品。prefer：'high' 先取标称品质高的，'low' 先取低的。
 * 返回新仓与取出的明细；库存不足返回 null。
 */
export function takeItems(
  holds: readonly Hold[],
  item: ItemId,
  qty: number,
  prefer: 'high' | 'low' = 'low',
  pred?: (s: Stack) => boolean,
): { holds: Hold[]; taken: Lot[] } | null {
  const out = clone(holds) as Hold[];
  const all = out.flatMap((h) => h.stacks.filter((s) => s.item === item && (!pred || pred(s))).map((s) => ({ h, s })));
  all.sort((a, b) => ((a.s.label ?? 1) - (b.s.label ?? 1)) * (prefer === 'high' ? -1 : 1) || a.s.qty - b.s.qty);
  let left = qty;
  const taken: Lot[] = [];
  for (const { h, s } of all) {
    if (left <= 0) break;
    const n = Math.min(s.qty, left);
    s.qty -= n;
    left -= n;
    const prev = taken.find((t) => sameKind(t, s));
    if (prev) prev.qty += n;
    else taken.push({ item, qty: n, ...(s.label !== undefined ? { label: s.label, real: s.real ?? s.label } : {}) });
    if (s.qty === 0) h.stacks = h.stacks.filter((x) => x !== s);
  }
  if (left > 0) return null;
  return { holds: out, taken };
}

export function findStack(holds: readonly Hold[], id: number): { hold: Hold; stack: Stack } | undefined {
  for (const hold of holds) {
    const stack = hold.stacks.find((s) => s.id === id);
    if (stack) return { hold, stack };
  }
  return undefined;
}

/**
 * 拖动一堆到目标仓坐标（可旋转）。落在同类堆上则合堆（满了剩余留在原处）。
 * 返回新仓副本或 null（碰撞 / 越界）。
 */
export function moveStack(holds: readonly Hold[], id: number, to: number, x: number, y: number, rot: boolean): Hold[] | null {
  const out = clone(holds) as Hold[];
  const src = findStack(out, id);
  const dst = out.find((h) => h.id === to);
  if (!src || !dst) return null;
  const { stack } = src;
  const target = dst.stacks.find((s) => {
    if (s.id === id) return false;
    const [sw, sh] = dims(s.item, s.rot);
    return x >= s.x && x < s.x + sw && y >= s.y && y < s.y + sh;
  });
  if (target && sameKind(target, stack)) {
    const room = ITEMS[stack.item].stack - target.qty;
    if (room <= 0) return null;
    const n = Math.min(room, stack.qty);
    target.qty += n;
    stack.qty -= n;
    if (stack.qty === 0) src.hold.stacks = src.hold.stacks.filter((s) => s.id !== id);
    return out;
  }
  const [w, h] = dims(stack.item, rot);
  if (!fits(dst, x, y, w, h, src.hold === dst ? id : undefined)) return null;
  src.hold.stacks = src.hold.stacks.filter((s) => s.id !== id);
  stack.x = x;
  stack.y = y;
  stack.rot = rot;
  dst.stacks.push(stack);
  return out;
}

/** 拆半堆到同仓或其他仓空位 */
export function splitStack(holds: readonly Hold[], id: number, nextId: { value: number }): Hold[] | null {
  const out = clone(holds) as Hold[];
  const src = findStack(out, id);
  if (!src || src.stack.qty < 2) return null;
  const half = Math.floor(src.stack.qty / 2);
  for (const hold of [src.hold, ...out.filter((h) => h !== src.hold)]) {
    const spot = findSpot(hold, src.stack.item);
    if (!spot) continue;
    src.stack.qty -= half;
    hold.stacks.push({ ...clone(src.stack), id: nextId.value++, qty: half, ...spot });
    return out;
  }
  return null;
}

export const holdLoad = (hold: Hold): number => hold.stacks.reduce((sum, s) => sum + s.qty * ITEMS[s.item].load, 0);
