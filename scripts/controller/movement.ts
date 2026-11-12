import { Transform, Name } from '@forgeax/engine/scene';
import { CharacterController, Collider, ColliderShapeValue, RigidBody, RigidBodyTypeValue, type PhysicsWorld } from '@forgeax/engine/physics';
import { vec3 } from '@forgeax/engine/math';
import type { World, EntityHandle } from '@forgeax/engine/ecs';
import { desiredMovement, resolveMovement, type State, type Intent, type Config } from '../logic/world.ts';
export interface Box { center: number[]; size: number[]; pitch: number; yaw: number }
export function spawnCollision(world: World, boxes: Box[], c: Config) {
  const walls: Box[]=[];
  for(const sign of [-1,1]) {
    walls.push({center:[sign*(c.limit+c.radius+.5),2,0],size:[1,6,c.limit*2+2],pitch:0,yaw:0});
    walls.push({center:[0,2,sign*(c.limit+c.radius+.5)],size:[c.limit*2+2,6,1],pitch:0,yaw:0});
  }
  return [{center:[0,-.2,0],size:[c.ground[0],.4,c.ground[1]],pitch:0,yaw:0},...boxes,...walls].map(b=> {
    const sx=Math.sin(b.pitch/2),cx=Math.cos(b.pitch/2),sy=Math.sin(b.yaw/2),cy=Math.cos(b.yaw/2);
    return world.spawn({component:Transform,data:{pos:b.center,quat:[sx*cy,cx*sy,-sx*sy,cx*cy]}},
      {component:Collider,data:{shape:ColliderShapeValue.cuboid,halfExtents:b.size.map(n=>n/2)}}).unwrap();
  });
}
export function spawnCharacter(world: World, c: Config) {
  return world.spawn({component:Name,data:{value:'Player Controller'}},
    {component:Transform,data:{pos:[c.start[0],c.height/2+c.skin,c.start[1]]}},
    {component:RigidBody,data:{type:RigidBodyTypeValue.kinematic}},
    {component:Collider,data:{shape:ColliderShapeValue.capsule,radius:c.radius,halfHeight:c.height/2-c.radius,collisionGroups:0x0002ffff}},
    {component:CharacterController,data:{offset:c.skin,maxSlopeClimbDeg:c.maxSlope,minSlopeSlideDeg:c.maxSlope,
      autoStepMaxHeight:c.stepHeight,autoStepMinWidth:c.stepWidth,snapToGroundDist:c.snapDistance}}).unwrap();
}
export function tickCharacter(world: World, physics: PhysicsWorld, entity: EntityHandle, s: State, input: Intent, dt: number, c: Config, yaw: number) {
  const d=desiredMovement(s,input,dt,c,yaw);
  physics.moveAndSlide(entity,vec3.create(...d));
  const p=world.get(entity,Transform).unwrap().pos;
  const next=resolveMovement(s,[p[0],p[1]-c.height/2,p[2]],world.get(entity,CharacterController).unwrap().grounded,input,dt,c);
  const hit=physics.raycast(vec3.create(next.x,next.y+.05,next.z),vec3.create(0,-1,0),40,0xffff0001);
  next.shadowY=hit ? hit.point[1] : 0;
  return next;
}
