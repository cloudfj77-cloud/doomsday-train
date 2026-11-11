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
