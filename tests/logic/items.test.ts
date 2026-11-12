import { describe,it,expect } from 'vitest';
import { itemConfig as configRaw } from '../../scripts/config/game.ts';
import { itemWorldConfig as worldRaw } from '../../scripts/config/game.ts';
import {validateItems,createItems,commandItems,assertItemState,stat,canPlace,projectItems,nearestItem,type Environment,type ItemCommand} from '../../scripts/logic/items.ts';
const {config:c,world:w}=validateItems(configRaw,worldRaw);
function setup() {
  const s=createItems(c,w),e:Environment={player:[-3.6,0,3.8],visible:()=>true,dropPosition:()=>[-3.2,0,3.8]};
  const run=(cmd:ItemCommand)=>commandItems(s,c,w,cmd,e);
  return {s,e,run};
}
describe('Item authoritative transactions',()=>{
  it('never projects a missing or already transferred pickup target',()=>{
    const {s,run}=setup();expect(projectItems(s,c,'item-1').target).toBe('item-1');
    run({type:'pickup',id:'item-1'});expect(projectItems(s,c,'item-1').target).toBeNull();
    expect(projectItems(s,c,'removed').target).toBeNull();
  });
  it('rejects malformed definitions, references and capabilities',()=>{
    for(const mutate of [(v:any)=>v.definitions.push(v.definitions[0]),(v:any)=>v.definitions[1].size[0]=0,(v:any)=>v.definitions[6].maxStack=2,(v:any)=>v.definitions[1].weight=-1]) {
      const v=structuredClone(configRaw);mutate(v);expect(()=>validateItems(v,w)).toThrow();
    }
    expect(()=>validateItems(c,{...w,backpackDefinition:'missing'})).toThrow();
    expect(()=>validateItems(c,{...w,drops:[{definition:'missing',quantity:1,position:[0,0,0]}]})).toThrow();
  });
  it('uses nearest visible world item and distance; cannot pickup while open',()=>{
    const {s,e,run}=setup();expect(nearestItem(s,c,w,e)).toBe('item-1');
    e.visible=()=>false;expect(run({type:'pickup',id:'item-1'}).ok).toBe(false);
    e.visible=()=>true;e.player=[50,0,0];expect(run({type:'pickup',id:'item-1'}).ok).toBe(false);
    e.player=[-3.6,0,3.8];run({type:'toggle'});expect(run({type:'pickup',id:'item-1'}).ok).toBe(false);
  });
  it('picks stacks, splits, partially drops, and picks up again without changing total',()=>{
    const {s,run,e}=setup();const total=()=>Object.values(s.items).filter(i=>i.definition==='scrap').reduce((n,i)=>n+i.quantity,0);
    expect(run({type:'pickup',id:'item-1'}).ok).toBe(true);
    const held=()=>Object.values(s.items).filter(i=>i.definition==='scrap'&&i.location.kind==='grid');
    expect(held().map(i=>i.quantity).sort((a,b)=>a-b)).toEqual([5,20]);run({type:'toggle'});
    const id=held().find(i=>i.quantity===20)!.id;
    expect(run({type:'split',id,quantity:3}).ok).toBe(true);expect(s.items[id].quantity).toBe(17);
    expect(run({type:'drop',id,quantity:4}).ok).toBe(true);expect(s.items[id].quantity).toBe(13);expect(total()).toBe(25);
    const drop=Object.values(s.items).find(i=>i.definition==='scrap'&&i.location.kind==='world')!;
    expect(drop.id).not.toBe(id);expect(drop.quantity).toBe(4);expect(s.open).toBe(true);
    run({type:'close'});e.player=[-3.2,0,3.8];expect(run({type:'pickup',id:drop.id}).ok).toBe(true);expect(total()).toBe(25);assertItemState(s,c);
  });
  it('preserves ID on whole drop and rejects failed drops and invalid quantities atomically',()=>{
    const {s,run,e}=setup();run({type:'pickup',id:'item-1'});run({type:'toggle'});
    const id=Object.values(s.items).find(i=>i.definition==='scrap'&&i.location.kind==='grid')!.id;
    const before=JSON.stringify(s.items);e.dropPosition=()=>undefined;
    expect(run({type:'drop',id,quantity:1}).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(before);
    for(const quantity of [0,-1,1.5,999,NaN])expect(run({type:'drop',id,quantity}).ok).toBe(false);
    e.dropPosition=()=>[0,0,0];expect(run({type:'drop',id,quantity:s.items[id].quantity}).ok).toBe(true);expect(s.items[id].location.kind).toBe('world');
  });
  it('checks overlap, rotation, bounds, move rollback and merge',()=>{
    const {s,run,e}=setup();run({type:'pickup',id:'item-1'});e.player=[-2.5,0,2.5];run({type:'pickup',id:'item-4'});run({type:'toggle'});
    const before=JSON.stringify(s.items);expect(run({type:'move',id:'item-4',x:9,y:5,rotated:true}).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(before);
    expect(run({type:'move',id:'item-4',x:8,y:5,rotated:true}).ok).toBe(true);
    expect(canPlace(s,c,'item-4',s.backpackId,0,0,false)).toBe(false);
    expect(canPlace(s,c,'item-1',s.backpackId,5,2,true)).toBe(false);
    const target=Object.values(s.items).find(i=>i.definition==='scrap'&&i.quantity===20)!;
    run({type:'drop',id:target.id,quantity:3});
    const l=target.location;if(l.kind!=='grid')throw Error('expected grid');
    expect(run({type:'move',id:'item-1',x:l.x,y:l.y,rotated:false}).ok).toBe(true);expect(s.items[target.id].quantity).toBe(20);expect(s.items['item-1'].quantity).toBe(2);
  });
  it('partially picks into a full grid and fails split without space',()=>{
    const cc=structuredClone(c);cc.definitions.find(d=>d.id==='backpack')!.container={columns:1,rows:1};cc.definitions.find(d=>d.id==='player')!.container={columns:1,rows:1};
    const s=createItems(cc,w),e:Environment={player:[-3.6,0,3.8],visible:()=>true,dropPosition:()=>[0,0,0]};
    s.items.blocker={id:'blocker',definition:'bandage',quantity:1,variables:{},modifiers:[],location:{kind:'grid',parent:s.playerId,x:0,y:0,rotated:false}};
    expect(commandItems(s,cc,w,{type:'pickup',id:'item-1'},e)).toMatchObject({ok:true,amount:20});
    expect(s.items['item-1'].quantity).toBe(5);expect(commandItems(s,cc,w,{type:'pickup',id:'item-1'},e).ok).toBe(false);
    commandItems(s,cc,w,{type:'toggle'},e);const id=Object.values(s.items).find(i=>i.location.kind==='grid'&&i.definition==='scrap')!.id;
    const before=JSON.stringify(s.items);expect(commandItems(s,cc,w,{type:'split',id,quantity:2},e).ok).toBe(false);expect(JSON.stringify(s.items)).toBe(before);
  });
  it('enforces slots, ownership, cycles and explicit stat targets; freezes nested projection',()=>{
    const {s}=setup();s.items.player.modifiers=[{stat:'health',target:'player',operation:'add',value:20},{stat:'health',target:'player',operation:'multiply',value:2}];
    expect(stat(s,c,'player','health')).toBe(240);assertItemState(s,c);
    const p=projectItems(s,c);expect(Object.isFrozen(p.items.player.variables)).toBe(true);
    const originalWater=structuredClone(s.items['item-4'].location),waterQuantity=s.items['item-4'].quantity;s.items['item-4'].quantity=1;s.items['item-4'].location={kind:'slot',parent:s.playerId,slot:'primary'};expect(()=>assertItemState(s,c)).toThrow('invalid-slot');s.items['item-4'].location=originalWater;s.items['item-4'].quantity=waterQuantity;
    s.items[s.backpackId].location={kind:'slot',parent:s.playerId,slot:'unknown'};expect(()=>assertItemState(s,c)).toThrow();
    s.items[s.backpackId].location={kind:'grid',parent:s.backpackId,x:0,y:0,rotated:false};expect(()=>assertItemState(s,c)).toThrow();
    s.items[s.backpackId].location={kind:'slot',parent:'missing',slot:'backpack'};expect(()=>assertItemState(s,c)).toThrow();
    s.items[s.backpackId].location={kind:'slot',parent:s.playerId,slot:'backpack'};s.items['item-1'].id='wrong';expect(()=>assertItemState(s,c)).toThrow();
  });
});
