import playerRaw from '../../config/gameplay/player.json';
import worldRaw from '../../config/gameplay/world.json';
import drops from '../../config/gameplay/drops.json';
import camera from '../../config/presentation/camera.json';
import palette from '../../config/presentation/palette.json';
import environment from '../../config/presentation/environment.json';
import actor from '../../config/presentation/actor.json';
import inventory from '../../config/presentation/inventory-ui.json';
import visuals from '../../config/presentation/item-visuals.json';
import holding from '../../config/presentation/holding.json';
import proximity from '../../config/presentation/drop-feedback.json';
import items from '../../config/gameplay/items.json';
import {validatePlayer} from '../logic/player.ts';
import {validateConfig} from '../logic/world.ts';
import {validateItems} from '../logic/items.ts';
import type {ItemStyle} from '../view/item-presentation.ts';
export function configFields(value:object,required:string[],label:string,optional:string[]=[]){
 if(!value||required.some(k=>!Object.hasOwn(value,k))||Object.keys(value).some(k=>![...required,...optional].includes(k)))throw Error('invalid-config-fields: '+label);
}
export function validateConfigResponsibilities(){
 configFields(worldRaw,['limit','ground','obstacles','surfaces','gravity'],'world');
 configFields(playerRaw.interactions,['interactionDistance','dropRadius','dropClearance','dropHeight'],'interactions');configFields(drops,['drops'],'drops');
 configFields(camera,['camera','zones'],'camera');configFields(palette,['palette'],'palette');configFields(environment,['layout'],'environment');configFields(actor,['actor'],'actor');
 configFields(inventory,['cellSize','colors','scrollbar'],'inventory-ui');configFields(visuals,['visuals'],'item-visuals');configFields(holding,['holding'],'holding');
 configFields(proximity,['outlineWidth','outlineColor','labelHeight','fontSize','pixelScale','textColor','backgroundColor','gap'],'drop-feedback');configFields(items,['definitions'],'items');
 for(const item of items.definitions)configFields(item,['id','name','description','tags','size','rotatable','maxStack','weight','value','portable'],'item '+item.id,['stats','slots','container','category','usage','unit','valueQuantity','weapon']);
}
validateConfigResponsibilities();
export const playerConfig=validatePlayer(playerRaw);
export const worldConfig=validateConfig({...worldRaw,...playerConfig.movement,...playerConfig.body,gravity:worldRaw.gravity,start:playerConfig.spawn});
export const sceneConfig={...camera,...palette,...environment,...actor,keys:playerConfig.keys.movement};
const checkedItems=validateItems({definitions:[{...playerConfig.item,container:playerConfig.pockets,slots:playerConfig.slots,stats:playerConfig.baseStats},...items.definitions]},
 {...playerConfig.interactions,...drops,playerDefinition:playerConfig.item.id,backpackDefinition:playerConfig.initialBackpack,backpackSlot:playerConfig.backpackSlot});
export const itemConfig=checkedItems.config,itemWorldConfig=checkedItems.world;
export const itemStyleConfig:ItemStyle={...inventory,...visuals,...holding,keys:playerConfig.keys.inventory,proximity};
