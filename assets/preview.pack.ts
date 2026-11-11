import presentation from './presentation.json';
import { definePack, definePackageId } from '@forgeax/engine/pack/source';
import { AssetGuid } from '@forgeax/engine/pack/guid';
import { ok } from '@forgeax/engine/types';
import type { Plugin } from '@forgeax/engine/plugin';
import { Time, Update } from '@forgeax/engine/ecs';
import { INPUT_SNAPSHOT_RESOURCE_KEY, type InputSnapshot } from '@forgeax/engine/input';
import { createBoxGeometry } from '@forgeax/engine/geometry';
import { Materials } from '@forgeax/engine/render';
import data from './world.json';
import { createState, step, project, validateConfig } from '../scripts/logic/world.ts';
import { camera, object, draw } from '../scripts/view/scene.ts';
import type {} from '@forgeax/engine/app';

export const preview: Plugin.Object<{ mesh: string; ground: string; actor: string }> = {
  name: 'doomsday-preview', inject: ['world', 'assets', 'gameHost'],
  async apply(ctx, configAssets) {
    await ctx.effect(async function* () {
      const config = validateConfig(data);
      let state = createState(config);
      const read = ctx.gameHost!.gameProjection?.registerRead({ id: 'preview', title: 'Movement state', read: () => project(state) });
      if (read) yield read;
      const assets = ctx.assets!;
      const mesh = (await assets.load(configAssets.mesh, 'mesh')).unwrap();
      const ground = (await assets.load(configAssets.ground, 'material')).unwrap();
      const actor = (await assets.load(configAssets.actor, 'material')).unwrap();
      yield () => { assets.release(mesh); assets.release(ground); assets.release(actor); };
      const entities = [camera(ctx.world), object(ctx.world, 'Ground', mesh, ground, presentation.ground.position as [number, number, number], presentation.ground.scale as [number, number, number])];
      const player = object(ctx.world, 'Player', mesh, actor, [state.x, presentation.player.height, state.z], presentation.player.scale as [number, number, number]);
      entities.push(player);
      yield () => { for (const entity of entities) ctx.world.despawn(entity); };
      ctx.world.addSystem(Update, { name: 'preview-movement', queries: [], fn(world) {
        const kb = world.getResource<InputSnapshot>(INPUT_SNAPSHOT_RESOURCE_KEY)?.keyboard;
        state = step(state, [Number(!!kb?.downCode(presentation.keys.right)) - Number(!!kb?.downCode(presentation.keys.left)), Number(!!kb?.downCode(presentation.keys.down)) - Number(!!kb?.downCode(presentation.keys.up))], world.getResource(Time).delta, config);
        world.insertResource('preview-state', project(state));
        draw(world, player, project(state));
      } }).unwrap();
      yield () => { ctx.world.removeSystem(Update, 'preview-movement'); };
    });
  },
};
const packageId = definePackageId('019fb7ce-3800-7000-8000-000000000000');
const ref = (key: string) => ({ $asset: AssetGuid.format(AssetGuid.derive(packageId, key)) });
export default definePack({ schemaVersion: '2.0.0', plugins: true, packageId, build: () => ok({
  'mesh/unit': createBoxGeometry(1, 1, 1).unwrap(),
  'material/ground': Materials.unlit(presentation.ground.color as [number, number, number, number]),
  'material/player': Materials.unlit(presentation.player.color as [number, number, number, number]),
  'plugin/engine': { kind: 'plugin', module: { specifier: './preview.pack.ts', export: 'preview' }, config: { mesh: ref('mesh/unit'), ground: ref('material/ground'), actor: ref('material/player') } }
}) });
