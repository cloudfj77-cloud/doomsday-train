# 末日列车 · Doomsday Train（可玩原型 v0.1）

**在线试玩：https://cloudfj77-cloud.github.io/doomsday-train/**
（需要支持 WebGPU 的桌面版 Chrome / Edge 113+）

俯视角体素废土风「模拟经营 + 弹幕射击」策略肉鸽的第一个可玩原型。
你用全部家当换来一张「希望线」运行许可，要还清 6000 欠款，成为末日里有名的列车长。

本原型只验证核心循环：**跑线 → 买卖 → 日结 → 还款**。数值取自策划案《二 玩法功能说明》的 0 Tag 首测基准。

## 玩法

| 环节 | 内容 |
|-|-|
| 跑线 | 每天从 3 条候选路线选 1 条（2 或 3 站，富人区 / 贫民区），列车逐站行驶 |
| 买卖 | 每站 3 名客商（每节存储车厢 +1）。供货客可单拿或整批折扣买下；采购客按牌价收货，每客只成交一次 |
| 载客 | 票价 24/段，上车即收全款；到站自动下车；在车乘客到站有 30% 概率想买东西 |
| 日结 | 燃油 = ceil(Σ(6 + 4×车厢数 + 0.5×载荷))，司机工资 3/段，生活维护 2/人；现金可暂时为负 |
| 还款 | 每 5 天偿债大结算，累计应还 600 / 1500 / 2500 / 3600 / 4800 / 6000；不足即失败，可先卖货补缺；可随时提前还款 |
| 整备 | 存储车厢 600（+16 格、每站 +1 客商）、载客车厢 800（+6 座） |
| 其他 | 中英双语（跟随浏览器，可手动切换）、自动存档、刷新可续玩 |

暂未包含（后续版本）：搜打撤战斗、种植 / 加工 / 军队车厢、司机招募、车厢升级、二维格子摆放、多存档位。

## 目录

| 路径 | 说明 |
|-|-|
| `assets/sim/` | 经营核心（纯 TypeScript，可单测）：数据表 `data.ts`、规则 `economy.ts` |
| `assets/voxels.pack.ts` | 体素模型：列车、车厢、站台、废墟、丧尸 |
| `assets/train/train.pack.ts` | 引擎插件：列车跑线、场景滚动、站点进出 |
| `assets/ui/` | 界面与交互；文字唯一来源 `game.ui.i18n.json`（由 `scripts/gen-i18n.py` 生成） |
| `scripts/balance-sim.mts` | 30 天平衡模拟（贪心机器人多种子统计各期通过率） |
| `qa/playthrough.cjs` | 真实点击验收脚本（24 项） |

## 本地运行

基于 [ForgeAX Engine](https://github.com/ForgeaXGame) 0.4.19。

```bash
pnpm install
bash scripts/build.sh                                  # 打包到 dist/
pnpm exec forgeax project preview --port 4195          # http://localhost:4195/
pnpm exec vitest run assets/sim                        # 经营逻辑单测
pnpm exec tsx scripts/balance-sim.mts 300              # 平衡模拟
node qa/playthrough.cjs "http://localhost:4195/?qa=1"  # 验收
```

发布到 GitHub Pages：`BASE=/doomsday-train/ OUT=site bash scripts/build.sh`，再把 `site/` 推到 `gh-pages` 分支。

---

## English

**Play online: https://cloudfj77-cloud.github.io/doomsday-train/** (desktop Chrome / Edge with WebGPU)

A voxel wasteland train-tycoon prototype. Each day pick a route, trade with suppliers and buyers at every stop,
carry paying riders, pay fuel and wages at day end, and meet the cumulative debt check every 5 days.
Clear the 6,000 debt to win. Chinese and English UI.
