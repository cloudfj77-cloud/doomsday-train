import presentation from '../config/presentation/scene.json';
import { validatePresentation } from '../scripts/view/presentation.ts';
import { definePack, definePackageId } from '@forgeax/engine/pack/source';
import { AssetGuid } from '@forgeax/engine/pack/guid';
import { ok } from '@forgeax/engine/types';
import type { Plugin } from '@forgeax/engine/plugin';
import { Time, Update, FixedUpdate, FixedTime } from '@forgeax/engine/ecs';
import { INPUT_SNAPSHOT_RESOURCE_KEY, type InputSnapshot } from '@forgeax/engine/input';
import { bake, parts, collisionBoxes, type Part } from '../scripts/view/voxels.ts';
import { Materials } from '@forgeax/engine/render';
import data from '../config/gameplay/world.json';
import { createState, project, validateConfig } from '../scripts/logic/world.ts';
import { createCamera, advanceCamera, groundPoint } from '../scripts/logic/camera.ts';
import { camera, drawCamera, object, draw } from '../scripts/view/scene.ts';
import physics3d from '@forgeax/engine/physics-rapier3d';
import type { PhysicsWorld } from '@forgeax/engine/physics';
import { spawnCharacter, spawnCollision, tickCharacter } from '../scripts/controller/movement.ts';
import type {} from '@forgeax/engine/app';

export const preview: Plugin.Object<{ meshes: Record<Part,string>; material: string }> = {
  name: 'doomsday-preview', inject: ['world', 'assets', 'gameHost'],
  async apply(ctx, configAssets) {
    await ctx.plugin(physics3d);
    await ctx.effect(async function* () {
      validatePresentation(presentation);
      const config = validateConfig(data);
      let state = createState(config);
      let view = createCamera([state.x,state.y,state.z],presentation.camera,presentation.zones);
      let pointer: {ray?:number[]; focused:boolean}={focused:true};
      const port=ctx.gameHost!.port;
      if(!port) throw Error('missing-controller-source-port');
      const receive=(event:MessageEvent)=>{ if(event.data?.type==='controller-pointer') pointer=event.data; };
      port.addEventListener('message',receive); port.start();
      yield ()=>port.removeEventListener('message',receive);
      const read = ctx.gameHost!.gameProjection?.registerRead({ id: 'preview', title: 'Controller state', read: () => ({...project(state),camera:JSON.parse(JSON.stringify(view)),aim:groundPoint(pointer.ray,state.y)??null}) });
      if (read) yield read;
      const assets = ctx.assets!;
      const material = (await assets.load(configAssets.material, 'material')).unwrap();
      yield () => assets.release(material);
      const entities = new Map<Part,ReturnType<typeof object>>();
      const eye = camera(ctx.world,view);
      yield () => ctx.world.despawn(eye);
      const physical=spawnCollision(ctx.world,collisionBoxes(),config);
      const character=spawnCharacter(ctx.world,config);
      yield ()=>{ for(const entity of [...physical,character]) ctx.world.despawn(entity); };
      for (const part of parts) {
        const mesh = (await assets.load(configAssets.meshes[part], 'mesh')).unwrap();
        yield () => assets.release(mesh);
        const entity = object(ctx.world,part,mesh,material,[0,0,0],[1,1,1]);
        entities.set(part,entity);
        yield () => ctx.world.despawn(entity);
      }
      draw(ctx.world, entities, project(state));
      ctx.world.addSystem(FixedUpdate, { name: 'preview-movement', after:['physicsSyncBackend'],before:['physicsStepSimulation'],queries: [], fn(world) {
        const physics=world.getResource<PhysicsWorld>('PhysicsWorld');
        if(!physics?.hasBody(character)) return;
        const kb = world.getResource<InputSnapshot>(INPUT_SNAPSHOT_RESOURCE_KEY)?.keyboard;
        const down=(key:string)=>pointer.focused&&!!kb?.downCode(key);
        state=tickCharacter(world,physics,character,state,{axis:[Number(down(presentation.keys.right))-Number(down(presentation.keys.left)),Number(down(presentation.keys.up))-Number(down(presentation.keys.down))],run:down(presentation.keys.run),aim:groundPoint(pointer.ray,state.y)},world.getResource(FixedTime).delta,config,presentation.camera.yaw);
        world.insertResource('preview-state', project(state));
      } }).unwrap();
      yield () => { ctx.world.removeSystem(FixedUpdate, 'preview-movement'); };
      ctx.world.addSystem(Update, {name:'preview-presentation',queries:[],fn(world){
        view=advanceCamera(view,[state.x,state.y,state.z],groundPoint(pointer.ray,state.y),world.getResource(Time).delta,presentation.camera,presentation.zones);
        const size=ctx.gameHost!.drawingBufferSize?.();
        drawCamera(world,eye,view,size&&size.height>0?size.width/size.height:presentation.camera.aspect);
        draw(world, entities, project(state));
      }}).unwrap();
      yield ()=>{ctx.world.removeSystem(Update,'preview-presentation');};
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

