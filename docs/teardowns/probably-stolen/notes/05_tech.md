# 05 · 技术架构拆解 —《Probably Stolen - Cyberpunk Shopkeeper Sim》

> 仅供学习研究。分析对象：Unity 2021.3.45f2 / Mono / Windows x64 版本（游戏内 `PlayerStore.VERSION = "VERSION 100-Rev12"`，对应商店版本 1.0.0.6，已包含 mod 安全更新）。
> 资料来源：`Probably Stolen_Data/Managed/*.dll` 清单与字符串、`RuntimeInitializeOnLoads.json`、`StreamingAssets/aa`，以及反编译出的 `~/ps_work/src/`（944 个 .cs，约 24.5 万行）。
> 文中 `文件:行号` 都指 `~/ps_work/src/` 下的文件。

---

## 0. 一页结论

| 维度 | 结论 |
|---|---|
| 工程血统 | 内部代号 **`dse`**（Harmony ID `dse.steamguard`、自编译的 `DseCecil.dll`、`StarphaseTools.Core.pdb` 路径里的 `GitHub\dse`）。代码里还留着一整套**太空/飞船/突袭/回合制战斗原型**（`GameMaster`→`ShipManager/RaidManager/TurnManager`，30 个 `Space*Trigger`），商店模拟是在这套"物品/背包/像素窗口"框架上长出来的。 |
| 内容生产方式 | **代码即数据**：约 750 个物品（`*Directory.cs` 里的 `Add("id", () => Factory())`）、约 280 种顾客（`StoreClientListDict` 里 id→工厂）、商店事件、网络升级、开局特长全部用 C# 硬编码；只有掉落表用 ScriptableObject + RNGNeeds，文本放 Unity Localization 表。 |
| 物品模型 | `GameItem` = 字符串键值的 `TagSystem`（运行时状态）+ 约 50 个 `Func/Action` 委托钩子（行为）+ `ItemFeature`（隐藏属性/瑕疵）+ 由精灵 alpha 自动生成的网格形状。行为靠 100 个 `*Helper.InitXxx(item)` 往委托里挂函数来"组装"。 |
| UI | 新旧三代并存：① 代码驱动的整数像素 UI（`PixelElement`/`PixelWindow`/`TreeNodeRender`）用于背包；② 传统场景式 `*UIManager`（47 个）；③ "UI3"：编辑器菜单 `Build > Generate … Prefab` 用代码生成预制体，配 `*Style`（静态色板）+ `*Frame/*Skin`（引用槽）；另有 Fluent 风格的 `CustomUIBuilder` 给 mod、调试面板和设置界面用。外加一个后来补上去的 `LetterboxManager`，负责 16:9 黑边。 |
| 存档 | Easy Save 3 写 JSON（不加密）：每个槽一个 `save_N.es3`（`playerStore`、`storeStation` 两个键，共约 160 个 `[ES3Serializable]` 字段）。背包是用 `JsonUtility` 先序列化成字符串、再塞进 ES3 的"JSON 套 JSON"。按天轮转历史快照，有隔离、修复、IO 重试。 |
| Mod | 本作最"重工程"的子系统（约 1 万行）：manifest.xml + 必须有 IMod DLL + Harmony 自动 PatchAll；用自带的 Cecil 做 **IL 静态扫描**（能力声明 + 硬拒绝两类规则）；SHA-256 指纹 + 本机信任记录 + **RSA 签名的远程信任列表/全局停用开关（kill switch）**；运行时用 Harmony 拦截 Steam 账号类 API 和 `Process.Start`，触发后直接熔断该 mod；数据型 mod 支持 LootTables JSON、CSV 本地化、贴图、音频、纸张书籍 JSON。 |
| 本地化 | Unity Localization（Addressables 打包）有 14 张表、约 6.9k 条/语言、13 种语言；自建 `LocHelper` 分表访问器、`GetLocalizedOrDefault` 英文兜底、SmartString 自动打标、`[g:…]`/`[r:…]` 性别/敬语标记；CJK 字体按字符区间逐串切换。 |
| 调试 | `dev.txt`/`1dev.txt` 标记文件控制开关；F9 DebugPanel（含"Marketing"一组按钮，专门给预告片/截图摆场景）；F8 Quantum Console（30 条 `[Command]`）；后台线程 StallWatchdog 卡顿面包屑；游戏内 WYSIWYG 的 PaperEditor；Unity Analytics 埋了 19 个事件，但发行版用 `OVERWRITE_DISABLE_ANALYTICS = true` 关掉了。 |

---

## 1. 第三方插件栈

### 1.1 Managed 目录清单与实际用途

| 程序集 | 插件 | 在 Assembly-CSharp 中的用途 / 用量 | 备注 |
|---|---|---|---|
| `Assembly-CSharp-firstpass.dll` | **Easy Save 3**（ES3*，约 156 个 ES3Type）+ **EPOOutline**（Easy Performant Outline）+ DOTween 的模块文件 | ES3：12 个文件，163 次调用（`ES3.Save` 50 / `KeyExists` 49 / `Load` 48 …），另有 23 个文件用了 `[ES3Serializable]`。EPO：`Interactable` 上挂 `[RequireComponent(typeof(Outlinable))]` 做鼠标悬停描边，`EmporiumEntry.outliner`，`LetterboxManager` 开关 Outliner | Plugins 目录里的插件编进 firstpass；`RuntimeInitializeOnLoads` 里有 `ES3ReferenceMgrBase.Init` 和 `OutlineEffect.InitMaterials` |
| `DOTween.dll` / `DOTweenPro.dll` / `DemiLib.dll` | DOTween Pro | 只在 `StoreClientMono` / `GenericClientMono` 中使用 `DOTweenPath`（顾客沿路径走进走出：`DOPlay/DOPlayBackwards/DORewind`） | 用量很轻，主要用的是 Pro 的可视化路径组件 |
| `Febucci.TextAnimator.*` | Text Animator for Unity | `TypewriterByCharacter`：对话气泡 `TextBubble`、`StoreUIManager` 里的顾客/玩家打字机、`GameOverUIManager`；设置项里有 `enableTypewriterEffect` | 随包带了 `Febucci.TextAnimator.Demo.Runtime.dll`，源码里也留着 `Febucci.UI.Examples/TAnimSoundWriter.cs`（Demo 代码被打进了正式包） |
| `QFSW.QC*`（9 个） | **Quantum Console** | 13 个文件，30 条 `[Command("…")]` 调试命令（afterhour-*、appraisal-*、loot-*、mod-*、paper-*、gun-*、loc-*、tutorial-*、serpico-test、client-faces）。`DevConsoleStripper` 在非开发模式下销毁所有 `QuantumConsole` 实例 | 同样带了 `QFSW.QC.Demo.dll` |
| `RNGNeeds.dll` + `StarphaseTools.Core.dll` | RNGNeeds（Starphase Lab 出品，StarphaseTools 是它的依赖） | `LootTable : ScriptableObject { ProbabilityList<string> table }`、`TableGroup { ProbabilityList<LootTable> }`；运行时 `LootRegistry` 把 ProbabilityList 摊平成自己的条目列表，方便 mod 打补丁 | 设计师在 Inspector 里调权重 |
| `SingularityGroup.HotReload.Runtime*.dll` + `UnityEngine.HotReloadModule` | Hot Reload for Unity | Assembly-CSharp 里**零引用**，也不在 RuntimeInitializeOnLoads 中 | 开发期工具，运行时 DLL 被一起打进了发行包（无害但冗余） |
| `Newtonsoft.Json.dll` | Json.NET | 只在 Paper 系统中使用（`PaperJson` 及 `Paper*Data` 的 `[JsonProperty]`），因为书籍 JSON 需要可选字段、多态、漂亮格式 | 其他地方（存档节点、Mod loot、ModTrust、ModConfig）都用 `JsonUtility` |
| `com.rlabrecque.steamworks.net.dll` + `steam_api64.dll` | Steamworks.NET | `SteamManager`（标准模板）、`AchievementTracker`（约 42 个成就，SetAchievement/SetStat/StoreStats）、`LocHelper`（`SteamApps.GetCurrentGameLanguage()` 自动选语言）、`ModWorkshop`（订阅项枚举、下载、退订、UGC 详情查询）、`ModSteamAuth`（代 mod 申请 Auth Session Ticket）、`ModSteamGuard/ModSteamRules/ModScanner`（安全规则） | — |
| `Unity.Addressables` / `Unity.ResourceManager` / `Unity.ScriptableBuildPipeline` | Addressables | 代码里零直接引用；`StreamingAssets/aa/StandaloneWindows64/` 下全是 `localization-*` bundle | **只作为 Localization 包的底层依赖**，其余资源走 `Resources` + 场景引用 |
| `Unity.Localization.dll` | Unity Localization | 30 个文件；13 种语言 bundle（en/zh/ja/ko/de/fr/it/es/pl/pt-BR/ru 等） | 见 §6 |
| `0Harmony.dll`（2.4 MB） | HarmonyX/Harmony 2 | `ModLoader` 为每个 mod 创建 `new Harmony("mod."+ID).PatchAll(asm)`；`ModSteamGuard` 自己用 `dse.steamguard` 打前缀补丁 | 见 §5 |
| `DseCecil.dll` | Mono.Cecil（自编译，程序集名改为 DseCecil） | `ModScanner` 读取 mod IL（`using DseCecil; using DseCecil.Cil;`） | 改名是为了**不和 mod 自带的 Mono.Cecil 撞名**；扫描器也会把 mod 自带 Harmony/MonoMod/Cecil 判为 `bundled-framework` 硬拒绝 |
| `Unity.Services.Analytics` + `Unity.Services.Core.*` | UGS Analytics | `EmporiumAnalytics` + 19 个 `*Event : Unity.Services.Analytics.Event` | 发行版已关闭（见 §7） |
| `Unity.2D.PixelPerfect` | Pixel Perfect Camera | `EmporiumOption.ApplyPixelPerfectCamera`、`LetterboxManager` 出现黑边时临时禁用它、`PixelPerfectHorizontalLayoutGroup` | — |
| `Unity.TextMeshPro` | TMP | 136 个文件 `using TMPro` | 所有文字都走 TMP |
| `Unity.Recorder*` | Unity Recorder | 代码零引用 | 推测用于录预告片（与 DebugPanel 的 Marketing 分组一起看） |
| `Unity.VisualScripting.*` | Visual Scripting | 零引用 | 默认包没删 |
| `Unity.2D.Animation/IK/SpriteShape/Tilemap.Extras`、`Unity.Burst`、`Unity.Collections`、`Unity.Mathematics` | 2D 包全家桶 | 基本没用（Mathematics 只被 StarphaseTools 引用） | 2D 模板默认依赖 |
| `Unity.Postprocessing.Runtime` | PPv2 | 代码零引用（可能在场景里挂着） | — |
| `CsvExport.dll`（Jitbit） | CSV 导出 | Assembly-CSharp 和 firstpass 都不引用 | 疑似编辑器期导出本地化或数据用，遗留在包里 |
| 未发现 | `ES3` 独立 dll | ES3 以源码形式在 firstpass 中 | — |

### 1.2 插件选型观察
- **全部是 Asset Store 上的成熟付费/免费组件**，覆盖存档、补间、打字机、描边、控制台、加权随机，团队精力集中在玩法代码上。
- **用量两极分化**：ES3、TMP、Localization、Steamworks 是深度依赖；DOTween、Febucci、EPO 只是点缀；Hot Reload、Recorder、VisualScripting、CsvExport、两个 Demo DLL 属于"装了就没删"。这对 mod 安全也有影响：`CollectGameAssemblyNames()` 会把 Managed 下所有 DLL 名都当成"游戏程序集"，mod 引用它们不会被判为 `unlisted-reference`。
- 序列化有三套并存：ES3（主存档/设置）、`JsonUtility`（背包节点、mod 数据、信任记录）、Newtonsoft（Paper）。

---

## 2. 代码组织模式

### 2.1 全局入口与单例

