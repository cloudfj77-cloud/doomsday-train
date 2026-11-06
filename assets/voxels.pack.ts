/**
 * 体素模型：全部由轴对齐方块拼成（列车、车厢、轨枕、站台建筑、废墟、丧尸）。
 * Each model is one multi-material mesh (one submesh per palette material) so the
 * runtime spawns a single entity per prop. Deterministic: pure functions of a seed.
 */
import { createBoxGeometry } from '@forgeax/engine/geometry';
import { definePack } from '@forgeax/engine/pack/source';
import type { MeshAsset } from '@forgeax/engine/types';
import { ok } from '@forgeax/engine/types';
import { PACKAGE_IDS, slot } from './shared/asset-refs.ts';
import { MeshKit } from './shared/mesh-kit.ts';
import type { PaletteKey } from './shared/palette.ts';

type V3 = readonly [number, number, number];

class Model {
  private readonly kit = new MeshKit();
  private readonly materials: PaletteKey[] = [];
  constructor(private readonly name: string) {}
  /** Axis-aligned box: centre + size. */
  box(material: PaletteKey, center: V3, size: V3): this {
    let index = this.materials.indexOf(material);
    if (index < 0) index = this.materials.push(material) - 1;
    const geometry = createBoxGeometry(Math.max(size[0], 0.01), Math.max(size[1], 0.01), Math.max(size[2], 0.01));
    this.kit.add(geometry, { t: center }, index);
    return this;
  }
  /** Box resting on y = base. */
  block(material: PaletteKey, x: number, base: number, z: number, w: number, h: number, d: number): this {
    return this.box(material, [x, base + h / 2, z], [w, h, d]);
  }
  build(): MeshAsset {
    return this.kit.build(this.materials.map((m) => slot(this.name, m)));
  }
}

