# 3D station exploration — 2026-11-10

Implemented requirements:
- Native Engine 3D station scene, using existing voxel palette, materials, dusk environment, ruins, trees and train assets.
- Camera uses the exact CAMERA_POSE orientation, target offset and FOV of the train view, translated with the explorer; returns to authored train camera on extraction.
- Three generated enterable buildings: warehouse, workshop and clinic. Doorways, cutaway foreground walls, solid collision, internal furnishings and E-to-survey state.
- WASD/arrows in the native Update system, screen-aligned movement, normalized diagonals and collision sliding.
- Train exit, minimap, building survey progress; return restores the station and departure action.
- Same autosave retains position and surveys across reloads. Legacy flat-map saves migrate to the 3D entrance.
- No resource reward, train ledger advancement or repeat sortie on successful return.

Validation on final game code:
- pnpm test: 11 passed.
- pnpm typecheck: passed.
- project play scripts/playthrough.mjs --require-ui --timeout-ms 180000 --json: PASSED, hardware Edge through FORGEAX_BROWSER_EXECUTABLE; 80 seconds, no console/page/input errors, layout warnings empty at final 1280x720 and 1280x600 frames.
- Real keyboard input entered and surveyed all three buildings, saved/reloaded, returned to train, reloaded again and departed to the next station.
- Named evidence: artifacts/play/scripts/playthrough/01-01-arrived-train.png; 02-02-3d-station-entry.png; 03-03-interior-warehouse.png; 04-03-interior-workshop.png; 05-03-interior-clinic.png; 06-04-exploration-reload.png; 07-05-returned-train.png; 08-06-next-station.png.
- Visual review: warm dusk matches train scene; palette is deliberately muted like existing assets; pale uniform and armband identify the player; buildings occupy the centre view with unobstructed room interiors; ground and distant ruins cover the scene boundary. HUD preserves the central play area.
- project check still blocked by installed pnpm 11.25.0 vs declared >=12.15.0 <13. No package-manager requirement was weakened.

Deferred: loot, combat, actual loadout/companions, extraction hold, remote exit and death recovery. Surveying buildings awards no inventory or progression.
