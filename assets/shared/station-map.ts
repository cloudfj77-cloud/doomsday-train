/** One deterministic layout shared by cooked meshes, collision and save state. */
export type BuildingKind = 'warehouse' | 'workshop' | 'clinic';
export interface StationBuilding { id: BuildingKind; x: number; z: number }
export interface MapBox { x: number; z: number; w: number; d: number; h: number }
export const SITE_ORIGIN = 180;
export const TRAIN_EXIT = { x: -11, z: 4 };
export const MAP_LIMIT = { left: -17, right: 17, back: -25, front: 6 };
export const PLAYER_RADIUS = 0.28;
export const EXPLORER_SPEED = 4;
/** Freight forecourt → service lane → medical compound, with two return loops. */
export const SITE_ROADS: readonly MapBox[] = [
  {x:0,z:-1.5,w:34,d:3,h:0.08},
  {x:0,z:-13,w:3,d:24,h:0.08},
  {x:0,z:-13,w:29,d:2.4,h:0.08},
  {x:-14,z:-12,w:2.4,d:23,h:0.08},
  {x:14,z:-12,w:2.4,d:23,h:0.08},
  {x:0,z:-23,w:29,d:2.4,h:0.08},
];
/** Physical cover stays out of door approaches and the central connector. */
export const SITE_COVER: readonly MapBox[] = [
  {x:-10,z:1,w:3,d:0.7,h:0.8}, {x:7,z:1,w:3,d:0.7,h:0.8},
  {x:-8,z:-12,w:2.5,d:0.65,h:0.9}, {x:8,z:-12,w:2.5,d:0.65,h:0.9},
  {x:-7,z:-18,w:2,d:1,h:1.2}, {x:10,z:-17,w:1,d:2.5,h:1.1},
];
export function roofHidden(seed:number, id:BuildingKind, x:number, z:number):boolean {
  const b=stationBuildings(seed).find(b=>b.id===id)!;
  // Extend to the threshold: the roof cannot cover the player while crossing it.
  return Math.abs(x-b.x)<4.25 && z>b.z-3.25 && z<b.z+3.65;
}
export function stationBuildings(seed: number): StationBuilding[] {
  const shift = Math.abs(seed % 3) - 1;
  return [
    { id: 'warehouse', x: -8 + shift, z: -7 },
    { id: 'workshop', x: 6 + shift, z: -7 },
    { id: 'clinic', x: 3 - shift, z: -19 },
  ];
}
/** All three rooms have a south-facing open doorway and cutaway foreground walls. */
export const ROOM_WALLS: readonly MapBox[] = [
  { x: 0, z: -3, w: 8, d: 0.3, h: 3.3 },
  { x: -4, z: 0, w: 0.3, d: 6, h: 2.7 },
  { x: 4, z: 0, w: 0.3, d: 6, h: 1.1 },
  { x: -2.6, z: 3, w: 2.8, d: 0.3, h: 0.8 },
  { x: 2.6, z: 3, w: 2.8, d: 0.3, h: 0.8 },
];
export const ROOM_FURNITURE: Record<BuildingKind, readonly MapBox[]> = {
  warehouse: [{ x: -2.5, z: -1.6, w: 1.3, d: 1.6, h: 1.4 }, { x: 2.4, z: -1.6, w: 1.8, d: 1.3, h: 1.1 }],
  workshop: [{ x: 0, z: -2.1, w: 4.6, d: 0.8, h: 1 }, { x: -2.6, z: 0, w: 1.2, d: 1.4, h: 1.6 }],
  clinic: [{ x: -2.4, z: -0.7, w: 1.3, d: 2.8, h: 0.7 }, { x: 2.4, z: -1.8, w: 1.8, d: 0.8, h: 1.8 }],
};
export function buildingAt(seed: number, x: number, z: number): StationBuilding | undefined {
  return stationBuildings(seed).find(b => Math.abs(x - b.x) < 3.8 && Math.abs(z - b.z) < 2.8);
}
export function canWalk(seed: number, x: number, z: number): boolean {
  const r = PLAYER_RADIUS;
  if (x < MAP_LIMIT.left + r || x > MAP_LIMIT.right - r || z < MAP_LIMIT.back + r || z > MAP_LIMIT.front - r) return false;
  if (SITE_COVER.some(box => Math.abs(x-box.x)<box.w/2+r && Math.abs(z-box.z)<box.d/2+r)) return false;
  return !stationBuildings(seed).some(b => [...ROOM_WALLS, ...ROOM_FURNITURE[b.id]].some(box =>
    Math.abs(x - b.x - box.x) < box.w / 2 + r && Math.abs(z - b.z - box.z) < box.d / 2 + r));
}
export function canExtract(x: number, z: number): boolean {
  return Math.hypot(x - TRAIN_EXIT.x, z - TRAIN_EXIT.z) < 1.65;
}
/** Small substeps prevent tunnelling; separate axes allow sliding along walls. */
export function walkSite(seed: number, x: number, z: number, dx: number, dz: number) {
  const steps = Math.max(1, Math.ceil(Math.hypot(dx, dz) / 0.15));
  for (let i = 0; i < steps; i++) {
    if (canWalk(seed, x + dx / steps, z)) x += dx / steps;
    if (canWalk(seed, x, z + dz / steps)) z += dz / steps;
  }
  return { x, z };
}
