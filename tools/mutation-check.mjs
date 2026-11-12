import { existsSync, readFileSync, writeFileSync, unlinkSync } from 'node:fs';
import { spawnSync } from 'node:child_process';
const run=()=>spawnSync(process.execPath,['node_modules/vitest/vitest.mjs','run'],{encoding:'utf8'});
// Windows file watchers and antivirus may hold a source briefly. Always retry restoration.
function retryFile(action) {
 for(let attempt=0;;attempt++)try{return action();}catch(error){
  if(attempt>=20||!['EPERM','EACCES','EBUSY','UNKNOWN'].includes(error.code))throw error;
  Atomics.wait(new Int32Array(new SharedArrayBuffer(4)),0,0,100);
 }
}
const replace=(before,after)=>s=>s.replace(before,after);
const cases=[
 ['config/gameplay/items.json',replace('"maxStack":20','"maxStack":0')],
 ['config/gameplay/item-world.json',replace('"interactionDistance":1.5','"interactionDistance":-1')],
 ['config/presentation/items.json',replace('"cellSize":44','"cellSize":1')],
 ['scripts/logic/items.ts',replace('ids.has(d.id)','false')],
 ['scripts/logic/items.ts',replace('x+width>grid.columns','false')],
 ['scripts/logic/items.ts',replace('return x+width<=l.x||l.x+ow<=x||y+height<=l.y||l.y+oh<=y','return true')],
 ['scripts/logic/items.ts',replace('rotated?[d.size[1],d.size[0]]','rotated?[d.size[0],d.size[1]]')],
 ['scripts/logic/items.ts',replace('v.distance<=w.interactionDistance&&e.visible(v.p)','true')],
 ['scripts/logic/items.ts',replace('if(s.open||nearestItem','if(false&&s.open||nearestItem')],
 ['scripts/logic/items.ts',replace('target.quantity+=n','target.quantity+=0')],
 ['scripts/logic/items.ts',replace('item.quantity-=cmd.quantity','item.quantity-=0')],
 ['scripts/logic/items.ts',replace('if(drop!==item)item.quantity-=cmd.quantity','if(false)item.quantity-=cmd.quantity')],
 ['scripts/logic/items.ts',replace('if(!place)return fail(\'no-space\',\'拆分需要额外空间\')','if(false)return fail(\'no-space\',\'拆分需要额外空间\')')],
 ['scripts/logic/items.ts',replace('!accept?.some(tag=>d.tags.includes(tag))','false')],
 ['scripts/logic/items.ts',replace('Object.freeze(v)','v')],
 ['scripts/logic/items.ts',replace("target&&s.items[target]?.location.kind==='world'?target:null","target??null")],
 ['scripts/logic/items.ts',replace('reduce((n,m)=>n*m.value,1)','reduce((n,m)=>n+m.value,1)')],
 ['scripts/controller/item-environment.ts',replace('return !hit||','return true||')],
 ['scripts/controller/item-environment.ts',replace('if(clear)return p','if(true)return p')],
 ['scripts/view/item-ui.ts',replace('parent.append(root)','doc.body.append(root)')],
 ['scripts/view/item-presentation.ts',replace('if(repeat)return','if(false)return')],
 ['config/presentation/scene.json',replace('"gravelCount": 195','"gravelCount": -2')],
 ['scripts/logic/world.ts',replace('ids.has(o.id)','false')],
 ['scripts/logic/world.ts',replace('if (!surface && c.start.every','if (false && !surface && c.start.every')],
 ['scripts/logic/world.ts',replace('Math.max(1,Math.hypot(...axis))','1')],
 ['scripts/logic/world.ts',replace('x*speed*dt','-x*speed*dt')],
 ['scripts/logic/world.ts',replace('input.run?c.runSpeed:c.speed','c.speed')],
 ['scripts/logic/world.ts',replace('s.grounded?x*speed*dt:0','x*speed*dt')],
 ['scripts/logic/world.ts',replace('Math.hypot(input.aim[0]-position[0],input.aim[1]-position[2])>c.aimDeadzone','true')],
 ['scripts/logic/world.ts',replace('Math.atan2(Math.sin(target-yaw),Math.cos(target-yaw))','(target-yaw)')],
 ['scripts/logic/world.ts',replace('moving=grounded && distance>1e-5','moving=true')],
 ['scripts/logic/world.ts',replace('s.walkDistance+(moving?distance:0)','s.walkDistance')],
 ['scripts/logic/world.ts',replace('Object.freeze({ ...state })','({ ...state })')],
 ['scripts/controller/movement.ts',replace('autoStepMaxHeight:c.stepHeight','autoStepMaxHeight:0')],
 ['scripts/controller/movement.ts',replace('maxSlopeClimbDeg:c.maxSlope','maxSlopeClimbDeg:89')],
 ['scripts/controller/movement.ts',replace('physics.moveAndSlide(entity,vec3.create(...d))','physics.moveAndSlide(entity,vec3.create(0,0,0))')],
 ['scripts/logic/camera.ts',replace('if(t<0) return;','if(false) return;')],
 ['scripts/logic/camera.ts',replace('const t=(y-ray[1])/ray[4]','const t=-ray[1]/ray[4]')],
 ['scripts/logic/camera.ts',replace('ids.has(z.id)','false')],
 ['scripts/logic/camera.ts',replace('-previous.offset[0]','')],
 ['scripts/logic/camera.ts',replace('c.yaw!==-30','false')],
 ['scripts/logic/camera.ts',replace('.sort((a,b)=>b-a)','.sort((a,b)=>a-b)')],
 ['scripts/logic/camera.ts',replace('Math.min(1,total)*remainder','0')],
 ['scripts/logic/camera.ts',replace('Math.min(1,parameters.aimOffset/Math.max(1e-9,Math.hypot(...delta)))','1')],
 ['scripts/logic/camera.ts',replace('1-Math.exp(-dt/time)','dt/time')],
 ['scripts/logic/camera.ts',replace('previous.offset.map((v,i)=>v+(delta[i]*scale-v)*blend(c.aimTime))','previous.offset')],
 ['scripts/logic/layout.test.ts',()=> '// misplaced test'],
 ['Documents/uncategorized.md',()=> '# misplaced document'],
 ['assets/world.json',()=> '{}'],
 ['config/gameplay/world.json',replace('"speed": 4','"speed": -1')],
 ['scripts/logic/world.ts',s=>s+'\n// document. forbidden boundary\n'],
 ['scripts/view/scene.ts',s=>s+'\n// step( forbidden boundary\n'],
 ['AGENTS.md',s=>s+'x'.repeat(20480)],
 ['forge.json',s=>{const f=JSON.parse(s);delete f.roots.engine;return JSON.stringify(f);}]
];
if(run().status!==0)throw Error('baseline must pass');
for(const [file,mutate] of cases){
 const existed=existsSync(file),original=existed?readFileSync(file,'utf8'):'';
 try {const broken=mutate(original);if(broken===original)throw Error('mutation did not apply: '+file);retryFile(()=>writeFileSync(file,broken));
   if(run().status===0)throw Error('mutation escaped: '+file);console.log('caught broken premise:',file);
 }finally{retryFile(()=>{if(existed)writeFileSync(file,original);else if(existsSync(file))unlinkSync(file);});}
 const restored=run();if(restored.status!==0)throw Error('restored checks failed: '+file+'\n'+restored.stdout+'\n'+restored.stderr);
}
console.log(`Passed ${cases.length} mutations and restorations`);
