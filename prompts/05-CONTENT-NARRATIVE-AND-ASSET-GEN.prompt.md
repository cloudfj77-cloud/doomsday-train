# Prompt 05 · 阶段三执行 Prompt：跨昼夜叙事线、物品图鉴与 AI 资产生成模板库

```xml
<task_prompt phase="3_content_narrative_assets">
## 一、跨昼夜融合剧情线（把两包经典 NPC 编织进同一条因果链）

每条剧情线必须要求玩家**白天在柜台做人情/道德抉择，夜里在 3D 封锁区找关键任务物**：

1. **D2 起 · 乔治奥的“飞向蛋星”火箭梦**（致敬 PS 乔治奥 + Duckov 蛋星过场）：
   - 下层区最狂热的发明鸭乔治奥来借 `300 cr` 并求购「J-Lab 推进器喷嘴」（可在码头暗仓/农场镇搜刮获得）。若借钱并供货，7 天后他真的造出破烂火箭并带着 **4 倍回报 (`1,200 cr` + 传说脉冲枪蓝图)** 回来报恩！
2. **D4 起 · 铁喙伦警官的止痛药深渊**（致敬 PS 伦警官 + Duckov 医疗物资任务）：
   - 正直的巡逻警鸭在零号街区负伤，来求购阿司匹林/急救包，随后逐渐索要违禁强效镇痛剂。给他真药能让他成为你在治安部最铁的内应（提前抹除 50 点证据）；给他贴假标的安慰剂助他戒断，还是用强效违禁药控制他腐败？由玩家决定。
3. **D6 起 · 老吴与乌鸦“黑子”的加密软盘**（致敬 Duckov 老吴/J-Lab 剧情 + PS 塞尔皮科三阵营线）：
   - 情报客老吴委托你夜袭潜入码头海关与 J-Lab 夺取「加密监控软盘」。用后屋解序仪破译后，揭露铁喙高层与乌鸦黑市联手制造封锁区灾变的黑幕——你可以把软盘**高价卖给乌鸦黑市**、**交给红蹼革命军引爆起义**，或**上交廉政督察塞尔皮科换取治安勋章与免死金牌**！

---

## 二、如果后续接入多模态生图/生 3D/生音效工作台的标准 Prompt 模板库

当使用造化素材库或 AI 生成管线替换/扩充资产时，统一采用以下风格锚定 Prompt，确保视觉与听觉 100% 统一：

### 1. 角色半身立绘生成 Prompt 模板（黑店柜台顾客）
> `2D cyberpunk pixel-vector hybrid half-body portrait, anthropomorphic duck character in dystopian bunker street style, [FACTION_ATTIRE: worn scavenger poncho / luxury gold-trimmed coat / riot police tactical helmet / black crow-masked trenchcoat], expressive eyes and orange duck beak, warm tungsten rim light from left and cyan neon backlight from right, crisp clean silhouette, dark slate background #141822, game dialogue portrait, 288x211 aspect ratio, no text`

### 2. 3D 俯视战区道具与枪械模型生成 Prompt 模板（搜打撤战场）
> ` Stylized low-poly 3D game asset, [ASSET_NAME: weathered AK74U assault rifle / rusted J-Lab bio-container / tactical LV4 kevlar vest], ~600-1500 triangles, hand-painted desaturated realistic PBR texture with subtle edge wear, readable silhouette from 55-degree top-down isometric camera, miniature tabletop diorama aesthetic, clean topology, centered origin`

### 3. 物品网格图标生成 Prompt 模板（背包与展柜共用）
> `Isometric 3D-rendered game inventory icon of [ITEM_NAME], soft top-left studio key light, subtle contact drop shadow, crisp dark 2px outline, transparent background, 256x256px, readable at 64x64px, Escape from Duckov x cyberpunk pawnshop aesthetic`

### 4. 程序化音效设计矩阵（WebAudio 实时合成参数规范）
- `验货揭穿破绽`：双音下行方波 + 高通金属瞬态（`880Hz → 440Hz, 0.14s`），干脆利落；
- `贴假标压印`：低频机械气缸咔哒声（白噪包络 `0.06s` + `140Hz` 正弦撞击）；
- `议价成功收银机`：清脆双泛音铃声（`1318Hz + 1760Hz` 指数衰减 `0.35s`）；
- `搜刮读条爬升`：随稀有度 `0~4` 逐级抬升基频（`320Hz + rarity * 110Hz`），金色传说揭晓时触发大三和弦琶音闪光；
- `枪械开火与爆头`：短促带通噪声冲击 + 低频胸腔震动，爆头额外叠加 `2100Hz` 钢盔清脆叮当声！
</task_prompt>
```
