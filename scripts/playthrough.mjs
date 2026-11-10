import assert from 'node:assert/strict';
export default async function(play){
 const click=async action=>play.page.locator('[data-act="'+action+'"]').first().click();
 const state=()=>play.state('doomsday.flow');
 async function go(x,z){
   for(let i=0;i<140;i++){
     const s=await state(),p=s.game.expedition; const dx=x-p.x,dz=z-p.y;
     if(Math.hypot(dx,dz)<0.45)return;
     const h=dx*0.855-dz*0.518,v=dx*0.518+dz*0.855;
     const keys=[];if(Math.abs(h)>Math.abs(v)*0.42)keys.push(h>0?'KeyD':'KeyA');if(Math.abs(v)>Math.abs(h)*0.42)keys.push(v>0?'KeyS':'KeyW');
     await play.keyboard.hold(keys,100);
   }
   throw Error('Could not walk to '+x+','+z+' from '+JSON.stringify((await state()).game.expedition));
 }
 await click('new-game');await click('choose-route');await click('depart');
 await play.waitFor(s=>!s.travelling&&s.game.phase==='station',{read:'doomsday.flow'});
 await play.screenshot('01-arrived-train');await click('explore');
 await play.waitFor(s=>s.engine?.mode==='exploration',{read:'doomsday.flow'});
 await play.screenshot('02-3d-station-entry');
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
 await play.screenshot('05-returned-train');
 await play.reload();await click('continue');assert.equal((await state()).game.expedition.status,'returned');
 await click('depart');await play.waitFor(s=>!s.travelling&&s.game.phase==='station',{read:'doomsday.flow'});
 await play.screenshot('06-next-station');return {passed:true,buildings:3};
}
