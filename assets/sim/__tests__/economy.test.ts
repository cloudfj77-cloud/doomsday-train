import { describe, expect, it } from 'vitest';
import { RULES } from '../data.ts';
import { countIn, fits, moveStack, placeLots, takeItems, type Hold } from '../grid.ts';
import { generateMap } from '../explore-map.ts';
import {
  arrive,
  boardPassenger,
  buyBatch,
  buyCar,
  buySingle,
  chooseRoute,
  closeBill,
  collectOutput,
  count,
  depart,
  dilute,
  endDay,
  endSortie,
  haggle,
  haggleOdds,
  harvest,
  hireDriver,
  inspect,
  installDevice,
  load,
  newGame,
  nodesLeft,
  paySettlement,
  plant,
  previewSale,
  retailPrice,
  routeActive,
  routeDone,
  sellToBuyer,
  sellToRider,
  startJob,
  startSortie,
  type Game,
} from '../economy.ts';

const head = (): Hold => ({ id: 0, w: 4, h: 2, stacks: [] });
const pureIdx = (g: Game, len?: number) =>
  g.routeOptions!.findIndex((r) => (len === undefined || r.length === len) && r.every((n) => n.kind === 'rich' || n.kind === 'slum'));
const runRoute = (g: Game) => {
  while (!routeDone(g)) {
    expect(depart(g).ok).toBe(true);
    expect(arrive(g).ok).toBe(true);
  }
};

describe('grid warehouse', () => {
  it('places the doc example: 2 water + 1 ration + 4 parts in 4×2〔05·02〕', () => {
    const holds = placeLots([head()], [{ item: 'water', qty: 2 }, { item: 'ration', qty: 1 }, { item: 'parts', qty: 4 }], { value: 1 })!;
    expect(holds).not.toBeNull();
    expect(holds[0]!.stacks.length).toBe(5);
    expect(countIn(holds, 'parts')).toBe(4);
  });
  it('scattered free cells cannot hold a 2×1 item', () => {
    const hold: Hold = { id: 0, w: 2, h: 2, stacks: [{ id: 1, item: 'ration', qty: 1, x: 0, y: 0, rot: false }, { id: 2, item: 'ration', qty: 1, x: 1, y: 1, rot: false }] };
    expect(fits(hold, 0, 1, 2, 1)).toBe(false);
    expect(placeLots([hold], [{ item: 'water', qty: 1 }], { value: 9 })).toBeNull();
  });
  it('rotates to fit', () => {
    const placed = placeLots([{ id: 0, w: 1, h: 2, stacks: [] }], [{ item: 'water', qty: 1 }], { value: 1 })!;
    expect(placed[0]!.stacks[0]!.rot).toBe(true);
  });
  it('61 rounds of ammo take 2 cells〔10·07〕', () => {
    expect(placeLots([head()], [{ item: 'ammo', qty: 61 }], { value: 1 })![0]!.stacks.length).toBe(2);
  });
  it('merges onto a same-kind stack when dragged onto it', () => {
    const holds = placeLots([head()], [{ item: 'parts', qty: 1 }], { value: 1 })!;
    holds[0]!.stacks.push({ id: 50, item: 'parts', qty: 1, x: 3, y: 1, rot: false });
    const moved = moveStack(holds, 50, 0, 0, 0, false)!;
    expect(moved[0]!.stacks.filter((s) => s.item === 'parts').length).toBe(1);
    expect(moved[0]!.stacks.find((s) => s.item === 'parts')!.qty).toBe(2);
  });
  it('take is atomic', () => {
    const holds = placeLots([head()], [{ item: 'parts', qty: 2 }], { value: 1 })!;
    expect(takeItems(holds, 'parts', 3)).toBeNull();
    expect(countIn(takeItems(holds, 'parts', 2)!.holds, 'parts')).toBe(0);
  });
});

