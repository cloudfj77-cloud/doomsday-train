import { dimensions, type ItemConfig, type ItemInstance } from '../logic/items.ts';
export interface GridPlacement {item:ItemInstance;x:number;y:number;rotated:boolean}
/** Pure layout for a virtual world-items panel; it never assigns Item locations. */
export function nearbyLayout(items:ItemInstance[],config:ItemConfig,columns:number):{rows:number;placements:GridPlacement[]} {
 const occupied=new Set<string>(),placements:GridPlacement[]=[];let rows=1;
 for(const item of items){const d=config.definitions.find(d=>d.id===item.definition)!,[w,h]=dimensions(d,false);if(w>columns)throw Error('nearby-grid-too-narrow');
  outer:for(let y=0;;y++)for(let x=0;x+w<=columns;x++){
   if(Array.from({length:w*h},(_,k)=>`${x+k%w},${y+Math.floor(k/w)}`).some(k=>occupied.has(k)))continue;
   for(let dy=0;dy<h;dy++)for(let dx=0;dx<w;dx++)occupied.add(`${x+dx},${y+dy}`);
   placements.push({item,x,y,rotated:false});rows=Math.max(rows,y+h);break outer;
  }
 }return {rows,placements};
}
export function mountGrid(doc:Document,options:{id:string;label:string;columns:number;rows:number;cell:number;placements:GridPlacement[];renderItem:(placement:GridPlacement)=>HTMLElement}) {
 const grid=doc.createElement('div');grid.className='inventory-grid';grid.dataset.containerId=options.id;grid.setAttribute('aria-label',options.label);
 grid.style.cssText=`width:${options.columns*options.cell}px;height:${options.rows*options.cell}px;background-size:${options.cell}px ${options.cell}px`;
 for(const p of options.placements)grid.append(options.renderItem(p));return grid;
}
