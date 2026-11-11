// v0.2 真实输入验收：驱动打包后的游戏（forgeax project preview），全部用真实鼠标 / 键盘。
// 用法：node qa/v02.cjs http://localhost:4195/?qa=1
const { launch, state, waitFor, click, count, shot, wait } = require('./lib.cjs');
const path = require('path');
const fs = require('fs');

const url = process.argv[2] || 'http://localhost:4195/?qa=1';
const out = path.join(__dirname, '..', 'artifacts', 'v02');
fs.mkdirSync(out, { recursive: true });
const results = [];
const check = (name, ok, detail = '') => {
  results.push({ name, ok: !!ok, detail });
  console.log(`${ok ? 'PASS' : 'FAIL'} ${name} ${detail}`);
};

async function frames(page, n = 6) {
  for (let i = 0; i < n; i += 1) await page.evaluate(() => new Promise((r) => requestAnimationFrame(() => r())));
}

function bfs(ex, tx, ty) {
  const W = ex.w;
  const wall = (x, y) => x < 0 || y < 0 || x >= W || y >= ex.h || ex.walls[y * W + x] === '1';
  const start = Math.floor(ex.y) * W + Math.floor(ex.x);
  const goal = Math.floor(ty) * W + Math.floor(tx);
  const prev = new Map([[start, start]]);
  const q = [start];
  while (q.length) {
    const i = q.shift();
    if (i === goal) break;
    const x = i % W, y = Math.floor(i / W);
    for (const [nx, ny] of [[x + 1, y], [x - 1, y], [x, y + 1], [x, y - 1]]) {
      // 保持离墙半格：四邻都不是墙才走
      if (wall(nx, ny) || prev.has(ny * W + nx)) continue;
      prev.set(ny * W + nx, i);
      q.push(ny * W + nx);
    }
  }
  if (!prev.has(goal)) return [[tx, ty]];
  const path = [];
  for (let i = goal; i !== start; i = prev.get(i)) path.push([(i % W) + 0.5, Math.floor(i / W) + 0.5]);
  return path.reverse().filter((_, k, a) => k % 2 === 1 || k === a.length - 1);
}

/** 按 BFS 路径用 WASD 走到目标（按渲染帧推进，不依赖墙钟） */
async function walkTo(page, tx, ty, r = 1.0, maxSteps = 1200) {
  let s0 = await state(page);
  if (!s0?.explore) return false;
  const path = bfs(s0.explore, tx, ty);
  for (let i = 0; i < maxSteps; i += 1) {
    const s = await state(page);
    const ex = s?.explore;
    if (!ex) return false;
    while (path.length > 1 && Math.hypot(path[0][0] - ex.x, path[0][1] - ex.y) < 0.6) path.shift();
    const [wx, wy] = path.length > 1 ? path[0] : [tx, ty];
    if (Math.hypot(tx - ex.x, ty - ex.y) < r) {
      for (const k of ['w', 'a', 's', 'd']) await page.keyboard.up(k);
      return true;
    }
    const dx = wx - ex.x;
    const dy = wy - ex.y;
    const want = { d: dx > 0.3, a: dx < -0.3, s: dy > 0.3, w: dy < -0.3 };
    for (const [k, on] of Object.entries(want)) await (on ? page.keyboard.down(k) : page.keyboard.up(k));
    await wait(120);
  }
  for (const k of ['w', 'a', 's', 'd']) await page.keyboard.up(k);
  return false;
}

