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
async function press(play,key){await play.page.keyboard.press(key);await play.advance(.12,{render:true});}
async function drag(play,id,x,y,rotate=false){
  const grid=await play.page.locator('.inventory-grid').boundingBox(),item=await play.page.locator(`[data-item-id="${id}"]`).boundingBox();assert.ok(grid&&item);
  await play.page.mouse.move(item.x+10,item.y+10);await play.page.mouse.down();await play.page.mouse.move(grid.x+x*44+12,grid.y+y*44+12,{steps:6});
  if(rotate)await play.page.keyboard.press('KeyR');await play.page.mouse.up();await play.advance(.12,{render:true});
}
async function route(play,height){
  const shot=n=>play.screenshot(`items-${height}-${n}`);
  await play.reload();await play.waitFor(s=>s.grounded&&s.inventory);await play.page.locator('#item-ui .item-hud').waitFor();
  await shot('six-world-drops');
  for(const [id,x,z] of [['item-1',-3.6,3.8],['item-2',-4.5,3.2],['item-3',-2.7,3.7],['item-4',-2.5,2.5],['item-5',-4.5,4.4],['item-6',-2.4,4.6]]){
    await go(play,x,z);await play.advance(.1,{render:true});let s=await play.state();assert.equal(s.inventory.target,id);
    await shot(`prompt-${s.inventory.items[id].definition}`);await press(play,'KeyE');s=await play.state();assert.ok(!s.inventory.items[id]||s.inventory.items[id].location.kind==='grid');
    await shot(`pickup-${id}`);
  }
  await press(play,'Tab');let s=await play.state();assert.equal(s.inventory.open,true);assert.equal(gridItems(s).length,7);assert.equal(quantity(s,'scrap','grid'),25);
  await shot('backpack-six-types');const before=s;await play.keyboard.hold('KeyW',150);s=await play.state();assert.ok(Math.hypot(s.x-before.x,s.z-before.z)<.03,'bag locks movement');
  await play.page.keyboard.down('Tab');await play.advance(.1,{render:true});await play.page.keyboard.down('Tab');await play.advance(.1,{render:true});await play.page.keyboard.up('Tab');assert.equal((await play.state()).inventory.open,false,'Tab repeat toggles once');await press(play,'Tab');
  const water=gridItems(await play.state()).find(i=>i.definition==='water');await drag(play,water.id,8,4,true);s=await play.state();assert.deepEqual(s.inventory.items[water.id].location,{kind:'grid',parent:s.inventory.backpackId,x:8,y:4,rotated:true});await shot('rotated-water');
  await drag(play,water.id,9,5);s=await play.state();assert.equal(s.inventory.items[water.id].location.x,8);await shot('invalid-placement');
  const scrap=gridItems(s).find(i=>i.definition==='scrap'&&i.quantity===20);await play.page.locator(`[data-item-id="${scrap.id}"]`).click();await play.page.getByRole('button',{name:'拆分',exact:true}).click();
  await play.page.getByRole('spinbutton',{name:'数量'}).fill('3');await play.page.getByRole('button',{name:'确定',exact:true}).click();await play.advance(.12,{render:true});s=await play.state();assert.equal(s.inventory.items[scrap.id].quantity,17);await shot('split-quantity');
  await play.page.locator(`[data-item-id="${scrap.id}"]`).click();await play.page.getByRole('button',{name:'丢弃',exact:true}).click();await play.page.getByRole('spinbutton',{name:'数量'}).fill('4');await play.page.getByRole('button',{name:'确定',exact:true}).click();await play.advance(.12,{render:true});
  s=await play.state();assert.equal(quantity(s,'scrap','world'),4);assert.equal(quantity(s,'scrap','grid'),21);assert.equal(s.inventory.open,true);const partial=Object.values(s.inventory.items).find(i=>i.definition==='scrap'&&i.location.kind==='world');await shot('partial-drop-world');
  await press(play,'Escape');await go(play,partial.location.position[0],partial.location.position[2]);await press(play,'KeyE');s=await play.state();assert.equal(quantity(s,'scrap','grid'),25);assert.equal(quantity(s,'scrap','world'),0);await shot('repick-partial-drop');
  await press(play,'Tab');const rifle=gridItems(await play.state()).find(i=>i.definition==='rifle');await play.page.locator(`[data-item-id="${rifle.id}"]`).click();await play.page.getByRole('button',{name:'丢弃',exact:true}).click();await play.advance(.12,{render:true});s=await play.state();assert.equal(s.inventory.items[rifle.id].location.kind,'world');await shot('whole-drop');
  await press(play,'Escape');const drop=s.inventory.items[rifle.id].location.position;await go(play,drop[0],drop[2]);await press(play,'KeyE');s=await play.state();assert.equal(s.inventory.items[rifle.id].location.kind,'grid');await shot('repick-whole-drop');
  await press(play,'Tab');await play.page.getByRole('button',{name:'关闭 · Tab'}).click();await play.advance(.12,{render:true});s=await play.state();assert.equal(s.inventory.open,false);await play.keyboard.hold('KeyD',120);const after=await play.state();assert.ok(Math.hypot(after.x-s.x,after.z-s.z)>.1);await shot('movement-restored');
  await play.reload();await play.waitFor(s=>s.grounded&&s.inventory);s=await play.state();assert.equal(gridItems(s).length,0);assert.equal(quantity(s,'scrap','world'),25);await go(play,-3.6,3.8);await press(play,'KeyE');await press(play,'Tab');s=await play.state();assert.equal(quantity(s,'scrap','grid'),25);assert.equal(s.inventory.open,true);await shot('reload-pickup-backpack');
}
export default async function(play){for(const height of [720,600]){await play.page.setViewportSize({width:1280,height});await play.step(`item-loop-${height}`,()=>route(play,height));}}
