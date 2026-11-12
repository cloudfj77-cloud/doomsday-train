import playerRaw from '../../config/gameplay/player.json';
import worldRaw from '../../config/gameplay/world.json';
import physics from '../../config/gameplay/physics.json';
import interactions from '../../config/gameplay/interactions.json';
import drops from '../../config/gameplay/drops.json';
import camera from '../../config/presentation/camera.json';
import palette from '../../config/presentation/palette.json';
import environment from '../../config/presentation/environment.json';
import actor from '../../config/presentation/actor.json';
import inventory from '../../config/presentation/inventory-ui.json';
import visuals from '../../config/presentation/item-visuals.json';
import holding from '../../config/presentation/holding.json';
import outline from '../../config/presentation/drop-outline.json';
import labels from '../../config/presentation/drop-labels.json';
import scrap from '../../config/gameplay/items/scrap.json';
import ammo from '../../config/gameplay/items/ammo.json';
import bandage from '../../config/gameplay/items/bandage.json';
import water from '../../config/gameplay/items/water.json';
import backpack from '../../config/gameplay/items/backpack.json';
import pistol from '../../config/gameplay/items/pistol.json';
import smg from '../../config/gameplay/items/smg.json';
import {validatePlayer} from '../logic/player.ts';
import {validateConfig} from '../logic/world.ts';
import {validateItems} from '../logic/items.ts';
import type {ItemStyle} from '../view/item-presentation.ts';
export function configFields(value:object,required:string[],label:string,optional:string[]=[]){
 if(!value||required.some(k=>!Object.hasOwn(value,k))||Object.keys(value).some(k=>![...required,...optional].includes(k)))throw Error('invalid-config-fields: '+label);
}
export function validateConfigResponsibilities(){
 configFields(worldRaw,['limit','ground','obstacles','surfaces'],'world');configFields(physics,['gravity'],'physics');
 configFields(interactions,['interactionDistance','dropRadius','dropClearance','dropHeight'],'interactions');configFields(drops,['drops'],'drops');
 configFields(camera,['camera','zones'],'camera');configFields(palette,['palette'],'palette');configFields(environment,['layout'],'environment');configFields(actor,['actor'],'actor');
 configFields(inventory,['cellSize','colors','scrollbar'],'inventory-ui');configFields(visuals,['visuals'],'item-visuals');configFields(holding,['holding'],'holding');
 configFields(outline,['outlineWidth','outlineColor'],'drop-outline');configFields(labels,['labelHeight','fontSize','pixelScale','textColor','backgroundColor','gap'],'drop-labels');
 for(const item of [scrap,ammo,bandage,water,backpack,pistol,smg])configFields(item,['id','name','description','tags','size','rotatable','maxStack','weight','value','portable'],'item '+item.id,['stats','slots','container','category','usage','unit','valueQuantity','weapon']);
}
validateConfigResponsibilities();
export const playerConfig=validatePlayer(playerRaw);
export const worldConfig=validateConfig({...worldRaw,...playerConfig.movement,...playerConfig.body,gravity:physics.gravity,start:playerConfig.spawn});
export const sceneConfig={...camera,...palette,...environment,...actor,keys:playerConfig.keys.movement};
const checkedItems=validateItems({definitions:[{...playerConfig.item,container:playerConfig.pockets,slots:playerConfig.slots,stats:playerConfig.baseStats},scrap,ammo,bandage,water,backpack,pistol,smg]},
 {...interactions,...drops,playerDefinition:playerConfig.item.id,backpackDefinition:playerConfig.initialBackpack,backpackSlot:playerConfig.backpackSlot});
export const itemConfig=checkedItems.config,itemWorldConfig=checkedItems.world;
export const itemStyleConfig:ItemStyle={...inventory,...visuals,...holding,keys:playerConfig.keys.inventory,proximity:{...outline,...labels}};
