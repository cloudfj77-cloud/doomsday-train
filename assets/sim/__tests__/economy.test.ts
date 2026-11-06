import { describe, expect, it } from 'vitest';
import { ITEMS, RULES } from '../data.ts';
import {
  arrive,
  boardPassenger,
  buyBatch,
  buyCar,
  buySingle,
  capacity,
  cellsFor,
  chooseRoute,
  closeBill,
  depart,
  endDay,
  forfeit,
  newGame,
  paySettlement,
  previewSale,
  repay,
  retailPrice,
  routeDone,
  sellToBuyer,
  sellToRider,
  type Game,
} from '../economy.ts';

/** A simple greedy player: sell what buyers want, buy cheap batches that fit, fill seats. */
function playStation(game: Game): void {
  for (const c of game.station.customers) if (c.kind === 'buyer' && !c.outcome && previewSale(game, c).total > 0) sellToBuyer(game, c.id);
  for (const r of game.riders) if (r.want && !r.want.sold) sellToRider(game, r.id);
  for (const p of game.station.passengers) if (!p.boarded && p.dest !== undefined) boardPassenger(game, p.id);
  for (const c of game.station.customers) {
    if (c.kind !== 'supplier' || c.outcome) continue;
    const valueOf = c.goods!.reduce((s, g) => s + g.qty * retailPrice(g.item, undefined), 0);
    if (c.batchPrice !== undefined && c.batchPrice < valueOf * 0.9 && game.cash - c.batchPrice > 120) buyBatch(game, c.id);
  }
}

function playDay(game: Game): void {
  expect(game.phase).toBe('depot');
  expect(chooseRoute(game, 2).ok || chooseRoute(game, 1).ok).toBe(true);
  playStation(game);
  while (!routeDone(game)) {
    expect(depart(game).ok).toBe(true);
    expect(arrive(game).ok).toBe(true);
    playStation(game);
  }
  expect(endDay(game).ok).toBe(true);
  expect(closeBill(game).ok).toBe(true);
}

