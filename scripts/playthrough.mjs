import assert from 'node:assert/strict';
export default async function(play){
 const click=async action=>play.page.locator('[data-act="'+action+'"]').first().click();
 const state=()=>play.state('doomsday.flow');
 async function go(x,z){
   for(let i=0;i<140;i++){
     const s=await state(),p=s.game.expedition; const dx=x-p.x,dz=z-p.y;
     if(Math.hypot(dx,dz)<0.45)return;
     const h=dx*0.8660254038+dz*0.5,v=-dx*0.5+dz*0.8660254038;
     const keys=[];if(Math.abs(h)>Math.abs(v)*0.42)keys.push(h>0?'KeyD':'KeyA');if(Math.abs(v)>Math.abs(h)*0.42)keys.push(v>0?'KeyS':'KeyW');
     await play.keyboard.hold(keys,100);
   }
   throw Error('Could not walk to '+x+','+z+' from '+JSON.stringify((await state()).game.expedition));
 }
 await click('new-game');await click('choose-route');await click('depart');
 await play.waitFor(s=>!s.travelling&&s.game.phase==='station',{read:'doomsday.flow'});
 await play.screenshot('01-arrived-train');await click('explore');
 await play.waitFor(s=>s.engine?.mode==='exploration',{read:'doomsday.flow'});
 await play.mouse.move([0.5,0.5]);await play.advance(2);
 const centred=await state(), cp=centred.engine.cameraPose;
 assert.equal(cp.fov,20);
 const offset=cp.pos.map((v,i)=>v-cp.target[i]);
 assert.ok(Math.abs(Math.hypot(...offset)-45)<0.001);
 assert.ok(Math.abs(Math.asin(offset[1]/45)*180/Math.PI-55)<0.001);
 assert.ok(Math.abs(Math.atan2(offset[0],offset[2])*180/Math.PI+30)<0.001);
 assert.ok(Math.hypot(centred.engine.cameraLead.x,centred.engine.cameraLead.z)<0.05);
 await play.screenshot('camera-centred-55pitch-minus30yaw-45distance-20fov');
 await play.mouse.move([0.75,0.4]);
 await play.keyboard.hold('KeyD',400);
 const walking=await state();
 assert.ok(walking.engine.controller.x > -11);
 assert.ok(walking.engine.aim);
 assert.ok(Math.hypot(walking.engine.cameraLead.x,walking.engine.cameraLead.z)>0.1);
 assert.ok(Math.hypot(walking.engine.cameraLead.x,walking.engine.cameraLead.z)<=5.01);
 await play.screenshot('controller-mouse-facing-strafe');
 const stamina=walking.engine.controller.stamina;
 const startRun=walking.game.expedition;
 await play.keyboard.hold(['ShiftLeft','KeyD'],400);
 const running=await state();
 assert.ok(running.engine.controller.stamina < stamina);
 assert.ok(Math.hypot(running.game.expedition.x-startRun.x,running.game.expedition.y-startRun.y)>2);
 await play.screenshot('controller-sprint-stamina');
 await play.keyboard.press('Space');
 const dashing=await state();assert.equal(dashing.engine.controller.action,'dash');
 assert.ok(dashing.engine.controller.stamina <= running.engine.controller.stamina-9);
 await play.screenshot('controller-dash');
 await play.advance(1);
 const buildings=(await state()).buildings;
 for(const b of buildings){
   await go(0,-1.5);
   if(b.id==='clinic'){await go(0,-12.5);await go(b.x,-13.5);}else await go(b.x,-1.5);
   await go(b.x,b.z+4.3);await go(b.x,b.z+0.8);
   await play.keyboard.press('KeyE');
   await play.waitFor(s=>s.game.expedition.surveyed.includes(b.id),{read:'doomsday.flow'});
   await play.screenshot('03-interior-'+b.id);
   await go(b.x,b.z+4.3);
   if(b.id==='clinic'){await go(0,-13.5);await go(0,-1.5);}
 }
 const before=(await state()).game.expedition;
 await play.reload();await click('continue');
 await play.waitFor(s=>s.engine?.mode==='exploration',{read:'doomsday.flow'});
 const after=(await state()).game.expedition;
 assert.equal(after.surveyed.length,3);assert.ok(Math.hypot(after.x-before.x,after.y-before.y)<0.6);
 await play.screenshot('04-exploration-reload');
 await go(0,1);await go(-11,4);await click('return-train');
 assert.equal((await state()).game.expedition.status,'returned');
 assert.equal((await state()).engine.cameraPose.fov,38);
 await play.screenshot('05-returned-train');
 await play.reload();await click('continue');assert.equal((await state()).game.expedition.status,'returned');
 await click('depart');await play.waitFor(s=>!s.travelling&&s.game.phase==='station',{read:'doomsday.flow'});
 await play.screenshot('06-next-station');return {passed:true,buildings:3};
}
