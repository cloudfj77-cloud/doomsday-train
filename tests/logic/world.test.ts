import { describe, it, expect } from 'vitest';
import { createState, step, project, validateConfig } from '../../scripts/logic/world.ts';
import data from '../../config/gameplay/world.json';
describe('real simulation', () => {
  it('loads JSON and moves from input', () => {
    const c = validateConfig(data), start = createState(c);
    expect(step(start, [1, 0], 1, c).x).toBeGreaterThan(start.x);
    expect(step(start, [0, 0], 1, c)).toEqual(start);
    expect(start.x).toBe(c.start[0]);
  });
  it('rejects broken config, then accepts restored config', () => {
    for (const broken of [null, { ...data, speed: -1 }, { ...data, start: [Infinity, 0] }, { ...data, limit: 0 }]) expect(() => validateConfig(broken)).toThrow('invalid-world-config');
    expect(() => validateConfig(data)).not.toThrow();
  });
  it('rejects broken inputs and exposes an immutable snapshot', () => {
    const c = validateConfig(data), s = createState(c);
    expect(() => step(s, [NaN, 0], 1, c)).toThrow();
    expect(() => step(s, [0, 0], -1, c)).toThrow();
    expect(Object.isFrozen(project(s))).toBe(true);
    expect(Math.abs(step(s, [1, 1], 100, c).x)).toBeLessThanOrEqual(c.limit);
  });
});


describe('railway movement', () => {
  const config = validateConfig(data);
  const spawn = createState(config);
  it('rejects a spawn inside the rock and duplicate landmark identities', () => {
    expect(() => validateConfig({ ...data, start: [0, 0] })).toThrow();
    expect(() => validateConfig({ ...data, obstacles: [data.obstacles[0], data.obstacles[0]] })).toThrow();
  });
  it('sweeps the whole segment and stops walking on contact', () => {
    const hit = step({ ...spawn, x: -4, z: 0 }, [1, 0], 10, config);
    expect(hit.x).toBeCloseTo(-1.22);
    const pressed = step(hit, [1, 0], 1, config);
    expect(pressed.moving).toBe(false);
    expect(pressed.walkDistance).toBe(hit.walkDistance);
    const depot = step({ ...spawn, x: 3, z: 2 }, [0, -1], 10, config);
    expect(depot.z).toBeCloseTo(-2.28);
  });
  it('slides beside the rock, retaining facing and stride when input is released', () => {
    const start = { ...spawn, x: -2, z: 0.8 };
    const slide = step(start, [1, -1], 0.4, config);
    expect(slide.x).toBeCloseTo(-1.22);
    expect(slide.z).toBeLessThan(-0.3);
    expect(slide.facingX).toBeCloseTo(Math.SQRT1_2);
    expect(slide.facingZ).toBeCloseTo(-Math.SQRT1_2);
    expect(slide.walkDistance).toBeGreaterThan(0);
    expect(step(slide, [0, 0], 1, config)).toEqual({ ...slide, moving: false });
  });
  it('reaches the depot approach around either side of the outcrop', () => {
    for (const x of [-2, 2]) {
      let s = step(spawn, [1, 0], (x - spawn.x) / config.speed, config);
      s = step(s, [0, -1], 5 / config.speed, config);
      expect(s.z).toBeCloseTo(-2);
      s = step(s, [1, 0], (3 - x) / config.speed, config);
      expect(s.x).toBeCloseTo(3);
    }
  });
});
