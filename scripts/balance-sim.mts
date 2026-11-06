/**
 * 30 天全程平衡模拟：用一个"贪心机器人"跑 N 个随机种子，统计六期大结算的通过率。
 * 用法：pnpm exec tsx scripts/balance-sim.mts [runs]
 * 仅用于给策划案提供数据参考；机器人不代表真人最优或平均水平。
 */
import { retailPrice, previewSale, newGame, chooseRoute, depart, arrive, routeDone, endDay, closeBill, paySettlement, sellToBuyer, sellToRider, boardPassenger, buyBatch, buySingle, buyCar, capacity, cellsUsed, debtPreview, type Game } from '../assets/sim/economy.ts';

type Policy = { name: string; storageCars: number; passengerCars: number; reserve: number; lookahead: number };

function station(game: Game, policy: Policy) {
  for (const c of game.station.customers) if (c.kind === 'buyer' && !c.outcome && previewSale(game, c).total > 0) sellToBuyer(game, c.id);
  for (const r of game.riders) if (r.want && !r.want.sold) sellToRider(game, r.id);
  const storage = game.cars.filter((c) => c === 'storage').length;
  const passenger = game.cars.filter((c) => c === 'passenger').length;
  // 扩建不挤占下一期应还本金
  const preview = debtPreview(game);
  const keep = policy.reserve + (preview && preview.day - game.day <= policy.lookahead ? preview.due : 0);
  if (storage < policy.storageCars && game.cash - 600 > keep) buyCar(game, 'storage');
  else if (passenger < policy.passengerCars && game.cash - 800 > keep) buyCar(game, 'passenger');
  for (const p of game.station.passengers) if (!p.boarded && p.dest !== undefined) boardPassenger(game, p.id);
  for (const c of game.station.customers) {
    if (c.kind !== 'supplier' || c.outcome) continue;
    const resale = c.goods!.reduce((s, g) => s + g.qty * retailPrice(g.item, undefined), 0);
    if (c.batchPrice !== undefined && c.batchPrice < resale * 0.88 && game.cash - c.batchPrice > policy.reserve) {
      if (buyBatch(game, c.id).ok) continue;
    }
    // 单拿利润足够的货
    const q = c.goods!.map((g) => (g.unit < retailPrice(g.item, undefined) * 0.85 ? g.qty : 0));
    const cost = c.goods!.reduce((s, g, i) => s + g.unit * q[i]!, 0);
    if (cost > 0 && game.cash - cost > policy.reserve && cellsUsed(game.inventory) < capacity(game)) buySingle(game, c.id, q);
  }
}

function run(seed: number, policy: Policy) {
  const game = newGame(seed);
  const passedAt: number[] = [];
  while (game.phase !== 'lost' && game.day <= 30) {
    if (game.phase === 'won') break;
    chooseRoute(game, 2);
    station(game, policy);
    while (!routeDone(game)) {
      depart(game);
      arrive(game);
      station(game, policy);
    }
    endDay(game);
    closeBill(game);
    if (game.phase === 'settlement') {
      for (const c of game.station.customers) if (c.kind === 'buyer' && !c.outcome) sellToBuyer(game, c.id);
      const day = game.day;
      if (paySettlement(game).ok) passedAt.push(day);
      else return { passed: passedAt.length, lostDay: day, cash: game.cash, cars: game.cars.length };
    }
  }
  return { passed: passedAt.length, lostDay: 0, cash: game.cash, cars: game.cars.length };
}

const runs = Number(process.argv[2] ?? 300);
if (process.env.DBG) { for (let s = 1; s <= 6; s += 1) console.log(JSON.stringify(run(s * 104729, { name: 'x', storageCars: 1, passengerCars: 1, reserve: 150, lookahead: 2 }))); }
const policies: Policy[] = [
  { name: '只用车头', storageCars: 0, passengerCars: 0, reserve: 100, lookahead: 2 },
  { name: '1 载客', storageCars: 0, passengerCars: 1, reserve: 100, lookahead: 2 },
  { name: '1 存储 + 1 载客', storageCars: 1, passengerCars: 1, reserve: 100, lookahead: 2 },
  { name: '2 存储 + 2 载客', storageCars: 2, passengerCars: 2, reserve: 100, lookahead: 2 },
  { name: '3 存储 + 3 载客', storageCars: 3, passengerCars: 3, reserve: 100, lookahead: 2 },
];
for (const policy of policies) {
  const counts = [0, 0, 0, 0, 0, 0, 0];
  for (let s = 1; s <= runs; s += 1) counts[run(s * 104729, policy).passed]! += 1;
  const reach = (k: number) => counts.slice(k).reduce((a, b) => a + b, 0);
  const line = [1, 2, 3, 4, 5, 6].map((k) => `第${k * 5}天 ${((reach(k) / runs) * 100).toFixed(0)}%`).join(' | ');
  console.log(`${policy.name.padEnd(14)}  ${line}`);
}
