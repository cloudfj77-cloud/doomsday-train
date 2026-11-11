// 驱动打包后的游戏（forgeax project preview 提供的 dist/），全部用真实点击。
const puppeteer = require('puppeteer-core');
const fs = require('fs');

/** 浏览器路径：优先环境变量 CHROME_PATH，否则按系统找 Chrome / Edge。 */
function browserPath() {
  if (process.env.CHROME_PATH) return process.env.CHROME_PATH;
  const pf = process.env.ProgramFiles || 'C:\\Program Files';
  const pf86 = process.env['ProgramFiles(x86)'] || 'C:\\Program Files (x86)';
  const local = process.env.LOCALAPPDATA || '';
  const candidates = {
    darwin: ['/Applications/Google Chrome.app/Contents/MacOS/Google Chrome', '/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge'],
    win32: [`${pf}\\Google\\Chrome\\Application\\chrome.exe`, `${pf86}\\Google\\Chrome\\Application\\chrome.exe`, `${local}\\Google\\Chrome\\Application\\chrome.exe`, `${pf86}\\Microsoft\\Edge\\Application\\msedge.exe`, `${pf}\\Microsoft\\Edge\\Application\\msedge.exe`],
    linux: ['/usr/bin/google-chrome', '/usr/bin/chromium', '/usr/bin/chromium-browser', '/usr/bin/microsoft-edge'],
  }[process.platform] || [];
  const found = candidates.find((p) => p && fs.existsSync(p));
  if (!found) throw new Error('找不到 Chrome / Edge，请设置环境变量 CHROME_PATH');
  return found;
}
const path = require('path');
const wait = (ms) => new Promise((r) => setTimeout(r, ms));

async function launch({ lang = 'zh-CN', width = 1280, height = 720 } = {}) {
  const browser = await puppeteer.launch({
    executablePath: browserPath(),
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
