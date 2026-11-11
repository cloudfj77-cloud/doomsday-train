import { describe, it, expect } from 'vitest';
import { createState, step, project, validateConfig } from './world.ts';
import data from '../../assets/world.json';
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
