# 换一台电脑开发《末日列车》（公司云电脑适用）

## 1. 先检查三件事（决定能不能在这台机器上跑游戏）

| 检查 | 方法 | 不满足怎么办 |
|-|-|-|
| 能访问 GitHub 和 npm | 浏览器打开 https://github.com 和 https://registry.npmjs.org/@forgeax/engine | 找 IT 开代理或白名单；npm 可改公司镜像 `pnpm config set registry <镜像地址>` |
| 浏览器支持 WebGPU | Chrome / Edge 打开 `chrome://gpu`，看 “WebGPU: Hardware accelerated”；或直接打开在线试玩 https://cloudfj77-cloud.github.io/doomsday-train/v0.2/ | 云电脑没有独立显卡时常见。可以只做写代码、单测、构建；画面验收回本机做，或用带 GPU 的云电脑规格 |
| 能装软件 | 能否装 Node / Git | 让 IT 预装下面的工具 |

## 2. 安装工具

| 工具 | 版本 | 说明 |
|-|-|-|
| Git | 任意新版 | Windows 用 Git for Windows（自带 Git Bash，构建脚本要用） |
| Node.js | 22.x（本机 22.22） | 自带 corepack |
| pnpm | 12.15.0 | `corepack enable && corepack prepare pnpm@12.15.0 --activate` |
| Python | 3.x | 构建脚本解析结果用；Windows 装好后命令叫 `python` 也行 |
| Chrome 或 Edge | 113+ | 运行游戏与自动验收 |

ForgeAX 引擎和命令行工具是 npm 包 `@forgeax/engine@0.4.19`，`pnpm install` 时自动装，不需要单独安装。

## 3. 拉代码

```bash
git clone https://github.com/cloudfj77-cloud/doomsday-train.git
cd doomsday-train
git checkout demo/v0.2        # v0.2：团队文档版 + 拆解玩法（还没合进 main）
# git checkout main           # 队友 SL_Skuy 的 3D 站点探索在 main
pnpm install
```

要推代码：在云电脑上生成 SSH key（`ssh-keygen -t ed25519`），把公钥加到 GitHub 账号；或用 HTTPS + Personal Access Token。

## 4. 日常命令

在 macOS / Linux 终端，或 Windows 的 **Git Bash** 里执行：

```bash
pnpm exec vitest run assets/sim                      # 经营与探索逻辑单测（22 项）
pnpm exec tsc --noEmit                               # 类型检查
bash scripts/build.sh                                # 打包到 dist/
pnpm exec forgeax project preview --port 4195        # 本地预览 http://localhost:4195/（注意用 localhost）
node qa/v02.cjs "http://localhost:4195/?qa=1&seed=3" # 真实输入验收（19 项，截图在 artifacts/v02/）
pnpm exec tsx scripts/balance-sim.mts 300            # 平衡模拟
python scripts/gen-i18n.py                           # 改界面文字后重新生成语言表
```

- 验收脚本自动找 Chrome / Edge；找不到时设置 `CHROME_PATH`（例：`set CHROME_PATH=C:\Program Files\Google\Chrome\Application\chrome.exe`）。
- Windows 也可以用队友写的 `preview.cmd` / `scripts/preview.ps1` 启动预览（main 分支上）。

## 5. 发布在线试玩

```bash
bash scripts/publish-pages.sh          # 根路径 /doomsday-train/（会覆盖 gh-pages，慎用）
```

v0.2 当前发布在 `gh-pages` 分支的 `v0.2/` 子目录。需要 SSH 推送权限。

## 6. 规则与约定

- 所有界面文字只改 `scripts/gen-i18n.py`，中英两套键必须一致；不要写死在代码里。
- 语言表键名禁止 `prototype` / `constructor` / `__proto__`。
- UI 样式受引擎限制：`outline: dashed`、`text-overflow` 等不支持，构建会报错并指出行号。
- 改 `assets/sim/` 规则时同步补单测；交付前跑一遍 `qa/v02.cjs`。
- 设计文档：`docs/v0.2-plan.md`；团队策划文档在飞书《00 功能文档总表｜末日列车》。
