# 01 · 核心经济循环与日程结构（逆向拆解）

> 游戏：《Probably Stolen - Cyberpunk Shopkeeper Sim》 VERSION 100-Rev12（`PlayerStore.VERSION_NUMBER`）
> 资料：`~/ps_work/src/*.cs`（反编译）+ `~/ps_work/loc/{chinese,english}.json`
> 约定：`文件名:行号` 为来源；`day` 指 `StoreStation.dayCounter`；`day%7` 为星期（0=周日，1=周一…，见 `StoreCalendar.cs:GetDayOfWeek`）。
> 所有数值均为代码常量/公式的直接读取，"推断"处会单独标注。

---

## 0. 先排雷：哪些文件不是这个店的循环

| 文件 | 结论 |
|---|---|
| `GameMaster.cs` / `TurnManager.cs` | **前作/原型残留**。里面是 Ship/Raid/Combat 的回合制（`MINUTES_PER_TURN=20`、`MINUTES_PER_HOUR=100`、`HOURS_PER_DAY=10`、`START_YEAR=2501`），店铺循环完全不调用。店铺没有"时钟"，只有 `dayCounter`。 |
| `StoreDate.cs` | `SetDateToday` 从未被调用，残留。 |
| `AmenitiesManager.cs` | 只被 `GameItem.cs` 引用（卫生/营养等"设施值"上限 20/50/30/30/10/10），**不是**电费/保险/清洁费。真正的固定开支在 `StoreService.cs` + `StoreServiceSheet.cs`。 |
| `PriceHelper.cs` | 只有一些物品基础价常量（如瓶装水 5、能量饮料 18、生肉 159、医疗用品 40）和按重量计价 `weightGram*pricePerKilo/1000`，`InitDict()` 为空——**真正的定价公式在 `GameItem.cs` + `ItemFeatureList.cs`**。 |
| `PlayerStore.loanValue / dayUntilLoan / STARTING_LOAN=200`、`GetLoan()/PaidLoan()` | 空函数，被砍掉的"早期贷款"系统；起始特性 `loan`/`debt`（`StartingPerkList.cs`）也只有 ID，无效果、无本地化。 |
| `PlayerStore.dayUntilRent` | 每天 `--`、付租后重置为 7，但**没有任何逻辑读取它**；真正的交租日由 `GetRentDayCounter()=(7-day%7)%7` 决定。 |

---

## 1. 一天的流程

### 1.1 状态机
`PlayerStore.StoreState { CLOSED, OPEN, MORNING }`（`PlayerStore.cs:9`）

```
[MORNING] ──点卷帘门──▶ [OPEN] ──点卷帘门(队列可解散)──▶ [CLOSED] ──点床铺/地图"睡觉"──▶ 夜间结算 ──▶ [MORNING]
                                                            └─(可选) 出门 Afterhours 一次
```

### 1.2 早上（MORNING）
1. `EndOfDayUIManager.OnEndDayButtonClick` 黑屏序列中调用 `PlayerStore.BeginDay()`（`PlayerStore.cs:1220`）：
   - `StoreStation.StartDay()`（`StoreStation.cs:58`）：`dayCounter++`；事件管理器 `OnDayStart`（排期事件、每日 25~50% 概率新"普通事件"，最多 3 个并发；保持 ≥2 个"装饰事件"）；每 5 天 `wrongfulReportCount--`；当日两种水的纯度随机；派系 `influence += strength`；店铺订单（StoreRequest）`duration--`；各网络升级 `OnDayStart`。
   - 自动售货机误标签结算、客户管理/治安/夜间事件/配给站 `OnNewDay`。
2. 早上死亡检查（`EndOfDayUIManager.cs:267-274`）：简易反应堆爆炸 → `reactor`；改造体存在度 `augPresence>=100` 且（难度 `augGameOverAtFullPresence` 或 Aug Menace 特性）→ `aug`。否则存档。
3. **晨报** `StartOfDayUIManager.InitPanel`：逐条显示昨夜 `nightLogs`；若距交租 ≤2 天且未买断，显示"X 天后需支付 N 信用点（租金/按揭）"。同时检测成就 Settled/Rich/Industrious/Shiny。

### 1.3 开门（OPEN）——顾客队列在这一刻一次性生成
`StoreShutterButton.OpenShutter()`（`StoreShutterButton.cs:91`）顺序：
1. `PlayerStore.OnShutterOpened()`：`day>=8` 解锁报纸；`CheckCrime()`（各种"脏水/误标/贿赂"累积量转成治安罪行）；已售物品回溯 `SoldItem.HandleSoldItem`；**`storeClientManager.GenerateClient()` 生成今天全部顾客**；处理"未来队列"中到期的客户。
2. `OpenShutter()`：状态→OPEN。
3. `OnShutterDoorOpenMorning()`：报纸、枪证发放（治安声望 ≥20/50/80）、新手期教学道具、股东分红、Lou、核废料等。
4. `OnLateShutterOpen()`：**"处刑型"早晨访客**——声望 -100 的终局特权（LLLYNCH 暴民/SEC_IMMEDIATE_ARREST/UL_HITMAN/REV_HIT 炸弹/CARTEL_RETIRED）直接插队并触发 game over；Lou 的早晨来访。

### 1.4 营业：顾客轮次
- 队列 `storeClientManager.clientStack`，按铃 `TryCallNextClient` → `GetNextClient` 取队首（`PlayerStore.cs:1331`）。
- 一个顾客 = 一"轮"。顾客意图 `ClientIntent`：BUY / SELL / SELLNBUY / BARTER / RENT / DIALOGUE / INSPECTION / APPRAISAL_SERVICE / WHOLESALE / PROCUREMENT_* / INFORMATION_DEALER…
- **没有计时器**：一天的长度 = 队列长度。"时间"由人头推进（Papers Please 式"队列即时钟"）。
- 不可打发的顾客（`dismissable=false`：房东、高利贷者、许可证官员、检查员…）在队列中或在场时 `CanEndDay()=false`，**不能关门**（`PlayerStore.cs:1816`）。
- 玩家可以**提前关门**：队列非空会弹确认框，剩余顾客 `DismissClientStack()` 直接作废（`StoreShutterButton.cs:78`、`PlayerStore.EndDay`）。

