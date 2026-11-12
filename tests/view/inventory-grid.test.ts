import {it,expect} from 'vitest';
import raw from '../../config/gameplay/items.json';
import world from '../../config/gameplay/item-world.json';
import {createItems,validateItems} from '../../scripts/logic/items.ts';
import {nearbyLayout} from '../../scripts/view/inventory-grid.ts';
import {heldTransform} from '../../scripts/view/scene.ts';
import {createState,validateConfig} from '../../scripts/logic/world.ts';
import movement from '../../config/gameplay/world.json';
it('lays out every nearby item without mutating world ownership or overlapping footprints',()=>{
 const {config:c,world:w}=validateItems(raw,world),s=createItems(c,w),items=Object.values(s.items).filter(i=>i.location.kind==='world'),before=JSON.stringify(items),layout=nearbyLayout(items,c,5),cells=new Set();
 expect(layout.placements.length).toBe(items.length);
 for(const p of layout.placements){const d=c.definitions.find(d=>d.id===p.item.definition)!;for(let dy=0;dy<d.size[1];dy++)for(let dx=0;dx<d.size[0];dx++){const key=`${p.x+dx},${p.y+dy}`;expect(cells.has(key)).toBe(false);cells.add(key);}}
 expect(JSON.stringify(items)).toBe(before);
});
it('uses distinct configured weapon offsets and follows facing and movement',()=>{
 const s=createState(validateConfig(movement)),a=heldTransform(s,'pistol'),b=heldTransform(s,'smg');expect(a.pos).not.toEqual(b.pos);
 const facing={...s,facingX:1,facingZ:0};expect(heldTransform(facing,'pistol').pos).not.toEqual(a.pos);
 expect(heldTransform({...s,moving:true,walkDistance:.2},'pistol').pos[1]).toBeGreaterThan(a.pos[1]);
});
