import type { sceneConfig as scene } from '../config/game.ts';
export type CameraConfig = typeof scene.camera;
export type Zone = typeof scene.zones[number];
type Point = readonly number[];
export type Parameters = Pick<CameraConfig,'pitch'|'distance'|'fov'|'aimOffset'>;
const fields = ['pitch','distance','fov','aimOffset'] as const;
export function validateCamera(c: CameraConfig, zones: Zone[]) {
  if (!c || !['pitch','distance','fov','aimOffset','followTime','aimTime','zoneTime','aspect'].every(k=>Number.isFinite(c[k as keyof CameraConfig]) && Number(c[k as keyof CameraConfig])>0)
    || c.pitch>=90 || c.fov>=180 || c.yaw!==-30 || !Array.isArray(zones)) throw Error('invalid-scene-camera');
  const ids=new Set<string>();
  for(const z of zones) {
    if(!z.id?.trim() || ids.has(z.id) || ![z.center,z.size].every(v=>Array.isArray(v)&&v.length===2&&v.every(Number.isFinite))
      || !z.size.every(v=>v>0) || !Number.isFinite(z.blend) || z.blend<=0 || !Number.isInteger(z.priority)
      || !z.camera || !fields.every(k=>Number.isFinite(z.camera[k])&&z.camera[k]>0) || z.camera.pitch>=90 || z.camera.fov>=180) throw Error('invalid-camera-zone');
    ids.add(z.id);
  }
}
export function groundPoint(ray: Point | undefined, y: number): [number,number] | undefined {
  if(!ray || ray.length!==6 || !Array.from(ray).every(Number.isFinite) || Math.abs(ray[4])<1e-6) return;
  const t=(y-ray[1])/ray[4];
  if(t<0) return;
  return [ray[0]+ray[3]*t,ray[2]+ray[5]*t];
}
export function regionParameters(position: Point, c: CameraConfig, zones: Zone[]) {
  const weights=zones.map(z=>({id:z.id,priority:z.priority,weight:Math.max(0,Math.min(1,
    Math.min(z.size[0]/2-Math.abs(position[0]-z.center[0]),z.size[1]/2-Math.abs(position[2]-z.center[1]))/z.blend)),camera:z.camera}));
  const parameters: Parameters={pitch:0,distance:0,fov:0,aimOffset:0};
  let remainder=1;
  for(const priority of [...new Set(weights.map(w=>w.priority))].sort((a,b)=>b-a)) {
    const group=weights.filter(w=>w.priority===priority), total=group.reduce((n,w)=>n+w.weight,0);
    const coverage=Math.min(1,total)*remainder;
    if(total>0) for(const k of fields) parameters[k]+=group.reduce((n,w)=>n+w.camera[k]*w.weight,0)/total*coverage;
    remainder-=coverage;
  }
  for(const k of fields) parameters[k]+=c[k]*remainder;
  return {parameters,weights:weights.map(({id,weight})=>({id,weight}))};
}
export function cameraPosition(target: Point, p: Parameters, yaw: number): number[] {
  const pitch=p.pitch*Math.PI/180, angle=yaw*Math.PI/180, horizontal=p.distance*Math.cos(pitch);
  return [target[0]+Math.sin(angle)*horizontal,target[1]+Math.sin(pitch)*p.distance,target[2]+Math.cos(angle)*horizontal];
}
export function createCamera(position: Point, c: CameraConfig, zones: Zone[]) {
  const {parameters,weights}=regionParameters(position,c,zones),target=[...position];
  return {follow:[...position],offset:[0,0],target,position:cameraPosition(target,parameters,c.yaw),parameters,weights};
}
export type CameraState=ReturnType<typeof createCamera>;
export function advanceCamera(previous: CameraState, position: Point, aim: readonly [number,number] | undefined, dt: number, c: CameraConfig, zones: Zone[]): CameraState {
  const region=regionParameters(position,c,zones),parameters={...previous.parameters};
  const blend=(time:number)=>1-Math.exp(-dt/time);
  for(const k of fields) parameters[k]+=(region.parameters[k]-parameters[k])*blend(c.zoneTime);
  const follow=previous.follow.map((v,i)=>v+(position[i]-v)*blend(c.followTime));
  // Remove our own pan from the submitted-frame cursor point, so a stationary
  // near-centre cursor cannot keep dragging the camera toward the offset limit.
  const delta=aim?[aim[0]-position[0]-previous.offset[0],aim[1]-position[2]-previous.offset[1]]:[0,0];
  const scale=Math.min(1,parameters.aimOffset/Math.max(1e-9,Math.hypot(...delta)));
  const offset=previous.offset.map((v,i)=>v+(delta[i]*scale-v)*blend(c.aimTime));
  const target=[follow[0]+offset[0],follow[1],follow[2]+offset[1]];
  return {follow,offset,target,position:cameraPosition(target,parameters,c.yaw),parameters,weights:region.weights};
}
