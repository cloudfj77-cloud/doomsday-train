# Character controller — 2026-11-11

Reference: docs/teardowns/escape-from-duckov/report.html, sections 4.1 and 4.4.

Implemented controls: WASD/arrows for camera-relative movement; cursor ground ray for independent facing; Shift held for running; Space pressed for directional dash (or facing direction when stationary); E for existing building/exit interactions. No weapons or weapon inputs were added.

Reference-backed behavior: movement/aim separation, ground-ray aiming, smooth camera look-ahead bounded at 5 units, dash cost 10 and cooldown 0.5 seconds, ordinary movement blocked during dash. Interactions cannot preempt dash in this limited controller. Exploration camera now uses report pitch 55°, arm yaw -30°, arm distance 45 and vertical FOV 20°. Train presentation camera remains separate. The camera pivots at character height 1 metre, translates with the character and adds a smoothed ground-plane lookahead of at most 5 metres measured relative to the viewport centre ray. A centred cursor yields zero lookahead, avoiding camera/aim feedback drift.

Provisional choices: report does not provide complete key bindings, run/walk speed, stamina capacity/drain/recovery, dash duration or curve samples. Bindings above are implementation defaults, not a verified extraction of Duckov bindings. Tuning is centralized in assets/shared/character-controller.ts: walk 4, run 6.5, stamina 100, run drain 12/s, recovery 20/s after 0.8s, dash duration 0.28s and speed 13 with a deceleration curve. Stamina is transient for this increment.

Architecture: pure controller state update, invoked in the native Engine Update system. Input comes from InputSnapshot; picking uses drawingBufferSize so Engine Workers do not require direct canvas ownership. Existing deterministic world collision is shared by walking, running and dash, with substeps to prevent tunnelling. Cursor direction changes facing independently from keyboard movement. Pose bob is distance-driven; dash has a compressed posture. No skeletal animation rig is introduced.

Final verification: 12 unit tests passed; typecheck passed; Engine project play passed in hardware Edge (87.6s), zero console/page/input errors and empty layout warnings. Real input asserted ground aim availability, movement, run speed/stamina and active dash/cost. The same run completed all buildings, reload persistence and return/depart regression.

Evidence: artifacts/play/scripts/playthrough/03-controller-mouse-facing-strafe.png, 04-controller-sprint-stamina.png, 05-controller-dash.png. These screenshots capture resulting positions and stamina; the transient dash action is asserted through the Engine read projection, as screenshot acquisition may complete after the dash finishes.