### 1.5 每天顾客数量如何决定（`StoreClientManager.GenerateClient`，`StoreClientManager.cs:149`）
生成顺序（决定队列先后）：
```
展示柜治安检查 → 重要剧情(高利贷首访/吊销枪证) → Wild 救济 → 卡特尔线 → Eleanor → 展示柜吸引客
→ 化学反馈 → 跳过教程客 → 常驻/周期客(AlwayClient) → 事件客 → 教程剧情客(第1-14天) → 检查员
→ 供应商 → 内容解锁/退休专家 → 角色线 → 远征 → 情报贩 → 发明家 → 小角色
→ 改造体(早) → 通缉犯 → 【普通随机客】 → 改造体 → 声望特权客 → 市场短缺/过剩客
→ 夜间服务推荐客 → Sawyer → Eleanor 叔叔 → 【交租日：商人+房东/高利贷】 → 去重 → Lou …
```

**普通随机客数量** `GetNormalCustomerCount()`（`StoreClientManager.cs:1390`）：
```
n = 0
  + 1  若 吸引力 ≥200 且未被破坏
  + 1  若 吸引力 ≥250 且未被破坏
  + 1  若 吸引力 ≥500 且未被破坏
  − 1  声望特权"差评" LLPOORREVIEWS          + 1  声望特权"有口皆碑" LLREFERAL
  − 1  起始特性"偏街陋巷" Side Street       + 1  "黄金地段" Prime Real Estate
  − 1  昨晚外出 Afterhours（"短眠者"免疫）
n = clamp(n, 0, 6)
```
- 先用展示柜预测（`ShowcaseHelper.GetDisplayCasePrediction`，每项按概率 Roll）填满这 n 个名额，剩余名额再 `PickClient()` 随机；当天若已排了通缉犯则再 −1。
- **新手保护**：非跳过教程时 day ≤13 不出普通随机客（只有脚本客），跳过教程时 day 1 也不出。
- **吸引力** `GetCurrentStoreAttractiveness = base(250) − 垃圾惩罚 + 投影仪加成(当天有效)`（`PlayerStore.cs:2371`）
  - 垃圾惩罚：`garbageLevel>2` 时 `base × garbageLevel × 5%`；不付清洁费每晚 `garbageLevel += 2`，上限 10（即最多 −50%）。
  - 装修 RENOVATION1/2/3（500/750/1000）各 `+150 base`（`NetworkUpgradeList.cs:463-477`）。
  - 吸引力分档：≥400 → 客户池 tier1，≥650 → tier2（更富的客户，`GetAttractivenessTier`）；预算加成 ≥300 +10%、≥450 +25%、≥600 +50%、≥750 +75%、≥900 +100%（`StoreClient.ApplyBudgetModifier`）。
  - 默认 250 → 每天 2 个普通客；第一次装修到 400 → 仍是 2 个但进 tier1 且预算 +10%；到 550 → 3 个。
- 其余每日附加：小角色（教程后第 3–7 天必出 1，8–15 天 50%，16–25 天 25%，之后 10%）；情报贩（≥7 天必来，≥4 天 25%）；通缉犯（基础 4% + 2%/天，16 天保底，`GetRandomClientSpawnChance`）；买家/卖家推荐服务各 +1；违禁品买家推荐 +1（并 BM 声望 +2）。

### 1.6 周历（day%7 固定访客，`StoreClientManager.cs` 多处）
| 星期 | 固定事件 |
|---|---|
| 周日 0 | **交租日**：商人(Merchant) + 房东/高利贷收款；供应商 Supply Runner（day 7 为 intro）；Rev 军需官；周末重置促销 `exchangeManager.ResetPromotional` |
| 周一 1 | 卡特尔周报（化学家线）、BM 小偷、低保食品券、下层稀有商人(LLRAREGOOD) |
| 周二 2 | 退休水产/化学家、毒贩 |
| 周三 3 | 退休废料客/水商、**营业许可证官员（day≥38 起每周三，直到买证）**、SEC 剩余物资商、day≥50 额外 FlexiBuyer |
| 周四 4 | 彩票推销员、退休枪匠、卡特尔检查、SEC 征用官 |
| 周五 5 | 发明家 Dr. Jackson（day≥12）、退休农场主/养鼠人、REV 劫掠品 |
| 周六 6 | FlexiBuyer、彩票开奖事件、黄牛 Scalper（哨兵特性） |

### 1.7 里程碑日（非跳过教程）
- d1 导师/食品买卖教学；d2 Afterhours 引导；d4 保险介绍（解锁保险服务）；d5 展示柜；d6 发明家介绍；**d7 Wild 网络介绍（获得 `networkIntroFavor` 恩惠）、供应商介绍、证物柜失窃事件**；d8 议价/鉴定委员会、报纸；d9 违禁品与检查；d10 Sawyer（解锁清洁服务）；d11 邮票/配给站；d12 改造体(Aug)出现；d13 赃物；d14 导师结业（成就 Tutorial）；d18 房东批发（−10%）；d24 导师提醒许可证；d25 地下交易所；d38+ 许可证官员；d43/57/71/85 房东推销售货机（每台 **永久 +150 租金**）；d50 起房价上涨；d62（12+50）改造体"全面清除"可能（需已买许可证 ≥25 天且 presence=0）。

### 1.8 打烊 → 夜间
1. 关卷帘门 `EndAndClose()`：`visitLeftTonight = 1`（每晚 1 次外出机会），桌上非己方物品清除，`EndDay()`：状态→CLOSED，解散剩余队列，`UpdateServiceCost()`（重算所有服务费用 + 冷却 −1）。
2. （可选）出门 **Afterhours**：垃圾场拾荒 / 配给站 / Dr. Jackson（`AfterhourLocation.cs`）。出门会使 `wentToAfterhoursLastNight=true` → 次日普通客 −1（短眠者除外）。
3. 点床铺 → **夜间服务平板** `EndOfDayUIManager`：勾选今晚要付的服务；合计 > 现金时"结束营业"按钮置灰（`RefreshUI`），**所以服务不能赊账，只能砍服务**。
4. `PlayerStore.EndNight()`（`PlayerStore.cs:1691`）结算顺序：
   ```
   记录 RunStats → 反应堆 → 刷新电力 → 机器"夜间早周期"
   → 扣除所有勾选服务费（部分返还 Wild 恩惠）
   → 其他服务(善后人) → 清洁(垃圾/交易所/破坏) → 电力(断电=黑暗) → 保险(盗窃判定)
   → 推荐服务 → 破坏判定 → 屠夫猎杀 → Lou → 检查预警 → 通缉未举报 → 升级提示(现金≥4×租金)
   → 治安警告 → 改造体预警 → 行动 → 外出惩罚
   → 远征 → 健康/成瘾 → 电话 → 前台清空 → 化学模拟/冰箱/漏斗 → 赃物热度衰减 → 机器生产三阶段
   → 状态 MORNING
   ```
