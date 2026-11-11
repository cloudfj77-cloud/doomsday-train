# Doomsday Train 骨架

ForgeaX + TypeScript，PC 2.5D 俯视角搜打撤项目。
本阶段仅实现几何地面、角色与 WASD 移动验证，不扩写战斗与经济。

- 设计真相：Documents/concept.md
- 复用流程：Documents/workflow.md
- 启动：powershell -File tools/preview.ps1
- 检查：powershell -File tools/check.ps1
- 反向验证：node tools/mutation-check.mjs
- 交互验收：powershell -File tools/verify.ps1

数值与内容修改 assets/world.json 和 assets/presentation.json 后重新预览。
