type Pair = [number, number];
interface Obstacle { id: string; center: Pair; size: Pair }
export interface Config { speed: number; limit: number; radius: number; start: Pair; obstacles: Obstacle[] }
const pair = (p: unknown): p is Pair => Array.isArray(p) && p.length === 2 && p.every(Number.isFinite);
const positive = (v: number) => Number.isFinite(v) && v > 0;
function bounds(o: Obstacle, axis: number, radius: number): Pair {
  const extent = o.size[axis] / 2 + radius;
  return [o.center[axis] - extent, o.center[axis] + extent];
}
export function validateConfig(value: unknown): Config {
  const c = value as Config;
  if (!c || !positive(c.speed) || !positive(c.limit) || !positive(c.radius) || !pair(c.start) ||
      c.start.some(n => Math.abs(n) > c.limit) || !Array.isArray(c.obstacles)) throw new Error('invalid-world-config');
  const ids = new Set<string>();
  for (const o of c.obstacles) {
    if (!o || typeof o.id !== 'string' || !o.id.trim() || ids.has(o.id) || !pair(o.center) || !pair(o.size) ||
        !o.size.every(positive)) throw new Error('invalid-world-config');
    if (c.start.every((n, axis) => {
      const [low, high] = bounds(o, axis, c.radius);
      return n > low && n < high;
    })) throw new Error('invalid-world-config');
    ids.add(o.id);
  }
  return c;
}
export function createState(c: Config) {
  return { x: c.start[0], z: c.start[1], facingX: 0, facingZ: 1, moving: false, walkDistance: 0 };
}
export type State = ReturnType<typeof createState>;

// Sweep the player's square footprint against both slabs of each obstacle.
function travel(origin: Pair, delta: Pair, c: Config): { end: Pair; axis: number; distance: number } {
  let time = 1, normal = -1, contact = 0;
  for (let axis = 0; axis < 2; axis++) {
    if (!delta[axis]) continue;
    const edge = Math.sign(delta[axis]) * c.limit;
    const t = (edge - origin[axis]) / delta[axis];
    if (t < time) { time = t; normal = axis; contact = edge; }
  }
  for (const o of c.obstacles) {
    let enter = -Infinity, exit = Infinity, axisHit = -1, face = 0;
    for (let axis = 0; axis < 2; axis++) {
      const [low, high] = bounds(o, axis, c.radius);
      const position = origin[axis], velocity = delta[axis];
      if (!velocity) {
        if (position <= low || position >= high) exit = -Infinity;
        continue;
      }
      const near = velocity > 0 ? low : high;
      const far = velocity > 0 ? high : low;
      const t = (near - position) / velocity;
      if (t > enter) { enter = t; axisHit = axis; face = near; }
      exit = Math.min(exit, (far - position) / velocity);
    }
    if (enter >= 0 && enter < time && enter < exit) {
      time = enter; normal = axisHit; contact = face;
    }
  }
  const end: Pair = [origin[0] + delta[0] * time, origin[1] + delta[1] * time];
  if (normal >= 0) end[normal] = contact;
  return { end, axis: normal, distance: Math.hypot(end[0] - origin[0], end[1] - origin[1]) };
}
export function step(state: State, input: readonly [number, number], dt: number, c: Config): State {
  if (!Number.isFinite(dt) || dt < 0 || !input.every(Number.isFinite)) throw new Error('invalid-input');
  const length = Math.hypot(...input);
  if (!length || !dt) return { ...state, moving: false };
  const facingX = input[0] / length, facingZ = input[1] / length;
  const distance = c.speed * dt * Math.min(1, length);
  const delta: Pair = [facingX * distance, facingZ * distance];
  const hit = travel([state.x, state.z], delta, c);
  let end = hit.end, traveled = hit.distance;
  if (hit.axis >= 0) {
    const remainder: Pair = [delta[0] - (end[0] - state.x), delta[1] - (end[1] - state.z)];
    remainder[hit.axis] = 0;
    const slide = travel(end, remainder, c);
    end = slide.end;
    traveled += slide.distance;
  }
  return { x: end[0], z: end[1], facingX, facingZ, moving: traveled > 0, walkDistance: state.walkDistance + traveled };
}
export function project(state: State): Readonly<State> { return Object.freeze({ ...state }); }