(async () => {
  const { browser, page, errors } = await launch({ lang: 'zh-CN' });
  try {
    await page.goto(url, { waitUntil: 'domcontentloaded' });
    await page.waitForSelector('html[data-forgeax-input="open"]', { timeout: 180000 });
    await page.evaluate(() => localStorage.clear());
    await page.reload({ waitUntil: 'domcontentloaded' });
    await page.waitForSelector('html[data-forgeax-input="open"]', { timeout: 180000 });
    await waitFor(page, (s) => s.engine?.ready, 120000);
    await frames(page, 8);
    await shot(page, out, '01-cover-zh');
    check('cover shows experiment toggle', (await count(page, '[data-act="toggle-exp"]')) === 1);

    // 新游戏（实验玩法默认开）
    await click(page, '[data-act="new-game"]');
    let s = await waitFor(page, (x) => x.game && x.game.phase === 'station');
    check('new game opening state', s.game.cash === 500 && s.game.debt === 6000 && s.game.holds[0].w === 4 && s.game.holds[0].h === 2 && s.game.exp === true);
    check('route modal offers 3 options', s.game.routeOptions.length === 3);
    await frames(page, 6);
    await shot(page, out, '02-route-choice');
    await click(page, '[data-act="defer-routes"]');
    await frames(page, 4);
    await shot(page, out, '03-loco-panel');

    // 火车头接待：验货 + 议价 + 单拿
    s = await state(page);
    const sup = s.game.station.customers.find((c) => c.kind === 'supplier' && c.car === 0);
    check('first supplier carries 2 water + 1 ration', sup.goods.some((g) => g.item === 'water' && g.qty === 2) && sup.goods.some((g) => g.item === 'ration'));
    if ((await count(page, `[data-act="inspect"][data-id="${sup.id}"]`)) > 0) {
      await click(page, `[data-act="inspect"][data-id="${sup.id}"]`);
      s = await state(page);
      check('inspection uses one magnifier', s.game.station.inspections === 1);
    }
    await click(page, `[data-act="haggle"][data-id="${sup.id}"][data-pct="10"]`);
    s = await state(page);
    const afterHaggle = s.game.station.customers.find((c) => c.id === sup.id);
    check('haggle resolved', !!afterHaggle.haggle, afterHaggle.haggle?.result);
    if (!afterHaggle.outcome) {
      await click(page, `[data-act="buy-single"][data-id="${sup.id}"]`);
      s = await state(page);
      const water = s.game.holds.flatMap((h) => h.stacks).filter((x) => x.item === 'water').length;
      check('single purchase lands in the grid', water === 2, `cash ${s.game.cash}`);
    }
    await frames(page, 4);
    await shot(page, out, '04-trade');

    // 货舱：打开、拖动一件物品、旋转
    await click(page, '[data-act="tab"][data-tab="cargo"]');
    await frames(page, 4);
    s = await state(page);
    const stk = s.game.holds[0].stacks[0];
    if (stk) {
      const from = await page.evaluate((id) => {
        const host = [...document.querySelectorAll('*')].find((e) => e.shadowRoot?.querySelector('[data-ui-part="hud"]'));
        const el = host.shadowRoot.querySelector(`[data-stack="${id}"]`);
        const grid = host.shadowRoot.querySelector('[data-hold-grid="0"]');
        const r = el.getBoundingClientRect();
        const g = grid.getBoundingClientRect();
        return { x: r.left + 8, y: r.top + 8, gx: g.left, gy: g.top, gw: g.width, gh: g.height };
      }, stk.id);
      // 拖到第 2 行第 3 列（2×1 物品放得下的空位）
      const cell = from.gw / 4;
      await page.mouse.move(from.x, from.y);
      await page.mouse.down();
      await page.mouse.move(from.x + 20, from.y + 10, { steps: 4 });
      await page.mouse.move(from.gx + 2 * cell + 8, from.gy + cell + 8, { steps: 8 });
      await page.mouse.up();
      await frames(page, 3);
      s = await state(page);
      const moved = s.game.holds[0].stacks.find((x) => x.id === stk.id);
      check('drag & drop moves a stack', moved && (moved.x !== stk.x || moved.y !== stk.y), JSON.stringify([stk.x, stk.y, moved?.x, moved?.y]));
    }
    await frames(page, 4);
    await shot(page, out, '05-cargo-grid');
    await click(page, '[data-act="tab"][data-tab="car"]');

    // 选线：优先含探索节点的路线
    await click(page, '[data-act="open-routes"]');
    s = await state(page);
    let idx = s.game.routeOptions.findIndex((r) => r[0].kind === 'resource' || r[0].kind === 'hunt');
    if (idx < 0) idx = s.game.routeOptions.findIndex((r) => r.some((n) => n.kind === 'resource' || n.kind === 'hunt'));
    if (idx < 0) idx = 0;
    await click(page, `[data-act="choose-route"][data-index="${idx}"]`);
    s = await state(page);
    check('route locked', !!s.game.route);

    // 跑到第一个探索节点（或跑完）
    let explored = false;
    for (let leg = 0; leg < 3; leg += 1) {
      s = await state(page);
      if (!s.game.route || s.game.route.arrived >= s.game.route.nodes.length) break;
      await click(page, '[data-act="depart"]');
      await waitFor(page, (x) => !x.travelling && x.game.phase === 'station', 60000);
      await frames(page, 6);
      s = await state(page);
      if (s.game.station.site && !explored) {
        await shot(page, out, '06-site-prep');
        await click(page, '[data-act="sortie"]');
        s = await waitFor(page, (x) => x.explore && x.explore.boxes, 30000);
        await frames(page, 10);
        await shot(page, out, '07-explore-start');
        check('explore map has 4-8 containers', s.explore.boxes.length >= 4 && s.explore.boxes.length <= 8, `${s.explore.boxes.length}`);
        check('explore has enemies', s.explore.enemies >= 8, `${s.explore.enemies}`);
        // 去最近的箱子搜
        const ex = s.explore;
        const order = [...ex.boxes].sort((a, b) => Math.hypot(a[1] - ex.x, a[2] - ex.y) - Math.hypot(b[1] - ex.x, b[2] - ex.y));
        let reached = false;
        for (const box of order.slice(0, 3)) {
          reached = await walkTo(page, box[1], box[2], 1.0);
          await frames(page, 3);
          s = await state(page);
          if (reached && s.explore?.focus === 'box') break;
          reached = false;
        }
        check('walked to a container with WASD', reached, `focus=${s.explore?.focus}`);
        await page.keyboard.press('e');
        for (let i = 0; i < 120; i += 1) {
          s = await state(page);
          if (!s.explore || s.explore.open || s.game.phase !== 'explore') break;
          if (!s.explore.task) await page.keyboard.press('e');
          await frames(page, 3);
        }
        await shot(page, out, '08-explore-search');
        s = await state(page);
        if (s.explore) {
          check('container searched with E (progress bar)', s.explore.open === 'box' || s.explore.boxes.some((b) => b[4]));
          await page.keyboard.press('f');
          await frames(page, 3);
          s = await state(page);
          // 开火一次（真实鼠标）
          await page.mouse.move(800, 300);
          await page.mouse.down();
          await page.mouse.up();
          await frames(page, 3);
          // 走回撤离点并按住 E
          await walkTo(page, s.explore.extract[0], s.explore.extract[1], 1.2, 400);
          await page.keyboard.down('e');
          for (let i = 0; i < 200; i += 1) {
            s = await state(page);
            if (s.game.phase !== 'explore') break;
            await frames(page, 3);
          }
          await page.keyboard.up('e');
        }
        s = await waitFor(page, (x) => x.game.phase === 'station' && x.summary, 60000);
        await frames(page, 4);
        await shot(page, out, '09-explore-summary');
        check('sortie ended (extract or death)', ['extract', 'death'].includes(s.summary.outcome), s.summary.outcome);
        await click(page, '[data-act="close-summary"]');
        s = await state(page);
        if (s.game.pending.length) {
          await click(page, '[data-act="tab"][data-tab="cargo"]');
          while ((await count(page, '[data-act="drop-pending"]')) > 0) await click(page, '[data-act="drop-pending"]');
          await click(page, '[data-act="tab"][data-tab="car"]');
        }
        explored = true;
      }
    }
    check('route reached an exploration node', explored);

    // 收车结算
    s = await state(page);
    if (s.game.station.kind === 'rich' || s.game.station.kind === 'slum') {
      await click(page, '[data-act="end-day"]');
      s = await waitFor(page, (x) => x.game.phase === 'dayEnd');
      await frames(page, 4);
      await shot(page, out, '10-day-end-bill');
      check('day-end bill has fuel, wages, living', s.game.bill.fuel > 0 && s.game.bill.wages > 0 && s.game.bill.living >= 4, JSON.stringify({ f: s.game.bill.fuel, w: s.game.bill.wages, l: s.game.bill.living, u: s.game.bill.upkeep }));
      await click(page, '[data-act="close-bill"]');
      s = await waitFor(page, (x) => x.game.day === 2);
      check('next day starts', s.game.day === 2);
    }

    // 整备页 + 英文
    if ((await count(page, '[data-act="defer-routes"]')) > 0) await click(page, '[data-act="defer-routes"]');
    await click(page, '[data-act="tab"][data-tab="depot"]');
    await frames(page, 3);
    await shot(page, out, '11-depot');
    await click(page, '[data-act="lang"][data-lang="en"]');
    await frames(page, 6);
    await shot(page, out, '12-english');
    s = await state(page);
    check('english switch', s.lang === 'en');
    check('no page errors', errors.length === 0, errors.slice(0, 3).join(' | '));
  } catch (error) {
    check('script completed', false, String(error).slice(0, 400));
    await shot(page, out, 'zz-failure').catch(() => {});
  } finally {
    fs.writeFileSync(path.join(out, 'results.json'), JSON.stringify(results, null, 2));
    const failed = results.filter((r) => !r.ok).length;
    console.log(`\n${results.length - failed}/${results.length} passed`);
    await browser.close();
  }
})();
