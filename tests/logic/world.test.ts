import { describe, it, expect, beforeAll } from 'vitest';
import data from '../../config/gameplay/world.json';
import { createState, desiredMovement, resolveMovement, screenMovement, project, validateConfig } from '../../scripts/logic/world.ts';
import { World, FixedUpdate, FixedTime } from '@forgeax/engine/ecs';
import { registerPropagateTransforms, Transform, GlobalTransform } from '@forgeax/engine/scene';
import { loadRapier3D, createRapier3DPhysicsWorld, registerPhysicsSystems } from '@forgeax/engine/physics-rapier3d';
import { spawnCharacter, spawnCollision, tickCharacter, type Box } from '../../scripts/controller/movement.ts';
import type { Intent } from '../../scripts/logic/world.ts';
const c=validateConfig(data);
let rapier: Awaited<ReturnType<typeof loadRapier3D>>;
beforeAll(async()=>{rapier=await loadRapier3D();});
function fixture(boxes:Box[]=[], start=[-3.5,3], foot=0) {
  const config={...c,start},world=new World();
  world.components.register(Transform).unwrap();world.components.register(GlobalTransform).unwrap();
  registerPropagateTransforms(world);
  const physics=createRapier3DPhysicsWorld(rapier);
  world.insertResource('PhysicsWorld',physics);
  const remove=registerPhysicsSystems(world);
  const colliders=spawnCollision(world,boxes,config),entity=spawnCharacter(world,config);
  world.set(entity,Transform,{pos:[start[0],foot+c.height/2+c.skin,start[1]]}).unwrap();
  let state={...createState(config),y:foot+c.skin},input:Intent={axis:[0,0],run:false};
  world.addSystem(FixedUpdate,{name:'test-character',after:['physicsSyncBackend'],before:['physicsStepSimulation'],queries:[],fn(){
    if(physics.hasBody(entity)) state=tickCharacter(world,physics,entity,state,input,world.getResource(FixedTime).delta,config,0);
  }}).unwrap();
  const advance=(seconds:number,next:Intent=input)=>{input=next;for(let i=0;i<Math.round(seconds*60);i++)world.update(1/60).unwrap();return state;};
  advance(.2);
  return {advance,get state(){return state;},close(){for(const e of [entity,...colliders])world.despawn(e).unwrap();remove();physics.dispose();}};
}
describe('controller intent and configuration',()=>{
  it('validates actual config, spawn and identity constraints',()=>{
    expect(validateConfig(data)).toBe(data);
    for(const bad of [null,{...data,speed:-1},{...data,height:.1},{...data,start:[0,0]},{...data,obstacles:[data.obstacles[0],data.obstacles[0]]},{...data,surfaces:[{...data.surfaces[0],pitch:NaN}]}]) expect(()=>validateConfig(bad)).toThrow();
  });
  it('maps camera-relative axes and normalizes diagonals',()=>{
    expect(screenMovement([0,1],-30)[0]).toBeCloseTo(.5);
    expect(screenMovement([0,1],-30)[1]).toBeCloseTo(-Math.sqrt(3)/2);
    expect(Math.hypot(...screenMovement([1,1],-30))).toBeCloseTo(1);
    const s={...createState(c),grounded:true};
    expect(desiredMovement(s,{axis:[1,0],run:true},.1,c,0)[0]).toBeCloseTo(.6);
    expect(desiredMovement(s,{axis:[1,0],run:false},.1,c,0)[0]).toBeCloseTo(.4);
    expect(desiredMovement({...s,grounded:false},{axis:[1,0],run:true},.1,c,0)[0]).toBe(0);
    expect(()=>desiredMovement(s,{axis:[NaN,0],run:false},.1,c,0)).toThrow();
  });
  it('turns independently, uses shortest arc and freezes projections',()=>{
    const s={...createState(c),x:0,z:0,yaw:Math.PI-.01};
    const turned=resolveMovement(s,[1,0,0],true,{axis:[1,0],run:false,aim:[.99,-5]},.1,c);
    expect(turned.yaw).toBeGreaterThan(s.yaw);
    const still=resolveMovement(s,[0,0,0],true,{axis:[0,0],run:false,aim:[0,0]},.1,c);
    expect(still.yaw).toBe(s.yaw);expect(still.moving).toBe(false);
    expect(Object.isFrozen(project(turned))).toBe(true);
  });
});
describe('real Rapier character controller',()=>{
  it('walks, runs, stops, slides and blocks tunnelling',()=>{
    const f=fixture([{center:[0,1,0],size:[1,2,8],pitch:0,yaw:0}],[-2,0]);
    try {
      const s=f.advance(.2,{axis:[1,0],run:false});expect(s.x).toBeCloseTo(-1.2,1);
      const blocked=f.advance(2);expect(blocked.x).toBeLessThan(-.7);expect(blocked.moving).toBe(false);
      const distance=blocked.walkDistance;
      const slide=f.advance(.2,{axis:[1,1],run:false});expect(slide.z).toBeLessThan(-.3);expect(slide.walkDistance).toBeGreaterThan(distance);
      const x=slide.x;expect(f.advance(.2,{axis:[0,0],run:false}).x).toBeCloseTo(x);
    } finally {f.close();}
    const f2=fixture();try{const start=f2.state.x;expect(f2.advance(.2,{axis:[1,0],run:true}).x-start).toBeCloseTo(1.2,1);}finally{f2.close();}
  });
  it('steps over low boxes but blocks high steps',()=>{
    for(const height of [.2,.7]) {
      const f=fixture([{center:[0,height/2,0],size:[2,height,2],pitch:0,yaw:0}],[-2,0]);
      try{const s=f.advance(.8,{axis:[1,0],run:false});if(height<.25){expect(s.x).toBeGreaterThan(0);expect(s.y).toBeGreaterThan(.18);}else expect(s.x).toBeLessThan(-1.2);}finally{f.close();}
    }
  });
  it('climbs and descends a gentle slope, drops and lands without air steering',()=>{
    const f=fixture([{center:[0,.5,0],size:[3,.2,4],pitch:20*Math.PI/180,yaw:0}],[0,2.6]);
    try {
      const up=f.advance(.7,{axis:[0,1],run:false});expect(up.y).toBeGreaterThan(.5);
      const down=f.advance(.6,{axis:[0,-1],run:false});expect(down.y).toBeLessThan(up.y);
    } finally {f.close();}
    const drop=fixture([{center:[0,.4,0],size:[2,.8,2],pitch:0,yaw:0}],[0,0],.8);
    try {const elevated=drop.advance(1);expect(elevated.y).toBeGreaterThan(.7);
      let airborne=false;for(let i=0;i<90;i++){const s=drop.advance(1/60,{axis:[1,0],run:false});if(!s.grounded&&s.y>.1)airborne=true;}
      expect(airborne).toBe(true);expect(drop.state.grounded).toBe(true);expect(drop.state.y).toBeLessThan(.05);expect(drop.state.vy).toBe(0);
    }finally{drop.close();}
  });
  it('rejects steep uphill travel',()=>{
    const f=fixture([{center:[0,.7,0],size:[3,.2,2],pitch:60*Math.PI/180,yaw:0}],[0,1.8]);
    try{const s=f.advance(1,{axis:[0,1],run:false});expect(s.z).toBeGreaterThan(0);expect(s.y).toBeLessThan(.8);}finally{f.close();}
  });
});
