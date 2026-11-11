# 末日列车 · Doomsday Train（可玩 Demo v0.2）

**在线试玩：https://cloudfj77-cloud.github.io/doomsday-train/**
（需要支持 WebGPU 的桌面版 Chrome / Edge 113+）

俯视角体素废土风「列车经营 + 搜打撤」策略游戏的可玩 Demo。你用全部家当换来一张「希望线」运行许可，要还清 6000 欠款。

v0.2 规则与数值按团队文档《00 功能文档总表｜末日列车》及 11 个分支表（0 Tag 首测基准）实现，
并加入从《可能是偷的》《逃离鸭科夫》拆解中提炼的补充玩法（标题页「实验玩法」可关闭）。设计说明见 `docs/v0.2-plan.md`。

## 玩法

| 环节 | 内容 | 文档出处 |
|-|-|-|
| 到站小循环 | 到站后点列车上的车厢进入功能：火车头（驾驶、司机、自用装备、基础接待）、存储 / 载客 / 种植 / 加工车厢 | 01·05 |
| 司机与路线 | 每天 3 条候选（含资源场 / 猎场的 7 种模板），整条须在司机额度内；中途商业站可提前收车、次日续线；司机 3/4/5/6 站，次日上岗 | 05·01、06·02 |
| 交易 | 基础 3 名客商归火车头，每节存储车厢按等级 +1/+2/+3 接待；单拿 ceil(V×a)、60% 整批 −10/15/20%；牌价 floor(V×1.2)；首站保障 2 净水 + 1 口粮 | 07 |
| 格子仓库 | 车头 4×2、存储 4×4/6×4/9×4 独立网格；矩形摆放、旋转、合堆、拖动跨仓；整批原子入库；载荷 Q 按件计 | 05·02、10·07 |
| 载客 | 普通 20—28 / 贵族 60—90 每段；期限当天 / 次日；按座位等级判定登车；逾期退款 + 赔偿；车上购物 | 05·03 |
| 生产 | 种植：2 种子 + 2 净水 → 4 蔬菜（300 工作量）；加工：净水器、冶金炉、制造台；只随真实车程推进 | 05·04、05·06 |
| 探索 | 资源场（3 种主题）/ 猎场俯视射击：手枪、近战 / 冲刺 / 远程敌人、视锥与声响、5 类容器、背包 4×2、绷带、撤离 5 秒、死亡遗失包一次回收 | 08、09 |
| 结算 | 燃油 ceil(Σ(6+4N+0.5Q))、司机工资、生活费 2 / 贵族 7、设备与枪械条件维护；负现金可继续经营但禁主动付款；第 5k 天偿债 | 11 |
| 【补充案】鉴定掺假 | 净水 / 零件 / 绷带有标称 / 真实品质；供货可能虚标，验货揭穿重算价格；可兑水、翻新；被识破扣声望，坑多了招来稽查员 | 《可能是偷的》 |
| 【补充案】议价声望 | 压价 / 抬价 10%、25%，成功率公开（100−2p+声望/6）；贪心扣声望；声望影响客流与预算 | 《可能是偷的》 |
| 【补充案】品质容器 | 普通 / 精良 / 稀有容器，读条时间 ×1/1.5/2，稀有箱必出高价组 | 《逃离鸭科夫》 |

暂未包含：军队车厢与随行军人、研发与精工奢侈品、维护员、贵族服务要求、3 存档位、难度词条与开局特性、随机事件。

## 目录

| 路径 | 说明 |
|-|-|
| `assets/sim/` | 经营核心（纯 TypeScript，可单测）：数据表 `data.ts`、规则 `economy.ts`、格子仓库 `grid.ts`、探索地图生成 `explore-map.ts` |
| `assets/voxels.pack.ts` | 体素模型：列车、车厢、站台、废墟、丧尸 |
| `assets/train/train.pack.ts` | 引擎插件：列车跑线、场景滚动、站点进出 |
| `assets/ui/` | 界面与交互（`controller.ts`），探索现场（`explore.ts`，Canvas 俯视射击）；文字唯一来源 `game.ui.i18n.json`（由 `scripts/gen-i18n.py` 生成） |
| `scripts/balance-sim.mts` | 30 天平衡模拟（贪心机器人，只经营不出击） |
| `qa/v02.cjs` | v0.2 真实输入验收（19 项，截图在 `artifacts/v02/`） |

## 本地运行

换电脑 / 云电脑开发看 [`docs/dev-setup.md`](docs/dev-setup.md)。

基于 [ForgeAX Engine](https://github.com/ForgeaXGame) 0.4.19。

```bash
pnpm install
bash scripts/build.sh                                  # 打包到 dist/
pnpm exec forgeax project preview --port 4195          # http://localhost:4195/
pnpm exec vitest run assets/sim                        # 经营逻辑单测
pnpm exec tsx scripts/balance-sim.mts 300              # 平衡模拟
node qa/v02.cjs "http://localhost:4195/?qa=1&seed=3"   # 验收
```

发布到 GitHub Pages：`bash scripts/publish-pages.sh`（按子路径打包并推送到 `gh-pages` 分支，约 1 分钟后生效）。

### 参考游戏拆解

`docs/teardowns/index.html`：《可能是偷的》（Probably Stolen）与《逃离鸭科夫》（Escape from Duckov）的系统、数值、叙事、美术拆解总览，以及对本项目的借鉴点。仅供内部学习研究，相关代码与美术版权归原开发商所有。

---

## English

**Play online: https://cloudfj77-cloud.github.io/doomsday-train/** (desktop Chrome / Edge with WebGPU)

A voxel wasteland train-tycoon + extraction-shooter demo (v0.2). Each day pick a route within your driver's quota;
at each stop click a car to trade, carry riders, grow or process goods; at resource fields and hunting grounds
go out on foot to loot and extract. Pay fuel, wages and upkeep at day end and meet the debt check every 5 days.
Rules follow the team design doc; optional experiments (appraisal & fakes, haggling & reputation, loot tiers)
come from our Probably Stolen and Escape from Duckov teardowns. Chinese and English UI.
