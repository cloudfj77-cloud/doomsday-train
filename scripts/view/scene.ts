import { actorPartPose } from './actor-model.ts';
import itemStyle from '../../config/presentation/items.json';
import presentation from '../../config/presentation/scene.json';
import type { AssetHandle } from '@forgeax/engine/types';
import { Transform, Name } from '@forgeax/engine/scene';
import { Camera, MeshFilter, MeshRenderer, perspective } from '@forgeax/engine/render';
import { quat } from '@forgeax/engine/math';
import type { World, EntityHandle } from '@forgeax/engine/ecs';
import type { State } from '../logic/world.ts';
import type { CameraState } from '../logic/camera.ts';

import type { Part } from './voxels.ts';

export function draw(world: World, entities: ReadonlyMap<Part,EntityHandle>, state: Readonly<State>,pose?:'pistol'|'smg') {
  const yaw = Math.atan2(state.facingX,state.facingZ);
  const swing = state.moving && state.grounded ? Math.sin(state.walkDistance * Math.PI * 2 / presentation.actor.stride) : 0;
  for (const [part, entity] of entities) {
    if (part === 'terrain') continue;
    const leg = part === 'left-leg' ? 1 : part === 'right-leg' ? -1 : 0;
    const stride = leg * swing * presentation.actor.legSwing;
    const lift = leg ? Math.max(0,leg*swing) * presentation.actor.bob : Math.abs(swing)*presentation.actor.bob;
    const arm=part==='left-arm'||part==='right-arm';
    const holding=pose?itemStyle.holding[pose]:undefined;
    const rest=actorPartPose(part,pose),pitch=rest.pitch,forward=rest.z;
    const armLift=rest.y+(arm&&holding?Math.abs(swing)*holding.bob:0);
    const angle = part === 'shade' ? 0 : yaw;
    world.set(entity,Transform,{
      pos:[state.x+(stride+forward)*state.facingX,part==='shade'?state.shadowY:state.y+lift+armLift,state.z+(stride+forward)*state.facingZ],
      quat:[Math.cos(angle/2)*Math.sin(pitch/2),Math.sin(angle/2)*Math.cos(pitch/2),-Math.sin(angle/2)*Math.sin(pitch/2),Math.cos(angle/2)*Math.cos(pitch/2)]
    }).unwrap();
  }
}
export function camera(world: World, state: CameraState) {
  const q = quat.fromLookAt(quat.create(), state.position, state.target, [0, 1, 0]);
  return world.spawn({ component: Name, data: { value: 'Preview Camera' } }, { component: Transform, data: { pos:state.position, quat: q } }, { component: Camera, data: { ...perspective({ fov: state.parameters.fov*Math.PI/180, aspect: presentation.camera.aspect }), clearColor: presentation.camera.background } }).unwrap();
}
export function drawCamera(world: World, eye: EntityHandle, state: CameraState, aspect: number) {
  const q=quat.fromLookAt(quat.create(),state.position,state.target,[0,1,0]);
  world.set(eye,Transform,{pos:state.position,quat:q}).unwrap();
  world.set(eye,Camera,{fov:state.parameters.fov*Math.PI/180,aspect}).unwrap();
}
export function object(world: World, name: string, mesh: AssetHandle<'mesh'>, material: AssetHandle<'material'>, pos: [number, number, number], scale: [number, number, number]) {
  return world.spawn({ component: Name, data: { value: name } }, { component: Transform, data: { pos, scale } }, { component: MeshFilter, data: { assetHandle: mesh } }, { component: MeshRenderer, data: { materials: [material] } }).unwrap();
}

export function heldTransform(state:Readonly<State>,pose:'pistol'|'smg'){
 const p=itemStyle.holding[pose],yaw=Math.atan2(state.facingX,state.facingZ),bob=state.moving?Math.abs(Math.sin(state.walkDistance*Math.PI*2/presentation.actor.stride))*p.bob:0;
 const angle=yaw-Math.PI/2;
 return {pos:[state.x+state.facingX*p.gunForward+state.facingZ*p.gunRight,state.y+p.gunHeight+bob,state.z+state.facingZ*p.gunForward-state.facingX*p.gunRight] as [number,number,number],quat:[0,Math.sin(angle/2),0,Math.cos(angle/2)] as [number,number,number,number],scale:[p.gunScale,p.gunScale,p.gunScale] as [number,number,number]};
}