- **全部在全局命名空间**（除了一个 `Directory.Item.Behaviour.Target` 命名空间），没有 asmdef 分层，944 个文件编进同一个 Assembly-CSharp。
- 单例写法很杂：`public static X current {get; private set;}`（28 个文件，老框架风格，比如 `GameMaster.current`、`OverlayHandler.current`、`RenderHandler.current`、`UISettings.current`）和 `public static X Instance`（86 个文件，新代码风格）。
- 文件名后缀约定：`*Manager`（72）、`*UIManager`（47）、`*Helper`（132，其中约 100 个是"物品行为 Helper"）、`*Directory`（38）、`*List`（顾客/事件/升级的定义表）、静态类 106 个。
- **静态访问点的热度**（全代码引用次数）：

| 访问点 | 次数 | 说明 |
|---|---|---|
| `PlayerStore.Instance` | 3773（197 个文件） | 4213 行的超级对象（God Object），保存几乎全部局内状态 |
| `EmporiumEntry.Instance` | 1137 | 商店场景入口，持有各个库存窗口（主背包、后柜台、展示柜、垃圾桶、贩卖机……） |
| `StoreStation.Instance` | 480 | 站点/区域状态 |
| `GameMaster.current` | 384 | **原型遗留**：其中 `shipManager` 195 次（`nextPOI`、`isTransit`、`ship`、能量电池……），`worldTags` 74 次 |
| `DialogUIManager.Instance` | 322 | |
| `StoreUIManager.Instance` | 286 | |
| `CustomUIManager.Instance` | 197 | |

- `GameMaster.Start()` 每局都会 new 出 `ShipScene/ShipManager/RaidScene/RaidManager/MenuScene/MenuManager/TurnManager`，`Update()` 每帧对三个 Scene 调用 `Validate`，说明原型框架仍在主循环里。
- 启动钩子（`RuntimeInitializeOnLoads.json`）：`SaveFiles.RepairSettingsFileAtStartup`、`LocSmartStringHelper.Init`、`StoreUISounds.Boot`、`CultureBootstrap.PinInvariantCulture`（把全线程 Culture 固定为 Invariant，避免小数点/日期格式在德语、俄语等系统下出错）、`SpriteAnimationManager.Bootstrap`、`LetterboxManager.Bootstrap`（运行时自动创建 DontDestroyOnLoad 对象）、`DevConsoleStripper.Init`、`StallWatchdog.Init`、`VerboseLog.ResetUnityLoggerFilter`、`SteamManager.InitOnPlayMode`。
- 一个"仪式性"的写法：**29 个实例方法 `private void Awake()` 上标了 `[RuntimeInitializeOnLoadMethod(SubsystemRegistration)]`**（`Directory<T>`、`ItemBehaviourManager`、`EventTriggerManager`、`GameMaster`、`CustomEventHandler` 等）。Unity 只会调用静态方法，所以这个特性完全没起作用，Awake 照常由生命周期调用。看起来是早期复制粘贴留下的。

### 2.2 "Directory" 数据定义模式

核心是 3 个类：

```
interface DirectoryEntry { string identifier; string identifierName; }          // DirectoryEntry.cs
abstract class Directory<T> : MonoBehaviour where T : DirectoryEntry             // Directory.cs
    Dictionary<string, Func<T>> factoryDictionary;
    Awake(): DirectoryMaster.AddDirectory(this) → InitDirectory()
    Create(id) → factory() 并回填 identifier / identifierName
    Add/Set/Has/Remove（字符串或 enum 键，enum 用 TagSystem.EnumToString 转换）
static class DirectoryMaster                                                     // DirectoryMaster.cs
    Dictionary<Type, List<object>> directories   // 按 T 分桶，同一 T 可有多个 Directory 子类
    Create<T>(id) 依次询问各 Directory；Item(id)/CharacterItem(id) 找不到时返回占位物品
```

- **每个 `XxxDirectory` 是挂在场景里的 MonoBehaviour**，在 `Awake` 里把自己注册到 `DirectoryMaster`，然后在 `InitDirectory()` 里批量 `Add("id", () => Method())`。
- `T` 有 4 种：`GameItem`（`ItemDirectory` 的 30 个子类）、`GameCharacterItem`、`CombatAbility`、`CombatAbilityEffect`（后两种属于原型遗留）。
- 直接 `Add("…")` 注册的物品 id 约 **749 条**，按规模排序：`MiscItemDirectory`（124 条 / 1548 行）、`GunModDirectory`（109）、`UnusedDirectory`（85 条 / 2734 行，**已经弃用的内容仍然注册着**）、`ModuleDirectory`（66）、`FoodItemDirectory`（40）、`MedsItemDirectory`（39）、`HusbandryDirectory`（38）、`AmenitiesItemDirectory`（30）……
- 工厂函数的典型写法（`MiscItemDirectory.cs:144`）：

```csharp
private GameItem EnergyCredit() {
    GameItem g = Empty().SetSpriteAndShape("Items/items_tool", "battery100"); // 精灵 + 自动形状
    g.name = LocHelper.GetLocalizedItem("item_energy_credit_name");            // 创建时就写入本地化文本
    g.EnableManufacturer("eminence");
    g.flavorText = …; g.shortDescription = …;
    PowerHelper.InitPowerSourceItem(g, 100, 10);   // 通过 Helper 挂上 Tag 和委托 = 组装行为
    g.unitBaseValue = 10L;
    return g;
}
```

- **存档与 Directory 的关系**：存档只存 `identifier` 加状态，读档时 `DirectoryMaster.Item(identifier)` 重新跑一遍工厂，把所有委托和行为重建出来，再覆盖 Tag、形状、名字、价值等字段。所以**行为不进存档，只有数据进存档**（§4）。
- **缺失兜底**：id 找不到时返回 `GameItemElement(identifier, "Items/itematlas", "unknown")`，并且**保留原 id**。卸载 mod 后，相关物品会以"未知物品"的形式留在存档里，下次重新启用 mod 还能恢复。
- **mod 接入**：场景里放了一个 `ModItemDirectory : ItemDirectory`，它的 `InitDirectory()` 只做一件事 `ModHook.FireOnModItemDirectoryInit(this)`，mod 在这个回调里 `dir.Add("mymod_item", …)`。`Set()` 可以覆盖原版条目。
- 小问题：`Directory.Add()` 会先调用 `DirectoryMaster.Remove<T>(identifier)`，日志写"ignoring new entry"，实际却是删掉旧条目、加入新条目。语义和日志对不上。
- **另外两套并行的注册表**（说明规范没有统一）：
  - 顾客：`StoreClientListDict.storeClientDict`，是一个静态 `Dictionary<string, Func<StoreClient>>`（约 276 个键，387 个 `public static StoreClient CreateXxx()` 工厂，分散在 30 个 `StoreClientList*.cs` 中，共约 2 万行）。
  - 商店事件：`StoreEventBlueprint { Func<StoreEvent> storeEventFunc; int odd; string identifier; }`，即工厂加权重。
  - 网络升级、开局特长、难度、Afterhour 地点和事件也各有一个 `*Registry`/`*List`。mod 通过 `ModHelper.AddXxx(modId, …)` 往这些表里加条目（§5.8）。

### 2.3 GameItem：Tag 状态 + 委托钩子 + Helper 组装 + ItemFeature

`GameItem : GameItemFunc, GraphNodeStorage, DirectoryEntry, CombatAbility`（2597 行），由四层组成：

1. **`TagSystem state / modifiedState`**（`TagSystem.cs`）：`Dictionary<string, TagState>`，`TagState` 同时带有 `valueEnabled/Int/Float/Long/Double/Bool/String` 七个槽位，相当于一个通用属性包。全代码大约有 **845 个不同的字面量 tag 键**，调用量：`ModifyTag` 1416、`IsTag` 1348、`GetTagReadonly` 1342、`EnableTag` 745。enum 也可以当键（`EnumToString`）。`GameMaster.worldTags` 是全局 Tag。
   - 好处是任何属性都能加，而且天然可序列化（存档只写非默认值，见 `SaveItemNode.OnBeforeSerialize`）。
   - 代价是没有类型约束、靠字符串约定（`"power_source_item_energy"`），常量分散在各个 Helper 里。
2. **`GameItemFunc` 的约 50 个委托属性**（`GameItemFunc.cs`），按 "may / can / on" 三段式命名：
   - 容器：`mayInventoryAddItemFunc`、`maxSlotAddItemFunc`、`onSlotAddItemFunc`、`slotPriorityFunc` ……
   - 交互：`mayItemTargetItemFunc / canItemTargetItemFunc / onItemTargetItemFunc`（拖 A 到 B），`ThisTarget`、`TargetThis`、`SlotItemTarget` 等多种方向
   - 激活/开关：`mayActivateSlotItemFunc` / `onActivateSlotItemFunc` / `onToggleSlotItemFunc`
   - 时间：`onCycleEndEarly/onCycleEnd/onCycleEndLate`（日终三阶段）、`onTurnEndSlotItemFunc`、`onOneSecondItemFunc`
   - 外观/形状：`parentItemShapeModifierForItemFunc`、`getItemSpriteModifierForItemFunc`、`tooltipRichTextFunc`
   - 生命周期：`onLoaded`、`onUnlockedFunc`
   - **多播合成**：`InvokeFuncExtensions.InvokeAllReduce(func, args, reduce)` 遍历委托链，用 `BoolReduceOR/AND/Max/Min` 归并结果。所以多个 Helper 可以往同一个 Func 上 `+=`，最后用 AND 或 OR 求值。这是一个很轻量的"组件化"手段。
3. **Helper 组装**：约 100 个 `*Helper`（`PowerHelper`、`ContrabandHelper`、`DurabilityHelper`、`LiquidContainerHelper`、`AquacultureHelper` 2462 行……）提供 `InitXxx(GameItem, …)`，内部做两件事：`EnableTag/ModifyTag` 写数据，`item.onXxxFunc += …` 挂行为。**物品类型 = 工厂函数里调用了哪些 Init**。这是"组合优于继承"的做法，只是没有用 Unity 组件，而是用委托。
4. **`ItemFeature`**（`ItemFeature.cs`、`ItemFeatureList.cs` 1209 行）：不是"组件"，而是对估值有影响的**隐藏特征/瑕疵**，对应这类游戏"鉴定真伪"的核心玩法。字段包括 `publicDisplay/actualDisplay`（对外宣称和真实情况）、`fakeCondition/realCondition`、`isFeatureExposed/Discovered`、`removedByTool`、`valueModifier`、`ValueStage{Innate,Market,Final}`、`FeatureType{Normal,Event,Special,Temporary,TemporaryBuying/Selling,ClientTemporary}`。分类常量有 `CATEGORY_RUSTED / EXPIRED / GENUINE_CIGARETTE / REPLICA / CHEMICAL_PURITY / WINE_QUALITY / SECOND_HAND …`。字段用 `[ES3Serializable]` 标注，存档时被单独收集到 `PlayerStore.savedItemFeatureList`，再通过 `parentItemUniqueId` 回连到物品。

补充两个巧妙的地方：
- **形状由精灵 alpha 自动生成**（`GameItem.BuildShapeFromSprite`，`GameItem.cs:348`）：`gridSize = 16`，按 `mipLevel = (int)Math.Sqrt(gridSize)` = 4 读取 mip 4 的像素，一个 texel 正好对应一个 16px 格子；alpha>0 的格子算作占用，得到俄罗斯方块式的背包形状。切片没对齐网格时会打橙色警告。（`sqrt` 只在 gridSize 为 4 或 16 时恰好等于 `log2`，属于"凑巧能用"。）这要求贴图开启 Read/Write 和 mipmap。
- **物品图结构**：`GraphNodeStorage { parents; children }`，链路是 `GameItem → contentWindow(PixelWindow) → child(GameInventory) → items`，容器可以无限嵌套（背包里有箱子，箱子里有瓶子）。`GraphUtils.FindAllChildrenType<T>()`、`TryAcceptAllOrDestroy` 等是通用的图操作。存档就是对这张图做 BFS 编码（§4）。

### 2.4 Behaviour 目录（`Directory.Item.Behaviour.Target`）

