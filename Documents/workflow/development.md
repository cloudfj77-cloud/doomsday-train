# 首场景开发与验收

- `powershell -File tools/preview.ps1`：调用已安装造化插件的官方 CLI，构建并打开 PC 预览；无需对话。
- `powershell -File tools/check.ps1`：环境、类型和真实逻辑检查，失败返回非零。
- `powershell -File tools/verify.ps1`：以上检查后用引擎 owner 浏览器执行真实按键、状态投影、截图和重载验收。
- 移动使用 WASD。首场景验收覆盖出生、跨轨、绕岩、抵达补给台和重载后的输入。
- `config/gameplay/world.json` 配置速度、边界、出生位置；`config/presentation/scene.json` 配置画面和按键。
- `scripts/logic/world.ts` 是纯状态变换；`scripts/view/scene.ts` 只消费投影；Pack 装配输入、逻辑和显示。
- 造化插件升级后若旧路径不存在，启动脚本明确失败；按已安装版本更新路径。
- 正向检查后，对实际源码做临时破坏验证检查会失败，必须在 finally 中恢复，再运行正向检查。禁止把破坏版本提交。

在 1280×720 与 1280×600 的实际游玩视角检查前景遮挡、角色步态和信号牌辨识度。检查玩家位置与朝向的只读投影，再查看真实按键产生的截图。

