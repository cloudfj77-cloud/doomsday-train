export type Position = [number, number, number];
export interface ItemDefinition {
  id:string; name:string; description:string; tags:string[]; size:[number,number]; rotatable:boolean;
  maxStack:number; weight:number; value:number; portable:boolean; stats?:Record<string,number>;
  category?:string; usage?:string; unit?:string; valueQuantity?:number; weapon?:{ammo:string;capacity:number;pose:'pistol'|'smg'};
  slots?:Record<string,string[]>; container?:{columns:number;rows:number};
}
export interface ItemConfig { definitions:ItemDefinition[] }
export interface ItemWorldConfig {
  playerDefinition:string; backpackDefinition:string; backpackSlot:string; interactionDistance:number;
  dropRadius:number; dropClearance:number; dropHeight:number; drops:{definition:string;quantity:number;position:Position}[];
}
export type Location = {kind:'world';position:Position}|{kind:'actor'}|
  {kind:'slot';parent:string;slot:string}|{kind:'grid';parent:string;x:number;y:number;rotated:boolean};
export interface Modifier { stat:string; target:string; operation:'add'|'multiply'; value:number }
export interface ItemInstance { id:string; definition:string; quantity:number; loaded?:number; location:Location; variables:Record<string,string|number|boolean>; modifiers:Modifier[] }
export interface ItemState { items:Record<string,ItemInstance>; playerId:string; backpackId:string; backpackSlot:string; nextId:number; open:boolean; result:Outcome }
export type Outcome = {ok:true;code:'ok';amount?:number}|{ok:false;code:string;expected:string;hint:string;detail?:string};
export type ItemCommand = {type:'toggle'}|{type:'close'}|{type:'pickup';id:string}|{type:'move';id:string;parent?:string;x:number;y:number;rotated:boolean}|
  {type:'split';id:string;quantity:number;parent?:string}|{type:'drop';id:string;quantity:number}|{type:'equip';id:string}|{type:'unequip';id:string;parent?:string;x?:number;y?:number;rotated?:boolean}|{type:'load';id:string}|{type:'unload';id:string}|{type:'take';id:string;parent?:string;x?:number;y?:number;rotated?:boolean};
