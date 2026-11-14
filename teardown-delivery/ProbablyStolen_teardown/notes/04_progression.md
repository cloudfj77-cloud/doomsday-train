# 04 成长系统与外围玩法 —— 《Probably Stolen》逆向拆解

> 方向：开局选择 / 王尔德人脉升级网络 / 夜间活动 / 彩票·报纸·电话·配给站·雇员 / 前身 Deepspace Emporium 的代码考古 / 设计点评
> 资料：`~/ps_work/src/*.cs`（反编译 C#），`~/ps_work/loc/chinese.json`（中文本地化）。文件名后的 `:行号` 指反编译源码中的位置。
> 约定：游戏日 `dayCounter` 从 1 开始，`dayCounter % 7`：0=周日、1=周一 … 6=周六（`AfterhourWeekday.cs`，`StartingPerkContent.HandleWelfareProgram` 把 `%7==1` 当周一，与文案“每周一”一致）。

---

## 0. 总览：一局游戏的成长骨架

```
开局：难度(4档) × 专精/职业(9个可选) × 起始特性(48个, 3点预算/4槽)
   │
   ├─ 第1–13天：导师按天投放新系统（夜间活动D2、杰克逊D6、王尔德网络D7、报纸D8、违禁品D9、食品券D11、赃物D13、彩票D15起）
   │
   ├─ 中期：王尔德人脉网络（基础页49个可见升级，总价29,790）
   │        ← 支付方式：先扣“王尔德恩惠”，不足再扣现金
   │        ← 派系声望Perk（6个派系，±20/40/60/80/100阈值）
   │        ← 退休供货商每周固定日来访（解锁一条生产线就多一个周常NPC）
   │
   ├─ 目标：买下店铺（普通难度基础价12,000，第50天后每天涨价）
   │
   └─ 终局：买下店铺后才出现“高级”页（4个升级，总价20,000）
            → 超科技走私贩 → 心智重编程器（局中改起始特性）
```

主要系统入口：

| 系统 | 核心文件 | 状态 |
|---|---|---|
| 开局专精 | `StartingScenario.cs`, `NewGameData.cs`, `NewGameSpecializationUI.cs` | 启用 |
| 起始特性 | `StartingPerkList.cs`, `StartingPerk.cs`, `StartingPerkContent.cs`, `NewGamePerksUI.cs` | 启用 |
| 局中改特性 | `MindReprogrammerHelper.cs` | 启用（终局物品） |
| 王尔德人脉网络 | `NetworkUpgrade.cs`, `NetworkUpgradeList.cs`, `NetworkUpgradeLayout.cs`, `WildNetworkUI.cs` | 启用（`WildUIManager.cs` 是旧版UI，逻辑与新UI重复） |
| 夜间活动 | `AfterhourLocation.cs`, `AfterhourEvents.cs`, `VanillaAfterhourEvents.cs`, `MapUIManager.cs`, `TransitUI.cs`, `ScavHelper.cs` | 启用 |
| 配给站/食品券 | `CommissaryData.cs`, `CommissaryLocation.cs`, `CommissaryUIManager.cs` | 启用 |
| 彩票/刮刮乐 | `LotteryHelper.cs`, `ScratcherHelper.cs` | 启用 |
| 报纸 | `NewsPaper.cs`, `NewsUIManager.cs`, `NewsUIElement.cs` | 启用 |
| 电话/供货商 | `PhoneUIManager.cs`, `StorePhoneClient.cs`, `PhoneClientList.cs`, `PhoneDialogList.cs` | 启用 |
| 雇员/远征 | `Hireling.cs`, `Expedition*.cs` | **《Probably Stolen》时期写的半成品，入口没接上** |
| 飞船/太空/战斗/突袭 | `GameMaster.cs`, `Ship*.cs`, `Space*Trigger.cs`, `TraderStation*Trigger.cs`, `Combat*.cs`, `Raid*.cs`, `TurnManager.cs` | **Deepspace Emporium 遗留，没有入口** |
| 明确的废弃文件 | `UnusedDirectory.cs`, `EventUnused.cs`, `StoreClientListUnused.cs`, `IntelUnusedHelper.cs` | **零引用** |

---

## 1. 开局选择

### 1.1 难度（`Difficulty.cs:202-390`，属于开局选择的一部分，只列与成长相关的字段）

| 字段 | cozy「哄抬物价」 | normal「加价」 | hard「薄利」 | challenge「赤字」 |
|---|---|---|---|---|
| upgradePriceMultiplier（王尔德升级价格倍率） | ×1 | ×1 | **×1.5** | **×2** |
| networkIntroFavor（网络引入时送的恩惠） | 300 | 200 | 100 | 0 |
| tradeFavorRate（每笔交易按原值返恩惠） | 10% | 5% | 0 | 0 |
| wildServiceFavorRefund（王尔德夜间服务费返还为恩惠） | 75% | 50% | 25% | 15% |
| landlordGraceOwnedAtStart（开局就有“租金宽限”） | 是 | 是 | 否 | 否 |
| wildBailout（财富下滑时王尔德送250恩惠） | 是 | 是 | 否 | 否 |
| wildReminders（6天没看网络就来提醒） | 是 | 是 | 否 | 否 |
| rentIncreaseMultiplier（每周房租涨幅） | 1.025 | 1.05 | 1.1 | 1.1 |
| propertyBasePrice（买店基础价） | 12,000 | 12,000 | 24,000 | 36,000 |
| propertyPriceIncreasePerDay / 上限（第50天起） | 25 / 3,000 | 50 / 5,000 | 50 / 10,000 | 100 / 20,000 |
| retiredSupplierMarkup（退休供货商加价%） | 0 | 15 | 30 | 45 |

开局通用参数（`PlayerStore.cs:806-830`）：现金 **600**、房租 **200**（`startingRent`）、导师贷款 200（12天后还）、第一次交租在 6 天后，之后每 7 天交一次。另有“无尽模式”开关（`NewGameData.endlessMode`）。

### 1.2 专精 / 开局场景（`StartingScenario.cs:220-248`，开局物资在 `NewGameData.HandleInitialItem` 中）

UI 每页 10 个，图标横向排列；`comingSoon=true` 的显示为“开发中(wip)”且不能开局；`hidden=true` 的不显示。展会模式（`isShowcase`）只能选学徒。

| # | id（StartType） | 中文名 | 标签 | 现金变化 | 房租变化 | 声望/解锁 | 主要开局物资 | 跳过序章 |
|---|---|---|---|---|---|---|---|---|
| 1 | apprentice (APPRENTICE) | 学徒 | 适合初次游玩 | −50 → **550** | — | — | 破旧储藏区 storage_bay，肉罐头×2、果汁、牙膏×2、厕纸×2 | 否 |
| 2 | water_merchant | 水商 | — | −200 → 400 | — | 直接拥有 **WATER_TRADER**（价值300） | 水资源手册、检测试纸/净水片整盒、幽灵水大桶、铁锈水壶、各尺寸瓶装水、滤水器、破旧便携净水器(75%耐久)、破旧瓶子打印机 | 否 |
| 3 | fence | 销赃人 | — | −150 → 450 | **+50** | 黑市 +30，治安部 −30，解锁地下交易所 | 破旧走私者暗格 smuggler_bay、梦尘×2、Fair 品相重型自制枪+弹×2、医疗/工程/治安钥匙卡、黑光灯×2 | 是 |
| 4 | rock_bottom | 一贫如洗 | 极限挑战（红色标签） | 现金 = **0** | — | 关闭王尔德救济和升级提醒（`IsRockBottomStart`） | 什么都没有，连开局的 4 盘损坏磁带也不给 | 是 |
| 5 | hydroponic_farmer (FARMER) | 水培农场主 | 进阶开局 | −150 → 450 | **+40** | 直接拥有 **RETIRED_FARMER**（350） | 水培手册、破旧旧式水培机、“老旧水培系统”（2 个生态模块+2 个损坏模块，都卡死拆不下来）、种子、营养片盒、水 | 否 |
| 6 | gunsmith | 枪械匠 | 进阶开局 | −450 → 150 | **+50** | 治安部 +25；直接拥有 **GUN_PERMIT_I + RETIRED_GUNSMITH**（共950），展示柜里放好 I 级许可证 | 旧枪械打印机、塑料×4、枪械储藏区、各类小口径弹药、多把不同品相的手枪、消音器/红点/弹匣/激光等模组、螺丝刀、枪械零件 | 是 |
| 7 | chemist (DRUG_DEALER) | 化学家 | 进阶开局 | −300 → 300 | — | 治安部 −15；解锁**卡特尔保护**服务和**卡特尔**派系（第6个派系，只在这个开局出现）；直接拥有 **RETIRED_CHEMIST**（1000） | 搅拌机、加热板、蒸发器、成品机、滴管、量筒、化学扫描仪、迷你走私暗格、化学手册 | 是 |
| 8 | rancher | 养鼠人 | 进阶开局 | 不变 600 | — | 直接拥有 **RETIRED_RANCHER**（750） | 破旧动物笼（带投喂器）、公母鼠各 3 只（年龄 5/3/1）、剁肉刀、标签机、记号笔、2 次用量的绿色血清、小营养果×6 | 否 |
| 9 | aquafarmer | 水产养殖户 | 进阶开局 | −450 → 150 | **+40** | 直接拥有 **AQUACULTURE**（400；正常情况下它的前置是 WATER_TRADER，这里跳过了） | 破旧 RAS 循环水系统×2、成年罗非鱼公母各 1、受精鱼卵、工业鱼饲料×2、废金属、普通电子件、刀、幽灵水×2 | 否 |
| — | junker (SCAV) | 废料客 | 开发中 | （代码里写了：−200） | — | 解锁隐藏升级 RUINED_MACHINE_UNLOCK | 拾荒令牌、铸锭手册、金属扫描仪、手电、挎包、熔炉 | — |
| — | organ_trader | 器官贩子 | 开发中 | **没有实现分支** | | | 文案写的是“器官冷藏容器、输血套件”（对应 UnusedDirectory 里的 organ_* 物品） | |
| — | street_food_vendor | 街头小吃摊贩 | 开发中 | **没有实现分支** | | | 文案写的是“炉灶、食材” | |
| — | moonshiner | 私酒贩子 | 开发中 + 隐藏 | **没有实现分支** | | | | |
| — | pharmacist | 药剂师 | 开发中 + 隐藏 | **没有实现分支** | | | | |
| — | MODDED | 模组场景 | — | 调用 `StartingScenario.onStart` | | | | |

