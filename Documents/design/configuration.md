# 配置职责与调参规范

## 规则

根目录 `config/` 下按系统聚合相关参数，避免逐个对象拆分JSON，按 gameplay 与 presentation 分类。玩家自身参数集中在玩家配置；其他系统不得再次保存玩家速度、出生、背包或按键的副本。所有物品类型集中在 items.json 的 definitions 数组，新增类型由同一校验入口检查。

`scripts/config/game.ts` 仅校验、引用和组装既有接口，不保存另一份数值，不引入运行时入口或生命周期；引擎插件仍只由 `forge.json` 装配。未知字段、缺少配置、重复控制键、无效引用和未登记的JSON必须报错，不能忽略或使用默认值替代。

## 当前文件

| 路径 | 唯一职责 |
| --- | --- |
| `gameplay/player.json` | 玩家物品身份、行走/跑步/转向参数、胶囊碰撞体尺寸与坡阶参数、出生位置、初始背包、随身格、装备槽、基础属性、控制按键与 interactions（拾取范围和丢弃条件） |
| `gameplay/world.json` | 地图边界、地面尺寸、障碍、可行走表面与 gravity |
| `gameplay/drops.json` | 地面物品出生表 |
| `gameplay/items.json` | 全部物品类型的名称、标签、格型、堆叠、载荷、参考价值、容器或枪械能力 |
| `presentation/camera.json` | 相机参数与相机区域混合 |
| `presentation/palette.json` | 全项目体素色板 |
| `presentation/environment.json` | 场景美术布局 |
| `presentation/actor.json` | 人物体素单位、步幅、摆腿与起伏表现；碰撞体尺寸仍由玩家配置维护 |
| `presentation/inventory-ui.json` | 物品网格及界面颜色、滚动条样式 |
| `presentation/item-visuals.json` | 物品图标和世界模型的共用体素外观 |
| `presentation/holding.json` | 持械姿态参数 |
| `presentation/drop-feedback.json` | 地面物品描边与悬停像素标签样式 |

## 玩家属性

`movement`保存移动和转向；`body`保存物理体积与坡阶条件；`spawn`保存初始坐标；`initialBackpack`引用物品类型。`keys.movement`和`keys.inventory`统一保存玩家控制键，必须是受支持且互不冲突的KeyboardEvent.code。

`baseStats.health`沿用现有Item属性值；`reservedStats.maxHealth/stamina/armor`以null明确预留，尚不启动生命、体力、护甲或死亡规则。启用新属性时要补充规则、校验和检查，不能只把null改成数值就默认生效。

本次调参原值保留：移动4、跑步6、胶囊半径0.22/高度1.5、出生[-3.5,3]、初始背包backpack、随身2×3。更改胶囊尺寸不会自动重画美术模型；美术比例由actor配置维护。

## 检查

- `tests/contracts/config.test.ts`对照实际JSON导入清单扫描config，拒绝未登记文件；检查玩家配置的单一来源、字段职责与键位冲突。
- 所有运行时所有者调用实际校验，组装不替代世界、物品、表现域的规则检查。
- 新增/调整配置后运行基础检查、类型检查、测试与反向验证；影响游玩或画面时执行相应真实输入路线并检查截图。
