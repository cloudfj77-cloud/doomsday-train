export type FactionId = 'lower' | 'upper' | 'security' | 'blackmarket' | 'rebel' | 'tourist';

export type ItemCategory =
  | 'weapon'
  | 'armor'
  | 'helmet'
  | 'med'
  | 'water'
  | 'chem'
  | 'intel'
  | 'valuables'
  | 'material'
  | 'attachment';

export type InspectTool = 'visual' | 'litmus' | 'scanner';

export type ContrabandTier = 0 | 1 | 2 | 3 | 4;

export type RarityTier = 0 | 1 | 2 | 3 | 4;

export type ItemLocation =
  | 'shop_stock'
  | 'showcase'
  | 'secret_stash'
  | 'equipped_weapon'
  | 'equipped_helmet'
  | 'equipped_armor'
  | 'raid_backpack'
  | 'safe_box';

export type DamageElement = 'physical' | 'fire' | 'electric' | 'poison';

export interface WeaponStats {
  readonly archetype: 'melee' | 'pistol' | 'smg' | 'rifle' | 'shotgun' | 'sniper' | 'pulse';
  readonly damage: number;
  readonly fireRate: number; // shots per second
  readonly armorPiercing: number; // 0..6
  readonly armorBreak: number; // durability damage per hit
  readonly critRate: number; // headshot probability 0..1
  readonly critMult: number;
  readonly spreadDeg: number;
  readonly pelletCount: number;
  readonly magSize: number;
  readonly reloadTime: number;
  readonly range: number;
  readonly soundRadius: number;
  readonly element: DamageElement;
  readonly bleedChance: number;
}

export interface AttachmentBonus {
  readonly slotType: 'muzzle' | 'mag' | 'grip';
  readonly damageBonus?: number;
  readonly pierceBonus?: number;
  readonly critBonus?: number;
  readonly magBonus?: number;
  readonly spreadMult?: number;
  readonly soundMult?: number;
}

export interface ArmorStats {
  readonly slot: 'helmet' | 'armor';
  readonly armorLevel: number; // 1..6
  readonly maxDurability: number;
}

export interface MedStats {
  readonly healAmount: number;
  readonly stopsBleed: boolean;
  readonly staminaBoost: number;
}

export interface ItemDef {
  readonly id: string;
  readonly name: string;
  readonly category: ItemCategory;
  readonly rarity: RarityTier;
  readonly baseValue: number;
  readonly contrabandTier: ContrabandTier;
  readonly inspectTool: InspectTool;
  readonly desc: string;
  readonly fakeHint: string;
  readonly realHint: string;
  readonly weapon?: WeaponStats;
  readonly attachment?: AttachmentBonus;
  readonly armor?: ArmorStats;
  readonly med?: MedStats;
}

/** Grade index: -2 (劣质 -50%), -1 (瑕疵 -25%), 0 (标准 0%), 1 (精良 +35%), 2 (军规 +80%), 3 (特级 +135%) */
export type ItemGrade = -2 | -1 | 0 | 1 | 2 | 3;

export interface DuckItem {
  uid: string;
  defId: string;
  claimedGrade: ItemGrade;
  actualGrade: ItemGrade;
  exposed: boolean;
  relabeled: boolean;
  inspectTested: Partial<Record<InspectTool, boolean>>;
  stolenHeat: number; // 0..100
  durability: number;
  maxDurability: number;
  location: ItemLocation;
  attachments: DuckItem[];
}

export type GamePhase =
  | 'CAREER_SELECT'
  | 'MORNING'
  | 'SHOP_OPEN'
  | 'AFTERHOURS'
  | 'NIGHT_RAID'
  | 'GAME_OVER'
  | 'VICTORY';

export type ClientIntent =
  | 'BUY'
  | 'SELL'
  | 'INSPECTION'
  | 'WHOLESALE_BUYER'
  | 'LANDLORD'
  | 'STORY';

