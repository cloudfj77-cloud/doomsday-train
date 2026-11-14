import { CAREERS, FACTION_META, ITEM_DEFS, MARKET_EVENTS, RAID_ZONES, WANTED_TRAITS } from './catalog.ts';
import { clamp, computeEvidenceDailyGain, createDuckItem } from './formulas.ts';
import type { DuckItem, FactionId, GamePhase, InspectorPersona, ItemGrade, MarketEventDef, ShopClient } from './types.ts';

export interface LogEntry { day: number; text: string; kind: 'info' | 'good' | 'bad' | 'gold'; }
let clientSeq = 1;

const FIRST = ['杰夫', '橘子', '老吴', '泥巴', '红七', '鲍勃', '阿澈', '团团', '老K', '米娅', '大壮', '小满', '灰灰', '铁蛋', '露露', '阿泽'];
const TITLES: Record<FactionId, string[]> = {
  lower: ['废料拾荒鸭', '下水道淘金鸭', '夜班搬运鸭'],
  upper: ['金羽收藏家', '沙龙名媛鸭', '义体鉴赏家'],
  security: ['巡逻警鸭', '海关缉查员', '治安协查鸭'],
  blackmarket: ['蒙面掮客', '乌鸦信使', '暗巷中间人'],
  rebel: ['红蹼联络员', '地下军需官', '传单印刷鸭'],
  tourist: ['观光团领队鸭', '打卡旅客鸭', '纪念品猎手鸭'],
};

export class GameStore {
  phase: GamePhase = 'CAREER_SELECT';
  careerId = 'apprentice';
  day = 1; cash = 0; favor = 0; rent = 220; rentDueIn = 6;
  items: DuckItem[] = [];
  rep: Record<FactionId, number> = { lower: 0, upper: 0, security: 0, blackmarket: 0, rebel: 0, tourist: 0 };
  crime = 0; evidence = 0;
  upgrades = new Set<string>();
  graceUsed = false; fixerHired = false;
  machineInputs = new Map<string, string>(); // machineId -> item uid
  queue: ShopClient[] = []; clientIndex = 0;
  events: MarketEventDef[] = [];
  wantedTraitId = 'scar'; wantedName = '左眼刀疤鸭';
  logs: LogEntry[] = [];
  mortgageRemaining = 0; mortgageWeekly = 0; owned = false;
  story: Record<string, number> = {};
  totalEarned = 0; totalRaids = 0; totalExtracts = 0; endingTitle = ''; endingText = '';
  raidedLastNight = false;

  log(text: string, kind: LogEntry['kind'] = 'info') { this.logs.unshift({ day: this.day, text, kind }); if (this.logs.length > 60) this.logs.pop(); }
  get currentClient(): ShopClient | undefined { return this.queue[this.clientIndex]; }
  get stashCapacity(): number { return this.upgrades.has('upg_stash_expand') ? 8 : 4; }
  get safeBoxCapacity(): number { return this.upgrades.has('upg_safebox_plus') ? 4 : 2; }
  byLocation(loc: DuckItem['location']) { return this.items.filter(i => i.location === loc); }
  find(uid: string) { return this.items.find(i => i.uid === uid); }
  equippedWeapon() { return this.byLocation('equipped_weapon')[0]; }
  equippedHelmet() { return this.byLocation('equipped_helmet')[0]; }
  equippedArmor() { return this.byLocation('equipped_armor')[0]; }
  hasDef(defId: string) { return this.items.some(i => i.defId === defId); }

  startCareer(id: string) {
    const c = CAREERS.find(x => x.id === id) ?? CAREERS[0]!;
    this.careerId = c.id; this.cash = c.startingCash; this.favor = c.startingFavor; this.rent = c.startingRent;
    for (const [k, v] of Object.entries(c.repInit)) this.rep[k as FactionId] = v ?? 0;
    this.upgrades = new Set(c.unlockedUpgrades);
    this.items = c.starterItemSpecs.map(s => createDuckItem(s.defId, { actualGrade: s.actualGrade, claimedGrade: s.claimedGrade, stolenHeat: s.heat ?? 0, location: s.location ?? 'shop_stock' }));
    this.phase = 'MORNING';
    this.rollDaySetup(true);
    this.log(`以「${c.name}」身份接手地堡黑店。现金 ${this.cash} cr，王尔德恩惠 ${this.favor}。`, 'gold');
  }

