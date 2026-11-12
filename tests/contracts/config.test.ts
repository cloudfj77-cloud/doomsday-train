import {readFileSync,readdirSync} from 'node:fs';
import {it,expect} from 'vitest';
import profile from '../../config/gameplay/player.json';
import world from '../../config/gameplay/world.json';
import {validatePlayer} from '../../scripts/logic/player.ts';
import {configFields,playerConfig,worldConfig,itemConfig,itemWorldConfig,sceneConfig,itemStyleConfig} from '../../scripts/config/game.ts';
it('assembles player parameters from their single source without changing defaults',()=>{
 expect(worldConfig.speed).toBe(playerConfig.movement.speed);expect(worldConfig.height).toBe(playerConfig.body.height);expect(worldConfig.start).toEqual(playerConfig.spawn);expect(itemWorldConfig.backpackDefinition).toBe(playerConfig.initialBackpack);
 expect(itemConfig.definitions.find(d=>d.id===playerConfig.item.id)!.stats).toEqual(playerConfig.baseStats);expect(sceneConfig.keys).toEqual(playerConfig.keys.movement);expect(itemStyleConfig.keys).toEqual(playerConfig.keys.inventory);
 expect(Object.keys(world).sort()).toEqual(['gravity','ground','limit','obstacles','surfaces']);
});
it('rejects mixed responsibilities, invalid profiles and conflicting controls',()=>{
 expect(()=>configFields({camera:{},speed:4},['camera'],'camera')).toThrow('config-fields');
 for(const edit of [(p:any)=>p.movement.speed=-1,(p:any)=>p.body.height=p.body.radius,(p:any)=>p.initialBackpack='',(p:any)=>p.keys.inventory.pickup=p.keys.movement.up,(p:any)=>p.reservedStats.stamina=100]){const p=structuredClone(profile);edit(p);expect(()=>validatePlayer(p)).toThrow();}
 const remapped=structuredClone(profile);remapped.keys.movement.up='ArrowUp';remapped.keys.movement.run='ShiftRight';expect(validatePlayer(remapped)).toBe(remapped);
});

it('registers every config exactly once in the runtime assembler',()=>{
 const walk=(dir:string):string[]=>readdirSync(dir,{withFileTypes:true}).flatMap(e=>e.isDirectory()?walk(dir+'/'+e.name):[dir+'/'+e.name]);
 const files=walk('config').filter(p=>p.endsWith('.json')).sort();
 const imports=[...readFileSync('scripts/config/game.ts','utf8').matchAll(/from '\.\.\/\.\.\/(config\/[^']+\.json)'/g)].map(m=>m[1]).sort();
 expect(imports).toEqual(files);expect(new Set(imports).size).toBe(imports.length);
});