export type InspectorPersona = 'strict' | 'corrupt' | 'lazy';

export interface WantedTrait {
  readonly id: string;
  readonly label: string;
  readonly visualTag: 'scar' | 'eyepatch' | 'red_scarf' | 'gold_beak' | 'cyber_visor';
}

export interface ShopClient {
  readonly id: string;
  readonly name: string;
  readonly title: string;
  readonly faction: FactionId;
  readonly intent: ClientIntent;
  readonly portraitSeed: number;
  readonly visualTrait?: WantedTrait['visualTag'];
  readonly isWanted: boolean;
  readonly wantedBounty: number;
  readonly isCriminal: boolean;
  readonly isEventDriven: boolean;
  readonly dialogue: string;
  readonly criminalTell?: string;
  readonly offerItem?: DuckItem; // when SELL or STORY gives/requests item
  readonly requestedCategory?: ItemCategory;
  readonly requestedDefId?: string;
  readonly budget: number;
  readonly inspectorPersona?: InspectorPersona;
  readonly bribeCost?: number;
  readonly storyId?: string;
  readonly storyStage?: number;
  bargainLocked: boolean;
  blackmailed: boolean;
  priceMultiplier: number;
  flawDiscountApplied: boolean;
}

export interface MarketEventDef {
  readonly id: string;
  readonly headline: string;
  readonly detail: string;
  readonly category: ItemCategory;
  readonly priceMod: number; // e.g. +0.55 or -0.25
}

export interface MachineDef {
  readonly id: string;
  readonly name: string;
  readonly subtitle: string;
  readonly inputDesc: string;
  readonly outputDesc: string;
  readonly inputFilter: (item: DuckItem) => boolean;
  readonly outputDefId: string;
  readonly outputGrade: ItemGrade;
  readonly powerCost: number;
  readonly unlockUpgradeId?: string;
}

export interface UpgradeDef {
  readonly id: string;
  readonly name: string;
  readonly branch: 'security' | 'industry' | 'raid';
  readonly cost: number;
  readonly desc: string;
  readonly nightlyUpkeep?: number;
}

export type RaidWeather = 'clear_night' | 'storm_rain';

export interface RaidZoneDef {
  readonly id: 'ground_zero' | 'warehouse' | 'farm_town' | 'jlab_37';
  readonly name: string;
  readonly subtitle: string;
  readonly dangerLevel: number;
  readonly weatherPool: readonly RaidWeather[];
  readonly enemyCount: number;
  readonly bossName: string;
  readonly lootCrateCount: number;
  readonly timeLimitSec: number;
  readonly requiredItemDefId?: string;
  readonly lootTags: readonly ItemCategory[];
  readonly rarityWeights: readonly [number, number, number, number, number];
}

export interface CareerDef {
  readonly id: 'apprentice' | 'fence' | 'gunsmith' | 'chemist';
  readonly name: string;
  readonly subtitle: string;
  readonly startingCash: number;
  readonly startingRent: number;
  readonly startingFavor: number;
  readonly repInit: Partial<Record<FactionId, number>>;
  readonly unlockedUpgrades: readonly string[];
  readonly starterItemSpecs: ReadonlyArray<{
    readonly defId: string;
    readonly actualGrade: ItemGrade;
    readonly claimedGrade: ItemGrade;
    readonly heat?: number;
    readonly location?: ItemLocation;
  }>;
}

export interface RaidCrateSlot {
  readonly slotIndex: number;
  readonly item: DuckItem;
  readonly inspectDurationSec: number;
  revealed: boolean;
  taken: boolean;
}

export interface RaidCrateSnapshot {
  readonly crateId: string;
  readonly label: string;
  readonly tier: 'civilian' | 'military' | 'jlab';
  readonly slots: RaidCrateSlot[];
}

export interface RaidEnemyBlip {
  readonly x: number;
  readonly z: number;
  readonly isBoss: boolean;
  readonly alerted: boolean;
  readonly hpRatio: number;
}

