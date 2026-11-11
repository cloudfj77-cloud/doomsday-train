/**
 * v0.2 平衡模拟：贪心机器人跑 N 局（只经营、不出击），统计各偿债期通过率。
 * 用法：pnpm exec tsx scripts/balance-sim.mts 300 [exp]
 */
import { ITEMS } from '../assets/sim/data.ts';
import {
  arrive,
  boardPassenger,
  buyBatch,
  buyCar,
  chooseRoute,
  closeBill,
  depart,
  endDay,
  hireDriver,
  newGame,
  paySettlement,
  routeActive,
  sellToBuyer,
  sellToRider,
  skipSite,
  type Game,
} from '../assets/sim/economy.ts';

const runs = Number(process.argv[2] ?? 200);
const exp = process.argv[3] === 'exp';
const passed = [0, 0, 0, 0, 0, 0];
let finalCash = 0;

function playStation(g: Game): void {
  for (const c of g.station.customers) if (!c.outcome && c.kind === 'buyer') sellToBuyer(g, c.id);
  for (const r of g.riders) if (r.want && !r.want.sold) sellToRider(g, r.id);
  for (const p of g.station.passengers) if (!p.status && p.dest !== undefined && (p.deadline! > g.day || (p.seg ?? 9) <= 1)) boardPassenger(g, p.id);
  for (const c of g.station.customers) {
    if (c.outcome || c.kind !== 'supplier' || !c.goods) continue;
    const resale = c.goods.reduce((s, x) => s + x.qty * Math.floor(ITEMS[x.item].value * 1.2), 0);
    const price = c.batchPrice ?? c.goods.reduce((s, x) => s + x.qty * x.unit, 0);
    if (price < resale * 0.85 && g.cash - price > 150) buyBatch(g, c.id);
  }
  if (g.day >= 3 && g.cash > 900 && !g.cars.some((c) => c.kind === 'storage')) buyCar(g, 'storage');
  if (g.station.driver && !g.pendingDriver && g.driver === 0 && g.cash > 700) hireDriver(g);
  if (g.station.site) skipSite(g);
}

for (let seed = 1; seed <= runs; seed += 1) {
  const g = newGame(seed * 7919, exp);
  for (let guard = 0; guard < 3000 && g.phase !== 'lost' && g.day <= 30; guard += 1) {
    if (g.phase === 'station') {
      playStation(g);
      if (routeActive(g)) {
        if (!depart(g).ok) endDay(g);
      } else if (g.routeOptions?.length) {
        const pure = g.routeOptions.findIndex((r) => r.every((n) => n.kind === 'rich' || n.kind === 'slum'));
        chooseRoute(g, pure >= 0 ? pure : 0);
      } else endDay(g);
    } else if (g.phase === 'travel') arrive(g);
    else if (g.phase === 'dayEnd') closeBill(g);
    else if (g.phase === 'settlement') {
      const k = g.settlement!.day / 5 - 1;
      if (paySettlement(g).ok) passed[k]! += 1;
      else g.phase = 'lost';
    } else if (g.phase === 'won') g.phase = 'station';
  }
  finalCash += g.cash;
}
console.log(`runs ${runs} (${exp ? 'experiments on' : 'team rules'})`);
console.log('pass rate by debt check (day 5/10/15/20/25/30):', passed.map((p) => `${Math.round((p / runs) * 100)}%`).join(' / '));
console.log('avg final cash', Math.round(finalCash / runs));
