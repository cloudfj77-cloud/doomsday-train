# Duckov-inspired station flow — 2026-11-11

References: `docs/teardowns/index.html` Duckov map section and
`docs/teardowns/escape-from-duckov/report.html` sections 5–6. This adapts their
district exploration and OcclusionFade principles to the train station rather
than importing the extracted game meshes.

Requested coverage:
- Map flow: train forecourt → freight warehouse → service workshop → medical
  compound → train. This is suggested ordering; early extraction remains possible.
- Two side lanes, a cross street and northern connection allow circular routes.
  Roads and physical cover are authored once for cooked geometry/collision;
  the minimap derives its roads from the same data.
- Distinct warehouse, workshop and medical roof shells; upper foreground walls
  join each shell. Approaching the doorway hides only that building's shell;
  leaving restores it. Interior furniture, lower walls and collision remain.
- Existing save, survey and train-return behavior remains; no new loot or combat
  system is claimed by this change.

Verification:
- `project check`: passed using a writable temporary history directory.
- `pnpm test`: 16 passed, including flood-fill accessibility of every room and
  both return lanes for all three layout variants.
- `pnpm typecheck`: passed.
- `scripts/playthrough.mjs` asserts roof presence outside, per-room hiding and
  restoration, real keyboard survey, northern/western loops, and reload/return.
  Named screenshots: `map-loop-route-roofs-exterior`, `03-interior-*`,
  `roof-restored-*`, `map-northern-loop`, `map-return-lane`,
  `04-exploration-reload`. These are planned checkpoint names, not captured evidence.
- Runtime acceptance remains unverified: default browser startup failed because
  Playwright Chromium was absent. Retrying with installed Edge remained at
  `opening the dev runtime and browser` through 630 seconds without screenshots;
  the stalled run was cancelled. No visual review, input or reload pass is claimed.
