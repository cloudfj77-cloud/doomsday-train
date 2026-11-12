import {mat4,ray,vec2} from '@forgeax/engine/math';
import {mapSceneToDisplay,type BarrelDistortionMapping} from '@forgeax/engine/render';
import type {ItemConfig,ItemProjection,Position} from '../logic/items.ts';
import type {ItemStyle} from './item-presentation.ts';
/** Use the accepted rendered camera, including output distortion, rather than a parallel camera. */
export function projectDropLabel(position:Position,mapping:BarrelDistortionMapping|undefined):{x:number;y:number}|undefined{
 if(!mapping?.camera)return;const vp=mat4.create();mat4.multiply(vp,mapping.camera.projectionMatrix,mapping.camera.viewMatrix);const point=vec2.create();
 const result=ray.worldToScreen(point,position,vp,mapping.width,mapping.height);if(result.behind||!result.onScreen)return;
 const display={x:0,y:0};if(!mapSceneToDisplay(display,mapping,point[0],point[1]))return;return {x:display.x/mapping.width,y:display.y/mapping.height};
}
export function mountDropLabels(parent:HTMLElement,canvas:HTMLCanvasElement,config:ItemConfig,style:ItemStyle){
 const doc=parent.ownerDocument,root=doc.createElement('div');root.id='drop-labels';root.style.cssText='position:fixed;inset:0;z-index:80;pointer-events:none;overflow:clip';parent.append(root);
 let state:ItemProjection|undefined,mapping:BarrelDistortionMapping|undefined;const nodes=new Map<string,{el:HTMLDivElement;text:string;width:number;height:number}>(),p=style.proximity;
 function make(id:string,value:string){
  const el=doc.createElement('div');el.className='drop-label';el.dataset.dropId=id;el.setAttribute('aria-label',value);el.style.cssText='position:absolute;pointer-events:none';
  const bitmap=doc.createElement('canvas'),ctx=bitmap.getContext('2d');if(!ctx)throw Error('drop-label-canvas-unavailable');ctx.font=p.fontSize+'px monospace';const width=Math.ceil(ctx.measureText(value).width)+6,height=p.fontSize+6;bitmap.width=width;bitmap.height=height;
  ctx.font=p.fontSize+'px monospace';ctx.textBaseline='top';ctx.fillStyle=p.textColor;ctx.fillText(value,3,2);const pixels=ctx.getImageData(0,0,width,height);for(let i=3;i<pixels.data.length;i+=4)pixels.data[i]=pixels.data[i]>=128?255:0;ctx.putImageData(pixels,0,0);
  bitmap.textContent=value;bitmap.style.cssText='display:block;image-rendering:pixelated;width:'+width*p.pixelScale+'px;height:'+height*p.pixelScale+'px;background:'+p.backgroundColor+';border:1px solid '+p.textColor+';box-shadow:2px 2px 0 #0008';
  const stem=doc.createElement('span');stem.className='label-stem';stem.style.cssText='position:absolute;left:50%;top:100%;border-left:1px solid '+p.textColor+'88';el.append(bitmap,stem);root.append(el);return {el,text:value,width:width*p.pixelScale+2,height:height*p.pixelScale+2};
 }
 function paint(){
  root.hidden=!state||state.open||!mapping;if(root.hidden||!state||!mapping)return;const rect=canvas.getBoundingClientRect(),used:{x:number;y:number;width:number;height:number}[]=[];
  const shown=state.hovered?[state.hovered]:[];const ids=new Set(shown);for(const [id,node] of nodes)if(!ids.has(id)){node.el.remove();nodes.delete(id);}
  for(const id of shown){const i=state.items[id];if(i?.location.kind!=='world')continue;const d=config.definitions.find(d=>d.id===i.definition)!,value=d.name+' ×'+i.quantity;
   let node=nodes.get(id);if(!node||node.text!==value){node?.el.remove();node=make(id,value);nodes.set(id,node);}
   const point=projectDropLabel([i.location.position[0],i.location.position[1]+p.labelHeight,i.location.position[2]],mapping);node.el.hidden=!point;if(!point)continue;
   const anchor={x:rect.left+point.x*rect.width,y:rect.top+point.y*rect.height},x=Math.max(8,Math.min(anchor.x-node.width/2,doc.defaultView!.innerWidth-node.width-8));let y=anchor.y-node.height-p.gap;
   for(let n=0;n<state.nearby.length;n++){const hit=used.find(r=>x<r.x+r.width+p.gap&&x+node!.width+p.gap>r.x&&y<r.y+r.height+p.gap&&y+node!.height+p.gap>r.y);if(!hit)break;y=hit.y-node.height-p.gap;}
   y=Math.max(8,y);used.push({x,y,width:node.width,height:node.height});node.el.style.left=Math.round(x)+'px';node.el.style.top=Math.round(y)+'px';(node.el.lastElementChild as HTMLElement).style.height=Math.max(0,anchor.y-y-node.height)+'px';
  }
 }
 return {update(next:ItemProjection){state=next;paint();},frame(next:BarrelDistortionMapping|undefined){mapping=next;paint();},dispose(){root.remove();nodes.clear();}};
}
