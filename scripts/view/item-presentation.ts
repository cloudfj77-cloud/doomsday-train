import type { ItemConfig, ItemProjection, ItemCommand } from '../logic/items.ts';
import { sceneConfig as palette } from '../config/game.ts';
export interface ItemStyle {
  keys:{toggle:string;close:string;pickup:string;rotate:string};cellSize:number;colors:Record<'panel'|'grid'|'line'|'accent'|'text'|'valid'|'invalid',string>;
  scrollbar:{width:number;track:string;thumb:string;hover:string};
  proximity:{outlineColor:number[];outlineWidth:number;labelHeight:number;fontSize:number;pixelScale:number;textColor:string;backgroundColor:string;gap:number};
  holding:Record<'pistol'|'smg',{armPitch:number;armLift:number;armForward:number;gunRight:number;gunHeight:number;gunForward:number;gunScale:number;bob:number}>;
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
  if(!v.scrollbar||!Number.isInteger(v.scrollbar.width)||v.scrollbar.width<4||v.scrollbar.width>20||![v.scrollbar.track,v.scrollbar.thumb,v.scrollbar.hover].every(c=>/^#[a-f0-9]{6}$/i.test(c)))throw Error('invalid-scrollbar-style');
  const p=v.proximity;if(!p||!Array.isArray(p.outlineColor)||p.outlineColor.length!==3||p.outlineColor.some(n=>!Number.isFinite(n)||n<0||n>1)||!Number.isInteger(p.outlineWidth)||p.outlineWidth<1||p.outlineWidth>8||![p.labelHeight,p.fontSize,p.pixelScale,p.gap].every(n=>Number.isFinite(n)&&n>0)||!Number.isInteger(p.fontSize)||!Number.isInteger(p.pixelScale)||![p.textColor,p.backgroundColor].every(c=>/^#[a-f0-9]{6}$/i.test(c)))throw Error('invalid-proximity-style');
  if(!v.holding||!['pistol','smg'].every(k=>{const p=v.holding[k as 'pistol'|'smg'];return p&&['armPitch','armLift','armForward','gunRight','gunHeight','gunForward','gunScale','bob'].every(key=>Number.isFinite(p[key as keyof typeof p])&&p[key as keyof typeof p]>=0)&&p.gunScale>0;}))throw Error('invalid-holding-pose');
  for(const d of c.definitions.filter(d=>d.portable)) {
    const a=v.visuals[d.id];if(!a||!/^#[\da-f]{6}$/i.test(a.color)||!a.symbol?.trim()||!Array.isArray(a.boxes)||!a.boxes.length||
      a.boxes.some(b=>b.length!==7||!b.slice(0,6).every(n=>typeof n==='number'&&Number.isFinite(n))||!b.slice(3,6).every(n=>Number(n)>0)||!(String(b[6]) in palette.palette)))throw Error('invalid-item-visual: '+d.id);
  }return v;
}