- `PriorityTrigger`（`PriorityTrigger.cs`）：用 `Assembly.GetExecutingAssembly().GetTypes()` **反射扫描所有非抽象子类**，逐个 `Activator.CreateInstance`，再按 `priority` 降序排序。
- `ItemBehaviourManager` 维护两张表：`ItemUseBehaviour` 和 `ItemTargetBehaviour`。每个行为都有 `Filter / May / Can / On / Tooltip` 五个虚方法，返回 `bool?`，`null` 表示"不表态"，形成一种三态投票机制。
- **实际只有 2 个实现**：`OrganBehavior`（空类）和 `Directory.Item.Behaviour.Target.TestTargetBehaviour`（"用锋利近战武器挠痒痒某角色"的测试代码，随正式版一起发布了）。命名空间是按文件夹 `Directory/Item/Behaviour/Target` 起的，可以看出原计划是"按目录放行为脚本"的插件式架构，**后来被 §2.3 的委托/Helper 模式取代**。
- 因为扫描的是 `GetExecutingAssembly()`，**mod DLL 里的子类不会被发现**，这条扩展路线对 mod 实际上是关闭的。

### 2.5 事件系统：四层并存

| 层 | 类 | 机制 | 用途 |
|---|---|---|---|
| 输入事件 | `CustomEventHandler`、`MultiEventHandler`、`TreeNodeRender`（实现了 17 个 `I*Handler`）、`IPointerDoubleClickHandler`（自定义接口） | 每帧对 `EventSystem.RaycastAll` 取最深的 `TreeNodeRender`，自己实现双击判定（阈值 0.5s，按钮和目标都要一致），`clearDoubleClickOnNextFrame` 静态开关 | 像素 UI 的点击、拖拽、双击、悬停 |
| 原型随机事件 | `EventTrigger : PriorityTrigger`（30 个 `Space*Trigger`、`TraderStation*Trigger`、`Test*EventTrigger`）+ `EventTriggerManager` | 反射发现，`Filter → Trigger → OverrideExistingEvent → GenerateEvent`，写入 `GameMaster.current.shipManager.currentEvent` | 太空航行遭遇事件（原型遗留） |
| 商店事件 | `StoreEvent`/`StoreEventBlueprint`/`StoreEventManager`/`StoreEventActionDict`/`StoreOperationManager`/`StorylineSchedule` | 工厂加权重抽取，带冷却、持续天数，可以挂 `NegociationData` 改价格 | 新闻、市场波动、剧情排期 |
| Mod 钩子 | `ModHook`（44 个 `static event`） | 大多成对出现：`…Early/…Late`（如 `OnShutterOpenedEarly/Late`、`OnGoingSleepEarly/Late`、`OnCreateTooltipEarly/Late`、`OnGenerateCustomerVeryEarly…VeryLate` 五档）。`SafeInvoke` 会**展开委托链、单独 try/catch 每一个订阅者**，一个 mod 出错不影响其他 mod；`SafeInvokeGenerateCustomer` 还会对比调用前后的 `clientStack`，把新增顾客打上来源 mod 标记（`ModSourceHelper.MarkNewClients`） | 对外稳定 API |

此外还有 `LetterboxManager.LayoutChanged`、`ModLoader.OnTrustRefreshed`、`ModTrustList.OnChanged`、`ModWorkshop.OnDetailsUpdated` 这类模块内部的 C# event。**项目没有统一的事件总线**。

### 2.6 TagSystem 小结
- `TagSystem` 同时承担"物品状态""世界状态（`worldTags`）""行为开关（如 `IMPORTANT_TAG`、`MELEE_SHARP_WEAPON_TAG`、`SPEAKS_FORMAL`）"三种职责。
- `TagState` 本身也实现了 `DirectoryEntry`（有 identifier/identifierName），`TagSystem.StringToDisplayString` 负责把 id 转成可读名。
- 存档序列化时用 `SaveItemNode : ISerializationCallbackReceiver` 把字典拆成 `_keys/_values` 两个列表，**过滤掉全为默认值的条目**来压缩体积。

---

## 3. UI 架构

### 3.1 第一代：代码驱动的整数像素 UI（背包/窗口）

- `interface PixelElement`（`PixelElement.cs`）：所有坐标和尺寸都是**整数像素**（`widthPixels/heightPixels`、`XClamp/YClamp`、`ResizePixels`），用 C# 8 默认接口方法实现 `SetXClamp`、`SetRenderingParent`、`ToFront/ToBack` 等。实现类约 30 个：`PixelWindow`、`GameGridInventory`、`GameSlotInventory`、`GameGridScrollableInventory`、`GameItemElement`、`GridPixelElement`、`SpritePixelElement`、`RichTextElement`、`ScrollElement`、`SelectableElement`、`TagElement`，以及一整套 `Machinery*`（按钮、滑条、单选、标签页、进度条、Intake），机器面板就是用它拼出来的。
- **逻辑和渲染分离**：`PixelElement` 是纯 C# 逻辑对象，渲染交给 `TreeNodeRender : MonoBehaviour`。后者从 `RenderHandler.current.renderNodePrefab / renderNodeTextPrefab` 实例化，持有 `Image / RectMask2D / TextMeshProUGUI`，并实现 17 个 EventSystems 接口。`handler`（外框）和 `inner`（内容挂点）分开。
- `PixelWindow : PixelElement, GraphNodeWindow, GraphNodeStorage, MachineryStorageOwner`（1085 行）：窗口本身也是物品图的一个节点（物品的 `contentWindow`），有 `canMove/isLocked/forceRaycast/style`。`WindowsHandler`（144 行）管理 `visibleWindows`，提供 `TryAddWindow/CloseWindow/HideAllMovable/HideAllNonImportantMovable`，并实现 `Cancelable`（ESC 逐层关闭）。存档前会让每个窗口记住自己的位置（`RememberContentWindowPositionIfMine`）。
- 窗口皮肤从 `Resources.LoadAll<Sprite>("Window/background" | "Window/border")` 读取；物品图集按路径 `Resources.LoadAll<Sprite>(atlasPath)` 加载（`Items/items_tool`、`items_misc`、`itematlas` 等十几张图集）。

### 3.2 RichText：分层文本
- `RichTextBuilder`（644 行）以"字符宽度"排版（`maxCharWidth = 100`），`Build()` 输出 `RichText`，其中包含 **5 个并行字符串层**：`nakedText` / `richTextStrikethrough` / `richTextUnderline` / `richTextForeground` / `richTextBackground`，外加推荐宽高和是否截断。用多层 TMP 叠加的方式实现背景色块、下划线、删除线，以及终端风格的 `AddProgressBar`。
- 物品 tooltip 由 `tooltipRichTextFunc` 委托和 `ModHook.OnCreateTooltipEarly/Late` 共同拼出来。
- 遗留痕迹：`AddProgressBarOld`、`AddProgressTest`、`GetLengthOld`，以及只有 3 行的空类 `RichTableBuilder`。

### 3.3 第二代：场景式 `*UIManager`
- 47 个 `*UIManager` 加上若干 `*UI.cs`（如 `StoreUIManager` 1484 行、`BargainUIManager`、`NegociationUIManager`、`DialogUIManager`、`PhoneUIManager`、`MapUIManager` 等）。这是典型的"Inspector 拖引用 + 单例 + SetActive"写法。`OverlayHandler` 是原型时代的总覆盖层。

### 3.4 第三代 "UI3"：代码生成预制体 + Style/Frame
- 报错文本和 Tooltip 透露了做法：`"tabletPrefab is not assigned. Run Build > Generate Tablet UI Prefabs in the store scene."`、`"UI3/Prefab/NegotiationTablet.prefab, from Build > Generate Tablet UI Prefabs"`，以及 `Generate Wild Network UI Prefab`、`Generate Credits/Load Game/New Game Difficulty/Perks/Specialization/Transit UI Prefab`、`Collect Paper UI Assets`。也就是说，**编辑器脚本用代码搭出预制体并保存**，运行时直接实例化。
- 分三层：
  - `*Style`（`TabletStyle`、`TransitStyle`、`WildNetworkStyle`、`NewGameStyle`）是**静态类**，存放设计 token（`INK = #414554`、`SCREEN = #d1c8a5`、各类 Tag 的底色/字色、间距……），生成器和运行时代码共用。
  - `*Frame/*Skin : MonoBehaviour`（`TabletFrame`、`TransitFrame`、`WildNetworkFrame`、`NewGamePanelFrame`、`CustomUIWindowFrame`）只是"引用槽位"（panel/art/frame/closeButton/section/body/scroll/footer…），没有逻辑。
  - `TabletUI`（静态类，504 行）是运行时填充逻辑（`TagSpec`、`FooterRow`），被 `BargainUIManager`、`EndOfDayUIManager`、`NegociationUIManager` 等复用。平板（Tablet）是谈判和日终结算的主要界面载体。
- 好处：预制体可以重复生成、风格统一、方便 diff，避免手工拖拽 UI 带来的不一致。

### 3.5 `CustomUIManager` + `CustomUIBuilder`：运行时 Fluent UI
- `CustomUIManager`（1083 行）持有一组"元素预制体合约"：`windowPrefab/labelPrefab/buttonPrefab/togglePrefab/sliderPrefab/inputFieldPrefab/dropdownPrefab/progressBarPrefab/scrollViewPrefab/tooltipPrefab`，外加一套 `meta*Prefab`（设置菜单风格），以及 UI3 的各窗口预制体。源码注释还提到内部文档 `docs/custom-ui.md`。
- `CustomUIBuilder`（595 行）是**链式构建器**，用 `Stack<RectTransform>` 管理容器嵌套：`SetSize/Center/SetDraggable/OnClose/SetCloseOnEscape`；元素有 `AddLabel/Header/Warning/Separator/Button/IconButton/Image/Toggle/Slider/Input/Dropdown/ProgressBar/Space/FlexibleSpace/WithTooltip`；布局有 `BeginRow/Column/Panel/Grid/Scroll/ScrollFlexible/Tabs/Tab … End()`；还有 `UseMetaPrefabs()` 和 `UseMachineryStyle()` 切换外观；`Last` 指向最近一个元素，`id` 用于之后 `handle.Get("id")` 查找。
- 使用方（25 个文件）：Mod 管理与启用确认/拒绝/警告窗口、ModConfigWindow、游戏选项 `GameOptionUIManager`（mod 可以往里加选项）、`CustomUIDebugPanel/Windows/ItemSpawner`、`PaperEditorWindow/SpritePicker`、`AppraisalItemsWindow`、`RunSummaryUI`、Afterhour 地点面板（`buildPanel: Action<CustomUIBuilder>` 直接暴露给 mod）、`MapUIManager`、部分机器。
- 结论：**是代码构建 UI**，属于 IMGUI 风格的 API 加 uGUI 预制体实现，是给 mod 和工具用的"保留模式即时 API"。

### 3.6 像素完美与黑边：`LetterboxManager`（569 行）
- 通过 `[RuntimeInitializeOnLoadMethod(AfterSceneLoad)] Bootstrap()` 自动创建并 DontDestroyOnLoad，**不依赖任何场景配置**，是一个"外挂式"补丁。
- `ComputeGameArea(w,h)` 固定 16:9 游戏区并计算偏移；屏幕不是 16:9 时启用自建的 `uiCamera`（depth 100）、`barsCamera`（150，`LetterboxBars` 画黑边）和 `dockCamera/dockCanvas`（200，黑边区域的 Dock UI，比如超宽屏两侧的面板）。
- 每 30 帧"扫描"一次：`RefreshWorldCameraList()` 加上 `ConvertCanvases()`（把 Overlay Canvas 改成相机空间渲染到 uiCamera），`LateUpdate` 里 `MaintainWorldCameras()` 调整世界相机的 viewport 和正交尺寸（`Get/SetIntendedOrthographicSize`）。
- 和 `PixelPerfectCamera` 的冲突：出现黑边时**禁用 PPC**，同时处理 EPO 的 `Outliner`；黑边消失后 `RestoreSuppressedPixelPerfect()` 再调用 `EmporiumOption.ApplyPixelPerfectCamera()` 恢复。像素完美本身是玩家可选的设置项。
- `RunNextUpdate(Action)` 提供一个延迟到下一帧执行的队列，`LayoutChanged` 事件通知 UI 重新布局。

