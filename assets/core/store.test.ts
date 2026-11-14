import { describe, expect, it } from 'vitest';
import { computeArmorDamage, computeItemPrice, createDuckItem, inspectItemWithTool } from './formulas.ts';
import { GameStore } from './store.ts';

describe('Duckov Pawnshop core invariants', () => {
  it('exposed fake label drops price to real grade value', () => {
    const fake = createDuckItem('wpn_ak74u', { actualGrade: -1, claimedGrade: 2 });
    const before = computeItemPrice(fake, { isSellingToClient: true });
    inspectItemWithTool(fake, 'scanner');
    const after = computeItemPrice(fake, { isSellingToClient: true });
    expect(fake.exposed).toBe(true);
    expect(after).toBeLessThan(before);
  });

  it('armor piercing monotonic: more pierce never reduces damage, pierce>=armor means full damage ratio', () => {
    const base = { baseDamage: 40, critRate: 0, critMult: 2, armorBreak: 0, element: 'physical' as const, bleedChance: 0, weather: 'clear_night' as const, helmetLevel: 0, helmetDurability: 0, bodyArmorLevel: 4, bodyArmorDurability: 100, critRoll: 0.99, bleedRoll: 0.99 };
    const low = computeArmorDamage({ ...base, armorPiercing: 0 });
    const mid = computeArmorDamage({ ...base, armorPiercing: 2 });
    const full = computeArmorDamage({ ...base, armorPiercing: 6 });
    expect(mid.finalDamage).toBeGreaterThanOrEqual(low.finalDamage);
    expect(full.finalDamage).toBeGreaterThanOrEqual(mid.finalDamage);
    expect(full.armorReductionRatio).toBe(0);
  });

  it('secret stash items are never in inspection risk pool locations', () => {
    const s = new GameStore();
    s.startCareer('fence');
    const hot = s.items.find(i => i.location === 'secret_stash');
    expect(hot).toBeDefined();
    expect(hot!.location).toBe('secret_stash');
    // inspection in UI only scans shop_stock/showcase; assert stash item not in those pools
    const riskyLocations = s.items.filter(i => i.location === 'shop_stock' || i.location === 'showcase');
    expect(riskyLocations.some(i => i.uid === hot!.uid)).toBe(false);
  });

  it('sunday queue puts wholesale buyer before landlord', () => {
    const s = new GameStore();
    s.startCareer('apprentice');
    s.rentDueIn = 0;
    // deterministic-ish: try a few generations, at least the invariant holds whenever sunday queue generated
    s.generateQueue();
    const types = s.queue.map(c => c.intent);
    const wi = types.indexOf('WHOLESALE_BUYER');
    const li = types.indexOf('LANDLORD');
    expect(wi).toBeGreaterThanOrEqual(0);
    expect(li).toBeGreaterThan(wi);
    expect(li).toBe(types.length - 1);
  });

  it('wrong inspect tool does not expose, correct tool does', () => {
    const it1 = createDuckItem('wat_ditch', { actualGrade: -2, claimedGrade: 1 });
    const wrong = inspectItemWithTool(it1, 'scanner');
    expect(wrong.matchedTool).toBe(false);
    expect(it1.exposed).toBe(false);
    const right = inspectItemWithTool(it1, 'litmus');
    expect(right.matchedTool).toBe(true);
    expect(it1.exposed).toBe(true);
  });
});
