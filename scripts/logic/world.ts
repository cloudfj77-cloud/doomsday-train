type Pair = readonly [number, number];
export interface Surface { id: string; center: number[]; size: number[]; pitch: number }
export interface Config {
  speed: number; runSpeed: number; limit: number; radius: number; height: number; skin: number;
  maxSlope: number; stepHeight: number; stepWidth: number; snapDistance: number; gravity: number;
  turnTime: number; aimDeadzone: number; start: number[]; ground: number[];
  obstacles: { id: string; center: number[]; size: number[]; height: number }[]; surfaces: Surface[];
}
const vector = (v: unknown, n: number): v is number[] => Array.isArray(v) && v.length === n && v.every(Number.isFinite);
export function validateConfig(value: unknown): Config {
  const c = value as Config;
  if (!c || !['speed','runSpeed','limit','radius','height','skin','maxSlope','stepHeight','stepWidth','snapDistance','gravity','turnTime','aimDeadzone'].every(k => Number.isFinite(c[k as keyof Config]) && Number(c[k as keyof Config]) > 0)
    || c.height <= c.radius * 2 || c.maxSlope >= 90 || c.runSpeed < c.speed || !vector(c.start,2) || c.start.some(n=>Math.abs(n)>c.limit)
    || !vector(c.ground,2) || !c.ground.every(n=>n>c.limit*2) || !Array.isArray(c.obstacles) || !Array.isArray(c.surfaces)) throw Error('invalid-world-config');
  const ids = new Set<string>();
  for (const o of [...c.obstacles,...c.surfaces]) {
    const surface = 'pitch' in o;
    if (!o.id?.trim() || ids.has(o.id) || !vector(o.center,surface?3:2) || !vector(o.size,surface?3:2) || !o.size.every(n=>n>0)
      || (surface ? !Number.isFinite(o.pitch) : !Number.isFinite(o.height) || o.height<=0)) throw Error('invalid-world-config');
    if (!surface && c.start.every((n,i)=>Math.abs(n-o.center[i]) < o.size[i]/2+c.radius)) throw Error('invalid-world-config');
    ids.add(o.id);
  }
  return c;
}
export function createState(c: Config) {
  return { x:c.start[0], y:c.skin, z:c.start[1], facingX:0, facingZ:1, yaw:0,
    moving:false, running:false, grounded:false, walkDistance:0, vx:0, vy:0, vz:0, shadowY:0 };
}
export type State = ReturnType<typeof createState>;
export interface Intent { axis: Pair; run: boolean; aim?: Pair }
export function screenMovement(axis: Pair, yawDegrees: number): Pair {
  const length=Math.max(1,Math.hypot(...axis)), a=yawDegrees*Math.PI/180;
  return [(axis[0]*Math.cos(a)-axis[1]*Math.sin(a))/length,(-axis[0]*Math.sin(a)-axis[1]*Math.cos(a))/length];
}
export function desiredMovement(s: State, input: Intent, dt: number, c: Config, cameraYaw: number) {
  if (!Number.isFinite(dt) || dt<=0 || !input.axis.every(Number.isFinite)) throw Error('invalid-input');
  const [x,z]=screenMovement(input.axis,cameraYaw), speed=input.run?c.runSpeed:c.speed;
  return [s.grounded?x*speed*dt:0,((s.grounded?0:s.vy)-c.gravity*dt)*dt,s.grounded?z*speed*dt:0] as const;
}
export function resolveMovement(s: State, position: readonly number[], grounded: boolean, input: Intent, dt: number, c: Config): State {
  const dx=position[0]-s.x,dz=position[2]-s.z, distance=Math.hypot(dx,dz);
  let yaw=s.yaw;
  if(input.aim && Math.hypot(input.aim[0]-position[0],input.aim[1]-position[2])>c.aimDeadzone) {
    const target=Math.atan2(input.aim[0]-position[0],input.aim[1]-position[2]);
    yaw += Math.atan2(Math.sin(target-yaw),Math.cos(target-yaw)) * (1-Math.exp(-dt/c.turnTime));
  }
  const moving=grounded && distance>1e-5;
  return {...s,x:position[0],y:position[1],z:position[2],yaw,facingX:Math.sin(yaw),facingZ:Math.cos(yaw),grounded,
    vx:dx/dt,vy:grounded?0:(position[1]-s.y)/dt,vz:dz/dt,moving,running:moving&&input.run,
    walkDistance:s.walkDistance+(moving?distance:0)};
}
export function project(state: State): Readonly<State> { return Object.freeze({ ...state }); }
