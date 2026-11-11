export interface Config { speed: number; limit: number; start: [number, number] }
export function validateConfig(value: unknown): Config {
  const c = value as Config;
  if (!c || !Number.isFinite(c.speed) || c.speed <= 0 || !Number.isFinite(c.limit) || c.limit <= 0 || !Array.isArray(c.start) || c.start.length !== 2 || !c.start.every(n => Number.isFinite(n) && Math.abs(n) <= c.limit)) throw new Error('invalid-world-config');
  return c;
}
export function createState(c: Config) { return { x: c.start[0], z: c.start[1] }; }
export type State = ReturnType<typeof createState>;
export function step(state: State, input: readonly [number, number], dt: number, c: Config): State {
  if (!Number.isFinite(dt) || dt < 0 || !input.every(Number.isFinite)) throw new Error('invalid-input');
  const length = Math.max(1, Math.hypot(...input));
  const bound = (n: number) => Math.max(-c.limit, Math.min(c.limit, n));
  return { x: bound(state.x + input[0] / length * c.speed * dt), z: bound(state.z + input[1] / length * c.speed * dt) };
}
export function project(state: State): Readonly<State> { return Object.freeze({ ...state }); }