要点：
- **专精 = 免费拿到一条生产线的入门升级 + 一套设备，代价是现金和房租。** 把“送了价值多少的网络升级”算进去：枪械匠 950 + 设备，化学家 1000 + 卡特尔线，养鼠人 750 且不扣钱，所以“进阶开局”的强度差别其实不小。
- 学徒的文案说“增加初始信用点”，代码里却是 −50。它只是比其他职业扣得少，“增加”是相对说法。
- 9 个可选 + 4 个开发中 + 1 个隐藏实现（junker）。enum 顺序是 `WATER_MERCHANT, APPRENTICE, SCAV, FENCE, MOONSHINER, DRUG_DEALER, ORGAN_TRADER, PHARMACIST, STREET_FOOD_VENDOR, FARMER, GUNSMITH, ROCK_BOTTOM, RANCHER, MODDED, AQUAFARMER`，AQUAFARMER 排在 MODDED 后面，说明它是后来补进去的（标题、描述还用 `CreatePending` 内置了英文兜底文本）。

### 1.3 起始特性（Starting Perk）

规则（`NewGamePerksUI.cs:26-28, 226-256`）：
- **点数预算 3**（`MAX_PERK_POINT`）。正面特性 cost>0 会消耗点数，负面特性 cost<0 会返还点数，`UsedPoints() <= 3` 才能开局。
- **特性数量上限 4**（`BASE_PERK_COUNT`），“奇人异士”可以 +3 个槽位（`maxSlot=3`）。
- 有互斥列表 `incompatiblePerks`。
- `mindEditable=true` 的特性在局中可以用心智重编程器修改（见 §2.6）。

选得到的 48 个特性（`StartingPerkList.InitStartingPerk`；数值来自 `StartingPerkList` 常量 + `StartingPerk.GetDescriptionArguments` + `StartingPerkContent`）：

| id | 中文名 | 类型 | 点数 | 互斥 | 可重编程 | 实际效果（代码） |
|---|---|---|---|---|---|---|
| saving | 存款 | + | 1 | | | 现金 +150 |
| model_citizen | 模范公民 | + | 1 | convict | | 治安部 +30；举报悬赏 ×1.5 |
| well_connected | 八面玲珑 | + | 2 | public_enemy | | 所有派系 +15 |
| sycophant | 马屁精 | + | 1 | | | 上层区 +15；上层区声望获取 +25%；上层区顾客预算 +10% |
| alert | 警觉 | + | 2 | | ✓ | 夜间不会被抢 |
| sawyer_crew | 索耶的伙计 | + | 1 | | | 开局有地下交易所；清洁服务免费（店被破坏时为 day/5+30） |
| diplomatic | 舌灿莲花 | + | 2 | | ✓ | 交易获得的声望 +33% |
| intimidating | 凶神恶煞 | + | 2 | | ✓ | 犯罪留下的可用证据 −33% |
| secret_admirer | 神秘仰慕者 | + | 1 | | | 开店时随机收到礼物：第 2 天必送；之后概率为：距上次送礼≤3天时 3%，此后每天 +6%，第 12 天 100%（`GetSecretAdmirerChance`）。礼物表约 20 种，会优先补急需品：哨兵病没有针剂时送真免疫宁，有成瘾时送黑针，受伤时送绷带×2 |
| welfare_program | 低保户 | + | 1 | banned_commissary | | 开局解锁配给站/食品券；每周一按“修正资产 = 现金 + (总资产−现金)/3”发券：≤1200 → 160、≤1800 → 120、≤2400 → 80、≤3000 → 40、更高没有 |
| proficient_scavenger | 熟练的拾荒客 | + | 1 | maimed | ✓ | 拾荒次数 +2 |
| revolution_sympathizer | 心向革命军 | + | 1 | | | 革命军 +25；革命军委托更多 |
| tapehead | 磁带迷 | + | 1 | | | 开局有 7 盘完好磁带（不发 4 盘损坏磁带） |
| handy | 心灵手巧 | + | 1 | | | 破旧工具箱（解锁 4 格）+ 螺丝刀、剪线钳、撬棍、等离子切割机 |
| wild_favorite | 王尔德的红人 | + | 1 | wild_bad_side | | 网络升级 ×0.9；保险 −50% |
| lucky | 幸运儿 | + | 1 | unlucky | | luck +15 |
| tinkerer | 改装能手 | + | 1 | | | 升级物品时 20% 概率不消耗材料 |
| vending | 自动贩卖机 | + | 2 | | | 开局送自动售货机（相当于 1200 的 VENDING_MACHINE 升级） |
| short_sleeper | 短眠者 | + | 1 | | ✓ | 夜间外出后第二天不少顾客 |
| working_pipes | 管道畅通 | + | 2 | | | 开局解锁 GUTTERFLOW_TAP + RUSTWATER_TAP（共 600） |
| colorful_character | 奇人异士 | 中性 | 1 | | | 特性槽 +3 |
| makeshift_reactor | 简易反应堆 | 中性 | 1 | | | 送可升级的微型核反应堆；不受停电影响；**网络里偷接私电 I–III 和商用级供电全部隐藏**（`NetworkUpgrade.IsHiddenForRun`） |
| hoarder | 囤积癖 | 中性 | 0 | | | 物品栏底部放 3 堆清不掉的杂物（`DebrisHelper.PlaceHoarderPiles`），可以付费清除（DEBRIS_REMOVAL_SERVICE，50/晚） |
| aug_menace | 改造体危机 | 中性 | 1 | | | 开局解锁霰弹枪和改造体内容，第 2–4 天出现第一个改造体；击杀奖励 +25%；放跑太多直接游戏结束（不管什么难度） |
| careful_scavenger | 谨慎的拾荒客 | 中性 | 0 | maimed | ✓ | 拾荒次数 −2，但完全不会受伤 |
| prime_real_estate | 黄金地段 | 中性 | 0 | side_street | | 每天顾客 +1；房租 +75 |
| bad_lease | 高昂房租 | − | −1 | | | 房租 +40 |
| convict | 前科犯 | − | −1 | model_citizen | | 治安部 −30；举报悬赏 ×0.75 |
| public_enemy | 全民公敌 | − | −2 | well_connected | | 所有派系 −15 |
| side_street | 偏街陋巷 | − | −1 | prime_real_estate | | 每天顾客 −1 |
| nicotine/narcotic/alcohol_addiction | 尼古丁/麻醉品/酒精成瘾 | − | −1 | | ✓ | 不定期摄入就有 8% 交易惩罚；开局送一份对应物品；可以治愈 |
| gambling_addiction | 赌博成瘾 | − | −1 | | ✓ | 开局解锁彩票（正常要第 15 天），送赌徒硬币、刮刮乐各 1；**卖家把彩票/刮刮乐放上柜台时会被强制买下**（第 1 张只要买得起就买；第 2 张要现金≥2 倍价格且 50%；第 3 张≥5 倍且 40%；第 4 张≥10 倍且 30%；第 5 张≥25 倍且 20%） |
| blackmail | 敲诈勒索 | − | −2 | | | 每周付封口费，不付就在多数派系大幅掉声望 |
| blabbermouth | 大嘴巴 | − | −2 | | ✓ | 可用证据 +33%（文案写 {0}%，参数复用了 intimidating 的 33）；黑市/革命军声望获取 −50% |
| minimalist | 极简主义者 | − | −2 | ascetic | ✓ | 主物品栏 24×10 → 20×10 |
| ascetic | 苦修者 | − | −4 | minimalist | ✓ | 主物品栏 → 16×10 |
| sentinel | 哨兵崩溃综合征 | − | −3 | | | 每周要打一针免疫宁，不打就死（`HealthData.sentinelPool>=20` 时游戏结束）；开局送 1 针 + 注射器手册 |
| agoraphobia | 广场恐惧症 | − | −1 | speedy | ✓ | 每次夜间外出后要在店里休息 2 晚 |
| maimed | 肢体残缺 | − | −2 | proficient/careful_scavenger, osteoarthritis | | 垃圾场不可用（`DumpingGroundLocation.IsDiscovered`） |
| wild_bad_side | 王尔德的眼中钉 | − | −1 | wild_favorite | | 网络升级 ×1.1；保险 +50% |
| unlucky | 厄运缠身 | − | −1 | lucky | | luck −15 |
| awful_negotiator | 不善议价 | − | −3 | | ✓ | 不能议价；永久 5% 交易惩罚 |
| banned_commissary | 被配给站拒之门外 | − | −1 | welfare_program | | 配给站和食品券永远不解锁 |
| illiterate | 文盲 | − | −1 | | ✓ | 看不了任何指南 |
| degenerative_osteoarthritis | 退行性骨关节炎 | − | −1 | maimed | | 拾荒上限每周 −1（`5 − (day−1)/7`），低于 3 就不能拾荒；送绷带×2 |
| known_snitch | 人尽皆知的告密者 | − | −2 | | | 通缉犯永远不来；革命军/黑市 −10 |