5. 事件 `OnDayEnd`（持续时间 −1）→ `BeginDay()` → 回到 1.2。

---

## 2. 房租、按揭、买断

### 2.1 房租（`PlayerStore.cs`）
| 项 | 值 | 来源 |
|---|---|---|
| 初始现金 | 600 | `STARTING_CASH`，`StartNewGame` |
| 初始房租 | 200（`startingRent`）；销赃人/枪匠 +50，水培/水产 +40；特性"高昂房租" +40、"黄金地段" +75 | `NewGameData.HandleInitialItem`、`StartingPerkContent.HandleNewGamePerk` |
| 交租日 | `day % 7 == 0`（第 7、14、21…天，周日） | `GetRentDayCounter()=(7-day%7)%7`、`StoreClientManager.CheckRentDay` |
| 收租方式 | 房东作为**不可打发的顾客**插在当天队列末尾（商人在其前面） | `CheckRentDay`、`StoreClientList.CreateLandlordClient` |
| 涨幅 | 每次（**付或不付都涨**）`rent = (int)(rent × rentIncreaseMultiplier)`：轻松 1.025 / 普通 1.05 / 困难 1.1 / 挑战 1.1 | `RentIncrease()` |
| 额外永久加租 | 接受房东售货机/净水喷泉：每台 `rentValue += 150` | `CreateLandlordVendingMachineClient` |

**付租** `OnRentPaid`：扣 `rentValue + temporaryRent`，清欠款，涨租。只有现金足够时才出现"支付"选项（`CanPayRent`）。买断选项在现金 ≥ 房价时额外出现。

**不付** `OnRentNotPaid`（"拒付"选项永远可选）：
- 若持有 Wild 的「租金宽限 LANDLORD_GRACE」：欠款 `temporaryRent += rentValue`，下周需付 `新租金 + 欠款`；宽限被消耗（state=1）。宽限需在 Wild 网络重新购买：100 恩惠起，**每次购买价格 ×2**，冷却 15 天（`NetworkUpgradeList.CreateLandlordGrace`、`LANDLORD_GRACE_COST_MULTIPLIER=2`）。
- 若无宽限：房东叫治安 → `ExecuteGameOver("rent")`「你拖欠了租金」。
- 难度：轻松/普通**开局自带一次宽限**（`landlordGraceOwnedAtStart=true` → state 2），困难/挑战没有（`StoreService.ApplyDifficultyStart`）。

**房租曲线（起租 200，按代码 int 截断模拟）**

| 周 | 1 | 4 | 8 | 12 | 16 | 20 | 26 |
|---|---|---|---|---|---|---|---|
| 轻松 ×1.025 | 200 | 215 | 235 | 258 | 282 | 310 | 356 |
| 普通 ×1.05 | 200 | 229 | 276 | 333 | 403 | 489 | 652 |
| 困难/挑战 ×1.1 | 200 | 266 | 388 | 565 | 826 | 1206 | 2132 |

累计（前 8/12/16 周）：轻松 1740/2736/3828；普通 1887/3130/4632；困难 2282/4255/7136。

### 2.2 房价（买断价）`GetStoreBuyPrice()`（`PlayerStore.cs:3892`）
```
price = propertyBasePrice + min( max(0, day − 50) × propertyPriceIncreasePerDay , propertyPriceIncreaseCap )
```
| 难度 | 基价 | 每日涨 | 封顶增量 | 封顶时间 | 最终价 |
|---|---|---|---|---|---|
| 轻松 | 12000 | 25 | 3000 | day 170 | 15000 |
| 普通 | 12000 | 50 | 5000 | day 150 | 17000 |
| 困难 | 24000 | 50 | 10000 | day 250 | 34000 |
| 挑战 | 36000 | 100 | 20000 | day 250 | 56000 |

（`PROPERTY_INCREASE_START_DAY=50`；`BASE_COST=12000`、`LEGACY_DOWN_PAYMENT=3000`、`LEGACY_MORTGAGE_WEEK_COUNT=14` 为旧存档兼容常量）

### 2.3 现金买断（胜利条件 A）
- 交租日房东对话中，`cash ≥ price` 时出现 `[买下店铺（X信用点）]`。
- `BuyOutStore()`：扣款 → `IsPropertyPaid=true` → 队列里的高利贷者被换成随机客 → 成就检测 → 获得房契 deed → `ExecuteGameOver("buyout")`「买断」（**带"继续游戏"按钮**，进入无租金的后期）。

### 2.4 按揭（高利贷者 Loan Shark，胜利条件 B）
1. **首访** `HandleImportantScriptedClient`：任何一天开门时若 `cash ≥ 首付` 且未买断 → 高利贷者介绍（只一次）。
2. **报价**：之后每个交租日，若 `cash ≥ 2 × 首付`、未在还款、未拒绝过、未买断 → `CreateLoanShark2` 来报价（选项：同意 / 拒绝(永久，`IsMortgageOfferDeclined`) / 再说）。同意需 `cash ≥ 首付`。
3. 公式（`PlayerStore.cs:3913-3944`）：
   ```
   首付 down    = max(500, RoundToInt(price × mortgageDownPaymentShare / 500) × 500)
   贷款 loan    = price − down
   利息 i       = loan × rate / 100 / 52 × weeks          （逐步整数除法；rate 为"年利率"）
   周供 weekly  = ceil((loan + i) / weeks)
   ```
4. 还款：开始按揭后不再交租（`IsPayingRent()=false`），每个交租日高利贷者 `CreateLoanSharkCollect` 来收周供；最后一期付清 → `OnMortgagePaidOff` → 房契 → `ExecuteGameOver("buyout_mortgage")`（同样可继续）。
5. **还不起**：
   - 若"可被收走的物品"总价值 ≥ 周供 → `Repo()`：按价值降序贪心拿走 ≤ 周供的物品（不足 80% 时再补一件最接近的），留下收据。**注意：`Repo()` 不会 `mortgageWeekLeft--`，这周不算还款——等于罚没物品+延长一周。**
   - 否则 → `ExecuteGameOver("repo")`「店铺被收回」。
   - 机器、文件、容器、锁定物品、走私舱内物品、重要物品不可被收走（`IsEligibleForRepo`）。

**按揭数值表（day ≤50 报价时）**

| 难度 | 房价 | 首付 | 报价门槛(2×首付) | 年利率 | 周数 | 周供 | 总支付 | 溢价 |
|---|---|---|---|---|---|---|---|---|
| 轻松 | 12000 | 2500 | 5000 | 25% | 8 | 1233 | 12360 | +3.0% |
| 普通 | 12000 | 2000 | 4000 | 50% | 12 | 930 | 13152 | +9.6% |
| 困难 | 24000 | 3000 | 6000 | 75% | 16 | 1615 | 28832 | +20.1% |
| 挑战 | 36000 | 7000 | 14000 | 100% | 16 | 2370 | 44912 | +24.8% |

