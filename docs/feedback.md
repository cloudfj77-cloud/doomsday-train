# Game Feedback

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
