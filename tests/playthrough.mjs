import assert from 'node:assert/strict';
const yaw=-Math.PI/6;
async function go(play,x,z,tolerance=.18) {
  for(let i=0;i<160;i++) {
    const s=await play.state(),dx=x-s.x,dz=z-s.z;
    if(Math.hypot(dx,dz)<tolerance)return s;
    const right=dx*Math.cos(yaw)-dz*Math.sin(yaw),up=-dx*Math.sin(yaw)-dz*Math.cos(yaw);
    const keys=[];
    if(Math.abs(right)>tolerance*.6)keys.push(right>0?'KeyD':'KeyA');
    if(Math.abs(up)>tolerance*.6)keys.push(up>0?'KeyW':'KeyS');
    await play.keyboard.hold(keys,60);
  }
  assert.fail(`unreachable waypoint ${x},${z}: ${JSON.stringify(await play.state())}`);
}
async function route(play,height) {
  const shot=name=>play.screenshot(`${height}-${name}`);
  await play.waitFor(s=>s.grounded);
  await shot('spawn');
  await play.mouse.move([.8,.35]);
  await play.advance(.8,{render:true});
  let s=await play.state();
  assert.ok(s.aim,'submitted-frame mouse ground point exists');
  assert.ok(Math.hypot(...s.camera.offset)>1,'mouse moves camera');
  assert.ok(Math.hypot(...s.camera.offset)<=5.001);
  const before=s;
  await play.keyboard.hold('KeyA',180);
  s=await play.state();assert.ok(s.x<before.x&&s.z<before.z,'A is camera-left');
  assert.ok(s.facingX>0,'character faces mouse while strafing');await shot('mouse-strafe');
  await play.keyboard.hold('KeyS',180);s=await play.state();assert.ok(s.z>before.z-.8,'backward movement');await shot('backward');
  await play.keyboard.hold(['KeyD','ShiftLeft'],s=>s.running&&Math.hypot(s.vx,s.vz)>5.5);
  await shot('running');
  await play.page.mouse.move(-20,-20);
  await play.advance(1.5,{render:true});
  s=await play.state();assert.ok(Math.hypot(...s.camera.offset)<.1,'pointer exit recentres camera');
  await go(play,-3,.4);await go(play,-3,-2.9);
  s=await play.state();assert.ok(s.y>.7&&s.grounded,'walk up slope');await shot('slope-up');
  // The ramp exit borders railway sleepers. Sample clear sand after leaving the ramp,
  // rather than assuming every point within the waypoint tolerance has ground height zero.
  await go(play,-3,.5);await go(play,-4,.9);s=await play.state();assert.ok(s.y<.08&&s.grounded,'descend slope onto clear sand');await shot('slope-down');
  await go(play,-3,.5);await go(play,-3,-2.9);await go(play,-3,-4.1);
  await shot('platform');
  let falling=false;
  await play.page.keyboard.down('KeyW');await play.page.keyboard.down('KeyA');
  try {for(let i=0;i<40;i++){s=await play.advance(.05,{render:true});if(!s.grounded&&s.y>.1){falling=true;await shot('falling');break;}}}
  finally{await play.page.keyboard.up('KeyW');await play.page.keyboard.up('KeyA');}
  assert.ok(falling,'walked off platform');
  await play.advance(1,{render:true});s=await play.state();assert.ok(s.grounded&&s.y<.1,'landed');await shot('landed');
  await go(play,-1,-6.2);await go(play,-1,-5);
  s=await play.state();assert.ok(s.y>.17,'cross low step');await shot('low-step');
  await go(play,-1,-6.3);await go(play,1,-6.3);
  await play.keyboard.hold(['KeyS','KeyD'],300);s=await play.state();assert.ok(s.z<-5.8&&s.y<.1,'high step blocks');await shot('high-step-blocked');
  await go(play,3,-6.4);await go(play,4.5,-4.8);await go(play,4.6,-1.5);await go(play,3.5,-1.7);
  await play.advance(1,{render:true});s=await play.state();assert.ok(s.camera.weights[0].weight>.5&&s.camera.parameters.distance<42,'region camera enters');await shot('region-camera');
  await go(play,3,2.6);await go(play,5,2.4);
  await play.keyboard.hold(['KeyW','KeyA'],300);s=await play.state();assert.ok(s.z>.8&&s.y<.6,'steep slope blocks');await shot('steep-slope');
  await go(play,3,2);await play.advance(1.5,{render:true});s=await play.state();assert.ok(s.camera.parameters.distance>44.9,'region camera exits');await shot('region-exit');
  await play.mouse.move([.7,.4]);await play.advance(2,{render:true});const stable=await play.state();await play.advance(2,{render:true});s=await play.state();
  assert.ok(Math.hypot(...s.camera.position.map((v,i)=>v-stable.camera.position[i]))<.05,'stationary pointer has no sustained camera drift');
  await shot('pointer-stable');
  await play.reload();await play.waitFor(s=>s.grounded);const reset=await play.state();assert.ok(Math.abs(reset.x+3.5)<.1&&Math.abs(reset.z-3)<.1);
  await play.keyboard.hold('KeyD',120);s=await play.state();assert.ok(s.x>reset.x+.2,'movement survives reload');
  await play.mouse.move([.8,.3]);await play.advance(.8,{render:true});s=await play.state();assert.ok(s.aim&&Math.hypot(...s.camera.offset)>1,'pointer survives reload');await shot('reload');
}
export default async function(play) {
  for(const height of [720,600]) {
    await play.page.setViewportSize({width:1280,height});await play.reload();
    await play.step(`controller-${height}`,()=>route(play,height));
  }
  await play.page.setViewportSize({width:1280,height:720});
}
