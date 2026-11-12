import { sceneConfig as scene } from '../config/game.ts';
import { actorBoxes, actorPartPose, type ActorPart } from './actor-model.ts';
import type { ItemStyle } from './item-presentation.ts';
type Point=[number,number,number];
export interface ModelFace {points:[number,number][];depth:number;color:string;part:string}
const project=([x,y,z]:Point):[number,number]=>[x*.9-z*.43,-y+x*.12+z*.24];
/** Orthographic projection of the same boxes cooked into the scene actor's Pack. */
export function characterFaces(style:ItemStyle,weapon?:{definition:string;pose:'pistol'|'smg'},hasBackpack=true):ModelFace[]{
 const faces:ModelFace[]=[];
 function box(center:number[],size:number[],color:string,part:string,transform:(p:Point)=>Point){
  const corners:Point[]=[];for(const x of [-1,1])for(const y of [-1,1])for(const z of [-1,1])corners.push(transform([center[0]+x*size[0]/2,center[1]+y*size[1]/2,center[2]+z*size[2]/2]));
  for(const [indices,shade] of [[[0,1,3,2],.7],[[4,6,7,5],.88],[[0,4,5,1],.6],[[2,3,7,6],1.12],[[0,2,6,4],.72],[[1,5,7,3],1]] as [number[],number][]){
   const points=indices.map(i=>corners[i]);const rgb=[1,3,5].map(i=>Math.min(255,Math.round(parseInt(color.slice(i,i+2),16)*shade)));
   faces.push({points:points.map(project),depth:points.reduce((n,[x,y,z])=>n+x*.43+y*.25+z*.9,0)/4,color:`rgb(${rgb.join(',')})`,part});
  }
 }
 for(const part of (['body','left-leg','right-leg','left-arm','right-arm',...(hasBackpack?['backpack']:[])] as ActorPart[])){const rest=actorPartPose(part,weapon?.pose),c=Math.cos(rest.pitch),s=Math.sin(rest.pitch);
  for(const b of actorBoxes(part))box(b.center,b.size,scene.palette[b.ink],part,([x,y,z])=>[x,c*y-s*z+rest.y,s*y+c*z+rest.z]);
 }
 if(weapon){const p=style.holding[weapon.pose];for(const b of style.visuals[weapon.definition].boxes)box(b.slice(0,3).map(Number),b.slice(3,6).map(Number),scene.palette[b[6] as keyof typeof scene.palette],'weapon',([x,y,z])=>[p.gunRight-z*p.gunScale,p.gunHeight+y*p.gunScale,p.gunForward+x*p.gunScale]);}
 return faces.sort((a,b)=>a.depth-b.depth);
}
export function characterPreview(doc:Document,style:ItemStyle,weapon?:{definition:string;pose:'pistol'|'smg'},hasBackpack=true){
 const ns='http://www.w3.org/2000/svg',svg=doc.createElementNS(ns,'svg'),faces=characterFaces(style,weapon,hasBackpack),points=faces.flatMap(f=>f.points);
 const minX=Math.min(...points.map(p=>p[0])),maxX=Math.max(...points.map(p=>p[0])),minY=Math.min(...points.map(p=>p[1])),maxY=Math.max(...points.map(p=>p[1]));
 svg.setAttribute('viewBox',`${minX-.12} ${minY-.12} ${maxX-minX+.24} ${maxY-minY+.24}`);svg.setAttribute('role','img');svg.setAttribute('aria-label','人物模型');svg.classList.add('character-model');
 for(const f of faces){const p=doc.createElementNS(ns,'polygon');p.setAttribute('points',f.points.map(p=>p.join(',')).join(' '));p.setAttribute('fill',f.color);p.dataset.modelPart=f.part;svg.append(p);}return svg;
}
