import {
  CONTRABAND_PREMIUM,
  FACTION_META,
  GRADE_MODIFIERS,
  ITEM_DEFS,
  RAID_ZONES,
} from './catalog.ts';
import type {
  DamageElement,
  DuckItem,
  FactionId,
  InspectTool,
  ItemGrade,
  ItemLocation,
  MarketEventDef,
  RaidWeather,
  RarityTier,
  RaidZoneDef,
  WeaponStats,
} from './types.ts';

export function clamp(val: number, min: number, max: number): number {
  return Math.min(max, Math.max(min, val));
}

let uidSeq = 1;
export function nextUid(prefix = 'itm'): string {
  uidSeq += 1;
  return `${prefix}_${Date.now().toString(36)}_${uidSeq.toString(36)}`;
}

export function createDuckItem(
  defId: string,
  options: {
    actualGrade?: ItemGrade;
    claimedGrade?: ItemGrade;
    exposed?: boolean;
    stolenHeat?: number;
    location?: ItemLocation;
    durabilityRatio?: number;
  } = {},
): DuckItem {
  const def = ITEM_DEFS[defId];
  if (!def) throw new Error(`Unknown ItemDef: ${defId}`);
  const actualGrade = options.actualGrade ?? 0;
  const claimedGrade = options.claimedGrade ?? actualGrade;
  const maxDurability = def.armor?.maxDurability ?? (def.weapon ? 100 : 100);
  const ratio = clamp(options.durabilityRatio ?? 1, 0.15, 1);
  return {
    uid: nextUid('duck'),
    defId,
    actualGrade,
    claimedGrade,
    exposed: options.exposed ?? false,
    relabeled: false,
    inspectTested: {},
    stolenHeat: clamp(Math.round(options.stolenHeat ?? 0), 0, 100),
    durability: Math.round(maxDurability * ratio),
    maxDurability,
    location: options.location ?? 'shop_stock',
    attachments: [],
  };
}

export interface PriceContext {
  readonly isSellingToClient: boolean;
  readonly clientFaction?: FactionId;
  readonly factionRep?: number;
  readonly activeEvents?: readonly MarketEventDef[];
  readonly retailMarkup?: number;
  readonly finalMultiplier?: number;
  readonly flawDiscountApplied?: boolean;
}

/**
 * Three-stage staged value formula adapted from Probably Stolen `GameItem.ComposeStagedValue`:
 * 1) Base * (1 + visibleGradeMod) * durabilityFactor
 * 2) * (1 + sum_of_additive_market_modifiers) + attachmentValues
 * 3) * finalMultiplier (bargain / flaw penalty)
 */
export function computeItemPrice(item: DuckItem, ctx: PriceContext): number {
  const def = ITEM_DEFS[item.defId];
  if (!def) return 1;
  const visibleGrade = item.exposed ? item.actualGrade : item.claimedGrade;
  const gradeMod = GRADE_MODIFIERS[visibleGrade] ?? 0;
  const hasDurability = Boolean(def.weapon || def.armor);
  const durFactor = hasDurability && item.maxDurability > 0
    ? 0.45 + 0.55 * clamp(item.durability / item.maxDurability, 0, 1)
    : 1;

  const retailMarkup = ctx.isSellingToClient ? (ctx.retailMarkup ?? 0.15) : -0.18;
  const contrabandMod = CONTRABAND_PREMIUM[def.contrabandTier] ?? 0;
  const eventMod = (ctx.activeEvents ?? [])
    .filter((ev) => ev.category === def.category)
    .reduce((acc, ev) => acc + ev.priceMod, 0);
  const touristMod = ctx.isSellingToClient && ctx.clientFaction === 'tourist' ? 0.15 : 0;
  const blackmarketFenceBonus =
    ctx.isSellingToClient &&
    ctx.clientFaction === 'blackmarket' &&
    (ctx.factionRep ?? 0) >= 20 &&
    item.stolenHeat > 0
      ? 0.2
      : 0;
  const rebelArmsBonus =
    ctx.isSellingToClient &&
    ctx.clientFaction === 'rebel' &&
    (ctx.factionRep ?? 0) >= 20 &&
    (def.category === 'weapon' || def.category === 'med')
      ? 0.2
      : 0;
  const fenceBuyDiscount = !ctx.isSellingToClient && item.stolenHeat > 0 ? -0.2 : 0;
  const lowerSellerFavor =
    !ctx.isSellingToClient && ctx.clientFaction === 'lower' && (ctx.factionRep ?? 0) >= 50
      ? -0.1
      : 0;

  const additiveMarket = Math.max(
    -0.65,
    retailMarkup +
      contrabandMod +
      eventMod +
      touristMod +
      blackmarketFenceBonus +
      rebelArmsBonus +
      fenceBuyDiscount +
      lowerSellerFavor,
  );

  const attachmentSum = item.attachments.reduce((acc, sub) => acc + computeItemPrice(sub, ctx), 0);
  const flawMult = !ctx.isSellingToClient && ctx.flawDiscountApplied ? 0.85 : 1.0;
  const finalMult = (ctx.finalMultiplier ?? 1.0) * flawMult;

  const staged = (def.baseValue * (1 + gradeMod) * durFactor * (1 + additiveMarket) + attachmentSum) * finalMult;
  return Math.max(5, Math.round(staged));
}

