/**
 * Frontend → Engine presentation protocol over GameHost.port.
 * The frontend owns the economy simulation and every player-visible word; the
 * Engine only stages the 3D view (train composition, station, travel animation).
 */
import type { CarKind, Region } from '../sim/data.ts';

export const VIEW_CHANNEL = 'doomsday/view';
export const ENGINE_CHANNEL = 'doomsday/engine';

export type ViewMode = 'title' | 'station' | 'travel' | 'exploration';

export interface ViewMessage {
  readonly channel: typeof VIEW_CHANNEL;
  readonly mode: ViewMode;
  /** Region of the station the train stands at (station) or is heading to (travel). */
  readonly region: Region;
  /** Variant seed for the station model (stable per station id). */
  readonly stationSeed: number;
  readonly cars: readonly CarKind[];
  /** Travel animation length in milliseconds. */
  readonly travelMs: number;
  /** Increments for each new travel, so re-sent messages do not restart the animation. */
  readonly travelSerial: number;
  /** Passengers on board (lit windows / silhouettes) */
  readonly riders: number;
  readonly explorer?: { x: number; z: number; surveyed: readonly string[] };
  /** Read-only economy snapshot for the Engine-owned acceptance projection. */
  readonly simulation?: string;
}

/** Engine → Frontend: presentation facts for QA. */
export interface EngineMessage {
  readonly channel: typeof ENGINE_CHANNEL;
  readonly kind: 'status';
  readonly ready: boolean;
  readonly mode: ViewMode;
  readonly travelling: boolean;
  readonly cars: number;
  readonly stationSeed: number;
  readonly frames: number;
  readonly explorer?: { x: number; z: number; surveyed: readonly string[]; building?: string };
  readonly extractRequested?: boolean;
  readonly controller?: {action: string; stamina: number; cooldown: number};
}

export function isViewMessage(value: unknown): value is ViewMessage {
  return !!value && typeof value === 'object' && (value as { channel?: unknown }).channel === VIEW_CHANNEL;
}
export function isEngineMessage(value: unknown): value is EngineMessage {
  return !!value && typeof value === 'object' && (value as { channel?: unknown }).channel === ENGINE_CHANNEL;
}

/** Fixed 3/4 camera; the train nose sits left of centre so the right HUD column never covers it. */
export const CAMERA_POSE = Object.freeze({
  pos: [12.5, 9.5, 19.5] as [number, number, number],
  target: [-1.0, 1.0, -2.8] as [number, number, number],
  fovDeg: 38,
});

export const CAR_SPACING = 4.6;
/** Coupling point behind the locomotive. */
export const FIRST_CAR_X = -5.15;
export const TRAVEL_DISTANCE = 64;
