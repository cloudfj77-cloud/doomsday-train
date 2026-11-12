import {it,expect} from 'vitest';
import { itemConfig as raw } from '../../scripts/config/game.ts';
import { itemWorldConfig as world } from '../../scripts/config/game.ts';
import {validateItems,createItems,commandItems,projectItems,assertItemState,nearbyItems,pickupTarget,canPlace,carriedByPlayer,type ItemCommand} from '../../scripts/logic/items.ts';
function setup(){const {config:c,world:w}=validateItems(structuredClone(raw),world),s=createItems(c,w),e={player:[-3.6,0,3.8] as [number,number,number],visible:()=>true,dropPosition:()=>[-3.3,0,3.8] as [number,number,number]|undefined};s.open=true;const run=(cmd:ItemCommand)=>commandItems(s,c,w,cmd,e);return {s,c,w,e,run};}
it('always projects 2x3 pockets and removes the backpack tab when detached',()=>{
 const {s,c,run}=setup(),bag=s.backpackId;expect(projectItems(s,c).containers.map(g=>[g.id,g.columns,g.rows])).toEqual([[s.playerId,2,3],[bag,10,6]]);
 expect(run({type:'unequip',id:bag}).ok).toBe(true);expect(s.items[bag].location.kind).toBe('world');expect(s.backpackId).toBe(s.playerId);
 expect(projectItems(s,c).containers.map(g=>g.id)).toEqual([s.playerId]);expect(projectItems(s,c).equippedBackpack).toBeNull();
 expect(run({type:'equip',id:bag}).ok).toBe(true);expect(projectItems(s,c).equippedBackpack).toBe(bag);assertItemState(s,c);
});
it('moves the whole bag tree without scattering, copying, or remotely consuming contents',()=>{
 const {s,c,e,run}=setup(),bag=s.backpackId;
 expect(run({type:'take',id:'item-1',parent:bag}).ok).toBe(true);e.player=[-4.5,0,3.2];expect(run({type:'take',id:'item-2',parent:bag}).ok).toBe(true);
 const children=Object.values(s.items).filter(i=>i.location.kind==='grid'&&i.location.parent===bag),before=structuredClone(children),mass=projectItems(s,c).weight;
 expect(run({type:'unequip',id:bag}).ok).toBe(true);expect(projectItems(s,c).weight).toBe(0);expect(children.map(i=>s.items[i.id])).toEqual(before);expect(carriedByPlayer(s,children[0].id)).toBe(false);
 e.player=[100,0,0];const snapshot=JSON.stringify(s.items);expect(run({type:'move',id:children[0].id,parent:s.playerId,x:0,y:0,rotated:false}).ok).toBe(false);expect(run({type:'equip',id:bag}).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(snapshot);
 e.player=[-3.3,0,3.8];expect(run({type:'equip',id:bag}).ok).toBe(true);expect(projectItems(s,c).weight).toBeCloseTo(mass);expect(children.map(i=>s.items[i.id])).toEqual(before);
});
it('keeps pocket items accessible with no bag and permits ammo loading from pockets',()=>{
 const {s,c,e,run}=setup();const bag=s.backpackId;run({type:'unequip',id:bag});e.player=[-4.5,0,3.2];
 expect(run({type:'take',id:'item-2',parent:s.playerId,x:0,y:0,rotated:false}).ok).toBe(true);e.player=[-4.5,0,4.4];
 expect(run({type:'take',id:'item-5',parent:s.playerId,x:0,y:1,rotated:false}).ok).toBe(true);expect(run({type:'load',id:'item-5'})).toMatchObject({ok:true,amount:12});expect(run({type:'equip',id:'item-5'}).ok).toBe(true);expect(projectItems(s,c).equipped).toBe('item-5');
});
it('rolls back bag removal and replacement when no safe ground is available',()=>{
 const {s,e,run}=setup();e.dropPosition=()=>undefined;const before=JSON.stringify(s.items);
 expect(run({type:'unequip',id:s.backpackId}).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(before);
 e.player=[-2.4,0,4.6];expect(run({type:'equip',id:'item-6'}).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(before);
});
it('preserves bag contents on replacement and rejects placing a bag into its descendant',()=>{
 const {s,c,e,run}=setup(),old=s.backpackId;run({type:'take',id:'item-1',parent:old});e.player=[-2.4,0,4.6];expect(run({type:'equip',id:'item-6'}).ok).toBe(true);
 expect(s.backpackId).toBe('item-6');expect(s.items[old].location.kind).toBe('world');expect(Object.values(s.items).filter(i=>i.location.kind==='grid'&&i.location.parent===old).length).toBeGreaterThan(0);
 s.items['item-6'].location={kind:'grid',parent:old,x:0,y:2,rotated:false};expect(canPlace(s,c,old,'item-6',0,0,false)).toBe(false);
});

it('updates the active container when an equipped bag is dropped or dragged into pockets',()=>{
 for(const type of ['drop','unequip'] as const){const {s,c,run}=setup(),bag=s.backpackId;if(type==='unequip'){c.definitions.find(d=>d.id==='backpack')!.size=[1,1];}expect(run(type==='drop'?{type,id:bag,quantity:1}:{type,id:bag,parent:s.playerId,x:0,y:0,rotated:false}).ok).toBe(true);expect(s.backpackId).toBe(s.playerId);assertItemState(s,c);expect(run({type:'close'}).ok).toBe(true);}
});
it('picks the hovered nearby item before the nearest and falls back for stale hover',()=>{
 const {s,c,w,e}=setup();s.open=false;
 const near=nearbyItems(s,c,w,e);expect(near.length).toBeGreaterThan(1);const hovered=near[near.length-1];
 expect(pickupTarget(s,c,w,{...e,hovered})).toBe(hovered);expect(commandItems(s,c,w,{type:'pickup',id:near[0]},{...e,hovered}).ok).toBe(false);expect(commandItems(s,c,w,{type:'pickup',id:hovered},{...e,hovered}).ok).toBe(true);
 expect(pickupTarget(s,c,w,{...e,hovered:'missing'})).toBe(near[0]);expect(pickupTarget(s,c,w,{...e,hovered})).toBe(near[0]);
});