---

## 4. 存档系统

### 4.1 文件布局（`Application.persistentDataPath`）

| 文件 | 内容 |
|---|---|
| ES3 默认文件（`new ES3Settings().FullPath`） | 全局设置：`locale`、`musicVolume/masterVolume/dialogVolume`、`resolution/fullscreenMode/enableFpsCap`、`dialogSpeed/enableTypewriterEffect/dialogBubbleScale`、一大批 QoL 开关（`singleClickInteract`、`instantCallNextCustomer`、`moveBoughtItemsToInventory`、`newspaperToTrash`、`disablePopCultureClients` …）、`consentAnalytics`、`totalRunCount`、`welcomeVersion`，以及 **mod 相关的 `modTrust`、`modOrder`、`workshopOwners`、`modUiWarningAcceptedVersion`** |
| `saves_index.es3` | 槽位索引：`preview_<slot>`（`SavePreviewData`，存档列表里显示用）、`history_<slot>`（历史快照列表）、`totalPlayTimeSeconds` 等 |
| `save_<N>.es3` | 槽位主存档：两个键 `playerStore`（`PlayerStore`，154 个 `[ES3Serializable]` 字段）、`storeStation`（`StoreStation`，8 个字段） |
| `save_<N>_day<D>_<yyyyMMdd-HHmmss>.es3` | 按天轮转的历史快照，保留天数由 `EmporiumOption.SaveHistoryDays` 决定 |
| `*.corrupt-<时间戳>` | 被隔离的损坏文件 |
| `debug_spawner.es3` | 调试面板的物品生成器收藏和最近使用（`favorites`/`recent`） |
| `ModData/<modId>/…`、`ModConfigs/<modId>.json`、信任列表缓存 | mod 沙盒数据、mod 配置、远程信任列表缓存 |
| 日志 | `LogWriter` 自己写的日志文件（StallWatchdog 也写到这里） |

格式：ES3 输出 **JSON 明文**。`SaveFiles.IsSaveFileIntact` 用"首个非空白字节是 `{`、末尾非空白字节是 `}`"来判断文件是否完整，`ReadHeadFields` 用正则 `"playerCash":(-?\d+)` 和 `"saveVersion":"…"` 直接从文件头读预览字段。由此可以确认**没有加密、没有压缩**。

### 4.2 存什么、怎么存

- **标量与列表**：`PlayerStore` 上用 `[ES3Serializable]` 标注字段，包括金钱、店名、房租/贷款倒计时、声望（`List<StoreReputation>`）、未来顾客队列（`futurStoreClientIdQueue` 和 `futurStoreClientDayQueue` 两个平行列表）、当前顾客 `StoreClientInstance`、夜间日志、对话历史、大量 `isXxxIntroduced` 教程标记、统计（屠宰数、刮刮乐数）、`saveVersion`、`ModData()` 字典（mod 局内数据，键为 `modId:key`）等。
- **物品图**：`PlayerStore` 里有 11 个 `string *InvJSON` 字段（`mainInvJSON`、`backInvJSON`、`showcaseInvJSON`、`soldInvJSON`、`hiddenInvJSON`、`hirelingInvJSON`、`vendingMachineInvJSON`、`vendingFountainInvJSON`、`trashcanInvJSON`、`docInvJSON`、`cassettePlayerInvJSON`）。流程是：
  1. `EncodeSaveItem(id, pixelElement, window)`：把一个库存 UI 元素临时 `Attach` 到一个"存档袋"物品（`saveBags`），
  2. `SaveManager.EncodeNodes(root)`：BFS 遍历物品图，给每个 `GameItem` 分配一个递增 uuid，生成 `SaveItemNode{ identifier, unitCount, uuid, itemType(FullName), itemState(Tag 字典), itemShape/ModifiedShape(GridShapeBuilder), name/shortDescription/flavorText, uniqueId, unitBaseValue/unitValue/lateUnitValue/backupUnitValue, spriteAtlasPath/spritePath, itemTypes, childItems(uuid 列表), childItemInventoryNode(子库存序号) }`，
  3. 再用 `JsonUtility.ToJson(new SaveState{saveItems})` 转成字符串，存进 `PlayerStore` 的字段，**最后由 ES3 再序列化一次**，形成 JSON 套 JSON。
- **读档**（`SaveManager.DecodeNodesInternal`）：先对所有节点调用 `DirectoryMaster.Item/CharacterItem(identifier)` 重建物品，覆盖状态、形状、文本（`LocHelper.RelocalizeIfMissing`，见 §6）、价值和精灵；然后对每个节点 BFS 编号它的子库存，按 `(childUuid, invIndex)` 调用 `UncheckedAccept` 恢复父子关系，失败只打警告。`IsDecoding` 标志位让各 Helper 在读档期间跳过副作用。
- `ItemFeature` 统一放进 `PlayerStore.savedItemFeatureList`，通过 `parentItemUniqueId` 回连。

### 4.3 健壮性（这部分很用心）
- `PlayerStore.SaveGame()`：`SaveFiles.WithIORetry`（遇到 IOException 最多重试 3 次，每次间隔 150ms，应对杀毒软件或网盘锁文件）。流程是**先把旧槽文件轮转进历史**（`RotateIntoHistory`），再用 `ES3Writer` 一次性写入两个键。锁定失败时 `RestoreFromHistory` 回滚；文件不可读时 `QuarantineFile` 后重写。
- `RepairSettingsFileAtStartup`：设置文件损坏（首尾字节检查加 `ES3.KeyExists("__settings_integrity_probe__")` 探测）时改名隔离，用默认值继续运行。
- `IndexLoad/IndexSave` 失败时 `TryHealIndex` 隔离 `saves_index.es3`；`RestoreHistoryFromDisk` 可以从磁盘上的历史文件名（正则 `^save_(\d+)_day(\d+)_(\d{8}-\d{6})$`）反推并重建索引；`RepairDamagedRuns/RepairDamagedRun` 会把最新的完整历史提升为主档。
- 支持复制槽（连同历史）、重命名、"按天回档"（存档界面可以选历史日期）。
- 版本：`saveVersion = PlayerStore.VERSION`，游戏 UI 显示 `"Game: …\nSave: …"`，**但没有看到迁移逻辑**，兼容性靠 ES3 的字段级容错，以及"行为由工厂重建"的天然优势。

---

## 5. Mod 系统（重点）

### 5.1 mod 目录结构

```
<游戏根>/Mods/<任意文件夹>/            ← 本地 mod（ModSource.Local）
<Steam Workshop 安装目录>/<id>/        ← 创意工坊 mod（ModSource.Workshop，通过 SteamUGC.GetItemInstallInfo 获取）
  manifest.xml          必需
  *.dll / Assemblies/*.dll   manifest 里列出的程序集（至少一个含 IMod 实现）
  Textures/*.png|jpg    普通纹理 → mod.Textures["name"]
  Textures/Items/*.png  物品精灵（Point 过滤、Clamp）→ mod.Sprites，配合 GameItem.SetSpriteAndShapeFromMod
  Textures/Paper/*.png  纸张精灵（Bilinear，不可读）→ mod.PaperSprites
  Localization/<locale>.csv   key,value（# 开头是注释，支持 "" 转义和 \n）→ mod.Localization[locale]
  Sounds/*.wav|ogg|mp3  经 UnityWebRequestMultimedia 加载 → AudioDirectory.RegisterModClip("<modId>…")
  LootTables/*.json     掉落表数据 mod（见 5.9）
  Books/*.json          纸张/书籍文档（PaperBookRegistry，可以用游戏内 PaperEditor 编辑）
```

manifest.xml（`ModLoader.ParseManifestCore`）：

```xml
<Manifest>
  <ID>my.mod-id</ID>            <!-- 必填，[A-Za-z0-9_.-]，≤64，不能以 . 开头 -->
  <Name/> <Author/> <ModVersion/> <Description/>
  <GameVersions><Version>1.0</Version></GameVersions>   <!-- 只在 UI 里标红/绿，不阻止加载 -->
  <Prerequisites><Mod>other.id</Mod></Prerequisites>     <!-- 只打警告 -->
  <Assemblies><Assembly>MyMod.dll</Assembly></Assemblies> <!-- 必填，相对路径 .dll，禁止 .. 和绝对路径 -->
  <Capabilities><Capability>network</Capability></Capabilities> <!-- 必填，可以为空 <Capabilities/> -->
</Manifest>
```

`IMod { Init(ModManifest); OnEnable(); OnDisable(); }`。注意：**纯数据 mod 也必须带一个实现了 IMod 的 DLL**（`IsWellFormed` 在原始字节里搜索 `"IMod\0"`，`AnyReferencesIMod`），否则不会被加载。

### 5.2 加载流程（`ModLoader.Awake`，DontDestroyOnLoad 单例）

```
CollectGameAssemblyNames()          // AppDomain 中已加载的程序集 + Managed/*.dll 的文件名 → 视为"游戏程序集"
AssemblyResolve += ResolveModDependency   // 只在 mod 自己列出的 DLL 里解析依赖
ModTrust.EnsureLoaded()             // ES3 键 modTrust（JsonUtility）
ModTrustList.LoadCache()            // 本地缓存的远程信任列表（要求签名）
LoadModOrder()                      // ES3 键 modOrder
LoadAllMods()        → 遍历 Mods/* → LoadModFromFolder(Local)
LoadWorkshopMods()   → SteamUGC 订阅项 → LoadModFromFolder(Workshop)；同 ID 时本地覆盖工坊
ApplySavedOrder(); ValidatePrerequisites()
LoadModAssets()      → Textures / Localization / ModLootFile.Load / PaperBookRegistry.LoadMod / Sounds 协程 → ModHook.OnModAssetsLoaded
LogAudit()           → 每个 mod 输出一行审计日志：id 版本 来源 工坊id 作者SteamID 运行状态 信任状态 能力 哈希
StartCoroutine(ModTrustList.Fetch())   // 5s 超时拉取远程列表
```

`LoadModFromFolder` 的处理：
1. 解析 manifest，处理重复 ID（本地优先，否则先到先得）；`IsWellFormed` 检查程序集路径必须在 mod 目录内、文件必须存在、必须包含 IMod；统计**未列出的 DLL**（给出警告，这些 DLL 永远不会被加载）。
2. **玩家没启用的 mod：只登记，不加载代码**（检查是否在 Stopped 名单中）。
3. 已启用的 mod 走 `EvaluateTrust()`（§5.6），`Load == false` 时只登记原因。
4. `LoadModDll`：拒绝与游戏程序集同名的 DLL（防止冒充/遮蔽），然后 `ModSteamGuard.Install()`（第一次时安装运行时守卫），`Assembly.LoadFrom`，取第一个 `IMod` 实现并创建实例，`new Harmony("mod."+ID).PatchAll(asm)`，再调用 `mod.Init(manifest); mod.OnEnable()`。出现异常时 `UnpatchAll("mod."+ID)` 回滚并记录 `LoadError`。
5. 启用/禁用、排序都需要**重启**才生效（`RestartRequired`）。没有热卸载，因为 Mono 不能卸载已加载的程序集。

### 5.3 能力（Capability）模型（`ModCapability.cs`）

| capability | 含义（游戏内向玩家展示的文案） | 风险等级 |
|---|---|---|
| `files` | 读写本机任意文件 | Severe |
| `network` | 访问互联网 | Severe |
| `dynamic-code` | 运行时加载或生成代码 | Severe |
| `native` | 调用原生系统库 | Severe |
| `reflection-external` | 通过反射访问系统和引擎内部 | Severe |
| `steam-auth` | 向 mod 的在线服务证明你拥有本游戏 | High |
| `steam` | 使用 Steam 功能（成就、好友、浮层） | Medium |
| `process` | 在浏览器中打开网页链接 | Medium |

