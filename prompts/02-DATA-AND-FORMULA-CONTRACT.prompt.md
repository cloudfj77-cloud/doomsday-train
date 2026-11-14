# Prompt 02 · 核心数据结构与精确数学公式契约 (SSOT Schema & Math Contract)

> 本 Prompt 将两款游戏反编译源码中经过验证的核心公式，提炼为无歧义的工程契约。编写业务代码时必须严格按此实现，不得退化为拍脑袋随机数。

```xml
<math_and_schema_contract>

## 一、统一物品模型 `DuckItem`（融合 `GameItem` 双值欺诈 + `ItemStatsSystem` 树）

每件物品实例包含：
- `uid: string` / `defId: string` / `name: string`
- `category`: `'weapon' | 'armor' | 'helmet' | 'med' | 'water' | 'chem' | 'intel' | 'valuables' | 'material' | 'attachment'`
- `rarity`: `0 (白·普通) | 1 (绿·优良) | 2 (蓝·稀有) | 3 (紫·史诗) | 4 (金·传说)`
- `baseValue: number`（基础信用点 cr）
- **双值特征系统 (`ItemFeature` 机制)**：
  - `claimedGrade: number`（标签宣称档位 `-2 ~ +3`，对应劣质 `-50%`、瑕疵 `-25%`、标准 `0%`、精良 `+35%`、军规 `+80%`、特级 `+135%`）
  - `actualGrade: number`（真实内在档位 `-2 ~ +3`）
  - `exposed: boolean`（是否已被验货仪器揭穿；未揭穿前按 `claimedGrade` 计价，揭穿后强制按 `actualGrade` 计价且若卖家虚标可额外压价 `-15%`！）
  - `inspectType`: `'visual' (紫外放大镜) | 'litmus' (化学试纸) | 'scanner' (频谱/基因扫描仪)`
  - `relabeled: boolean`（是否被玩家用贴标机篡改过标签）
- **风险标签 (`TagSystem` 机制)**：
  - `stolenHeat: number`（赃物热度 `0 ~ 100`，代表治安盘查被当场查获的概率 `%`；每晚结算自动 `-18`，降至 `0` 洗白！）
  - `contrabandTier: 0 | 1 | 2 | 3 | 4`（违禁等级 C0~C4，售价溢价分别为 `0% / +35% / +70% / +100% / +150%`）
  - `location`: `'shop_stock' | 'showcase' | 'secret_stash' | 'raid_loadout' | 'raid_backpack' | 'safe_box' | 'machine_slot'`（其中 `secret_stash` 为海报后暗格，容量上限 4 件，治安盘查 100% 免检！）
- **搜打撤战斗属性 (`ItemStats` 机制，适用于武器/护甲/头盔/医疗品/配件)**：
  - 武器：`damage`, `fireRate`, `armorPiercing`, `armorBreak`, `critRate`（爆头率）, `critMult`, `spreadBase`, `spreadMax`, `recoil`, `magSize`, `reloadTime`, `pelletCount`, `element` (`'physical' | 'fire' | 'electric' | 'poison'`), `slots: DuckItem[]`（枪口/弹匣/握把配件直接做加法/乘法修正！）
  - 护甲/头盔：`armorLevel` (`1 ~ 6`), `durability`, `maxDurability`
  - 消耗品：`healAmount`, `stopsBleed: boolean`, `staminaRestore: number`

---

## 二、《可能是偷的》经营数学公式（源码级还原）

### 1. 三段式统一定价公式（源自 `GameItem.ComposeStagedValue`）
市场修正采用**先加法求和、再单次相乘**，防止多词条叠乘爆炸：
```ts
const gradeMod = GRADE_MODIFIERS[item.exposed ? item.actualGrade : item.claimedGrade]; // [-0.50, -0.25, 0, +0.35, +0.80, +1.35]
const durabilityRatio = item.maxDurability ? (0.4 + 0.6 * (item.durability / item.maxDurability)) : 1;
const attachmentValue = (item.slots ?? []).reduce((sum, s) => sum + computeItemPrice(s, ctx), 0);
const marketAdditive =
  ctx.difficultyRetailMarkup // 普通 +0.15
  + CONTRABAND_PREMIUM[item.contrabandTier] // [0, 0.35, 0.70, 1.00, 1.50]
  + ctx.activeEventModifierFor(item.category) // 市场新闻事件 -0.30 ~ +0.60
  + (ctx.clientFaction === 'tourist' ? 0.15 : 0); // 游客税 +15%
