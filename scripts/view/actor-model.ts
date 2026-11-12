import style from '../../config/presentation/scene.json';
import itemStyle from '../../config/presentation/items.json';
export type ActorPart='body'|'left-leg'|'right-leg'|'left-arm'|'right-arm';
export interface ActorBox {center:[number,number,number];size:[number,number,number];ink:keyof typeof style.palette}
export function actorBoxes(part:ActorPart):ActorBox[] {
  const boxes:ActorBox[]=[],u=style.actor.unit;
  const block=(x:number,y:number,z:number,w:number,h:number,d:number,ink:ActorBox['ink'])=>boxes.push({center:[x*u,y*u,z*u],size:[w*u,h*u,d*u],ink});
  if(part==='body') {
    block(0,8.7,0,4.1,5.3,2.8,'coat');block(0,6.65,0,4.2,.9,2.9,'leather');
    block(0,12.3,.1,3.3,3.2,2.9,'skin');block(0,13.1,-.5,3.45,2.2,2.1,'coal');
    block(0,14.15,.25,6,.65,5,'timber');block(0,15,0,4.1,1.45,3.4,'grain');block(0,14.45,0,4.2,.43,3.48,'leather');
    block(0,11.1,.6,3.65,1,2.9,'red');block(1,10.2,1.56,.88,1.9,.4,'red');
    block(0,9,-2,3.8,4.7,2.1,'leather');block(0,10.65,-2.25,4.1,1.15,2.1,'grain');

  } else if(part==='left-arm'||part==='right-arm'){
    const side=part==='left-arm'?-1:1;block(side*2.55,9.05,0,1.12,3.4,1.35,'teal');block(side*2.55,7.3,.1,1,1,1.18,'skin');
  } else {
    const x=part==='left-leg'?-1.14:1.14;
    block(x,4.8,0,1.6,3.4,1.8,'teal');block(x,2,0,1.45,2.5,1.5,'coal');block(x,.63,.32,1.85,1.25,2.4,'timber');
  }
  return boxes;
}
/** Rest transform shared by world rendering and the inventory model. */
export function actorPartPose(part:string,pose?:'pistol'|'smg'){
 if(!pose||(part!=='left-arm'&&part!=='right-arm'))return {pitch:0,y:0,z:0};
 const holding=itemStyle.holding[pose],pitch=-holding.armPitch,pivot=style.actor.unit*10.75;
 return {pitch,y:holding.armLift+pivot*(1-Math.cos(pitch)),z:-Math.sin(pitch)*pivot};
}
