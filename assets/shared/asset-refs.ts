import { AssetGuid } from '@forgeax/engine/pack/guid';
import { definePackageId } from '@forgeax/engine/pack/source';
import type { PackageId } from '@forgeax/engine/pack/source';
import type { AssetGuid as AssetGuidType, MeshMaterialSlot } from '@forgeax/engine/types';

export function guidText(value: AssetGuidType): string {
  return AssetGuid.format(value);
}

export function assetGuid(packageId: PackageId, sourceKey: string): AssetGuidType {
  return AssetGuid.derive(packageId, sourceKey);
}

/** One package namespace per authored Pack; GUID = UUIDv5(packageId, sourceKey). */
export const PACKAGE_IDS = Object.freeze({
  environment: definePackageId('019fb7ce-3100-7000-8000-000000000000'),
  materials: definePackageId('019fb7ce-3200-7000-8000-000000000000'),
  voxels: definePackageId('019fb7ce-3300-7000-8000-000000000000'),
  scene: definePackageId('019fb7ce-3400-7000-8000-000000000000'),
  ui: definePackageId('019fb7ce-3600-7000-8000-000000000000'),
  trainPlugin: definePackageId('019fb7ce-4100-7000-8000-000000000000'),
});

/** Palette keys are camelCase; pack source keys must be kebab-case. */
export const kebab = (key: string): string => key.replace(/[A-Z]/g, (c) => `-${c.toLowerCase()}`);
export const materialGuid = (key: string): AssetGuidType => assetGuid(PACKAGE_IDS.materials, `material/${kebab(key)}`);
export const voxelGuid = (key: string): AssetGuidType => assetGuid(PACKAGE_IDS.voxels, `mesh/${key}`);
export const pluginGuid = (packageId: PackageId, key: string): string => guidText(assetGuid(packageId, `plugin/${key}`));

/** Direction stored by DirectionalLight (outgoing): a low, smoky dusk sun from the right. */
export const SUN_OUTGOING_DIRECTION = [-0.55, -0.62, -0.56] as const;

/** Mesh material slot; source keys must be unique per mesh, so derive them from mesh + slot. */
export const slot = (mesh: string, material: string): MeshMaterialSlot => ({
  slotName: material,
  sourceKey: `dt:${mesh}:${kebab(material)}`,
  defaultMaterial: materialGuid(material),
});
