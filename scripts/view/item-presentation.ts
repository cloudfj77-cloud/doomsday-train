import type { ItemConfig, ItemProjection, ItemCommand } from '../logic/items.ts';
import palette from '../../config/presentation/scene.json';
export interface ItemStyle {
  keys:{toggle:string;close:string;pickup:string;rotate:string};cellSize:number;colors:Record<'panel'|'grid'|'line'|'accent'|'text'|'valid'|'invalid',string>;
  visuals:Record<string,{color:string;symbol:string;boxes:(number|string)[][]}>;
}
export function itemKeyCommand(code:string,repeat:boolean,keys:ItemStyle['keys'],state:ItemProjection|undefined):ItemCommand|undefined {
  if(repeat)return;
  if(code===keys.toggle)return {type:'toggle'};
  if(code===keys.pickup&&state&&!state.open&&state.target)return {type:'pickup',id:state.target};
}
export function validateItemStyle(value:unknown,c:ItemConfig):ItemStyle {
  const v=value as ItemStyle;
  if(!v||!Number.isFinite(v.cellSize)||v.cellSize<24||v.cellSize>60||!v.keys||new Set(Object.values(v.keys)).size!==4||
    !['toggle','close','pickup','rotate'].every(k=>typeof v.keys[k as keyof typeof v.keys]==='string'&&v.keys[k as keyof typeof v.keys].length>0)||
    !v.colors||!['panel','grid','line','accent','text','valid','invalid'].every(k=>/^#[\da-f]{6}$/i.test(v.colors[k as keyof typeof v.colors]))||!v.visuals)throw Error('invalid-item-presentation');
  for(const d of c.definitions.filter(d=>d.portable)) {
    const a=v.visuals[d.id];if(!a||!/^#[\da-f]{6}$/i.test(a.color)||!a.symbol?.trim()||!Array.isArray(a.boxes)||!a.boxes.length||
      a.boxes.some(b=>b.length!==7||!b.slice(0,6).every(n=>typeof n==='number'&&Number.isFinite(n))||!b.slice(3,6).every(n=>Number(n)>0)||!(String(b[6]) in palette.palette)))throw Error('invalid-item-visual: '+d.id);
  }return v;
}
