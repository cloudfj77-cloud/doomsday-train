import { sceneConfig as scene } from '../config/game.ts';
import { validateCamera } from '../logic/camera.ts';

const positive = (value: number) => Number.isFinite(value) && value > 0;
const vector = (value: number[], size: number) =>
  Array.isArray(value) && value.length === size && value.every(Number.isFinite);

export function validatePresentation(value: typeof scene) {
  const { camera, layout, actor, palette, keys } = value;
  validateCamera(camera,value.zones);
  if (!vector(camera.background,4)) throw Error('invalid-scene-camera');
  if (!Object.values(keys).every(key=>typeof key==='string'&&key.length>0)
    || new Set(Object.values(keys)).size !== 5) throw Error('invalid-scene-keys');
  if (!Object.values(actor).every(positive)) throw Error('invalid-scene-actor');
  if (!Object.values(palette).every(hex => /^#[a-f0-9]{6}$/i.test(hex))
    || !layout.stoneBands.length || !layout.stoneBands.every(ink => Object.hasOwn(palette, ink)))
    throw Error('invalid-scene-palette');
  if (!vector(layout.signal, 2)
    || !vector(layout.rail.center, 2) || !Number.isFinite(layout.rail.yaw) || !positive(layout.rail.length)
    || !Number.isInteger(layout.rail.ties) || layout.rail.ties < 2 || !positive(layout.rockHeight)
    || !Number.isInteger(layout.seed) || !layout.cliffs.every(cliff => vector(cliff, 5) && cliff.slice(2).every(positive))
    || ![layout.gravelCount, layout.grassCount].every(n => Number.isInteger(n) && n >= 0))
    throw Error('invalid-scene-layout');
  return value;
}
