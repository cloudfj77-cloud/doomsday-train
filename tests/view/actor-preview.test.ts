import {it,expect} from 'vitest';
import {actorBoxes,actorPartPose,type ActorPart} from '../../scripts/view/actor-model.ts';
import {characterFaces} from '../../scripts/view/actor-preview.ts';
import {validateItemStyle} from '../../scripts/view/item-presentation.ts';
import {validateItems} from '../../scripts/logic/items.ts';
import raw from '../../config/gameplay/items.json';
import world from '../../config/gameplay/item-world.json';
import styleRaw from '../../config/presentation/items.json';
const style=validateItemStyle(styleRaw,validateItems(raw,world).config);
it('projects the actual shared actor geometry with all five body parts',()=>{
 const parts:ActorPart[]=['body','left-leg','right-leg','left-arm','right-arm'],faces=characterFaces(style);
 expect(faces.length).toBe(parts.reduce((n,part)=>n+actorBoxes(part).length*6,0));
 expect(new Set(faces.map(f=>f.part))).toEqual(new Set(parts));expect(faces.every(f=>f.points.every(p=>p.every(Number.isFinite)))).toBe(true);
 expect(faces.every((f,i)=>i===0||f.depth>=faces[i-1].depth)).toBe(true);
});
it('reflects equipped weapon geometry and the same arm rest pose used in the scene',()=>{
 const pistol=characterFaces(style,{definition:'pistol',pose:'pistol'}),smg=characterFaces(style,{definition:'smg',pose:'smg'});
 expect(pistol.filter(f=>f.part==='weapon').length).toBe(style.visuals.pistol.boxes.length*6);
 expect(smg.filter(f=>f.part==='weapon').length).toBe(style.visuals.smg.boxes.length*6);
 expect(pistol.filter(f=>f.part==='left-arm')).not.toEqual(smg.filter(f=>f.part==='left-arm'));
 expect(actorPartPose('body','smg')).toEqual({pitch:0,y:0,z:0});expect(actorPartPose('left-arm').pitch).toBe(0);expect(actorPartPose('left-arm','pistol').pitch).toBeLessThan(0);
});
