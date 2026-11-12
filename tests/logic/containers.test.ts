import {it,expect} from 'vitest';
import { itemConfig as raw } from '../../scripts/config/game.ts';
import { itemWorldConfig as world } from '../../scripts/config/game.ts';
import {validateItems,createItems,commandItems,projectContainer,assertItemState} from '../../scripts/logic/items.ts';
it('reuses grid placement and split across independent container sizes',()=>{
 const {config:c,world:w}=validateItems(raw,world),s=createItems(c,w);
 c.definitions.push({...c.definitions.find(d=>d.id==='backpack')!,id:'store',container:{columns:4,rows:3}});
 s.items.store={id:'store',definition:'store',quantity:1,variables:{},modifiers:[],location:{kind:'world',position:[-3.6,0,3.8]}};
 const e={player:[-3.6,0,3.8] as [number,number,number],visible:()=>true,dropPosition:()=>undefined};
 commandItems(s,c,w,{type:'pickup',id:'item-1'},e);s.open=true;
 const i=Object.values(s.items).find(i=>i.location.kind==='grid'&&i.quantity===20)!;
 expect(commandItems(s,c,w,{type:'move',id:i.id,parent:'store',x:3,y:2,rotated:false},e).ok).toBe(true);
 expect(projectContainer(s,c,'store').items[0].id).toBe(i.id);
 expect(commandItems(s,c,w,{type:'split',id:i.id,quantity:2,parent:'store'},e).ok).toBe(true);
 expect(projectContainer(s,c,'store').items.length).toBe(2);
 const before=JSON.stringify(s.items);
 expect(commandItems(s,c,w,{type:'move',id:i.id,parent:'store',x:4,y:0,rotated:false},e).ok).toBe(false);
 expect(JSON.stringify(s.items)).toBe(before);assertItemState(s,c);
});