**代码里有、但选不到的特性（考古价值）**：
- `loan`、`debt`：只有常量和 `IsLoan()/IsDebt()`，**调用次数为 0**；`UnusedDirectory` 里还有一个 `loan_contract`（“与高利贷签下的合同”）。可以推测曾设计过“借高利贷开局”。
- `speedy`（行动敏捷，“每晚最多去 2 个夜间地点”）：有中文文案，有互斥关系（agoraphobia 互斥 speedy），`IsSpeedy()` 调用 0 次。现在每晚外出次数写死为 1（`StoreShutterButton.cs:217 visitLeftTonight = 1`）。
- `unsubscribed`（取消订阅，没有报纸）：逻辑还在（`StoreEventManager.cs:572`），但不在特性列表里。
- `expedition_organizer`（探险组织者）：远征系统唯一的解锁入口，不在特性列表里，见 §5。
- `IsExperiencedChemist()` 写死返回 true，原来应该是一个特性，后来改成所有人默认拥有。

---

## 2. 王尔德人脉网络（NetworkUpgrade）

### 2.1 机制

- **数据结构**（`NetworkUpgrade.cs`）：`cost`、`prerequisite`（前置列表，要求全部解锁）、`state`（0 未开放 / 1 可买 / 2 已解锁）、`isRepeatable`、`cooldownDuration/cooldownCurrent`（按天递减）、`heldUntilUsed`、`tab`（"" = 基础，"Advanced" = 高级）、`subtitle`（分组卡片）、`recommended`（置顶“推荐”卡）、`hidden`、`lockInDemo`。
- **价格**（`GetCost`）：`cost × 难度倍率 × (LLPILLAR 下层区传奇 0.85) × (王尔德红人 0.9) × (王尔德眼中钉 1.1)`。
- **支付**（`WildUIManager.OnUnlockClicked` / `WildNetworkUI.cs:566-583`）：**先扣“王尔德的恩惠”，不够的部分再扣现金**，买不买得起按 `现金 + 恩惠 >= 价格` 判断。
- **恩惠来源**：

| 来源 | 代码 | 数值 |
|---|---|---|
| 第 7 天王尔德引入网络 | `StoreClientListStory.cs:377` | `networkIntroFavor` 300/200/100/0 |
| 每笔交易 | `StoreReputation.OnItemTraded:221` | 原值 × `tradeFavorRate`（10%/5%/0/0） |
| 下层区声望每涨 1 点 | `StoreReputation.ModReputation:428` | **+40 恩惠** |
| 王尔德系夜间服务（保险、买家推荐、卖家推荐、善后人、罪犯人脉） | `PlayerStore.cs:1709-1716` | 服务费 × 75/50/25/15% 返为恩惠 |
| 王尔德救济（财富趋势下滑且王尔德电话已开通） | `StoreClientManager.HandleWildBailout` | 一次性 +250（只有 cozy/normal，且一贫如洗开局没有） |

- **获取渠道**：店里的电话拨 **8376**（王尔德助理）可以随时打开网络（`PhoneDialogList.PhoneWild`）；去杰克逊处时也能看。连续 6 天没打开网络，会来一个“升级提醒”NPC（`StoreClientManager.cs:1671-1685`，从第 5 天起计数）。
- **布局算法**（`NetworkUpgradeLayout.Build`）：先分页（tab），再按 subtitle 分卡片；卡片内按前置关系串成链（一行最多 3 格），所有 `recommended` 的升级再复制一份到页面最上方的“推荐”卡（最多 10 列）。这是一个**数据驱动、不需要美术摆放**的升级树，模组用 `RegisterModUpgrade / RegisterTabLabel` 就能加升级和新页。
- **“高级”页解锁**：`IsHiddenForRun` 规定 tab=="Advanced" 时，必须 `IsPropertyPaid`（已买下店铺）才显示。

### 2.2 基础页完整升级表（54 个定义 = 49 个可见 + 1 个隐藏 + 4 个高级）

价格为普通难度原价。“→”表示前置。🔒 = 试玩版锁定，⭐ = 推荐卡。

**① 引荐（Introduction）——供货商与股份**

| id | 名称 | 价格 | 前置 | 效果 |
|---|---|---|---|---|
| BREWER ⭐ | 红魔鬼酿酒厂 | 200 | — | 获得供货商名片；电话 8815 开通（啤酒 8 份次日到 / 累计消费≥400 后可订 16 份 2 日到；冷却 4 天） |
| CHEMIST 🔒 | 下层区化学家 | 250 | — | 电话 3319 Ashish（梦尘 8/16，消费≥400 解锁大单，冷却 4 天） |
| PHARMA 🔒 | 上层区药剂师 | 300 | — | 电话 4615 Dale（奥克西莫注射器 5/9，消费≥500，冷却 7 天） |
| MINER | GP 采矿合作社 | 250 | — | 电话 56371 Caulson（矿石 6/12，消费≥350，冷却 5 天） |
| BREWER_SHAREHOLDER | 小股东：红魔鬼 | 1,450 | → BREWER | 批发折扣 +15%；**每周四**柜台收到 1 箱啤酒分红 |
| MINER_SHAREHOLDER | 小股东：GP 采矿 | 900 | → MINER | 折扣 +15%；**每周五** 6 份矿石 |
| CHEMIST_SHAREHOLDER 🔒 | 小股东：化学家 | 1,440 | → CHEMIST | 折扣 +15%；**每周六** 8 份街头级梦尘 |
| PHARMA_SHAREHOLDER 🔒 | 小股东：药剂师 | 2,500 | → PHARMA | 折扣 +15%；**每周日** 5 支奥克西莫 |

**② 恩惠（Favor）——可重复购买的保险**

| id | 名称 | 价格 | 前置 | 效果 |
|---|---|---|---|---|
| LANDLORD_GRACE | 租金宽限 | 100（每用一次价格 ×2） | — | 欠租一次可以并到下周交；可重复，冷却 15 天，`heldUntilUsed`；cozy/normal 开局就有 |
| SEC_GRACE | 豁免权 | 250（每用一次 +100） | — | 盘查发现会导致坐牢的违禁品时，只给警告；可重复 |

**③ 线人（Informant）——治安对抗**

