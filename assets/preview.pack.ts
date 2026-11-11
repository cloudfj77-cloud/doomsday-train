import { definePack, definePackageId } from '@forgeax/engine/pack/source';
import { ok } from '@forgeax/engine/types';
import type { Plugin } from '@forgeax/engine/plugin';
import { Time, Update } from '@forgeax/engine/ecs';
import { INPUT_SNAPSHOT_RESOURCE_KEY, type InputSnapshot } from '@forgeax/engine/input';
import { createBoxGeometry } from '@forgeax/engine/geometry';
import { Materials } from '@forgeax/engine/render';
import { createState, step, project, validateConfig } from '../scripts/logic/world.ts';
import { camera, object, draw } from '../scripts/view/scene.ts';
import type {} from '@forgeax/engine/app';

export const preview: Plugin = {
  name: 'doomsday-preview', inject: ['world', 'assets'],
  async apply(ctx) {
    await ctx.effect(async function* () {
      const config = validateConfig({ speed: 4, limit: 5, start: [0, 0] });
      let state = createState(config);
      const assets = ctx.assets!;
      const mesh = assets.create(createBoxGeometry(1, 1, 1).unwrap()).unwrap();
      const ground = assets.create(Materials.unlit([0.12, 0.19, 0.24, 1])).unwrap();
      const actor = assets.create(Materials.unlit([0.3, 0.9, 0.72, 1])).unwrap();
      yield () => { assets.release(mesh); assets.release(ground); assets.release(actor); };
      const entities = [camera(ctx.world), object(ctx.world, 'Ground', mesh, ground, [0, -0.25, 0], [12, 0.5, 12])];
      const player = object(ctx.world, 'Player', mesh, actor, [state.x, 0.5, state.z], [0.6, 1, 0.6]);
      entities.push(player);
      yield () => { for (const entity of entities) ctx.world.despawn(entity); };
      ctx.world.addSystem(Update, { name: 'preview-movement', queries: [], fn(world) {
        const kb = world.getResource<InputSnapshot>(INPUT_SNAPSHOT_RESOURCE_KEY)?.keyboard;
        state = step(state, [Number(!!kb?.downCode('KeyD')) - Number(!!kb?.downCode('KeyA')), Number(!!kb?.downCode('KeyS')) - Number(!!kb?.downCode('KeyW'))], world.getResource(Time).delta, config);
        world.insertResource('preview-state', project(state));
        draw(world, player, project(state));
      } }).unwrap();
      yield () => { ctx.world.removeSystem(Update, 'preview-movement'); };
    });
  },
};
export default definePack({ schemaVersion: '2.0.0', plugins: true, packageId: definePackageId('019fb7ce-3800-7000-8000-000000000000'), build: () => ok({ 'plugin/engine': { kind: 'plugin', module: { specifier: './preview.pack.ts', export: 'preview' }, config: {} } }) });