const stolenFenceDiscount = (!ctx.isSellingToClient && item.stolenHeat > 0) ? -0.20 : 0; // 收赃自动压价 20%

const finalPrice = Math.max(1, Math.round(
  ((item.baseValue * (1 + gradeMod) * durabilityRatio) * (1 + marketAdditive + stolenFenceDiscount) + attachmentValue)
  * ctx.bargainFinalMultiplier
));
```

### 2. 公开概率议价与声望暗账公式（源自 `BargainUIManager`）
设玩家要求的加价（卖给顾客）或压价（向顾客收购）幅度为 `p`（`p ∈ {10, 25, 50}`）：
- `repBonus = factionRep >= 0 ? factionRep / 6 : (factionRep / 6) * 2`（负声望惩罚加倍）
- `eventBonus = client.isEventDriven ? 15 : 0`
- `upgradeBonus = hasTradeTerminal ? 8 : 0`
- **接受线** `A = clamp(100 - 2.0 * p + repBonus + eventBonus + upgradeBonus, 0, 98)`
- **拒绝但留下再谈线** `B = clamp(100 - 1.2 * p + repBonus + eventBonus, A, 99)`
- 掷骰 `r = randomInt(0, 99)`：
  - `r < A`：**成交！** 但触发**贪心税**：若 `p > 0`，派系声望变动 `-(basePrice * (p / 20) * factionMult) / 400`；若主动让利打折，声望增加 `+(basePrice * (0.8 + p / 25) * factionMult) / 400`。
  - `A <= r < B`：**拒绝本次报价但留在柜台**（允许改报更温和的价码）。
  - `r >= B`：**愤而离店！** 派系声望直接 `-3`。
- **敲诈（仅对隐藏罪犯有效）**：成功率 `clamp(60 + blackmarketRep / 4, 20, 95)%`，成功则成交价直接向玩家倾斜 `20%`；若误敲诈清白市民，声望 `-8` 且治安罪值 `+40`。

### 3. 派系声望、罪证暗账与治安盘查公式
- **6 大派系**：`lower`（下层拾荒鸭，每涨 1 点返 40 王尔德恩惠）、`upper`（金羽上层区，高预算，不识普通假货）、`security`（铁喙治安部，拒收违禁品）、`blackmarket`（乌鸦黑市，收赃声望 `×2`）、`rebel`（红蹼革命军）、`tourist`（观光鸭团，无声望条，`+15%` 游客税且不识假货）。
- **造假识破率**：卖贴假标商品时，`security` 识破率 `85%`，`blackmarket` `75%`，`rebel` `45%`，`lower` `35%`，`upper` `15%`，`tourist` `0%`。被识破当场拒买、声望 `-4`、欺诈罪值 `+成交额 * 0.5`；未被识破成交后，也会记入延迟衰减的欺诈暗账（游客除外）。
- **罪值 → 证据进度 → 判决警官**：
  - 证据进度 `evidenceProgress ∈ [0, 100]`：每晚根据当前总 `crimeScore` 增长（`0`→`+0`，`1~99`→`+4`，`100~299`→`+9`，`300~699`→`+16`，`>=700`→`+28`；若雇佣“王尔德善后人”则每晚罪值额外 `-45`、证据 `-8`）。
  - 当 `evidenceProgress < 80` 时，每晚 `crimeScore = Math.max(0, crimeScore - 20)`。
  - 当 `evidenceProgress >= 100`：次日清晨**判决警官**必定登门！`crimeScore < 300` 罚款放行（清空证据）；`300 ~ 1499` 若无“王尔德豁免权”则逮捕结局；`>= 1500` 处决结局。
- **例行盘查员三人格**：`strict`（严格，绝不受贿）、`corrupt`（可贿赂，贿赂金 `clamp(100 + 4 * day - 2 * securityRep, 40, 450)`）、`lazy`（懒惰，漏检率 `50%`）。盘查只扫 `shop_stock` 与 `showcase`，**绝不扫 `secret_stash`（海报后 2×2 暗格）**！

### 4. 指数房租、流动性阀门与胜利买断公式
- 初始周租 `220 cr`，每 7 天（周日 `day % 7 === 0`）收租一次，收租后 `rent = Math.floor(rent * 1.08)`（针对紧凑单局体验调优的复利曲线）。
- **流动性阀门**：每周日收租官排在队尾，他前面必定插入一名高预算**周日大宗收购商**（以 `85%` 折扣爽快现金吃下你手头积压货，防止有货无现卡死）。
- **双重胜利出口**：
  1. **现金买断产权**：支付 `12,000 cr`（第 28 天后每日 `+150 cr`）直接买下黑店产权，达成自由结局并解锁无尽黑市大亨模式！
  2. **锁定按揭贷款**：周日现金 `>= 3,600 cr` 时可付首付 `1,800 cr`，将暴涨房租锁定为固定周供 `920 cr × 6 周`，供完即获产权胜利！

---

## 三、《逃离鸭科夫》3D 搜打撤数学公式（源码级还原）

### 1. 伤害、爆头与护甲减伤曲线（源自 `Health.cs:340-420`）
```ts
const isCritHeadshot = rng() < attacker.critRate; // 暴击即爆头，判定头盔护甲！
let dmg = baseDamage * (isCritHeadshot ? attacker.critMult : 1.0);
const targetArmorPiece = isCritHeadshot ? defender.helmet : defender.bodyArmor;
const armorVal = (targetArmorPiece && targetArmorPiece.durability > 0) ? targetArmorPiece.armorLevel : 0;

