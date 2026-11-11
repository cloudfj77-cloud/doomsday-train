# Doomsday Train · 风沙铁路补给点

ForgeaX + TypeScript，PC 2.5D 俯视角搜打撤项目。
首场景是沙漠中的铁路遗迹。用 WASD 从浅沙地沿半埋轨枕向信号牌移动，绕过中央断岩可抵达补给台。

- 设计真相：Documents/design/concept.md
- 体素美术与首场景：Documents/design/voxel-art.md
- 复用流程：Documents/workflow/development.md
- 启动：powershell -File tools/preview.ps1
- 检查：powershell -File tools/check.ps1
- 反向验证：node tools/mutation-check.mjs
- 交互验收：powershell -File tools/verify.ps1

数值与内容修改 config/gameplay/world.json 和 config/presentation/scene.json 后重新预览。

