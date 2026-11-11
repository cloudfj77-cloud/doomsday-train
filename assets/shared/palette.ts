/**
 * 体素废土调色板（参照飞书《三 UI 需求与原型图》主界面：暗夜铁轨、锈红车头、暖黄车灯、灰白丧尸）。
 * Linear-ish RGB in 0..1. Every material in materials.pack.ts is one of these.
 */
export type RGB = readonly [number, number, number];

export const PALETTE = Object.freeze({
  // ground & track
  dirt: [0.11, 0.095, 0.085],
  dirtDark: [0.07, 0.062, 0.058],
  gravel: [0.2, 0.19, 0.18],
  sleeper: [0.17, 0.11, 0.075],
  rail: [0.42, 0.4, 0.38],
  // train
  rust: [0.42, 0.11, 0.07],
  rustDark: [0.24, 0.07, 0.05],
  iron: [0.2, 0.2, 0.21],
  ironDark: [0.1, 0.1, 0.11],
  cream: [0.72, 0.62, 0.48],
  olive: [0.22, 0.24, 0.16],
  wood: [0.32, 0.2, 0.12],
  // lights (emissive)
  lamp: [1.0, 0.72, 0.36],
  neonPink: [1.0, 0.25, 0.55],
  neonCyan: [0.25, 0.9, 1.0],
  // buildings
  concrete: [0.3, 0.3, 0.31],
  concreteDark: [0.17, 0.17, 0.19],
  glass: [0.12, 0.16, 0.2],
  tin: [0.33, 0.31, 0.27],
  tinRust: [0.36, 0.2, 0.12],
  tarp: [0.18, 0.26, 0.3],
  // life
  zombieSkin: [0.52, 0.55, 0.47],
  zombieCloth: [0.3, 0.12, 0.11],
  deadTree: [0.14, 0.11, 0.09],
  leaf: [0.11, 0.25, 0.04],
  leafDark: [0.05, 0.13, 0.02],
  smoke: [0.36, 0.35, 0.34],
} satisfies Record<string, RGB>);

export type PaletteKey = keyof typeof PALETTE;
export const EMISSIVE: readonly PaletteKey[] = ['lamp', 'neonPink', 'neonCyan'];