day 150 时：普通房价 17000 → 首付 2500、周供 1348；困难 day 250 房价 34000 → 首付 4000、周供 2307；挑战 day 250 房价 56000 → 首付 11000、周供 3678。

**设计观察**：普通难度周供 930 ≈ 当前房租的 4.6 倍——按揭不是"省钱"，是"提前锁价 + 把指数房租换成固定年金"。在困难/挑战里房租 ×1.1/周，第 20 周已 1206，第 26 周 2132，会**超过**固定周供 1615 → 存在明确的"按揭交叉点"，鼓励中期锁定。

### 2.5 买断后
- 不再交租；交租日只来商人（`CheckRentDay` 第二分支）。
- Wild 网络的 **Advanced 标签**仅在 `IsPropertyPaid` 后显示（`NetworkUpgrade.IsHiddenForRun`）：超科技走私贩推荐 2500（之后每晚服务 500，冷却 5 天）、Endorsement I/II/III 2500/5000/10000（每级客户预算 +25%）。——后期的钱有去处。
- 无尽模式 `endlessMode`：关闭角色剧情线与小角色。

---

## 3. 四个难度 DifficultyRules 全参数（`Difficulty.cs:208-389`）

标题（`MainMenu` 表）：轻松「漫天要价 Price Gouge」/ 普通「加价销售 Mark Up」/ 困难「微薄利润 Thin Margins」/ 挑战「赤字连连 Red Ink」。

| 参数 | 含义 / 使用处 | cozy | normal | hard | challenge |
|---|---|---|---|---|---|
| retailMarkup | 卖给接受加价的顾客时 +X%（市场阶段）`GameItem.GetCurrentValue`、`ItemFeatureList.RetailMarkUp` | **20** | **15** | **0** | **0** |
| rentIncreaseMultiplier | 每周房租倍率 | 1.025 | 1.05 | 1.10 | 1.10 |
| landlordGraceOwnedAtStart | 开局自带租金宽限 | ✔ | ✔ | ✘ | ✘ |
| powerCostMultiplier | 电费倍率 | 1 | 1 | 2 | 2 |
| insuranceCostMultiplier | 保险倍率 | 1 | 1 | 2 | 3 |
| startPowerCostBonus | 开局电费加值（见 §4 注） | 0 | 0 | 3 | 3 |
| startJanitorialCostBonus | 开局清洁费加值 | 0 | 0 | 3 | 3 |
| startInsuranceCostBonus | 开局保险加值 | 0 | 0 | 4 | 5 |
| appraisalJoinCostMultiplier | 鉴定委员会入会费 85×N | 1 | 1 | 2 | 3 |
| vendorPermitFee | 营业许可证 | 500 | 1500 | 2000 | 3000 |
| newCustomerDiscount | 新客/首单折扣（买入时 −X%） | 75 | 75 | 35 | 10 |
| upgradePriceMultiplier | Wild 网络升级价倍率 | 1.0 | 1.0 | 1.5 | 2.0 |
| inventorMarkup | 发明家卖给你 +X% | 0 | 15 | 50 | 100 |
| supplierMarkupMultiplier | 供应商基础加价 50% 的倍率 | 1 | 1 | 2 | 3 |
| retiredSupplierMarkup | 退休专家卖给你 +X% | 0 | 15 | 30 | 45 |
| merchantDiscount | 交租日商人收购折扣 −X% | 10 | 15 | 20 | 25 |
| scalperMarkupBase | 黄牛基础加价 | 0 | 15 | 25 | 25 |
| scalperMarkupPerDay | 黄牛每日递增 | 0.5 | 1 | 2 | 3 |
| propertyBasePrice | 房价基价 | 12000 | 12000 | 24000 | 36000 |
| propertyPriceIncreasePerDay | day>50 每日涨 | 25 | 50 | 50 | 100 |
| propertyPriceIncreaseCap | 涨幅封顶 | 3000 | 5000 | 10000 | 20000 |
| mortgageInterestRate | 按揭年利率% | 25 | 50 | 75 | 100 |
| mortgageWeeks | 按揭周数 | 8 | 12 | 16 | 16 |
| mortgageDownPaymentShare | 首付比例 | 0.20 | 0.15 | 0.125 | 0.20 |
| reputationGainMultiplier | 声望获得倍率 | 1.5 | 1.0 | 0.75 | 0.5 |
| reputationLossMultiplier | 声望损失倍率 | 0.5 | 1.0 | 1.25 | 2.0 |
| appraisalProgressGainMultiplier | 鉴定等级经验获得 | 1.25 | 1.0 | 1.0 | 0.75 |
| appraisalProgressLossMultiplier | 鉴定经验损失 | 0.5 | 1.0 | 1.5 | 2.0 |
| tradeFavorRate | 每笔交易额转 Wild 恩惠 | 10% | 5% | 0 | 0 |
| wildServiceFavorRefund | Wild 系服务费返还为恩惠 | 75% | 50% | 25% | 15% |
| networkIntroFavor | 第 7 天 Wild 介绍送恩惠 | 300 | 200 | 100 | 0 |
| augKillPresenceDrop | 击杀改造体降低存在度 | 25 | 25 | 10 | 8 |
| augLeftAlivePresenceRise | 放走改造体增加存在度 | 10 | 10 | 15 | 16 |
| augWarning | 改造体来袭夜报预警 | ✔ | ✔ | ✘ | ✘ |
| augGameOverAtFullPresence | 存在度 100 即结局 | ✘ | ✘ | ✔ | ✔ |
| louDemandValue | Lou 勒索金额 | 500 | 750 | 1000 | 1500 |
| wildBailout | Wild 一次性救济（250 恩惠） | ✔ | ✔ | ✘ | ✘ |
| wildReminders | 升级/网络提醒客 | ✔ | ✔ | ✘ | ✘ |
| displayCaseTip | 展示柜时机提示 | ✔ | ✔ | ✘ | ✘ |
| eventEndCountdown | 新闻显示事件剩余天数 | ✔ | ✔ | ✘ | ✘ |
| impostorTell | 冒充者破绽提示 | ✔ | ✔ | ✘ | ✘ |

难度迁移：旧存档只有 `isHardMode`，`ResolveId` 映射为 normal/hard；模组可 `Difficulty.Register` 新难度（未设 rules 时克隆 normal）。`IsHardOrAbove()` 用 order（100/200/300/400）判断，影响成就 Nothing to Riches 等。