  rollDaySetup(first = false) {
    // 1-2 market events, avoid same category twice
    const pool = [...MARKET_EVENTS].sort(() => Math.random() - 0.5);
    this.events = pool.slice(0, this.day % 3 === 0 ? 2 : 1);
    const trait = WANTED_TRAITS[Math.floor(Math.random() * WANTED_TRAITS.length)]!;
    this.wantedTraitId = trait.id; this.wantedName = trait.label;
    if (!first) this.log(`今日头条：${this.events.map(e => e.headline).join(' / ')}`, 'info');
  }

  generateQueue() {
    const q: ShopClient[] = [];
    const isSunday = this.rentDueIn === 0;
    const day = this.day;
    const mk = (p: Partial<ShopClient> & { faction: FactionId; intent: ShopClient['intent'] }): ShopClient => ({
      id: `c${clientSeq++}`, name: FIRST[Math.floor(Math.random() * FIRST.length)]!, title: TITLES[p.faction][Math.floor(Math.random() * TITLES[p.faction].length)]!,
      portraitSeed: Math.floor(Math.random() * 10000), isWanted: false, wantedBounty: 0, isCriminal: false, isEventDriven: false,
      dialogue: '', budget: 300, bargainLocked: false, blackmailed: false, priceMultiplier: 1, flawDiscountApplied: false, ...p,
    } as ShopClient);

    // Story clients (cross day/night lines)
    if (day >= 2 && !this.story.georgio) {
      this.story.georgio = 1;
      q.push(mk({ faction: 'lower', intent: 'STORY', storyId: 'georgio', name: '乔治奥', title: '狂热发明鸭', dialogue: '店主！借我 300cr，再帮我找一个「微型姿态推进喷嘴」，我要造火箭飞向蛋星！七天后四倍奉还！', requestedDefId: 'val_thruster', budget: 0 }));
    } else if (day >= 4 && !this.story.officer) {
      this.story.officer = 1;
      q.push(mk({ faction: 'security', intent: 'STORY', storyId: 'officer', name: '伦警官', title: '铁喙巡逻警长', dialogue: '昨夜在零号街区挨了一刀…你这儿有靠谱的止痛药吗？别声张。', requestedCategory: 'med', budget: 260 }));
    } else if (day >= 6 && !this.story.wu) {
      this.story.wu = 1;
      q.push(mk({ faction: 'blackmarket', intent: 'STORY', storyId: 'wu', name: '老吴', title: '情报掮客', dialogue: '码头海关藏着一张 J-Lab 加密监控软盘。夜里替我取回来，解序仪破译后，我们能卖个大价钱——或者捅翻天。', requestedDefId: 'int_encrypted_disk', budget: 500 }));
    }

    // Showcase attraction: a buyer for showcased category
    const show = this.byLocation('showcase')[0];
    if (show) {
      const def = ITEM_DEFS[show.defId]!;
      const factions: FactionId[] = def.contrabandTier >= 2 ? ['blackmarket', 'rebel'] : ['upper', 'lower', 'tourist'];
      const f = factions[Math.floor(Math.random() * factions.length)]!;
      q.push(mk({ faction: f, intent: 'BUY', requestedCategory: def.category, requestedDefId: show.defId, isEventDriven: true, budget: 500 + Math.random() * 700, dialogue: `展柜里那件${def.name}我盯了很久了，开个价吧。` }));
    }

    // Inspector: guaranteed if showcase has contraband, else periodic
    const showcaseRisk = this.byLocation('showcase').some(i => (ITEM_DEFS[i.defId]?.contrabandTier ?? 0) >= 2 || i.stolenHeat >= 50);
    const inspectorDue = showcaseRisk || day % 3 === 0 || this.evidence >= 55;
    if (inspectorDue) {
      const personas: InspectorPersona[] = ['strict', 'corrupt', 'lazy'];
      const persona = personas[Math.floor(Math.random() * personas.length)]!;
      q.push(mk({ faction: 'security', intent: 'INSPECTION', name: '盘查员·' + (persona === 'strict' ? '铁面' : persona === 'corrupt' ? '油滑' : '困倦'), title: '铁喙治安盘查员', inspectorPersona: persona, bribeCost: clamp(Math.round(120 + day * 4 - this.rep.security * 2), 40, 450), dialogue: persona === 'strict' ? '例行盘查。把柜台和展柜都打开，别耍花样。' : persona === 'corrupt' ? '例行盘查……当然，懂规矩的人，事情都好商量。' : '例行盘查。快点，我还想回去补个觉。' }));
    }

    // Regular clients 4-6
    const n = 4 + Math.floor(Math.random() * 3) - (this.raidedLastNight ? 1 : 0);
    for (let i = 0; i < Math.max(3, n); i++) q.push(this.randomClient(mk));

    // Sunday liquidity valve + landlord at tail
    if (isSunday) {
      q.push(mk({ faction: 'upper', intent: 'WHOLESALE_BUYER', name: '周日收购商·金掌柜', title: '大宗现金收购商', budget: 99999, dialogue: '听说今天有人要交租？我现金全吃你积压的货，八五折，不墨迹。' }));
      q.push(mk({ faction: 'security', intent: 'LANDLORD', name: '房东·铁算盘', title: '地堡产权代理人', dialogue: this.mortgageRemaining > 0 ? `按揭周供 ${this.mortgageWeekly}cr 到期了。` : `周租 ${this.rent}cr，今天到期。` }));
    }
    // Shuffle middle but keep landlord last-ish: keep story first, then shuffle rest except Sunday tail
    const tail = isSunday ? q.splice(q.length - 2, 2) : [];
    const head = q.length && q[0]?.intent === 'STORY' ? [q.shift()!] : [];
    for (let i = q.length - 1; i > 0; i--) { const j = Math.floor(Math.random() * (i + 1)); [q[i], q[j]] = [q[j]!, q[i]!]; }
    this.queue = [...head, ...q, ...tail];
    this.clientIndex = 0;
  }

