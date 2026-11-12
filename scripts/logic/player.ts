import type profile from '../../config/gameplay/player.json';
export type PlayerConfig=typeof profile;
export function validatePlayer(raw:unknown):PlayerConfig {
 const p=raw as PlayerConfig;
 const exact=(v:object|undefined,keys:string[])=>!!v&&Object.keys(v).length===keys.length&&keys.every(k=>Object.hasOwn(v,k));
 if(!p||!exact(p,['item','movement','body','spawn','initialBackpack','backpackSlot','pockets','slots','baseStats','reservedStats','keys'])||
 !exact(p.movement,['speed','runSpeed','turnTime','aimDeadzone'])||!Object.values(p.movement).every(n=>Number.isFinite(n)&&n>0)||p.movement.runSpeed<p.movement.speed||
 !exact(p.body,['radius','height','skin','maxSlope','stepHeight','stepWidth','snapDistance'])||!Object.values(p.body).every(n=>Number.isFinite(n)&&n>0)||p.body.height<=p.body.radius*2||p.body.maxSlope>=90||
 !Array.isArray(p.spawn)||p.spawn.length!==2||!p.spawn.every(Number.isFinite)||!p.initialBackpack?.trim()||!p.backpackSlot?.trim()||
 !exact(p.pockets,['columns','rows'])||!Object.values(p.pockets).every(n=>Number.isInteger(n)&&n>0)||!exact(p.item,['id','name','description','tags','size','rotatable','maxStack','weight','value','portable'])||!p.item?.id||p.item.portable!==false||
 !p.slots?.[p.backpackSlot as keyof typeof p.slots]||!exact(p.baseStats,['health'])||!Object.values(p.baseStats).every(n=>Number.isFinite(n)&&n>0)||
 !exact(p.reservedStats,['maxHealth','stamina','armor'])||!Object.values(p.reservedStats).every(v=>v===null)||!exact(p.keys,['movement','inventory'])||
 !exact(p.keys.movement,['left','right','up','down','run'])||!exact(p.keys.inventory,['toggle','close','pickup','rotate']))throw Error('invalid-player-config');
 const codes=[...Object.values(p.keys.movement),...Object.values(p.keys.inventory)];
 if(codes.some(code=>typeof code!=='string'||!/^(Key[A-Z]|Digit[0-9]|Arrow(Up|Down|Left|Right)|Shift(Left|Right)|Control(Left|Right)|Alt(Left|Right)|Tab|Escape|Space|Backspace|Enter)$/.test(code))||new Set(codes).size!==codes.length)throw Error('invalid-player-keys');
 return p;
}