| id | 名称 | 价格 | 前置 | 效果 |
|---|---|---|---|---|
| INSPECTION_INFORMANT | 盘查预警服务 | 300 | — | 解锁夜间服务 SEC_INFORMANT（10/晚）：提前通知常规盘查 |
| WILD_FIXER | 王尔德的善后人 | 350 | — | 解锁夜间服务 WILD_FIXER：每晚嫌疑值 −15% 再 −50；夜费 25 |
| WILD_FIXER2 | 善后人 II | 450 | → WILD_FIXER | −20% / −150；夜费 45 |
| WILD_FIXER3 | 善后人 III | 550 | → WILD_FIXER2 | −25% / −400；夜费 70（`NetworkUpgradeList.WILD_FIXER_*`，`SecData.OnFixerUsed`） |
| EVIDENCE_REMOVAL | 清除证据 | 500（每用一次 +100） | — | 证据等级和调查状态直接清零；冷却 14 天 |

**④ 客源（Referral）——顾客流量与新 NPC**

| id | 名称 | 价格 | 前置 | 效果 |
|---|---|---|---|---|
| BUY_REFERAL ⭐ | 买家推荐服务 | 200 | — | 夜间服务（10/晚）：推荐买家；展示柜吸引力提高 |
| SELL_REFERAL | 卖家推荐服务 | 200 | — | 夜间服务（10/晚）：推荐卖家 |
| CRIMINEL_NETWORK 🔒 | 罪犯人脉 | 350 | — | 夜间服务（45/晚，冷却 3 天）：带来有钱的违禁品买家，并给一点黑市声望 |
| RETIRED_CHEMIST 🔒 | 退休化学家 | 1,000 | — | 一整套破旧化学设备 + 手册；退休化学家**每周二**来，另外解锁卡特尔系的“可疑农夫”（周三/周六）和“毒贩”（周二/周五） |
| RETIRED_RANCHER | 养殖 | 750 | — | 鼠笼 + 6 只鼠 + 剁肉刀等；养鼠人**每周五**来 |
| WINEMAKER ⭐ | 退休酿酒师 | 200 | — | 酵母×3、酒瓶×3、浆果×2、放大镜、酿酒书；**每周五**来 |
| RUINED_MACHINE_UNLOCK 🔒（**hidden**） | 废弃机器拆解 | 300 | — | 废料客 + 机器拆解 NPC；**UI 里不显示**，只有“开发中”的 junker 开局能拿到，正式版拿不到 |

**⑤ 服务（Service）——电力与委托**

| id | 名称 | 价格 | 前置 | 效果 |
|---|---|---|---|---|
| POWER_HIJACK | 偷接私电 | 300 | — | 每晚电费（基础 5）变 0；不受停电影响；电流转换器费用 −15% |
| POWER_HIJACK2 | 偷接私电 II | 600 | → I | 转换器费用 −25% |
| POWER_HIJACK3 | 偷接私电 III | 1,000 | → II | 转换器费用 −50% |
| COMMERCIAL_POWER | 商用级供电单元 | 350 | — | 充能器/能量阵列充电速度 +100% |
| MARKETPLACE_REQUESTS 🔒 | 市场委托 | 250 | — | 打开店内 PC 的委托系统，委托报酬高于市价；第一单固定是乔治奥要厕纸 |

（选了“简易反应堆”特性时，本组的 4 个电力升级全部隐藏。）

**⑥ 店铺（Store）——硬件扩容**

| id | 名称 | 价格 | 前置 | 效果 |
|---|---|---|---|---|
| SHOWCASE_I ⭐ | 展示柜扩建 I | 200 | — | 展示柜 7×5 → 8×6；每天展示柜买家 +1 |
| SHOWCASE_II 🔒 | 展示柜扩建 II | 500 | → I | 9×7；再 +1 买家 |
| DOSSIER_UPGRADE | 档案箱升级 | 300 | — | 档案箱格子 → 17×10 |
| TRASHCAN_UPGRADE | 垃圾桶升级 | 300 | — | 垃圾桶格子 → 17×10 |
| VENDING_MACHINE | 自动售货机 | 1,200 | — | 夜间自动卖货 |
| VENDING_FOUNTAIN | 自动售水机 | 1,200 | — | 夜间自动卖水 |
| RENOVATION1 | 店铺翻新 I | 500 | — | 基础吸引力 +150 |
| RENOVATION2 | 店铺翻新 II | 750 | → I | +150 |
| RENOVATION3 🔒 | 店铺翻新 III | 1,000 | → II | +150（吸引力≥750/900 时顾客预算还有额外加成，见 `StoreClient.cs:1043+`） |

**⑦ 生产线专线（subtitle 直接借用开局职业名）**

| 分组 | id | 名称 | 价格 | 前置 | 效果 |
|---|---|---|---|---|---|
| 水商 | WATER_TRADER ⭐ | 水资源交易 | 300 | — | 一套水贸易装备；水贩/买水 NPC 来访；退休水商**每周三**来 |
| 水商 | GUTTERFLOW_TAP | 沟渠水直供 | 250 | → WATER_TRADER | 水龙头无限出沟渠水，纯度每天变化 |
| 水商 | RUSTWATER_TAP | 铁锈水直供 | 350 | → GUTTERFLOW | 无限铁锈水 |
| 水培农场主 | RETIRED_FARMER 🔒 | 退休农夫 | 350 | — | 入门水培设备、种子；**每周五**来 |
| 水培农场主 | PLANT_DNA | 植物基因 | 1,250 | → RETIRED_FARMER | DNA 编辑器 + 基因扫描仪 + 4 张空白 DNA 盘；之后杰克逊常备 DNA 设备（读档时会把旧存档里没买的价格强制改成 1250，说明调过价） |
| 枪械匠 | RETIRED_GUNSMITH 🔒 | 退休枪匠 | 650 | — | 改装套件 + 破旧枪械打印机（带 100 能量）；**每周四**来 |
| 枪械匠 | GUN_PERMIT_I 🔒 | 武器许可证 I | 300 | — | 治安部声望≥20 时发证、<0 吊销；可以合法卖低级枪；可以保留霰弹枪；送枪械招牌 |
| 枪械匠 | GUN_PERMIT_II 🔒 | 武器许可证 II | 600 | → I | 要求治安部≥50 |
| 枪械匠 | GUN_PERMIT_III 🔒 | 武器许可证 III | 900 | → II | 要求治安部≥80；送重型枪械招牌 |
| 水产养殖 | AQUACULTURE ⭐ | 水产养殖 | 400 | → WATER_TRADER | RAS + 水箱 + 罗非鱼卵 + 饲料；退休水产户**每周二**来 |
| 水产养殖 | AQUACULTURE_II | 水产养殖 II | 1,200 | → AQUACULTURE | 送一对幼年鳗鱼（单价 100）；之后来访时卖更大的鱼种 |

**⑧ 供货商（Supplier）——杰克逊博士**

| id | 名称 | 价格 | 前置 | 效果 |
|---|---|---|---|---|
| JACKSON1 | 杰克逊投资 I | 400 | — | `jacksonInvestment +10`：预算/库存永久提高，并且始终有模组可买 |
| JACKSON2 🔒 | 杰克逊投资 II | 650 | → I | +20 |
| JACKSON_SPECIAL_STOCK 🔒 | 周一特供 | 1,250 | → II | 原来周一杰克逊不营业，买了之后周一改为“特别供货”：稀有物品，价格 +150%（见 §3） |

**基础页 49 个可见升级合计 29,790 信用点**（不含重复购买的涨价）。

### 2.3 “高级”页（买下店铺后才出现）

| id | 名称 | 价格 | 前置 | 效果 |
|---|---|---|---|---|
| HYPERTECH_SMUGGLER ⭐🔒 | 超科技走私贩 | 2,500 | — | 解锁夜间服务（500/次，冷却 5 天），超科技走私贩来访。货单：超科技电子件、超科技能量块、超科技涡轮、**超科技储藏区**、**心智重编程器**、未封顶神经核（后三样每次来最多各卖 1 件，`StoreClientListSupplier.cs:10-12`） |
| ENDORSEMENT1 | 王尔德的背书 I | 2,500 | — | 所有顾客预算 +25% |
| ENDORSEMENT2 | 背书 II | 5,000 | → I | 再 +25%（合计 +50%） |
| ENDORSEMENT3 | 背书 III | 10,000 | → II | 再 +25%（合计 +75%；`StoreClient.cs:1039`，`tier × 25%`） |

高级页合计 20,000。背书是纯数值的“收入乘子”，是买下店铺后无尽玩法的主要资金去处。

### 2.4 升级树的结构特征

