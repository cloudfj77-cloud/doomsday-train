import { canPlace, dimensions, type ItemConfig, type ItemCommand, type ItemProjection } from '../logic/items.ts';
import { itemKeyCommand, type ItemStyle } from './item-presentation.ts';
export function mountInventory(parent:HTMLElement,config:ItemConfig,style:ItemStyle,send:(cmd:ItemCommand)=>void) {
  const doc=parent.ownerDocument;
  const root=doc.createElement('div');root.id='item-ui';parent.append(root);
  const css=doc.createElement('style');css.textContent=`
  #item-ui{position:fixed;inset:0;z-index:100;pointer-events:none;font:14px/1.5 system-ui,sans-serif;color:${style.colors.text}}
  #item-ui *{box-sizing:border-box} #item-ui button,#item-ui input{font:inherit;color:inherit}
  #item-ui button{cursor:pointer;border:1px solid ${style.colors.line};background:#293833;border-radius:4px;padding:8px 12px}
  #item-ui button:focus-visible,#item-ui input:focus-visible{outline:2px solid ${style.colors.accent}}
  #item-ui .item-hud{position:absolute;left:24px;bottom:24px;background:#152022eb;border:1px solid ${style.colors.line};padding:10px 16px;border-radius:6px}
  #item-ui .inventory-shell{position:absolute;inset:0;background:#07101175;display:flex;align-items:center;justify-content:center;pointer-events:auto}
  #item-ui .inventory-window{width:min(880px,96vw);max-height:94vh;overflow:auto;background:${style.colors.panel};border:1px solid #6c796b;box-shadow:0 20px 80px #0009;border-radius:10px;padding:22px}
  #item-ui .inventory-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:16px} #item-ui h2{margin:0;font-size:24px;font-weight:600} #item-ui .muted{color:#b6c0ac;font-size:12px}
  #item-ui .inventory-body{display:flex;gap:22px;align-items:flex-start} #item-ui .grid-wrap{overflow:auto;max-width:100%;padding:2px}
  #item-ui .inventory-grid{position:relative;background-color:${style.colors.grid};background-image:linear-gradient(to right,${style.colors.line} 1px,transparent 1px),linear-gradient(to bottom,${style.colors.line} 1px,transparent 1px);border-right:1px solid ${style.colors.line};border-bottom:1px solid ${style.colors.line};touch-action:none}
  #item-ui .inventory-item{position:absolute;display:flex;align-items:center;justify-content:center;padding:3px;user-select:none;touch-action:none;border-radius:3px;overflow:hidden;background:#35443e}
  #item-ui .inventory-item.selected{outline:2px solid ${style.colors.accent};outline-offset:-3px} #item-ui .inventory-item .quantity{position:absolute;right:5px;bottom:2px;background:#142020dd;padding:0 3px;font-size:12px}
  #item-ui .item-icon{width:80%;height:80%;max-width:100px;max-height:76px;image-rendering:pixelated}
  #item-ui .item-detail{flex:1;min-width:200px;max-width:290px} #item-ui .item-detail h3{font-size:20px;margin:4px 0 12px} #item-ui .detail-actions{display:flex;gap:8px;margin-top:18px;flex-wrap:wrap}
  #item-ui .inventory-footer{margin-top:16px;display:flex;justify-content:space-between;gap:12px;font-size:12px;color:#b6c0ac}
  #item-ui .placement{position:absolute;pointer-events:none;border:2px solid;z-index:4;opacity:.65}
  #item-ui .quantity-overlay{position:absolute;inset:0;background:#071011ba;display:flex;align-items:center;justify-content:center;pointer-events:auto;z-index:8}
  #item-ui .quantity-dialog{width:300px;padding:22px;border:1px solid ${style.colors.line};border-radius:8px;background:${style.colors.panel}} #item-ui input{width:100%;margin:12px 0;padding:8px;background:${style.colors.grid};border:1px solid ${style.colors.line}}
  #item-ui .notice{color:${style.colors.accent};min-height:20px;margin-top:12px} @media(max-width:740px){#item-ui .inventory-body{flex-direction:column}#item-ui .item-detail{max-width:none}#item-ui .inventory-window{padding:14px}}`;
  doc.head.append(css);
  let state:ItemProjection|undefined,selected:string|undefined,last='',modal:{type:'split'|'drop';id:string;max:number}|undefined;
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
  function render() {
    if(!state)return;root.replaceChildren();
    const hud=text('div','Tab 背包  ·  WASD 移动  ·  Shift 跑步');hud.className='item-hud';
    if(state.target&&!state.open){const i=state.items[state.target];hud.append(text('div',`E 拾取 ${definition(i.id).name} ×${i.quantity}`));}
    if(!state.result.ok&&!state.open)hud.append(text('div',state.result.hint));root.append(hud);
    if(!state.open){modal=undefined;cancel();return;}
    const shell=doc.createElement('div');shell.className='inventory-shell';const panel=doc.createElement('section');panel.className='inventory-window';panel.setAttribute('aria-label','角色背包');
    const header=doc.createElement('div');header.className='inventory-header';const title=doc.createElement('div');title.append(text('h2','行囊'),text('div','铁路拾荒者 / FIELD INVENTORY'));title.lastElementChild!.className='muted';header.append(title,button('关闭 · Tab',()=>{cancel();modal=undefined;send({type:'close'});}));panel.append(header);
    const body=doc.createElement('div');body.className='inventory-body';const wrap=doc.createElement('div');wrap.className='grid-wrap';const grid=doc.createElement('div');grid.className='inventory-grid';grid.setAttribute('aria-label','背包网格');grid.style.cssText=`width:${state.grid.columns*cell}px;height:${state.grid.rows*cell}px;background-size:${cell}px ${cell}px`;wrap.append(grid);body.append(wrap);
    for(const i of Object.values(state.items)) {
      const l=i.location;if(l.kind!=='grid'||l.parent!==state.backpackId)continue;
      const d=definition(i.id),[w,h]=dimensions(d,l.rotated),v=style.visuals[d.id];
      const b=button('',()=>{selected=i.id;render();});b.className='inventory-item'+(selected===i.id?' selected':'');b.dataset.itemId=i.id;b.setAttribute('aria-label',`${d.name} ×${i.quantity}`);
      b.style.cssText=`left:${l.x*cell+2}px;top:${l.y*cell+2}px;width:${w*cell-3}px;height:${h*cell-3}px;border-color:${v.color};background:${v.color}20`;
      b.append(icon(i.id));const n=text('span',`×${i.quantity}`);n.className='quantity';b.append(n);
      b.onpointerdown=ev=>{if(ev.button!==0||modal)return;selected=i.id;drag={id:i.id,rotated:l.rotated,x:l.x,y:l.y,active:false,startX:ev.clientX,startY:ev.clientY};ev.preventDefault();};grid.append(b);
    }
    const detail=doc.createElement('aside');detail.className='item-detail';detail.setAttribute('aria-label','物品详情');const chosen=selected&&state.items[selected];
    if(chosen&&chosen.location.kind==='grid') {
      const d=definition(chosen.id);detail.append(text('h3',d.name),text('p',d.description),text('p',`数量 ${chosen.quantity}  ·  尺寸 ${d.size.join('×')}`),text('p',`重量 ${(d.weight*chosen.quantity).toFixed(2)} kg`),text('p',`基础价值 ${d.value} / 件`));
      const actions=doc.createElement('div');actions.className='detail-actions';if(chosen.quantity>1)actions.append(button('拆分',()=>quantity('split')));actions.append(button('丢弃',()=>quantity('drop')));detail.append(actions);
    }else detail.append(text('h3','选择物品'),text('p','点击查看详情，拖拽整理位置。'),text('p','物品占用不同大小的格子。拾取时自动堆叠并寻找空位。'));
    body.append(detail);panel.append(body);const footer=doc.createElement('div');footer.className='inventory-footer';footer.append(text('span',`负重 ${state.weight.toFixed(2)} kg · ${state.grid.columns}×${state.grid.rows}`),text('span','拖拽移动 · R旋转 · Esc返回'));panel.append(footer);
    const notice=text('div',state.result.ok?'':state.result.hint);notice.className='notice';notice.setAttribute('role','status');panel.append(notice);shell.append(panel);root.append(shell);
    if(modal) {
      const overlay=doc.createElement('div');overlay.className='quantity-overlay';const dialog=doc.createElement('form');dialog.className='quantity-dialog';dialog.setAttribute('role','dialog');dialog.setAttribute('aria-label',modal.type==='drop'?'丢弃数量':'拆分数量');
      dialog.append(text('strong',modal.type==='drop'?'丢弃到脚边':'拆分物品'));const input=doc.createElement('input');input.type='number';input.min='1';input.max=String(modal.max);input.step='1';input.value=String(modal.type==='drop'?modal.max:1);input.required=true;input.setAttribute('aria-label','数量');dialog.append(input,text('div',`可选数量：1—${modal.max}`));
      const submit=button('确定',()=>{});submit.type='submit';dialog.append(submit,button('取消',()=>{modal=undefined;render();}));dialog.onsubmit=ev=>{ev.preventDefault();const n=Number(input.value);if(modal&&Number.isInteger(n)&&n>=1&&n<=modal.max){send({type:modal.type,id:modal.id,quantity:n});modal=undefined;render();}};overlay.append(dialog);root.append(overlay);
    }
  }
  function preview() {
    if(!drag||!state)return;const grid=root.querySelector<HTMLElement>('.inventory-grid');if(!grid)return;
    root.querySelector('.placement')?.remove();const p=doc.createElement('div');p.className='placement';const [w,h]=dimensions(definition(drag.id),drag.rotated);
    let valid=canPlace(state,config,drag.id,state.backpackId,drag.x,drag.y,drag.rotated);
    const own=state.items[drag.id];
    if(!valid)valid=Object.values(state.items).some(i=>{const l=i.location;if(l.kind!=='grid'||l.parent!==state!.backpackId||i.id===own.id||i.definition!==own.definition||i.quantity>=definition(i.id).maxStack)return false;const [iw,ih]=dimensions(definition(i.id),l.rotated);return drag!.x>=l.x&&drag!.x<l.x+iw&&drag!.y>=l.y&&drag!.y<l.y+ih;});
    p.style.cssText=`left:${drag.x*cell}px;top:${drag.y*cell}px;width:${w*cell}px;height:${h*cell}px;border-color:${valid?style.colors.valid:style.colors.invalid};background:${valid?style.colors.valid:style.colors.invalid}44`;grid.append(p);
  }
  const move=(ev:PointerEvent)=>{if(!drag)return;if(Math.hypot(ev.clientX-drag.startX,ev.clientY-drag.startY)>4)drag.active=true;if(!drag.active)return;const grid=root.querySelector('.inventory-grid');if(!grid)return;const r=grid.getBoundingClientRect();drag.x=Math.floor((ev.clientX-r.left)/cell);drag.y=Math.floor((ev.clientY-r.top)/cell);preview();};
  const up=(ev:PointerEvent)=>{if(!drag)return;const d=drag;cancel();if(d.active){const grid=root.querySelector('.inventory-grid'),r=grid?.getBoundingClientRect();if(r&&ev.clientX>=r.left&&ev.clientX<r.right&&ev.clientY>=r.top&&ev.clientY<r.bottom)send({type:'move',id:d.id,x:d.x,y:d.y,rotated:d.rotated});}render();};
  const key=(ev:KeyboardEvent)=>{
    if(ev.code===style.keys.toggle){ev.preventDefault();ev.stopPropagation();if(!ev.repeat){cancel();modal=undefined;send({type:'toggle'});}return;}
    if(ev.code===style.keys.pickup){ev.preventDefault();const command=itemKeyCommand(ev.code,ev.repeat,style.keys,state);if(command)send(command);return;}
    if(!state?.open)return;
    if(ev.code===style.keys.close){ev.preventDefault();if(!ev.repeat){if(modal){modal=undefined;render();}else if(drag){cancel();render();}else send({type:'close'});}}
    if(ev.code===style.keys.rotate&&drag&&definition(drag.id).rotatable&&!ev.repeat){ev.preventDefault();drag.rotated=!drag.rotated;drag.active=true;preview();}
  };
  const blur=()=>{cancel();modal=undefined;render();};
  doc.addEventListener('pointermove',move);doc.addEventListener('pointerup',up);doc.addEventListener('keydown',key,true);doc.defaultView!.addEventListener('blur',blur);
  return {update(next:ItemProjection){const signature=JSON.stringify(next);state=next;if(signature!==last){last=signature;if(!drag&&!modal)render();}},isOpen:()=>!!state?.open,
    dispose(){doc.removeEventListener('pointermove',move);doc.removeEventListener('pointerup',up);doc.removeEventListener('keydown',key,true);doc.defaultView!.removeEventListener('blur',blur);root.remove();css.remove();}};
}
