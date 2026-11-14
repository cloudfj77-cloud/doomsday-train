import {
  createBoxGeometry,
  createCylinderGeometry,
  createSphereGeometry,
  createTorusGeometry,
  meshFromInterleaved,
} from '@forgeax/engine/geometry';
import { definePack } from '@forgeax/engine/pack/source';
import type { AssetGuid, MeshAsset } from '@forgeax/engine/types';
import { ok } from '@forgeax/engine/types';
import { DUCKOV_OBJ_MESHES } from './duckov-meshes-data.ts';
import { assetGuid, PACKAGE_IDS, RUSTED_IRON_MATERIAL_GUID } from './shared/asset-refs.ts';

function withMaterial(mesh: MeshAsset, material: AssetGuid, sourceKey: string): MeshAsset {
  return {
    ...mesh,
    materialSlots: [{ slotName: 'surface', sourceKey, defaultMaterial: material }],
  };
}

function fromObj(key: string, material: AssetGuid, sourceKey: string): MeshAsset {
  const data = DUCKOV_OBJ_MESHES[key]!;
  const verts = new Float32Array(data.v);
  const idx = new Uint16Array(data.i);
  const m = meshFromInterleaved(verts, idx);
  if (!m.ok) throw m.error;
  return withMaterial(m.value, material, sourceKey);
}

export default definePack({
  schemaVersion: '2.0.0',
  packageId: PACKAGE_IDS.geometry,
  name: 'Game 3D / Geometry',
  build: () => {
    const ground = createBoxGeometry(28, 0.4, 28);
    if (!ground.ok) return ground;
    const cube = createBoxGeometry(1.5, 1.5, 1.5);
    if (!cube.ok) return cube;
    const sphere = createSphereGeometry(0.9, 48, 32);
    if (!sphere.ok) return sphere;
    const pedestal = createCylinderGeometry(0.75, 0.9, 1, 32, 1);
    if (!pedestal.ok) return pedestal;
    const torus = createTorusGeometry(1, 0.24, 20, 64);
    if (!torus.ok) return torus;
    const light = createSphereGeometry(0.13, 16, 10);
    if (!light.ok) return light;
    const step = createBoxGeometry(2.4, 0.45, 2.4);
    if (!step.ok) return step;
    const ramp = createBoxGeometry(3.2, 0.3, 4.4);
    if (!ramp.ok) return ramp;
    const goldMat = assetGuid(PACKAGE_IDS.materials, 'material/fantasy-gold');
    const whiteMat = assetGuid(PACKAGE_IDS.materials, 'material/white');
    const signalMat = assetGuid(PACKAGE_IDS.materials, 'material/light');
    const metalMat = assetGuid(PACKAGE_IDS.materials, 'material/metal');
    const paintedMat = assetGuid(PACKAGE_IDS.materials, 'material/painted');
    const azureMat = assetGuid(PACKAGE_IDS.materials, 'material/fantasy-azure');
    return ok({
      'mesh/ground': withMaterial(ground.value, assetGuid(PACKAGE_IDS.materials, 'material/ground'), 'game-3d:ground'),
      'mesh/cube': withMaterial(cube.value, paintedMat, 'game-3d:cube'),
      'mesh/sphere': withMaterial(sphere.value, metalMat, 'game-3d:sphere'),
      'mesh/pedestal': withMaterial(pedestal.value, whiteMat, 'game-3d:pedestal'),
      'mesh/torus': withMaterial(torus.value, metalMat, 'game-3d:torus'),
      'mesh/light': withMaterial(light.value, signalMat, 'game-3d:light'),
      'mesh/step': withMaterial(step.value, assetGuid(PACKAGE_IDS.materials, 'material/obstacle'), 'game-3d:step'),
      'mesh/ramp': withMaterial(ramp.value, RUSTED_IRON_MATERIAL_GUID, 'game-3d:ramp'),
      // Real EscapeFromDuckov 3D OBJ models
      'mesh/duck-body': fromObj('duck_body', whiteMat, 'game-3d:duck-body'),
      'mesh/duck-beak': fromObj('duck_beak', signalMat, 'game-3d:duck-beak'),
      'mesh/duck-foot': fromObj('duck_foot', signalMat, 'game-3d:duck-foot'),
      'mesh/duck-glove': fromObj('duck_glove', goldMat, 'game-3d:duck-glove'),
      'mesh/crown': fromObj('crown', goldMat, 'game-3d:crown'),
      'mesh/wpn-ak74u': fromObj('ak74u', RUSTED_IRON_MATERIAL_GUID, 'game-3d:wpn-ak74u'),
      'mesh/wpn-cubegun': fromObj('cubegun', azureMat, 'game-3d:wpn-cubegun'),
      'mesh/wpn-mosin': fromObj('mosin', metalMat, 'game-3d:wpn-mosin'),
      'mesh/medbag': fromObj('medbag', paintedMat, 'game-3d:medbag'),
      'mesh/moto': fromObj('moto', metalMat, 'game-3d:moto'),
    });
  },
});