describe('economy v0.2 (team doc rules, experiments off)', () => {
  it('starts with the doc opening state〔05·01/01.01〕', () => {
    const g = newGame(7, false);
    expect(g.cash).toBe(500);
    expect(g.debt).toBe(6000);
    expect(g.holds[0]!.w * g.holds[0]!.h).toBe(8);
    expect(g.kit).toEqual({ gun: 'pistol', ammo: 60 });
    expect(load(g)).toBe(1.5);
    expect(g.station.customers.length).toBe(3);
    expect(g.station.customers[0]!.goods!.map((x) => [x.item, x.qty])).toEqual([['water', 2], ['ration', 1]]);
    expect(g.station.driver?.grade).toBe(1);
    const opts = g.routeOptions!;
    expect(opts.length).toBe(3);
    expect(opts.some((r) => r.length === 2)).toBe(true);
    expect(opts.some((r) => r.length === 3)).toBe(true);
    expect(pureIdx(g)).toBeGreaterThanOrEqual(0);
    for (const r of opts) expect(['rich', 'slum']).toContain(r[r.length - 1]!.kind);
  });

  it('first day, locomotive only, 3 segments costs 21 + 9 + 4 = 34〔05·01/01.03〕', () => {
    for (let seed = 1; seed < 50; seed += 1) {
      const g = newGame(seed, false);
      const idx = g.routeOptions!.findIndex((r) => r.length === 3);
      chooseRoute(g, idx);
      runRoute(g);
      expect(endDay(g).ok).toBe(true);
      expect(g.bill!.fuel).toBe(21);
      expect(g.bill!.wages).toBe(9);
      expect(g.bill!.living).toBe(4);
      expect(g.cash).toBe(466);
    }
  });

  it('single and batch purchase follow ceil(V×a) and the batch discount〔07·02〕', () => {
    const g = newGame(3, false);
    const c = g.station.customers[0]!;
    const total = c.goods!.reduce((s, x) => s + x.unit * x.qty, 0);
    if (c.batchPrice !== undefined) expect(c.batchPrice).toBe(Math.ceil(total * (1 - c.discount!)));
    expect(buySingle(g, c.id, c.goods!.map((x) => x.qty)).ok).toBe(true);
    expect(count(g, 'water')).toBe(2);
    expect(g.cash).toBe(500 - total);
    expect(buySingle(g, c.id, [1, 0]).ok).toBe(false);
  });

  it('retail price is floor(V×1.2)〔07·03〕', () => {
    const g = newGame(1, false);
    expect(retailPrice(g, 'pistol')).toBe(144);
    expect(retailPrice(g, 'parts', 1)).toBe(24);
  });

  it('a driver hired today drives from tomorrow〔05·01/01.04〕', () => {
    const g = newGame(5, false);
    expect(hireDriver(g).ok).toBe(true);
    expect(g.cash).toBe(320);
    expect(nodesLeft(g)).toBe(3);
    chooseRoute(g, pureIdx(g, 2));
    runRoute(g);
    endDay(g);
    expect(g.bill!.living).toBe(6);
    closeBill(g);
    expect(g.driver).toBe(1);
    expect(nodesLeft(g)).toBe(4);
    expect(g.today.wageRate).toBe(5);
  });

  it('an early stop carries the route over to the next day〔06·02〕', () => {
    let tested = false;
    for (let seed = 1; seed < 60 && !tested; seed += 1) {
      const g = newGame(seed, false);
      const idx = pureIdx(g, 3);
      if (idx < 0) continue;
      chooseRoute(g, idx);
      depart(g);
      arrive(g);
      expect(endDay(g).ok).toBe(true);
      closeBill(g);
      expect(routeActive(g)).toBe(true);
      expect(g.routeOptions).toBeUndefined();
      expect(depart(g).ok).toBe(true);
      tested = true;
    }
    expect(tested).toBe(true);
  });

  it('settlement on day 5 requires 600 repaid〔11·04〕', () => {
    const g = newGame(9, false);
    for (let day = 1; day <= 5; day += 1) {
      const idx = pureIdx(g);
      chooseRoute(g, idx >= 0 ? idx : 0);
      runRoute(g);
      g.pending = [];
      if (g.station.site) g.station.site.closed = true;
      endDay(g);
      closeBill(g);
    }
    expect(g.phase).toBe('settlement');
    expect(g.settlement!.due).toBe(600);
    g.cash = 700;
    expect(paySettlement(g).ok).toBe(true);
    expect(g.repaid).toBe(600);
    expect(g.cash).toBe(100);
  });

  it('overdue riders are refunded with compensation〔05·03/03.02〕', () => {
    let tested = false;
    for (let seed = 1; seed < 200 && !tested; seed += 1) {
      const g = newGame(seed, false);
      const idx = pureIdx(g, 3);
      if (idx < 0) continue;
      chooseRoute(g, idx);
      const cand = g.station.passengers.find((p) => p.deadline === 1);
      if (!cand || !boardPassenger(g, cand.id).ok) continue;
      const rider = g.riders[0]!;
      endDay(g);
      expect(g.bill!.overdue).toBe(1);
      expect(g.bill!.refunds).toBe(rider.fare + Math.ceil(rider.fare * rider.comp - 1e-9));
      tested = true;
    }
    expect(tested).toBe(true);
  });

  it('planting and processing advance only with real segments〔05·04〕〔05·06〕', () => {
    const g = newGame(17, false);
    g.cash = 5000;
    expect(buyCar(g, 'planting').ok).toBe(true);
    expect(buyCar(g, 'processing').ok).toBe(true);
    g.holds.push({ id: 999, w: 9, h: 4, stacks: [] });
    g.holds = placeLots(g.holds, [{ item: 'seed', qty: 2 }, { item: 'water', qty: 2 }, { item: 'scrap', qty: 6 }], { value: 5000 })!;
    g.nextId = 6000;
    const planting = g.cars.find((c) => c.kind === 'planting')!;
    const processing = g.cars.find((c) => c.kind === 'processing')!;
    expect(plant(g, planting.uid, 0).ok).toBe(true);
    expect(installDevice(g, processing.uid, 'furnace').ok).toBe(true);
    expect(startJob(g, processing.devices![0]!.uid, 'ingot').ok).toBe(true);
    expect(harvest(g, planting.uid, 0).ok).toBe(false);
    chooseRoute(g, g.routeOptions!.findIndex((r) => r.length === 3));
    runRoute(g);
    expect(harvest(g, planting.uid, 0).ok).toBe(true);
    expect(count(g, 'veg')).toBe(4);
    expect(collectOutput(g, processing.devices![0]!.uid).ok).toBe(true);
    expect(count(g, 'ingot')).toBe(4);
    endDay(g);
    expect(g.bill!.upkeep).toBe(6);
  });
});