> 难度描述面板会把与 normal 不同的项列出（`Difficulty.CreateEffects`），但并非所有参数都有描述——例如 inventor/scalper/merchant/mortgage 细节、改造体参数玩家在 UI 上看不到，属于"隐藏难度"。

---

## 4. 固定开支与收入构成

### 4.1 夜间服务（`StoreService.cs`、`StoreServiceSheet.cs`）
每晚在平板上勾选，`EndNight` 扣费。`UpdateCost()` 在每次打烊 `EndDay()` 时重算。

| 服务 (loc) | 解锁 | 费用公式 | 不付的后果 |
|---|---|---|---|
| 电力服务 POWER | 开局，默认勾选 | `(day/10 + 5) × powerCostMultiplier`；电网劫持(Wild 300) → 0 且常开；全站停电事件当晚 0；简易反应堆特性 → 移除 | 断电：店内变暗，仅能量电池供电的机器运转 |
| 保险服务 INSURANCE | d4 剧情（化学家开局由卡特尔保护替代；BM"无法参保"特权移除） | `(净资产/400 + 3)`，×2 若"沦为猎物"LLTARGET，× `insuranceCostMultiplier`，×0.5 王尔德红人，×1.5 王尔德眼中钉 | 盗窃概率 `33% + 5% × 连续未保天数`（LLTARGET +33，SEC_PROTECTION −16，SEC_VIP 免疫）；警报器有概率拦截；被偷物品直接销毁。投保时有 6% 发生"被偷后追回"的叙事日志 |
| 清洁服务 JANITORIAL | d10 Sawyer（Sawyer Crew 特性开局，费用 0） | `day/10 + 3`；被破坏时 `day/10 + 75`；BM 不信任 ×2 | 垃圾 +2/晚（上限 10），吸引力下降；不清空垃圾桶；不处理地下交易所；不清理破坏 |
| 盘查预警 SEC_INFORMANT | Wild 300 | 10 | 无检查倒计时提示（检查前 3/2/1 天夜报） |
| 买家/卖家推荐 | Wild 各 200 | 各 10 | — 付费则次日 +1 购买客/供货客 |
| 违禁品买家推荐 CRIMINEL_NETWORK | Wild 350 | 45，冷却 3 天 | 付费：次日 +1 罪犯买家/佣兵，BM 声望 +2 |
| 王尔德的善后人 WILD_FIXER | Wild 350/450/550 | 25 / 45 / 70（三级） | 付费可降低治安证据（`secData.OnFixerUsed`） |
| 卡特尔保护费 | 化学家开局，强制 | 0 | 替代保险 |
| 杂物清运 DEBRIS | 有杂物堆时 | 50 | — |
| 猎杀屠夫 | 剧情 | 500 | — |
| 电流转换器 | 有转换器时 | `CurrentConverterHelper.UpdateServiceCost` | — |
| 超科技走私贩 | 买断后 Advanced 2500 | 500，冷却 5 | 付费次日来超科技商人 |

Wild 系服务（保险、两种推荐、善后人、违禁品推荐）付费后返还 `cost × wildServiceFavorRefund` 恩惠（`PlayerStore.cs:1709-1717`）。

**典型夜间账单**（普通难度）：day 1–9 仅电 5；d10 电 6 + 清洁 4 + 保险(净资产 2000) 8 ≈ 18/晚；d50 电 10 + 清洁 8 + 保险(净资产 10000) 28 ≈ 46/晚；困难同期 ≈ 20+8+56 = 84/晚；挑战 ≈ 20+8+84 = 112/晚。

> ⚠️ **疑似失效参数**：`ApplyDifficultyStart` 把 `startPowerCostBonus / startJanitorialCostBonus / startInsuranceCostBonus` 加在初始 cost 上，但第一天打烊 `EndDay → UpdateServiceCost` 就用公式整体重算覆盖，玩家看到的平板永远是公式值。这三个参数在实际账单里不起作用（只影响难度描述"夜间服务费用较高"）。电费/保险的难度差异真正来自两个 Multiplier。

### 4.2 其他一次性 / 事件型支出
| 项 | 数值 | 来源 |
|---|---|---|
| 营业许可证 | 500/1500/2000/3000 | `GetBusinessPermitPrice`、`CreateBusinessPermit` |
| 拒办许可证 | 治安罪行 OPERATING_WITHOUT_LICENSE `20 × (day/7) + 50` | `StoreClientList.cs:4159` |
| 许可证"捐赠"路线 | 从 d45 起每周三累计 `25 + 10 × (day/7)`（最低 10），用物品价值抵扣 | `StoreClientManager.cs:881`、`GetBribeAmount` |
| 鉴定委员会入会 | 85 × (1/1/2/3)；晋级费 = 入会费 × rank.costShare | `GetAppraisalJoinCost`、`AppraisalRankHelper.cs:167` |
| 治安罚款 | = 当前罪行总值；证据 ≥95 时警官上门：罪行 <300 罚款（付不起=逮捕）、300–1499 逮捕、≥1500 处决 | `StoreClientListSecurity.cs:35-50`、`SecData.GetFineValue` |
| 错误举报 | 常量 50（`WRONGFUL_REPORT_FINE`），上限 5 次 | `PlayerStore.cs:524` |
| Lou 勒索 | `louDemandValue`（不还 → `lou_bombed_debt` 结局） | `StoreClientData.GetLouDemandValue` |
| Wild 网络升级 | 见 `NetworkUpgradeList.cs` 常量，× `GetCost()` 倍率（难度 ×1/1/1.5/2，下层传奇 ×0.85，王尔德红人 ×0.9，眼中钉 ×1.1）；可用"恩惠+现金"支付（先扣恩惠） | `NetworkUpgrade.GetCost`、`WildNetworkUI.cs:567` |
| 盗窃损失 | 被偷物品价值 | `StealItem` |
| 按揭收走 | ≤ 周供价值的物品 | `Repo` |

主要升级价（基价）：装修 500/750/1000、展示柜 I/II 200/500、电网劫持 300/600/1000、商业电力 350、售货机/净水喷泉 1200、自来水 250/350、退休专家线 300–1000、枪证 300/600/900、股东 900–2500、Endorsement 2500/5000/10000、超科技走私 2500、宽限 100（×2 递增）、治安宽限 250、证据清除 500。