- **很浅、很宽**：前置链最长 3 级（WILD_FIXER、POWER_HIJACK、GUN_PERMIT、RENOVATION、JACKSON、ENDORSEMENT），没有跨分组的复杂依赖，跨组前置只有 WATER_TRADER → AQUACULTURE。
- **三类升级**：
  1. **内容解锁型**（引荐、退休 X、水/鱼/植物基因）：打开新的商品线和新的周常 NPC，是横向扩展；
  2. **数值型**（展示柜、翻新、背书、股东折扣、杰克逊投资、电力）：纵向加成；
  3. **风险对冲型**（恩惠、线人、清除证据、善后人）：给违法玩法兜底，可以重复购买或有冷却，属于持续的资源消耗。
- **订阅制服务**：好几个升级买下来只是“开通”，之后每晚还要交服务费（推荐 10、罪犯人脉 45、善后人 25/45/70、盘查预警 10、走私贩 500），夜间结算界面可以逐个开关（`tuto_nightly_service`）。服务费的一部分又返还为恩惠，形成“花钱 → 恩惠 → 下一个升级”的循环。
- **残留**：本地化里有 `network_subtitle_Permit`（许可），但代码里枪证用的 subtitle 是 "Gunsmith"，说明枪证原来单独成组，后来并进了“枪械匠”卡片。

### 2.5 派系声望 Perk（`StoreReputationPerk.cs`，`RepPerkTable`）——与网络并行的第二条成长轴

声望从 0 开始，正向阈值 +20/40/60/80/100 解锁增益，负向 −20…−100 触发惩罚。下层区声望的增长同时会换成王尔德恩惠（每点 40），所以下层区声望和网络升级是绑在一起的。

| 派系 | 正向 Perk（阈值） | 负向 Perk（阈值） |
|---|---|---|
| 下层区 LL | 有口皆碑 20、稀有货物 40、市井传言 60、守口如瓶 80（证据 50%）、**下层区传奇 100（王尔德/杰克逊 −15%）** | 差评 −20、遭到猜疑 −40、沦为猎物 −60（保险 ×2）、狗嫌人厌 −80（供货商拒接电话、杰克逊夜间地点消失）、暴民私刑 −100 |
| 治安部 SEC | 征用令 0、富余物资 20、庇护 40、值得信赖 60、可靠供应商 80（+50% 预算）、VIP 100（不再被盘查） | 重点盯防 −20（调查 ×1.2）、报警不理 −40、头号通缉 −60（×1.5）、彻底查抄 −80、毫不留情/不择手段 −100 |
| 黑市 BM | 盗亦有道 0、小牌掮客 20、销赃信誉 40、地下网络 60 | 黑名单 −40、无法参保 −60 |
| 上层区 UL | 品味不凡 0、小费 20（+15%）、施舍 40、打听八卦 60、家喻户晓 80、好评如潮 100（+25% 预算） | 旅游陷阱 −40、联名检举 −60、雇佣杀手 −100 |
| 革命军 REV | 市场委托 20、军需官 40、革命军之友 60、突袭所得 80 | 黑名单 −40、暗杀名单 −100 |
| 卡特尔（只有化学家开局） | 增加津贴 25、可靠化学家 50、金盆洗手 100 | 强制退休 −100 |

### 2.6 心智重编程器——局中改起始特性（`MindReprogrammerHelper.cs`）

- 来源：超科技走私贩（高级页，买下店铺后才有）。
- 充能：把**超科技电子件**（单价 2500）用在它上面，每件 +1 充能（`MaterialDirectory.cs:358-366`）。
- 规则：只能操作 `mindEditable` 的特性；**负面特性只能移除，不能添加**；正面或中性特性可以添加，但要检查互斥；消耗 `|cost|` 点充能；之后冷却 `|cost| × 5` 天。
- 设计含义：终局时可以“治好”开局为了换点数而选的负面特性，例如苦修者要 4 充能、冷却 20 天，相当于 4 × 2500 的超科技电子件。这把开局的取舍变成了终局可以回收的“债”。

---

## 3. 夜间活动（Afterhour）系统

### 3.1 流程（`MapUIManager.cs`, `StoreDoor.cs`, `TransitUI.cs`, `EmporiumEntry.OnLeaveStore/OnArriveStore`）

1. 关店后点店门 → 选背包（`response_afterhours_title`，可以不带）→ 交通卡片 UI（TransitUI）里选地点。
2. 出门条件（`StoreDoor.cs:50`）：没有重伤、“严打 crackdown”事件没在进行、广场恐惧症不在休息期、已在第 2 天由导师引入（`isAfterhoursIntroduced`）、店已关、`visitLeftTonight > 0`。**每晚只能出门 1 次**（`StoreShutterButton.cs:217`）。
3. **代价**：出过门的第二天，顾客数 −1（`StoreClientManager.cs:1422`；夜间日志 “你今天开店迟到了，错过了一名顾客”），“短眠者”免除这个惩罚。
4. 地面上没带走的物品会永久丢失；在杰克逊处例外，他会派助手把东西送回店里。

### 3.2 地点（`AfterhourLocation.EnsureVanilla`）

| id | 地点 | 何时出现 | 玩法 |
|---|---|---|---|
| dumping_ground | 垃圾倾倒场 | 一开始就有（选了“肢体残缺”则没有） | 拾荒：基础次数 5（熟练 +2、谨慎 −2、骨关节炎每周 −1、事件修正）；每次搜索后，轻伤概率 = (5% + 已搜次数×1%)，重伤概率 = (1% + 次数×0.25%)；带手电概率减半；乘以事件修正 `(1+injury_chance/100)`；前 10/20 次有新手免伤保护；运气可以抵消受伤。掉落表 `dumpingGroundTG`（稀有 0.5、中 1、垃圾 2、废料 5 的权重）；带螺丝刀/剪线钳/等离子切割机时各有 10% 额外掉落，3% 捡到背包。这里还有 Serpico 剧情线、屠夫的跟踪（`response_afterhours_need_gun`：不带枪会被杀） |
| commissary | 配给站 | 第 11 天导师引入食品券后（低保户开局就有；被拒之门外则没有） | 用食品券兑换的“配给站票券”买固定价格的物资，见 §4.1 |
| dr_jackson | 杰克逊博士 | 第 6 天杰克逊引入后（下层区 −80“狗嫌人厌”时消失） | 发明家商店：卖机器、模组、DNA 设备；可以当场把物品卖给他 |
| （模组） | `AfterhourLocation.Register` / `DelegateAfterhourLocation` | — | 留了完整的模组接口 |

### 3.3 地点事件（`VanillaAfterhourEvents.RegisterAll`）

事件框架：`AfterhourEventDefinition`（固定星期 `Every(...)` 或每日概率 `dailyChancePercent`；持续天数；`closesLocation`；`canStart` 条件；效果列表 `AfterhourEffect(modifierKey, value, polarity)`）。效果的正负决定卡片色调（好 / 坏 / 混合）。每天开始时（`AfterhourEvents.OnNewDay`）先尝试排期事件，**当天如果已有排期事件触发，就不再掷概率事件**。

| 事件 id | 地点 | 触发 | 时长 | 效果（modifier） | 色调 |
|---|---|---|---|---|---|
| fresh_dump 新倒的垃圾 | 垃圾场 | 每周三 | 1 天 | junk_chance −50%（垃圾更少）；injury_chance +25%（更容易受伤） | 混合 |
| georgio_assistance 乔治奥的援助 | 垃圾场 | 0% 概率，只能由 NPC 乔治奥触发：付 **50** 信用点 | **3 晚** | injury_immunity（不受伤）；scav_attempts +5 | 好 |
| jackson_closed 未营业 | 杰克逊 | 每周一（没有买周一特供时） | 1 天 | 地点关闭 | 坏 |
| special_stock 特别供货 | 杰克逊 | 每周一（买了 JACKSON_SPECIAL_STOCK 后） | 1 天 | 卖稀有物品；inventor_sell_price +150% | 混合 |
| jackson_discount 折扣 | 杰克逊 | 每周日 | 1 天 | inventor_sell_price −10% | 好 |
| commissary_burglary 入室盗窃 | 配给站 | 小偷 NPC 偷走配给站物资时（`StoreClientList.RemoveCommissaryStock`） | 1 天 | 库存减少 | 坏 |
| fresh_restock 新货上架 | 配给站 | 每周五 | 1 天 | 每种商品补库存上限的 35% | 好 |