describe('【补充案】experiments', () => {
  it('haggle odds match the public formula', () => {
    const g = newGame(1, true);
    expect(haggleOdds(g, 10)).toEqual({ accept: 80, stay: 8, walk: 12 });
    expect(haggleOdds(g, 25)).toEqual({ accept: 50, stay: 20, walk: 30 });
    g.rep.rich = -30;
    expect(haggleOdds(g, 10).accept).toBe(70);
  });
  it('haggle resolves once per customer', () => {
    const g = newGame(2, true);
    const c = g.station.customers[0]!;
    expect(haggle(g, c.id, 10).ok).toBe(true);
    expect(haggle(g, c.id, 10).ok).toBe(false);
  });
  it('inspection reveals fakes and reprices them', () => {
    let found = false;
    for (let seed = 1; seed < 400 && !found; seed += 1) {
      const g = newGame(seed, true);
      const c = g.station.customers[0]!;
      const fake = c.goods!.find((x) => (x.label ?? 1) > (x.real ?? 1));
      if (!fake) continue;
      const before = fake.unit;
      const r = inspect(g, c.id);
      expect(r.ok && r.note).toBe('fake');
      expect(fake.unit).toBeLessThan(before);
      found = true;
    }
    expect(found).toBe(true);
  });
  it('diluted water sells at the labelled price unless caught', () => {
    const g = newGame(4, true);
    g.holds = placeLots(g.holds, [{ item: 'water', qty: 1, label: 1, real: 1 }, { item: 'sewage', qty: 1 }], { value: 900 })!;
    g.nextId = 1000;
    expect(dilute(g).ok).toBe(true);
    expect(count(g, 'water')).toBe(2);
    const fake = g.holds[0]!.stacks.find((s) => s.item === 'water')!;
    expect([fake.label, fake.real]).toEqual([1, 0]);
    g.station.customers.push({ id: 77, kind: 'buyer', car: 0, needs: [{ item: 'water', qty: 2, price: 14 }], budget: 100 });
    expect(previewSale(g, g.station.customers.at(-1)!).fake).toBe(true);
    const r = sellToBuyer(g, 77);
    expect(r.ok).toBe(true);
    if (r.ok && r.note === 'caught') expect(g.rep.rich).toBeLessThan(0);
    else expect(g.cheats.rich).toBe(1);
  });
});