export interface BargainOdds {
  readonly acceptPct: number;
  readonly stayPct: number;
  readonly leavePct: number;
  readonly repDeltaOnDeal: number;
}

/**
 * Public-probability bargaining formula from Probably Stolen `BargainUIManager`:
 * A = 100 - 2.0*p + repBonus + eventBonus + terminalBonus
 * B = 100 - 1.2*p + repBonus + eventBonus + terminalBonus
 */
export function computeBargainOdds(params: {
  readonly markupPct: number; // positive = greedy (+10, +25, +50); negative = generous (-10, -25)
  readonly faction: FactionId;
  readonly factionRep: number;
  readonly basePrice: number;
  readonly isEventDriven: boolean;
  readonly hasTradeTerminal: boolean;
}): BargainOdds {
  const { markupPct, faction, factionRep, basePrice, isEventDriven, hasTradeTerminal } = params;
  const factionMeta = FACTION_META[faction];
  if (markupPct <= 0) {
    const generousBoost = markupPct === 0 ? 0.65 : 0.85 + Math.abs(markupPct) / 18;
    const repGain =
      factionMeta.repMultiplier === 0
        ? 0
        : Math.max(1, Math.round((basePrice * generousBoost * factionMeta.repMultiplier) / 140));
    return { acceptPct: 100, stayPct: 0, leavePct: 0, repDeltaOnDeal: repGain };
  }

  const repBonus = factionRep >= 0 ? factionRep / 6 : (factionRep / 6) * 2;
  const eventBonus = isEventDriven ? 15 : 0;
  const terminalBonus = hasTradeTerminal ? 8 : 0;

  const lineA = clamp(Math.round(100 - 2.0 * markupPct + repBonus + eventBonus + terminalBonus), 0, 98);
  const lineB = clamp(Math.round(100 - 1.2 * markupPct + repBonus + eventBonus + terminalBonus), lineA, 99);

  const greedFactor = -(0.5 + markupPct / 16);
  const repLoss =
    factionMeta.repMultiplier === 0
      ? 0
      : Math.min(-1, Math.round((basePrice * greedFactor * factionMeta.repMultiplier) / 160));

  return {
    acceptPct: lineA,
    stayPct: lineB - lineA,
    leavePct: 100 - lineB,
    repDeltaOnDeal: repLoss,
  };
}

export function resolveBargainRoll(
  odds: BargainOdds,
  roll0To99: number,
): 'accept' | 'reject_stay' | 'walk_away' {
  if (roll0To99 < odds.acceptPct) return 'accept';
  if (roll0To99 < odds.acceptPct + odds.stayPct) return 'reject_stay';
  return 'walk_away';
}

