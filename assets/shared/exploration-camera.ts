/** Duckov report §4.4. Yaw is the camera arm's azimuth from world +Z. */
export const EXPLORATION_CAMERA = { pitch: 55, yaw: -30, distance: 45, fov: 20, aimOffset: 5, pivotHeight: 1 } as const;
const rad = Math.PI / 180;
export const EXPLORATION_OFFSET: [number, number, number] = [
  Math.sin(EXPLORATION_CAMERA.yaw * rad) * Math.cos(EXPLORATION_CAMERA.pitch * rad) * EXPLORATION_CAMERA.distance,
  Math.sin(EXPLORATION_CAMERA.pitch * rad) * EXPLORATION_CAMERA.distance,
  Math.cos(EXPLORATION_CAMERA.yaw * rad) * Math.cos(EXPLORATION_CAMERA.pitch * rad) * EXPLORATION_CAMERA.distance,
];
/** Relative to the viewport centre ray, not the player: avoids camera/aim positive feedback. */
export function aimLead(dx: number, dz: number) {
  const scale = Math.min(1, EXPLORATION_CAMERA.aimOffset / Math.max(0.001, Math.hypot(dx, dz)));
  return { x: dx * scale, z: dz * scale };
}