describe('exploration', () => {
  it('generates the doc encounter deterministically〔09·03〕', () => {
    const a = generateMap(42, 'resource', 'yard', true);
    expect(JSON.stringify(a)).toBe(JSON.stringify(generateMap(42, 'resource', 'yard', true)));
    expect(a.enemies.filter((e) => e.kind === 'melee').length).toBe(6);
    expect(a.enemies.filter((e) => e.kind === 'charger').length).toBe(1);
    expect(a.enemies.filter((e) => e.kind === 'ranged').length).toBe(1);
    expect(a.boxes.length).toBeGreaterThanOrEqual(4);
    expect(a.boxes.length).toBeLessThanOrEqual(6);
    for (const e of a.enemies) expect(Math.hypot(e.x - a.entry[0], e.y - a.entry[1])).toBeGreaterThanOrEqual(8);
    expect(generateMap(7, 'hunt', 'hunt', true).enemies.length).toBe(12);
  });
  it('death leaves a lost bag with one recovery; extraction stores loot〔08·02〕', () => {
    const g = newGame(31, false);
    g.station = { ...g.station, kind: 'resource', customers: [], passengers: [], site: { seed: 1, area: 'resource', theme: 'yard', sorties: 0, closed: false, map: { opened: [], revealed: {}, taken: [], dead: [], searched: [] }, recovery: false } };
    expect(startSortie(g, 0).ok).toBe(true);
    expect(g.phase).toBe('explore');
    endSortie(g, { outcome: 'death', hp: 0, gun: 'pistol', ammo: 30, backpack: [{ item: 'parts', qty: 2 }], map: g.station.site!.map, fired: true, x: 10, y: 10 });
    expect(g.hp).toBe(RULES.hpAfterDeath);
    expect(g.kit.gun).toBeNull();
    expect(g.station.site!.lost!.items.length).toBe(1);
    expect(startSortie(g, 0).ok).toBe(true);
    endSortie(g, { outcome: 'extract', hp: 55, gun: 'pistol', ammo: 30, backpack: [{ item: 'parts', qty: 2 }, { item: 'scrap', qty: 3 }], map: g.station.site!.map, fired: true });
    expect(g.kit.gun).toBe('pistol');
    expect(count(g, 'parts')).toBe(2);
    expect(g.station.site!.closed).toBe(true);
    expect(startSortie(g, 0).ok).toBe(false);
    expect(g.today.fired).toEqual(['pistol']);
  });
});

describe('long greedy run', () => {
  it('never throws and keeps stacks valid', () => {
    for (const seed of [1, 2, 3]) {
      const g: Game = newGame(seed, true);
      for (let step = 0; step < 600 && g.phase !== 'lost' && g.day < 12; step += 1) {
        if (g.phase === 'station') {
          for (const c of g.station.customers) if (!c.outcome && c.kind === 'buyer') sellToBuyer(g, c.id);
          for (const r of g.riders) if (r.want && !r.want.sold) sellToRider(g, r.id);
          for (const p of g.station.passengers) if (!p.status && p.dest !== undefined) boardPassenger(g, p.id);
          for (const c of g.station.customers) if (!c.outcome && c.kind === 'supplier' && c.batchPrice !== undefined && g.cash > c.batchPrice + 150) buyBatch(g, c.id);
          if (g.station.site) g.station.site.closed = true;
          if (routeActive(g)) {
            if (!depart(g).ok) endDay(g);
          } else if (g.routeOptions?.length) chooseRoute(g, 0);
          else endDay(g);
        } else if (g.phase === 'travel') arrive(g);
        else if (g.phase === 'dayEnd') closeBill(g);
        else if (g.phase === 'settlement') {
          if (!paySettlement(g).ok) g.phase = 'lost';
        } else if (g.phase === 'won') g.phase = 'station';
        for (const h of g.holds) for (const s of h.stacks) expect(s.qty).toBeGreaterThan(0);
      }
      expect(g.day).toBeGreaterThan(3);
    }
  });
});