export function inspectItemWithTool(
  item: DuckItem,
  tool: InspectTool,
): { readonly matchedTool: boolean; readonly exposedDiscrepancy: boolean; readonly message: string } {
  const def = ITEM_DEFS[item.defId];
  item.inspectTested[tool] = true;
  if (!def) return { matchedTool: false, exposedDiscrepancy: false, message: '未知物品' };
  if (def.inspectTool !== tool) {
    const toolNames: Record<InspectTool, string> = {
      visual: '紫外放大镜',
      litmus: '化学试纸',
      scanner: '频谱扫描仪',
    };
    return {
      matchedTool: false,
      exposedDiscrepancy: false,
      message: `该品类需使用【${toolNames[def.inspectTool]}】才能测出深层真伪！`,
    };
  }
  const hadDiscrepancy = item.claimedGrade !== item.actualGrade;
  item.exposed = true;
  if (hadDiscrepancy && item.actualGrade < item.claimedGrade) {
    return {
      matchedTool: true,
      exposedDiscrepancy: true,
      message: `破绽揭穿！${def.fakeHint} —— 真实品质仅为「${item.actualGrade}档」，可狠狠压价！`,
    };
  }
  if (hadDiscrepancy && item.actualGrade > item.claimedGrade) {
    return {
      matchedTool: true,
      exposedDiscrepancy: false,
      message: `意外捡漏！卖家不识货，${def.realHint}，真实品质更高！`,
    };
  }
  return {
    matchedTool: true,
    exposedDiscrepancy: false,
    message: `检验无误：${def.realHint}，货版相符。`,
  };
}

/**
 * Compute final effective weapon stats including nested slot attachments + item actualGrade!
 * Fuses Duckov `ItemAgent_Gun` Stat+Modifier tree with Probably Stolen `actualGrade`!
 */
export function computeEffectiveWeaponStats(weaponItem: DuckItem | undefined): WeaponStats {
  const fallback = ITEM_DEFS.wpn_cleaver!.weapon!;
  if (!weaponItem) return fallback;
  const base = ITEM_DEFS[weaponItem.defId]?.weapon ?? fallback;
  const gradeScale = 1 + weaponItem.actualGrade * 0.08;
  let damage = Math.round(base.damage * gradeScale);
  let armorPiercing = base.armorPiercing + Math.max(0, weaponItem.actualGrade * 0.25);
  let critRate = base.critRate;
  let magSize = base.magSize;
  let spreadDeg = base.spreadDeg;
  let soundRadius = base.soundRadius;

  for (const att of weaponItem.attachments) {
    const bonus = ITEM_DEFS[att.defId]?.attachment;
    if (!bonus) continue;
    damage += bonus.damageBonus ?? 0;
    armorPiercing += bonus.pierceBonus ?? 0;
    critRate += bonus.critBonus ?? 0;
    magSize += bonus.magBonus ?? 0;
    spreadDeg *= bonus.spreadMult ?? 1;
    soundRadius *= bonus.soundMult ?? 1;
  }

  return {
    ...base,
    damage: Math.max(4, damage),
    armorPiercing: Number(armorPiercing.toFixed(2)),
    critRate: clamp(critRate, 0.05, 0.85),
    magSize: Math.max(1, magSize),
    spreadDeg: Math.max(0.5, Number(spreadDeg.toFixed(2))),
    soundRadius: Number(soundRadius.toFixed(1)),
  };
}

export interface CombatHitInput {
  readonly baseDamage: number;
  readonly critRate: number;
  readonly critMult: number;
  readonly armorPiercing: number;
  readonly armorBreak: number;
  readonly element: DamageElement;
  readonly bleedChance: number;
  readonly weather: RaidWeather;
  readonly helmetLevel: number;
  readonly helmetDurability: number;
  readonly bodyArmorLevel: number;
  readonly bodyArmorDurability: number;
  readonly critRoll: number; // 0..1
  readonly bleedRoll: number; // 0..1
}

export interface CombatHitResult {
  readonly finalDamage: number;
  readonly isHeadshotCrit: boolean;
  readonly armorReductionRatio: number;
  readonly newHelmetDurability: number;
  readonly newBodyArmorDurability: number;
  readonly inflictedBleed: boolean;
}

/**
 * Exact Duckov `Health.cs:340-420` armor & headshot pipeline:
 * - Crit == Headshot (checks Helmet armor instead of Body armor)
 * - mult = 2 / (clamp(armor - armorPiercing, 0, 999) + 2)
 * - Rain weather: fire * 0.85, electric * 1.20
 */
