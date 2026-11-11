# CODEBUDDY.md · 《末日列车》开发交接

给在新电脑（公司云电脑）上接手的 CodeBuddy。引擎规则、构建 / 资源硬约束见 `AGENTS.md`；装环境、常用命令见 `docs/dev-setup.md`。

## 项目是什么

俯视角体素废土风「列车经营 + 搜打撤」游戏，ForgeAX Engine 0.4.19（TypeScript，WebGPU）。玩家还清 6000 欠款即胜利。

- 策划依据：飞书表格《00 功能文档总表｜末日列车》https://fcnx9i8w726h.feishu.cn/sheets/UtYWsRxSwhEicKtv0RXc4rU2ncT 。总表只是目录，正文在它链接的 11 个分支表（01 总体玩法 … 11 费用结算），要逐个读。
- 文档提炼稿（数值带出处、缺口清单）、拆解原始产物、设计进度在**私有仓库** `cloudfj77-cloud/doomsday-train-workspace`（含商业游戏反编译内容，只能内部研究，不得放进本公开仓库或游戏里）。
- 设计方案：`docs/v0.2-plan.md`；拆解总览：`docs/teardowns/index.html`。

## 当前状态（2026-11-11）

| 分支 | 内容 | 在线 |
|-|-|-|
| `demo/v0.2` | 团队文档版：车厢点击经营、二维格子仓库、司机与跨日续线、载客、种植加工、资源场 / 猎场 Canvas 搜打撤；实验玩法（鉴定掺假、议价声望、品质容器，标题页可关） | https://cloudfj77-cloud.github.io/doomsday-train/v0.2/ |
| `main` | v0.1 + 队友 SL_Skuy 的 3D 站点探索（角色控制器、探索镜头、进屋隐藏屋顶），尚无战斗 / 搜刮 | 未发布 |
| `gh-pages` | 根目录 v0.1，`v0.2/` 子目录 v0.2 | — |

`demo/v0.2` 与 `main` 都改了 `assets/sim/economy.ts`、`assets/ui/controller.ts`、`assets/shared/protocol.ts`，**还没合并**。

## 待老板拍板（不要自行决定）

1. 合并方式：建议以 v0.2 为底，把队友的 3D 行走 / 屋顶隐藏接进资源场探索，替换 2D 俯视画面。
2. 后续补什么（已向老板提议，等确认）：还款日必来收购商、罪证延迟账本、市场事件、物品统一属性与护甲公式；参照拆解美术风格**自制**顾客半身立绘、分层站点场景、统一物品图标（不得用原素材）。
3. 平衡：贪心机器人只经营 300 局，第 5 天还款通过 23%（实验玩法 28%），第 10 天起 0%。文档自称数值未实测，未擅自调参。
4. 文档缺口的暂定处理：基础 3 名客商归火车头、存储车厢接待新增名额；富人区有整备、贫民区无；乘客购物基准 B=120；奢侈品未接入。

## 工作方式（老板的长期要求）

- 每推进一步先说整体计划，再简要汇报进展；阶段性产出（截图、预览、报告）随时发出来，不阻塞继续做。
- 需求有歧义、范围要缩小或有取舍时先问，不要自己定。
- 中英双语：文字全走 `scripts/gen-i18n.py` 生成的语言表，跟随浏览器语言并可手动切换；英文不得截断重叠；每个游戏要有英文封面。
- 交付标准：`bash scripts/build.sh` 构建后，在 `forgeax project preview` 上用真实鼠标键盘验收（`qa/v02.cjs`）才算完成；测完、汇报前关掉 preview 等后台服务。
- 回复用中文，结论先行，不写空话。

## 已踩过的坑

- 构建必须走 `scripts/build.sh`（带 `--html-head web/head.html`），直接 `forgeax project build` 会丢本地化加载画面。
- preview 只监听 localhost（IPv6），用 `http://localhost:4195/`，不要用 127.0.0.1。
- UI 导入器限制：不支持 `outline: dashed`、`text-overflow`；`data-ui-part` 须唯一；HUD 可点元素要 `pointer-events: auto`。
- 语言表禁止键名 `prototype` / `constructor` / `__proto__`（报错不指明哪个键）。
- 无头 Chrome 软件渲染只有几帧每秒：游戏循环已改固定步长子步；验收按状态或墙钟等待，不要假设帧率。
- 验收读状态用 `?qa=1`（`window.__doomsday`）；`&seed=3` 固定开局，首日有资源场路线。
- macOS 自带 bash 3.2：空数组展开要写 `${arr[@]+"${arr[@]}"}`（已修在 build.sh）。
- 写飞书 / 企业微信等外部文档前先确认；读飞书表格用 `+workbook-info` 再逐表 `+csv-get`。
