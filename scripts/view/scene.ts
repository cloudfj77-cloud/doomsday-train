import presentation from '../../config/presentation/scene.json';
import type { AssetHandle } from '@forgeax/engine/types';
import { Transform, Name } from '@forgeax/engine/scene';
import { Camera, MeshFilter, MeshRenderer, perspective } from '@forgeax/engine/render';
import { quat } from '@forgeax/engine/math';
import type { World, EntityHandle } from '@forgeax/engine/ecs';
import type { State } from '../logic/world.ts';

import type { Part } from './voxels.ts';

export function draw(world: World, entities: ReadonlyMap<Part,EntityHandle>, state: Readonly<State>) {
  const yaw = Math.atan2(state.facingX,state.facingZ);
  const swing = state.moving ? Math.sin(state.walkDistance * Math.PI * 2 / presentation.actor.stride) : 0;
  for (const [part, entity] of entities) {
    if (part === 'terrain') continue;
    const leg = part === 'left-leg' ? 1 : part === 'right-leg' ? -1 : 0;
    const stride = leg * swing * presentation.actor.legSwing;
    const lift = leg ? Math.max(0,leg*swing) * presentation.actor.bob : Math.abs(swing)*presentation.actor.bob;
    const angle = part === 'shade' ? 0 : yaw;
    world.set(entity,Transform,{
      pos:[state.x+stride*state.facingX,part==='shade'?0:lift,state.z+stride*state.facingZ],
      quat:[0,Math.sin(angle/2),0,Math.cos(angle/2)]
    }).unwrap();
  }
}
export function camera(world: World) {
  const pos: [number, number, number] = presentation.camera.position as [number, number, number];
  const q = quat.fromLookAt(quat.create(), pos, presentation.camera.target, [0, 1, 0]);
  return world.spawn({ component: Name, data: { value: 'Preview Camera' } }, { component: Transform, data: { pos, quat: q } }, { component: Camera, data: { ...perspective({ fov: presentation.camera.fov, aspect: presentation.camera.aspect }), clearColor: presentation.camera.background } }).unwrap();
}
export function object(world: World, name: string, mesh: AssetHandle<'mesh'>, material: AssetHandle<'material'>, pos: [number, number, number], scale: [number, number, number]) {
  return world.spawn({ component: Name, data: { value: name } }, { component: Transform, data: { pos, scale } }, { component: MeshFilter, data: { assetHandle: mesh } }, { component: MeshRenderer, data: { materials: [material] } }).unwrap();
}