function rng(seed: number) {
  let s = seed | 0;
  return () => {
    s = (s + 0x6d2b79f5) | 0;
    let t = Math.imul(s ^ (s >>> 15), s | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
  };
}

// --- Train ------------------------------------------------------------------------------

function bogie(m: Model, x: number): void {
  for (const dx of [-0.45, 0.45]) for (const z of [-0.78, 0.78]) m.box('ironDark', [x + dx, 0.42, z], [0.6, 0.6, 0.18]);
  m.box('iron', [x, 0.42, 0], [1.4, 0.18, 1.4]);
}

function locomotive(): MeshAsset {
  const m = new Model('loco');
  m.box('ironDark', [0, 0.78, 0], [5.4, 0.3, 1.8]);
  for (const x of [-1.9, -0.65, 0.6, 1.85]) for (const z of [-0.82, 0.82]) m.box('ironDark', [x, 0.48, z], [0.78, 0.78, 0.2]);
  for (const x of [-1.9, -0.65, 0.6, 1.85]) for (const z of [-0.94, 0.94]) m.box('rust', [x, 0.48, z], [0.24, 0.24, 0.05]);
  // boiler
  m.box('rust', [0.6, 1.5, 0], [3.4, 1.15, 1.56]);
  m.box('rustDark', [0.6, 2.12, 0], [3.2, 0.12, 1.3]);
  m.box('cream', [0.6, 1.18, 0], [3.42, 0.2, 1.58]);
  m.box('cream', [2.32, 1.5, 0], [0.08, 1.0, 1.2]);
  // V stripe on the nose
  for (let i = 0; i < 4; i += 1) {
    m.box('cream', [2.4, 1.0 + i * 0.1, -0.15 - i * 0.16], [0.1, 0.12, 0.16]);
    m.box('cream', [2.4, 1.0 + i * 0.1, 0.15 + i * 0.16], [0.1, 0.12, 0.16]);
  }
  // cab
  m.box('rustDark', [-1.55, 1.95, 0], [1.75, 2.05, 1.76]);
  m.box('iron', [-1.55, 3.05, 0], [2.0, 0.16, 1.96]);
  m.box('lamp', [-1.4, 2.25, 0], [0.9, 0.5, 1.78]);
  m.box('lamp', [-0.66, 2.25, 0], [0.04, 0.5, 1.1]);
  // chimney, dome, armour plates
  m.box('ironDark', [1.75, 2.45, 0], [0.42, 0.8, 0.42]);
  m.box('iron', [1.75, 2.9, 0], [0.66, 0.16, 0.66]);
  m.box('iron', [0.4, 2.28, 0], [0.6, 0.36, 0.6]);
  m.box('rustDark', [0.9, 2.2, 0.55], [1.6, 0.14, 0.5]);
  m.box('iron', [0.6, 2.0, -0.84], [3.0, 0.06, 0.06]);
  m.box('iron', [0.6, 2.0, 0.84], [3.0, 0.06, 0.06]);
  // front: headlight, buffers, cowcatcher
  m.box('lamp', [2.42, 1.82, 0], [0.16, 0.34, 0.34]);
  m.box('ironDark', [2.36, 1.82, 0], [0.12, 0.46, 0.46]);
  for (const z of [-0.62, 0.62]) m.box('lamp', [2.75, 1.0, z], [0.12, 0.2, 0.2]);
  m.box('rustDark', [2.75, 0.78, 0], [0.5, 0.36, 1.8]);
  m.box('rust', [3.05, 0.55, 0], [0.32, 0.3, 1.5]);
  m.box('rustDark', [3.28, 0.4, 0], [0.24, 0.22, 1.1]);
  for (const z of [-0.6, -0.2, 0.2, 0.6]) m.box('iron', [3.12, 0.6, z], [0.5, 0.06, 0.08]);
  m.box('ironDark', [-2.75, 0.78, 0], [0.2, 0.2, 0.6]);
  return m.build();
}

function carBase(m: Model): void {
  m.box('ironDark', [0, 0.78, 0], [4.4, 0.28, 1.76]);
  bogie(m, -1.45);
  bogie(m, 1.45);
  m.box('ironDark', [2.3, 0.78, 0], [0.2, 0.2, 0.6]);
  m.box('ironDark', [-2.3, 0.78, 0], [0.2, 0.2, 0.6]);
}

function storageCar(): MeshAsset {
  const m = new Model('car-storage');
  carBase(m);
  m.box('olive', [0, 1.72, 0], [4.2, 1.6, 1.64]);
  for (const x of [-1.6, -0.55, 0.55, 1.6]) m.box('ironDark', [x, 1.72, 0], [0.12, 1.62, 1.68]);
  m.box('rust', [0, 1.6, 0], [1.0, 1.25, 1.68]);
  m.box('ironDark', [0, 1.6, 0], [0.06, 1.25, 1.7]);
  m.box('tinRust', [0, 2.6, 0], [4.32, 0.16, 1.8]);
  m.box('wood', [1.2, 2.88, 0.3], [0.7, 0.42, 0.7]);
  m.box('wood', [-1.0, 2.86, -0.25], [0.9, 0.38, 0.6]);
  m.box('tarp', [-1.0, 3.07, -0.25], [0.94, 0.06, 0.64]);
  return m.build();
}

function passengerCar(): MeshAsset {
  const m = new Model('car-passenger');
  carBase(m);
  m.box('cream', [0, 1.32, 0], [4.2, 0.8, 1.64]);
  m.box('rust', [0, 2.12, 0], [4.2, 0.8, 1.64]);
  for (let i = 0; i < 5; i += 1) m.box('lamp', [-1.6 + i * 0.8, 2.1, 0], [0.5, 0.4, 1.66]);
  m.box('rustDark', [2.0, 1.75, 0], [0.18, 1.6, 1.66]);
  m.box('iron', [0, 2.6, 0], [4.32, 0.14, 1.76]);
  m.box('iron', [0, 2.72, 0], [4.1, 0.12, 1.3]);
  m.box('ironDark', [0, 2.84, 0], [3.6, 0.1, 0.6]);
  return m.build();
}

// --- Track & ground ---------------------------------------------------------------------

function sleepers(): MeshAsset {
  const m = new Model('sleepers');
  for (let i = 0; i < 8; i += 1) m.box('sleeper', [-3.5 + i, 0.14, 0], [0.36, 0.12, 2.3]);
  for (let i = 0; i < 6; i += 1) m.box('gravel', [-3.2 + i * 1.3, 0.09, (i % 2 ? 1 : -1) * 1.45], [0.4, 0.1, 0.3]);
  return m.build();
}

function groundAndRails(): MeshAsset {
  const m = new Model('ground');
  m.box('dirt', [0, -0.1, 0], [240, 0.2, 90]);
  m.box('gravel', [0, 0.04, 0], [240, 0.08, 3.4]);
  m.box('dirtDark', [0, 0.01, 6], [240, 0.02, 4]);
  m.box('dirtDark', [0, 0.01, -14], [240, 0.02, 10]);
  for (const z of [-0.62, 0.62]) {
    m.box('rail', [0, 0.25, z], [240, 0.12, 0.1]);
    m.box('iron', [0, 0.2, z], [240, 0.04, 0.22]);
  }
  return m.build();
}

// --- Stations ---------------------------------------------------------------------------

function platform(m: Model, length: number): void {
  m.block('concrete', 0, 0, -2.75, length, 0.55, 2.6);
  m.block('cream', 0, 0.55, -1.55, length, 0.03, 0.22);
  for (let x = -length / 2 + 2; x <= length / 2 - 2; x += 5) {
    m.block('iron', x, 0.55, -3.6, 0.16, 3.0, 0.16);
    m.box('iron', [x, 3.55, -3.35], [0.12, 0.12, 0.6]);
    m.box('lamp', [x, 3.42, -3.1], [0.4, 0.14, 0.34]);
  }
}

function tower(m: Model, x: number, z: number, w: number, d: number, floors: number, accent: PaletteKey, rand: () => number): void {
  const h = floors * 1.4;
  m.block('concreteDark', x, 0, z, w, h, d);
  for (let f = 0; f < floors; f += 1) {
    const y = 0.7 + f * 1.4;
    for (let k = 0; k < Math.floor(w / 0.9); k += 1) {
      const lit = rand() < 0.42;
      m.box(lit ? 'lamp' : 'glass', [x - w / 2 + 0.45 + k * 0.9, y, z + d / 2 + 0.01], [0.5, 0.6, 0.04]);
    }
  }
  m.block('concrete', x, h, z, w + 0.3, 0.3, d + 0.3);
  if (rand() < 0.7) m.block('ironDark', x + w * 0.2, h + 0.3, z, 0.3, 1.6, 0.3);
  m.box(accent, [x, h * 0.62, z + d / 2 + 0.12], [Math.min(w * 0.7, 2.6), 0.5, 0.1]);
}

function richStation(seed: number): MeshAsset {
  const m = new Model(`station-rich-${seed}`);
  const rand = rng(seed * 977 + 11);
  platform(m, 18);
  // gate canopy with neon
  m.block('ironDark', -3, 0.55, -2.2, 0.3, 3.4, 0.3);
  m.block('ironDark', 3, 0.55, -2.2, 0.3, 3.4, 0.3);
  m.block('concrete', 0, 3.95, -2.6, 7.2, 0.35, 2.2);
  m.box(seed % 2 ? 'neonCyan' : 'neonPink', [0, 3.75, -1.48], [5.0, 0.18, 0.06]);
  // skyline
  const xs = [-11, -6, -0.5, 5, 10.5];
  xs.forEach((x, i) => {
    const w = 3 + rand() * 1.6;
    tower(m, x + (rand() - 0.5), -9 - rand() * 3, w, 3 + rand() * 1.5, 4 + Math.floor(rand() * 5) + (i === 2 ? 2 : 0), rand() < 0.5 ? 'neonPink' : 'neonCyan', rand);
  });
  // wall + searchlight posts
  m.block('concrete', 0, 0, -5.6, 26, 1.6, 0.4);
  for (const x of [-12, -4, 4, 12]) m.block('ironDark', x, 0, -5.6, 0.5, 2.2, 0.5);
  return m.build();
}

function shack(m: Model, x: number, z: number, rand: () => number): void {
  const w = 2.4 + rand() * 1.4;
  const d = 2.2 + rand() * 1.2;
  const h = 1.9 + rand() * 1.2;
  const wall: PaletteKey = rand() < 0.5 ? 'tin' : 'tinRust';
  m.block(wall, x, 0, z, w, h, d);
  m.block('ironDark', x - w * 0.15, 0, z + d / 2 + 0.01, 0.8, 1.4, 0.04);
  if (rand() < 0.6) m.box('lamp', [x + w * 0.25, h * 0.6, z + d / 2 + 0.02], [0.5, 0.4, 0.04]);
  // stepped lean-to roof
  for (let i = 0; i < 3; i += 1) m.block(rand() < 0.3 ? 'tarp' : 'tinRust', x, h + i * 0.18, z - i * 0.25, w + 0.4 - i * 0.2, 0.18, d + 0.3 - i * 0.5);
  if (rand() < 0.5) {
    m.block('ironDark', x + w * 0.3, h, z, 0.3, 1.0, 0.3);
  }
  if (rand() < 0.5) m.block(wall === 'tin' ? 'tinRust' : 'tin', x + (rand() - 0.5) * w * 0.4, h + 0.54, z, w * 0.6, 1.4, d * 0.6);
}

function slumStation(seed: number): MeshAsset {
  const m = new Model(`station-slum-${seed}`);
  const rand = rng(seed * 433 + 7);
  // low wooden platform
  m.block('wood', 0, 0, -2.75, 16, 0.45, 2.6);
  for (let x = -7.5; x <= 7.5; x += 1.5) m.block('sleeper', x, 0.45, -1.5, 0.12, 0.02, 0.12);
  for (const x of [-6, 0, 6]) {
    m.block('wood', x, 0.45, -3.7, 0.18, 2.8, 0.18);
    m.box('lamp', [x, 3.1, -3.4], [0.3, 0.3, 0.3]);
  }
  for (let i = 0; i < 7; i += 1) shack(m, -12 + i * 4 + (rand() - 0.5) * 1.5, -6.5 - rand() * 4, rand);
  // containers & barrels
  m.block('tinRust', 9, 0, -4.8, 3.2, 1.4, 1.5);
  m.block('olive', 9.4, 1.4, -4.8, 2.6, 1.3, 1.4);
  for (let i = 0; i < 4; i += 1) m.block(rand() < 0.5 ? 'rust' : 'iron', -8 + i * 0.7, 0.45, -3.6, 0.5, 0.8, 0.5);
  m.block('tarp', -3, 2.6, -3.0, 4.6, 0.12, 2.4);
  m.block('wood', -5, 0.45, -2.2, 0.15, 2.15, 0.15);
  m.block('wood', -1, 0.45, -2.2, 0.15, 2.15, 0.15);
  return m.build();
}

// --- Scenery ----------------------------------------------------------------------------

function ruin(seed: number): MeshAsset {
  const m = new Model(`ruin-${seed}`);
  const rand = rng(seed * 131 + 5);
  const w = 4 + rand() * 3;
  for (let x = 0; x < w; x += 0.5) {
    const h = 1 + rand() * (seed % 2 ? 4 : 2.5) * Math.sin((x / w) * Math.PI);
    m.block(rand() < 0.8 ? 'concreteDark' : 'concrete', x - w / 2, 0, 0, 0.5, Math.max(0.4, h), 0.6);
  }
  for (let i = 0; i < 6; i += 1) m.block('concreteDark', (rand() - 0.5) * w, 0, 0.6 + rand() * 1.2, 0.5, 0.3, 0.5);
  if (seed % 2) m.block('ironDark', w * 0.2, 0, 0, 0.12, 4.5, 0.12);
  return m.build();
}

function wreck(): MeshAsset {
  const m = new Model('wreck');
  m.block('rustDark', 0, 0.25, 0, 3.4, 0.7, 1.6);
  m.block('rust', -0.2, 0.95, 0, 1.8, 0.6, 1.4);
  m.box('glass', [0.75, 1.2, 0], [0.06, 0.4, 1.2]);
  for (const x of [-1.1, 1.1]) for (const z of [-0.8, 0.8]) m.box('ironDark', [x, 0.25, z], [0.5, 0.5, 0.2]);
  return m.build();
}

function deadTree(): MeshAsset {
  const m = new Model('tree');
  m.block('deadTree', 0, 0, 0, 0.36, 3.2, 0.36);
  m.box('deadTree', [0.5, 2.3, 0], [0.9, 0.22, 0.22]);
  m.box('deadTree', [0.9, 2.7, 0], [0.22, 0.8, 0.22]);
  m.box('deadTree', [-0.45, 2.8, 0.1], [0.8, 0.2, 0.2]);
  m.box('deadTree', [-0.8, 3.15, 0.1], [0.2, 0.6, 0.2]);
  m.box('deadTree', [0, 3.3, -0.35], [0.2, 0.2, 0.7]);
  return m.build();
}

function pole(): MeshAsset {
  const m = new Model('pole');
  m.block('wood', 0, 0, 0, 0.22, 4.6, 0.22);
  m.box('wood', [0, 4.2, 0], [0.12, 0.12, 1.6]);
  for (const z of [-0.65, 0.65]) m.box('cream', [0, 4.32, z], [0.1, 0.12, 0.1]);
  return m.build();
}

function zombie(variant: number): MeshAsset {
  const m = new Model(`zombie-${variant}`);
  const cloth: PaletteKey = variant === 0 ? 'zombieCloth' : variant === 1 ? 'tarp' : 'olive';
  m.block('ironDark', -0.13, 0, 0, 0.22, 0.75, 0.24);
  m.block('ironDark', 0.13, 0, 0.05, 0.22, 0.72, 0.24);
  m.block(cloth, 0, 0.75, 0, 0.58, 0.72, 0.34);
  m.box('zombieSkin', [0, 1.72, 0.04], [0.42, 0.42, 0.42]);
  m.box('ironDark', [-0.09, 1.76, 0.26], [0.08, 0.06, 0.02]);
  m.box('ironDark', [0.09, 1.76, 0.26], [0.08, 0.06, 0.02]);
  // arms reaching forward (+Z is its face)
  m.box(cloth, [-0.36, 1.32, 0.16], [0.16, 0.16, 0.5]);
  m.box('zombieSkin', [-0.36, 1.3, 0.5], [0.15, 0.15, 0.22]);
  m.box(cloth, [0.36, 1.25, 0.2], [0.16, 0.16, 0.56]);
  m.box('zombieSkin', [0.36, 1.22, 0.56], [0.15, 0.15, 0.22]);
  return m.build();
}

function smokePuff(): MeshAsset {
  const m = new Model('smoke');
  m.box('smoke', [0, 0, 0], [0.5, 0.5, 0.5]);
  m.box('smoke', [0.2, 0.18, 0.1], [0.32, 0.32, 0.32]);
  return m.build();
}

export default definePack({
  schemaVersion: '2.0.0',
  packageId: PACKAGE_IDS.voxels,
  name: 'Doomsday Train / Voxels',
  build: () =>
    ok({
      'mesh/loco': locomotive(),
      'mesh/car-storage': storageCar(),
      'mesh/car-passenger': passengerCar(),
      'mesh/sleepers': sleepers(),
      'mesh/ground': groundAndRails(),
      'mesh/station-rich-0': richStation(0),
      'mesh/station-rich-1': richStation(1),
      'mesh/station-slum-0': slumStation(0),
      'mesh/station-slum-1': slumStation(1),
      'mesh/ruin-0': ruin(0),
      'mesh/ruin-1': ruin(1),
      'mesh/wreck': wreck(),
      'mesh/tree': deadTree(),
      'mesh/pole': pole(),
      'mesh/zombie-0': zombie(0),
      'mesh/zombie-1': zombie(1),
      'mesh/zombie-2': zombie(2),
      'mesh/smoke': smokePuff(),
    }),
});