describe('末日列车 economy', () => {
  it('starts with the 0-Tag baseline', () => {
    const game = newGame(7);
    expect(game.cash).toBe(500);
    expect(game.debt).toBe(6000);
    expect(capacity(game)).toBe(8);
    expect(game.station.customers.length).toBe(3);
    const first = game.station.customers[0]!;
    expect(first.kind).toBe('supplier');
    expect(first.goods!.map((g) => g.item)).toEqual(['water', 'ration']);
    expect(game.routeOptions!.length).toBe(3);
    expect(game.routeOptions!.some((r) => r.nodes.length === 2)).toBe(true);
    expect(game.routeOptions!.some((r) => r.nodes.length === 3)).toBe(true);
  });

  it('uses the item grid footprint and stack limits', () => {
    expect(cellsFor('parts', 4)).toBe(2);
    expect(cellsFor('water', 2)).toBe(4);
    expect(cellsFor('scrap', 5)).toBe(4);
    expect(cellsFor('repair', 1)).toBe(4);
    expect(retailPrice('parts', undefined)).toBe(24);
    expect(retailPrice('parts', { kind: 'up', item: 'parts' })).toBe(30);
    expect(ITEMS.ration.value).toBe(32);
  });

  it('runs depart → arrive → day end with fuel, wages and living costs', () => {
    const game = newGame(11);
    chooseRoute(game, 2);
    const route = game.route!;
    expect(route.nodes.length).toBe(3);
    let fuel = 0;
    while (!routeDone(game)) {
      fuel += 6 + 0.5 * 1.5;
      depart(game);
      arrive(game);
    }
    endDay(game);
    expect(game.bill!.fuel).toBe(Math.ceil(fuel));
    expect(game.bill!.wages).toBe(9);
    expect(game.bill!.living).toBe(4);
    expect(game.cash).toBe(500 - Math.ceil(fuel) - 13);
  });

  it('collects the full fare on boarding and drops riders at their destination', () => {
    const game = newGame(3);
    chooseRoute(game, 1);
    const p = game.station.passengers[0]!;
    expect(p.dest).toBe(1); // S0 前 2 名保证去下一站
    expect(boardPassenger(game, p.id)).toEqual({ ok: true, amount: 24 });
    expect(game.cash).toBe(524);
    depart(game);
    arrive(game);
    expect(game.riders.find((r) => r.id === p.id)).toBeUndefined();
    expect(game.today.delivered).toBe(1);
  });

  it('enforces seats, cash and cargo space', () => {
    const game = newGame(5);
    chooseRoute(game, 1);
    const bound = game.station.passengers.filter((p) => p.dest !== undefined);
    for (const p of bound) boardPassenger(game, p.id);
    expect(game.riders.length).toBe(Math.min(2, bound.length));
    if (bound.length > 2) expect(boardPassenger(game, bound[2]!.id)).toEqual({ ok: false, code: 'seats' });
    expect(buyCar(game, 'passenger').ok).toBe(false); // 800 > cash
    game.cash = 2000;
    expect(buyCar(game, 'storage')).toEqual({ ok: true, amount: 600 });
    expect(capacity(game)).toBe(24);
    const supplier = game.station.customers[0]!;
    expect(buySingle(game, supplier.id, [2, 1]).ok).toBe(true);
    expect(game.inventory.water).toBe(2);
    expect(buySingle(game, supplier.id, [1, 0])).toEqual({ ok: false, code: 'done' });
    game.cash = -10;
    const other = game.station.customers.find((c) => c.kind === 'supplier' && !c.outcome);
    if (other) expect(buySingle(game, other.id, other.goods!.map((g) => g.qty)).ok).toBe(false);
  });

  it('settles on day 5: pays the cumulative 600 or fails', () => {
    const game = newGame(9);
    for (let d = 1; d <= 5; d += 1) {
      chooseRoute(game, 0);
      while (!routeDone(game)) {
        depart(game);
        arrive(game);
      }
      endDay(game);
      closeBill(game);
    }
    expect(game.phase).toBe('settlement');
    expect(game.settlement!.due).toBe(600);
    game.cash = 700;
    expect(paySettlement(game)).toEqual({ ok: true, amount: 600 });
    expect(game.day).toBe(6);
    expect(game.repaid).toBe(600);
    expect(game.debt).toBe(5400);

    const poor = newGame(9);
    for (let d = 1; d <= 5; d += 1) {
      chooseRoute(poor, 0);
      while (!routeDone(poor)) {
        depart(poor);
        arrive(poor);
      }
      endDay(poor);
      closeBill(poor);
    }
    expect(paySettlement(poor)).toEqual({ ok: false, code: 'cash' });
    forfeit(poor);
    expect(poor.phase).toBe('lost');
  });

  it('early repayment counts toward the cumulative requirement and can clear the debt', () => {
    const game = newGame(1);
    game.cash = 7000;
    expect(repay(game, 1000).ok).toBe(true);
    expect(game.repaid).toBe(1000);
    expect(repay(game, 99999).ok).toBe(true);
    expect(game.debt).toBe(0);
    expect(game.phase).toBe('won');
  });

  it('a greedy bot can reach and usually pass the first settlement', () => {
    let passed = 0;
    const runs = 40;
    const cash: number[] = [];
    for (let seed = 1; seed <= runs; seed += 1) {
      const game = newGame(seed * 7919);
      for (let d = 1; d <= 5; d += 1) playDay(game);
      expect(game.phase).toBe('settlement');
      // sell leftovers at the final station before paying
      for (const c of game.station.customers) if (c.kind === 'buyer' && !c.outcome) sellToBuyer(game, c.id);
      cash.push(game.cash);
      if (paySettlement(game).ok) passed += 1;
    }
    // eslint-disable-next-line no-console
    console.log('day-5 cash before paying', cash.sort((a, b) => a - b).join(','), 'passed', passed, '/', runs);
    expect(passed).toBeGreaterThan(runs * 0.5);
    expect(RULES.dueCumulative[0]).toBe(600);
  });
});