  private randomClient(mk: (p: Partial<ShopClient> & { faction: FactionId; intent: ShopClient['intent'] }) => ShopClient): ShopClient {
    const factions: FactionId[] = ['lower', 'lower', 'upper', 'blackmarket', 'rebel', 'tourist', ...(this.rep.upper >= 20 ? (['upper'] as FactionId[]) : []), ...(this.rep.blackmarket >= 20 ? (['blackmarket'] as FactionId[]) : [])];
    const faction = factions[Math.floor(Math.random() * factions.length)]!;
    const sell = Math.random() < 0.52;
    const isWanted = Math.random() < 0.14;
    const trait = isWanted ? WANTED_TRAITS.find(t => t.id === this.wantedTraitId) : WANTED_TRAITS[Math.floor(Math.random() * WANTED_TRAITS.length)];
    if (sell) {
      // Client sells an item, often mislabeled
      const defs = Object.values(ITEM_DEFS).filter(d => !['int_blackfile'].includes(d.id));
      const def = defs[Math.floor(Math.random() * defs.length)]!;
      const actual = ([-2, -1, 0, 0, 1, 1, 2] as ItemGrade[])[Math.floor(Math.random() * 7)]!;
      const lying = Math.random() < 0.48;
      const claimed = lying ? (clamp(actual + 1 + (Math.random() < 0.4 ? 1 : 0), -2, 3) as ItemGrade) : actual;
      const item = createDuckItem(def.id, { actualGrade: actual, claimedGrade: claimed, stolenHeat: def.contrabandTier >= 2 ? 25 + Math.random() * 45 : (Math.random() < 0.2 ? 30 : 0) });
      return mk({ faction, intent: 'SELL', offerItem: item, isWanted, wantedBounty: isWanted ? 350 + Math.floor(Math.random() * 400) : 0, visualTrait: trait?.visualTag, isCriminal: Math.random() < 0.22, criminalTell: '他说话时总瞥向门口，袖口沾着海关封条的碎屑。', budget: 9999, dialogue: `这件${def.name}可是${claimed >= 2 ? '军规好货' : '正经渠道来的'}，你给个实在价。` });
    }
    const cats = Object.values(ITEM_DEFS).map(d => d.category).filter((v, i, a) => a.indexOf(v) === i);
    const cat = cats[Math.floor(Math.random() * cats.length)]!;
    const budgetBase = faction === 'upper' ? 700 : faction === 'tourist' ? 550 : faction === 'blackmarket' ? 600 : 380;
    return mk({ faction, intent: 'BUY', requestedCategory: cat, isWanted, wantedBounty: isWanted ? 350 + Math.floor(Math.random() * 400) : 0, visualTrait: trait?.visualTag, isCriminal: Math.random() < 0.18, criminalTell: '他的报价单背面，隐约印着乌鸦黑市的暗纹。', budget: Math.round(budgetBase * (0.7 + Math.random() * 0.9)), dialogue: cat === 'weapon' ? '最近封锁区不太平，我想添件趁手的家伙。' : cat === 'med' ? '家里有人等着用药，有靠谱的货吗？' : cat === 'water' ? '净水越来越紧俏，你这儿还有存货？' : '听说你这儿总能淘到别处没有的好东西。' });
  }