export interface RaidFloatingText {
  readonly id: number;
  readonly text: string;
  readonly color: string;
  readonly screenX: number;
  readonly screenY: number;
}

export interface RaidHudSnapshot {
  readonly active: boolean;
  readonly zoneId: RaidZoneDef['id'];
  readonly zoneName: string;
  readonly weather: RaidWeather;
  readonly timeLeftSec: number;
  readonly hp: number;
  readonly maxHp: number;
  readonly stamina: number;
  readonly maxStamina: number;
  readonly bleeding: boolean;
  readonly helmetDurability: number;
  readonly helmetMaxDurability: number;
  readonly helmetLevel: number;
  readonly armorDurability: number;
  readonly armorMaxDurability: number;
  readonly armorLevel: number;
  readonly weaponName: string;
  readonly weaponDamage: number;
  readonly weaponPierce: number;
  readonly ammo: number;
  readonly magSize: number;
  readonly reloading: boolean;
  readonly reloadProgress: number;
  readonly dashCooldown: number;
  readonly medCount: number;
  readonly extractProgress: number; // 0..1
  readonly inExtractZone: boolean;
  readonly nearbyCrateId: string | null;
  readonly playerPos: readonly [number, number];
  readonly arenaHalfSize: number;
  readonly extractPoints: ReadonlyArray<readonly [number, number]>;
  readonly cratePoints: ReadonlyArray<{ readonly id: string; readonly x: number; readonly z: number; readonly opened: boolean }>;
  readonly enemies: readonly RaidEnemyBlip[];
  readonly soundRadiusVisual: number;
  readonly kills: number;
  readonly floatingTexts: readonly RaidFloatingText[];
}

export interface RaidStartPayload {
  readonly zoneId: RaidZoneDef['id'];
  readonly weather: RaidWeather;
  readonly weaponStats: WeaponStats;
  readonly weaponName: string;
  readonly helmetLevel: number;
  readonly helmetDurability: number;
  readonly helmetMaxDurability: number;
  readonly armorLevel: number;
  readonly armorDurability: number;
  readonly armorMaxDurability: number;
  readonly medItems: ReadonlyArray<{ readonly uid: string; readonly name: string; readonly heal: number; readonly stopsBleed: boolean }>;
  readonly seed: number;
}

export interface BunkerPreviewPayload {
  readonly mode: 'bunker';
  readonly helmetLevel: number;
  readonly armorLevel: number;
  readonly weaponArchetype: WeaponStats['archetype'];
  readonly hasClient: boolean;
  readonly clientFaction: FactionId | null;
  readonly clientIsInspector: boolean;
  readonly activeMachineCount: number;
  readonly powerOn: boolean;
}

export const DUCKOV_CHANNEL = 'duckov-pawnshop/bridge';

export type FrontendToEngineMessage =
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'sync_bunker'; readonly payload: BunkerPreviewPayload }
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'start_raid'; readonly payload: RaidStartPayload }
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'abort_raid' }
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'raid_use_med' }
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'raid_mark_crate_opened'; readonly crateId: string }
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'set_paused'; readonly paused: boolean };

export type EngineToFrontendMessage =
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'engine_ready' }
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'raid_hud'; readonly snapshot: RaidHudSnapshot }
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'raid_interact_crate'; readonly crateId: string }
  | { readonly channel: typeof DUCKOV_CHANNEL; readonly kind: 'raid_sfx'; readonly sfx: string }
  | {
      readonly channel: typeof DUCKOV_CHANNEL;
      readonly kind: 'raid_finished';
      readonly extracted: boolean;
      readonly reason: string;
      readonly kills: number;
      readonly bossDefeated: boolean;
      readonly remainingHelmetDurability: number;
      readonly remainingArmorDurability: number;
      readonly usedMedUids: readonly string[];
    };