### 4.3 收入来源
1. **柜台零售**（核心）：卖给 BUY/SELLNBUY 顾客，受顾客预算 `clientBudget` 封顶（`SellItem` 超预算按预算收）。预算基数随天数增长，如商人 `200 + rand(0,100) + 3×day`、佣兵 `300 + 2×day`。
2. **低买高卖差价**：从 SELL 顾客收购（赃物额外 −10/−15/−20/−25% 按热度）、批发 `SetWholesale`、物物交换。
3. **违禁品溢价**：卖出时 +35%/+70%/+100%/+150%（低/中/高/危急），持许可证后变"受限"减半为 +17/+35/+50%（`ItemFeatureList.cs:400-500`）。
4. **生产链**：开局职业机器（水培、枪打印机、化学、养鼠、水产…）夜间产出，卖成品。
5. **自动售货机/净水喷泉**：夜间自动售出，售价强制带零售加价；概率 `吸引力 × 品类系数 × 200/(200+价格)`，消耗品 ×1.5、模块/化工 0.15、奢侈品 0.165、材料 0.33、工具/电池 0.5；保底售出 1/2/3 件（吸引力 <25/≥25/≥50）（`VendingMachineHelper.cs:420-470`）。
6. **通缉犯举报赏金** `GrantWantedCaptureReward`：`reportReward`（受"前科犯/模范市民"特性修正）+ 治安声望。
7. **鉴定服务**、**股东分红**（周四啤酒/周五矿石/周六梦尘/周日药品实物）、**彩票/刮刮乐**、**拾荒与远征**、**低保食品券**（周一，按资产 ≤600→160 …）、**Georgio 借贷线**、**Wild 救济 250 恩惠**（8 日净资产连续下滑时一次）。
8. **Wild 恩惠**（准货币，只能买升级/宽限）：交易额 × tradeFavorRate、服务返还、下层声望每 +1 → +40 恩惠（`StoreReputation.ModReputation`）。

---

## 5. 定价与加价公式

### 5.1 统一价值公式（`GameItem.ComposeStagedValue`，`GameItem.cs:1733`）
```
基础   B = unitValue + 动力源价值(GetComplexeItemValue)
内在   I = Σ 内在阶段 ItemFeature%（品质、成色、纯度…）
市场   M = Σ 市场阶段 ItemFeature%（事件、零售加价、违禁品溢价、各种折扣/加价、议价）
最终   F = Π (1 + 最终阶段%/100)（交易/议价结果）
杂项   X = lateUnitValue + 子物品价值（容器内物品各自按同样规则计算）
容器   W = 液体/水的价值（按市场缩放）

价格 = max(0, round( ( max(0, B×(1+I/100)) + W ) × max(0, 1+M/100) + X ) × F )
```
- **所有市场修正是加法叠加后一次相乘**：例如零售 +15% 与事件 +30% 合并为 ×1.45，而不是 1.15×1.30。
- `GetNegociatedValue()` 是成交价；`GetCurrentValue(useRetailMarkup)` 用于估值/保险/收走（收走、保险、净资产都**不带零售加价**）。

### 5.2 各类加价/折扣一览
| 名称 | 方向 | 数值 | 触发 | 来源 |
|---|---|---|---|---|
| 零售加价 retailMarkUp | 卖出 | +retailMarkup（20/15/0/0） | 自有物品放上秤、顾客 `acceptRetailMarkup`（商人/黑市商人=false） | `GameItem.OnAddedToWeightedArea`、`IsCurrentClientAcceptMarkup` |
| 违禁品溢价 | 卖出 | +35/70/100/150 | 违禁品等级 1–4 | `ItemFeatureList.Contraband*MarkUp` |
| 受限品溢价 | 卖出 | +17/35/50 | 持对应许可证 | `Restricted*MarkUp` |
| 商人折扣 merchantDiscountBuy | 卖给商人 | −merchantDiscount（10/15/20/25） | 交租日商人 | `CreateMerchant`、`MerchantDiscountBuy` |
| 供应商加价 premiumBuy | 买入 | +50 × supplierMarkupMultiplier（50/50/100/150） | 周日 Supply Runner | `PremiumBuy`、`SupplierMarkup(50)` |
| 退休专家加价 premiumBuy15 | 买入 | +retiredSupplierMarkup（0/15/30/45） | 退休专家周访 | `PremiumBuy15` |
| 发明家加价 | 买入 | +inventorMarkup（0/15/50/100）+ 事件价格波动；下层传奇 −15 | 周五 Dr. Jackson | `InventorMarkup`、`CreateInventor` |
| 超科技加价 | 买入 | +SupplierMarkup(100)/2（50/50/100/150） | 买断后 | `PremiumBuyHypertech` |
| 黄牛加价 | 买入 | `scalperMarkupBase + floor(scalperMarkupPerDay × 距首访天数)` | 哨兵特性周六 | `StoreClientListSpec.GetScalperMarkup` |
| 新客折扣 firstTimeDiscount | 买入 | −newCustomerDiscount（75/75/35/10） | 教程期首单 | `FirstTimeDiscount`、`StoreClientListStory.cs:1321` |
| 赃物折扣 | 买入 | −10/−15/−20/−25（热度 ≤25/≤50/≤75/>75） | 收购赃物 | `StolenHelper.ApplyStolenDiscount` |
| 违禁品收购折扣 | 买入 | −溢价/2 | 部分黑市客 | `DiscountedLowContraband…` |
| 房东批发 | 买入 | −10% | d18 | `LandlordWholesale` |
| 下层传奇 LLPILLAR | 买入 | −15%（发明家/部分客） | 下层声望特权 | `DiscountBuy15` |
| 不善议价 | 双向 | −5% 交易惩罚，且禁用议价 | 起始特性 | `AWFUL_NEGOTIATOR_PRICE_PENALTY` |
| 成瘾惩罚 | 双向 | 按成瘾程度 | 健康系统 | `AddictionBuy/SellPenalty` |
| 小费 Pourboire | 卖出 | UL_TIPS 特权 | 上层客 | `ItemFeatureList.Pourboire` |
| 多样性买家 | 卖出 | +15% | 要求多品类的客 | `VarietyBuyerBonus` |

### 5.3 议价（`BargainUIManager.GetBargainSuccessChance`，`BargainUIManager.cs:1258`）
- 让利（卖出降价/买入加价）：100% 成功。
- 抬价（卖出加价 / 买入压价）成功率：`clamp(100 − 2×|p%| + 声望/6(负数×2) + 事件客15 + DealMaker, 0, 100)`；`alwaysRejectBadBargain` 的客户（如房东批发）为 0。
- 离开率：`100 − max(成功率, 100 − 1.2×|p%| + 声望修正 + 事件15)`。
- 勒索 Blackmail：基础 60% + 声望/4 + 事件 15。
- 顾客预算不足、实际成交额低于标价时（`SellItem` 中按预算收款，`OnItemTraded` 里 `value != originalValue`），该笔交易声望倍率 +1——"让利换口碑"。