设计观察：夜间活动用的是**周历驱动 + 少量随机**的结构，把“每周几去哪里最划算”变成可以学习的知识（周三垃圾场好东西多但更危险，周日杰克逊打折，周五配给站补货）。事件系统是后来重构出来的通用框架，有 ModHook、`AfterhourDebug.cs` 和自定义面板 `CustomUIAfterhourEventsPanel`，目前原版内容只有 7 个事件。

---

## 4. 其他外围玩法

### 4.1 配给站 & 食品券（`CommissaryData.cs`）

- 食品券面值 40/80/120/160/200/240，160 及以上带防伪标；有伪造券（logo、字体、芯片），需要靠《食品券真伪鉴定手册》辨别，和主玩法“鉴定真伪”是同一套机制。
- 来源：顾客拿来卖（市场上常低于面值折现）、低保户特性每周一发放。
- 兑换：在配给站把有效食品券换成票券 `redeemedCredit`；中奖彩票也可以在这里**兑成现金**（不是票券）。
- 货架（固定价格，单位为票券）：

| 物品 | 价格 | 每日补货 | 上限 | 类型 |
|---|---|---|---|---|
| 加工肉 processed_meat | 25 | 2 | 12 | FOOD |
| 营养果 nutrifruit | 12 | 4 | 20 | FOOD |
| 银河混合咖啡 galaxy_blend | 25 | 2 | 10 | FOOD |
| 加工果汁 | 14 | 2 | 12 | FOOD |
| 加工牛奶 | 16 | 2 | 12 | FOOD |
| 水配给 water_ration | 30 | 2 | 8 | WATER |
| 绷带 bandage_item | 8 | 2 | 8 | MEDICAL |

- **补货受市场事件影响**（`GetStockMult`）：同类商品的市场价格修正总和 >0（短缺）时补货倍率为 −2/−3/−5（修正 <33/<66/≥66），也就是会扣库存；<0（过剩）时为 ×2/×3/×4。设计意图是“价格固定，但供应随市场波动”：短缺时配给站也缺货，正好是用票券囤货、转手卖高价的套利窗口。
- 周五“新货上架”另外补上限的 35%。小偷事件会扣配给站库存并触发“入室盗窃”。

### 4.2 彩票（`LotteryHelper.cs`）

- 解锁：**第 15 天起**（`dayCounter > 14`，赌博成瘾开局就解锁）。
- 彩票代表 NPC **每周四**来店（`StoreClientManager.cs:865`），卖彩票（25）和刮刮乐（20，一叠 scratcher_stack 100）。
- 开奖：**周五夜结算（周六早上）**（`HealthData.cs:331`，`day%7==5`）；从 1–49 中抽 6 个不重复号码，开奖号码登在报纸上，期号从 1405 开始。
- 奖金：中 6 → 1,000,000；中 5 → 100,000；中 4 → 2,500；中 3 → 300；**中 2 → 35**。
  - ⚠ 文本不一致：说明文字 `lottery_intro / event_lottery_draw_desc` 写的是“至少 3 位相符才能中奖”，代码对 2 个号码相符也发 35，并且有 `lottery_match2` 文案。说明 2 等奖是后来加的，说明文字没有同步更新。
- 期望值（按组合数计算）：每张 ≈ **14.27 / 25 ≈ 57% 返还率**（中 2 的概率 13.2%，中 3 为 1.77%，中 4 为 0.097%）。
- **运气介入**：有效运气 = max(玩家运气, 购票时记录的运气)；额外重抽次数 = |luck|/100，再加上 (|luck| % 100)% 的概率多抽一次。运气为正时，在几组号码里取总奖金更高的一组（为负时取更低的）。例如运气 +15，有 15% 概率多抽一次、取更好的结果。

### 4.3 刮刮乐（`ScratcherHelper.cs`）

- 3 个格子，5 种图案（1 = 营养果，是大奖图案）。生成方式是“倾向重复”：每格有 20%×(1+运气/100) 的概率复制“偏好图案”（第 1 格偏好营养果，后面的格子偏好前一格），否则随机出其他图案。
- 奖金：任意 2 个相同 10、任意 3 个相同 75、2 个营养果 30、3 个营养果 350（触发 `CheckLucky` 成就）。
- 我模拟 40 万次的期望值：运气 0 → **11.8 / 20（59%）**；+15 → 14.7（74%）；+35 → 18.9（94%）；−15 → 9.5。也就是说，**就算叠满运气也不能长期赚钱**，这是有意压住的。
- 赌徒硬币：累计开始刮 10 张后奖励一枚（赌博成瘾开局直接送）；带在身上时运气 +5，刮擦半径 ×1.33，并且显示“平均每张赢多少”的统计。另有“液态幸运”药剂会叠加运气。
- 设计定位：一个**用运气属性做可视化反馈的小赌局**（刮的时候会提示“运气帮了忙/倒霉了”），和“幸运儿/厄运缠身/赌博成瘾”这几个特性配合，给特性提供能被看见的效果。

### 4.4 报纸（`NewsPaper.cs`, `NewsUIManager.cs`, `NewsUIElement.cs`）

- 第 8 天起（`PlayerStore.OnShutterOpened:1291`）每天早上开门时报纸送到柜台；“取消订阅”特性（已移除）会停送，设置里可以选择直接丢进垃圾桶。
- 内容：当前所有**已知**的市场事件 `StoreEvent`（过滤掉 `isHiddenFromNewsPaper`），按事件数量选 1/2/3/4 栏版式；没有事件时自动生成“一切平静”的装饰性事件。
- 每条显示标题和正文；非永久事件会标红显示“今天结束”或“N 天后结束”（倒计时只在 `eventEndCountdown` 开启时显示，hard 和 challenge 难度关闭）。
- `NewsUIElement` 继承 `NegociationElement`，报纸上的事件可以当成议价工具（拿新闻来压价或抬价）。
- 本质上是**市场信息的 UI 载体**；派系的“打听八卦 / 地下网络 / 革命军之友”等 Perk 能更早得到事件预告，这是信息优势型的成长。

### 4.5 电话（`PhoneUIManager.cs`, `PhoneBookUIManager.cs`, `StorePhoneClient.cs`）

- 真的要按数字键拨号（支持小键盘），停顿 2.25 秒后自动拨出，等 1–5 秒接通。号码不存在时提示无人接听；“打过一次”的号码才会出现在电话簿里，之后可以快速拨号。
- 号码表：

| 号码 | 联系人 | 类型 | 冷却 | 用途 |
|---|---|---|---|---|
| 8376 | 王尔德的助理 | — | — | 打开人脉网络 |
| 8815 | 红魔鬼代表 | 供货商 | 4 天 | 啤酒 8（次日到）/ 16（2 天到，累计消费≥400 解锁） |
| 3319 | Ashish（化学家） | 供货商 | 4 | 梦尘 8/16（≥400） |
| 56371 | Caulson（采矿） | 供货商 | 5 | 矿石 6/12（≥350） |
| 4615 | Dale（药剂师） | 供货商 | 7 | 奥克西莫 5/9（≥500） |
| 51189 | Odin Haraldsson（革命军商人） | — | 5 | 一开始不接电话；剧情（通缉犯 wanted4）后开通，可以约他上门 |

- 首次通过引荐打通时，第二天会免费送一单（courrier*Free）。下层区 −80“狗嫌人厌”时**所有供货商拒接电话**，声望惩罚直接切断供应链。
- 代码里还有治安部接线员、配给站自动电话、测试号码的工厂方法，但没有放进号码表，属于废弃内容。

### 4.6 雇员（Hireling）与远征（Expedition）——见 §5.3，结论：**是半成品**

### 4.7 每周日历（汇总代码里所有 `day%7` 判断）

| 星期 | 发生的事 |
|---|---|
| 周一 (1) | 低保户发食品券；杰克逊不营业 / 周一特供 |
| 周二 (2) | 退休化学家、退休水产户来；卡特尔毒贩（还有周五） |
| 周三 (3) | 垃圾场“新倒的垃圾”；退休废料客、退休水商来；可疑农夫（还有周六）；第 50 天后 FlexiBuyer |
| 周四 (4) | 彩票代表来；退休枪匠来；红魔鬼股东分红 |
| 周五 (5) | 配给站补货；退休农夫、酿酒师、养鼠人来；采矿股东分红；彩票当晚开奖 |
| 周六 (6) | 彩票号码见报；化学家股东分红 |
| 周日 (0) | 杰克逊打折；药剂师股东分红 |
| 每 7 天 | 交房租，房租按难度倍率上涨 |

---

## 5. 考古：从 Deepspace Emporium 到 Probably Stolen

### 5.1 判断方法

