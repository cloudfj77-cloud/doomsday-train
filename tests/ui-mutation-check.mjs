import assert from 'node:assert/strict';
import {readFileSync,writeFileSync,existsSync,mkdirSync} from 'node:fs';
import {resolve} from 'node:path';
import {spawnSync} from 'node:child_process';
// Exercise the real owner browser against deliberately broken UI, then restore.
const file=resolve('scripts/view/item-ui.ts'),original=readFileSync(file,'utf8'),cli=resolve('node_modules/@forgeax/engine/dist/bin/forgeax.mjs');
const env={...process.env,FORGEAX_HISTORY_DIR:process.env.FORGEAX_HISTORY_DIR??resolve('qa/operation-history')};
if(!env.FORGEAX_BROWSER_EXECUTABLE&&process.platform==='win32'){
 const edge=resolve(process.env['ProgramFiles(x86)']??'C:/Program Files (x86)','Microsoft/Edge/Application/msedge.exe');
 if(existsSync(edge))env.FORGEAX_BROWSER_EXECUTABLE=edge;
}
function write(source){for(let i=0;;i++)try{writeFileSync(file,source);return;}catch(e){if(i===20||!['EPERM','EACCES','EBUSY'].includes(e.code))throw e;Atomics.wait(new Int32Array(new SharedArrayBuffer(4)),0,0,100);}}
const results=[];
for(const [name,signature,expected] of [['hover','function showTooltip(){','tooltip'],['context','function renderMenu(){','menu']]){
 assert.ok(original.includes(signature));
 try{
  write(original.replace(signature,signature+'if(true)return;'));
  const run=spawnSync(process.execPath,[cli,'project','play','tests/items-playthrough.mjs','--require-ui','--viewports','1280x720','--timeout-ms','60000','--json'],{env,encoding:'utf8',timeout:190000,windowsHide:true,maxBuffer:2_000_000});
  assert.ok(!run.error,run.error?.message);const report=JSON.parse(run.stdout.trim());
  assert.notEqual(run.status,0,'broken UI must fail acceptance');assert.equal(report.error?.code,'play-step-failed');
  assert.ok(report.error.detail.message.includes(expected),'failure must identify the intentionally missing '+expected);
  results.push({name,caught:true,error:report.error});console.log('caught UI mutation:',name);
 }finally{write(original);}
}
mkdirSync('qa/items',{recursive:true});writeFileSync('qa/items/ui-mutations.json',JSON.stringify(results,null,2));
console.log('Passed 2 UI mutations; source restored. Rerun the full playthrough for acceptance.');
