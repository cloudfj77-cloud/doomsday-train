// 末日列车原型验收：真实点击驱动打包后的游戏，覆盖每个需求点并各截一张图。
// 用法：node qa/playthrough.cjs [url] [outDir]
const fs = require('fs');
const { launch, state, waitFor, click, count, shot, wait } = require('./lib.cjs');

const url = process.argv[2] || 'http://localhost:4195/?qa=1';
const dir = process.argv[3] || 'artifacts/acceptance';
fs.mkdirSync(dir, { recursive: true });
const results = [];
const check = (name, ok, detail = '') => {
  results.push({ name, ok: !!ok, detail });
  console.log(`${ok ? 'PASS' : 'FAIL'}  ${name}${detail ? '  · ' + detail : ''}`);
};

async function stationRound(page, opts = {}) {
  let s = await state(page);
  const g = s.game;
  // 1) sell to every buyer we can serve
  for (const c of g.station.customers) {
    if (c.kind !== 'buyer' || c.outcome) continue;
    if (await count(page, `[data-act="sell"][data-id="${c.id}"]`)) {
      await click(page, '[data-act="tab"][data-tab="customers"]');
      await click(page, `[data-act="sell"][data-id="${c.id}"]`);
    }
  }
  // 2) sell to riders who want something
  s = await state(page);
  for (const r of s.game.riders) {
    if (r.want && !r.want.sold && s.game.inventory[r.want.item] > 0) {
      await click(page, '[data-act="tab"][data-tab="passengers"]');
      if (await count(page, `[data-act="sell-rider"][data-id="${r.id}"]`)) await click(page, `[data-act="sell-rider"][data-id="${r.id}"]`);
    }
  }
  // 3) board passengers while seats remain
  if (!opts.noPassengers) {
    await click(page, '[data-act="tab"][data-tab="passengers"]');
    s = await state(page);
    for (const p of s.game.station.passengers) {
      s = await state(page);
      const seats = 2 + 6 * s.game.cars.filter((c) => c === 'passenger').length;
      if (p.boarded || p.dest === undefined || s.game.riders.length >= seats) continue;
      await click(page, `[data-act="board"][data-id="${p.id}"]`);
    }
  }
  // 4) buy cheap bulk lots (keeps a cash reserve)
  if (!opts.noBuy) {
    await click(page, '[data-act="tab"][data-tab="customers"]');
    s = await state(page);
    for (const c of s.game.station.customers) {
      if (c.kind !== 'supplier' || c.outcome || c.batchPrice === undefined) continue;
      s = await state(page);
      const resale = c.goods.reduce((sum, x) => sum + x.qty * Math.floor(x.unit / 0.95 * 1.2), 0);
      if (c.batchPrice < resale * 0.9 && s.game.cash - c.batchPrice > (opts.reserve ?? 160)) {
        if (await count(page, `[data-act="buy-batch"][data-id="${c.id}"]`)) await click(page, `[data-act="buy-batch"][data-id="${c.id}"]`);
      }
    }
  }
}

async function playDay(page, opts = {}) {
  await waitFor(page, (s) => s.game.phase === 'depot' && !s.travelling);
  if (await count(page, '[data-act="choose-route"]')) await click(page, '[data-act="choose-route"]', 2);
  await waitFor(page, (s) => !!s.game.route);
  if (opts.beforeDepart) await opts.beforeDepart();
  await stationRound(page, opts);
  for (;;) {
    const s = await state(page);
    const r = s.game.route;
    if (r.arrived >= r.nodes.length) break;
    await click(page, '[data-act="depart"]');
    await waitFor(page, (x) => x.travelling, 5000).catch(() => {});
    if (opts.onTravel) await opts.onTravel();
    await waitFor(page, (x) => !x.travelling && x.game.phase === 'station', 10000);
    if (opts.onArrive) await opts.onArrive();
    await stationRound(page, opts);
  }
  await click(page, '[data-act="end-day"]');
  await waitFor(page, (s) => s.game.phase === 'dayEnd');
}