  nextClient() { this.clientIndex++; if (this.clientIndex >= this.queue.length) { this.phase = 'AFTERHOURS'; this.log('今日顾客接待完毕，进入打烊整备。', 'info'); } }

  // Night settlement after sleep OR after raid returns
  settleNight(raidNote?: string) {
    const lines: string[] = [];
    // machines
    for (const [machineId, uid] of [...this.machineInputs.entries()]) {
      const item = this.find(uid);
      if (!item) continue;
      this.items = this.items.filter(i => i.uid !== uid);
      // output def resolved by caller via catalog in UI; here generic handled in UI before calling? UI sets pendingOutputs
      lines.push(`机器 ${machineId} 完成一批加工。`);
    }
    this.machineInputs.clear();
    // heat decay + crime decay
    for (const i of this.items) i.stolenHeat = Math.max(0, i.stolenHeat - 18);
    const gain = computeEvidenceDailyGain(this.crime);
    if (this.upgrades.has('upg_fixer')) { this.crime = Math.max(0, this.crime - 45); this.evidence = clamp(this.evidence - 8, 0, 100); }
    if (this.rep.rebel >= 50) this.evidence = clamp(this.evidence - 4, 0, 100);
    this.evidence = clamp(this.evidence + gain, 0, 100);
    if (this.evidence < 80) this.crime = Math.max(0, this.crime - 20);
    // upkeep
    let upkeep = 8 + Math.floor(this.day * 0.6);
    for (const u of this.upgrades) { const def = (requireUpkeep(u)); upkeep += def; }
    this.cash -= upkeep; lines.push(`夜间电费与线人维护 -${upkeep}cr。证据进度 ${Math.round(this.evidence)}/100，罪值 ${Math.round(this.crime)}。`);
    if (raidNote) lines.push(raidNote);
    for (const l of lines) this.log(l, 'info');
    // judge
    if (this.evidence >= 100) {
      if (this.upgrades.has('upg_sec_grace') && !this.graceUsed) {
        this.graceUsed = true; this.upgrades.delete('upg_sec_grace'); this.evidence = 20; this.crime = 0;
        this.log('判决警官登门！王尔德豁免令生效，证据当场销毁，你逃过一劫。', 'gold');
      } else if (this.crime >= 300) {
        this.phase = 'GAME_OVER'; this.endingTitle = '铁喙判决 · 锒铛入狱'; this.endingText = `第 ${this.day} 天清晨，判决警官带着完整证据链封了你的黑店。罪值 ${Math.round(this.crime)}，地堡卷帘门最后一次落下。`; return;
      } else {
        const fine = Math.min(this.cash, 400 + Math.round(this.crime));
        this.cash -= fine; this.evidence = 0; this.crime = 0;
        this.log(`判决警官罚款 ${fine}cr 后放行，证据清零。下次可没这么便宜了。`, 'bad');
      }
    }
    // day advance + rent countdown
    this.day++;
    if (this.rentDueIn > 0) this.rentDueIn--; // landlord day will be when 0; after landlord handled UI resets
    this.raidedLastNight = false;
    this.rollDaySetup();
    if (this.phase !== 'GAME_OVER') this.phase = 'MORNING';
  }
}
function requireUpkeep(id: string): number { if (id === 'upg_informant') return 12; if (id === 'upg_fixer') return 22; return 0; }
export { FACTION_META };
