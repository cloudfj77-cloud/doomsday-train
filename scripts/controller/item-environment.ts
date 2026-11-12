import { vec3 } from '@forgeax/engine/math';
import type { PhysicsWorld } from '@forgeax/engine/physics';
import type { Environment, ItemWorldConfig, Position } from '../logic/items.ts';
export function itemEnvironment(physics:PhysicsWorld,c:ItemWorldConfig,player:Position):Environment {
  const visible=(p:Position)=>{
    const origin:Position=[player[0],player[1]+.5,player[2]],delta=[p[0]-origin[0],p[1]+.15-origin[1],p[2]-origin[2]];
    const length=Math.hypot(...delta);if(length<1e-6)return true;
    const hit=physics.raycast(vec3.create(...origin),vec3.create(delta[0]/length,delta[1]/length,delta[2]/length),length,0xffff0001);
    return !hit||Math.hypot(hit.point[0]-origin[0],hit.point[1]-origin[1],hit.point[2]-origin[2])>=length-.04;
  };
  return {player,visible,dropPosition:()=>{
    for(let i=0;i<8;i++) {
      const a=i*Math.PI/4,x=player[0]+Math.cos(a)*c.dropRadius,z=player[2]+Math.sin(a)*c.dropRadius;
      const ground=physics.raycast(vec3.create(x,player[1]+.5,z),vec3.create(0,-1,0),1.5,0xffff0001);
      if(!ground||ground.normal[1]<.9)continue;
      const p:Position=[x,ground.point[1]+.015,z];if(!visible(p))continue;
      let clear=!physics.raycast(vec3.create(x,p[1]+.02,z),vec3.create(0,1,0),c.dropHeight,0xffff0001);
      for(let j=0;j<8&&clear;j++) {
        const b=j*Math.PI/4,dx=Math.cos(b),dz=Math.sin(b);
        for(const height of [.06,c.dropHeight/2,c.dropHeight])if(physics.raycast(vec3.create(x,p[1]+height,z),vec3.create(dx,0,dz),c.dropClearance,0xffff0001))clear=false;
        const edge=physics.raycast(vec3.create(x+dx*c.dropClearance,p[1]+.15,z+dz*c.dropClearance),vec3.create(0,-1,0),.22,0xffff0001);
        if(!edge||Math.abs(edge.point[1]-ground.point[1])>.08)clear=false;
      }
      if(clear)return p;
    }
    return undefined;
  }};
}
