# Exploration camera correction — 2026-11-11

Reference: docs/teardowns/escape-from-duckov/report.html §4.4.
- Pitch 55 degrees, camera arm yaw -30 degrees from world +Z, arm length 45m, vertical perspective FOV 20 degrees. Configuration is in assets/shared/exploration-camera.ts.
- Follow anchor: character position plus 1m vertical pivot. Fixed rotation, independent of character facing. Offset is computed from spherical arm coordinates and added to the pivot, rather than reusing the train camera pose.
- Mouse ground ray controls character facing. Lookahead uses the difference between the mouse ground ray and viewport centre ground ray, capped to 5m and exponentially smoothed. It does not feed the existing camera offset back into the target. A centred cursor returns to the character pivot.
- Camera-relative keyboard movement derives from the same exploration arm basis.
- Returning to the train restores original camera pose and FOV 38 degrees. No ADS or weapons added.

Verification: 14 tests passed, typecheck passed, final Engine project play passed in hardware Edge in 86.7 seconds; no browser errors or layout warnings. Read projection asserted actual pose offset distance, pitch, yaw, FOV, centred lookahead, bounded offset, and FOV restoration on return. Full control/exploration/reload/depart playthrough also passed. Opened screenshots confirm centred character, high-angle miniature perspective, room visibility, and restored train view.
Evidence: artifacts/play/scripts/playthrough/02-camera-centred-55pitch-minus30yaw-45distance-20fov.png; 03-controller-mouse-facing-strafe.png; 06-03-interior-warehouse.png; 10-05-returned-train.png.
