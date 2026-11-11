import scene from '../../config/presentation/scene.json';

const positive = (value: number) => Number.isFinite(value) && value > 0;
const vector = (value: number[], size: number) =>
  Array.isArray(value) && value.length === size && value.every(Number.isFinite);

export function validatePresentation(value: typeof scene) {
  const { camera, layout, actor, palette, keys } = value;
  if (!vector(camera.position, 3) || !vector(camera.target, 3) || !vector(camera.background, 4)
    || !positive(camera.fov) || camera.fov >= Math.PI || !positive(camera.aspect)
    || camera.position.every((v, i) => v === camera.target[i])) throw Error('invalid-scene-camera');
  if (!Object.values(keys).every(key => /^Key[A-Z]$/.test(key))
    || new Set(Object.values(keys)).size !== 4) throw Error('invalid-scene-keys');
  if (!Object.values(actor).every(positive)) throw Error('invalid-scene-actor');
  if (!Object.values(palette).every(hex => /^#[a-f0-9]{6}$/i.test(hex))
    || !layout.stoneBands.length || !layout.stoneBands.every(ink => Object.hasOwn(palette, ink)))
    throw Error('invalid-scene-palette');
  if (!vector(layout.ground, 2) || !layout.ground.every(positive) || !vector(layout.signal, 2)
    || !vector(layout.rail.center, 2) || !Number.isFinite(layout.rail.yaw) || !positive(layout.rail.length)
    || !Number.isInteger(layout.rail.ties) || layout.rail.ties < 2 || !positive(layout.rockHeight)
    || !Number.isInteger(layout.seed) || !layout.cliffs.every(cliff => vector(cliff, 5) && cliff.slice(2).every(positive))
    || ![layout.gravelCount, layout.grassCount].every(n => Number.isInteger(n) && n >= 0))
    throw Error('invalid-scene-layout');
  return value;
}
