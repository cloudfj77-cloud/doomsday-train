export type Position = [number, number, number];
export interface ItemDefinition {
  id:string; name:string; description:string; tags:string[]; size:[number,number]; rotatable:boolean;
  maxStack:number; weight:number; value:number; portable:boolean; stats?:Record<string,number>;
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
export interface ItemInstance { id:string; definition:string; quantity:number; location:Location; variables:Record<string,string|number|boolean>; modifiers:Modifier[] }
export interface ItemState { items:Record<string,ItemInstance>; playerId:string; backpackId:string; nextId:number; open:boolean; result:Outcome }
export type Outcome = {ok:true;code:'ok';amount?:number}|{ok:false;code:string;expected:string;hint:string;detail?:string};
export type ItemCommand = {type:'toggle'}|{type:'close'}|{type:'pickup';id:string}|{type:'move';id:string;parent?:string;x:number;y:number;rotated:boolean}|
  {type:'split';id:string;quantity:number;parent?:string}|{type:'drop';id:string;quantity:number};
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
    ids.add(d.id);
  }
  const defs=Object.fromEntries(c.definitions.map(d=>[d.id,d]));
  if(!w||!defs[w.playerDefinition]||!defs[w.backpackDefinition]?.container||
    !defs[w.playerDefinition].slots?.[w.backpackSlot]?.some(t=>defs[w.backpackDefinition].tags.includes(t))||
    ![w.interactionDistance,w.dropRadius,w.dropClearance,w.dropHeight].every(n=>Number.isFinite(n)&&n>0)||!Array.isArray(w.drops)||
    w.drops.some(d=>!defs[d.definition]?.portable||!integer(d.quantity)||!position(d.position)))throw Error('invalid-item-world-config');
  return {config:c,world:w};
}
export function dimensions(d:ItemDefinition,rotated:boolean):[number,number] {return rotated?[d.size[1],d.size[0]]:[...d.size];}
export function createItems(c:ItemConfig,w:ItemWorldConfig):ItemState {
  validateItems(c,w);
  const s:ItemState={items:{},playerId:'player',backpackId:'player-backpack',nextId:1,open:false,result:{ok:true,code:'ok'}};
  const make=(id:string,definition:string,quantity:number,location:Location):ItemInstance=>({id,definition,quantity,location,variables:{},modifiers:[]});
  s.items.player=make('player',w.playerDefinition,1,{kind:'actor'});
  s.items[s.backpackId]=make(s.backpackId,w.backpackDefinition,1,{kind:'slot',parent:s.playerId,slot:w.backpackSlot});
  for(const d of w.drops){const id=`item-${s.nextId++}`;s.items[id]=make(id,d.definition,d.quantity,{kind:'world',position:[...d.position]});}
  assertItemState(s,c);return s;
}
export function canPlace(s:ItemState,c:ItemConfig,id:string,parent:string,x:number,y:number,rotated:boolean):boolean {
  const item=s.items[id],p=s.items[parent],d=c.definitions.find(d=>d.id===item?.definition),grid=c.definitions.find(d=>d.id===p?.definition)?.container;
  if(!item||!d?.portable||!grid||id===parent||!Number.isInteger(x)||!Number.isInteger(y)||typeof rotated!=='boolean'||(rotated&&!d.rotatable))return false;
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
  const slots=new Set<string>();
  for(const [key,item] of Object.entries(s.items)) {
    const d=defs[item.definition],l=item.location;
    if(key!==item.id||!d||!integer(item.quantity)||!item.variables||!Array.isArray(item.modifiers)||
      Object.values(item.variables).some(v=>!['string','number','boolean'].includes(typeof v)||(typeof v==='number'&&!Number.isFinite(v)))||
      item.modifiers.some(m=>!s.items[m.target]||!defs[s.items[m.target].definition]?.stats||!(m.stat in defs[s.items[m.target].definition].stats!)||!['add','multiply'].includes(m.operation)||!Number.isFinite(m.value)))throw Error('invalid-item-instance');
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
export function nearestItem(s:ItemState,c:ItemConfig,w:ItemWorldConfig,e:Environment):string|undefined {
  return Object.values(s.items).filter(i=>i.location.kind==='world'&&c.definitions.find(d=>d.id===i.definition)?.portable)
    .map(i=>({i,p:(i.location as Extract<Location,{kind:'world'}>).position}))
    .map(({i,p})=>({id:i.id,p,distance:Math.hypot(...p.map((v,k)=>v-e.player[k]))}))
    .filter(v=>v.distance<=w.interactionDistance&&e.visible(v.p)).sort((a,b)=>a.distance-b.distance||a.id.localeCompare(b.id))[0]?.id;
}
export function commandItems(s:ItemState,c:ItemConfig,w:ItemWorldConfig,cmd:ItemCommand,e:Environment):Outcome {
  const fail=(code:string,hint:string):Outcome=>s.result={ok:false,code,expected:'合法的物品操作',hint};
  const draft:ItemState=structuredClone(s);
  if(cmd.type==='toggle'||cmd.type==='close') {s.open=cmd.type==='toggle'?!s.open:false;return s.result={ok:true,code:'ok'};}
  if(!['pickup','move','split','drop'].includes(cmd.type))return fail('invalid-command','未知操作');
  const item=draft.items[cmd.id];if(!item)return fail('missing-item','物品已不存在');
  const d=c.definitions.find(d=>d.id===item.definition)!;let amount=0;
  const newInstance=(quantity:number)=>{const copy=structuredClone(item);copy.id=`item-${draft.nextId++}`;copy.quantity=quantity;draft.items[copy.id]=copy;return copy;};
  if(cmd.type==='pickup') {
    if(s.open||nearestItem(s,c,w,e)!==item.id)return fail('not-interactable','请靠近物品并关闭背包');
    const before=item.quantity;
    for(const target of Object.values(draft.items))if(target.location.kind==='grid'&&target.location.parent===draft.backpackId&&compatible(draft,item,target)) {
      const n=Math.min(item.quantity,d.maxStack-target.quantity);target.quantity+=n;item.quantity-=n;
    }
    while(item.quantity>0) {
      const place=findSpace(draft,c,item.id);if(!place)break;
      if(item.quantity<=d.maxStack){item.location=place;amount=before;break;}
      const copy=newInstance(d.maxStack);copy.location=place;item.quantity-=d.maxStack;
    }
    if(item.quantity===0)delete draft.items[item.id];
    if(item.location.kind==='world')amount=before-item.quantity;
    if(!amount)return fail('no-space','背包空间不足');
  } else {
    if(!s.open||item.location.kind!=='grid'||!s.items[item.location.parent])return fail('not-in-backpack','请打开背包选择物品');
    const sourceParent=item.location.parent;
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
export function projectItems(s:ItemState,c:ItemConfig,target?:string) {
  const bag=c.definitions.find(d=>d.id===s.items[s.backpackId].definition)!.container!;
  const weight=Object.values(s.items).filter(i=>i.location.kind==='grid'&&i.location.parent===s.backpackId)
    .reduce((n,i)=>n+c.definitions.find(d=>d.id===i.definition)!.weight*i.quantity,0);
  const value=structuredClone({...s,grid:bag,weight,target:target&&s.items[target]?.location.kind==='world'?target:null});
  const freeze=(v:unknown):void=>{if(v&&typeof v==='object'){Object.values(v).forEach(freeze);Object.freeze(v);}};freeze(value);return value;
}
export type ItemProjection=ReturnType<typeof projectItems>;

export function projectContainer(s:ItemState,c:ItemConfig,parent:string) {
 const item=s.items[parent],grid=c.definitions.find(d=>d.id===item?.definition)?.container;
 if(!grid)throw Error('not-a-container');
 return {id:parent,...grid,items:Object.values(s.items).filter(i=>i.location.kind==='grid'&&i.location.parent===parent).map(i=>structuredClone(i))};
}