规则如下：
- manifest **必须声明**扫描器检测到的所有能力，否则拒绝启用（"mod 作者需要更新 manifest"）。
- 声明了能力且不在官方 Verified 列表中的 mod，启用时会弹出**能力确认框**（`ModEnablePromptUI`），框里还会显示创意工坊的上传时间（<3 天 Severe、<14 天 High…）和评分人数风险分级，本地 mod 则提示"无法验证"。
- **不声明任何能力的 mod 不弹框**，直接启用。
- mod 不需要 `files` 能力就可以使用沙盒文件 API：`ModHelper.ReadModFile*`（只读自己的 mod 目录）、`Read/Write/DeleteModDataFile*`（`persistentDataPath/ModData/<id>/`）。实现上用 `ResolveInside/RequireInside` 防止路径穿越，写操作还会通过调用栈确认调用者确实是该 mod（`RequireOwnDataFolder` → `ModLoader.CallingModId()`，其他 mod 的数据目录只读）。

### 5.4 IL 静态扫描（`ModScanner.cs`，`VERSION = 3`）

用 DseCecil 以 `ReadingMode.Immediate + InMemory` 把每个列出的 DLL 读进内存（不锁文件），逐个类型、字段、方法、指令扫描，输出 `Capabilities`（需要声明）和 `HardFlags`（直接拒绝，开发者模式除外）两类结果。

**A. 能力检测（CheckCall / CheckTypeUse / ScanMethod）**

| 能力 | 触发的 IL 模式 |
|---|---|
| files | 调用、构造或 `ldtoken` 以下类型：`System.IO.File/FileStream/Directory/FileInfo/DirectoryInfo/FileSystemInfo/FileSystemWatcher/DriveInfo`、`ZipFile`、`UnityEngine.Windows.File/Directory`、`ScreenCapture`、`Microsoft.Win32.Registry*`、`ES3File/ES3Writer/ES3Reader/ES3Stream/ES3Directory`；`System.IO.IsolatedStorage.*`、`MemoryMappedFiles.*`；带路径的 `AssetBundle.LoadFromFile*`；`StreamWriter/StreamReader(string)` 构造；`XDocument/XElement/XmlDocument/XmlReader/XmlWriter` 的 `Load/Save/Create(string…)`；`ES3` 的 23 个路径类方法（SaveRaw/LoadRawBytes/CopyFile/DeleteFile/GetFiles…）或 ≥2 个字符串参数的重载；`new ES3Settings(string…)`；对 `ES3Settings.path` 字段赋值（`stfld`） |
| network | `System.Net.*`（排除 `WebUtility/IPAddress/IPEndPoint/Mime`）、`UnityWebRequest*`、`UnityEngine.WWW/Ping`、`Steamworks.SteamHTTP/SteamNetworking/SteamNetworkingSockets/SteamNetworkingMessages` |
| dynamic-code | `System.Reflection.Emit.*`、`System.Runtime.Loader.*`、`System.CodeDom.*`、`CSharpCodeProvider`、`BinaryFormatter` 等 `Serialization.Formatters.*`、`NetDataContractSerializer`；`Assembly.Load/LoadFrom/LoadFile/…`（8 种）；`AppDomain.Load/DefineDynamicAssembly/ExecuteAssembly/CreateInstance*`（8 种）；`Activator.CreateInstance(string…)/CreateInstanceFrom`；`Expression.Compile()` |
| native | P/Invoke（`DllImport`）；指针或函数指针出现在字段、签名或局部变量中；`calli` 指令；`Marshal`、`NativeLibrary`、`Unsafe`、`UnsafeUtility` |
| reflection-external | `Microsoft.CSharp.RuntimeBinder.*` 和 `CallSite`（即 `dynamic`）；`Type.GetType(string)`、`Type.GetMethod/GetField/GetProperty/InvokeMember…`（15 种查找方法）、`Assembly.GetType/GetTypes` 等：扫描器会**用数据流回溯参数来源**（`FindProducer`：从调用点向前模拟栈深度，遇到分支目标、跳转或 return 就放弃），判断目标类型属于 mod 自身（放行）、外部作用域（mscorlib/System/Unity/Harmony/Steamworks… 需要声明能力）、受保护的守卫类型（硬拒绝），还是无法判定的"computed"（按需要声明能力处理） |
| steam | `ModSteamRules.Classify`：`SteamUGC/SteamRemoteStorage` 的只读方法，以及其他 Steamworks 类型的非值类型调用 |
| steam-auth | 调用 `ModHelper.RequestSteamAuthTicket/CancelSteamAuthTicket` |
| process | `Application.OpenURL("http(s)://字面量")`，参数是字面量网址时才算 |

**B. 硬拒绝（HardFlag）**

| Flag | 条件 |
|---|---|
| `bundled-framework` | 程序集名是 0Harmony/MonoMod*/Mono.Cecil*，或者把 `HarmonyLib.*`、`MonoMod.*`、`Mono.Cecil*` 命名空间的类型编进了 mod |
| `unlisted-reference` | 引用了既不是游戏程序集、也不在自己 `<Assemblies>` 里的程序集（**因此 mod 之间不能互相引用代码**） |
| `loader-target` | `[HarmonyPatch]`（类型或字符串参数）指向守卫类型；反射目标是守卫类型；`ES3.Save/Delete…("modTrust"|"modOrder"|"workshopOwners"|"modUiWarningAcceptedVersion")` 字面量写入信任相关的键；`Harmony.Unpatch*("dse.*")`。守卫类型有 14 个：`ModLoader, ModTrust, ModTrustRecord, ModFingerprint, ModTrustList, ModScanner, ModCapability, ModSecurityDebug, ModSteamRules, ModSteamGuard, ModSteamAuth, ModWorkshop, SteamManager, PaperBookRegistry`，另加 Steamworks 中的敏感类型 |
| `unpatch-all` | `Harmony.UnpatchAll()` 不带 id（`ldnull`），或 `Unpatch*("*")` |
| `process-start` | 调用或引用 `System.Diagnostics.Process/ProcessStartInfo`；`Application.OpenURL` 的参数是非 http(s) 字面量或计算出来的值 |
| `steam-account` | `SteamUser` 的票据/认证/语音（GetAuthSessionTicket、GetEncryptedAppTicket、BeginAuthSession…）；`SteamFriends` 发消息、邀请；`SteamMatchmaking` 发大厅消息；直接 DllImport steam_api 等原生库；字符串常量里出现 `steam_api64.dll`、`steamclient`、`isteamuser_` 等内部符号 |
| `workshop-publish` | `SteamUGC` 的写操作（CreateItem/SubmitItemUpdate/DeleteItem/SubscribeItem/投票/收藏/依赖…共 29 个）、`SteamRemoteStorage` 写操作（17 个） |
| `stealer-strings` | `ldstr` 中出现窃密特征串：`local storage\leveldb`、`\login data`、`\web data`、`\cookies`、`loginusers.vdf`、`ssfn`、`\discord\`、`discordcanary`、`wallet.dat`、`metamask`、`exodus\`、`electrum\`、`\tdata`、`config.vdf`、`\steam\config` 等 24 条 |
| `opaque-string` | ≥1024 字符的 base64 字面量 |
| `payload-resource` / `opaque-resource` | 嵌入资源或字段初始值（RVA 数据）以 `MZ` 开头或包含 "This program cannot be run in DOS mode"；或者 ≥512 字节、熵 >7.5 bit/byte、且不是已知媒体格式（PNG/JPG/GIF/BMP/OGG/RIFF/MP3/FLAC/MP4/字体/UnityFS/PDB/可打印文本…） |
| `payload-file` / `opaque-resource` / `oversized` | **扫描整个 mod 文件夹**：29 种可执行或脚本扩展名（.exe/.dll（未列出的）/.bat/.ps1/.vbs/.js/.lnk/.reg/.jar/.so/.dylib…）；文件头是 MZ/ELF/`#!`/Mach-O；未知格式且熵 >7.5；>8MB 的未知格式文件；文件数超过 5 万 |
| `unreadable` | IL 或资源读取失败，或 `[HarmonyPatch]` 参数无法解析 |

**C. 设计特点**
- 规则是**白名单式的"能力声明" + 黑名单式的"恶意特征"**两层，扫描结果用玩家能看懂的文案展示（`FormatFindings`），也可以在控制台用 `mod-scan` 查看全部发现。
- 扫描器有版本号：信任记录里保存 `scannerVersion`，扫描器升级（目前是 3）后，已经批准过的 mod 会被**重新扫描**；新规则不通过就进入 `Changed` 状态，提示"游戏更新后的扫描器拒绝了它"。
- **局限**（属于静态分析的固有局限，开发者也通过运行时守卫和远程列表做纵深防御）：`FindProducer` 只在基本块内回溯，参数是计算出来的值时只能降级为"需要声明能力"或者放过。例如 `ES3.Save` 在默认设置文件上写一个非字面量的键，既不触发 `files`，也不触发 `loader-target`，而信任记录 `modTrust` 正好存放在这个文件里。另外，mod 获得的是**完整的进程内信任**（Harmony 可以给任何非守卫类型打补丁），所以扫描是"安全带"，不是沙箱。

### 5.5 运行时守卫（`ModSteamGuard.cs`）
- 第一次加载 mod 代码前，用 `Harmony("dse.steamguard")` 给以下方法打 **priority=800 的 Prefix**：
  - `System.Diagnostics.Process` 的所有 `Start` 重载：`PrefixProcess` 通过 `ModLoader.CallingModId()` 检查调用栈（遍历 StackTrace 帧，按程序集对象或文件路径前缀匹配 mod 目录），**只有调用者是 mod 时才拦截**，游戏自身调用照常放行。
  - `SteamUGC/SteamRemoteStorage/SteamUser/SteamFriends/SteamMatchmaking` 中 `IsGuardedAtRuntime` 的方法（写工坊、账号票据、发消息等）：`Prefix` **无条件返回 false**，唯一例外是 `ModSteamAuth` 自己正在代 mod 签发票据时（`IsIssuing`）的 `SteamUser.GetAuthSessionTicket`。
- 拦截后把记录放进 `ConcurrentQueue`（可能来自任意线程），在主线程的 `Update → Drain()` 中调用 `ModLoader.OnSteamGuardTripped`：把该 mod 当前指纹写入 `ModTrust.Stopped`（**这个版本永久不能再启用**）、禁用它、`UnpatchAll` 它的补丁、提示重启，并输出"请向开发者举报"的日志。
- 防篡改：每 10 秒 `Verify()` 一次，检查每个守卫方法上是否还有 owner 为 `dse.steamguard` 的前缀补丁，如果被移除就重新打上（静态规则也禁止 mod 调用 `Unpatch("dse.*")` 和 `UnpatchAll()`）。
- `ModSteamAuth`：给需要后端校验所有权的 mod 一条**受控通道**。`ModHelper.RequestSteamAuthTicket(serverIdentity, onTicket)` 要求：调用者确实是某个正在运行的 mod，manifest 声明了 `steam-auth`，在主线程调用，`serverIdentity` 必须是 `steamid:` 或 `ip:` 格式（把票据绑定到具体服务器，防止转发给第三方），Steam 已初始化。每个 mod 同时只能有一个票据，有 20s 超时，mod 停止时自动 `CancelAuthTicket`。

### 5.6 信任链：指纹 → 本机记录 → 远程列表
- **指纹**（`ModFingerprint`）：`SHA256(manifest.xml)` 加上每个列出 DLL 的 SHA256，组合后再算一次哈希得到 `Combined`。**只覆盖 manifest 和 DLL**，贴图、JSON、CSV 不在指纹里（这些文件由文件夹扫描负责检查危险内容）。
- **本机信任记录**（`ModTrust`，ES3 键 `modTrust`，以 JsonUtility 字符串形式保存）：`ModTrustRecord{ id, source, workshopId, manifestHash, files[], combinedHash, declared[], detected[], gameVersion, approvedAt, updatedAt, developerMode, scannerVersion }`，外加 `Stopped` 名单（哈希加原因）。
- `EvaluateTrust` 的状态机：

