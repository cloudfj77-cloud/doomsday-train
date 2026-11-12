import {beforeAll,it,expect} from 'vitest';
import {World} from '@forgeax/engine/ecs';
import {Transform,GlobalTransform,registerPropagateTransforms} from '@forgeax/engine/scene';
import {loadRapier3D,createRapier3DPhysicsWorld,registerPhysicsSystems} from '@forgeax/engine/physics-rapier3d';
import {spawnCollision,type Box} from '../../scripts/controller/movement.ts';
import {itemEnvironment} from '../../scripts/controller/item-environment.ts';
import {validateConfig} from '../../scripts/logic/world.ts';
import {validateItems} from '../../scripts/logic/items.ts';
import raw from '../../config/gameplay/items.json';
import rawWorld from '../../config/gameplay/item-world.json';
import data from '../../config/gameplay/world.json';
let rapier:Awaited<ReturnType<typeof loadRapier3D>>;
beforeAll(async()=>{rapier=await loadRapier3D();});
function fixture(boxes:Box[]) {
  const world=new World();world.components.register(Transform).unwrap();world.components.register(GlobalTransform).unwrap();registerPropagateTransforms(world);
  const physics=createRapier3DPhysicsWorld(rapier);world.insertResource('PhysicsWorld',physics);const remove=registerPhysicsSystems(world);
  const entities=spawnCollision(world,boxes,validateConfig(data));for(let i=0;i<4;i++)world.update(1/60).unwrap();
  const c=validateItems(raw,rawWorld).world;
  return {environment:itemEnvironment(physics,c,[0,.015,0]),close(){for(const e of entities)world.despawn(e).unwrap();remove();physics.dispose();}};
}
it('uses real physics to block wall pickup and choose grounded drop positions',()=>{
  const f=fixture([{center:[.5,.6,0],size:[.15,1.2,3],pitch:0,yaw:0}]);
  try{expect(f.environment.visible([1,0,0])).toBe(false);expect(f.environment.visible([-.8,0,0])).toBe(true);const p=f.environment.dropPosition();expect(p).toBeDefined();expect(p![0]).toBeLessThan(.5);expect(p![1]).toBeCloseTo(.015,2);}finally{f.close();}
});
it('refuses drop when all feet positions are occupied',()=>{
  const boxes:Box[]=[];for(let i=0;i<8;i++){const a=i*Math.PI/4;boxes.push({center:[Math.cos(a)*.65,.6,Math.sin(a)*.65],size:[.48,1.2,.48],pitch:0,yaw:0});}
  const f=fixture(boxes);try{expect(f.environment.dropPosition()).toBeUndefined();}finally{f.close();}
});
it('refuses a clear center when surrounding footprint crosses a raised curb',()=>{
  const radius=rawWorld.dropRadius+rawWorld.dropClearance;
  const boxes:Box[]=[];for(let i=0;i<8;i++){const a=i*Math.PI/4;boxes.push({center:[Math.cos(a)*radius,.06,Math.sin(a)*radius],size:[.15,.12,.15],pitch:0,yaw:0});}
  const f=fixture(boxes);try{expect(f.environment.dropPosition()).toBeUndefined();}finally{f.close();}
});