---

## 6. 失败 / 胜利条件与成就

### 6.1 结局（`PlayerStore.ExecuteGameOver`，`PlayerStore.cs:1520`；文本 `Mechanic` 表）
| id | 中文 | 触发 | 经济相关性 |
|---|---|---|---|
| **buyout** | 买断（胜利，可继续） | 交租日现金 ≥ 房价，向房东买断 | ★核心胜利 |
| **buyout_mortgage** | 买断（按揭版，可继续） | 付清最后一期周供 | ★核心胜利 |
| **rent** | 你拖欠了租金 | 无宽限时拒付/付不起房租 | ★核心失败 |
| **repo** | 店铺被收回 | 按揭周供付不起且可收走物品不足 | ★核心失败 |
| arrest | 被捕 | 治安证据满且罪行 300–1499，或罚款付不起 | 罚款=钱 |
| execution | 被处决 | 罪行 ≥1500 | — |
| mob / sec / hitman / bombed / cartel_retired | 遭私刑 / 遭栽赃 / 遭暗杀 / 死于爆炸 / 被"退休" | 下层/治安/上层/革命军/卡特尔声望 −100 的早晨访客 | 声望 = 隐性资产 |
| lou_bombed / lou_bombed_debt | 死于爆炸 | 动 Lou 的玩具箱 / 欠 Lou 的钱 | 勒索 |
| aug | 改造同化 | 存在度 100（困难/挑战或 Aug Menace） | — |
| sentinel | 哨兵崩溃 | 哨兵特性漏打免疫宁 | 每周购药开支 |
| reactor | 堆芯熔毁 | 简易反应堆缺维护 | 维修开支 |
| vendor_permit | 无证经营 | **代码中没有任何调用**（只有文本和 RunSummary 显示）→ 疑似被砍；无证改为通过治安罪行间接致败 | — |

### 6.2 Steam 成就（`AchievementTracker.cs`；loc 中无成就名，以下为按 API 名意译）
| API | 条件 | 经济向 |
|---|---|---|
| ACH_TUTORIAL | 导师结业（d14） | |
| **ACH_BUYOUT** | 拥有店铺（现金或按揭） | ★ |
| **ACH_HONEST_MERCHANT** | 拥有店铺且本局从未卖出赃物/违禁品（`soldStolenOrContrabandThisRun`） | ★ 道德×经济 |
| ACH_AUTHORIZED | 买营业许可证 | ★ |
| ACH_RATS | 以养鼠人开局 | |
| **ACH_RAG_TO_RICHES** | 一贫如洗（0 现金）开局拥有店铺 | ★ |
| **ACH_NOTHING_TO_RICHES** | 一贫如洗 + 困难及以上拥有店铺 | ★ |
| ACH_PERFECT_MEDIATOR | 任一派系声望 ≥200 | |
| ACH_LUCKY | 刮刮乐中奖 ≥350 | |
| **ACH_SETTLED** | 净资产 >10,000（晨报时检测） | ★ |
| **ACH_RICH** | 净资产 >100,000 | ★ |
| ACH_LAB_PURE | 化学产物达"实验室纯度" | |
| ACH_HUGE_NUTRIFRUIT | 种出巨型营养果 | |
| **ACH_PRODIGIOUS** | 第 50 天前拥有店铺 | ★（实际只能现金买断：最晚 d49 交租日；按揭最少 8 周还完来不及） |
| ACH_SHINY | 吸引力 ≥1000 | |
| ACH_OVERLOAD | 模块品质加成 ≥250 | |
| ACH_MODEL_CITIZEN | 累计举报 10 名罪犯 | |
| ACH_AUG_HUNTER | 累计射杀 10 个改造体 | |
| ACH_LOWER_LEVELS_SPECIAL | 第一次用贴标机改标签（"下层特供"） | 欺诈 |
| ACH_MASTER_APPRAISER | 累计 10 次完美鉴定 | |
| ACH_INDUSTRIOUS | 同时拥有 ≥10 台机器 | |

净资产 = 柜台/展示柜/库存/档案/垃圾桶/售货机内**自有物品成交价之和 + 现金**（`GetTotalEstimatedValue`，`PlayerStore.cs:3627`）——同时用于保险费、Wild 救济判断、成就、RunStats。

### 6.3 RunStats（结算页统计，`RunStats.cs`）
记录：每日净资产/现金曲线、总销售、最佳单日、最佳倒卖（买价/卖价）、最大单笔、售货机/喷泉收入、已付房租/周数/欠租周数、电/清洁/保险/网络开销、**"最险的一次"（付租/周供后剩余现金最少，`closestCallCredits`）**、买店日/价格/是否按揭、许可证/贿赂、Georgio 借还、拾荒、检查/没收/行贿、盗窃损失、错误举报、射杀改造体。→ 这是"复盘叙事"的素材库。

---

## 7. 起始配置对经济的影响（`NewGameData.HandleInitialItem`、`StartingScenario.cs:148`）
| 开局 (NewGame 表) | 初始现金 | 房租 | 特点 |
|---|---|---|---|
| 学徒 Apprentice | 550 | 200 | 新手推荐，食品/日用品 |
| 水商 | 400 | 200 | 水检测/净化/装瓶，解锁 WATER_TRADER |
| 销赃人 Fence（跳教程） | 450 | **250** | 走私舱、违禁品；BM +30 / 治安 −30；交易所解锁 |
| 一贫如洗 Rock Bottom（跳教程） | **0** | 200 | 无 Wild 救济、无提醒；成就向 |
| 水培农场主 | 450 | **240** | 水培机 |
| 枪械匠（跳教程） | **150** | **250** | 枪打印机、一级枪证、治安 +25 |
| 化学家（跳教程） | 300 | 200 | 化学设备；卡特尔保护替代保险；卡特尔周线 |
| 养鼠人 Rat Wrangler | 600 | 200 | 鼠笼 + 6 只鼠 |
| 水产养殖户 | **150** | **240** | 两套 RAS 鱼缸 |
| 废料客 Junker（Coming soon） | 400 | 200 | — |

经济向起始特性（`PerkTable`）：存款 +150；高昂房租 +40 租；黄金地段 +75 租、+1 客/天；偏街陋巷 −1 客/天；低保户（周一按资产送食品券）；王尔德红人（升级 −10%、保险 −50%）/眼中钉（+10%/+50%）；不善议价（−5%、禁议价）；自动贩卖机开局；简易反应堆（无电费、需维修、可能熔毁）；短眠者（外出不掉客）；囤积癖（3 堆杂物，清运 50/次）。