```
远程列表 Blocked ─────────────→ Blocked（撤销信任并静默禁用）
本机 Stopped 名单命中 ───────→ Stopped
kill switch 开启（可设"已验证 mod 除外"）→ Paused
无记录 ─────────────────────→ Unknown（需要玩家在 Mod 界面重新启用并确认）
来源或工坊 ID 变了 ──────────→ Changed
目录里有未列出的 DLL ────────→ Changed
哈希一致且扫描器版本一致 ───→ Match（直接加载，不重新扫描）
否则重新扫描：
   本地 + 开发者模式 → DeveloperMode（扫描拒绝也照样加载，只写日志）
   扫描拒绝 → Changed
   检测到未声明的能力 → Changed
   能力超出上次批准的范围 → Changed（"新版本请求了新能力"）
   否则 → Updated（自动接受更新，刷新记录）
```

   效果：**mod 更新时，只要能力没有扩大就静默放行；一旦扩大就必须让玩家重新确认**。这和手机 App 权限升级的提示逻辑是一样的。
- **远程信任列表**（`ModTrustList`）：从 GitHub raw 拉取 `trust-list.json`，结构是信封 `{payload, signature}`，用**内置 RSA-2048 公钥（PKCS#1 v1.5 + SHA-256）验签**。payload 包含 `issued`、`blocked[{id,hash,reason}]`、`verified[{id,hash}]`、`blockedWorkshopItems`、`blockedPublishers`（按工坊作者的 SteamID）、`blockedFiles`（按单个文件哈希）、`blockedAuthors`、`killSwitch{allMods, exceptVerified, message}`。
  - 防回滚：拉到的列表 `issued` 不比缓存新时不更新；超过 30 天标记为 stale。
  - 只有验签通过才写入本地缓存；`mod-trust-list-load` 可以在 devMode 下加载未签名的测试列表（标记为 `UNSIGNED DEV LIST`，且不缓存）。
  - `Verified` 状态的 mod 启用时可以免去能力确认框。

### 5.7 面向玩家的 UI 流程
- 首次打开 Mod 界面时显示免责声明 `ModWarningUI`（`TEXT_VERSION = 2`，文案更新后会再显示一次）。正文是 `UI/ui_mods_dll_warning`：*"A mod can contain code that runs with the same access to your computer as the game itself. The game checks mods before activation, but cannot make them safe. Only activate mods from authors you trust…"*。开发者明确把扫描定位为"检查"，而不是"沙箱"。
- 启用 mod 时调用 `CheckEnable`：检查未列出的 DLL、远程列表、Stopped、kill switch、同名程序集、扫描结果、未声明能力。有问题时弹出 `ModRefusalUI`（可以"复制日志"），否则在需要时弹出 `ModEnablePromptUI`（列出能力并按风险着色，显示工坊上传时间和评分人数风险），确认后 `ApproveAndEnable` 写入信任记录。
- 本地 mod 可以开启"开发者模式"（`SetDeveloperMode`，只对 Local 有效），方便作者迭代时跳过扫描拒绝。
- 本地 mod 加载失败时，`ModLoader.Start` 会主动打开 Quantum Console 显示错误（前提是 dev 工具已开启，否则控制台已被 Stripper 销毁）。
- Mod 列表支持排序（`modOrder`）、工坊退订、显示预览图和统计、GameVersions 红/绿标记；tooltip 和 Recorder 里可以显示物品来自哪个 mod（`showModSourceInTooltip`，通过 `MOD_ID_TAG/MOD_NAME_TAG` 实现）。

### 5.8 Mod 能做什么：API 面
- **Harmony**：自动 `PatchAll`，可以 Prefix/Postfix/Transpiler 任意游戏方法（守卫类型除外）。这是能力上限最高、也最脆弱的扩展方式。
- **`ModHook` 44 个事件**：物品目录初始化、顾客生成 5 档、读档 4 档（Init/Early/Normal/Late）、卷帘门开/关、睡觉/醒来、离开/返回商店、夜间服务、放置物品、tooltip（普通和开发者两种，各分 Early/Late）、资源加载完成、配置应用、掉落表加载完成/每次掷骰（`LootRollContext`）、打开纸张、网络升级解锁、新游戏开始、难度应用、Afterhour 到达/离开/事件开始/结束。
- **`ModHelper` 133 个静态 API**（按领域分组）：
  - 经济/物品：`AddCredit`、`SpawnItem`、`SetItemOwnerShip`、`MoveItemTo{FrontCounter,BackCounter,Inventory,Trash,DisplayCase,Afterhours}`、`SetSource`
  - 本地化/音频：`GetLocalized(modId,key,args)`（当前语言 → en → key 本身的回退链）、`PlaySound/2/3`
  - 调试面板：`AddDebugFunction(modId, category, …)`（分类有 Store/Command/Spawn/Special/Modding/Marketing）
  - 设置：`GetConfig`、`AddGameOption{Section,Toggle,Slider,Dropdown,KeyBind}`，把 mod 选项**嵌进游戏原生设置菜单**；`ModConfig` 落盘到 `ModConfigs/<id>.json`
  - 内容注册（都带 `Remove*(modId)` 批量撤销）：`RegisterLootTable/Group` 及增删改、`AddNetworkUpgrade`（Wild 网络科技树，可指定页签、前置、冷却）、`AddStartingScenario`、`AddStartingPerk`、`AddDifficulty/ModifyDifficulty`、`AddAppraisalItem`（新增可鉴定品类，包含随机生成和正确估值函数）、`AddAfterhourLocation`（可以用 `Action<CustomUIBuilder>` 搭建地点面板）、`AddAfterhourEvent/AfterhourEffect`、`RegisterAfterhourAmbience`
  - 纸张：`OpenPaper/ClosePaper/RegisterPaperBook/ParsePaperBook/CreatePaper/SpawnPaperItem`
  - 存储：`Set/GetModData*`（随存档保存，按 `modId:key` 命名空间隔离，支持 string/int/float/bool）、沙盒文件 API、`LoadModAssetBundle`（只能加载自己目录里的）
  - Steam：`RequestSteamAuthTicket/CancelSteamAuthTicket`
- 所有注册 API 的第一个参数都是 `modId`，这样可以**按 mod 归属批量撤销**，也能在 UI 里标注来源。这是一个值得借鉴的约定。

### 5.9 数据 mod：LootTables JSON（`ModLootFile.cs`）

`LootTables/*.json` 按文件名排序后依次应用，用 `JsonUtility` 解析：

```json
{
  "tables":  [ { "id": "my_table",  "entries": [ { "id": "energy_credit", "weight": 3 }, { "id": "glass_shard", "weight": 1 } ] } ],
  "groups":  [ { "id": "my_group",  "entries": [ { "id": "my_table", "weight": 1 } ] } ],
  "patches": [ { "table": "vanilla_table_id", "clear": false,
                 "add": [ { "id": "my_item", "weight": 2 } ],
                 "remove": [ "glass_shard" ],
                 "setWeight": [ { "id": "energy_credit", "weight": 0.5 } ] },
               { "group": "vanilla_group_id", "add": [ … ] } ]
}
```

- `table` 的条目是物品 id，`group` 的条目是 table id。`patch` 指定 `table` 或 `group` 之一，支持 `clear/add/remove/setWeight`。
- `LootRegistry` 按 mod 记录操作日志（`Op`），所以可以撤销，也能用 `loot-list` 查看"每个条目的有效概率和是哪个 mod 加的"，用 `loot-roll` 做蒙特卡洛统计。原版表来自 RNGNeeds 的 `ProbabilityList`，在运行时被摊平；不支持的 RNGNeeds 设置会触发 `WarnUnsupportedSettings`。
- 其他数据 mod 通道：`Localization/*.csv`、`Textures/**`、`Sounds/*`、`Books/*.json`（Newtonsoft，`PaperBookData{id,title,background,width/height,designWidth/Height,chrome[],templates[],pages[{name,template,elements[]}],item,locales[]}`，元素类型有 text/image/button/flow/line，`action` 可以是 close/zoom/flip_left/flip_right/page_indicator）。
- 读取 Books 时有防越界检查："refusing to read …, outside the mod folders or not a .json file"。

### 5.10 小结
- **加载**：manifest 驱动、显式列出 DLL、必须实现 IMod、Harmony 自动打补丁、启用/禁用需重启。
- **安全**：静态 IL 扫描（能力和恶意特征两类规则）+ 文件夹扫描 + 哈希指纹信任 + 能力单调性（扩大即需重新确认）+ 签名远程封禁/验证列表/全局停用开关 + 运行时 Harmony 拦截并永久熔断 + 受控的 Steam 票据通道 + 沙盒数据目录。对独立游戏来说，这套设计相当完整，有明显的供应链安全意识（stealer 特征串、Steam 账号接管、工坊发布滥用都有专门规则），看起来是被真实事件推动做出来的（1.0.0.6 的"安全更新"）。
- **局限**：进程内全信任（Harmony + 反射）、静态分析可以被计算型参数绕过、信任记录和普通设置放在同一个 ES3 文件里、指纹不覆盖非 DLL 资源、mod 之间不能共享代码。

---

## 6. 本地化方案

- **框架**：Unity Localization 1.x，字符串表由 Addressables 打包（`StreamingAssets/aa/StandaloneWindows64/localization-string-tables-<lang>_assets_all.bundle`）。共 **13 种语言**：en、zh（简体）、ja、ko、de、fr、it、es、pl、pt-BR、ru 等。自动检测列表 `AUTO_DETECT_LOCALES` 目前只有 8 个（en/ja/de/ru/zh/fr/es/pl）。
- **14 张表**（以导出的 `~/ps_work/loc/english.json` 为准，每种语言约 6,900 条）：Dialogue 2544、Mechanic 1577、Item 980、UI 804、MainMenu 203、ClientName 192、RepPerkTable 141、NetworkUpgrade 119、PerkTable 104、PhoneDialogue 82、NewGame 77、Hint 34、Tutorial 26、Interactable 12。
- **`LocHelper`（408 行）**：
  - 每张表一个访问器：`GetLocalizedDialogue`（全代码调用 2318 次）、`GetLocalizedMechanic`（1601）、`GetLocalizedItem`（1053）、`GetLocalizedClientName`（383）、`GetLocalizedUI`（318）……
  - `GetLocalizedOrDefault(table, key, englishFallback, args)`（213 次，另有 UI/Mechanic 版本）：**表里还没有这个键时直接显示代码里写的英文**，每个缺失键只记一次日志，并指向内部文档 `docs/pending-localization.md`。这样策划和程序可以先写代码、后补翻译。
  - 格式化失败时打橙色错误日志，回退显示原始条目，并提示查看 `docs/localization-plurals.md`。
  - 启动语言选择：ES3 中有 `locale` 就用它；否则 Steam 客户端语言（`SteamApps.GetCurrentGameLanguage()` 映射表）→ 系统语言 → en，结果写回 ES3。
  - `RelocalizeIfMissing`：用 `NoTranslationFoundMessage` 模板反推出正则，识别存档里保存下来的"No translation found for key X in table Y"文本，读档时重新查表修复。
- **`LocSmartStringHelper`**：Localization 表加载完成和每次切换语言时，遍历 14 张表，把包含 `:plural:` 或 `:choose(` 的条目**自动设为 `IsSmart = true`**，省去译者手动勾选 Smart 的步骤（漏勾的话复数格式会原样输出）。配合调试命令 `loc-plural-test`（用一组数字测试当前语言的所有复数形式）和 `loc-smart-list`。
- **`LocGenderHelper`（387 行）**：自定义标记 `[g:男|女|中]`（性别）和 `[r:正式|非正式]`（敬语/语域），配合角色 Tag `SPEAKS_FORMAL/SPEAKS_INFORMAL`，在 Smart String 之外再做一层文本替换，服务于德、法、波、俄、日等语言的语法需求。
- **字体**：
  - `FontLoader`（单例）持有大约 20 个 TMP 字体：western、ja/ko/zh-Hans 的正文、手写和科幻风格、NotoSans/Serif 各字重、Rubik、Caveat、Lubrifont、Tektur，以及**波兰语专用的展示字体**（`GetFontByRole(FontRole.Lubrifont, pl)` 替换成 `polishDisplayFont`，原字体缺少波兰语字符）。
  - `ScriptFontSwap.Apply(tmp, text)`：**逐字符扫描 Unicode 区间**（韩文音节/字母 > 假名 > CJK 统一汉字/扩展 A/兼容/全角/代理对），给 TMP 动态挂上组件并切换字体。优先级是韩 > 日 > 中，用于玩家输入的店名、mod 文本、混合语言字符串等**不依赖当前 locale 的场景**。`TreeNodeRender.InstantiateText()` 会调用 `ApplyLocaleFont`。
  - 还有 `TMPLocalizer`、`TMPInputFieldLocalizer`、`DropdownLocalizer`、`ResolutionLocalizer`、`LocalizedLines` 等组件化本地化器，用于场景里的静态 UI。
