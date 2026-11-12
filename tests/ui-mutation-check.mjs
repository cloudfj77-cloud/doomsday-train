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
 {name:'hover',file:'scripts/view/item-ui.ts',script:'items',expected:'tooltip',mutate:change('function showTooltip(){','function showTooltip(){if(true)return;')},
 {name:'context',file:'scripts/view/item-ui.ts',script:'items',expected:'menu',mutate:change('function renderMenu(){','function renderMenu(){if(true)return;')},
 {name:'scroll',file:'scripts/view/item-ui.ts',script:'backpacks',expected:'off-grid preview must not grow scroll extents',mutate:s=>{assert.ok(s.includes('if(width<=0||height<=0)return;'));return s.replaceAll('overflow:clip','overflow:auto').replace('if(width<=0||height<=0)return;','').replace('left:${left}px;top:${top}px;width:${width}px;height:${height}px;','left:${drag.x*cell}px;top:${drag.y*cell}px;width:${w*cell}px;height:${h*cell}px;');}},
 {name:'outline',file:'assets/preview.pack.ts',script:'backpacks',expected:'real camera outline selection',mutate:change('entities:nearby.map(id=>drops.get(id)).filter((e):e is NonNullable<typeof e>=>e!==undefined)','entities:[]')},
 {name:'labels',file:'scripts/view/drop-labels.ts',script:'backpacks',expected:'pixel labels match nearby drops',mutate:change('root.hidden=!state||state.open||!mapping','root.hidden=true')},
];
const results=[];
for(const test of cases.filter(c=>!process.argv.includes('--storage-only')||c.script==='backpacks')){
 const file=resolve(test.file),original=readFileSync(file,'utf8');
 try{
  write(file,test.mutate(original));
  const run=spawnSync(process.execPath,[cli,'project','play','tests/'+test.script+'-playthrough.mjs','--require-ui','--viewports','1280x720','--timeout-ms','60000','--json'],{env,encoding:'utf8',timeout:190000,windowsHide:true,maxBuffer:2_000_000});
  assert.ok(!run.error,run.error?.message);const report=JSON.parse(run.stdout.trim());assert.notEqual(run.status,0,'broken UI must fail acceptance');assert.equal(report.error?.code,'play-step-failed');
  assert.ok(report.error.detail.message.includes(test.expected),'unexpected failure: '+report.error.detail.message);
  results.push({name:test.name,caught:true,error:report.error});console.log('caught UI mutation:',test.name);
 }finally{write(file,original);}
}
mkdirSync('qa/items',{recursive:true});writeFileSync('qa/items/ui-mutations.json',JSON.stringify(results,null,2));console.log('Passed '+results.length+' UI mutations; source restored. Rerun the full playthrough for acceptance.');