export function computeArmorDamage(input: CombatHitInput): CombatHitResult {
  const isHeadshotCrit = input.critRoll < input.critRate;
  let dmg = input.baseDamage * (isHeadshotCrit ? input.critMult : 1.0);

  let newHelmetDur = input.helmetDurability;
  let newBodyDur = input.bodyArmorDurability;
  let activeArmor = 0;

  if (isHeadshotCrit) {
    if (newHelmetDur > 0 && input.helmetLevel > 0) {
      activeArmor = input.helmetLevel;
      newHelmetDur = Math.max(0, newHelmetDur - input.armorBreak);
    }
  } else if (newBodyDur > 0 && input.bodyArmorLevel > 0) {
    activeArmor = input.bodyArmorLevel;
    newBodyDur = Math.max(0, newBodyDur - input.armorBreak);
  }

  const effectiveArmor = clamp(activeArmor - input.armorPiercing, 0, 999);
  const armorMult = 2 / (effectiveArmor + 2);
  dmg *= armorMult;

  if (input.weather === 'storm_rain') {
    if (input.element === 'fire') dmg *= 0.85;
    else if (input.element === 'electric') dmg *= 1.2;
  }

  const finalDamage = Math.max(1, Math.round(dmg));
  const inflictedBleed = input.bleedRoll < input.bleedChance * armorMult;

  return {
    finalDamage,
    isHeadshotCrit,
    armorReductionRatio: Number((1 - armorMult).toFixed(2)),
    newHelmetDurability: Number(newHelmetDur.toFixed(1)),
    newBodyArmorDurability: Number(newBodyDur.toFixed(1)),
    inflictedBleed,
  };
}

/** Duckov rarity-scaled loot inspection duration (creates anticipation for purple/gold drops!) */
export function computeLootInspectDuration(rarity: RarityTier): number {
  const table: readonly [number, number, number, number, number] = [0.45, 0.85, 1.35, 2.0, 2.8];
  return table[rarity] ?? 0.8;
}

/** Generate loot item for a Raid Zone using Tag weights x Rarity weights */
export function rollRaidLootItem(zoneId: RaidZoneDef['id'], randomFn: () => number): DuckItem {
  const zone = RAID_ZONES.find((z) => z.id === zoneId) ?? RAID_ZONES[0]!;
  const weights = zone.rarityWeights;
  const totalWeight = weights.reduce((a, b) => a + b, 0);
  let pick = randomFn() * totalWeight;
  let targetRarity: RarityTier = 0;
  for (let r = 0; r < weights.length; r += 1) {
    pick -= weights[r]!;
    if (pick <= 0) {
      targetRarity = r as RarityTier;
      break;
    }
  }

  const allDefs = Object.values(ITEM_DEFS);
  let candidates = allDefs.filter(
    (d) => zone.lootTags.includes(d.category) && d.rarity === targetRarity && d.id !== 'int_blackfile',
  );
  if (candidates.length === 0) {
    candidates = allDefs.filter((d) => d.rarity === targetRarity && d.id !== 'int_blackfile');
  }
  if (candidates.length === 0) {
    candidates = [ITEM_DEFS.med_bandage!];
  }

  const chosen = candidates[Math.floor(randomFn() * candidates.length)] ?? candidates[0]!;
  const grades: readonly ItemGrade[] = [-1, 0, 0, 1, 2, 3];
  const gradeIdx = clamp(Math.floor(randomFn() * 3) + Math.floor(targetRarity * 0.8), 0, grades.length - 1);
  const actualGrade = grades[gradeIdx] ?? 0;
  const stolenHeat = chosen.contrabandTier >= 2 ? Math.round(20 + randomFn() * 45) : 0;

  return createDuckItem(chosen.id, {
    actualGrade,
    claimedGrade: actualGrade,
    exposed: true,
    stolenHeat,
    location: 'raid_backpack',
    durabilityRatio: 0.65 + randomFn() * 0.35,
  });
}

export function computeEvidenceDailyGain(crimeScore: number): number {
  if (crimeScore <= 0) return 0;
  if (crimeScore < 100) return 4;
  if (crimeScore < 300) return 9;
  if (crimeScore < 700) return 16;
  return 28;
}