- **Mod 本地化**：mod 用 `Localization/<locale>.csv` 提供自己的字典，通过 `ModHelper.GetLocalized` 访问，**不进入 Unity Localization 表**，回退顺序是当前语言 → en → key。
- **问题**：物品的 `name/shortDescription/flavorText` 在**创建时就被解析成当前语言的字符串，并写进存档**。切换语言后，老物品仍然显示旧语言（只有"缺失翻译"的情况会被 `RelocalizeIfMissing` 修复）。更合理的做法是存 key，显示时再解析。

---

## 7. 开发与调试工具

| 工具 | 机制 | 要点 |
|---|---|---|
| **DevTools** | 游戏目录下有 `dev.txt` 就启用（F9 调试面板、F8 控制台），`1dev.txt` 是 Full 模式，编辑器内总是开启 | 用标记文件做开关，**发行版本身就带着全部调试能力**，玩家或 mod 作者放一个 txt 即可打开 |
| **DevConsoleStripper** | `AfterSceneLoad` 时，如果 `!DevTools.IsEnabled`，就在每次加载场景时 `FindObjectsOfType<QuantumConsole>(includeInactive)` 并 Destroy | 控制台默认放在场景里，靠运行时剥离 |
| **Quantum Console 命令**（30 条） | `[Command("name","描述",Platform.AllPlatforms,MonoTargetType.Single)]` | Afterhour（列出/开始/结束事件）、鉴定（等级/封禁/排队顾客）、Loot（list/roll）、Mod 安全（mod-scan、mod-trust、mod-steam-guard、mod-trust-list[-load/-fetch]、mod-preview-warning/refusal/prompt）、Paper（list/open/close/reload/edit/item）、枪单测试、本地化复数测试、教程气泡、剧情重放（serpico-test）、顾客脸统计。**安全子系统的每个 UI 都可以预览**，方便 QA 和翻译 |
| **DebugPanel**（1665 行，F9） | 分 5 组：spawn、kit、ui、cmd、**marketing** | Marketing 组共 30 个按钮：一键刷出枪械/种植/植物 DNA/化学/彩票套件、各类商品、名片、磁带，"Toggle Tooltip"（cinematic 模式隐藏 UI）、排队特定剧情角色、"Queue Normal Seller/Buyer/Security Inspection"（名字里带 Trailer）、随机日期和金额、顾客与义体外观快速轮换（0.8s 一次，拍素材用）。**说明发行流程里把录预告片和截图当作一项正式需求**，配合包内的 Unity Recorder |
| **CustomUIDebugPanel/Windows/ItemSpawner**（约 1.7k 行） | 用 CustomUIBuilder 搭建的新版调试 UI；物品生成器有收藏和最近使用（存进 `debug_spawner.es3`） | mod 可以通过 `ModHelper.AddDebugFunction` 往里加按钮 |
| **DevMode** | 线程安全的懒汉单例，`isOn = true`，`DevModeBuffEntity` 把战斗实体的命中和伤害设为 200% | 原型遗留，和 DevTools 无关 |
| **PaperEditor**（Paper 子系统共 29 个文件、约 1 万行） | 游戏内的所见即所得文档编辑器：拖拽、缩放、框选、对齐（6 种）、吸附网格和其他元素（阈值 3px）、标尺、ID 显示、剪贴板、**JSON 快照式撤销/重做**（`PaperEditorHistory`）、**0.6s 自动保存**、**1s 轮询磁盘热重载**（外部编辑器改了 JSON 会自动刷新）、Inspect 模式、精灵选择器（需要编辑器预先 `Collect Paper UI Assets` 生成 `PaperAssetCatalog`）、校验（`PaperEditorValidation`）、**导出为 C# Fluent API 代码**（"// Generated by the paper editor…"）、起始模板（`Resources/PaperUI/Starters`） | 原版的报纸、指南、合同等"纸面道具"和 mod 书籍共用一套数据格式，**同一个工具既服务开发也服务 mod 社区** |
| **StallWatchdog** | 后台线程每 250ms 检查主线程最后一帧的时间戳（`Interlocked`），超过 5s 判定为卡顿，之后每 30s 报告一次：帧号、场景、是否有焦点、托管堆、工作集，以及**最近 24 条面包屑**（`Crumb("save")`、`"load"`、`"begin day"`、`"client …"`、`"dialogue …"`、`"window …"`、`"paper …"`、`"customui"`、`"display apply"`……）；主线程恢复时记录总卡顿时长 | 通过 `LogWriter.AppendDirect` 直接写文件，不依赖已经卡住的主线程。这是排查玩家"游戏卡死"反馈的低成本方案 |
| **LogWriter / VerboseLog** | 自建日志文件（10s 刷新一次缓冲，带文件锁）；`VerboseLog.ResetUnityLoggerFilter` 控制日志级别 | 全代码有 801 处 `Debug.Log*`，大量带 `[模块]` 前缀，可以 grep |
| **EmporiumAnalytics** | UGS Analytics。`InitAnalytics` 要求 `consentAnalytics` 同意，环境名为 `internal_playtest`（还预留了 `public-playtest`）。**`OVERWRITE_DISABLE_ANALYTICS = true`，发行版完全不上报** | 19 个事件，见下表 |
| 编辑器工具（从字符串推断） | `Build > Generate Tablet/Transit/Wild Network/Credits/Load Game/New Game … UI Prefab(s)`、`Collect Paper UI Assets`；内部文档 `docs/custom-ui.md`、`docs/localization-plurals.md`、`docs/pending-localization.md` | 团队有"文档 + 代码生成"的习惯 |

**埋点事件**（基类 `Unity.Services.Analytics.Event`，大多带公共参数 `run_id / run_number / spec(开局专精) / day / current_credit / total_play_time_seconds / run_play_time_seconds`）：

| 事件 | 额外参数 | 用于观察 |
|---|---|---|
| `new_game_started` | skip_intro, hard_mode, endless_mode | 开局选择 |
| `starting_perk_selected` | perk_id | 特长选择率 |
| `load_game` / `leave_to_main_menu` / `game_exit` | days_since_inspection_loss, last_inspection_loss_value, days_since_theft, last_theft_value | **流失点：是否在被查抄或被偷之后退出** |
| `on_sleep`（每日结束） | estimated_total_wealth, after_hour_activity, 8 个阵营声望（下层/上层/安保/黑市/革命/游客/教会/卡特尔）, is_insurance_active, is_alarm_active | 每日经济和声望曲线 |
| `game_over` | gameover_reason | 失败原因 |
| `inspector_arrived` / `inspection_bribe` / `on_inspected` | inspector_type, is_from_showcase, has/used_security_grace, bribe_amount, outcome_type, has_smuggler_bay, total_lost_item_value/count | 核心风险机制（查抄）的平衡 |
| `confront` | item_id, confront_type, outcome | 当面揭穿顾客 |
| `customer_reported` / `wanted_not_reported` | customer_id, report_type | 举报通缉犯的选择 |
| `appraisal_sent` / `appraisal_guild_interaction` | item_id, actual_value vs sent_value / action | 鉴定准确度 |
| `business_permit_bought` | — | 营业许可购买时机 |
| `item_of_interest_acquisition` | machine_id, machinery_bought_count | 机器购买顺序 |
| `wild_ui_opened` / `wild_upgrade_purchased` | upgrade_id | 科技树使用情况 |

这套埋点覆盖了经营曲线、风险事件、流失时机，是一份不错的"经营模拟最小埋点集"参考。

---

## 8. 代码规模统计

统计方式：按文件名前缀和后缀规则把 944 个文件归类（脚本在 `/tmp/ps_stats/classify.py`，规则依次匹配，先命中先得）。总计 **944 文件 / 约 245,159 行**（反编译代码，没有注释，实际源码会更长）。

| 子系统 | 文件数 | 行数 | 占比 | 最大文件 |
|---|---:|---:|---:|---|
| UI 框架 + 各界面 | 187 | 39,712 | 16.2% | StoreUIManager 1484、RunSummaryUI 1239、GameItemElement 1187、EmporiumOption 1109、OverlayHandler 1093、PixelWindow 1085 |
| 物品行为 Helper（玩法规则） | 103 | 38,592 | 15.7% | AquacultureHelper 2462、PreBuiltItemHelper 2219、HusbandryHelper 1724、WaterHelper 1646、HopperHelper 1536、ChemicalHelper 1221 |
| 顾客 / 对话 / 谈判 | 83 | 36,575 | 14.9% | StoreClientList 4252、StoreClientUniqueList 2914、StoreClientManager 2239、BargainUIManager 1681、StoreClientListWanted 1611、NegociationUIManager 1578 |
| 商店核心 / 经营循环 | 161 | 35,407 | 14.4% | PlayerStore 4213、EmporiumEntry 1984、NetworkUpgradeList 1208、NetworkUpgrade 1163、StartingPerkList 882、GunOrderData 814 |
| 物品 / 库存模型 | 50 | 17,297 | 7.1% | GameItem 2597、GameCharacterItem 1578、ItemFeatureList 1209、GameGridInventory 1021、ItemMultiSelectHandler 968 |
| 遗留原型（战斗/飞船/太空/突袭） | 103 | 15,637 | 6.4% | ShipHelper 1105、RaidScene 666、CombatManager 528、GameCharacterRaidInventory 527、MenuScene 512 |
| 机器 Machine | 51 | 14,206 | 5.8% | MachineAutoButcher 1340、MachineStorageCoordinator 1013、MachineDnaManipulator 710 |
| 数据目录 Directory | 38 | 14,053 | 5.7% | UnusedDirectory 2734、MiscItemDirectory 1548、GunModDirectory 1198、ModuleDirectory 1042 |
| Paper 纸张文档 + 编辑器 | 29 | 10,303 | 4.2% | PaperEditor 2382、PaperEditorWindow 1670、PaperUI 1492 |
| Mod 系统（加载/安全/API/UI） | 41 | 10,072 | 4.1% | ModLoader 1474、ModScanner 1372、ModUIManager 1321、ModHelper 1242、ModTrustList 572、ModHook 430 |
| 调试 / 开发工具 | 23 | 4,828 | 2.0% | DebugPanel 1665、CustomUIDebugPanel 1183、CustomUIDebugWindows 466 |
| 埋点 Analytics | 20 | 2,049 | 0.8% | EmporiumAnalytics 630 |
| 存档 | 8 | 1,745 | 0.7% | SaveFiles 1067、SaveManager 270 |
| 入夜外出 Afterhour / 地点 | 17 | 1,724 | 0.7% | AfterhourEvents 296、CommissaryData 248 |
| 本地化 / 字体 | 12 | 1,504 | 0.6% | LocHelper 408、LocGenderHelper 387 |
| 掉落 / 随机 | 16 | 1,374 | 0.6% | LootRegistry 619、RNG 264 |
| 未分类 | 2 | 81 | 0.0% | TAnimSoundWriter（Febucci demo）、AssemblyInfo |

几个观察：
- **"内容即代码"占了大头**：顾客定义（约 3.7 万行）、物品 Helper（约 3.9 万行）、物品目录（约 1.4 万行）三项合计约 9 万行，占 37%。
- **工具和平台类代码比例很高**：Mod 1 万行 + Paper 1 万行 + 调试 0.5 万行 + 埋点 0.2 万行，接近全部代码的 11%。
- 遗留原型约 1.6 万行（6.4%），加上 `UnusedDirectory`（2734）、`StoreClientListUnused`、`EventUnused`、`IntelUnusedHelper` 等显式标为 Unused 的内容，死代码或半死代码估计 **7–8%**。
- 单文件超过 2000 行的有 9 个，`StoreClientList.cs` 和 `PlayerStore.cs` 都超过 4000 行。

---

## 9. 设计点评