if (targetArmorPiece && targetArmorPiece.durability > 0) {
  targetArmorPiece.durability = Math.max(0, targetArmorPiece.durability - attacker.armorBreak);
}
const effectiveArmor = Math.clamp(armorVal - attacker.armorPiercing, 0, 999);
const armorMultiplier = 2 / (effectiveArmor + 2); // 甲2减50%，甲4减67%，甲6减75%；穿甲直接抵消护甲！
dmg *= armorMultiplier;

// 天气与元素联动（源自 Weathers 系统：雨夜火伤 -15%，电伤 +20%）
if (weather === 'rain') {
  if (element === 'fire') dmg *= 0.85;
  if (element === 'electric') dmg *= 1.20;
}
const finalDamage = Math.max(1, Math.round(dmg));
// 附带流血判定
if (rng() < bleedChance) applyBleed(defender); // 每秒持续扣血，需用止血绷带清除
```

### 2. 动作优先级抢占与翻滚冲刺（源自 `CharacterActionBase.cs` / `CA_Dash`）
- 优先级：`Idle/Move (0) < Reload (1) < Shoot (2) < UseMed (3) < Dash (4)`。
- 冲刺翻滚（`Space`）：消耗体力 `18`（总值 `100`，每秒恢复 `16`），冷却 `0.55s`，持续 `0.24s` 高速位移并短暂规避弹道，可随时抢占打断换弹或读条！

### 3. 搜刮箱品质读条公式（源自 `GameplayDataSettings.Looting.inspectingTimes`）
- 战区物资箱包含 2~4 件未揭示战利品，按 **战区 Tag 权重 × 稀有度权重** 生成。
- 逐格揭示读条时长：`[0.45s (白), 0.85s (绿), 1.35s (蓝), 2.0s (紫), 2.8s (金)]`——读条越久越说明里面藏着紫货/金货（如 J-Lab 加密软盘、金五角星、药用级梦尘原液、满配 SV98），配合音调爬升制造极致开箱多巴胺！
</math_and_schema_contract>
```
