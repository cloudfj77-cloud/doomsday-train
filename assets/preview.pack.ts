import { pickTriangleDisplay } from '@forgeax/engine/picking';
import { sceneConfig as presentation } from '../scripts/config/game.ts';
import { validatePresentation } from '../scripts/view/presentation.ts';
import { definePack, definePackageId } from '@forgeax/engine/pack/source';
import { AssetGuid } from '@forgeax/engine/pack/guid';
import { ok } from '@forgeax/engine/types';
import type { Plugin } from '@forgeax/engine/plugin';
import { Time, Update, FixedUpdate, FixedTime } from '@forgeax/engine/ecs';
import { INPUT_SNAPSHOT_RESOURCE_KEY, type InputSnapshot } from '@forgeax/engine/input';
import { bake, bakeItem, parts, collisionBoxes, type Part } from '../scripts/view/voxels.ts';
import { Outline, OutlineOcclusionValue, Materials } from '@forgeax/engine/render';
import { worldConfig as data } from '../scripts/config/game.ts';
import { createState, project, validateConfig } from '../scripts/logic/world.ts';
import { createCamera, advanceCamera, groundPoint } from '../scripts/logic/camera.ts';
import { camera, drawCamera, object, draw, heldTransform } from '../scripts/view/scene.ts';
import physics3d from '@forgeax/engine/physics-rapier3d';
import type { PhysicsWorld } from '@forgeax/engine/physics';
import { spawnCharacter, spawnCollision, tickCharacter } from '../scripts/controller/movement.ts';
import type {} from '@forgeax/engine/app';
import { itemConfig as itemData } from '../scripts/config/game.ts';
import { itemWorldConfig as itemWorldData } from '../scripts/config/game.ts';
import { itemStyleConfig as itemStyle } from '../scripts/config/game.ts';
import { createItems, validateItems, projectItems, commandItems, nearestItem, nearbyItems, type ItemCommand } from '../scripts/logic/items.ts';
import { validateItemStyle } from '../scripts/view/item-presentation.ts';
import { itemEnvironment } from '../scripts/controller/item-environment.ts';
import type { BarrelDistortionMapping } from '@forgeax/engine/render';
import { Transform } from '@forgeax/engine/scene';
import type { AssetHandle } from '@forgeax/engine/types';

