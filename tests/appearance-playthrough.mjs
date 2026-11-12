import assert from 'node:assert/strict';
import {projectDropLabel} from '../scripts/view/drop-labels.ts';
export async function hoverDrop(play,id){
 for(let n=0;n<12;n++){
  const s=await play.state(),item=s.inventory.items[id];assert.equal(item.location.kind,'world');
  const frame=await play.page.locator('canvas').first().evaluate(el=>el.forgeaxFrameProgress?.submitted?.barrelDistortion);
  assert.ok(frame?.camera,'accepted frame camera is available');const p=projectDropLabel([item.location.position[0],item.location.position[1]+.24,item.location.position[2]],frame);assert.ok(p);
  await play.mouse.move([p.x,p.y]);await play.advance(.12,{render:true});const next=await play.state();if(next.proximity.hovered===id)return next;
 }assert.fail('pointer did not hit visible nearby item');
}
async function go(play,x,z){const yaw=-Math.PI/6;for(let n=0;n<120;n++){const s=await play.state(),dx=x-s.x,dz=z-s.z;if(Math.hypot(dx,dz)<.12)return;const right=dx*Math.cos(yaw)-dz*Math.sin(yaw),up=-dx*Math.sin(yaw)-dz*Math.cos(yaw),keys=[];if(Math.abs(right)>.06)keys.push(right>0?'KeyD':'KeyA');if(Math.abs(up)>.06)keys.push(up>0?'KeyW':'KeyS');await play.keyboard.hold(keys,40);}assert.fail('waypoint');}
export default async function(play){
 await play.waitFor(s=>s.grounded&&s.inventory);await go(play,-3.6,3.8);await play.advance(.2,{render:true});let s=await play.state();console.log('appearance-initial',JSON.stringify(s.appearance));assert.equal(s.appearance.bodyPositions,s.appearance.authoredBodyPositions);assert.equal(s.appearance.backpackEntities.length,1);
 await play.page.keyboard.press('Tab');await play.advance(.15,{render:true});const bag=s.inventory.equippedBackpack;await play.page.locator('[data-item-id="'+bag+'"]').click({button:'right'});await play.page.getByRole('menuitem',{name:'卸下',exact:true}).click();await play.advance(.15,{render:true});s=await play.state();console.log('appearance-detached',JSON.stringify(s.appearance));assert.equal(s.appearance.backpackEntities.length,0);assert.equal(s.inventory.equippedBackpack,null);
 await play.page.keyboard.press('Tab');await play.advance(.2,{render:true});await play.screenshot('world-without-backpack');assert.equal(await play.page.locator('.drop-label:visible').count(),0);
 await hoverDrop(play,bag);assert.equal(await play.page.locator('.drop-label:visible').count(),1);await play.screenshot('hover-only-pixel-description');await play.mouse.move([.95,.95]);await play.advance(.15,{render:true});assert.equal(await play.page.locator('.drop-label:visible').count(),0);
}
