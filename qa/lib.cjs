// 驱动打包后的游戏（forgeax project preview 提供的 dist/），全部用真实点击。
const puppeteer = require('/Users/cloud/WorkBuddy/2026-10-31-16-30-00/qa/node_modules/puppeteer-core');
const path = require('path');
const wait = (ms) => new Promise((r) => setTimeout(r, ms));

async function launch({ lang = 'zh-CN', width = 1280, height = 720 } = {}) {
  const browser = await puppeteer.launch({
    executablePath: '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome',
    headless: 'new',
    args: ['--no-sandbox', '--enable-unsafe-webgpu', `--window-size=${width},${height + 80}`, `--lang=${lang}`],
    defaultViewport: { width, height },
  });
  const page = (await browser.pages())[0];
  await page.evaluateOnNewDocument((language) => {
    Object.defineProperty(navigator, 'language', { get: () => language });
    Object.defineProperty(navigator, 'languages', { get: () => [language] });
  }, lang);
  const errors = [];
  page.on('pageerror', (e) => errors.push(`pageerror: ${e.message}`));
  page.on('console', (m) => { if (m.type() === 'error') errors.push(`console.error: ${m.text()}`); });
  return { browser, page, errors };
}
const state = (page) => page.evaluate(() => window.__doomsday ?? null);
async function waitFor(page, pred, timeout = 30000) {
  const end = Date.now() + timeout;
  let last;
  while (Date.now() < end) {
    last = await state(page);
    try { if (last && pred(last)) return last; } catch {}
    await wait(150);
  }
  throw new Error('waitFor timeout; last=' + JSON.stringify(last)?.slice(0, 400));
}
/** Click a visible element inside the HUD shadow root (real mouse click at its centre). */
async function click(page, selector, index = 0) {
  const point = await page.evaluate((selector, index) => {
    const host = [...document.querySelectorAll('*')].find((e) => e.shadowRoot?.querySelector('[data-ui-part="hud"]'));
    const els = [...(host?.shadowRoot?.querySelectorAll(selector) ?? [])].filter((n) => n.getClientRects().length > 0 && !n.disabled);
    const el = els[index];
    if (!el) return null;
    el.scrollIntoView({ block: 'center' });
    const r = el.getBoundingClientRect();
    const x = r.left + r.width / 2, y = r.top + r.height / 2;
    const top = host.shadowRoot.elementFromPoint(x, y);
    if (!top || !(el === top || el.contains(top))) return { covered: true };
    return { x, y };
  }, selector, index);
  if (!point) throw new Error('no clickable element: ' + selector + ' #' + index);
  if (point.covered) throw new Error('element is covered by another layer: ' + selector);
  await page.mouse.click(point.x, point.y);
  await wait(120);
}
async function count(page, selector) {
  return page.evaluate((selector) => {
    const host = [...document.querySelectorAll('*')].find((e) => e.shadowRoot?.querySelector('[data-ui-part="hud"]'));
    return [...(host?.shadowRoot?.querySelectorAll(selector) ?? [])].filter((n) => n.getClientRects().length > 0 && !n.disabled).length;
  }, selector);
}
async function shot(page, dir, name) {
  const file = path.join(dir, name + '.png');
  await page.screenshot({ path: file });
  return file;
}
module.exports = { launch, state, waitFor, click, count, shot, wait };