1. 代码层：找调用入口（`grep -lw 类名`），看引用是否只在旧系统内部互相调用；
2. 数据层：物品或目录是否注册（`Directory<T>` 是挂在场景上的 MonoBehaviour，通过 `InitDirectory()` 注册），有没有本地化 key；
3. 流程层：正式版主菜单 `MainMenuUIController.StartNewGameClick` 直接 `LoadScene("InventoryScene")`；`EscapeUIManager` 返回 `"EmporiumMenu"`。旧版入口是 `GameMaster.Start → MenuScene.StartMenu → MenuManager.GetAvailableFactionScenarios → GameMaster.NewGame → shipManager.StartShip`，在正式版代码里**没有任何地方调用它**。

> 场景命名本身就是证据：主菜单场景叫 **EmporiumMenu**，游戏场景叫 **InventoryScene**，核心单例叫 **EmporiumEntry**、设置叫 **EmporiumOption**、统计叫 **EmporiumAnalytics**，这些全是前作 Deepspace Emporium（太空背包管理/商人 roguelite）的命名。**EmporiumEntry/Option/Screen 并不是“多店铺扩张”**：Entry 是店铺场景的总入口，Option 是设置项（音量、分辨率、快捷操作等），Screen 是 PC 屏幕的发光控制。

### 5.2 遗留且已废弃（没有入口）

| 系统 | 文件 | 内容 | 判定依据 |
|---|---|---|---|
| **旧游戏主控** | `GameMaster.cs` | 持有 MenuScene/MenuManager/RaidScene/RaidManager/ShipScene/ShipManager/TurnManager | `InRaid()` **写死 `return false`**，所以 `InCombat()` 永远是 false。正式版的 `GameItem.MaySelectSlotItem/MayTarget` 每次使用物品都会调用 `GameMaster.current.InCombat()`，可见 GameMaster 单例在 InventoryScene 中存在（推测组件挂着，但 Start 里的旧场景流程没跑或被隐藏），并被改造成“永远不在战斗中”的开关。`NewGame()/ContinueGame()` 只被 `MenuScene` 调用 |
| **飞船选择开局** | `MenuManager.cs`, `Scenario.cs`, `MenuScene.cs` | 4 艘船：Sok-27C 货船（商人/走私）、A-101 Albatross 军用运输舰（战斗/客运）、Type-54 Bian Yuan 工程船（采矿/拾荒）、The Bus 难民船（困难），每艘 5000 点数；4 名默认船员 albert/benjamin/charlotte/daniel，带战术/技术/医疗等熟练度和技能 | `Scenario` 只被 MenuManager/MenuScene 使用；**`ShipItemDirectory.InitDirectory()` 是空的**，cargo_ship 等飞船物品根本没注册 |
| **太空航行/大地图** | `ShipManager.cs`, `ShipScene.cs`, `ShipHelper.cs`(44KB), `NavigationHelper.cs`, `NavDiskHelper.cs`, `PropulsionHelper.cs`, `OverworldStation.cs` | 跃迁、ETA、导航扫描、推进、船体修复、外部挂点；空间站分为中转站/军事/水培/医院/研究五类，有大中小尺寸，敌对概率 75% | 引用全部在 GameMaster/TurnManager 内部 |
| **太空事件** | 21 个 `Space*Trigger.cs` + 4 个 `TraderStation*Trigger.cs`/Test | 小行星采矿、强盗、两支舰队交战、“友善的屠夫商店”、货运车队、漂流货舱、碎片带、真假求救信号、制毒船、燃料运输船、幽灵船、医疗船、游商/黄牛、采矿船、共和国海军补给舰、Pastel 营养果无人机商船、难民船、维修船、打捞船、温泉船；贸易站：封锁、自由港、入港检查 | 由 `EventTriggerManager` 在 `TurnManager.NextTurn` 里检查，回合推进只来自旧流程；文本全是硬编码英文，没有本地化 |
| **回合制战斗/突袭** | `Combat*.cs`(14 个), `RaidManager/RaidScene`, `TurnManager.cs`, `CombatEndedUIController.cs`, `SceneSwitcher.cs` | 行动点 100/回合、技能（45 个标准技能、100+ 个效果）、AI、队伍、死亡概率、命中率；NPC：袭击者（技师/拾荒者/医疗兵/陆战队/嗜杀变体/头目）、逃兵、**异种 xeno_herald/hunter/brewer/drone**、海盗 | `CombatManager` 只被 Combat 系文件引用；`InRaid()==false` 封死了所有分支；`CombatEndedUIController` 用 UIToolkit 并按 `buildIndex−1` 跳场景，是早期多场景结构的残留 |
| **角色 Perk** | `PerkDirectory.cs` | 讨厌鬼、低调、健康/体弱、坚韧、血友病/易栓症、毒液适应、恐医、恐针、手巧、纵火狂……（共 20 个，都是 CombatAbilityEffect） | 只被 CombatAbilityEffect / GameCharacterItem 引用；正式版的“起始特性”是另一套（StartingPerk），设计思路相近（正负特性），但重写了 |
| **船员生存** | `Survival.cs`, `TutorialTrigger.cs` | 口渴值 1000、每回合 −15（在基地 −7）；旧教程讲导航系统、推进系统、船体、船员生存（口渴/饥饿） | `TutorialTrigger` 依赖 `worldTags` 和 OverlayHandler，没有入口；正式版教程是 `TutorialUIManager` + Tutorial/Hint 表 |
| **废弃物品** | `UnusedDirectory.cs`（123KB，59 个物品） | 战利品箱 S/M/L、技师背包、急救箱、各种药瓶（Bicarsole/Ocutol/Fixalin/Phagimycin/Oxycodone）、注射器/医疗/通用/补水挂包、模块化背包、**器官运输箱 + 心/肺/肾/肝**、**高利贷合同 loan_contract**、开锁工具、**异能燃料电池（FTL 跃迁用）**、钉子、垃圾堆（“我当拾荒客时的垃圾，每天能翻一次”）、扳手、钢板、光谱分析仪、燃烧瓶、**保险箱 safe**（“存放的物品不会被偷但会被查”）、大型储藏区、芦荟/生命草、古董手雷、燃烧手雷、治安部名片、**鉴定指南合集手册**、Lancewell 左轮×3、弹药箱、密码本、止血粉、测试水×3、泰坦 9/10 系列护甲 | **零引用**；物品没有本地化 key；IntelUnusedHelper 也只被它引用 |
| **废弃世界观情报** | `IntelUnusedHelper.cs` | 情报文件夹（带 NFT 真伪认证）：Pastel（“国家安全风险”、营养果）、神经教会（以 AI 为神的后“裂隙”宗教）、神经守望（极端派）、共和国（“裂隙”之后的共和国、共和国海军情报局 R.N.I.）；**开发者 Todo：“拜访发明家 / 弄个熔炉把废金属炼成锭 / 去超市”** | 零引用。“超市”在正式版里由“配给站”替代；“营养果”和“共和国食品券”这些世界观元素保留了下来 |
| **废弃市场事件** | `EventUnused.cs` | 食品船即将到达/已到达/延误/失踪/迟到、食品短缺、疫情爆发、谋杀率上升、连环杀手（出现/落网）、戒严/解除戒严、治安加强、治安预算削减、内乱、不满、抗议、政变、恐袭/恐怖组织被捣毁、营养果繁荣 | 零引用，大多数 identifier 为空。说明“空间站政治动荡”这一层叙事被砍掉或缩小了 |
| **废弃顾客** | `StoreClientListUnused.cs` | Billy 三部曲：乞讨食物 → 找到治安部工作来买手电和武器 → 拿“工作奖金”（赃物）来卖，另配一名治安官追查上层区医疗仓库失窃案；以及“革命宣言”NPC（房租每周涨、诊所缺药、巨型企业压榨……发传单） | 零引用；硬编码英文。设计价值：这是早期用来验证“顾客会成长、物品有来历、来源可追溯”的原型，后来被系统化成通缉犯/赃物/革命军声望 |

### 5.3 正式版时期写的、但没完成的内容（不是前作遗留）

