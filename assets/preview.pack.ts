import presentation from '../config/presentation/scene.json';
import { validatePresentation } from '../scripts/view/presentation.ts';
import { definePack, definePackageId } from '@forgeax/engine/pack/source';
import { AssetGuid } from '@forgeax/engine/pack/guid';
import { ok } from '@forgeax/engine/types';
import type { Plugin } from '@forgeax/engine/plugin';
import { Time, Update } from '@forgeax/engine/ecs';
import { INPUT_SNAPSHOT_RESOURCE_KEY, type InputSnapshot } from '@forgeax/engine/input';
import { bake, parts, type Part } from '../scripts/view/voxels.ts';
import { Materials } from '@forgeax/engine/render';
import data from '../config/gameplay/world.json';
import { createState, step, project, validateConfig } from '../scripts/logic/world.ts';
import { camera, object, draw } from '../scripts/view/scene.ts';
import type {} from '@forgeax/engine/app';

export const preview: Plugin.Object<{ meshes: Record<Part,string>; material: string }> = {
  name: 'doomsday-preview', inject: ['world', 'assets', 'gameHost'],
  async apply(ctx, configAssets) {
    await ctx.effect(async function* () {
      validatePresentation(presentation);
      const config = validateConfig(data);
      let state = createState(config);
      const read = ctx.gameHost!.gameProjection?.registerRead({ id: 'preview', title: 'Movement state', read: () => project(state) });
      if (read) yield read;
      const assets = ctx.assets!;
      const material = (await assets.load(configAssets.material, 'material')).unwrap();
      yield () => assets.release(material);
      const entities = new Map<Part,ReturnType<typeof object>>();
      const eye = camera(ctx.world);
      yield () => ctx.world.despawn(eye);
      for (const part of parts) {
        const mesh = (await assets.load(configAssets.meshes[part], 'mesh')).unwrap();
        yield () => assets.release(mesh);
        const entity = object(ctx.world,part,mesh,material,[0,0,0],[1,1,1]);
        entities.set(part,entity);
        yield () => ctx.world.despawn(entity);
      }
      draw(ctx.world, entities, project(state));
      ctx.world.addSystem(Update, { name: 'preview-movement', queries: [], fn(world) {
        const kb = world.getResource<InputSnapshot>(INPUT_SNAPSHOT_RESOURCE_KEY)?.keyboard;
        state = step(state, [Number(!!kb?.downCode(presentation.keys.right)) - Number(!!kb?.downCode(presentation.keys.left)), Number(!!kb?.downCode(presentation.keys.down)) - Number(!!kb?.downCode(presentation.keys.up))], world.getResource(Time).delta, config);
        world.insertResource('preview-state', project(state));
        draw(world, entities, project(state));
      } }).unwrap();
      yield () => { ctx.world.removeSystem(Update, 'preview-movement'); };
    });
  },
};
const packageId = definePackageId('019fb7ce-3800-7000-8000-000000000000');
const ref = (key: string) => ({ $asset: AssetGuid.format(AssetGuid.derive(packageId, key)) });
export default definePack({ schemaVersion: '2.0.0', plugins: true, packageId, build: () => ok({
  ...Object.fromEntries(parts.map(part => ['mesh/'+part,bake(part)])),
  'material/voxel': Materials.unlit([1,1,1,1]),
  'plugin/engine': { kind: 'plugin', module: { specifier: './preview.pack.ts', export: 'preview' },
    config: { meshes: Object.fromEntries(parts.map(part => [part,ref('mesh/'+part)])), material: ref('material/voxel') } }
}) });

