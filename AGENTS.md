# 项目说明书

## 一句话游戏是什么
2.5D 俯视角搜打撤：进入 3D 地图搜索物品，撤出售卖赚金币，再进入下一局。设计真相：[concept](Documents/design/concept.md)。

## 目录地图
- `Documents/design/`：设计真相和玩法专题。
- `Documents/decisions/`：拍板记录及变更历史。
- `Documents/workflow/`：开发与验证流程。
- `scripts/logic/`：纯逻辑和只读状态投影，不依赖显示或 DOM。
- `scripts/view/`：读取投影绘制场景，不计算游戏规则。
- `assets/`：Pack 和资产源；根插件只负责装配。
- `config/gameplay/`：逻辑数值和出生配置。
- `config/presentation/`：画面与按键配置。
- `tests/`：逻辑测试、工程合同检查和交互验收。
- `tools/`：可重复启动、检查和验证脚本。
- `skills/`：引擎接口及工作流参考。
- `docs/`：引擎问题反馈。
- `qa/`：本地检查记录（不提交生成物）。

## 硬约定
- 数值、场景和内容配置放根目录 config/ 下按专题分类的 JSON，运行时加载和校验；禁止静默回退。
- 逻辑与画面分离，画面读取逻辑投影；引擎装配以 forge.json 为唯一 authority。
- 约定不能只写在注释里：前提被破坏会静默出错时，必须有无窗口检查。
- 截图只证明画出来了，不证明点得动。
- 每条检查都要反着验一次：故意改坏 → 看它报错 → 改回 → 通过。
- 检查调用真实代码，不在检查里把公式重新推一遍。
- 修一个 bug = 修复 + 一条新的检查。
- AGENTS.md 小于 20KB；玩法细节只写 Documents/ 专题并链接。
- Documents/ 下文档必须按专题目录分类；测试和验收脚本统一放 tests/，不得混入 scripts/ 正式代码。
- assets/ 内 JSON 仅用于资产元数据（*.meta.json）或 Pack（*.pack.json），游戏配置统一放 config/。

## 验证命令
基础环境：`pnpm exec forgeax project check --json`；类型：`pnpm typecheck`；测试：`pnpm test`。
稳定入口：`powershell -File tools/preview.ps1`；检查：`powershell -File tools/check.ps1`；反向验证：`node tools/mutation-check.mjs`；真实交互：`powershell -File tools/verify.ps1`。流程见 [workflow](Documents/workflow/development.md)；运行、交互与重载通过才可声明可玩。

## 拍板记录
见 [独立拍板记录](Documents/decisions/records.md)，后续记录仅维护该文档。

## Git 节奏
Git 提交信息使用中文。每完成一个阶段提交一次；仅提交本阶段文件；验证阻塞必须如实记录，不当成通过。

---
# ForgeaX Game Project Guide

AI-first TypeScript game on `@forgeax/engine`. Hard rules and routing here;
open a reference only when its task applies. Never read whole large `SKILL.md` files;
stop after their short start section unless the task needs more.

## Start here

1. Imports and signatures: [the API index](skills/forgeax-engine-sdk/references/api-index.md) before searching `node_modules`.
2. Effects in an `empty` project: [the effects cookbook](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/effects-cookbook.md)
   indexes topics; open ONLY one.
3. Commands: the Quick contract atop [`forgeax-engine-cli`](skills/forgeax-engine-cli/SKILL.md);
   exact inputs come from `pnpm exec forgeax help <path> --json`, never guesses.