| 系统 | 证据 | 判定 |
|---|---|---|
| **雇员/远征** `Hireling.cs`, `Expedition*.cs` | 使用 `EmporiumEntry`、`StoreClient`、`GraphUtils.EmporiumTryAdd` 等正式版 API，说明是新写的；但：① 唯一解锁入口是特性 `expedition_organizer`，**不在特性列表里**；② `ExpeditionData.CreateExpedition()` **没有任何调用**，远征根本发起不了；③ 只有“垃圾场”一个地点、3 个 POI（垃圾堆/新鲜废料/废料堆，成功率都是 85%）；④ `HirelingTrait`（斗士/懦夫/幸存者/嗜杀/屠夫/成瘾/孤僻/幸运/废料客/寻宝者/友善，共 11 个）**完全没用**；`ExpeditionPoiType.Combat/Thieving`、`HirelingState.Combat` 只声明了；⑤ 日志是硬编码英文；⑥ 唯一雇员 Juan（复用通缉犯 wanted7 的立绘，雇佣费 80） | **只做了一半就停了的“派人代你拾荒”系统**。在主角每晚只能出门一次的约束下，它本来能提供“并行产出”的成长，现在被冻结了 |
| **废料客开局 + 废弃机器拆解** | junker `comingSoon`；`RUINED_MACHINE_UNLOCK.hidden=true` | 内容链存在（退休废料客每周三来、机器拆解 NPC），但正式版拿不到 |
| **器官贩子/小吃摊/私酒/药剂师开局** | 文案齐全，`HandleInitialItem` 没有分支 | 计划中的专精；器官相关物品在 UnusedDirectory 里，私酒文案里提到的“酿造设备”现在部分由“退休酿酒师”（葡萄酒）承担 |

### 5.4 遗留并被“改造复用”的部分（正式版仍在用）

| 原用途 | 现在的用途 | 证据 |
|---|---|---|
| 飞船舱室模块 `ShipSystemDirectory` | **店铺储物家具**：储藏区、大型储藏区、扩容套件、机器舱、枪械/化学储藏区、迷你/标准/改装**走私者暗格**、超科技储藏区 | 物品带 `SYSTEM_TAG_UTILITY`，贴图来自 `Items/items_ship`，名称已本地化；销赃人开局送 smuggler_bay，学徒送 storage_bay |
| 网格背包/物品图（GameItem / GameGridInventory / PixelWindow / GraphUtils） | 店铺的柜台、展示柜、物品栏、档案箱、垃圾桶 | 整个 UI 和物品框架；展示柜升级就是 `SetShape(8,6)` |
| 飞船机械 / 电力（MachineryHelper / PowerHelper） | 店内机器（熔炉、水培、RAS、打印机、蒸发器、DNA 编辑器、售货机……）的功耗和性能 | `Machine*.cs` 调用 `InitMachinery / GetMachinePowerUsage` 等；带 `isFromShip` 参数的函数仍然留着 |
| 爆炸物目录 `ExplosiveItemDirectory` | `lou_bomb` 用在正式版剧情里（Lou 勒索） | 烟雾弹/闪光弹/C4 是战斗遗留 |
| 器官系统 `OrganHelper` | 正式版里有器官相关的鉴定/交易内容 | `OrganHelper` 引用 organ_heart 等 |
| “营养果”、“共和国”、“食品券” | 正式版的经济和世界观符号 | 刮刮乐大奖图案就是营养果 |

### 5.5 转型决策解读

1. **从“开着飞船的 roguelite 商人 + 回合制小队战斗”收缩为“固定地点的店主模拟”**：飞船、航行、太空事件、战斗、船员生存整体被封存（`InRaid=false` 一行代码就切断了全部战斗分支），而物品、网格、机器、电力这层“底层框架”全部保留。这是典型的“保留引擎、换掉玩法”：把前作最独特也最难做好的“背包俄罗斯方块 + 物品图”转化为“店铺柜台 + 鉴定”。
2. **“移动”被降维成“夜间外出”**：太空 POI 探索 → 3 个固定夜间地点 + 周历事件；太空商人/黄牛 → 杰克逊博士与周常 NPC；Pastel 无人机商船 → 配给站；“超市”Todo → 配给站。
3. **“战斗”被降维成“治安博弈 + 极少量射击”**：小队战斗没了，换成证据/嫌疑度/盘查/善后人/许可证/霰弹枪/改造体这一套“风险管理”玩法，王尔德网络里整整两组（线人、恩惠）都在服务这条线。
4. **叙事尺度缩小**：EventUnused 里的政变、恐袭、连环杀手、戒严这类“空间站大事件”被砍掉，正式版的事件以市场供需和派系动态为主，更贴近店主视角。
5. **还在扩张、但收不住的线**：远征/雇员、4 个“开发中”专精、隐藏的废料客线，说明制作组还想在固定地点的框架里补回“并行产出 / 多线经营”的成长，但优先级让给了水产、植物基因、售货机这类“新生产线”（`AQUACULTURE_IN_BUILD / PLANT_DNA_IN_BUILD / VENDING_IN_BUILD = true` 这几个编译开关说明它们是近期上线的功能）。

---

## 6. 设计点评：中长期成长与重玩性

### 6.1 优点

- **起始特性 = 用负面特性换点数的 roguelite 构筑**。3 点预算、4 槽，48 个特性里约一半是负面的，可以做出“苦修者(−4) + 王尔德红人 + 存款 + 自动贩卖机”这类高风险组合。互斥规则清楚，数值都能在描述里看到。再加上专精 × 难度，组合数很高。
- **专精决定第一条生产线，王尔德网络决定后续扩张**。专精其实就是“免费送 1–2 个网络升级 + 设备”，玩家之后可以用网络补上其他线（水 → 水产 → 鳗鱼；农夫 → 植物基因）。专精给的是起点，不封死后续路线。
- **恩惠这种二级货币把成长和行为绑在一起**：下层区声望每点 40 恩惠，王尔德服务费返还恩惠，交易抽成返恩惠。玩家“讨好下层区”或“用王尔德的服务”，本身就在给下一次升级打折。难度通过调这几个比例（而不只是调价格）来改变成长速度，手段比较细。
- **周常 NPC 是升级的回报节奏**：每个“退休 X”升级都会多一个固定星期来访的商人，配合股东分红、彩票、杰克逊的周一/周日事件，形成能背下来的周历。这让游戏中期有规律可循，减少“今天该干什么”的迷茫。
- **终局有明确的二阶段**：买下店铺 → 高级页（背书 +75% 预算、超科技走私贩）→ 心智重编程器回收开局的负面特性。从“活下去”切换到“越做越大”，再接上无尽模式。
- **高度模组化**：升级、页签、特性、场景、夜间地点、夜间事件、难度都有 Register 接口，UI 是数据驱动的自动布局，社区可以自己扩展内容。

### 6.2 问题与风险

- **升级树太浅**：最长 3 级，多数是单节点。49 个升级里约 40% 是“开一条线 + 送一套设备”，中后期很容易“买完就没有目标”，长线追求主要靠背书（10,000）这种纯数值的消耗。可以考虑加入互斥的分支选择（例如“善后人”和“线人”二选一的派系路线），或者让后期升级和派系声望阈值联动，制造更多取舍。
- **成长的取舍偏弱**：除了“简易反应堆”会隐藏电力分支外，没有“买了 A 就买不了 B”的设计。钱够了就能全买，重玩时差异主要来自开局，而不是中期的构筑。
- **外围小游戏的收益被刻意压低**：彩票 57%、刮刮乐 59%（运气 +35 也只有 94%）的返还率，说明它们主要是为特性和运气属性提供可见反馈的“风味系统”。这没问题，但赌博成瘾的“强制购买”在玩家现金不多时惩罚很重，赌徒硬币的统计又显示得太晚，需要注意。
- **每晚只能去一个地点，出门还要少一个顾客**，这个机会成本压得很紧，而且“行动敏捷”被移除、远征没完成，导致“夜间产出”这条成长轴到后期没有升级空间（拾荒次数只受特性影响，不受升级影响）。远征/雇员如果完成，正好能补上这块。
- **文本与代码不一致**：彩票 2 等奖、学徒“增加初始信用点”、大嘴巴的证据参数复用了凶神恶煞的 33%……说明数值迭代频繁，但文案没跟上，对依赖“读描述做构筑”的特性系统来说是信任风险。
- **专精之间的平衡**：养鼠人不扣钱还送 750 的升级，枪械匠扣 450 送 950 + 许可证，水产户扣 450 + 房租 +40、送的价值却只有 400 + 设备。“进阶开局”的门槛差别比较大。

### 6.3 一句话总结

《Probably Stolen》把前作 Deepspace Emporium 的“背包物品图 + 机器/电力”框架原样保留，把飞船、战斗、太空事件这一层整块封存。成长的骨架是“**开局构筑（专精 × 特性 × 难度）→ 中期横向开生产线（王尔德网络 + 派系 Perk + 周常 NPC）→ 终局买下店铺后的数值放大与特性回收**”。重玩性主要来自开局组合和派系路线；中期升级树的取舍不足，夜间轴缺少成长，是目前最明显的短板，而远征/雇员这条已经写了一半的线正好对应这块空缺。
