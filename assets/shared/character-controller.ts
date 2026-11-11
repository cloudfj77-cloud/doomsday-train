import { walkSite } from './station-map.ts';

// Report-backed dash cost/cooldown; other values are first-pass tuning, not recovered Duckov values.
export const CHARACTER = { walk: 4, run: 6.5, stamina: 100, runDrain: 12, recovery: 20,
  recoveryDelay: 0.8, dashCost: 10, dashCooldown: 0.5, dashDuration: 0.28, dashSpeed: 13 } as const;
export interface CharacterState {
  x: number; z: number; yaw: number; stamina: number; cooldown: number;
  dashLeft: number; dashX: number; dashZ: number; recoveryDelay: number;
  action: 'idle' | 'walk' | 'run' | 'dash'; distance: number;
}
export interface CharacterInput { x: number; z: number; run: boolean; dash: boolean; aim?: { x: number; z: number } }
export function createCharacter(x: number, z: number): CharacterState {
  return { x, z, yaw: 0, stamina: CHARACTER.stamina, cooldown: 0, dashLeft: 0,
    dashX: 0, dashZ: 1, recoveryDelay: 0, action: 'idle', distance: 0 };
}
export function stepCharacter(s: CharacterState, input: CharacterInput, dt: number, seed: number): void {
  dt = Math.max(0, Math.min(dt, 0.1));
  s.cooldown = Math.max(0, s.cooldown - dt);
  s.recoveryDelay = Math.max(0, s.recoveryDelay - dt);
  const length = Math.hypot(input.x, input.z);
  const x = length ? input.x / length : 0, z = length ? input.z / length : 0;
  if (input.dash && !s.dashLeft && !s.cooldown && s.stamina >= CHARACTER.dashCost) {
    s.stamina -= CHARACTER.dashCost; s.dashLeft = CHARACTER.dashDuration;
    s.cooldown = CHARACTER.dashCooldown; s.recoveryDelay = CHARACTER.recoveryDelay;
    s.dashX = length ? x : Math.sin(s.yaw); s.dashZ = length ? z : Math.cos(s.yaw);
  }
  let dx = 0, dz = 0;
  if (s.dashLeft > 0) {
    const used = Math.min(dt, s.dashLeft);
    const speed = CHARACTER.dashSpeed * (0.65 + 0.7 * s.dashLeft / CHARACTER.dashDuration);
    dx = s.dashX * speed * used; dz = s.dashZ * speed * used;
    s.dashLeft = Math.max(0, s.dashLeft - dt); s.action = 'dash';
  } else {
    const running = length > 0 && input.run && s.stamina > CHARACTER.runDrain * dt;
    s.action = length ? running ? 'run' : 'walk' : 'idle';
    const speed = running ? CHARACTER.run : CHARACTER.walk;
    dx = x * speed * dt; dz = z * speed * dt;
    if (running) { s.stamina = Math.max(0, s.stamina - CHARACTER.runDrain * dt); s.recoveryDelay = CHARACTER.recoveryDelay; }
    if (input.aim && Math.hypot(input.aim.x-s.x, input.aim.z-s.z) > 0.2) s.yaw = Math.atan2(input.aim.x-s.x, input.aim.z-s.z);
    else if (length) s.yaw = Math.atan2(x,z);
  }
  const next = walkSite(seed,s.x,s.z,dx,dz);
  s.distance += Math.hypot(next.x-s.x,next.z-s.z); s.x=next.x;s.z=next.z;
  if (!s.recoveryDelay && s.action !== 'run' && s.action !== 'dash') s.stamina = Math.min(CHARACTER.stamina,s.stamina + CHARACTER.recovery*dt);
}
