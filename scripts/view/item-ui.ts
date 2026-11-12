import { characterPreview } from './actor-preview.ts';
import { mountGrid, nearbyLayout, type GridPlacement } from './inventory-grid.ts';
import { canPlace, dimensions, type ItemConfig, type ItemCommand, type ItemProjection } from '../logic/items.ts';
import { itemKeyCommand, type ItemStyle } from './item-presentation.ts';
export function mountInventory(parent:HTMLElement,config:ItemConfig,style:ItemStyle,send:(cmd:ItemCommand)=>void) {
  const doc=parent.ownerDocument;
  const root=doc.createElement('div');root.id='item-ui';parent.append(root);
  const css=doc.createElement('style');css.textContent=`
  #item-ui{position:fixed;inset:0;z-index:100;pointer-events:none;font:13px/1.45 system-ui,sans-serif;color:${style.colors.text}}
  #item-ui *{box-sizing:border-box} #item-ui button,#item-ui input{font:inherit;color:inherit}
  #item-ui button{cursor:pointer;border:1px solid ${style.colors.line};background:#243332;padding:8px 12px}
  #item-ui button:focus-visible,#item-ui input:focus-visible{outline:2px solid ${style.colors.accent}}
  #item-ui .item-hud{position:absolute;left:24px;bottom:24px;background:#152022eb;border:1px solid ${style.colors.line};padding:10px 16px}
  #item-ui .inventory-shell{position:absolute;inset:0;background:#071011c9;display:flex;align-items:center;justify-content:center;pointer-events:auto}
  #item-ui .inventory-window{width:min(1240px,98vw);max-height:96vh;overflow:auto;background:linear-gradient(115deg,#1b292b,#131f22);border:1px solid #53635f;box-shadow:0 20px 80px #0009;padding:18px 20px}
  #item-ui .inventory-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:18px;padding-bottom:12px;border-bottom:1px solid #40504b}
  #item-ui h2{margin:0;font-size:20px;font-weight:500;letter-spacing:5px} #item-ui h3{margin:0 0 12px;font-size:13px;font-weight:500;letter-spacing:2px;color:#bdc8be}
  #item-ui .inventory-body{display:grid;grid-template-columns:minmax(250px,1fr) 444px minmax(222px,.8fr);gap:18px;align-items:start}
  #item-ui .character-section{min-width:0} #item-ui .character-loadout{display:grid;grid-template-columns:minmax(120px,1fr) 78px;gap:12px;align-items:center;min-height:360px}
  #item-ui .character-stage{height:min(430px,calc(100vh - 220px));min-height:320px;background:radial-gradient(ellipse at 50% 45%,#34413b70,transparent 70%);display:flex;align-items:center;justify-content:center}
  #item-ui .character-model{width:100%;height:100%;filter:drop-shadow(0 14px 10px #0006)}
  #item-ui .equipment{display:flex;flex-direction:column;gap:12px} #item-ui .slot{width:78px;height:78px;padding:5px;display:flex;align-items:center;justify-content:center;position:relative;background:#0f1a1d;border:1px solid #4b5d57;touch-action:none}
  #item-ui .slot[data-slot=primary]{height:62px} #item-ui .slot[aria-disabled=true]{border-style:dashed;opacity:.5;cursor:default}
  #item-ui .slot-outline{width:40px;height:40px;fill:none;stroke:#71877e;stroke-width:1.5;stroke-linecap:round;stroke-linejoin:round}
  #item-ui .slot-lock{position:absolute;right:4px;bottom:3px;width:11px;height:12px;fill:none;stroke:#819287;stroke-width:1.5}
  #item-ui .grid-wrap{overflow:auto;max-width:100%;padding:1px} #item-ui .inventory-grid{position:relative;background-color:${style.colors.grid};background-image:linear-gradient(to right,${style.colors.line} 1px,transparent 1px),linear-gradient(to bottom,${style.colors.line} 1px,transparent 1px);border-right:1px solid ${style.colors.line};border-bottom:1px solid ${style.colors.line};touch-action:none}
  #item-ui .inventory-item{position:absolute;display:flex;align-items:center;justify-content:center;padding:3px;user-select:none;touch-action:none;border-radius:2px;overflow:hidden;background:#35443e}
  #item-ui .inventory-item.selected,#item-ui .slot.selected{outline:2px solid ${style.colors.accent};outline-offset:-3px}
  #item-ui .quantity{position:absolute;right:4px;bottom:2px;background:#142020dd;padding:0 3px;font-size:11px;letter-spacing:0}
  #item-ui .item-icon{width:88%;height:88%;max-width:120px;max-height:94px;image-rendering:pixelated}
  #item-ui .nearby-wrap{max-height:calc(100vh - 190px);overflow:auto;padding:1px}
  #item-ui .inventory-footer{margin-top:14px;display:flex;justify-content:space-between;gap:12px;font-size:11px;color:#92a69c}
  #item-ui .placement{position:absolute;pointer-events:none;border:2px solid;z-index:4;opacity:.65}
  #item-ui .item-tooltip,#item-ui .item-context{position:fixed;z-index:110;background:#111c20f5;border:1px solid #738779;box-shadow:0 12px 36px #0009;max-height:calc(100vh - 24px);overflow:auto}
  #item-ui .item-tooltip{width:260px;padding:14px;pointer-events:none} #item-ui .item-tooltip h4{margin:4px 0 10px;font-size:17px;font-weight:500;color:#f0cf91}
  #item-ui .item-tooltip p{font-size:12px;line-height:1.5;margin:6px 0;color:#bcc8bc} #item-ui .item-tooltip .tooltip-meta{font-size:10px;color:#94aaa0;letter-spacing:1px}
  #item-ui .item-context{width:168px;padding:4px;pointer-events:auto} #item-ui .item-context button{display:block;width:100%;border:0;background:transparent;text-align:left;padding:9px 12px}
  #item-ui .item-context button:hover,#item-ui .item-context button:focus-visible{background:#394a41;color:#f1cf91} #item-ui button:disabled{opacity:.45;cursor:default}
  #item-ui .quantity-overlay{position:absolute;inset:0;background:#071011ba;display:flex;align-items:center;justify-content:center;pointer-events:auto;z-index:120}
  #item-ui .quantity-dialog{width:300px;padding:22px;border:1px solid ${style.colors.line};background:${style.colors.panel}} #item-ui input{width:100%;margin:12px 0;padding:8px;background:${style.colors.grid};border:1px solid ${style.colors.line}}
  #item-ui .notice{color:${style.colors.accent};font-size:12px} #item-ui .notice:not(:empty){margin-top:10px}
  @media(max-width:1080px){#item-ui .inventory-body{grid-template-columns:240px 444px 222px;gap:12px}#item-ui .inventory-window{overflow:auto}}
  `;
  doc.head.append(css);
  let state:ItemProjection|undefined,selected:string|undefined,last='',modal:{type:'split'|'drop';id:string;max:number}|undefined;
  let hover:{id?:string;label?:string;x:number;y:number}|undefined,menu:{id:string;x:number;y:number}|undefined;
  let drag:{id:string;rotated:boolean;x:number;y:number;active:boolean;startX:number;startY:number}|undefined;
  const cell=style.cellSize;
  const definition=(id:string)=>config.definitions.find(d=>d.id===state?.items[id]?.definition)!;
  const text=(tag:string,value:string)=>{const el=doc.createElement(tag);el.textContent=value;return el;};
  const button=(label:string,action:()=>void)=>{const b=text('button',label) as HTMLButtonElement;b.type='button';b.onclick=action;return b;};
  function cancel(){drag=undefined;root.querySelector('.placement')?.remove();}
  function quantity(type:'split'|'drop') {
    if(!state||!selected||!state.items[selected])return;
    const i=state.items[selected],max=i.quantity-(type==='split'?1:0);if(max<1)return;
    if(type==='drop'&&max===1){send({type,id:i.id,quantity:1});return;}
    modal={type,id:i.id,max};render();
    (root.querySelector('input') as HTMLInputElement)?.focus();
  }
  function icon(id:string) {
    const v=style.visuals[state!.items[id].definition],svg=doc.createElementNS('http://www.w3.org/2000/svg','svg');
    svg.setAttribute('viewBox','0 0 100 70');svg.classList.add('item-icon');svg.setAttribute('aria-hidden','true');
    for(const box of v.boxes) {
      const [x,y,,w,h]=box.map(Number),r=doc.createElementNS(svg.namespaceURI,'rect');
      r.setAttribute('x',String(50+x*90-w*45));r.setAttribute('y',String(65-y*100-h*50));r.setAttribute('width',String(w*90));r.setAttribute('height',String(h*100));r.setAttribute('fill',v.color);r.setAttribute('stroke','#17211e');r.setAttribute('stroke-width','2');svg.append(r);
    }return svg;
  }
  function hideTooltip(){hover=undefined;root.querySelector('.item-tooltip')?.remove();}
  function closeMenu(){menu=undefined;root.querySelector('.item-context')?.remove();}
  function placeOverlay(el:HTMLElement,x:number,y:number){
    root.append(el);const r=el.getBoundingClientRect(),win=doc.defaultView!;
    el.style.left=Math.max(12,Math.min(x+14,win.innerWidth-r.width-12))+'px';el.style.top=Math.max(12,Math.min(y+12,win.innerHeight-r.height-12))+'px';
  }
  function showTooltip(){
    root.querySelector('.item-tooltip')?.remove();if(!hover||menu||drag||modal||!state?.open)return;
    const panel=doc.createElement('aside');panel.className='item-tooltip';panel.setAttribute('role','tooltip');
    const i=hover.id&&state.items[hover.id];
    if(i){const d=definition(i.id),category=text('div',(d.category??'物资')+' · '+(d.usage??d.tags.join(' / ')));category.className='tooltip-meta';
      const mass=d.weight*i.quantity+(d.weapon?(i.loaded??0)*config.definitions.find(a=>a.id===d.weapon!.ammo)!.weight:0);
      panel.append(category,text('h4',d.name),text('p',d.description),text('p','数量 '+i.quantity+' · 格型 '+d.size.join('×')+' · 堆叠 '+d.maxStack),text('p','载荷 '+mass.toFixed(2)+' kg'),text('p','参考价值 '+d.value+' / '+(d.valueQuantity??1)+(d.unit??'件')+' · 合计 '+(d.value*i.quantity/(d.valueQuantity??1)).toFixed(2)),text('p','归属：'+(i.location.kind==='world'?'附近地面':i.location.kind==='slot'?'角色装备':'背包')+' · 预留：无'));
      if(d.weapon)panel.append(text('p','枪内弹 '+i.loaded+' / '+d.weapon.capacity));
    }else panel.append(text('h4',hover.label??''));
    placeOverlay(panel,hover.x,hover.y);
  }
  function bindItem(b:HTMLElement,id:string,rotated=false,x=0,y=0,draggable=true){
    b.onpointerenter=ev=>{hover={id,x:ev.clientX,y:ev.clientY};showTooltip();};
    b.onpointermove=ev=>{if(hover?.id===id){hover.x=ev.clientX;hover.y=ev.clientY;showTooltip();}};
    b.onpointerleave=()=>hideTooltip();
    b.onfocus=()=>{const r=b.getBoundingClientRect();hover={id,x:r.right,y:r.top};showTooltip();};b.onblur=()=>hideTooltip();
    b.oncontextmenu=ev=>{ev.preventDefault();ev.stopPropagation();selected=id;hideTooltip();cancel();menu={id,x:ev.clientX,y:ev.clientY};render();};
    if(draggable)b.onpointerdown=ev=>{if(ev.button!==0||modal)return;closeMenu();hideTooltip();selected=id;drag={id,rotated,x,y,active:false,startX:ev.clientX,startY:ev.clientY};ev.preventDefault();};
  }
  function slotOutline(slot:string){
    const ns='http://www.w3.org/2000/svg',svg=doc.createElementNS(ns,'svg');svg.setAttribute('viewBox','0 0 48 48');svg.classList.add('slot-outline');svg.setAttribute('aria-hidden','true');
    const shapes:Record<string,string>={head:'M9 28v-8a15 15 0 0 1 30 0v8M7 29h34M13 29v8h6v-8m10 0v8h6v-8M18 8v12m12-12v12',armor:'M16 6l8 5 8-5 8 9-6 5v22H14V20l-6-5zM18 24h12v12H18z',primary:'M5 19h30v9H21l-4 12H9l4-12H5zM35 20h9v5h-9M23 28v6h8v-6',backpack:'M17 9V6h14v3M13 11h22l4 8v23H9V19zM14 27h20v11H14zM17 16h14'};
    const path=doc.createElementNS(ns,'path');path.setAttribute('d',shapes[slot]);svg.append(path);return svg;
  }
  function renderMenu(){
    if(!menu||!state||!state.items[menu.id]){closeMenu();return;}const i=state.items[menu.id],d=definition(i.id),panel=doc.createElement('div');panel.className='item-context';panel.setAttribute('role','menu');panel.setAttribute('aria-label',d.name+'操作');
    const action=(label:string,run:()=>void)=>{const b=button(label,()=>{closeMenu();hideTooltip();run();});b.setAttribute('role','menuitem');panel.append(b);};
    if(i.location.kind==='world')action('拾取',()=>send({type:'take',id:i.id}));
    if(i.location.kind==='grid'){if(i.quantity>1)action('拆分',()=>quantity('split'));action('丢弃',()=>quantity('drop'));}
    if(d.weapon&&i.location.kind!=='world'){
      action(i.location.kind==='slot'?'卸下':'装备',()=>send({type:i.location.kind==='slot'?'unequip':'equip',id:i.id}));action('装填',()=>send({type:'load',id:i.id}));action('卸弹',()=>send({type:'unload',id:i.id}));
    }
    if(!panel.childElementCount){const b=button('固定装备',()=>{});b.disabled=true;b.setAttribute('role','menuitem');panel.append(b);}
    placeOverlay(panel,menu.x,menu.y);
  }
  function render() {
    if(!state)return;root.replaceChildren();
    const hud=text('div','Tab 背包  ·  WASD 移动  ·  Shift 跑步');hud.className='item-hud';
    if(state.target&&!state.open){const i=state.items[state.target];hud.append(text('div',`E 拾取 ${definition(i.id).name} ×${i.quantity}`));}
    if(!state.result.ok&&!state.open)hud.append(text('div',state.result.hint));if(!state.open)root.append(hud);
    if(!state.open){modal=undefined;hover=undefined;menu=undefined;cancel();return;}
    const shell=doc.createElement('div');shell.className='inventory-shell';const panel=doc.createElement('section');panel.className='inventory-window';panel.setAttribute('aria-label','角色背包');
    const header=doc.createElement('div');header.className='inventory-header';const title=doc.createElement('div');title.append(text('h2','行囊'));header.append(title,button('关闭 · Tab',()=>{cancel();modal=undefined;hideTooltip();closeMenu();send({type:'close'});}));panel.append(header);
    shell.oncontextmenu=ev=>ev.preventDefault();
    const body=doc.createElement('div');body.className='inventory-body';
    const character=doc.createElement('section');character.className='character-section';character.setAttribute('aria-label','人物与装备');character.append(text('h3','装备'));
    const loadout=doc.createElement('div');loadout.className='character-loadout';const stage=doc.createElement('div');stage.className='character-stage';
    const held=state.equipped&&state.items[state.equipped],weapon=held&&definition(held.id).weapon;
    stage.append(characterPreview(doc,style,held&&weapon?{definition:held.definition,pose:weapon.pose}:undefined));loadout.append(stage);
    const equipment=doc.createElement('div');equipment.className='equipment';
    for(const [slot,label] of [['head','头部 · 未开放'],['armor','护甲 · 未开放'],['primary','主武器'],['backpack','背包']]){
      const i=Object.values(state.items).find(i=>i.location.kind==='slot'&&i.location.parent===state!.playerId&&i.location.slot===slot);
      const b=button('',()=>{if(i){selected=i.id;render();}});b.className='slot'+(i&&selected===i.id?' selected':'');b.dataset.slot=slot;b.setAttribute('aria-label',label+(i?' · '+definition(i.id).name:''));
      if(i){b.dataset.itemId=i.id;b.append(icon(i.id));bindItem(b,i.id,false,0,0,slot==='primary');if(definition(i.id).weapon){const n=text('span',i.loaded+'/'+definition(i.id).weapon!.capacity);n.className='quantity';b.append(n);}}
      else {b.append(slotOutline(slot));b.onpointerenter=ev=>{hover={label,x:ev.clientX,y:ev.clientY};showTooltip();};b.onpointerleave=hideTooltip;}
      if(slot==='head'||slot==='armor'){b.setAttribute('aria-disabled','true');const lock=doc.createElementNS('http://www.w3.org/2000/svg','svg');lock.setAttribute('viewBox','0 0 12 14');lock.classList.add('slot-lock');lock.setAttribute('aria-hidden','true');const path=doc.createElementNS(lock.namespaceURI,'path');path.setAttribute('d','M3 6V4a3 3 0 0 1 6 0v2M2 6h8v6H2z');lock.append(path);b.append(lock);}
      equipment.append(b);
    }loadout.append(equipment);character.append(loadout);body.append(character);
    const renderItem=({item:i,x,y,rotated}:GridPlacement)=>{
      const d=definition(i.id),[w,h]=dimensions(d,rotated),v=style.visuals[d.id];
      const b=button('',()=>{selected=i.id;render();});b.className='inventory-item'+(selected===i.id?' selected':'');b.dataset.itemId=i.id;b.setAttribute('aria-label',d.name+' ×'+i.quantity);
      b.style.cssText='left:'+(x*cell+2)+'px;top:'+(y*cell+2)+'px;width:'+(w*cell-3)+'px;height:'+(h*cell-3)+'px;border-color:'+v.color+';background:'+v.color+'20';
      b.append(icon(i.id));const n=text('span','×'+i.quantity);n.className='quantity';b.append(n);
      bindItem(b,i.id,rotated,x,y);return b;
    };
    for(const container of state.containers){
      const wrap=doc.createElement('div');wrap.className='grid-wrap';wrap.append(text('h3','背包'));
      wrap.append(mountGrid(doc,{id:container.id,label:'背包网格',columns:container.columns,rows:container.rows,cell,placements:container.items.map(item=>{const l=item.location as {x:number;y:number;rotated:boolean};return {item,...l};}),renderItem}));body.append(wrap);
    }
    const near=doc.createElement('div');near.append(text('h3','散落物'));const nearWrap=doc.createElement('div');nearWrap.className='nearby-wrap';
    const layout=nearbyLayout(state.nearby.map(id=>state!.items[id]).filter(Boolean),config,5);
    nearWrap.append(mountGrid(doc,{id:'nearby',label:'附近物品网格',columns:5,rows:Math.max(6,layout.rows),cell,placements:layout.placements,renderItem}));near.append(nearWrap);body.append(near);
    panel.append(body);const footer=doc.createElement('div');footer.className='inventory-footer';footer.append(text('span',`载荷 ${state.weight.toFixed(2)} kg`),text('span','悬停查看 · 右键操作 · 拖拽整理'));panel.append(footer);
    const notice=text('div',state.result.ok?'':state.result.hint);notice.className='notice';notice.setAttribute('role','status');panel.append(notice);shell.append(panel);root.append(shell);showTooltip();renderMenu();
    if(modal) {
      const overlay=doc.createElement('div');overlay.className='quantity-overlay';const dialog=doc.createElement('form');dialog.className='quantity-dialog';dialog.setAttribute('role','dialog');dialog.setAttribute('aria-label',modal.type==='drop'?'丢弃数量':'拆分数量');
      dialog.append(text('strong',modal.type==='drop'?'丢弃到脚边':'拆分物品'));const input=doc.createElement('input');input.type='number';input.min='1';input.max=String(modal.max);input.step='1';input.value=String(modal.type==='drop'?modal.max:1);input.required=true;input.setAttribute('aria-label','数量');dialog.append(input,text('div',`可选数量：1—${modal.max}`));
      const submit=button('确定',()=>{});submit.type='submit';dialog.append(submit,button('取消',()=>{modal=undefined;render();}));dialog.onsubmit=ev=>{ev.preventDefault();const n=Number(input.value);if(modal&&Number.isInteger(n)&&n>=1&&n<=modal.max){send({type:modal.type,id:modal.id,quantity:n});modal=undefined;render();}};overlay.append(dialog);root.append(overlay);
    }
  }
  function preview() {
    if(!drag||!state)return;const grid=root.querySelector<HTMLElement>('[data-container-id="'+state.backpackId+'"]');if(!grid)return;
    root.querySelector('.placement')?.remove();const p=doc.createElement('div');p.className='placement';const [w,h]=dimensions(definition(drag.id),drag.rotated);
    let valid=canPlace(state,config,drag.id,state.backpackId,drag.x,drag.y,drag.rotated);
    const own=state.items[drag.id];
    if(!valid)valid=Object.values(state.items).some(i=>{const l=i.location;if(l.kind!=='grid'||l.parent!==state!.backpackId||i.id===own.id||i.definition!==own.definition||i.quantity>=definition(i.id).maxStack)return false;const [iw,ih]=dimensions(definition(i.id),l.rotated);return drag!.x>=l.x&&drag!.x<l.x+iw&&drag!.y>=l.y&&drag!.y<l.y+ih;});
    p.style.cssText=`left:${drag.x*cell}px;top:${drag.y*cell}px;width:${w*cell}px;height:${h*cell}px;border-color:${valid?style.colors.valid:style.colors.invalid};background:${valid?style.colors.valid:style.colors.invalid}44`;grid.append(p);
  }
  const move=(ev:PointerEvent)=>{if(!drag||!state)return;if(Math.hypot(ev.clientX-drag.startX,ev.clientY-drag.startY)>4)drag.active=true;if(!drag.active)return;const grid=root.querySelector('[data-container-id="'+state.backpackId+'"]');if(!grid)return;const r=grid.getBoundingClientRect();drag.x=Math.floor((ev.clientX-r.left)/cell);drag.y=Math.floor((ev.clientY-r.top)/cell);preview();};
  const up=(ev:PointerEvent)=>{
    if(!drag||!state)return;const d=drag;cancel();selected=d.id;
    const hit=(selector:string)=>{const el=root.querySelector(selector),r=el?.getBoundingClientRect();return !!r&&ev.clientX>=r.left&&ev.clientX<r.right&&ev.clientY>=r.top&&ev.clientY<r.bottom;};
    if(d.active){
      const i=state.items[d.id];
      if(hit('[data-slot="primary"]'))send({type:'equip',id:d.id});
      else if(hit('[data-container-id="'+state.backpackId+'"]')){
        if(i.location.kind==='world')send({type:'take',id:d.id,x:d.x,y:d.y,rotated:d.rotated});
        else if(i.location.kind==='slot')send({type:'unequip',id:d.id});
        else send({type:'move',id:d.id,parent:state.backpackId,x:d.x,y:d.y,rotated:d.rotated});
      }else if(hit('[data-container-id="nearby"]')&&i.location.kind==='grid')quantity('drop');
    }render();
  };
  const key=(ev:KeyboardEvent)=>{
    if(ev.code===style.keys.toggle){ev.preventDefault();ev.stopPropagation();if(!ev.repeat){cancel();modal=undefined;closeMenu();hideTooltip();send({type:'toggle'});}return;}
    if(ev.code===style.keys.pickup){ev.preventDefault();const command=itemKeyCommand(ev.code,ev.repeat,style.keys,state);if(command)send(command);return;}
    if(!state?.open)return;
    if(ev.code===style.keys.close){ev.preventDefault();if(!ev.repeat){if(modal){modal=undefined;render();}else if(menu){closeMenu();hideTooltip();}else if(drag){cancel();render();}else {hideTooltip();send({type:'close'});}}}
    if(ev.code===style.keys.rotate&&drag&&definition(drag.id).rotatable&&!ev.repeat){ev.preventDefault();drag.rotated=!drag.rotated;drag.active=true;preview();}
  };
  const outside=(ev:PointerEvent)=>{if(!(ev.target instanceof doc.defaultView!.Node))return;if(!root.querySelector('.item-context')?.contains(ev.target))closeMenu();};
  const blur=()=>{cancel();modal=undefined;closeMenu();hideTooltip();render();};
  doc.addEventListener('pointerdown',outside,true);doc.addEventListener('pointermove',move);doc.addEventListener('pointerup',up);doc.addEventListener('keydown',key,true);doc.defaultView!.addEventListener('blur',blur);
  return {update(next:ItemProjection){const signature=JSON.stringify(next);state=next;if(signature!==last){last=signature;if(!drag&&!modal)render();}},isOpen:()=>!!state?.open,
    dispose(){doc.removeEventListener('pointerdown',outside,true);doc.removeEventListener('pointermove',move);doc.removeEventListener('pointerup',up);doc.removeEventListener('keydown',key,true);doc.defaultView!.removeEventListener('blur',blur);root.remove();css.remove();}};
}