export interface Environment { player:Position; visible:(position:Position)=>boolean; dropPosition:()=>Position|undefined }
const integer=(v:unknown):v is number=>Number.isInteger(v)&&Number(v)>0;
const position=(v:unknown):v is Position=>Array.isArray(v)&&v.length===3&&v.every(Number.isFinite);
const strings=(v:unknown):v is string[]=>Array.isArray(v)&&v.length>0&&v.every(x=>typeof x==='string'&&x.trim().length>0)&&new Set(v).size===v.length;
export function validateItems(raw:unknown,worldRaw:unknown):{config:ItemConfig;world:ItemWorldConfig} {
  const c=raw as ItemConfig,w=worldRaw as ItemWorldConfig;
  if(!c||!Array.isArray(c.definitions)||!c.definitions.length)throw Error('invalid-items-config');
  const ids=new Set<string>();
  for(const d of c.definitions) {
    if(!d||typeof d.id!=='string'||!d.id.trim()||ids.has(d.id)||!d.name?.trim()||typeof d.description!=='string'||!strings(d.tags)||
      !Array.isArray(d.size)||d.size.length!==2||!d.size.every(integer)||typeof d.rotatable!=='boolean'||typeof d.portable!=='boolean'||
      !integer(d.maxStack)||!Number.isFinite(d.weight)||d.weight<0||!Number.isFinite(d.value)||d.value<0||
      (d.stats&&Object.entries(d.stats).some(([k,v])=>!k.trim()||!Number.isFinite(v)))||
      (d.container&&(!integer(d.container.columns)||!integer(d.container.rows)))||
      (d.slots&&Object.entries(d.slots).some(([k,tags])=>!k.trim()||!strings(tags)))||((d.slots||d.container)&&d.maxStack!==1))throw Error('invalid-item-definition: '+d?.id);
    if(d.valueQuantity!==undefined&&(!integer(d.valueQuantity)||!d.unit?.trim()))throw Error('invalid-value-unit');
    if(d.weapon&&(!integer(d.weapon.capacity)||!['pistol','smg'].includes(d.weapon.pose)||d.maxStack!==1||!d.tags.includes('weapon')))throw Error('invalid-weapon');
    ids.add(d.id);
  }
  const defs=Object.fromEntries(c.definitions.map(d=>[d.id,d]));
  for(const d of c.definitions)if(d.weapon&&(!defs[d.weapon.ammo]?.tags.includes('ammo')||defs[d.weapon.ammo].weapon))throw Error('invalid-ammo-reference');
  if(!w||!defs[w.playerDefinition]?.container||!defs[w.backpackDefinition]?.container||
    !defs[w.playerDefinition].slots?.[w.backpackSlot]?.some(t=>defs[w.backpackDefinition].tags.includes(t))||
    ![w.interactionDistance,w.dropRadius,w.dropClearance,w.dropHeight].every(n=>Number.isFinite(n)&&n>0)||!Array.isArray(w.drops)||
    w.drops.some(d=>!defs[d.definition]?.portable||!integer(d.quantity)||!position(d.position)))throw Error('invalid-item-world-config');
  return {config:c,world:w};
}
export function dimensions(d:ItemDefinition,rotated:boolean):[number,number] {return rotated?[d.size[1],d.size[0]]:[...d.size];}
export function createItems(c:ItemConfig,w:ItemWorldConfig):ItemState {
  validateItems(c,w);
  const s:ItemState={items:{},playerId:'player',backpackId:'player-backpack',backpackSlot:w.backpackSlot,nextId:1,open:false,result:{ok:true,code:'ok'}};
  const make=(id:string,definition:string,quantity:number,location:Location):ItemInstance=>({id,definition,quantity,location,variables:{},modifiers:[],...(c.definitions.find(d=>d.id===definition)?.weapon?{loaded:0}:{})});
  s.items.player=make('player',w.playerDefinition,1,{kind:'actor'});
  s.items[s.backpackId]=make(s.backpackId,w.backpackDefinition,1,{kind:'slot',parent:s.playerId,slot:w.backpackSlot});
  for(const d of w.drops){const id=`item-${s.nextId++}`;s.items[id]=make(id,d.definition,d.quantity,{kind:'world',position:[...d.position]});}
  assertItemState(s,c);return s;
}
export function canPlace(s:ItemState,c:ItemConfig,id:string,parent:string,x:number,y:number,rotated:boolean):boolean {
  const item=s.items[id],p=s.items[parent],d=c.definitions.find(d=>d.id===item?.definition),grid=c.definitions.find(d=>d.id===p?.definition)?.container;
  if(!item||!d?.portable||!grid||id===parent||!Number.isInteger(x)||!Number.isInteger(y)||typeof rotated!=='boolean'||(rotated&&!d.rotatable))return false;
  const ancestors=new Set<string>([id]);let cursor=p;
  while(cursor){if(ancestors.has(cursor.id))return false;ancestors.add(cursor.id);const l=cursor.location;if(l.kind!=='grid'&&l.kind!=='slot')break;cursor=s.items[l.parent];}
  const [width,height]=dimensions(d,rotated);
  if(x<0||y<0||x+width>grid.columns||y+height>grid.rows)return false;
  return Object.values(s.items).every(other=>{
    const l=other.location;if(other.id===id||l.kind!=='grid'||l.parent!==parent)return true;
    const [ow,oh]=dimensions(c.definitions.find(d=>d.id===other.definition)!,l.rotated);
    return x+width<=l.x||l.x+ow<=x||y+height<=l.y||l.y+oh<=y;
  });
}
export function findSpace(s:ItemState,c:ItemConfig,id:string,parent:string=s.backpackId):Extract<Location,{kind:'grid'}>|undefined {
  const grid=c.definitions.find(d=>d.id===s.items[parent].definition)!.container!;
  const d=c.definitions.find(d=>d.id===s.items[id].definition)!;
  for(const rotated of d.rotatable?[false,true]:[false])for(let y=0;y<grid.rows;y++)for(let x=0;x<grid.columns;x++)
    if(canPlace(s,c,id,parent,x,y,rotated))return {kind:'grid',parent,x,y,rotated};
}
export function assertItemState(s:ItemState,c:ItemConfig):void {
  const defs=Object.fromEntries(c.definitions.map(d=>[d.id,d]));
  if(!s.items[s.playerId]||!s.items[s.backpackId])throw Error('missing-item-roots');
  if(s.backpackId!==(equippedBackpack(s)??s.playerId))throw Error('invalid-active-container');
  const slots=new Set<string>();
  for(const [key,item] of Object.entries(s.items)) {
    const d=defs[item.definition],l=item.location;
    if(key!==item.id||!d||!integer(item.quantity)||!item.variables||!Array.isArray(item.modifiers)||
      Object.values(item.variables).some(v=>!['string','number','boolean'].includes(typeof v)||(typeof v==='number'&&!Number.isFinite(v)))||
      item.modifiers.some(m=>!s.items[m.target]||!defs[s.items[m.target].definition]?.stats||!(m.stat in defs[s.items[m.target].definition].stats!)||!['add','multiply'].includes(m.operation)||!Number.isFinite(m.value)))throw Error('invalid-item-instance');
    if(d.weapon?(item.quantity!==1||!Number.isInteger(item.loaded)||item.loaded!<0||item.loaded!>d.weapon.capacity):item.loaded!==undefined)throw Error('invalid-loaded-ammo');
    if(l.kind!=='world'&&item.quantity>d.maxStack)throw Error('invalid-stack');
    if(l.kind==='world'&&(!d.portable||!position(l.position)))throw Error('invalid-world-location');
    if(l.kind==='actor'&&item.id!==s.playerId)throw Error('invalid-actor');
    if(!['world','actor','grid','slot'].includes(l.kind))throw Error('invalid-item-location');
    const seen=new Set([item.id]);let cursor=item;
    while(cursor.location.kind==='slot'||cursor.location.kind==='grid') {
      const parent=s.items[cursor.location.parent];if(!parent||seen.has(parent.id))throw Error('item-tree-cycle-or-missing-parent');
      seen.add(parent.id);cursor=parent;
    }
    if(l.kind==='grid'&&!canPlace(s,c,item.id,l.parent,l.x,l.y,l.rotated))throw Error('invalid-grid-placement');
    if(l.kind==='slot') {
      const accept=defs[s.items[l.parent].definition].slots?.[l.slot],key=l.parent+':'+l.slot;
      if(!accept?.some(tag=>d.tags.includes(tag))||slots.has(key)||item.quantity!==1)throw Error('invalid-slot');slots.add(key);
    }
  }
}
export function stat(s:ItemState,c:ItemConfig,id:string,key:string):number {
  const base=c.definitions.find(d=>d.id===s.items[id]?.definition)?.stats?.[key];
  if(base===undefined)throw Error('unknown-item-stat');
  const modifiers=Object.values(s.items).flatMap(i=>i.modifiers).filter(m=>m.target===id&&m.stat===key);
  return (base+modifiers.filter(m=>m.operation==='add').reduce((n,m)=>n+m.value,0))*modifiers.filter(m=>m.operation==='multiply').reduce((n,m)=>n*m.value,1);
}
const compatible=(s:ItemState,a:ItemInstance,b:ItemInstance)=>a.id!==b.id&&a.definition===b.definition&&
  [a,b].every(i=>!Object.keys(i.variables).length&&!i.modifiers.length&&!Object.values(s.items).some(j=>('parent' in j.location)&&j.location.parent===i.id));
