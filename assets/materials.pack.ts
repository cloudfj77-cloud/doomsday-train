import { definePack } from '@forgeax/engine/pack/source';
import { Materials } from '@forgeax/engine/render';
import type { MaterialRootAsset } from '@forgeax/engine/types';
import { ok } from '@forgeax/engine/types';
import { kebab, PACKAGE_IDS } from './shared/asset-refs.ts';
import { EMISSIVE, PALETTE, type PaletteKey } from './shared/palette.ts';

/** One flat, rough standard material per palette colour: the voxel look comes from geometry, not textures. */
function paletteMaterial(key: PaletteKey): MaterialRootAsset {
  const c = PALETTE[key];
  if (EMISSIVE.includes(key)) {
    return Materials.standard({ baseColor: [c[0], c[1], c[2], 1], emissive: [c[0], c[1], c[2]], emissiveIntensity: 4, metallic: 0, roughness: 0.4 });
  }
  const metal = key === 'rail' || key === 'iron' || key === 'ironDark';
  const glossy = key === 'glass';
  return Materials.standard({
    baseColor: [c[0], c[1], c[2], 1],
    metallic: metal ? 0.55 : 0,
    roughness: glossy ? 0.25 : metal ? 0.5 : 0.88,
  });
}

export default definePack({
  schemaVersion: '2.0.0',
  packageId: PACKAGE_IDS.materials,
  name: 'Doomsday Train / Materials',
  build: () =>
    ok(Object.fromEntries((Object.keys(PALETTE) as PaletteKey[]).map((key) => [`material/${kebab(key)}`, paletteMaterial(key)]))),
});
