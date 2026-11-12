import assert from 'node:assert/strict';
import {readFileSync,writeFileSync,existsSync,mkdirSync} from 'node:fs';
import {resolve} from 'node:path';
import {spawnSync} from 'node:child_process';
const cli=resolve('node_modules/@forgeax/engine/dist/bin/forgeax.mjs');
const env={...process.env,FORGEAX_HISTORY_DIR:process.env.FORGEAX_HISTORY_DIR??resolve('qa/operation-history')};
if(!env.FORGEAX_BROWSER_EXECUTABLE&&process.platform==='win32'){
 const edge=resolve(process.env['ProgramFiles(x86)']??'C:/Program Files (x86)','Microsoft/Edge/Application/msedge.exe');if(existsSync(edge))env.FORGEAX_BROWSER_EXECUTABLE=edge;
}
function write(file,source){for(let i=0;;i++)try{writeFileSync(file,source);return;}catch(e){if(i===20||!['EPERM','EACCES','EBUSY'].includes(e.code))throw e;Atomics.wait(new Int32Array(new SharedArrayBuffer(4)),0,0,100);}}
const change=(a,b)=>s=>{assert.ok(s.includes(a),'mutation must apply');return s.replace(a,b);};
const cases=[
 {name:'wrong-container',file:'scripts/view/item-ui.ts',script:'backpacks',expected:'drag into the backpack uses its actual container',mutate:change("else send({type:'move',id:d.id,parent,x:d.x","else send({type:'move',id:d.id,parent:state.pocketsId,x:d.x")},
 {name:'clipped-region',file:'scripts/view/item-ui.ts',script:'backpacks',expected:'clipped grid region must not accept drops',mutate:change('el.contains(doc.elementFromPoint(x,y))&&','')},

 {name:'hover-only',file:'scripts/view/drop-labels.ts',script:'backpacks',expected:'no item description without pointer hover',mutate:change('const shown=state.hovered?[state.hovered]:[]','const shown=state.nearby')},
 {name:'world-backpack',file:'assets/preview.pack.ts',script:'backpacks',expected:'world backpack entity must be removed',mutate:change("if(!bagEquipped&&entities.has('backpack'))","if(false&&entities.has('backpack'))")},
 {name:'visible-scrollbar',file:'scripts/view/item-ui.ts',script:'backpacks',viewport:'1280x600',expected:'custom scrollbar visible exactly on overflow',mutate:change('rail.hidden=max<=0','rail.hidden=true')},
 {name:'thumb-scroll',file:'scripts/view/item-ui.ts',script:'backpacks',viewport:'1280x600',expected:'dragging the scrollbar scrolls real grids',mutate:change('if(!thumbDrag)return;','if(true)return;')},
 {name:'list-scrollbar',file:'scripts/view/item-ui.ts',script:'backpacks',expected:'container list must scroll only on overflow',mutate:change('overflow-y:auto;overflow-x:clip','overflow-y:scroll;overflow-x:clip')},

 {name:'hover',file:'scripts/view/item-ui.ts',script:'items',expected:'tooltip',mutate:change('function showTooltip(){','function showTooltip(){if(true)return;')},
 {name:'context',file:'scripts/view/item-ui.ts',script:'items',expected:'menu',mutate:change('function renderMenu(){','function renderMenu(){if(true)return;')},
 {name:'scroll',file:'scripts/view/item-ui.ts',script:'backpacks',expected:'off-grid preview must not grow scroll extents',mutate:s=>{assert.ok(s.includes('if(width<=0||height<=0)return;'));return s.replace('const grid=gridAt(ev.clientX,ev.clientY);',"const grid=gridAt(ev.clientX,ev.clientY)??root.querySelector<HTMLElement>('.storage-list .container-section:last-child .inventory-grid');").replaceAll('overflow:clip','overflow:auto').replace('if(width<=0||height<=0)return;','').replace('left:${left}px;top:${top}px;width:${width}px;height:${height}px;','left:${drag.x*cell}px;top:${drag.y*cell}px;width:${w*cell}px;height:${h*cell}px;');}},
 {name:'outline',file:'assets/preview.pack.ts',script:'backpacks',expected:'real camera outline selection',mutate:change('entities:nearby.map(id=>drops.get(id)).filter((e):e is NonNullable<typeof e>=>e!==undefined)','entities:[]')},
 {name:'labels',file:'scripts/view/drop-labels.ts',script:'backpacks',expected:'only the hovered nearby item has a label',mutate:change('root.hidden=!state||state.open||!mapping','root.hidden=true')},
];
const results=[];
const only=process.argv.find(a=>a.startsWith('--case='))?.slice(7),selected=cases.filter(c=>(!process.argv.includes('--storage-only')||c.script==='backpacks')&&(!only||c.name===only));assert.ok(selected.length,'unknown mutation case');
for(const test of selected){
 const file=resolve(test.file),original=readFileSync(file,'utf8');
 try{
  write(file,test.mutate(original));
  const run=spawnSync(process.execPath,[cli,'project','play','tests/'+test.script+'-playthrough.mjs','--require-ui','--viewports',test.viewport??'1280x720','--timeout-ms','60000','--json'],{env,encoding:'utf8',timeout:190000,windowsHide:true,maxBuffer:2_000_000});
  assert.ok(!run.error,run.error?.message);const report=JSON.parse(run.stdout.trim());assert.notEqual(run.status,0,'broken UI must fail acceptance');assert.equal(report.error?.code,'play-step-failed');
  assert.ok(report.error.detail.message.includes(test.expected),'unexpected failure: '+report.error.detail.message);
  results.push({name:test.name,caught:true,error:report.error});console.log('caught UI mutation:',test.name);
 }finally{write(file,original);}
}
mkdirSync('qa/items',{recursive:true});writeFileSync('qa/items/ui-mutations'+(only?'-'+only:'')+'.json',JSON.stringify(results,null,2));console.log('Passed '+results.length+' UI mutations; source restored. Rerun the full playthrough for acceptance.');
