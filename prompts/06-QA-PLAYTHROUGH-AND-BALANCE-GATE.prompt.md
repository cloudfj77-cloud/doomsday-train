# Prompt 06 · 阶段四执行 Prompt：数值防崩自检、原版十大 Bug 规避与交付验收 Gate

```xml
<task_prompt phase="4_qa_and_delivery_gate">
## 一、原版逆向拆解发现的历史 Bug 强制规避清单（精准排雷）

根据拆解报告（《可能是偷的》§10 与《逃离鸭科夫》§2/§8），我们的融合实现必须从根源杜绝以下原版缺陷：

1. **禁止开局费用修正被首夜重算覆盖**：原版 `start*CostBonus` 在第一晚打烊重算账单时被直接冲掉；本工程必须将难度/职业修正作为无状态纯函数乘数参与每次日结计算。
2. **禁止派系声望 Perk 跨界串台**：原版“好评如潮 +25% 预算”误对全体顾客生效；本工程每个声望阈值 Perk 必须显式校验 `client.faction === perk.faction`。
3. **禁止按揭周数与速通成就死锁**：原版第 50 天成就在 8 周按揭下数学不可达；本工程按揭周期设定为紧凑 6 周，确保按揭流与现金流两条路线均可顺畅通关。
4. **禁止升级树无脑全买导致后期乏味**：本工程黑店升级与夜袭战区钥匙卡深度绑定，每晚只有一次夜袭机会且消耗体力/次日客流，迫使玩家做“走私炼金流 vs 军火改装流 vs 情报破译流”的策略取舍。
5. **禁止无头浏览器/资源泄漏与冗余打包**：严禁把未引用的测试大文件打入 `dist/`，所有 ECS 系统与 DOM 事件监听必须在 `ctx.effect` 中配对 `yield` 释放。

---

## 二、经济与战斗数值平衡断言（单元测试必测项）

编写自动化测试（`vitest`）验证以下核心不变量：
1. **双值欺诈闭环**：同一件 `actualGrade = -1`（瑕疵）、`claimedGrade = +2`（军规）的物品，`exposed = false` 时售价必须高于 `exposed = true` 时售价；验货揭穿后向卖家收购价必须精确下降。
2. **护甲穿透单调性**：在 `2 / (clamp(A - P, 0, 999) + 2)` 公式下，提高穿甲 `P` 必须严格不降低最终伤害；当 `P >= A` 时减伤倍率必须精确回归 `1.0`（无减伤）。
3. **暗格绝对安全**：当 `item.location === 'secret_stash'` 时，无论 `strict` 盘查员盘查多少次、赃物热度是否 `100`，被查获列表长度必须为 `0`。
4. **周日流动性兜底**：第 7、14、21 天生成的顾客队列中，收租官之前必须至少存在 1 名 `intent === 'WHOLESALE_BUYER'` 的现金收购商。
5. **夜袭撤离与阵亡结算**：成功撤离必须将 `raid_backpack` 与 `safe_box` 全部转入 `shop_stock`；阵亡则清空 `raid_backpack` 但 100% 保留 `safe_box` 内物品。

---

## 三、交付前最终 Gate（按顺序全部跑绿）

1. `pnpm typecheck` —— 零 TypeScript 错误；
2. `pnpm test` —— 核心公式、状态流转与双核闭环单测全部通过；
3. `pnpm exec forgeax project check --json` & `pnpm exec forgeax project build --json` —— 资产与构建校验通过；
4. 检查 `.game-builder/project.json` 显式维护 `"recommended_capabilities": ["storage"]`；
5. 调用造化插件 `game_builder_preview_reload` + `game_builder_preview_status` 确认最新构建已在本地 PreviewHost 就绪且无运行时错误，将干净的 `previewUrl` 交付给用户。
</task_prompt>
```