---

## 8. 设计点评

### 8.1 压力曲线：三条"斜率"叠加
1. **周租指数曲线**（主压力，固定节拍）：每 7 天一次硬检查，金额 ×1.025/1.05/1.10。普通难度 15 周翻倍，困难约 7.3 周翻倍（第 26 周 2132/周）。**付不付都涨**，拖欠只是借时间，没有"省钱"选项。
2. **房价线性曲线**（第二压力，d50 后开启）：+25~100/天并封顶。它制造了一个"窗口期"：第 1–50 天是"锁价期"，之后每多拖一天胜利成本上升——对应 ACH_PRODIGIOUS（<50 天）这个高手目标。
3. **随财富增长的"财富税"**：保险 = 净资产/400+3（×难度），清洁/电费随天数线性涨。越有钱，每晚固定开支越高——防止囤货躺赢，也让"把资产换成现金去买断"有动力。

而收入侧：普通客数量由吸引力阶梯（200/250/500）决定、最多 6 人，顾客预算随天数 +2~3/天、随吸引力最多 +100%。**收入是线性+阶梯，支出是指数**——这保证了中后期必须靠"质变"（升级、生产链、违禁品、买断）而不是重复劳动来脱离压力。

### 8.2 早期的保护垫（新手节奏）
- d1–13 无普通随机客，全部是脚本教学客；新客首单 −75% 进货折扣（轻松/普通）。
- 开局自带一次租金宽限（轻松/普通）；Wild 介绍送 200–300 恩惠；Wild 8 日资产下滑救济 250 恩惠；第 7 天才第一次交租，且**交租日必定有一个预算 200+ 的商人**在房东前面——这是精心设计的"流动性阀门"：库存不够现金时可以打折（−10~25%）变现。商人折扣随难度加深，等于"变现成本"也是难度旋钮。
- 困难/挑战把这些全部拿掉（无宽限、无救济、无提示、无零售加价），难度差异主要体现在"**容错**"而非数值。

### 8.3 关键决策点
| 决策 | 张力 |
|---|---|
| 每晚服务勾选 | 电/保险/清洁是"小额必需品 vs 风险"：不保险 38%+ 被盗、不清洁掉吸引力→掉客。服务费不能赊账，现金紧时被迫砍保险——"穷人更容易被偷"的反馈环。 |
| 提前关门 vs 接完客 | 没有计时器，但出门 Afterhours 会让次日 −1 客；关门剩余客作废。 |
| 交租日：付 / 拒付(用宽限) / 买断 / 按揭 | 宽限价格每次 ×2，越用越贵；按揭是"把指数换成年金"，困难难度有明确交叉点（约第 20 周后房租 > 周供）。 |
| 按揭违约 | 还不起时被收走物品且**这一周不计入**——故意设计成"拖延=加倍惩罚"。 |
| 许可证 | 付现金（500–3000）/ 捐物贿赂（d45 后每周累加）/ 拒绝（累积罪行→罚款→逮捕）。三种付款"币种"：现金、库存、治安风险。 |
| 违禁品/赃物 | +35~150% 溢价、−10~25% 收购折扣 vs 治安证据与 Honest Merchant 成就。 |
| 零售加价 vs 议价 | 零售加价是"白送"的利润率（轻松 20%/普通 15%），困难起为 0，利润全靠议价（每 1% 抬价 −2% 成功率）和信息差（鉴定、赃物、误标）。困难的副标题「微薄利润」就是这个意思。 |
| Wild 恩惠 | 第二货币，只能买升级；交易返恩惠（轻松 10%）让"多交易"本身就有复利。困难起 0 → 升级全靠现金且 ×1.5/×2。 |

### 8.4 钩子与长线目标
- **明确的终点**（买断）+ "继续游戏"：结局是阶段性胜利，之后解锁 Advanced 升级（Endorsement 10000、超科技走私 2500+500/晚）作为后期资金黑洞，加上 ACH_RICH（10 万净资产）。
- **RunStats 复盘**：最险的一次、最佳倒卖、最大单笔——把经济数据转成可分享的故事。
- **多开局 × 多特性 × 四难度**：起始现金/房租/客流修正组合出不同的经济起点，"一贫如洗 + 困难" 是成就级挑战。
- **周历节奏**：固定星期出现的商人/供应商/发明家/退休专家，让玩家形成"周日卖、周日进货、周五找 Jackson"的计划感，契合 7 天交租周期。

### 8.5 可借鉴 / 可改进
- 借鉴：①"队列即时钟"省去时间管理 UI；②交租日配套流动性阀门（商人）；③付不付都涨租，消除"拖欠占便宜"；④难度主要拆在"容错开关"+"利润率"两层，参数表化、模组可扩展（`Difficulty.Register`）。
- 观察到的问题/残留：①`start*CostBonus` 三个难度参数被 `UpdateCost` 覆盖，实际无效；②`vendor_permit` 结局无触发；③`dayUntilRent`、旧贷款字段、`loan/debt` 特性、TurnManager/StoreDate/AmenitiesManager 为残留；④按揭 Repo 不推进周数，若玩家不了解会感觉"被骗"，UI 上应明确提示；⑤房价封顶后（普通 d150）买断压力消失，长线后期主要靠房租指数驱动，轻松模式房租 ×1.025 几乎无压力。

---

### 附：关键公式速查
```
交租日           day % 7 == 0
房租             rent_{n+1} = int(rent_n × {1.025,1.05,1.1,1.1})        （付/不付都涨）
欠租             owe = rent + temporaryRent；无宽限拒付 → GAME OVER
房价             base + min(max(0,day−50)×perDay, cap)
首付             max(500, round(price×share/500)×500)
周供             ceil((L + L×rate/100/52×W) / W)，L = price − 首付
按揭报价门槛     cash ≥ 2×首付（交租日）；高利贷首访 cash ≥ 首付
普通客数         clamp([att≥200]+[att≥250]+[att≥500] ± perks − afterhours, 0, 6)
吸引力           250(+150/装修) − 250×garbage×5%(garbage>2) + 投影仪
电费             (day/10+5)×{1,1,2,2}
清洁             day/10+3（被破坏 +72）
保险             (净资产/400+3)×{1,1,2,3}
未保险被盗率     33% + 5%×连续未保天数
价格             round(((B(1+I%)+W)(1+ΣM%)+X)×ΠF)
议价成功率       clamp(100 − 2|p| + rep/6 + 15(事件) + dealmaker)
黄牛加价         base + floor(perDay × 距首访天数)
```
