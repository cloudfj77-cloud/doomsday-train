# Game Feedback

### 2026-11-11 — Project play startup exceeded the advertised startup budget
- Scope: SDK / browser; Engine 0.4.19, Windows, installed Edge via `FORGEAX_BROWSER_EXECUTABLE`.
- Reproduction: `pnpm exec forgeax project play scripts/playthrough.mjs --require-ui --timeout-ms 180000 --json`.
- Expected: a ready frame, or a bounded startup failure (progress advertised 120 seconds startup).
- Actual: progress remained `opening the dev runtime and browser` through 630 seconds, without a play checkpoint or screenshot. Cancelled with Ctrl+C, exit 1. Root cause is undiagnosed; this does not establish a game rendering defect.
- Default Chromium attempt separately returned `play-not-ready` / `browser-capture-failed` because the Playwright browser executable was not installed.
- Status: blocked; runtime/visual acceptance remains unverified.

Record confirmed Engine, SDK, template, build, asset, runtime, or browser issues
encountered while developing this game. This file and the root `README.md` are
shipped unchanged in the Web ZIP. Keep credentials and private URLs out of it.

```markdown
### YYYY-MM-DD — Short issue title

- Scope: Engine | SDK | template | build | asset | runtime | browser
- Reproduction: exact command or smallest user-visible sequence
- Expected: what should happen
- Actual: what happened
- Evidence: log, screenshot, URL, or commit path
- Status: open | workaround | fixed | blocked
```

### 2026-11-06 — UI localization rejects the key `prototype` with a generic error
- Reproduction: a `*.ui.i18n.json` containing a nested key named `prototype` (e.g. `"cover": { "prototype": "..." }`).
- Expected/Actual: a diagnostic naming the forbidden key; actual is only `expected string-valued i18next resources with fallbackLng and defaultNS` at line 1 column 1 (`isUiLocalization` rejects `__proto__`/`prototype`/`constructor` keys silently).
- Workaround: rename the key (`protoTag`).

### 2026-11-11 — Project init injects an unpublished devkit version
- Scope: SDK / initialization, Engine 0.4.19, Node 24.19.0, pnpm 12.15.0 on Windows.
- Reproduction: `pnpm exec forgeax project init --json` in the existing game.
- Expected: initialize dependencies using the pinned Engine release.
- Actual: init adds `@forgeax/engine-devkit: 0.0.0` to devDependencies; installation fails with `ERR_PNPM_NO_MATCHING_VERSION` from the public npm registry.
- Recovery: removed only the invalid dependency injected by the failed command; kept the pinned Engine and lockfile.
- Status: blocked; initialization and runtime acceptance have not passed.

### 2026-11-11 — pnpm default workspace pattern mismatch
- Engine 0.4.19 project check reports project-pnpm-workspace-state-stale after official platform SDK installation and pnpm install.
- Manifest omits packages; pnpm 12 records workspacePackagePatterns ["."], while Engine compares omitted packages as [].
- Project workaround: explicitly declare packages: ['.'] in pnpm-workspace.yaml and reinstall. Do not modify installed records or Engine output.
- Browser acceptance on final scene: project play passed with installed Edge, keyboard movement and reload, no console/page errors. Earlier blocked report above is superseded for this scene.
