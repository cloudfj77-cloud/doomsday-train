import { CAREERS, FACTION_META, GRADE_LABELS, ITEM_DEFS, MACHINES, RAID_ZONES, RARITY_COLORS, RARITY_NAMES, UPGRADES, WANTED_TRAITS } from '../core/catalog.ts';
import { clamp, computeBargainOdds, computeEffectiveWeaponStats, computeItemPrice, createDuckItem, inspectItemWithTool, resolveBargainRoll } from '../core/formulas.ts';
import { sfx } from '../core/audio.ts';
import { GameStore } from '../core/store.ts';
import type { DuckItem, FactionId, InspectTool, ShopClient } from '../core/types.ts';
import { RaidGame3D } from './raid3d.ts';

const esc = (s: unknown) => String(s).replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]!));
const gradeName = (g: number) => GRADE_LABELS[g as keyof typeof GRADE_LABELS] ?? String(g);
const ART = '/art/';
function portraitPath(c: ShopClient): string {
  if (c.storyId?.startsWith('georgio')) return `${ART}portraits/georgio.png`;
  if (c.storyId === 'officer') return `${ART}portraits/officer.png`;
  if (c.intent === 'LANDLORD') return `${ART}portraits/landlord.png`;
  if (c.intent === 'INSPECTION') return `${ART}portraits/security${c.portraitSeed % 4}.png`;
  return `${ART}portraits/${c.faction}${c.portraitSeed % 4}.png`;
}
function itemIcon(defId: string): string { return `${ART}items/${defId}.png`; }

