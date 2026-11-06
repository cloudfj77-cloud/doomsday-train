import { quat } from '@forgeax/engine/math';
import { definePack } from '@forgeax/engine/pack/source';
import {
  ANTIALIAS_FXAA,
  Camera,
  DirectionalLight,
  Fog,
  FogModeValue,
  MeshFilter,
  MeshRenderer,
  perspective,
  SKYBOX_MODE_CUBEMAP,
  SkyboxBackground,
  Skylight,
  TONEMAP_ACES_FILMIC,
} from '@forgeax/engine/render';
import { Name, Transform } from '@forgeax/engine/scene';
import type { SceneAsset, SceneEntity } from '@forgeax/engine/types';
import { ok } from '@forgeax/engine/types';
import { assetGuid, guidText, PACKAGE_IDS, SUN_OUTGOING_DIRECTION, voxelGuid } from './shared/asset-refs.ts';
import { CAMERA_POSE } from './shared/protocol.ts';

export const GAME_SCENE_SOURCE_KEY = 'scene/line';

/** Static stage: ground + rails, dusk sky, haze and the fixed 3/4 camera. Moving props are spawned by the train plugin. */
function line(): SceneAsset {
  const entities: Record<string, SceneEntity> = {};
  const add = (key: string, components: Record<string, unknown>) => {
    entities[key] = { components } as SceneEntity;
  };
  const sky = guidText(assetGuid(PACKAGE_IDS.environment, 'environment/dusk'));
  add('ground', {
    Name: { value: 'Ground And Rails' },
    Transform: { pos: [0, 0, 0] },
    MeshFilter: { assetHandle: guidText(voxelGuid('ground')) },
    MeshRenderer: { materials: [] },
  });
  add('sun', {
    Name: { value: 'Dusk Sun' },
    DirectionalLight: { direction: SUN_OUTGOING_DIRECTION, color: [1, 0.72, 0.5], intensity: 1.6, castShadow: true, shadowDistance: 60 },
  });
  add('skylight', { Name: { value: 'Skylight' }, Skylight: { equirect: sky, color: [1, 1, 1], intensity: 0.9 } });
  add('sky-background', { Name: { value: 'Sky' }, SkyboxBackground: { equirect: sky, mode: SKYBOX_MODE_CUBEMAP } });
  add('fog', {
    Name: { value: 'Ash Haze' },
    Fog: { color: [0.09, 0.07, 0.065], density: 0.022, heightFalloff: 0.05, maxOpacity: 0.92, mode: FogModeValue.exp },
  });
  const rotation = quat.fromLookAt(quat.create(), CAMERA_POSE.pos, CAMERA_POSE.target, [0, 1, 0]);
  add('camera', {
    Name: { value: 'Main Camera' },
    Transform: { pos: CAMERA_POSE.pos, quat: [rotation[0] ?? 0, rotation[1] ?? 0, rotation[2] ?? 0, rotation[3] ?? 1] },
    Camera: {
      ...perspective({ fov: (CAMERA_POSE.fovDeg * Math.PI) / 180, aspect: 16 / 9, near: 0.1, far: 260 }),
      tonemap: TONEMAP_ACES_FILMIC,
      antialias: ANTIALIAS_FXAA,
      exposure: 1.15,
      clearColor: [0.03, 0.03, 0.04, 1],
    },
  });
  return { kind: 'scene', entities };
}

export default definePack({
  schemaVersion: '2.0.0',
  packageId: PACKAGE_IDS.scene,
  name: 'Doomsday Train / Scene',
  sceneComponents: [Camera, DirectionalLight, Fog, MeshFilter, MeshRenderer, Name, SkyboxBackground, Skylight, Transform],
  build: () => ok({ [GAME_SCENE_SOURCE_KEY]: line() }),
});
