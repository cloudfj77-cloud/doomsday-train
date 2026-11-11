import type { AssetHandle } from '@forgeax/engine/assets-runtime';
import { Transform, Name } from '@forgeax/engine/scene';
import { Camera, MeshFilter, MeshRenderer, perspective } from '@forgeax/engine/render';
import { quat } from '@forgeax/engine/math';
import type { World, EntityHandle } from '@forgeax/engine/ecs';
import type { State } from '../logic/world.ts';

export function draw(world: World, player: EntityHandle, state: Readonly<State>) {
  world.set(player, Transform, { pos: [state.x, 0.5, state.z] }).unwrap();
}
export function camera(world: World) {
  const pos: [number, number, number] = [0, 14, 12];
  const q = quat.fromLookAt(quat.create(), pos, [0, 0, 0], [0, 1, 0]);
  return world.spawn({ component: Name, data: { value: 'Preview Camera' } }, { component: Transform, data: { pos, quat: q } }, { component: Camera, data: { ...perspective({ fov: Math.PI / 4, aspect: 16 / 9 }), clearColor: [0.025, 0.035, 0.05, 1] } }).unwrap();
}
export function object(world: World, name: string, mesh: AssetHandle<'mesh'>, material: AssetHandle<'material'>, pos: [number, number, number], scale: [number, number, number]) {
  return world.spawn({ component: Name, data: { value: name } }, { component: Transform, data: { pos, scale } }, { component: MeshFilter, data: { assetHandle: mesh } }, { component: MeshRenderer, data: { materials: [material] } }).unwrap();
}

