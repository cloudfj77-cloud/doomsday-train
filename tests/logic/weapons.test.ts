import {it,expect} from 'vitest';
import raw from '../../config/gameplay/items.json';
import world from '../../config/gameplay/item-world.json';
import {validateItems,createItems,commandItems,assertItemState,projectItems,nearbyItems,type ItemCommand} from '../../scripts/logic/items.ts';
function setup(){const {config:c,world:w}=validateItems(structuredClone(raw),world),s=createItems(c,w),e={player:[-3.8,0,4] as [number,number,number],visible:()=>true,dropPosition:()=>[0,0,0] as [number,number,number]};s.open=true;const run=(cmd:ItemCommand)=>commandItems(s,c,w,cmd,e);for(const id of ['item-2','item-5','item-7'])expect(run({type:'take',id}).ok).toBe(true);return {s,c,w,e,run};}
it('conserves actual rounds across loading, equipment, dropping and unloading',()=>{
 const {s,c,run}=setup();const rounds=()=>Object.values(s.items).reduce((n,i)=>n+(i.definition==='ammo'?i.quantity:0)+(i.loaded??0),0);
 expect(rounds()).toBe(90);const before=projectItems(s,c).weight;
 expect(run({type:'load',id:'item-5'})).toMatchObject({ok:true,amount:12});expect(s.items['item-5'].loaded).toBe(12);expect(rounds()).toBe(90);expect(projectItems(s,c).weight).toBeCloseTo(before);
 expect(run({type:'equip',id:'item-5'}).ok).toBe(true);expect(projectItems(s,c).equipped).toBe('item-5');
 expect(run({type:'load',id:'item-7'})).toMatchObject({ok:true,amount:30});expect(run({type:'equip',id:'item-7'}).ok).toBe(true);expect(s.items['item-5'].location.kind).toBe('grid');expect(rounds()).toBe(90);
 expect(run({type:'unload',id:'item-7'})).toMatchObject({ok:true,amount:30});expect(run({type:'unequip',id:'item-7'}).ok).toBe(true);
 expect(run({type:'drop',id:'item-5',quantity:1}).ok).toBe(true);expect(s.items['item-5'].loaded).toBe(12);expect(rounds()).toBe(90);assertItemState(s,c);
});
it('rolls back equipment replacement and unloading when no placement exists',()=>{
 const {s,c,run}=setup();run({type:'equip',id:'item-7'});run({type:'load',id:'item-7'});
 // A two-cell bag can contain the pistol, but cannot receive the larger equipped SMG.
 for(const [id,i] of Object.entries(s.items))if(i.location.kind==='grid'&&id!=='item-5')delete s.items[id];
 c.definitions.find(d=>d.id==='backpack')!.container={columns:2,rows:1};s.items['item-5'].location={kind:'grid',parent:s.backpackId,x:0,y:0,rotated:false};
 for(let n=0;n<6;n++){const id='block-'+n;s.items[id]={id,definition:'bandage',quantity:1,variables:{},modifiers:[],location:{kind:'grid',parent:s.playerId,x:n%2,y:Math.floor(n/2),rotated:false}};}
 let before=JSON.stringify(s.items);expect(run({type:'equip',id:'item-5'}).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(before);
 expect(run({type:'unload',id:'item-7'}).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(before);
 expect(run({type:'equip',id:s.backpackId}).ok).toBe(false);
});
it('validates ammo references and rejects out of capacity loaded state',()=>{
 const {s,c,w}=setup();c.definitions.find(d=>d.id==='pistol')!.weapon!.ammo='missing';expect(()=>validateItems(c,w)).toThrow('ammo-reference');
 c.definitions.find(d=>d.id==='pistol')!.weapon!.ammo='ammo';s.items['item-5'].loaded=13;expect(()=>assertItemState(s,c)).toThrow('loaded');
});
it('nearby rechecks range and visibility even while inventory is open',()=>{
 const {s,c,w,e,run}=setup();expect(nearbyItems(s,c,w,e).length).toBeGreaterThan(0);e.visible=()=>false;
 expect(nearbyItems(s,c,w,e)).toEqual([]);const before=JSON.stringify(s.items);expect(run({type:'take',id:'item-1'}).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(before);
 e.visible=()=>true;e.player=[100,0,0];expect(run({type:'take',id:'item-1'}).ok).toBe(false);
});
it('transfers a world stack to a specified cell without losing its remainder',()=>{
 const {config:c,world:w}=validateItems(raw,world),s=createItems(c,w),e={player:[-4.5,0,3.2] as [number,number,number],visible:()=>true,dropPosition:()=>undefined};s.open=true;
 expect(commandItems(s,c,w,{type:'take',id:'item-2',x:2,y:2,rotated:false},e)).toMatchObject({ok:true,amount:60});
 expect(s.items['item-2'].quantity).toBe(30);const held=Object.values(s.items).find(i=>i.definition==='ammo'&&i.location.kind==='grid')!;
 expect(held.quantity).toBe(60);commandItems(s,c,w,{type:'drop',id:held.id,quantity:1},{...e,dropPosition:()=>[0,0,0]});
 expect(commandItems(s,c,w,{type:'take',id:'item-2',x:2,y:2,rotated:false},e)).toMatchObject({ok:true,amount:1});expect(s.items['item-2'].quantity).toBe(29);expect(s.items[held.id].quantity).toBe(60);
 const before=JSON.stringify(s.items);expect(commandItems(s,c,w,{type:'take',id:'item-2',x:99,y:2,rotated:false},e).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(before);
});
