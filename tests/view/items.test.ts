import {it,expect} from 'vitest';
import style from '../../config/presentation/items.json';
import raw from '../../config/gameplay/items.json';
import world from '../../config/gameplay/item-world.json';
import {validateItems} from '../../scripts/logic/items.ts';
import {validateItemStyle,itemKeyCommand} from '../../scripts/view/item-presentation.ts';
import {createItems,projectItems} from '../../scripts/logic/items.ts';
import {bakeItem} from '../../scripts/view/voxels.ts';
const {config}=validateItems(raw,world);
it('validates actual presentation and cooks every item mesh',()=>{
  expect(validateItemStyle(style,config)).toBe(style);
  for(const d of config.definitions.filter(d=>d.portable))expect(bakeItem(d.id).attributes.position).toBeInstanceOf(Float32Array);
  const broken=structuredClone(style);broken.visuals.water.boxes[0][6]='missing-color';expect(()=>validateItemStyle(broken,config)).toThrow();
  const keys=structuredClone(style);keys.keys.pickup=keys.keys.toggle;expect(()=>validateItemStyle(keys,config)).toThrow();
});
it('submits one pickup on the keydown edge and suppresses repeats or open bag input',()=>{
  const w=validateItems(raw,world).world,s=createItems(config,w),keys=validateItemStyle(style,config).keys;
  expect(itemKeyCommand('KeyE',false,keys,projectItems(s,config,'item-1'))).toEqual({type:'pickup',id:'item-1'});
  expect(itemKeyCommand('KeyE',true,keys,projectItems(s,config,'item-1'))).toBeUndefined();
  s.open=true;expect(itemKeyCommand('KeyE',false,keys,projectItems(s,config,'item-1'))).toBeUndefined();
  expect(itemKeyCommand('KeyE',false,keys,projectItems(s,config))).toBeUndefined();
});
