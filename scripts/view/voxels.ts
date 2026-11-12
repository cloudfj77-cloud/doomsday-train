import { actorBoxes } from './actor-model.ts';
import { createBoxGeometry, meshFromInterleaved, withVertexColors } from '@forgeax/engine/geometry';
import style from '../../config/presentation/scene.json';
import data from '../../config/gameplay/world.json';
import itemStyle from '../../config/presentation/items.json';
import itemData from '../../config/gameplay/items.json';
import itemWorld from '../../config/gameplay/item-world.json';
import { validateItems } from '../logic/items.ts';
import { validateItemStyle } from './item-presentation.ts';

type Ink = keyof typeof style.palette;
const cube = createBoxGeometry(1, 1, 1).unwrap();
function attribute(name: 'position' | 'normal' | 'uv') {
  const values=cube.attributes[name];
  if (!(values instanceof Float32Array)) throw Error('missing-unit-cube-attribute: '+name);
  return values;
}
const position=attribute('position'), normal=attribute('normal'), uv=attribute('uv');
const indices=cube.indices;
if (!indices) throw Error('missing-unit-cube-indices');

// Each box contributes flat-shaded faces to a single indexed part.
export interface Box { center: number[]; size: number[]; pitch: number; yaw: number }
class Blocks {
  boxes: Box[] = [];
  vertices: number[] = [];
  indices: number[] = [];
  colors: number[] = [];
  box(x:number,y:number,z:number,w:number,h:number,d:number, ink: Ink, yaw = 0, pitch = 0) {
    if (![x,y,z,w,h,d,yaw].every(Number.isFinite) || Math.min(w,h,d)<=0) throw Error('invalid-voxel-box');
    if (ink !== 'shadow' && ink !== 'grass' && y+h/2>.04) this.boxes.push({center:[x,y,z],size:[w,h,d],pitch,yaw});
    const hex = style.palette[ink];
    if (!/^#[a-f0-9]{6}$/i.test(hex)) throw Error('invalid-voxel-palette');
    const rgb = [1,3,5].map(i => parseInt(hex.slice(i,i+2),16)/255);
    const offset = this.vertices.length / 8, c = Math.cos(yaw), s = Math.sin(yaw);
    for(let i=0;i<position.length/3;i++) {
      const px=position[i*3]*w,pz=position[i*3+2]*d;
      const nx=normal[i*3]*c+normal[i*3+2]*s,nz=-normal[i*3]*s+normal[i*3+2]*c,ny=normal[i*3+1];
      const cp=Math.cos(pitch),sp=Math.sin(pitch),py=position[i*3+1]*h;
      const ry=cp*py-sp*pz,rz=sp*py+cp*pz;
      this.vertices.push(x+c*px+s*rz,y+ry,z-s*px+c*rz,c*normal[i*3]+s*(sp*ny+cp*normal[i*3+2]),cp*ny-sp*normal[i*3+2],-s*normal[i*3]+c*(sp*ny+cp*normal[i*3+2]),uv[i*2],uv[i*2+1]);
      const shade=.76+.23*Math.max(ny,0)+.13*Math.max(-nx,0)+.03*Math.max(nz,0);
      this.colors.push(rgb[0]*shade,rgb[1]*shade,rgb[2]*(shade+(1-shade)*.27));
    }
    for (const index of indices!) this.indices.push(offset+index);
  }
  finish() {
    const mesh = meshFromInterleaved(new Float32Array(this.vertices),new Uint32Array(this.indices)).unwrap();
    return withVertexColors(mesh,this.colors,{space:'srgb'}).unwrap();
  }
}

export function bakeItem(id:string) {
  const {config}=validateItems(itemData,itemWorld);
  const style=validateItemStyle(itemStyle,config),visual=style.visuals[id];
  if(!visual)throw Error('missing-item-mesh: '+id);
  const blocks=new Blocks();
  for(const b of visual.boxes)blocks.box(Number(b[0]),Number(b[1]),Number(b[2]),Number(b[3]),Number(b[4]),Number(b[5]),b[6] as Ink);
  return blocks.finish();
}

function terrain() {
  const b=new Blocks(), layout=style.layout;
  let seed=layout.seed;
  const rand=()=>{seed=(Math.imul(seed,1664525)+1013904223)>>>0;return seed/4294967296;};
  const [w,d]=data.ground;
  b.box(0,-.2,0,w,.4,d,'sand');
  for(let i=0;i<layout.gravelCount;i++) {
    const x=rand()*24-12,z=rand()*18-9,r=rand();
    b.box(x,.009,z,.04+r*.15,.016,.04+r*.07,r>.6?'light':'ripple');
    if(r>.75) b.box(x+.24,.01,z+.07,.055,.02,.065,'ripple');
  }
  function cliff(x:number,z:number,width:number,depth:number,height:number) {
    // Stepped shadow strips follow the same easterly sun direction throughout.
    for(let j=0;j<4;j++) b.box(x+width*.20+height*j*.17,.014+j*.001,z+depth*.13+height*j*.09,width*(.86-j*.08),.012,depth*.78,'shadow');
    layout.stoneBands.forEach((ink,i)=>{
      const count=5, unit=width/count, layer=height/layout.stoneBands.length;
      for(let k=0;k<count;k++) {
        const taper=1-i*.07, dz=depth*(.65+rand()*.32)*taper;
        b.box(x+(k-2)*unit*taper,(i+.5)*layer,z+(rand()-.5)*depth*.09,unit*taper*1.04,layer+.002,dz,ink as Ink);
      }
    });
  }
  for(const [x,z,w,d,h] of layout.cliffs) cliff(x,z,w,d,h);
  function obstacle(id:string) {
    const o=data.obstacles.find(o=>o.id===id);
    if(!o) throw Error('missing-render-obstacle: '+id);
    return o;
  }
  const rock=obstacle('outcrop');
  cliff(rock.center[0],rock.center[1],rock.size[0],rock.size[1],layout.rockHeight);
  const rail=layout.rail,c=Math.cos(rail.yaw),s=Math.sin(rail.yaw);
  function railway(a:number,t:number,y:number,w:number,h:number,d:number,ink:Ink) {
    b.box(rail.center[0]+c*a+s*t,y,rail.center[1]-s*a+c*t,w,h,d,ink,rail.yaw);
  }
  for(let i=0;i<rail.ties;i++) {
    const t=rail.length*(i/(rail.ties-1)-.5);
    railway(0,t,.047,1.32,.094,.17,i%3?'timber':'grain');
    for(const a of [-.44,.44]) railway(a,t,.103,.19,.026,.19,'steel');
  }
  for(const a of [-.44,.44]) {
    railway(a,-1,.143,.064,.08,rail.length-2,'steel');
    railway(a,-1,.19,.09,.022,rail.length-2,'rail');
  }
  // Thin drifts bury the broken end in steps across a few ties.
  for(let i=0;i<5;i++) railway(.65-i*.13,rail.length*.33+i*.36,.06,.65+i*.13,.13,.39,'sand');
  const depot=obstacle('depot'),[dx,dz]=depot.center;
  b.box(dx,.18,dz,depot.size[0],.36,depot.size[1],'timber');
  for(let i=0;i<7;i++) b.box(dx+depot.size[0]*(i-3)/7,.39,dz,depot.size[0]/7-.016,.07,depot.size[1],'grain');
  function crate(x:number,z:number,size:number,y:number) {
    b.box(x,y+size/2,z,size,size,size,'grain');
    for(const side of [-1,1]) {
      b.box(x+side*size*.34,y+size*.51,z,size*.11,size*1.025,size*1.028,'timber');
      b.box(x,y+size*.5+side*size*.37,z,size*1.015,size*.1,size*1.015,'timber');
    }
  }
  crate(dx-.41,dz,.68,.43);crate(dx+.28,dz+.05,.52,.43);crate(dx-.45,dz,.42,1.11);
  const [sx,sz]=layout.signal;
  b.box(sx+.51,.024,sz+.26,1.2,.015,.18,'shadow',-.26);
  b.box(sx,.12,sz,.42,.24,.37,'darkrock');
  b.box(sx,.99,sz,.17,1.74,.16,'timber');
  b.box(sx,1.77,sz,.93,.38,.16,'red');
  b.box(sx-.23,1.77,sz+.086,.11,.26,.018,'cream');
  for(let i=0;i<layout.grassCount;i++) {
    const x=rand()*20-10,z=rand()*17-8.5;
    if(Math.abs(x)<4 && Math.abs(z)<4)continue;
    for(let j=0;j<4;j++) {
      const h=.11+rand()*.27;
      b.box(x+(rand()-.5)*.31,h*.5,z+(rand()-.5)*.22,.06,h,.06,'grass');
    }
  }
  for (const o of data.surfaces) b.box(o.center[0],o.center[1],o.center[2],o.size[0],o.size[1],o.size[2],'rock',0,o.pitch*Math.PI/180);
  return b;
}

// These exact geometry descriptors also assemble the collision scene.
export function collisionBoxes() { return terrain().boxes; }
export const parts=['terrain','body','left-leg','right-leg','left-arm','right-arm','backpack','shade'] as const;
export type Part=typeof parts[number];
export function bake(part:Part) {
  if(part==='terrain')return terrain().finish();
  const b=new Blocks();
  if(part==='shade')b.box(.28,.027,.12,.91,.013,.35,'shadow',-.25);
  else for(const box of actorBoxes(part))b.box(...box.center,...box.size,box.ink);
  return b.finish();
}