export function nearbyItems(s:ItemState,c:ItemConfig,w:ItemWorldConfig,e:Environment):string[] {
  return Object.values(s.items).filter(i=>i.location.kind==='world'&&c.definitions.find(d=>d.id===i.definition)?.portable)
    .map(i=>({i,p:(i.location as Extract<Location,{kind:'world'}>).position}))
    .map(({i,p})=>({id:i.id,p,distance:Math.hypot(...p.map((v,k)=>v-e.player[k]))}))
    .filter(v=>v.distance<=w.interactionDistance&&e.visible(v.p)).sort((a,b)=>a.distance-b.distance||a.id.localeCompare(b.id)).map(v=>v.id);
}
export function nearestItem(s:ItemState,c:ItemConfig,w:ItemWorldConfig,e:Environment){return nearbyItems(s,c,w,e)[0];}
export function equippedBackpack(s:ItemState):string|undefined {
 return Object.values(s.items).find(i=>i.location.kind==='slot'&&i.location.parent===s.playerId&&i.location.slot===s.backpackSlot)?.id;
}
export function carriedByPlayer(s:ItemState,id:string):boolean {
 const seen=new Set<string>();let item=s.items[id];
 while(item){if(seen.has(item.id))return false;seen.add(item.id);if(item.id===s.playerId)return true;const l=item.location;if(l.kind!=='grid'&&l.kind!=='slot')return false;item=s.items[l.parent];}return false;
}
export function carriedContainers(s:ItemState):string[]{const bag=equippedBackpack(s);return bag?[bag,s.playerId]:[s.playerId];}
function findCarrySpace(s:ItemState,c:ItemConfig,id:string){for(const parent of carriedContainers(s)){const place=findSpace(s,c,id,parent);if(place)return place;}}
export function commandItems(s:ItemState,c:ItemConfig,w:ItemWorldConfig,cmd:ItemCommand,e:Environment):Outcome {
  const fail=(code:string,hint:string):Outcome=>s.result={ok:false,code,expected:'合法的物品操作',hint};
  const draft:ItemState=structuredClone(s);
  if(cmd.type==='toggle'||cmd.type==='close') {s.open=cmd.type==='toggle'?!s.open:false;return s.result={ok:true,code:'ok'};}
  if(!['pickup','move','split','drop','equip','unequip','load','unload','take'].includes(cmd.type))return fail('invalid-command','未知操作');
  const item=draft.items[cmd.id];if(!item)return fail('missing-item','物品已不存在');
  const d=c.definitions.find(d=>d.id===item.definition)!;let amount=0;
  const newInstance=(quantity:number)=>{const copy=structuredClone(item);copy.id=`item-${draft.nextId++}`;copy.quantity=quantity;draft.items[copy.id]=copy;return copy;};
  const accessible=(parent:string)=>carriedByPlayer(draft,parent)||nearbyItems(draft,c,w,e).includes(parent);
  const finish=()=>{draft.backpackId=equippedBackpack(draft)??draft.playerId;assertItemState(draft,c);Object.assign(s,draft);return s.result={ok:true,code:'ok',amount} as Outcome;};
  if((cmd.type==='equip'||cmd.type==='unequip')&&d.tags.includes('backpack')&&d.container){
    if(!s.open)return fail('inventory-closed','请打开背包操作装备');
    const wearing=item.location.kind==='slot'&&item.location.parent===s.playerId&&item.location.slot===s.backpackSlot;
    if(cmd.type==='equip'){
      if(wearing)return fail('already-equipped','背包已装备');
      if(!carriedByPlayer(draft,item.id)&&!(item.location.kind==='world'&&nearbyItems(draft,c,w,e).includes(item.id)))return fail('not-interactable','背包不在可装备范围');
      const oldId=equippedBackpack(draft);item.location={kind:'slot',parent:s.playerId,slot:s.backpackSlot};
      if(oldId){const old=draft.items[oldId],place=findSpace(draft,c,old.id,s.playerId);if(place)old.location=place;else{const p=e.dropPosition();if(!p||!position(p))return fail('no-drop-position','脚边没有安全位置放下原背包');old.location={kind:'world',position:[...p]};}}
    }else{
      if(!wearing)return fail('not-equipped','背包尚未装备');
      const parent=cmd.parent??s.playerId;
      if(!c.definitions.find(d=>d.id===draft.items[parent]?.definition)?.container||!accessible(parent))return fail('invalid-container','目标容器不可用');
      const place=cmd.x!==undefined?(canPlace(draft,c,item.id,parent,cmd.x,cmd.y!,cmd.rotated!)?{kind:'grid' as const,parent,x:cmd.x,y:cmd.y!,rotated:cmd.rotated!}:undefined):findSpace(draft,c,item.id,parent);
      if(place)item.location=place;
      else if(cmd.parent||cmd.x!==undefined)return fail('no-space','目标格子放不下背包');
      else{const p=e.dropPosition();if(!p||!position(p))return fail('no-drop-position','脚边没有安全的背包落点');item.location={kind:'world',position:[...p]};}
    }
    return finish();
  }
  if(cmd.type==='equip'||cmd.type==='unequip'||cmd.type==='load'||cmd.type==='unload') {
    const held=item.location.kind==='grid'&&carriedByPlayer(draft,item.id);
    const equipped=item.location.kind==='slot'&&item.location.parent===s.playerId&&item.location.slot==='primary';
    if(!s.open||!d.weapon||(!held&&!equipped))return fail('not-owned-weapon','请从背包或主武器槽选择枪械');
    if(cmd.type==='equip') {
      if(!held)return fail('already-equipped','武器已装备');
      const old=Object.values(draft.items).find(i=>i.location.kind==='slot'&&i.location.parent===s.playerId&&i.location.slot==='primary');
      item.location={kind:'slot',parent:s.playerId,slot:'primary'};
      if(old){const place=findCarrySpace(draft,c,old.id);if(!place)return fail('no-space','背包空间不足，无法放回原武器');old.location=place;}
    }else if(cmd.type==='unequip') {
      if(!equipped)return fail('not-equipped','武器尚未装备');
      if(cmd.parent&&!carriedContainers(draft).includes(cmd.parent))return fail('invalid-container','目标不是可用携带容器');
      const place=cmd.parent?(cmd.x===undefined?findSpace(draft,c,item.id,cmd.parent):(canPlace(draft,c,item.id,cmd.parent,cmd.x,cmd.y!,cmd.rotated!)?{kind:'grid' as const,parent:cmd.parent,x:cmd.x,y:cmd.y!,rotated:cmd.rotated!}:undefined)):findCarrySpace(draft,c,item.id);if(!place)return fail('no-space','背包空间不足，无法卸下武器');item.location=place;
    }else if(cmd.type==='load') {
      let need=d.weapon.capacity-item.loaded!;
      if(!need)return fail('magazine-full','弹匣已满');
      for(const ammo of Object.values(draft.items).sort((a,b)=>a.id.localeCompare(b.id)))if(ammo.definition===d.weapon.ammo&&ammo.location.kind==='grid'&&carriedContainers(draft).includes(ammo.location.parent)) {
        const n=Math.min(need,ammo.quantity);ammo.quantity-=n;item.loaded!+=n;need-=n;amount+=n;if(!ammo.quantity)delete draft.items[ammo.id];if(!need)break;
      }
      if(!amount)return fail('no-ammo','背包没有适配的通用弹药');
    }else {
      amount=item.loaded!;if(!amount)return fail('magazine-empty','枪内没有弹药');
      const ad=c.definitions.find(v=>v.id===d.weapon!.ammo)!;let remaining=amount;
      for(const ammo of Object.values(draft.items).sort((a,b)=>a.id.localeCompare(b.id)))if(ammo.definition===ad.id&&ammo.location.kind==='grid'&&carriedContainers(draft).includes(ammo.location.parent)){const n=Math.min(remaining,ad.maxStack-ammo.quantity);ammo.quantity+=n;remaining-=n;}
      while(remaining){const id='item-'+draft.nextId++;const ammo:ItemInstance={id,definition:ad.id,quantity:Math.min(remaining,ad.maxStack),variables:{},modifiers:[],location:{kind:'world',position:[...e.player]}};draft.items[id]=ammo;const place=findCarrySpace(draft,c,id);if(!place)return fail('no-space','背包空间不足，无法卸弹');ammo.location=place;remaining-=ammo.quantity;}
      item.loaded=0;
    }
  }else if(cmd.type==='pickup'||cmd.type==='take') {
    if(item.location.kind!=='world'||(cmd.type==='pickup'?(s.open||nearestItem(s,c,w,e)!==item.id):(!s.open||!nearbyItems(s,c,w,e).includes(item.id))))return fail('not-interactable','物品不在可拾取范围或操作入口不可用');
    const destination=cmd.type==='take'?cmd.parent:undefined;
    if(destination&&(!carriedContainers(draft).includes(destination)))return fail('invalid-container','目标不是随身格或已装备背包');
    const pickupParent=destination??draft.backpackId;
    if(cmd.type==='take'&&cmd.x!==undefined){
      const target=Object.values(draft.items).find(i=>{const l=i.location;if(l.kind!=='grid'||l.parent!==pickupParent)return false;const [width,height]=dimensions(c.definitions.find(d=>d.id===i.definition)!,l.rotated);return cmd.x!>=l.x&&cmd.x!<l.x+width&&cmd.y!>=l.y&&cmd.y!<l.y+height;});
      if(target&&compatible(draft,item,target)){
        const n=Math.min(item.quantity,d.maxStack-target.quantity);if(!n)return fail('stack-full','目标堆叠已满');target.quantity+=n;item.quantity-=n;if(!item.quantity)delete draft.items[item.id];amount=n;
      }else{
        if(!canPlace(draft,c,item.id,pickupParent,cmd.x,cmd.y!,cmd.rotated!))return fail('invalid-placement','此处不能放置');
        const moved=item.quantity>d.maxStack?newInstance(d.maxStack):item;if(moved!==item)item.quantity-=moved.quantity;
        moved.location={kind:'grid',parent:pickupParent,x:cmd.x,y:cmd.y!,rotated:cmd.rotated!};amount=moved.quantity;
      }
      return finish();
    }
    const before=item.quantity;
    for(const target of Object.values(draft.items))if(target.location.kind==='grid'&&(destination?target.location.parent===destination:carriedContainers(draft).includes(target.location.parent))&&compatible(draft,item,target)) {
      const n=Math.min(item.quantity,d.maxStack-target.quantity);target.quantity+=n;item.quantity-=n;
    }
    while(item.quantity>0) {
      const place=destination?findSpace(draft,c,item.id,destination):findCarrySpace(draft,c,item.id);if(!place)break;
      if(item.quantity<=d.maxStack){item.location=place;amount=before;break;}
      const copy=newInstance(d.maxStack);copy.location=place;item.quantity-=d.maxStack;
    }
    if(item.quantity===0)delete draft.items[item.id];
    if(item.location.kind==='world')amount=before-item.quantity;
    if(!amount)return fail('no-space','背包空间不足');
  } else {
    if(!s.open||!((item.location.kind==='grid'&&accessible(item.location.parent))||(cmd.type==='drop'&&item.location.kind==='slot'&&item.location.slot===s.backpackSlot&&item.location.parent===s.playerId)))return fail('not-in-backpack','请打开背包选择物品');
    const sourceParent=(item.location as {parent:string}).parent;
    if((cmd.type==='move'||cmd.type==='split')&&cmd.parent&&(!c.definitions.find(d=>d.id===draft.items[cmd.parent!]?.definition)?.container||!accessible(cmd.parent)))return fail('invalid-container','目标不是可用容器，或不允许嵌套容器');
    if(cmd.type==='move') {
      const target=Object.values(draft.items).find(i=>{
        const l=i.location;if(i.id===item.id||l.kind!=='grid'||l.parent!==(cmd.parent??sourceParent))return false;
        const [width,height]=dimensions(c.definitions.find(d=>d.id===i.definition)!,l.rotated);
        return cmd.x>=l.x&&cmd.y>=l.y&&cmd.x<l.x+width&&cmd.y<l.y+height;
      });
      if(!Number.isInteger(cmd.x)||!Number.isInteger(cmd.y)||typeof cmd.rotated!=='boolean')return fail('invalid-placement','无效位置');
      if(target&&compatible(draft,item,target)) {
        amount=Math.min(item.quantity,d.maxStack-target.quantity);if(!amount)return fail('stack-full','目标堆叠已满');
        target.quantity+=amount;item.quantity-=amount;if(!item.quantity)delete draft.items[item.id];
      }else {
        if(!canPlace(draft,c,item.id,cmd.parent??sourceParent,cmd.x,cmd.y,cmd.rotated))return fail('invalid-placement','此处不能放置');
        item.location={kind:'grid',parent:cmd.parent??sourceParent,x:cmd.x,y:cmd.y,rotated:cmd.rotated};
      }
    } else {
      if(!integer(cmd.quantity)||cmd.quantity>item.quantity)return fail('invalid-quantity','数量必须是有效整数');
      if(cmd.type==='split') {
        if(cmd.quantity===item.quantity||d.maxStack===1||Object.keys(item.variables).length||item.modifiers.length)return fail('invalid-split','无法拆分此物品');
        const copy=newInstance(cmd.quantity),place=findSpace(draft,c,copy.id,cmd.parent??sourceParent);
        if(!place)return fail('no-space','拆分需要额外空间');copy.location=place;item.quantity-=cmd.quantity;
      }else {
        const p=e.dropPosition();if(!p||!position(p))return fail('no-drop-position','脚边没有安全的丢弃位置');
        const drop=cmd.quantity===item.quantity?item:newInstance(cmd.quantity);
        if(drop!==item)item.quantity-=cmd.quantity;drop.location={kind:'world',position:[...p]};
      }
      amount=cmd.quantity;
    }
  }
  assertItemState(draft,c);Object.assign(s,draft);return s.result={ok:true,code:'ok',amount};
}
export function projectItems(s:ItemState,c:ItemConfig,target?:string,nearby:string[]=[]) {
  const bag=c.definitions.find(d=>d.id===s.items[s.backpackId].definition)!.container!;
  const equipped=Object.values(s.items).find(i=>i.location.kind==='slot'&&i.location.parent===s.playerId&&i.location.slot==='primary');
  const carried=Object.values(s.items).filter(i=>i.id!==s.playerId&&carriedByPlayer(s,i.id));
  const totalWeight=carried.reduce((n,i)=>{const d=c.definitions.find(d=>d.id===i.definition)!;return n+d.weight*i.quantity+(d.weapon?(i.loaded??0)*c.definitions.find(a=>a.id===d.weapon!.ammo)!.weight:0);},0);
  const value=structuredClone({...s,grid:bag,pocketsId:s.playerId,equippedBackpack:equippedBackpack(s)??null,containers:[projectContainer(s,c,s.playerId),...(equippedBackpack(s)?[projectContainer(s,c,equippedBackpack(s)!)]:[])],equipped:equipped?.id??null,nearby,weight:totalWeight,target:target&&s.items[target]?.location.kind==='world'?target:null});
  const freeze=(v:unknown):void=>{if(v&&typeof v==='object'){Object.values(v).forEach(freeze);Object.freeze(v);}};freeze(value);return value;
}
export type ItemProjection=ReturnType<typeof projectItems>;

export function projectContainer(s:ItemState,c:ItemConfig,parent:string) {
 const item=s.items[parent],grid=c.definitions.find(d=>d.id===item?.definition)?.container;
 if(!grid)throw Error('not-a-container');
 return {id:parent,...grid,items:Object.values(s.items).filter(i=>i.location.kind==='grid'&&i.location.parent===parent).map(i=>structuredClone(i))};
}
