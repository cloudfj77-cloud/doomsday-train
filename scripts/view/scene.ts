import presentation from '../../config/presentation/scene.json';
import type { AssetHandle } from '@forgeax/engine/types';
import { Transform, Name } from '@forgeax/engine/scene';
import { Camera, MeshFilter, MeshRenderer, perspective } from '@forgeax/engine/render';
import { quat } from '@forgeax/engine/math';
import type { World, EntityHandle } from '@forgeax/engine/ecs';
import type { State } from '../logic/world.ts';

export function draw(world: World, player: EntityHandle, state: Readonly<State>) {
  world.set(player, Transform, { pos: [state.x, presentation.player.height, state.z] }).unwrap();
}
export function camera(world: World) {
  const pos: [number, number, number] = presentation.camera.position as [number, number, number];
  const q = quat.fromLookAt(quat.create(), pos, presentation.camera.target, [0, 1, 0]);
  return world.spawn({ component: Name, data: { value: 'Preview Camera' } }, { component: Transform, data: { pos, quat: q } }, { component: Camera, data: { ...perspective({ fov: presentation.camera.fov, aspect: presentation.camera.aspect }), clearColor: presentation.camera.background } }).unwrap();
}
export function object(world: World, name: string, mesh: AssetHandle<'mesh'>, material: AssetHandle<'material'>, pos: [number, number, number], scale: [number, number, number]) {
  return world.spawn({ component: Name, data: { value: name } }, { component: Transform, data: { pos, scale } }, { component: MeshFilter, data: { assetHandle: mesh } }, { component: MeshRenderer, data: { materials: [material] } }).unwrap();
}