### 9.1 值得小团队借鉴的工程实践

1. **代码即数据 + 工厂注册**：`Add("id", () => Factory())` 加上 `Directory<T>` 和 `DirectoryMaster`。对于 1–3 人、程序主导内容的团队，C# 编写内容可以直接跳转、重构、断点调试，组合 Helper 比搭配置表灵活；存档只存 id 和状态，"行为由工厂重建"，**换掉逻辑不需要做存档迁移**。缺失 id 时保留原 id 生成占位物品，对 mod 增删很友好。
2. **字符串 Tag 加委托钩子的轻量组合**：`TagSystem` 存数据，`Func/Action` 多播加 `InvokeAllReduce(AND/OR/Max/Min)` 存行为，`Helper.InitXxx(item)` 负责组装。不引入 ECS 也能得到"组件化物品"，而且 Tag 天然可序列化，存档只写非默认值。
3. **形状从精灵自动推导**：美术画完就能进背包，切片不对齐网格时自动警告，省掉了手填形状矩阵的工作。
4. **存档健壮性清单**：写前轮转出历史、按天保留快照、首尾字节完整性检查、损坏文件隔离（不删除）、索引自愈、IO 锁重试、启动时修复设置文件、`CultureInfo.Invariant` 固定。每一条都对应一类常见的玩家报错，可以直接照搬。
5. **Mod 安全的分层思路**（即使不做 mod，也可以参考它的威胁模型）：
   - "能力声明 + 静态检测 + 玩家确认 + 能力单调性（扩大即需重新确认）"，与手机权限模型一致；
   - 指纹固定版本，更新时自动重新评估，扫描器带版本号、升级后触发全量重扫；
   - 签名的远程封禁列表加 kill switch，并且有防回滚；
   - 运行时拦截高危 API，触发后**永久熔断该哈希版本**，并且定期自检守卫补丁是否还在；
   - 给"合理但危险"的需求提供受控通道（Steam 票据必须绑定 serverIdentity），而不是一刀切禁止；
   - 自带改名的 Cecil，避免与 mod 自带的版本冲突；所有注册 API 都带 `modId`，可以按 mod 撤销，并在 UI 中标注来源；
   - 每个 Hook 订阅者单独 try/catch，一个 mod 出错不影响其他 mod。
6. **本地化的"英文兜底"工作流**：`GetLocalizedOrDefault(table, key, "English")` 让功能先上线、翻译后补，缺失键只记一次日志；SmartString 自动打标；性别/敬语标记；按 Unicode 区间切换 CJK 字体。这些都是多语种独立游戏的高性价比做法。
7. **可观测性**：StallWatchdog（后台线程、面包屑、直接写文件）是排查"卡死"反馈的极低成本方案；`[模块]` 前缀日志加 `LogAudit` 一行一个 mod 的审计输出；每个安全 UI 都有 `mod-preview-*` 命令可以预览。
8. **工具投入**：PaperEditor（WYSIWYG、热重载、撤销、导出代码）和 UI3 预制体生成器，说明团队愿意为"重复劳动"写工具，并且**让开发工具和 mod 工具共用一套**（Paper JSON、CustomUIBuilder、调试面板 API），一份投入有两份收益。
9. **营销需求进入代码**：DebugPanel 的 Marketing 分组、cinematic 模式、外观轮换，加上包内的 Unity Recorder，把"录预告片和截图"当作一项正式需求来支持。
10. **埋点设计**：所有事件带 run_id/day/spec/current_credit 等公共参数，重点埋在"风险事件"和"退出时机"（被查抄或被偷后多少天退出），适合经营模拟分析流失原因；并且有同意开关和环境区分。

### 9.2 技术债迹象

| 类别 | 现象 | 影响 |
|---|---|---|
| **原型遗留** | `GameMaster` 每局创建 Ship/Raid/Menu/Turn 管理器并每帧 Validate；`shipManager` 被商店代码引用 195 次（能量电池、`nextPOI`、`isTransit`）；30 个太空遭遇 Trigger；`DevMode` 战斗 buff；`CombatAbility` 至今仍是 `GameItem` 的接口之一 | 新人理解成本高、每帧都有无谓的开销、命名误导（商店的"能量"其实挂在飞船管理器下） |
| **God Object** | `PlayerStore` 4213 行，约 160 个可序列化字段，被引用 3773 次；`EmporiumEntry` 被引用 1137 次 | 改一处牵动全身，存档结构与运行时结构强耦合 |
| **三代 UI 并存** | PixelElement / 场景 UIManager / UI3 生成预制体 / CustomUIBuilder 四套写法；`RichTextBuilder` 中留有 `*Old/*Test` 方法和空的 `RichTableBuilder` | 风格不统一，新功能该用哪一套没有明确规则 |
| **注册规范不统一** | 物品用 `Directory<T>`（场景 MonoBehaviour）；顾客用静态字典；事件用 Blueprint 列表；升级、特长、难度又各有一个 Registry | mod API 不得不对每一种单独封装 |
| **废弃的扩展机制** | `PriorityTrigger` 反射行为体系只有 1 个空实现和 1 个"挠痒痒"测试实现；只扫描本程序集，所以 mod 用不了 | 死代码；"按目录放行为脚本"的设计没有落地 |
| **仪式性/错误代码** | 29 处实例 `Awake` 上标了 `[RuntimeInitializeOnLoadMethod]`（不会生效）；`Directory.Add` 的行为与日志相反；`mipLevel = sqrt(gridSize)` 只是碰巧成立；`StaticClass.call()`、`NewBehaviourScript`、`TestCoin/TestTraderStationEventTrigger`、Febucci/QC 的 Demo 都进了正式包 | 小坑，但能看出缺少代码评审 |
| **弃用内容未删除** | `UnusedDirectory`（85 个物品 / 2734 行）仍然注册；还有 `StoreClientListUnused`、`EventUnused`、`IntelUnusedHelper`；`KeyItemDirectory/OrganDirectory/RuinedMachineDirectory/ShipItemDirectory` 是空壳或零注册 | 包体与认知负担；不过保留 id 也可能是为了老存档兼容 |
| **吞异常** | 26 个空 `catch {}`（`ItemBehaviourManager`、`EventTriggerManager` 全部 try/catch 静默） | 隐藏 bug |
| **字符串类型系统** | 约 845 个字面量 Tag 键、图集路径字符串（`"Items/items_tool"` 出现 82 次）、`Resources.LoadAll` 按路径加载 | 拼写错误要到运行时才暴露，重命名资源有风险 |
| **本地化文本进存档** | 物品名和描述在创建时解析成当前语言并写入存档 | 切换语言后老物品不跟着变；存档体积变大 |
| **JSON 套 JSON** | 背包先用 `JsonUtility` 序列化成字符串，再作为 ES3 字段写入 | 无法增量写入，调试时需要二次解析，体积膨胀 |
| **无存档迁移** | 只有 `saveVersion` 字符串，没有迁移代码 | 目前靠工厂重建兜底，大的结构变更会比较痛苦 |
| **依赖冗余** | Hot Reload、Recorder、VisualScripting、CsvExport、Demo DLL、Addressables（只给 Localization 用）都在包里 | 包体变大；同时扩大了 mod 扫描器眼中"游戏程序集"的白名单 |
| **Mod 安全的残余风险** | 进程内全信任；静态扫描对计算型参数只能降级处理；信任记录与普通设置共用 ES3 默认文件；指纹不覆盖资源文件；DevTools 用一个 txt 就能开启 | 对独立游戏来说已经超出平均水平，但本质上是"安全带"而不是沙箱，开发者在免责声明 UI 中也是这么定位的 |
| **单程序集** | 944 个文件全在全局命名空间、一个 Assembly-CSharp 中，没有 asmdef | 编译慢（所以引入了 Hot Reload）；mod 能看到并 patch 所有内部类型，公共 API 和内部实现之间没有边界 |

### 9.3 一句话
这是一个**从原型框架上长出来的、程序主导的内容型独立游戏**：内容硬编码、组合靠委托和 Tag、UI 新旧几代叠在一起；但在**存档健壮性、mod 安全、本地化流程、可观测性和工具化**这些上线后才会遇到的问题上投入很深，非常适合作为小团队"先把游戏做出来，再补平台化能力"的参考样本。

---

## 附录 A：关键文件索引

| 主题 | 文件 |
|---|---|
| 数据目录 | `Directory.cs`, `DirectoryMaster.cs`, `DirectoryEntry.cs`, `ItemDirectory.cs`, `MiscItemDirectory.cs`, `ModItemDirectory.cs`, `StoreClientListDict.cs`, `StoreEventBlueprint.cs` |
| 物品模型 | `GameItem.cs`(:339 SetSpriteAndShape, :348 BuildShapeFromSprite), `GameItemFunc.cs`, `InvokeFuncExtensions.cs`, `TagSystem.cs`, `TagState.cs`, `ItemFeature.cs`, `ItemFeatureList.cs`, `GraphNodeStorage.cs`, `GraphUtils.cs` |
| 行为/事件 | `PriorityTrigger.cs`, `ItemBehaviourManager.cs`, `ItemTargetBehaviour.cs`, `Directory.Item.Behaviour.Target/TestTargetBehaviour.cs`, `EventTrigger.cs`, `EventTriggerManager.cs`, `CustomEventHandler.cs`, `ModHook.cs`, `StoreEventManager.cs` |
| UI | `PixelElement.cs`, `TreeNodeRender.cs`, `PixelWindow.cs`, `WindowsHandler.cs`, `RenderHandler.cs`, `RichTextBuilder.cs`, `RichText.cs`, `CustomUIManager.cs`, `CustomUIBuilder.cs`, `TabletUI.cs`, `TabletFrame.cs`, `TabletStyle.cs`, `LetterboxManager.cs` |
| 存档 | `SaveManager.cs`, `SaveFiles.cs`, `SaveItemNode.cs`, `SaveState.cs`, `PlayerStore.cs`(~:878 编码库存, ~:947 WriteSlotFile, ~:985 读档) |
| Mod | `ModLoader.cs`, `ModScanner.cs`, `ModCapability.cs`, `ModTrust.cs`, `ModTrustRecord.cs`, `ModFingerprint.cs`, `ModTrustList.cs`, `ModSteamGuard.cs`, `ModSteamRules.cs`, `ModSteamAuth.cs`, `ModWorkshop.cs`, `ModHelper.cs`, `ModHook.cs`, `ModLootFile.cs`, `ModLoot*Json.cs`, `ModEnablePromptUI.cs`, `ModWarningUI.cs`, `ModRefusalUI.cs`, `ModSecurityDebug.cs`, `ModSourceHelper.cs`, `LootRegistry.cs`, `PaperBookRegistry.cs` |
| 本地化 | `LocHelper.cs`, `LocSmartStringHelper.cs`, `LocGenderHelper.cs`, `ScriptFontSwap.cs`, `FontLoader.cs`, `CultureBootstrap.cs`, `TMPLocalizer.cs` |
| 调试 | `DevTools.cs`, `DevConsoleStripper.cs`, `DevMode.cs`, `DebugPanel.cs`, `CustomUIDebugPanel.cs`, `StallWatchdog.cs`, `LogWriter.cs`, `EmporiumAnalytics.cs`, `*Debug.cs`（QC 命令）, `PaperEditor*.cs` |

## 附录 B：可复现的统计命令

```bash
# 分类统计（规则见脚本）
python3 /tmp/ps_stats/classify.py
# 第三方库使用
cd ~/ps_work/src && grep -l 'using DG.Tweening\|Febucci\|QFSW\|RNGNeeds\|HarmonyLib\|EPOOutline' *.cs
grep -ho 'ES3\.[A-Za-z]*' *.cs | sort | uniq -c | sort -rn
# Directory 注册数
grep -hoE '^\s*Add\("[^"]+"' *Directory.cs StationMachinery.cs | wc -l
# Tag 键数量
grep -ohE '(GetTag|GetTagReadonly|EnableTag|DisableTag|IsTag|ModifyTag)[A-Za-z]*\("[A-Za-z0-9_]+"' *.cs | sed -E 's/.*\("//;s/"//' | sort -u | wc -l
```