export const preview: Plugin.Object<{ meshes: Record<Part,string>; itemMeshes:Record<string,string>; material: string }> = {
  name: 'doomsday-preview', inject: ['world', 'assets', 'gameHost'],
  async apply(ctx, configAssets) {
    await ctx.plugin(physics3d);
    await ctx.effect(async function* () {
      validatePresentation(presentation);
      const config = validateConfig(data);
      let state = createState(config);
      const {config:itemsConfig,world:itemsWorld}=validateItems(itemData,itemWorldData);
      validateItemStyle(itemStyle,itemsConfig);
      const inventory=createItems(itemsConfig,itemsWorld);
      const pending:ItemCommand[]=[];let lastProjection='',target:string|undefined;
      let hovered:string|undefined;let readAppearance:()=>{backpackEntities:number[];bodyPositions:number;authoredBodyPositions:number}|null=()=>null;let nearby:string[]=[];let readOutline:()=>number[]=()=>[];
      let view = createCamera([state.x,state.y,state.z],presentation.camera,presentation.zones);
      let pointer: {ray?:number[];display?:{x:number;y:number;mapping:BarrelDistortionMapping}; focused:boolean}={focused:true};
      const port=ctx.gameHost!.port;
      if(!port) throw Error('missing-controller-source-port');
      const receive=(event:MessageEvent)=>{
        if(event.data?.type==='controller-pointer')pointer=event.data;
        if(event.data?.type==='item-command'&&pending.length<32)pending.push(event.data.command);
        if(event.data?.type==='item-ready')lastProjection='';
      };
      port.addEventListener('message',receive); port.start();
      yield ()=>port.removeEventListener('message',receive);
      const inventoryProjection=()=>projectItems(inventory,itemsConfig,target,nearby,hovered);
      const read = ctx.gameHost!.gameProjection?.registerRead({ id: 'preview', title: 'Controller and Item state', read: () => ({...project(state),camera:JSON.parse(JSON.stringify(view)),aim:inventory.open?null:groundPoint(pointer.ray,state.y)??null,inventory:JSON.parse(JSON.stringify(inventoryProjection())),proximity:{outlined:readOutline(),hovered:hovered??null},appearance:readAppearance()}) });
      if (read) yield read;
      const assets = ctx.assets!;
      const material = (await assets.load(configAssets.material, 'material')).unwrap();
      yield () => assets.release(material);
      const dropMeshes=new Map<string,AssetHandle<'mesh'>>();
      for(const [id,guid] of Object.entries(configAssets.itemMeshes)) {
        const mesh=(await assets.load(guid,'mesh')).unwrap();dropMeshes.set(id,mesh);yield ()=>assets.release(mesh);
      }
      let held:ReturnType<typeof object>|undefined,heldDefinition:string|undefined;
      yield ()=>{if(held)ctx.world.despawn(held);};
      const drops=new Map<string,ReturnType<typeof object>>();
      yield ()=>{for(const entity of drops.values())ctx.world.despawn(entity);};
      const backpackHistory=new Set<ReturnType<typeof object>>();
      const entities = new Map<Part,ReturnType<typeof object>>(),partMeshes=new Map<Part,AssetHandle<'mesh'>>();
      yield ()=>{for(const entity of entities.values())ctx.world.despawn(entity);};
      const eye = camera(ctx.world,view);
      yield () => {readOutline=()=>[];ctx.world.despawn(eye);};
      ctx.world.addComponent(eye,{component:Outline,data:{entities:[],visibleColor:itemStyle.proximity.outlineColor,hiddenColor:itemStyle.proximity.outlineColor,width:itemStyle.proximity.outlineWidth,occlusion:OutlineOcclusionValue.visible}}).unwrap();
      readOutline=()=>Array.from(ctx.world.get(eye,Outline).unwrap().entities);
      const physical=spawnCollision(ctx.world,collisionBoxes(),config);
      const character=spawnCharacter(ctx.world,config);
      yield ()=>{ for(const entity of [...physical,character]) ctx.world.despawn(entity); };
      for (const part of parts) {
        const mesh = (await assets.load(configAssets.meshes[part], 'mesh')).unwrap();
        yield () => assets.release(mesh);
        const entity = object(ctx.world,part,mesh,material,[0,0,0],[1,1,1]);
        entities.set(part,entity);partMeshes.set(part,mesh);if(part==='backpack')backpackHistory.add(entity);
      }
      const bodyData=assets.read(partMeshes.get('body')!).unwrap(),authoredBodyPositions=(bake('body').attributes.position as Float32Array).length;
      if((bodyData.attributes.position as Float32Array).length!==authoredBodyPositions)throw Error('actor-body-geometry-outdated');
      readAppearance=()=>({backpackEntities:Array.from(backpackHistory).filter(e=>ctx.world.get(e,Transform).ok),bodyPositions:(bodyData.attributes.position as Float32Array).length,authoredBodyPositions});
      draw(ctx.world, entities, project(state));
      const blockedKeys=new Set<string>();
      ctx.world.addSystem(FixedUpdate, { name: 'preview-movement', after:['physicsSyncBackend'],before:['physicsStepSimulation'],queries: [], fn(world) {
        const physics=world.getResource<PhysicsWorld>('PhysicsWorld');
        if(!physics?.hasBody(character)) return;
        const kb = world.getResource<InputSnapshot>(INPUT_SNAPSHOT_RESOURCE_KEY)?.keyboard;
        for(const key of Object.values(presentation.keys)) {
          if(!kb?.downCode(key))blockedKeys.delete(key);
          else if(inventory.open||!pointer.focused)blockedKeys.add(key);
        }
        const environment=itemEnvironment(physics,itemsWorld,[state.x,state.y,state.z]);
        for(const cmd of pending.splice(0))if(cmd&&typeof cmd.type==='string')commandItems(inventory,itemsConfig,itemsWorld,cmd,environment);
        const down=(key:string)=>pointer.focused&&!inventory.open&&!blockedKeys.has(key)&&!!kb?.downCode(key);
        state=tickCharacter(world,physics,character,state,{axis:[Number(down(presentation.keys.right))-Number(down(presentation.keys.left)),Number(down(presentation.keys.up))-Number(down(presentation.keys.down))],run:down(presentation.keys.run),aim:inventory.open?undefined:groundPoint(pointer.ray,state.y)},world.getResource(FixedTime).delta,config,presentation.camera.yaw);
        target=nearestItem(inventory,itemsConfig,itemsWorld,itemEnvironment(physics,itemsWorld,[state.x,state.y,state.z]));
        nearby=nearbyItems(inventory,itemsConfig,itemsWorld,itemEnvironment(physics,itemsWorld,[state.x,state.y,state.z]));
        world.insertResource('preview-state', project(state));
      } }).unwrap();
      yield () => { ctx.world.removeSystem(FixedUpdate, 'preview-movement'); };
      ctx.world.addSystem(Update, {name:'preview-presentation',queries:[],fn(world){
        view=advanceCamera(view,[state.x,state.y,state.z],inventory.open?undefined:groundPoint(pointer.ray,state.y),world.getResource(Time).delta,presentation.camera,presentation.zones);
        const size=ctx.gameHost!.drawingBufferSize?.();
        drawCamera(world,eye,view,size&&size.height>0?size.width/size.height:presentation.camera.aspect);
        const equipped=Object.values(inventory.items).find(i=>i.location.kind==='slot'&&i.location.parent===inventory.playerId&&i.location.slot==='primary');
        const weapon=itemsConfig.definitions.find(d=>d.id===equipped?.definition)?.weapon;
        const bagEquipped=inventory.backpackId!==inventory.playerId;
        if(!bagEquipped&&entities.has('backpack')){world.despawn(entities.get('backpack')!).unwrap();entities.delete('backpack');}
        if(bagEquipped&&!entities.has('backpack')){const entity=object(world,'backpack',partMeshes.get('backpack')!,material,[0,0,0],[1,1,1]);entities.set('backpack',entity);backpackHistory.add(entity);}
        draw(world, entities, project(state),weapon?.pose);
        if(heldDefinition!==equipped?.definition){if(held)world.despawn(held);held=undefined;heldDefinition=equipped?.definition;if(equipped)held=object(world,'Held '+equipped.definition,dropMeshes.get(equipped.definition)!,material,[0,0,0],[1,1,1]);}
        if(held&&weapon)world.set(held,Transform,heldTransform(project(state),weapon.pose)).unwrap();
        for(const [id,entity] of drops)if(inventory.items[id]?.location.kind!=='world'){world.despawn(entity);drops.delete(id);}
        for(const item of Object.values(inventory.items))if(item.location.kind==='world') {
          let entity=drops.get(item.id);
          if(!entity){const mesh=dropMeshes.get(item.definition);if(!mesh)throw Error('missing-item-mesh: '+item.definition);entity=object(world,'Drop '+item.id,mesh,material,item.location.position,[1,1,1]);drops.set(item.id,entity);}
          const scale=item.id===target?1.18:1;
          world.set(entity,Transform,{pos:item.location.position,scale:[scale,scale,scale]}).unwrap();
        }
        world.set(eye,Outline,{entities:nearby.map(id=>drops.get(id)).filter((e):e is NonNullable<typeof e>=>e!==undefined)}).unwrap();
        hovered=undefined;
        if(!inventory.open&&pointer.focused&&pointer.display){const d=pointer.display,hit=pickTriangleDisplay(world,d.x,d.y,d.mapping,d.mapping.width,d.mapping.height);if(hit.status==='hit')hovered=nearby.find(id=>drops.get(id)===hit.hit.entity);}
        const projection=inventoryProjection(),signature=JSON.stringify(projection);
        if(signature!==lastProjection){lastProjection=signature;port.postMessage({type:'item-projection',state:projection});}
      }}).unwrap();
      yield ()=>{ctx.world.removeSystem(Update,'preview-presentation');};
    });
  },
};
const packageId = definePackageId('019fb7ce-3800-7000-8000-000000000000');
const ref = (key: string) => ({ $asset: AssetGuid.format(AssetGuid.derive(packageId, key)) });
export default definePack({ schemaVersion: '2.0.0', plugins: true, packageId, build: () => ok({
  ...Object.fromEntries(parts.map(part => ['mesh/'+part,bake(part)])),
  ...Object.fromEntries(itemData.definitions.filter(d=>d.portable).map(d=>['mesh/item/'+d.id,bakeItem(d.id)])),
  'material/voxel': Materials.unlit([1,1,1,1]),
  'plugin/engine': { kind: 'plugin', module: { specifier: './preview.pack.ts', export: 'preview' },
    config: { meshes: Object.fromEntries(parts.map(part => [part,ref('mesh/'+part)])), itemMeshes:Object.fromEntries(itemData.definitions.filter(d=>d.portable).map(d=>[d.id,ref('mesh/item/'+d.id)])), material: ref('material/voxel') } }
}) });
