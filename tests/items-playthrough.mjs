import assert from 'node:assert/strict';
const yaw=-Math.PI/6;
async function go(play,x,z) {
  for(let i=0;i<100;i++){
    const s=await play.state(),dx=x-s.x,dz=z-s.z;if(Math.hypot(dx,dz)<.12)return;
    const right=dx*Math.cos(yaw)-dz*Math.sin(yaw),up=-dx*Math.sin(yaw)-dz*Math.cos(yaw),keys=[];
    if(Math.abs(right)>.06)keys.push(right>0?'KeyD':'KeyA');if(Math.abs(up)>.06)keys.push(up>0?'KeyW':'KeyS');
    await play.keyboard.hold(keys,40);
  }assert.fail('unreachable item position');
}
const gridItems=s=>Object.values(s.inventory.items).filter(i=>i.location.kind==='grid');
const quantity=(s,definition,kind)=>Object.values(s.inventory.items).filter(i=>i.definition===definition&&(!kind||i.location.kind===kind)).reduce((n,i)=>n+i.quantity,0);
async function press(play,key){await play.page.keyboard.press(key);await play.advance(.12,{render:true});if(key==='Tab'&&(await play.state()).inventory.open)await play.page.getByRole('tab',{name:'背包 10×6',exact:true}).click();}
async function drag(play,id,x,y,rotate=false){
  const grid=await play.page.locator('[aria-label="背包网格"]').boundingBox(),item=await play.page.locator(`[data-item-id="${id}"]`).boundingBox();assert.ok(grid&&item);
  await play.page.mouse.move(item.x+10,item.y+10);await play.page.mouse.down();await play.page.mouse.move(grid.x+x*44+12,grid.y+y*44+12,{steps:6});
  if(rotate)await play.page.keyboard.press('KeyR');await play.page.mouse.up();await play.advance(.12,{render:true});
}
async function route(play,height){
  const shot=n=>play.screenshot(`items-${height}-${n}`);
  await play.reload();await play.waitFor(s=>s.grounded&&s.inventory);await play.page.locator('#item-ui .item-hud').waitFor();
  await shot('seven-world-drops');
  await go(play,-3.6,3.8);await press(play,'Tab');let nearbyState=await play.state();assert.ok(nearbyState.inventory.nearby.length>=2,'all reachable nearby objects are listed');
  assert.equal(await play.page.locator('[data-container-id=nearby] [data-item-id]').count(),nearbyState.inventory.nearby.length);await shot('all-nearby-types');
  const character=await play.page.locator('.character-section').boundingBox(),bag=await play.page.locator('[aria-label="背包网格"]').boundingBox(),nearby=await play.page.locator('[data-container-id=nearby]').boundingBox();
  assert.ok(character.x+character.width<=bag.x&&bag.x+bag.width<=nearby.x,'character / backpack / nearby order');assert.equal(await play.page.locator('.character-section [data-slot]').count(),4);assert.equal(await play.page.locator('.grid-wrap [data-slot]').count(),0);assert.equal(await play.page.locator('.slot.selected').count(),0,'empty slots are not selected');const footer=await play.page.locator('.inventory-footer').boundingBox();assert.ok(footer.y+footer.height<height-18,'footer clears engine overlay');assert.ok(await play.page.locator('.character-model polygon').count()>100);
  assert.equal(await play.page.locator('.slot').evaluateAll(els=>els.every(el=>el.textContent.trim()==='')),true,'slots use icons rather than text');
  const hovered=nearbyState.inventory.nearby[0];await play.page.locator('[data-item-id="'+hovered+'"]').hover();await play.page.getByRole('tooltip').waitFor();assert.equal(await play.page.locator('.item-detail').count(),0);await shot('hover-detail');
  const tooltip=await play.page.getByRole('tooltip').boundingBox();assert.ok(tooltip.x>=0&&tooltip.y>=0&&tooltip.x+tooltip.width<=1280&&tooltip.y+tooltip.height<=height,'tooltip stays in viewport');
  await play.page.locator('[data-item-id="'+hovered+'"]').click({button:'right'});await play.page.getByRole('menu').waitFor();assert.equal(await play.page.getByRole('tooltip').count(),0);await shot('context-actions');const menuBounds=await play.page.getByRole('menu').boundingBox();assert.ok(menuBounds.x>=0&&menuBounds.y>=0&&menuBounds.x+menuBounds.width<=1280&&menuBounds.y+menuBounds.height<=height,'menu stays in viewport');await play.page.locator('.inventory-header h2').click();assert.equal(await play.page.getByRole('menu').count(),0,'outside click closes menu');await play.page.locator('[data-item-id="'+hovered+'"]').click({button:'right'});
  await press(play,'Escape');assert.equal(await play.page.getByRole('menu').count(),0);assert.equal((await play.state()).inventory.open,true,'Escape dismisses menu first');await press(play,'Tab');
  for(const [id,x,z] of [['item-1',-3.6,3.8],['item-2',-4.5,3.2],['item-3',-2.7,3.7],['item-4',-2.5,2.5],['item-5',-4.5,4.4],['item-6',-2.4,4.6],['item-7',-3.8,4.7]]){
    await go(play,x,z);await play.advance(.1,{render:true});let s=await play.state();assert.equal(s.inventory.target,id);
    await shot(`prompt-${s.inventory.items[id].definition}`);await press(play,'KeyE');s=await play.state();assert.ok(!s.inventory.items[id]||s.inventory.items[id].location.kind==='grid');
    await shot(`pickup-${id}`);
  }
  await press(play,'Tab');let s=await play.state();assert.equal(s.inventory.open,true);assert.equal(gridItems(s).length,9);assert.equal(quantity(s,'scrap','grid'),25);
  await shot('backpack-seven-types');
  const pistol=gridItems(s).find(i=>i.definition==='pistol'),smg=gridItems(s).find(i=>i.definition==='smg');
  const click=async(id,label)=>{await play.page.locator('[data-item-id="'+id+'"]').click({button:'right'});await play.page.getByRole('menuitem',{name:label,exact:true}).click();await play.advance(.15,{render:true});};
  await click(pistol.id,'装填');s=await play.state();assert.equal(s.inventory.items[pistol.id].loaded,12);assert.equal(quantity(s,'ammo','grid'),78);await shot('pistol-loaded');
  const from=await play.page.locator('[data-item-id="'+pistol.id+'"]').boundingBox(),slot=await play.page.locator('[data-slot="primary"]').boundingBox();
  await play.page.mouse.move(from.x+10,from.y+10);await play.page.mouse.down();await play.page.mouse.move(slot.x+30,slot.y+20,{steps:6});await play.page.mouse.up();await play.advance(.15,{render:true});
  s=await play.state();assert.equal(s.inventory.equipped,pistol.id);assert.ok(await play.page.locator('.character-model [data-model-part=weapon]').count()>0);await shot('pistol-equipped');
  await press(play,'Tab');await play.mouse.move([.8,.35]);await play.keyboard.hold('KeyD',160);await shot('pistol-moving');await press(play,'Tab');
  await click(smg.id,'装填');await click(smg.id,'装备');s=await play.state();assert.equal(s.inventory.equipped,smg.id);assert.equal(s.inventory.items[pistol.id].loaded,12);assert.equal(quantity(s,'ammo','grid'),48);await shot('smg-equipped');
  await press(play,'Tab');await play.keyboard.hold('KeyA',160);await shot('smg-moving');await press(play,'Tab');
  await click(smg.id,'卸弹');s=await play.state();assert.equal(s.inventory.items[smg.id].loaded,0);assert.equal(quantity(s,'ammo','grid'),78);await shot('unloaded-rounds');await click(smg.id,'卸下');s=await play.state();assert.equal(s.inventory.equipped,null);
const before=s;await play.keyboard.hold('KeyW',150);s=await play.state();assert.ok(Math.hypot(s.x-before.x,s.z-before.z)<.03,'bag locks movement');
  await play.page.keyboard.down('Tab');await play.advance(.1,{render:true});await play.page.keyboard.down('Tab');await play.advance(.1,{render:true});await play.page.keyboard.up('Tab');assert.equal((await play.state()).inventory.open,false,'Tab repeat toggles once');await press(play,'Tab');
  const water=gridItems(await play.state()).find(i=>i.definition==='water');await drag(play,water.id,8,4,true);s=await play.state();assert.deepEqual(s.inventory.items[water.id].location,{kind:'grid',parent:s.inventory.backpackId,x:8,y:4,rotated:true});await shot('rotated-water');
  await drag(play,water.id,9,5);s=await play.state();assert.equal(s.inventory.items[water.id].location.x,8);await shot('invalid-placement');
  const scrap=gridItems(s).find(i=>i.definition==='scrap'&&i.quantity===20);await play.page.locator(`[data-item-id="${scrap.id}"]`).click({button:'right'});await play.page.getByRole('menuitem',{name:'拆分',exact:true}).click();
  await play.page.getByRole('spinbutton',{name:'数量'}).fill('3');await play.page.getByRole('button',{name:'确定',exact:true}).click();await play.advance(.12,{render:true});s=await play.state();assert.equal(s.inventory.items[scrap.id].quantity,17);await shot('split-quantity');
  await play.page.locator(`[data-item-id="${scrap.id}"]`).click({button:'right'});await play.page.getByRole('menuitem',{name:'丢弃',exact:true}).click();await play.page.getByRole('spinbutton',{name:'数量'}).fill('4');await play.page.getByRole('button',{name:'确定',exact:true}).click();await play.advance(.12,{render:true});
  s=await play.state();assert.equal(quantity(s,'scrap','world'),4);assert.equal(quantity(s,'scrap','grid'),21);assert.equal(s.inventory.open,true);const partial=Object.values(s.inventory.items).find(i=>i.definition==='scrap'&&i.location.kind==='world');await shot('partial-drop-world');
  await press(play,'Escape');await go(play,partial.location.position[0],partial.location.position[2]);await press(play,'Tab');
  s=await play.state();assert.ok(s.inventory.nearby.includes(partial.id));assert.equal(await play.page.locator('[data-container-id=nearby] [data-item-id]').count(),s.inventory.nearby.length,'visible nearby grid matches authoritative projection');await play.page.locator('[data-item-id="'+partial.id+'"]').click({button:'right'});await play.page.getByRole('menuitem',{name:'拾取',exact:true}).click();await play.advance(.15,{render:true});await shot('nearby-click-pickup');await press(play,'Tab');s=await play.state();assert.equal(quantity(s,'scrap','grid'),25);assert.equal(quantity(s,'scrap','world'),0);await shot('repick-partial-drop');
  await press(play,'Tab');const rifle=gridItems(await play.state()).find(i=>i.definition==='pistol');await play.page.locator(`[data-item-id="${rifle.id}"]`).click({button:'right'});await play.page.getByRole('menuitem',{name:'丢弃',exact:true}).click();await play.advance(.12,{render:true});s=await play.state();assert.equal(s.inventory.items[rifle.id].location.kind,'world');await shot('whole-drop');
  await press(play,'Escape');const drop=s.inventory.items[rifle.id].location.position;await go(play,drop[0],drop[2]);await press(play,'KeyE');s=await play.state();assert.equal(s.inventory.items[rifle.id].location.kind,'grid');await shot('repick-whole-drop');
  await press(play,'Tab');const source=await play.page.locator('[data-item-id="'+rifle.id+'"]').boundingBox(),near=await play.page.locator('[data-container-id="nearby"]').boundingBox();
  await play.page.mouse.move(source.x+10,source.y+10);await play.page.mouse.down();await play.page.mouse.move(near.x+20,near.y+20,{steps:6});await play.page.mouse.up();await play.advance(.15,{render:true});s=await play.state();assert.equal(s.inventory.items[rifle.id].location.kind,'world');assert.equal(s.inventory.items[rifle.id].loaded,12);await shot('drag-to-nearby');
  await drag(play,rifle.id,6,4);s=await play.state();assert.equal(s.inventory.items[rifle.id].location.kind,'grid');assert.equal(s.inventory.items[rifle.id].location.x,6);await shot('nearby-drag-pickup');await press(play,'Tab');
  await press(play,'Tab');await play.page.getByRole('button',{name:'关闭 · Tab'}).click();await play.advance(.12,{render:true});s=await play.state();assert.equal(s.inventory.open,false);await play.keyboard.hold('KeyD',120);const after=await play.state();assert.ok(Math.hypot(after.x-s.x,after.z-s.z)>.1);await shot('movement-restored');
  await play.reload();await play.waitFor(s=>s.grounded&&s.inventory);s=await play.state();assert.equal(gridItems(s).length,0);assert.equal(quantity(s,'scrap','world'),25);await go(play,-3.6,3.8);await press(play,'KeyE');await press(play,'Tab');s=await play.state();assert.equal(quantity(s,'scrap','grid'),25);assert.equal(s.inventory.open,true);await shot('reload-pickup-backpack');
}
export default async function(play){for(const height of [720,600]){await play.page.setViewportSize({width:1280,height});await play.step(`item-loop-${height}`,()=>route(play,height));}}