export function mountGame(shadow: ShadowRoot): { dispose(): void } {
  const store = new GameStore();
  let root = shadow.querySelector<HTMLElement>('#duckov-root');
  if (!root) { root = document.createElement('div'); root.id = 'duckov-root'; shadow.appendChild(root); }
  let selectedUid: string | null = null;
  let selectedTool: InspectTool = 'visual';
  let raid: RaidGame3D | null = null;
  let disposed = false;
  let showTutorial = false;
  let tutorialStep = 0;

  const priceCtx = (sell: boolean, c?: ShopClient, mult = 1, flaw = false) => ({
    isSellingToClient: sell, clientFaction: c?.faction, factionRep: c ? store.rep[c.faction] : 0,
    activeEvents: store.events, finalMultiplier: mult, flawDiscountApplied: flaw,
  });
  const itemPriceSell = (it: DuckItem, c?: ShopClient, mult = 1) => computeItemPrice(it, priceCtx(true, c, mult));
  const itemPriceBuy = (it: DuckItem, c?: ShopClient, mult = 1, flaw = false) => computeItemPrice(it, priceCtx(false, c, mult, flaw));

  function addRep(f: FactionId, d: number) { store.rep[f] = clamp(store.rep[f] + d, -200, 200); }

  function render() {
    if (disposed || !root) return;
    if (tut && !tutGrad) while (tut.step < tutSteps.length && tutSteps[tut.step]!.done()) tut.step++;
    if (store.phase === 'NIGHT_RAID') { renderRaidShell(); return; }
    root.innerHTML = `
    <div class="app ${tut && !tutGrad ? 'tut-mode' : ''}">
      ${header()}
      ${store.phase === 'CAREER_SELECT' ? careerView() : ''}
      ${store.phase === 'MORNING' ? morningView() : ''}
      ${store.phase === 'SHOP_OPEN' ? shopView() : ''}
      ${store.phase === 'AFTERHOURS' ? afterView() : ''}
      ${(store.phase === 'GAME_OVER' || store.phase === 'VICTORY') ? endingView() : ''}
      ${store.phase !== 'CAREER_SELECT' && store.phase !== 'GAME_OVER' && store.phase !== 'VICTORY' ? logView() : ''}
      ${showTutorial ? tutorialView() : ''}
      ${tutCoachView()}${tutGradView()}
    </div>`;
    bind();
    applyTutFocus();
    tutMsg = '';
  }

  // ---------- 新手教学关（可玩、逐步引导：开店 → 验货 → 卖货 → 盘查 → 机器 → 3D 夜袭） ----------
  interface TutStep { title: string; text: string; focus: () => string; allow: (act: string, d: DOMStringMap) => boolean; done: () => boolean; wrong?: string }
  let tut: { step: number; dustUid: string; lastFocus: number } | null = null;
  let tutGrad = false;
  let tutMsg = '';
  const dust = () => (tut ? store.find(tut.dustUid) : undefined);
  const tutSteps: TutStep[] = [
    { title: '欢迎来到地堡黑店', text: '我是塞尔皮科，带你跑完第一天。<br>玩法一句话：<b>白天开店低买高卖，晚上去封锁区搜货</b>，攒够钱买下这座地堡。<br>顶上一排是 💰现金、🏠房租、🔍证据。现在点亮着的 <b>「拉开卷帘门」</b> 开始接客。', focus: () => '[data-act="open-shop"]', allow: a => a === 'open-shop', done: () => store.phase !== 'MORNING' },
    { title: '第 1 位客人：来卖货的', text: '这只鸭子要卖你一个「义手伺服核心」，标签写着<b>「精品」</b>。卖货的经常<b>以次充好</b>，所以先验货！<br>义手是<b>金属零件</b>，要用 <b>📡 频谱扫描仪</b>，点它选中。<br><small>口诀：金属电子 → 扫描仪；水和药 → 试纸；外观 → 放大镜</small>', focus: () => '[data-act="tool"][data-t="scanner"]', allow: a => a === 'tool', done: () => selectedTool === 'scanner', wrong: '不对哦，义手是金属零件，选「📡 频谱扫描仪」。' },
    { title: '验货', text: '工具选好了，点 <b>「验货」</b>。', focus: () => '[data-act="inspect-offer"]', allow: a => a === 'inspect-offer' || a === 'tool', done: () => !!store.queue[0]?.offerItem?.exposed },
    { title: '抓到假货了！', text: '揭穿了！它的真实品质只是<b>「普通」</b>，标签是假的。<br>点 <b>「以瑕疵为由再砍 15% 成交」</b>，用便宜价把它收下。', focus: () => '[data-act="buy-flaw"]', allow: a => a === 'buy-flaw', done: () => store.clientIndex >= 1 },
    { title: '第 2 位客人：来买货的', text: '巧了，这位医生正好想买义手核心。中间列出了你库存里对得上的货。<br>点 <b>「原价」</b> 卖给他。刚才压价收、现在原价卖，<b>差价就是你赚的</b>。<br><small>想多赚可以点 +25%，但成功率会降，失败了客人可能直接走。</small>', focus: () => '[data-act="sell-to"][data-m="0"]', allow: a => a === 'sell-to', done: () => store.clientIndex >= 2 },
    { title: '🚨 盘查员来了', text: '盘查员会查你的<b>柜台和展柜</b>。柜台上那瓶<b>「蓝晶镇痛合剂」是 C3 违禁品</b>，被查到要罚款、涨罪值！<br>先在右边库存里<b>点一下这瓶蓝晶合剂</b>。', focus: () => `[data-act="select-item"][data-uid="${tut?.dustUid}"]`, allow: () => false, done: () => selectedUid === tut?.dustUid || dust()?.location === 'secret_stash', wrong: '点右边库存里亮着的那瓶「蓝晶镇痛合剂」。' },
    { title: '藏进海报暗格', text: '点 <b>「→暗格藏匿」</b>。海报后面的暗格<b>绝对不会被查</b>，但只有 4 格，要省着用。', focus: () => `[data-act="move"][data-loc="secret_stash"][data-uid="${tut?.dustUid}"]`, allow: (a, d) => a === 'move' && d.uid === tut?.dustUid && d.loc === 'secret_stash', done: () => dust()?.location === 'secret_stash', wrong: '点「→暗格藏匿」，把蓝晶合剂藏起来。' },
    { title: '接受盘查', text: '柜台干净了。点 <b>「接受盘查」</b>，干净的柜台还能涨治安声望。', focus: () => '[data-act="inspection-submit"]', allow: a => a === 'inspection-submit', done: () => store.phase !== 'SHOP_OPEN' },
    { title: '打烊：装机器', text: '今天的客人接待完了。后屋机器能把<b>便宜原料变成值钱货</b>，过一夜就出货。<br>点亮着的 <b>「装入 …脏水」</b>，把脏水放进净水塔。', focus: () => '[data-act="machine-load"]', allow: a => a === 'machine-load', done: () => store.machineInputs.size > 0 },
    { title: '夜袭：搜打撤', text: '晚上可以睡觉，也可以去封锁区<b>「搜打撤」</b>：<b>搜</b>物资箱、<b>打</b>敌鸭、活着<b>撤</b>离，带回来的货明天能卖。<br>点 <b>「零号街区」</b> 出发。这是原版鸭科夫 3D 地图，教学关里<b>你不会死</b>，放心打。', focus: () => '[data-act="start-raid"][data-id="ground_zero"]', allow: (a, d) => a === 'start-raid' && d.id === 'ground_zero', done: () => store.phase === 'NIGHT_RAID' || tutGrad },
  ];
  const TUT_FREE = ['select-item', 'tutorial', 'close-tutorial', 'tutorial-next', 'tutorial-prev', 'tut-skip', 'tut-continue', 'tut-restart'];

  function startTutorial() {
    store.startCareer('apprentice');
    const dustItem = createDuckItem('chem_dreamdust', { actualGrade: 1, claimedGrade: 1, stolenHeat: 30, location: 'shop_stock' });
    store.items.push(dustItem);
    const fake = createDuckItem('val_prosthetic', { actualGrade: 0, claimedGrade: 2 });
    const base = { isWanted: false, wantedBounty: 0, isCriminal: false, isEventDriven: false, bargainLocked: false, blackmailed: false, priceMultiplier: 1, flawDiscountApplied: false };
    store.queue = [
      { ...base, id: 'tut_seller', name: '拾荒鸭·阿灰', title: '下层区拾荒者', faction: 'lower', intent: 'SELL', portraitSeed: 1, offerItem: fake, budget: 9999, dialogue: '老板，正宗军用义手核心，「精品」！便宜点给你。' },
      { ...base, id: 'tut_buyer', name: '义体医生·白喙', title: '上层区诊所医生', faction: 'upper', intent: 'BUY', portraitSeed: 2, requestedDefId: 'val_prosthetic', requestedCategory: 'valuables', budget: 9999, dialogue: '我在找义手伺服核心，品质普通也行，价格公道就收。' },
      { ...base, id: 'tut_cop', name: '盘查员·铁面', title: '铁喙治安盘查员', faction: 'security', intent: 'INSPECTION', portraitSeed: 3, inspectorPersona: 'strict', bribeCost: 150, budget: 0, dialogue: '例行盘查。把柜台和展柜都打开，别耍花样。' },
    ];
    store.clientIndex = 0;
    store.cash = Math.max(store.cash, itemPriceBuy(fake, store.queue[0]) + 300);
    tut = { step: 0, dustUid: dustItem.uid, lastFocus: -1 };
    showTutorial = false; sfx.play('bell');
  }

  function tutCoachView() {
    if (!tut || tutGrad) return '';
    const st = tutSteps[tut.step]; if (!st) return '';
    return `<aside class="coach ${tutMsg ? 'shake' : ''}"><img src="/art/portraits/serpico.png" alt="" onerror="this.style.display='none'"><div>
      <small>🎓 新手教学关 · 第 ${tut.step + 1} / ${tutSteps.length + 1} 步（最后一步是 3D 夜袭）</small><b>${st.title}</b><p>${st.text}</p>
      ${tutMsg ? `<p class="coach-warn">${tutMsg}</p>` : ''}<button data-act="tut-skip" class="coach-skip">跳过教学</button></div></aside>`;
  }

  function tutGradView() {
    if (!tutGrad) return '';
    return `<div class="tutorial-mask"><div class="tutorial-card grad">
      <div class="grad-head"><img src="/art/portraits/serpico.png" alt="" onerror="this.style.display='none'"><h2>🎓 出师了！</h2></div>
      <p>${esc(store.logs.find(l => l.text.includes('夜袭'))?.text ?? '你跑完了完整的一天。')}</p>
      <p>以后每天就是这个循环：</p>
      <ul class="grad-list"><li>☀️ 白天：<b>选对工具验货 → 揭穿假货压价收 → 原价或加价卖出</b></li>
      <li>🚨 盘查前：<b>违禁品藏进暗格</b></li><li>🏭 打烊：<b>原料装进机器</b>，过一夜出货</li>
      <li>🌙 晚上：<b>开箱搜货 → 打敌鸭 → 跟绿箭头撤离</b>（死了只保住最值钱的几件）</li>
      <li>🏠 每周日：<b>留够钱交房租</b>。攒到 12000 买断地堡就赢了</li></ul>
      <div class="tutorial-nav"><button class="primary" data-act="tut-continue">继续经营这家店（第 ${store.day} 天）</button><button data-act="tut-restart">重新选职业开新局</button></div></div></div>`;
  }

  function applyTutFocus() {
    if (!tut || tutGrad) return;
    const st = tutSteps[tut.step]; if (!st) return;
    const el = root!.querySelector<HTMLElement>(st.focus()); if (!el) return;
    el.classList.add('tut-focus');
    if (tut.lastFocus !== tut.step) { tut.lastFocus = tut.step; requestAnimationFrame(() => el.scrollIntoView({ block: 'center', behavior: 'smooth' })); }
  }

  function header() {
    if (store.phase === 'CAREER_SELECT') return `<header class="topbar brandonly"><div class="brand">🦆 鸭科夫黑店 <span>DUCKOV PAWNSHOP</span></div><a href="/explorer3d/index.html" target="_blank" class="explorerlink">🗺 浏览 9 张 3D 战区与 16 个模型</a></header>`;
    const ev = Math.round(store.evidence), cr = Math.round(store.crime);
    return `<header class="topbar">
      <div class="brand">🦆 鸭科夫黑店 <span>暗账与撤离</span></div>
      <div class="stats">
        <b>第 ${store.day} 天</b><span>💰 ${store.cash} cr</span><span>✦ 恩惠 ${store.favor}</span>
        <span class="${store.rentDueIn === 0 ? 'danger' : ''}">🏠 ${store.owned ? '已买断产权' : store.mortgageRemaining > 0 ? `按揭剩 ${store.mortgageRemaining}` : `房租 ${store.rent} · ${store.rentDueIn === 0 ? '今天到期!' : store.rentDueIn + ' 天后'}`}</span>
        <span class="${ev >= 70 ? 'danger' : ''}">🔍 证据 ${ev}/100</span><span class="${cr >= 200 ? 'danger' : ''}">☠ 罪值 ${cr}</span>
        <span>📦 库存 ${store.items.length}</span>
        <button data-act="tutorial" class="helpbtn">❓ 新手教程</button>
        <a href="/explorer3d/index.html" target="_blank" class="explorerlink">🗺 3D 战区全景(9张GLB)</a>
      </div>
      <div class="repbar">${(Object.keys(FACTION_META) as FactionId[]).map(f => `<span style="--fx-duckov-fc:${FACTION_META[f].color}"><i></i>${FACTION_META[f].short} ${store.rep[f] > 0 ? '+' : ''}${store.rep[f]}</span>`).join('')}</div>
    </header>`;
  }

  function careerView() {
    return `<main class="careers"><h1>白天开黑店，夜晚进封锁区</h1>
    <p class="sub">验货贴标、议价盘查、藏匿暗格、周日交租——再亲自下场搜打撤，把战利品喂进后屋机器，第二天高价卖出。</p>
    <button class="tut-entry" data-act="tut-start"><img src="/art/portraits/serpico.png" alt="" onerror="this.style.display='none'"><div><b>🎓 新手教学关（第一次玩点这里）</b>
      <span>师傅手把手带你玩完完整的一天：开店 → 验货抓假货 → 卖货赚差价 → 应付盘查 → 装机器 → 进原版鸭科夫 3D 地图搜打撤。约 10 分钟，教学关里不会死。</span></div><em>开始 ▶</em></button>
    <p class="hint">老手可以直接选一个身份开局：</p>
    <div class="career-grid">${CAREERS.map(c => `<button class="career-card" data-act="career" data-id="${c.id}">
      <h3>${esc(c.name)}</h3><p>${esc(c.subtitle)}</p>
      <div class="career-meta"><span>💰 ${c.startingCash}</span><span>🏠 租 ${c.startingRent}/周</span><span>✦ ${c.startingFavor}</span></div>
      <small>${c.starterItemSpecs.length} 件初始装备 · ${c.unlockedUpgrades.length ? '已解锁专长产线' : '白手起家'}</small></button>`).join('')}</div>
    <p class="hint">目标：攒够 12000 cr 现金买断地堡产权，或走按揭路线还清贷款。证据满 100 且罪值过高，将被判决警官清算。</p></main>`;
  }

  function morningView() {
    const hasInspector = store.queue.length === 0 && (store.day % 3 === 0);
    return `<main class="morning">
      ${objectiveBanner()}
      <section class="panel hero"><img class="shop-scene" src="/art/scene/store.png" alt="地堡黑店内景" onerror="this.style.display='none'"><h2>🌅 第 ${store.day} 天 · 晨报</h2>
      ${store.events.map(e => `<div class="event"><b>${esc(e.headline)}</b><p>${esc(e.detail)}</p></div>`).join('')}
      <div class="wanted"><img class="wanted-img" src="/art/wanted/wanted${(store.day % 9) + 1}.png" alt="" onerror="this.style.display='none'">🚨 <b>今日通缉令：</b>留意「${esc(store.wantedName)}」特征的客人，举报属实有高额赏金，误报罚声望。</div>
      ${store.upgrades.has('upg_informant') && hasInspector ? `<div class="warn">📟 内线预警：今天队列里大概率有治安盘查员！先把违禁品挪进海报暗格。</div>` : ''}
      <div class="morning-actions"><button class="primary big" data-act="open-shop">🔔 拉开卷帘门 · 开始接客</button>
      <button data-act="goto-after">跳过白天 → 打烊整备</button></div>
      <p class="hint">开门前先在下方库存里整理展柜与暗格：展柜吸客但会被盘查，暗格绝对安全（容量 ${store.stashCapacity}）。</p></section>
      ${inventoryView(true)}
    </main>${''}`;
  }

  function tutorialView() {
    const steps = [
      { t: '🎯 这游戏要干什么？', b: '你是一只在封锁区开黑店的鸭子。<b>白天卖货赚钱，夜晚亲自去封锁区抢货回来卖。</b><br>🏆 赢：攒够 <b>12000 金币</b>买断地堡（或按揭还清）。<br>💀 输：每周交不起房租，或者"证据"涨到 100 且罪太重被警官抓走。' },
      { t: '1️⃣ 第一步：选职业，点开店', b: '第一次玩就选<b>「学徒杂货鸭」</b>，钱最多、最安全。<br>进入晨报后，看一眼今天什么涨价，然后点黄色大按钮<b>「拉开卷帘门」</b>开始接客。' },
      { t: '2️⃣ 白天卖货：有人买，有人卖', b: '🛒 <b>客人来买东西</b>：中间面板会列出你能卖的货，点价格按钮成交。想多赚就点 +25%/+50%，但成功率会明码标出来，失败人家可能直接走。<br>💼 <b>客人来卖东西</b>：先别急着买！看下一页验货。' },
      { t: '3️⃣ 验货：这是赚钱核心', b: '卖货的鸭子经常<b>拿假货冒充好货</b>。中间面板有 3 个验货工具：<br>🔍 放大镜查外观造假 · 🧪 试纸查药品和水 · 📡 扫描仪查枪械金属<br><b>选对工具点「验货」</b>，揭穿假货就能砍价 15% 收下，转手按真价卖。不验货直接买，大概率当冤大头。' },
      { t: '4️⃣ 右边库存：挪货、贴假标、藏暗格', b: '点右边任意一件货，可以：<br>✨ 挪到<b>展柜</b>：吸引有钱客人，但会被警察查<br>🖼 挪进<b>海报暗格</b>：违禁品藏这里，警察绝对查不到（只有 4 格）<br>🏷 <b>贴假标</b>：把次品包装成好货高价卖，但被识货的人揭穿会掉声望、涨罪值' },
      { t: '5️⃣ 警察盘查怎么办？', b: '队列里出现 🚨 盘查员时，<b>先去右边库存把违禁品（带 C2/C3 字样的）全挪进暗格</b>，再回来点「接受盘查」。<br>遇到"可贿赂型"也可以直接塞钱让他走。柜台干净的话，盘查反而涨治安声望。' },
      { t: '6️⃣ 打烊后：机器 + 夜晚出击', b: '客人走完进入打烊：<br>🏭 把原料装进机器（比如脏水装进净水塔），睡一觉就变成值钱货<br>👑 有钱了买升级，解锁更赚钱的机器和大暗格<br>🌙 然后二选一：<b>睡觉</b>，或者选一个战区<b>夜袭</b>——下一页教打仗。' },
      { t: '7️⃣ 夜晚打仗：搜打撤', b: '键盘 <b>W A S D</b> 移动，<b>鼠标</b>瞄准，<b>按住左键</b>开枪。<br><b>空格</b>翻滚躲子弹 · <b>R</b>换子弹 · <b>Q</b>吃药回血<br>走到发光的物资箱旁按 <b>E</b> 开箱拿货；跟着屏幕上的<b>绿色箭头</b>走进绿色光柱，站 3 秒就撤离成功。<br>⏱ 时间到没撤 = 失败；被打死 = 背包里的货全丢（只有最值钱的几件能保住）。量力而行，新手先去「零号街区」。' },
      { t: '✅ 记住这一句就够了', b: '<b>白天：验货 → 低买 → 贴标/进机器 → 高卖</b><br><b>夜晚：搜箱 → 打得过就打 → 残血就撤</b><br><b>每周：留够交房租的钱</b><br><br>第一次玩，建议关掉这页，点首页的「🎓 新手教学关」实际玩一遍。' },
    ];
    const s = steps[tutorialStep] ?? steps[0]!;
    return `<div class="tutorial-mask"><div class="tutorial-card">
      <div class="tutorial-head"><span>🦆 新手教程 ${tutorialStep + 1} / ${steps.length}</span><button data-act="close-tutorial">✕ 跳过</button></div>
      <h2>${s.t}</h2><p>${s.b}</p>
      <div class="tutorial-dots">${steps.map((_, i) => `<i class="${i === tutorialStep ? 'on' : ''}"></i>`).join('')}</div>
      <div class="tutorial-nav">
        <button data-act="tutorial-prev" ${tutorialStep === 0 ? 'disabled' : ''}>← 上一页</button>
        ${tutorialStep < steps.length - 1 ? `<button class="primary" data-act="tutorial-next">下一页 →</button>` : `<button class="primary" data-act="close-tutorial">开始游戏 🎮</button>`}
      </div></div></div>`;
  }

  function objectiveBanner() {
    const c = store.currentClient;
    if (tut && !tutGrad) return '';
    if (store.phase === 'MORNING') return `<div class="objective">👉 现在该做：看一眼涨价新闻，把违禁品挪进右边「海报暗格」，然后点「拉开卷帘门」</div>`;
    if (store.phase === 'AFTERHOURS') return `<div class="objective">👉 现在该做：装一台机器 → 买得起就买升级 → 新手选「直接睡觉」，想赚钱就选「零号街区」夜袭</div>`;
    if (!c) return '';
    const map: Record<string, string> = {
      BUY: '👉 现在该做：在中间面板选一件货，点价格成交。新手就点「原价」，稳赚不翻车',
      SELL: '👉 现在该做：先在中间面板选对验货工具点「验货」，揭穿假货再压价买下；没问题就原价收',
      INSPECTION: '👉 现在该做：先去右边库存把带 C2/C3 的违禁品挪进「海报暗格」，再回来点「接受盘查」',
      WHOLESALE_BUYER: '👉 现在该做：这是周日卖货换现金交房租的机会，缺钱就点「全部卖出」',
      LANDLORD: '👉 现在该做：交房租！钱够就点支付；不够就先回去卖货，或者办按揭/买断',
      STORY: '👉 现在该做：这是特殊剧情客人，看中间面板的说明做选择，会影响后面几天的收益',
    };
    return `<div class="objective">${map[c.intent] ?? ''}</div>`;
  }

  function clientPortrait(c: ShopClient) {
    const colors = ['#2fe6d6', '#ffb547', '#4d94ff', '#b580ff', '#ff4d8d', '#9be15d'];
    const col = colors[c.portraitSeed % colors.length];
    const trait = c.visualTrait === 'scar' ? '🩹' : c.visualTrait === 'eyepatch' ? '🏴‍☠️' : c.visualTrait === 'red_scarf' ? '🧣' : c.visualTrait === 'gold_beak' ? '✨' : c.visualTrait === 'cyber_visor' ? '🤖' : '';
    return `<div class="portrait" style="--fx-duckov-pc:${col}"><img src="${portraitPath(c)}" alt="${esc(c.name)}" onerror="this.style.display='none'"><span class="trait">${trait}</span></div>`;
  }

  function shopView() {
    const c = store.currentClient;
    if (!c) return `<main class="morning"><section class="panel">队列已空。</section></main>`;
    const fm = FACTION_META[c.faction];
    return `<main class="shop">
      ${objectiveBanner()}
      <section class="panel client">
        <div class="queue-tag">队列 ${store.clientIndex + 1} / ${store.queue.length} · ${intentLabel(c)}</div>
        ${clientPortrait(c)}
        <h2>${esc(c.name)}</h2><p class="faction" style="color:${fm.color}">${esc(c.title)} · ${fm.name}（声望 ${store.rep[c.faction]}）</p>
        ${c.visualTrait ? `<p class="traitline">外观特征：${esc(WANTED_TRAITS.find(t => t.visualTag === c.visualTrait)?.label ?? '')}</p>` : ''}
        <blockquote>“${esc(c.dialogue)}”</blockquote>
        ${c.criminalTell && store.upgrades.has('upg_trade_terminal') ? `<p class="tell">终端标记：${esc(c.criminalTell)}</p>` : ''}
        <div class="client-actions">${clientActions(c)}</div>
      </section>
      <section class="panel deal"><img class="shop-scene" src="/art/scene/store.png" alt="地堡黑店柜台" onerror="this.style.display='none'">${dealView(c)}</section>
      ${inventoryView(false)}
    </main>`;
  }

  function intentLabel(c: ShopClient) {
    return c.intent === 'BUY' ? '🛒 求购客户' : c.intent === 'SELL' ? '💼 上门卖货' : c.intent === 'INSPECTION' ? '🚨 治安盘查' : c.intent === 'WHOLESALE_BUYER' ? '💰 大宗收购商' : c.intent === 'LANDLORD' ? '🏠 房东收租' : '📖 剧情访客';
  }

  function clientActions(c: ShopClient): string {
    const parts: string[] = [];
    if (c.isWanted || c.visualTrait) parts.push(`<button data-act="report">🚔 举报通缉（特征比对今日通缉令）</button>`);
    if (c.isCriminal) parts.push(`<button data-act="blackmail">🕶 敲诈 ${c.blackmailed ? '(已敲诈)' : ''}</button>`);
    parts.push(`<button data-act="skip-client">婉拒 / 请他离开</button>`);
    return parts.join('');
  }

  function dealView(c: ShopClient): string {
    if (c.intent === 'SELL' && c.offerItem) {
      const it = c.offerItem, def = ITEM_DEFS[it.defId]!;
      const base = itemPriceBuy(it, c);
      return `<h3>对方出售：${esc(def.name)}</h3>
      ${itemCard(it, true)}
      <p>收购基准价 <b>${base} cr</b>（标签品质 ${gradeName(it.claimedGrade)}${it.exposed ? ` · 已验明真实 ${gradeName(it.actualGrade)}` : ''}）</p>
      <div class="tools"><span>验货工具：</span>${(['visual', 'litmus', 'scanner'] as InspectTool[]).map(t => `<button data-act="tool" data-t="${t}" class="${selectedTool === t ? 'sel' : ''}">${t === 'visual' ? '🔍 紫外放大镜' : t === 'litmus' ? '🧪 化学试纸' : '📡 频谱扫描仪'}</button>`).join('')}
      <button class="warnbtn" data-act="inspect-offer">验货（用所选工具）</button></div>
      <p class="hint">选对工具才能揭穿：金属/光学看扫描、液体药品看试纸、外观造假看放大镜。用错只会浪费机会并得到提示。</p>
      <div class="bargain"><span>出价博弈（公开概率）：</span>
      ${[-25, -10, 0, 10].map(m => { const odds = computeBargainOdds({ markupPct: -m === 0 ? 0 : m < 0 ? -m : 0, faction: c.faction, factionRep: store.rep[c.faction], basePrice: base, isEventDriven: c.isEventDriven, hasTradeTerminal: store.upgrades.has('upg_trade_terminal') }); const price = Math.max(5, Math.round(base * (1 + m / 100))); return `<button data-act="buy-from" data-m="${m}">出价 ${price}cr ${m === 0 ? '（稳妥）' : m < 0 ? `(压价${-m}% · 成功约${odds.acceptPct}%)` : `(溢价+声望)`}</button>`; }).join('')}
      ${it.exposed && it.actualGrade < it.claimedGrade ? `<button class="warnbtn" data-act="buy-flaw">以瑕疵为由再砍 15% 成交</button>` : ''}
      </div>`;
    }
    if (c.intent === 'BUY') {
      const wantDef = c.requestedDefId ? ITEM_DEFS[c.requestedDefId] : undefined;
      const candidates = store.items.filter(i => (i.location === 'shop_stock' || i.location === 'showcase') && (c.requestedDefId ? i.defId === c.requestedDefId : ITEM_DEFS[i.defId]?.category === c.requestedCategory));
      return `<h3>求购：${wantDef ? esc(wantDef.name) : esc(c.requestedCategory ?? '任意好货')} <small>预算 ${c.budget} cr</small></h3>
      ${candidates.length === 0 ? `<p class="empty">库存里没有对版的货。从右侧库存把合适的货挪到柜台/展柜，或婉拒他。</p>` : candidates.map(it => { const base = itemPriceSell(it, c); return `<div class="dealrow">${itemCard(it, false)}<div class="bargain inline"><span>基准 ${base}cr：</span>${[0, 10, 25, 50, -10].map(m => { const odds = computeBargainOdds({ markupPct: m, faction: c.faction, factionRep: store.rep[c.faction], basePrice: base, isEventDriven: c.isEventDriven, hasTradeTerminal: store.upgrades.has('upg_trade_terminal') }); return `<button data-act="sell-to" data-uid="${it.uid}" data-m="${m}">${m === 0 ? `原价 ${base}` : m > 0 ? `+${m}% (${odds.acceptPct}%成)` : `-10% 让利`} </button>`; }).join('')}</div></div>`; }).join('')}`;
    }
    if (c.intent === 'INSPECTION') {
      const risky = store.items.filter(i => (i.location === 'shop_stock' || i.location === 'showcase') && ((ITEM_DEFS[i.defId]?.contrabandTier ?? 0) >= 2 || i.stolenHeat >= 40));
      return `<h3>🚨 治安盘查 · ${c.inspectorPersona === 'strict' ? '严格型' : c.inspectorPersona === 'corrupt' ? '可贿赂型' : '懒惰型'}</h3>
      <p>盘查只查柜台与展柜，<b>海报暗格绝对安全</b>。当前暴露风险品 ${risky.length} 件：</p>
      ${risky.map(i => itemCard(i, false)).join('') || '<p class="good">柜台干净，没有高危品。</p>'}
      <div class="bargain"><button class="warnbtn" data-act="inspection-submit">接受盘查</button>
      ${c.inspectorPersona === 'corrupt' ? `<button data-act="inspection-bribe">💸 贿赂 ${c.bribeCost} cr</button>` : `<button data-act="inspection-bribe">尝试塞钱（对方未必收）</button>`}
      <span class="hint">先去右侧库存把危险品挪进暗格再回来接受盘查！</span></div>`;
    }
    if (c.intent === 'WHOLESALE_BUYER') {
      const stock = store.items.filter(i => i.location === 'shop_stock' || i.location === 'showcase');
      const total = stock.reduce((a, i) => a + Math.round(itemPriceSell(i, c) * 0.85), 0);
      return `<h3>💰 周日大宗收购</h3><p>金掌柜愿以零售价 85% 现金全吃柜台+展柜 ${stock.length} 件货，合计 <b>${total} cr</b>。交租前最后的流动性阀门！</p>
      <div class="bargain"><button class="primary" data-act="wholesale-all" ${stock.length === 0 ? 'disabled' : ''}>全部卖出 +${total}cr</button></div>
      ${stock.map(i => itemCard(i, false)).join('')}`;
    }
    if (c.intent === 'LANDLORD') {
      const due = store.mortgageRemaining > 0 ? Math.min(store.mortgageWeekly, store.mortgageRemaining) : store.rent;
      return `<h3>🏠 ${store.mortgageRemaining > 0 ? '按揭周供到期' : '周租到期'}：${due} cr</h3>
      <p>当前现金 ${store.cash} cr。租金每周复利 +18%，越拖越贵。买断产权需 12000 cr 一次付清；按揭首付 1800 + 周供 920，还清即产权到手。</p>
      <div class="bargain">
      <button class="primary" data-act="pay-rent">支付 ${due} cr</button>
      ${store.mortgageRemaining === 0 ? `<button data-act="start-mortgage">办理按揭（首付 1800）</button>` : ''}
      <button class="goldbtn" data-act="buyout">💎 12000cr 现金买断产权（胜利）</button>
      <button class="warnbtn" data-act="cant-pay">实在没钱……</button></div>`;
    }
    // STORY
    return storyView(c);
  }

  function storyView(c: ShopClient): string {
    if (c.storyId === 'georgio') return `<h3>📖 乔治奥的火箭梦</h3><p>借他 300cr 并交给他一枚「微型姿态推进喷嘴」（夜袭码头暗仓可搜），7 天后他承诺 4 倍奉还并附赠脉冲枪蓝图情报。</p><div class="bargain"><button data-act="story-georgio">借钱 + 交喷嘴</button><button data-act="story-georgio-loan">只借 300cr</button></div>`;
    if (c.storyId === 'officer') return `<h3>📖 伦警官的止痛药</h3><p>给他真药，他欠你人情（治安声望+、证据消退）；给他蓝晶合剂，暴利人情更大，但罪证暗账会记上一笔。</p><div class="bargain"><button data-act="story-officer-med">送一盒正规医疗品</button><button data-act="story-officer-dust">递上蓝晶镇痛合剂</button></div>`;
    if (c.storyId === 'wu') return `<h3>📖 老吴与加密软盘</h3><p>夜袭码头暗仓搜出「J-Lab 加密监控软盘」交给他，或先装上磁芯解序仪破译成《37号黑幕铁证》，再决定卖给黑市、交给革命军还是呈交督察——三条路，三种结局加成。</p><div class="bargain"><button data-act="story-wu-disk">交出加密软盘 (+500cr)</button><button data-act="story-wu-hold">先留着，我要自己破译</button></div>`;
    if (c.storyId === 'georgio_return') return `<h3>📖 火箭升空之日</h3><p>乔治奥的土火箭居然真的升空了！他兑现承诺，连本带利送回一大笔钱，还留下一张手绘星图。</p><div class="bargain"><button class="goldbtn" data-act="story-georgio-return">收下 1600cr 与祝福</button></div>`;
    if (c.storyId === 'blackfile_choice') return `<h3>📖 《37号黑幕铁证》烫手抉择</h3><p>铁证在手，三方都在出价：</p><div class="bargain"><button data-act="blackfile-sell">卖给乌鸦黑市 (+1800cr，黑市声望+30)</button><button data-act="blackfile-rebel">交给革命军 (革命军+40，恩惠+200)</button><button data-act="blackfile-sec">呈交廉政督察 (治安+40，证据清零)</button></div>`;
    return `<h3>📖 访客</h3><p>${esc(c.dialogue)}</p>`;
  }

  function itemCard(it: DuckItem, showGrades: boolean): string {
    const def = ITEM_DEFS[it.defId]!;
    const col = RARITY_COLORS[def.rarity];
    const locName: Record<string, string> = { shop_stock: '柜台', showcase: '展柜', secret_stash: '暗格', equipped_weapon: '武器', equipped_helmet: '头盔', equipped_armor: '护甲', raid_backpack: '背包', safe_box: '安全箱' };
    return `<div class="item ${selectedUid === it.uid ? 'sel' : ''}" data-act="select-item" data-uid="${it.uid}" style="--fx-duckov-rc:${col}">
      <img class="itemicon" src="${itemIcon(it.defId)}" alt="" onerror="this.style.display='none'">
      <b>${esc(def.name)}</b><span class="rarity">${RARITY_NAMES[def.rarity]} · ${esc(locName[it.location] ?? it.location)}</span>
      <small>${showGrades || it.exposed ? `标签:${gradeName(it.claimedGrade)} / 真实:${gradeName(it.actualGrade)}${it.exposed ? ' ✔已验' : ''}` : `品质:${gradeName(it.claimedGrade)}${it.relabeled ? ' 🏷已贴标' : ''}`}${def.contrabandTier ? ` · C${def.contrabandTier}违禁` : ''}${it.stolenHeat ? ` · 热度${Math.round(it.stolenHeat)}` : ''}${def.armor ? ` · 甲${def.armor.armorLevel} 耐${Math.round(it.durability)}/${it.maxDurability}` : ''}${def.weapon ? ` · 伤${def.weapon.damage} 穿${def.weapon.armorPiercing}` : ''}</small>
    </div>`;
  }

  function inventoryView(compact: boolean): string {
    const groups: Array<[string, DuckItem['location']]> = [['🧰 已装备', 'equipped_weapon'], ['柜台库存', 'shop_stock'], ['✨ 展示柜（吸客/被查）', 'showcase'], ['🖼 海报暗格（免检）', 'secret_stash']];
    const sel = selectedUid ? store.find(selectedUid) : undefined;
    return `<section class="panel inventory ${compact ? 'wide' : ''}"><h3>📦 地堡库存 <small>点选一件进行操作</small></h3>
    ${groups.map(([label, loc]) => { const list = store.items.filter(i => loc === 'equipped_weapon' ? i.location.startsWith('equipped') : i.location === loc); return `<div class="inv-group"><h4>${label} (${list.length}${loc === 'secret_stash' ? '/' + store.stashCapacity : ''})</h4>${list.map(i => itemCard(i, false)).join('') || '<p class="empty">空</p>'}</div>`; }).join('')}
    ${sel ? itemActions(sel) : '<p class="hint">选中物品后可：挪柜台/展柜/暗格、贴假标、装备、装配件、喂机器。</p>'}
    </section>`;
  }

  function itemActions(it: DuckItem): string {
    const def = ITEM_DEFS[it.defId]!;
    const parts = [`<div class="itemops"><b>${esc(def.name)}</b>`];
    if (it.location !== 'shop_stock') parts.push(`<button data-act="move" data-uid="${it.uid}" data-loc="shop_stock">→柜台</button>`);
    if (it.location !== 'showcase') parts.push(`<button data-act="move" data-uid="${it.uid}" data-loc="showcase">→展柜</button>`);
    if (it.location !== 'secret_stash') parts.push(`<button data-act="move" data-uid="${it.uid}" data-loc="secret_stash">→暗格藏匿</button>`);
    if (def.category === 'weapon') parts.push(`<button data-act="equip" data-uid="${it.uid}">装备为武器</button>`);
    if (def.category === 'helmet') parts.push(`<button data-act="equip" data-uid="${it.uid}">戴上头盔</button>`);
    if (def.category === 'armor') parts.push(`<button data-act="equip" data-uid="${it.uid}">穿上护甲</button>`);
    if (def.category === 'attachment' && store.equippedWeapon()) parts.push(`<button data-act="attach" data-uid="${it.uid}">装到当前武器</button>`);
    if (!it.exposed || it.relabeled) parts.push(`<button data-act="relabel" data-uid="${it.uid}">🏷 贴假标升一档 (25cr)</button>`);
    if (def.category === 'med') parts.push(`<span class="hint">医疗品将在夜袭整备时自动携带</span>`);
    parts.push(`<button data-act="discard" data-uid="${it.uid}">丢弃</button></div>`);
    return parts.join('');
  }

  function afterView() {
    const w = store.equippedWeapon(), ws = w ? computeEffectiveWeaponStats(w) : null;
    return `<main class="after">
      <section class="panel"><h2>🌙 打烊整备 · 第 ${store.day} 天结束</h2>
      ${objectiveBanner()}
      <h3>🏭 后屋机器（装填后夜间自动结算）</h3>
      ${MACHINES.map(m => { const locked = m.unlockUpgradeId && !store.upgrades.has(m.unlockUpgradeId); const inputUid = store.machineInputs.get(m.id); const input = inputUid ? store.find(inputUid) : undefined; const candidates = store.items.filter(i => i.location === 'shop_stock' && m.inputFilter(i));
        return `<div class="machine"><b>${esc(m.name)}</b><small>${esc(m.subtitle)}</small><p>${esc(m.inputDesc)} → ${esc(m.outputDesc)}</p>
        ${locked ? `<span class="lock">🔒 需先购买升级「${esc(UPGRADES.find(u => u.id === m.unlockUpgradeId)?.name ?? '')}」</span>` : input ? `<span class="good">已装填：${esc(ITEM_DEFS[input.defId]?.name ?? '')}</span> <button data-act="machine-clear" data-id="${m.id}">取出</button>` : candidates.length ? candidates.slice(0, 3).map(cand => `<button data-act="machine-load" data-id="${m.id}" data-uid="${cand.uid}">装入 ${esc(ITEM_DEFS[cand.defId]?.name ?? '')}</button>`).join('') : '<span class="empty">柜台无对版原料（夜袭去搜）</span>'}</div>`; }).join('')}
      <h3>👑 王尔德升级网（现金购买，部分有夜维护费）</h3>
      <div class="upgrades">${UPGRADES.map(u => `<div class="upgrade"><b>${esc(u.name)}</b><small>${u.branch === 'security' ? '安保' : u.branch === 'industry' ? '产业' : '夜袭'} · ${u.cost}cr${u.nightlyUpkeep ? ` · 夜费${u.nightlyUpkeep}` : ''}</small><p>${esc(u.desc)}</p>${store.upgrades.has(u.id) ? '<span class="good">已拥有</span>' : `<button data-act="buy-upgrade" data-id="${u.id}" ${store.cash < u.cost ? 'disabled' : ''}>购买</button>`}</div>`).join('')}</div>
      </section>
      <section class="panel raidprep"><h3>🎒 夜袭整备</h3>
      <div class="prep-gallery"><img src="/art/maps/base.jpg" alt="基地俯视图" title="地堡基地俯视图"><img src="/art/maps/meshes_preview.jpg" alt="3D模型图鉴" title="已接入天工引擎的16个3D模型"></div>
      <p>${w ? `武器：<b>${esc(ITEM_DEFS[w.defId]?.name)}</b> 伤${ws?.damage} 穿${ws?.armorPiercing} 弹${ws?.magSize}` : '未装备武器！'} · 头盔 ${store.equippedHelmet() ? ITEM_DEFS[store.equippedHelmet()!.defId]?.name : '无'} · 护甲 ${store.equippedArmor() ? ITEM_DEFS[store.equippedArmor()!.defId]?.name : '无'}</p>
      <p class="hint">WASD 移动 · 鼠标瞄准 · 按住左键射击 · Shift 空格翻滚 · R 换弹 · E 开箱/撤离 · Q 用医疗品。阵亡丢背包，安全箱（${store.safeBoxCapacity}格，高稀有优先）保留。</p>
      <div class="zones">${RAID_ZONES.map(z => { const locked = z.requiredItemDefId && !store.hasDef(z.requiredItemDefId); return `<button class="zone" data-act="start-raid" data-id="${z.id}" ${locked || !w ? 'disabled' : ''}><img class="zone-thumb" src="/art/maps/${z.id}_card.png" alt=""><b>${esc(z.name)}</b><small>危险 ${'☠'.repeat(z.dangerLevel)} · ${z.enemyCount} 敌 · ${z.lootCrateCount} 箱 · ${z.timeLimitSec}s</small><span>${esc(z.subtitle)}</span>${locked ? '<em>🔒 需要 J-Lab 通行卡</em>' : ''}</button>`; }).join('')}</div>
      <button class="primary big" data-act="sleep">😴 不夜袭 · 直接睡觉到天亮</button>
      <p class="hint">夜袭代价：次日随机顾客 -1（熬夜脸）。成功撤离把背包战利品带回柜台。</p></section>
      ${inventoryView(false)}
    </main>`;
  }

  function endingView() {
    const win = store.phase === 'VICTORY';
    return `<main class="ending"><section class="panel hero"><h1>${win ? '👑' : '⛓'} ${esc(store.endingTitle)}</h1><p>${esc(store.endingText)}</p>
    <p>存活天数 ${store.day} · 累计进账 ${store.totalEarned}cr · 夜袭 ${store.totalRaids} 次 · 成功撤离 ${store.totalExtracts} 次</p>
    <button class="primary big" onclick="location.reload()">重新开店</button></section></main>`;
  }

  function logView() {
    return `<footer class="logbar">${store.logs.slice(0, 6).map(l => `<div class="log ${l.kind}">[D${l.day}] ${esc(l.text)}</div>`).join('')}</footer>`;
  }

  // ---------- actions ----------
  function bind() {
    root!.querySelectorAll<HTMLElement>('[data-act]').forEach(el => {
      el.addEventListener('click', (ev) => { ev.stopPropagation(); handle(el.dataset.act!, el.dataset); });
    });
  }

  function finishClient(msg: string, kind: 'good' | 'bad' | 'gold' | 'info' = 'info') {
    store.log(msg, kind); sfx.play(kind === 'good' || kind === 'gold' ? 'cash' : kind === 'bad' ? 'fail' : 'click');
    store.nextClient(); render();
  }

  function handle(act: string, d: DOMStringMap) {
    const c = store.currentClient;
    if (tut && !tutGrad && !TUT_FREE.includes(act)) {
      const st = tutSteps[tut.step];
      if (st && !st.allow(act, d)) { tutMsg = st.wrong ?? `先按师傅说的做：${st.title}`; sfx.play('fail'); render(); return; }
    }
    switch (act) {
      case 'tut-start': startTutorial(); break;
      case 'tut-skip': tut = null; break;
      case 'tut-continue': tut = null; tutGrad = false; break;
      case 'tut-restart': location.reload(); return;
      case 'tutorial': showTutorial = true; tutorialStep = 0; break;
      case 'close-tutorial': showTutorial = false; break;
      case 'tutorial-next': tutorialStep = Math.min(8, tutorialStep + 1); sfx.play('click'); break;
      case 'tutorial-prev': tutorialStep = Math.max(0, tutorialStep - 1); break;
      case 'career': sfx.play('bell'); showTutorial = false; store.startCareer(d.id!); break;
      case 'open-shop': sfx.play('shutter'); if (store.queue.length === 0) store.generateQueue(); store.phase = 'SHOP_OPEN'; break;
      case 'goto-after': store.phase = 'AFTERHOURS'; break;
      case 'select-item': selectedUid = d.uid!; sfx.play('click'); break;
      case 'tool': selectedTool = d.t as InspectTool; break;
      case 'move': { const it = store.find(d.uid!); if (it) { if (d.loc === 'secret_stash' && store.byLocation('secret_stash').length >= store.stashCapacity && it.location !== 'secret_stash') { store.log('暗格已满！先扩容或挪出东西。', 'bad'); } else { it.location = d.loc as DuckItem['location']; sfx.play('click'); } } break; }
      case 'equip': { const it = store.find(d.uid!); if (it) { const cat = ITEM_DEFS[it.defId]?.category; const loc = cat === 'weapon' ? 'equipped_weapon' : cat === 'helmet' ? 'equipped_helmet' : 'equipped_armor'; const prev = store.items.find(i => i.location === loc); if (prev) prev.location = 'shop_stock'; it.location = loc as DuckItem['location']; sfx.play('click'); store.log(`装备了 ${ITEM_DEFS[it.defId]?.name}。`); } break; }
      case 'attach': { const att = store.find(d.uid!), w = store.equippedWeapon(); if (att && w) { store.items = store.items.filter(i => i.uid !== att.uid); w.attachments.push(att); sfx.play('inspect_ok'); store.log('配件已装到武器上，夜袭属性提升。', 'good'); } break; }
      case 'relabel': { const it = store.find(d.uid!); if (it && store.cash >= 25) { store.cash -= 25; it.claimedGrade = clamp(it.claimedGrade + 1, -2, 3) as DuckItem['claimedGrade']; it.relabeled = true; it.exposed = false; store.crime += (ITEM_DEFS[it.defId]?.contrabandTier ?? 0) >= 2 ? 18 : 7; sfx.play('relabel'); store.log(`给 ${ITEM_DEFS[it.defId]?.name} 贴了假标（标签升至${gradeName(it.claimedGrade)}），罪值上升。卖给识货的派系会被揭穿！`); } else store.log('现金不足 25cr。', 'bad'); break; }
      case 'discard': store.items = store.items.filter(i => i.uid !== d.uid); selectedUid = null; break;
      case 'inspect-offer': { if (c?.offerItem) { const r = inspectItemWithTool(c.offerItem, selectedTool); store.log(r.message, r.exposedDiscrepancy ? 'bad' : 'good'); sfx.play(r.exposedDiscrepancy ? 'inspect_expose' : 'inspect_ok'); if (r.matchedTool) c.flawDiscountApplied = r.exposedDiscrepancy; } break; }
      case 'buy-flaw': { if (c?.offerItem) { const price = Math.round(itemPriceBuy(c.offerItem, c) * 0.85); doBuyFrom(c, price, true); } break; }
      case 'buy-from': { if (c?.offerItem) { const m = Number(d.m); const base = itemPriceBuy(c.offerItem, c); const price = Math.max(5, Math.round(base * (1 + m / 100)));
        if (m >= 0) { doBuyFrom(c, price, false); } else { const odds = computeBargainOdds({ markupPct: -m, faction: c.faction, factionRep: store.rep[c.faction], basePrice: base, isEventDriven: c.isEventDriven, hasTradeTerminal: store.upgrades.has('upg_trade_terminal') }); const roll = Math.floor(Math.random() * 100); const res = resolveBargainRoll(odds, roll); if (res === 'accept') doBuyFrom(c, price, false); else if (res === 'reject_stay') { store.log(`对方摇头：这个价不行（掷骰 ${roll}）。可以再试或原价收。`, 'bad'); sfx.play('fail'); } else finishClient('对方觉得你太黑，转身走了。', 'bad'); } } break; }
      case 'sell-to': { const it = store.find(d.uid!); if (it && c) doSellTo(c, it, Number(d.m)); break; }
      case 'report': { if (c) { if (c.isWanted) { store.cash += c.wantedBounty; addRep('security', 12); finishClient(`举报属实！治安部当场铐走通缉犯，赏金 +${c.wantedBounty}cr，治安声望+12。`, 'gold'); } else { addRep(c.faction, -10); addRep('security', -6); finishClient('误报！对方不是通缉犯，当众出丑，声望受损。', 'bad'); } } break; }
      case 'blackmail': { if (c && !c.blackmailed) { c.blackmailed = true; const chance = 0.55 + (store.rep.blackmarket >= 50 ? 0.15 : 0); if (Math.random() < chance) { const amt = 120 + Math.floor(Math.random() * 180); store.cash += amt; store.crime += 22; store.log(`敲诈成功 +${amt}cr，但罪值+22。对方恨恨地记下了你。`, 'gold'); sfx.play('cash'); } else { addRep(c.faction, -15); store.log('敲诈翻车！对方根本不吃这套，声望-15。', 'bad'); sfx.play('fail'); } } break; }
      case 'skip-client': if (c) finishClient(`婉拒了 ${c.name}。`, 'info'); break;
      case 'inspection-submit': doInspection(false); break;
      case 'inspection-bribe': doInspection(true); break;
      case 'wholesale-all': { if (c) { const stock = store.items.filter(i => i.location === 'shop_stock' || i.location === 'showcase'); const total = stock.reduce((a, i) => a + Math.round(itemPriceSell(i, c) * 0.85), 0); store.items = store.items.filter(i => !(i.location === 'shop_stock' || i.location === 'showcase')); store.cash += total; store.totalEarned += total; finishClient(`大宗出货 +${total}cr，柜台清空了。`, 'gold'); } break; }
      case 'pay-rent': { if (c) { const due = store.mortgageRemaining > 0 ? Math.min(store.mortgageWeekly, store.mortgageRemaining) : store.rent; if (store.cash >= due) { store.cash -= due; if (store.mortgageRemaining > 0) { store.mortgageRemaining -= due; if (store.mortgageRemaining <= 0) { store.phase = 'VICTORY'; store.endingTitle = '地堡之王 · 按揭还清'; store.endingText = `第 ${store.day} 天，你还清最后一笔按揭，地堡产权证烫着金边送到柜台上。从今夜起，卷帘门只为你自己升起。`; render(); break; } } else { store.rent = Math.round(store.rent * 1.18); } store.rentDueIn = 7; finishClient(`已支付 ${due}cr，下周房租 ${store.mortgageRemaining > 0 ? '按揭周供 ' + store.mortgageWeekly : store.rent + 'cr'}。`, 'good'); } else finishClient('现金不够交租！先卖货或办理按揭。', 'bad'); } break; }
      case 'start-mortgage': { if (store.cash >= 1800) { store.cash -= 1800; store.mortgageRemaining = 5520; store.mortgageWeekly = 920; store.rentDueIn = 7; finishClient('按揭生效：总贷 5520cr，周供 920。还清即赢得产权。', 'gold'); } else store.log('按揭首付需 1800cr，现金不足。', 'bad'); break; }
      case 'buyout': { if (store.cash >= 12000) { store.cash -= 12000; store.owned = true; store.phase = 'VICTORY'; store.endingTitle = '地堡之王 · 现金买断'; store.endingText = `第 ${store.day} 天，你把 12000cr 现金拍在房东桌上，买断了整座地堡。白天黑店，夜晚王座——鸭科夫从此姓你。`; render(); } else store.log(`买断需 12000cr，还差 ${12000 - store.cash}cr。`, 'bad'); break; }
      case 'cant-pay': { store.phase = 'GAME_OVER'; store.endingTitle = '卷帘门落下'; store.endingText = `第 ${store.day} 天，你交不起房租。房东收回地堡，黑店的灯牌在雨夜里熄灭了。`; render(); break; }
      case 'story-georgio': case 'story-georgio-loan': { if (c) { const thr = store.items.find(i => i.defId === 'val_thruster'); if (store.cash >= 300) { store.cash -= 300; if (act === 'story-georgio' && thr) { store.items = store.items.filter(i => i.uid !== thr.uid); store.story.georgio = 2; store.story.georgioReturn = store.day + 7; finishClient('乔治奥抱着喷嘴和钱狂奔而去，喊着七天后见。', 'gold'); } else finishClient('你只借了钱。乔治奥有些失望，但还是道了谢。', 'info'); } else finishClient('你囊中羞涩，婉拒了乔治奥。', 'bad'); } break; }
      case 'story-georgio-return': store.cash += 1600; store.totalEarned += 1600; store.favor += 80; finishClient('火箭升空！+1600cr，恩惠+80。乔治奥成了下层区的传说。', 'gold'); break;
      case 'story-officer-med': { const med = store.items.find(i => ITEM_DEFS[i.defId]?.category === 'med' && (i.location === 'shop_stock' || i.location === 'showcase')); if (med && c) { store.items = store.items.filter(i => i.uid !== med.uid); addRep('security', 18); store.evidence = clamp(store.evidence - 15, 0, 100); finishClient('伦警官收下真药，拍拍你的肩：以后盘查，我尽量睁一只眼。治安+18，证据-15。', 'good'); } else store.log('柜台没有医疗品可送。', 'bad'); break; }
      case 'story-officer-dust': { const dust = store.items.find(i => i.defId === 'chem_dreamdust'); if (dust && c) { store.items = store.items.filter(i => i.uid !== dust.uid); addRep('security', 25); store.crime += 35; store.cash += 260; finishClient('伦警官的手在抖。他塞给你 260cr，转身消失在雨幕里。治安+25，但罪值+35。', 'gold'); } else store.log('你没有蓝晶合剂（合成台或夜袭可得）。', 'bad'); break; }
      case 'story-wu-disk': { const disk = store.items.find(i => i.defId === 'int_encrypted_disk'); if (disk && c) { store.items = store.items.filter(i => i.uid !== disk.uid); store.cash += 500; addRep('blackmarket', 20); finishClient('老吴收下软盘：黑市欠你一个人情。+500cr，黑市+20。', 'gold'); } else store.log('你手头没有加密软盘——去码头暗仓夜袭搜刮。', 'bad'); break; }
      case 'story-wu-hold': if (c) finishClient('你决定自己破译。先买下磁芯解序仪升级吧。', 'info'); break;
      case 'blackfile-sell': { const f = store.items.find(i => i.defId === 'int_blackfile'); if (f) { store.items = store.items.filter(i => i.uid !== f.uid); store.cash += 1800; addRep('blackmarket', 30); store.crime += 30; finishClient('铁证卖给乌鸦黑市 +1800cr。鸭科夫的夜更深了。', 'gold'); } break; }
      case 'blackfile-rebel': { const f = store.items.find(i => i.defId === 'int_blackfile'); if (f) { store.items = store.items.filter(i => i.uid !== f.uid); addRep('rebel', 40); store.favor += 200; finishClient('革命军连夜印刷传单，铁喙高层丑闻传遍全城。革命军+40，恩惠+200。', 'gold'); } break; }
      case 'blackfile-sec': { const f = store.items.find(i => i.defId === 'int_blackfile'); if (f) { store.items = store.items.filter(i => i.uid !== f.uid); addRep('security', 40); store.evidence = 0; store.crime = Math.max(0, store.crime - 100); finishClient('督察塞尔皮科收下铁证，作为交换，你的暗账被一笔勾销。证据清零！', 'gold'); } break; }
      case 'machine-load': store.machineInputs.set(d.id!, d.uid!); sfx.play('click'); break;
      case 'machine-clear': store.machineInputs.delete(d.id!); break;
      case 'buy-upgrade': { const u = UPGRADES.find(x => x.id === d.id); if (u && store.cash >= u.cost) { store.cash -= u.cost; store.upgrades.add(u.id); if (u.id === 'upg_decoder') { store.items.push(createDuckItem('int_jlab_keycard', { actualGrade: 1, claimedGrade: 1 })); store.log('解序仪附赠 J-Lab 37号通行卡已入库！终极战区解锁。', 'gold'); } sfx.play('cash'); store.log(`已购升级：${u.name}`, 'good'); } break; }
      case 'sleep': doSleep(); break;
      case 'start-raid': startRaid(d.id!); return;
    }
    render();
  }

  function doBuyFrom(c: ShopClient, price: number, flawUsed: boolean) {
    if (!c.offerItem) return;
    if (store.cash < price) { store.log('现金不足，买不起这件货。', 'bad'); sfx.play('fail'); render(); return; }
    store.cash -= price;
    const it = c.offerItem; it.location = 'shop_stock';
    if ((ITEM_DEFS[it.defId]?.contrabandTier ?? 0) >= 2) store.crime += 8;
    if (it.stolenHeat > 40) store.crime += 6;
    store.items.push(it);
    addRep(c.faction, flawUsed ? -2 : 2);
    finishClient(`以 ${price}cr 收下 ${ITEM_DEFS[it.defId]?.name}${flawUsed ? '（瑕疵压价）' : ''}。`, 'good');
  }

  function doSellTo(c: ShopClient, it: DuckItem, markup: number) {
    const base = itemPriceSell(it, c);
    const odds = computeBargainOdds({ markupPct: markup, faction: c.faction, factionRep: store.rep[c.faction], basePrice: base, isEventDriven: c.isEventDriven, hasTradeTerminal: store.upgrades.has('upg_trade_terminal') });
    const price = Math.max(5, Math.round(base * (1 + markup / 100)));
    const doDeal = (p: number) => {
      // fake detection
      const meta = FACTION_META[c.faction];
      if (it.relabeled && it.claimedGrade !== it.actualGrade && Math.random() < meta.fakeDetectChance) {
        addRep(c.faction, -18); store.crime += 20; it.exposed = true;
        finishClient(`被识破了！${c.name} 当场揭穿假标，声望-18，罪值+20，这单黄了。`, 'bad'); return;
      }
      if (p > c.budget && markup > 0) { store.log(`对方预算只有 ${c.budget}cr，买不起这个价。试试原价或让利。`, 'bad'); sfx.play('fail'); render(); return; }
      store.items = store.items.filter(i => i.uid !== it.uid);
      store.cash += p; store.totalEarned += p;
      addRep(c.faction, odds.repDeltaOnDeal);
      const def = ITEM_DEFS[it.defId]!;
      if (def.contrabandTier >= 2 && c.faction !== 'security') store.crime += 10 + def.contrabandTier * 4;
      if (it.relabeled && c.faction !== 'tourist') store.crime += 8;
      finishClient(`成交！${def.name} 卖出 +${p}cr（${FACTION_META[c.faction].short}声望 ${odds.repDeltaOnDeal >= 0 ? '+' : ''}${odds.repDeltaOnDeal}）。`, 'gold');
    };
    if (markup <= 0) { doDeal(price); return; }
    const roll = Math.floor(Math.random() * 100);
    const res = resolveBargainRoll(odds, roll);
    if (res === 'accept') doDeal(price);
    else if (res === 'reject_stay') { store.log(`对方砍价失败但还没走（掷骰 ${roll}，接受线 ${odds.acceptPct}%）。再试一次或原价卖。`, 'bad'); sfx.play('fail'); render(); }
    else finishClient(`对方嫌太贵，甩袖而去（掷骰 ${roll}）。${FACTION_META[c.faction].short}声望 ${odds.repDeltaOnDeal}。`, 'bad');
  }

  function doInspection(bribe: boolean) {
    const c = store.currentClient; if (!c) return;
    if (bribe) {
      if (store.cash < (c.bribeCost ?? 99999)) { store.log('贿赂的钱都不够，场面一度尴尬。', 'bad'); render(); return; }
      if (c.inspectorPersona === 'strict') { store.evidence = clamp(store.evidence + 20, 0, 100); finishClient('铁面盘查员反手记下贿赂未遂：证据+20！老实接受盘查吧。', 'bad'); return; }
      store.cash -= c.bribeCost!;
      finishClient(`盘查员掂了掂信封，挥挥手走了。本次盘查跳过（-${c.bribeCost}cr）。`, 'gold'); return;
    }
    sfx.play('siren');
    const risky = store.items.filter(i => (i.location === 'shop_stock' || i.location === 'showcase') && ((ITEM_DEFS[i.defId]?.contrabandTier ?? 0) >= 2 || i.stolenHeat >= 40));
    if (risky.length === 0) { addRep('security', 6); finishClient('盘查通过，柜台干干净净。治安声望+6。', 'good'); return; }
    let caught = 0, fine = 0;
    for (const it of risky) {
      const detectP = c.inspectorPersona === 'strict' ? 0.92 : c.inspectorPersona === 'lazy' ? 0.42 : 0.65;
      if (Math.random() < detectP) { caught++; fine += 90 + (ITEM_DEFS[it.defId]?.contrabandTier ?? 0) * 70; store.items = store.items.filter(i => i.uid !== it.uid); }
    }
    if (caught === 0) { finishClient('盘查员草草翻了翻，没发现问题。你后背全是汗。', 'good'); return; }
    store.cash = Math.max(0, store.cash - fine); store.crime += caught * 28; store.evidence = clamp(store.evidence + caught * 14, 0, 100);
    finishClient(`查获 ${caught} 件违禁品！没收并罚款 ${fine}cr，罪值大涨。暗格里的货安然无恙。`, 'bad');
  }

  function doSleep() {
    // machine outputs
    for (const [mid, uid] of [...store.machineInputs.entries()]) {
      const m = MACHINES.find(x => x.id === mid); const input = store.find(uid);
      if (m && input) { store.items = store.items.filter(i => i.uid !== uid); const out = createDuckItem(m.outputDefId, { actualGrade: m.outputGrade, claimedGrade: m.outputGrade }); store.items.push(out); store.log(`${m.name} 产出：${ITEM_DEFS[m.outputDefId]?.name}（${gradeName(m.outputGrade)}）已入库。`, 'good'); if (m.outputDefId === 'int_blackfile') injectBlackfileChoice(); }
    }
    store.machineInputs.clear();
    store.settleNight();
    if (store.phase === 'MORNING') checkGeorgioReturn();
    render();
  }

  function injectBlackfileChoice() {
    // will be offered as story client next morning via queue check in open-shop generation: add flag
    store.story.blackfileReady = 1;
  }
  function checkGeorgioReturn() {
    if (store.story.georgio === 2 && store.day >= (store.story.georgioReturn ?? 999)) store.story.georgioDue = 1;
  }

  // Patch generateQueue for story returns/blackfile: wrap open-shop
  const origGen = store.generateQueue.bind(store);
  store.generateQueue = () => {
    origGen();
    if (store.story.georgioDue) { store.story.georgioDue = 0; store.story.georgio = 3; store.queue.unshift({ id: 'story_gr', name: '乔治奥', title: '火箭发明鸭', faction: 'lower', intent: 'STORY', portraitSeed: 7, isWanted: false, wantedBounty: 0, isCriminal: false, isEventDriven: true, dialogue: '我回来了！带着星星的味道！', budget: 0, storyId: 'georgio_return', bargainLocked: false, blackmailed: false, priceMultiplier: 1, flawDiscountApplied: false }); }
    if (store.story.blackfileReady) { store.story.blackfileReady = 0; store.queue.unshift({ id: 'story_bf', name: '三方买家', title: '铁证抉择', faction: 'blackmarket', intent: 'STORY', portraitSeed: 9, isWanted: false, wantedBounty: 0, isCriminal: false, isEventDriven: true, dialogue: '铁证在你手里，整座鸭科夫都在等你的开价。', budget: 0, storyId: 'blackfile_choice', bargainLocked: false, blackmailed: false, priceMultiplier: 1, flawDiscountApplied: false }); }
  };

  // ---------- RAID ----------
  function renderRaidShell() {
    if (!root || !raid) return;
    let host = root.querySelector<HTMLElement>('.raidwrap.r3');
    if (!host) { root.innerHTML = '<div class="raidwrap r3"></div>'; host = root.querySelector<HTMLElement>('.raidwrap.r3')!; }
    raid.start(host);
  }

  function startRaid(zoneId: string) {
    const zone = RAID_ZONES.find(z => z.id === zoneId) ?? RAID_ZONES[0]!;
    const weapon = store.equippedWeapon(); if (!weapon) return;
    const ws = computeEffectiveWeaponStats(weapon);
    const meds = store.items.filter(i => ITEM_DEFS[i.defId]?.category === 'med' && i.location === 'shop_stock').slice(0, 3);
    store.phase = 'NIGHT_RAID';
    const tutorialRaid = !!tut && !tutGrad;
    raid = new RaidGame3D(store, zone, ws, meds, (extracted, reason, usedMeds, loot, helmDur, armorDur) => {
      // consume meds
      for (const uid of usedMeds) store.items = store.items.filter(i => i.uid !== uid);
      const helm = store.equippedHelmet(), arm = store.equippedArmor();
      if (helm) helm.durability = Math.max(0, helmDur);
      if (arm) arm.durability = Math.max(0, armorDur);
      store.totalRaids++;
      let note: string;
      if (extracted) {
        store.totalExtracts++;
        for (const it of loot) { it.location = 'shop_stock'; store.items.push(it); }
        note = `夜袭成功撤离，带回 ${loot.length} 件战利品入柜台。`;
        sfx.play('extract');
      } else {
        const keep = [...loot].sort((a, b) => ITEM_DEFS[b.defId]!.rarity - ITEM_DEFS[a.defId]!.rarity).slice(0, store.safeBoxCapacity);
        for (const it of keep) { it.location = 'shop_stock'; store.items.push(it); }
        note = `夜袭失败（${reason}）：背包丢失，安全箱保住 ${keep.length} 件高价值战利品。`;
      }
      store.raidedLastNight = true;
      raid = null;
      // machine outputs also settle same night
      for (const [mid, uid] of [...store.machineInputs.entries()]) { const m = MACHINES.find(x => x.id === mid); const input = store.find(uid); if (m && input) { store.items = store.items.filter(i => i.uid !== uid); store.items.push(createDuckItem(m.outputDefId, { actualGrade: m.outputGrade, claimedGrade: m.outputGrade })); store.log(`${m.name} 在你夜袭时完成加工，产出已入库。`, 'good'); } }
      store.machineInputs.clear();
      store.settleNight(note);
      if (store.phase === 'MORNING') checkGeorgioReturn();
      if (tutorialRaid) tutGrad = true;
      render();
    }, { tutorial: tutorialRaid });
    render();
  }

  render();
  return { dispose() { disposed = true; raid?.dispose(); raid = null; } };
}