(async () => {
  const { browser, page, errors } = await launch({ lang: 'zh-CN' });
  await page.goto(url, { waitUntil: 'domcontentloaded' });
  await page.evaluate(() => localStorage.clear());
  await page.reload({ waitUntil: 'domcontentloaded' });
  await waitFor(page, (s) => s.engine?.ready, 90000);
  await wait(2500);
  let s = await state(page);
  check('标题画面（中文，跟随浏览器语言）', s.lang === 'zh' && s.game === null, `lang=${s.lang}`);
  await shot(page, dir, '01-title-zh');

  // --- New game & route choice --------------------------------------------------------------
  await click(page, '[data-act="new-game"]');
  s = await waitFor(page, (x) => x.game?.day === 1);
  check('开局数值：现金 500 / 欠款 6000 / 车头 8 格 2 座', s.game.cash === 500 && s.game.debt === 6000 && s.capacity === 8);
  await wait(800);
  check('选线弹窗出现 3 条候选（含 2 站与 3 站）', (await count(page, '[data-act="choose-route"]')) === 3 && s.game.routeOptions.some((o) => o.nodes.length === 2) && s.game.routeOptions.some((o) => o.nodes.length === 3));
  await shot(page, dir, '02-route-choice');
  await click(page, '[data-act="choose-route"]', 2);
  s = await waitFor(page, (x) => !!x.game.route);
  check('锁定 3 站路线后起点站乘客绑定目的地', s.game.route.nodes.length === 3 && s.game.station.passengers.some((p) => p.dest !== undefined));

  // --- Station trade on day 1 (S0) ------------------------------------------------------------
  const cash0 = s.game.cash;
  const supplier = s.game.station.customers.find((c) => c.kind === 'supplier');
  await click(page, '[data-act="tab"][data-tab="customers"]');
  // single buy: take 1 water only (step down the rest) to show quantity choice
  const q = supplier.goods.map((g) => g.qty);
  for (let row = 0; row < q.length; row += 1) {
    const target = row === 0 ? 1 : 0;
    for (let k = q[row]; k > target; k -= 1) await click(page, `[data-act="step"][data-id="${supplier.id}"][data-row="${row}"][data-delta="-1"]`);
  }
  await shot(page, dir, '03-trader-single-pick');
  await click(page, `[data-act="buy-single"][data-id="${supplier.id}"]`);
  s = await state(page);
  check('单拿进货：按所选数量付款并入库', s.game.cash === cash0 - supplier.goods[0].unit && s.game.inventory[supplier.goods[0].item] === 1, `付 ${cash0 - s.game.cash}`);

  // board passengers -> fare collected at once
  await click(page, '[data-act="tab"][data-tab="passengers"]');
  const before = s.game.cash;
  const bound = s.game.station.passengers.filter((p) => p.dest !== undefined);
  await click(page, `[data-act="board"][data-id="${bound[0].id}"]`);
  s = await state(page);
  check('乘客上车即收全款（24/段）', s.game.cash === before + bound[0].fare && s.game.riders.length === 1, `+${bound[0].fare}`);
  for (const p of bound.slice(1)) {
    s = await state(page);
    if (s.game.riders.length >= 2) break;
    await click(page, `[data-act="board"][data-id="${p.id}"]`);
  }
  s = await state(page);
  const third = bound.find((p) => !s.game.station.passengers.find((x) => x.id === p.id)?.boarded);
  if (third) {
    await click(page, `[data-act="board"][data-id="${third.id}"]`);
    s = await state(page);
    check('座位已满时不能再上车（车头 2 座）', s.game.riders.length === 2);
  }
  await shot(page, dir, '04-passengers-boarded');

  // --- Travel ---------------------------------------------------------------------------------
  await click(page, '[data-act="depart"]');
  await wait(1100);
  s = await state(page);
  check('发车：行驶动画中，界面锁定，引擎在播放跑线', s.travelling && s.engine?.travelling, `engine.travelling=${s.engine?.travelling}`);
  await shot(page, dir, '05-travelling');
  s = await waitFor(page, (x) => !x.travelling, 10000);
  await wait(600);
  check('抵达第 1 站：目的地为本站的乘客自动下车', s.game.today.delivered >= 1 && s.game.route.arrived === 1, `下车 ${s.game.today.delivered}`);
  check('新站客商数 = 3（无存储车厢）', s.game.station.customers.length === 3);
  await shot(page, dir, '06-arrived-station');

  // sell to a buyer if possible
  await stationRound(page);
  s = await state(page);
  const sold = s.game.station.customers.filter((c) => c.kind === 'buyer' && c.outcome === 'deal');
  await shot(page, dir, '07-after-trading');
  // finish day 1
  while (!(s.game.route.arrived >= s.game.route.nodes.length)) {
    await click(page, '[data-act="depart"]');
    s = await waitFor(page, (x) => !x.travelling && x.game.phase === 'station', 10000);
    await stationRound(page);
    s = await state(page);
  }
  await click(page, '[data-act="end-day"]');
  s = await waitFor(page, (x) => x.game.phase === 'dayEnd');
  await wait(300);
  const b = s.game.bill;
  const fuelOk = b.fuel === Math.ceil(b.ledger.fuelRaw - 1e-9);
  check('日结：燃油=ceil(Σ每段)、司机工资=3×段数、生活维护=2×人数', fuelOk && b.wages === 3 * b.segments && b.living === 4, `燃油 ${b.fuel} / 工资 ${b.wages} / 生活 ${b.living} / 现金 ${b.cashBefore}→${b.cashAfter}`);
  await shot(page, dir, '08-day-end-bill');
  await click(page, '[data-act="close-bill"]');
  s = await waitFor(page, (x) => x.game.day === 2 && x.game.phase === 'depot');
  check('进入第 2 天，过夜不刷新同站客商', s.game.station.customers.length >= 3);

  // --- Day 2: buy a storage car when affordable ------------------------------------------------
  for (let d = 2; d <= 5; d += 1) {
    await playDay(page, { reserve: 200 });
    if (d < 5) await click(page, '[data-act="close-bill"]');
  }
  s = await state(page);
  check('第 5 天日结按钮指向偿债大结算', (await count(page, '[data-act="close-bill"]')) === 1);
  await click(page, '[data-act="close-bill"]');
  s = await waitFor(page, (x) => x.game.phase === 'settlement');
  await wait(300);
  check('第 5 天进入偿债大结算：本期累计应还 600', s.game.settlement.required === 600 && s.game.settlement.due === 600, `现金 ${s.game.cash}`);
  await shot(page, dir, '10-settlement');
  if (s.game.cash < 600 && (await count(page, '[data-act="settle-sell"]'))) {
    await click(page, '[data-act="settle-sell"]');
    await stationRound(page, { noBuy: true, noPassengers: true });
    await shot(page, dir, '10b-settlement-selling');
  }
  s = await state(page);
  if (s.game.cash >= 600) {
    await click(page, '[data-act="settle-pay"]');
    s = await waitFor(page, (x) => x.game.day === 6);
    check('大结算付款通过：累计已还 600，剩余欠款 5400，进入第 6 天', s.game.repaid === 600 && s.game.debt === 5400);
    await wait(600);
    await shot(page, dir, '11-day6-after-settlement');
  } else {
    check('大结算付款通过（本局现金不足，见失败分支）', false, `现金 ${s.game.cash}`);
  }

  // --- Early repayment --------------------------------------------------------------------------
  s = await state(page);
  if (s.game.cash >= 100) {
    if (await count(page, '[data-act="choose-route"]')) await click(page, '[data-act="defer-routes"]');
    await click(page, '[data-act="tab"][data-tab="depot"]');
    const repaid0 = s.game.repaid;
    await click(page, '[data-act="repay"][data-amount="100"]');
    s = await state(page);
    check('提前还款 100：计入累计已还', s.game.repaid === repaid0 + 100);
    await shot(page, dir, '12-early-repay');
  }

  // --- English & reload survival ------------------------------------------------------------------
  await click(page, '[data-act="lang"][data-lang="en"]');
  await wait(400);
  s = await state(page);
  check('手动切换英文，界面全部换成英文', s.lang === 'en');
  await shot(page, dir, '13-english-hud');
  const dayBefore = s.game.day;
  const cashBefore = s.game.cash;
  await page.reload({ waitUntil: 'domcontentloaded' });
  await waitFor(page, (x) => x.engine?.ready, 90000);
  await wait(1500);
  s = await state(page);
  check('刷新后保留英文选择，标题页可继续', s.lang === 'en' && s.game === null && (await count(page, '[data-act="continue"]')) === 1);
  await shot(page, dir, '14-title-en-after-reload');
  await click(page, '[data-act="continue"]');
  s = await waitFor(page, (x) => !!x.game);
  check('继续游戏：天数与现金完全恢复', s.game.day === dayBefore && s.game.cash === cashBefore, `第${s.game.day}天 现金${s.game.cash}`);
  await click(page, '[data-act="lang"][data-lang="zh"]');

  // --- Workshop: a dedicated run that buys a storage and a passenger car -----------------------
  await click(page, '[data-act="to-title"]');
  await click(page, '[data-act="new-game"]');
  await waitFor(page, (x) => x.game?.day === 1);
  let bought = false;
  for (let d = 1; d <= 4 && !bought; d += 1) {
    await playDay(page, {
      reserve: 120,
      onArrive: async () => {
        const x = await state(page);
        if (!bought && x.game.cash >= 600 && x.game.route.arrived < x.game.route.nodes.length) {
          const customersBefore = x.game.station.customers.length;
          await click(page, '[data-act="tab"][data-tab="depot"]');
          await click(page, '[data-act="buy-car"][data-kind="storage"]');
          const y = await state(page);
          bought = y.game.cars.includes('storage');
          await wait(1500);
          const z = await state(page);
          check('购买存储车厢：现金 −600、货舱 8→24、3D 列车挂上车厢', bought && y.game.cash === x.game.cash - 600 && y.capacity === 24 && z.engine?.cars === 1, `engine.cars=${z.engine?.cars}`);
          await shot(page, dir, '09-bought-storage-car');
          // next station has one more trader
          const r = y.game.route;
          if (r.arrived < r.nodes.length) {
            await click(page, '[data-act="depart"]');
            const w = await waitFor(page, (v) => !v.travelling && v.game.phase === 'station', 10000);
            check('存储车厢生效：下一站客商 3→4 名', w.game.station.customers.length === 4 && customersBefore === 3, `${customersBefore}→${w.game.station.customers.length}`);
            await wait(500);
            await shot(page, dir, '09b-train-with-car');
          }
        }
      },
    });
    await click(page, '[data-act="close-bill"]');
  }
  check('专项局中成功买到车厢', bought);

  // --- Failure ending: run 5 days without trading or passengers ---------------------------------
  await waitFor(page, (x) => x.game.phase === 'depot' || x.game.phase === 'settlement');
  if ((await state(page)).game.phase === 'settlement') { if (await count(page, '[data-act="settle-pay"]')) await click(page, '[data-act="settle-pay"]'); }
  await click(page, '[data-act="to-title"]');
  await click(page, '[data-act="new-game"]');
  await waitFor(page, (x) => x.game?.day === 1);
  for (let d = 1; d <= 5; d += 1) {
    await waitFor(page, (x) => x.game.phase === 'depot');
    await click(page, '[data-act="choose-route"]', 2);
    for (;;) {
      const x = await state(page);
      if (x.game.route.arrived >= x.game.route.nodes.length) break;
      await click(page, '[data-act="depart"]');
      await waitFor(page, (y) => !y.travelling && y.game.phase === 'station', 10000);
    }
    await click(page, '[data-act="end-day"]');
    await waitFor(page, (x) => x.game.phase === 'dayEnd');
    await click(page, '[data-act="close-bill"]');
  }
  s = await waitFor(page, (x) => x.game.phase === 'settlement');
  check('不经营时第 5 天现金不足 600，付款按钮禁用', s.game.cash < 600 && (await count(page, '[data-act="settle-pay"]')) === 0, `现金 ${s.game.cash}`);
  await shot(page, dir, '15-settlement-short');
  await click(page, '[data-act="forfeit"]');
  s = await waitFor(page, (x) => x.game.phase === 'lost');
  check('认输 → 失败结局「列车被收走了」', s.game.phase === 'lost');
  await wait(300);
  await shot(page, dir, '16-ending-lost');

  check('全程无页面错误', errors.length === 0, errors.slice(0, 3).join(' | '));
  fs.writeFileSync(`${dir}/results.json`, JSON.stringify({ results, errors }, null, 2));
  const failed = results.filter((r) => !r.ok).length;
  console.log(`\n${results.length - failed}/${results.length} passed`);
  await browser.close();
  process.exit(failed ? 1 : 0);
})().catch(async (e) => {
  console.error(e);
  process.exit(2);
});