4. Pick the route in [Run, test, build, and release](#run-test-build-and-release), then edit.
5. Rendering has native equivalents for most Three.js core features: look an effect up in
   [the Three.js equivalence guide](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/three-equivalents.md) (with gaps) before simplifying it.

## Hard rules

- `forge.json` owns project configuration and `forge.json#roots` is the sole plugin
  assembly authority; stable GUIDs own asset identity. Never add parallel manifests,
  lifecycles or runtime entrypoints.
- Assets: author source → importer/cooker → DDC → catalog → `assets.load(guid, kind)`.
  Author in `assets/` (`*.pack.ts`, `*.pack.json`, `<file>.meta.json`); never edit `dist/`,
  `pack-index.json`, DDC or `.forgeax/`, or fetch assets manually.
- Content made during play skips Packs: bind `ctx.assets.create(...)` output, release it
  from the effect ([`forgeax-engine-assets`](skills/forgeax-engine-assets/SKILL.md#runtime-identity-and-content)).
- Each Pack `build()` key is a `sourceKey`: lowercase segments matching
  `^[a-z0-9][a-z0-9._-]*(/[a-z0-9][a-z0-9._-]*)*$` (`mesh/crate`). GUID =
  `UUIDv5(packageId, sourceKey)`. ScriptablePack files end in `.pack.ts`.
- Import only `@forgeax/engine` or `@forgeax/engine/<package-directory>`, never
  `@forgeax/engine-*`.
- Add dependencies with `pnpm add <name>`, not by editing `package.json`. If a frozen
  lockfile rejects an edit or `packageManager` change, run `pnpm install --no-frozen-lockfile`.
- Game logic is a native Cordis `Plugin`; cleanup belongs to `ctx.effect()`. DOM, Web
  Audio and browser SDKs belong to the frontend root, never Engine Workers.
- Expected failures return `Result`: branch on `.code`, keep `.expected`/`.hint`/`.detail`.
- Run `pnpm exec forgeax <path> --json`; add `--detail` only for `omitted.fields` you need.
- `game-3d` content is a reference, not the game: record its
  [Template Disposition](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/game-project-guide.md#game-3d-template-disposition) first.
- Before delivery, list each requested requirement; the playthrough asserts each through
  the read projection and saves one named screenshot per requirement (every ending, each
  object type, multi-enemy states, the core input's visible effect; failure and debug
  frames belong in `.forgeax/diag/`, not `artifacts/`). Never bend game code
  for the test (enemies spawned ahead, exposed aim targets). See
  [Requirement coverage](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/game-delivery.md#requirement-coverage).
- Append confirmed Engine/SDK issues to `docs/feedback.md` at once (no secrets);
  `project package` ships it.
- Edit this project directly. A ForgeaX closed loop needs explicit user agreement.
- Capture with [`forgeax-engine-rhi-debug`](skills/forgeax-engine-rhi-debug/SKILL.md) only for
  unexplained black frames, missing objects, wrong lighting or flicker, before editing
  shaders; never to deliver.

## Run, test, build, and release

| User intent | Route |
|:--|:--|
| Create, change or locally play (default) | [Local playable delivery](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/game-delivery.md#local-playable-delivery) |
| Publish, deploy or distribute | [Game release](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/game-delivery.md#game-release) |
| Engine/SDK change, SDK publication, performance | Owning package, [SDK production](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/sdk-production.md), performance skill |

```bash
pnpm exec forgeax project check --json
pnpm test && pnpm typecheck
pnpm exec forgeax project play tests/playthrough.mjs --json
```

Accept local delivery only on the final code: `project play` (or `dev verify`) passes; you opened the PNG, see the requested scene/UI and passed the
[visual self-review](skills/forgeax-engine-sdk/SKILL.md#visual-self-review); you drove
the core input and saw its result; both survive a reload. A build, URL, HTTP 200 or unit
test alone accepts nothing. A blocked check stays unverified: report it, never a pass.

Evidence comes only from these commands' owner browser. Hand over the `dev status` URL;
never open or drive another tab for acceptance (it may be backgrounded; its
`live-bridge-not-owner` warning is expected). Run `project play` and `dev verify` blocking
in one call with a large exec yield (21+ min at default budgets: ~14 per play, +7 when queued),
or lower `--timeout-ms`; never background or poll them. Each prints one JSON result and at
most one stderr progress line per 30 s. The live owner is optional: `dev` observations start
it on demand; upkeep is only `dev start`/`dev stop`. For timers or long waits, call
`play.advance(seconds)`, not `play.wait`/long `waitFor` timeouts. Fix its `layout` warnings (HUD text overlapping
or clipped at 1280x720/1280x600) before committing.

On an unchanged `empty` template, `play-runtime-failed` with reason `flat-canvas` is the
expected start, not an environment failure.

## Task routing

| To | Read |
|:--|:--|
| Layout, SDK setup, skill links, Engine model, View | [project guide](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/game-project-guide.md) |
| Asset forms, import, verify | [Asset authority](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/game-project-guide.md#asset-authority-and-formats), then `forgeax-engine-assets` |
| Roots, plugins, third-party SDK startup | [Game code](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/game-project-guide.md#game-code-and-plugins), `forgeax-engine-app` |
| Pick Engine capabilities | [SDK skill](skills/forgeax-engine-sdk/SKILL.md) table; feature catalog on demand |
| Camera, lights, shadows, post | Quick contract of `forgeax-engine-render-pipeline` |
| Materials, textures, WGSL | `forgeax-engine-material`, `forgeax-engine-shader` |
| Input, frame loop, HUD, pointer lock | `forgeax-engine-app`; cursor games call `gameHost.setPointerLockAllowed(false)` once |
| Physics, character motion | `forgeax-engine-physics` |
| Blocked checks, Linux headless, release | [delivery guide](https://github.com/ForgeaXGame/forgeax-engine-harness/blob/main/docs/engine/skills/forgeax-engine-sdk/references/game-delivery.md) |
| Scripted playthrough and acceptance | `forgeax project play` in the CLI skill quick contract; edit `tests/playthrough.mjs` |



