《逃离鸭科夫》Escape from Duckov —— 拆解全量（拆解于 2026-11-07，快照 2026-11-12）
仅供内部学习研究，代码与美术版权归 TeamSoda / bilibili，禁止公开、转发、商用，游戏里不得直接使用。

duckov_re/report/index.html     拆解报告（代码 + 美术，双击打开）
duckov_re/src/                  反编译 C# 工程
duckov_re/art/                  美术提取：sprites/ 1272 个图标切片，tex/ 299 张贴图样本，mesh/ 16 个模型(OBJ)，text/ 71 份本地化与配置表，scene/ 宣传图+小地图，preview/ 总览图，统计 JSON
duckov_re/viewer/               9 张地图的 3D 场景模型(glb)与浏览器资源
duckov_re/explorer.html         3D 场景浏览器（需要用本地服务打开：在 duckov_re 目录运行 python3 -m http.server 8000，再访问 http://localhost:8000/explorer.html）
duckov_re/shots/                场景截图
duckov_re/*.py                  提取与建场景用的脚本
overview-hub/                   两款游戏的总览页（index.html）

未拆：238 段动画与 37 个状态机、196 个蒙皮角色完整模型、物品/任务数值表逐项、FMOD 音频、3182 个粒子系统、贴图（4381 张只导出 299 张）和模型（3887 个只导出 16 个）的全量。
