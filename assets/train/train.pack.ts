/**
 * Engine plugin: stages the moving world. The locomotive stays at x = 0 and the
 * world scrolls past it (sleepers, ruins, poles, zombies), so a fixed camera reads
 * like a tracking shot. Stations slide in from the right at the end of a trip.
 */
import { definePack } from '@forgeax/engine/pack/source';
import { Time, Update, type EntityHandle } from '@forgeax/engine/ecs';
import type { Plugin } from '@forgeax/engine/plugin';
import { MeshFilter, MeshRenderer } from '@forgeax/engine/render';
import { Transform } from '@forgeax/engine/scene';
import { quat } from '@forgeax/engine/math';
import { INPUT_SNAPSHOT_RESOURCE_KEY, type InputSnapshot } from '@forgeax/engine/input';
import { resolveGameEntity } from '../shared/scene-refs.ts';
import { SITE_ORIGIN, TRAIN_EXIT, stationBuildings, buildingAt, walkSite, canExtract, EXPLORER_SPEED } from '../shared/station-map.ts';
import { ok } from '@forgeax/engine/types';
import type {} from '@forgeax/engine/app';
import { guidText, materialGuid, PACKAGE_IDS, voxelGuid } from '../shared/asset-refs.ts';
import {
  CAR_SPACING,
  CAMERA_POSE,
  ENGINE_CHANNEL,
  FIRST_CAR_X,
  isViewMessage,
  TRAVEL_DISTANCE,
  type EngineMessage,
  type ViewMessage,
} from '../shared/protocol.ts';
import type { CarKind, Region } from '../sim/data.ts';

type V3 = [number, number, number];
const yaw = (a: number): [number, number, number, number] => [0, Math.sin(a / 2), 0, Math.cos(a / 2)];
const easeInOut = (t: number) => (t < 0.5 ? 2 * t * t : 1 - (-2 * t + 2) ** 2 / 2);

/** Scenery props with a base x that scrolls and wraps over a fixed span. */
const SPAN = 132;
const wrap = (x: number) => ((((x + SPAN / 2) % SPAN) + SPAN) % SPAN) - SPAN / 2;

interface Prop {
  readonly entity: EntityHandle;
  base: number;
  readonly y: number;
  readonly z: number;
  readonly rot: number;
  readonly zombie?: { readonly phase: number; readonly speed: number };
}

export const train: Plugin = {
  name: 'doomsday/train',
  inject: ['world', 'assets', 'gameHost', 'gameScene'],
  async apply(ctx) {
    const world = ctx.world;
    const assets = ctx.assets!;
    const host = ctx.gameHost!;
    const port = host.port;

    await ctx.effect(async function* () {
      // --- Load every mesh and the materials its slots point at ----------------------
      const materialCache = new Map<string, number>();
      const meshes = new Map<string, { mesh: number; materials: number[] }>();
      const keys = [
        'site-ground', 'room-warehouse', 'room-workshop', 'room-clinic', 'explorer', 'exit-marker', 'survey-marker',
        'loco', 'car-storage', 'car-passenger', 'sleepers',
        'station-rich-0', 'station-rich-1', 'station-slum-0', 'station-slum-1',
        'ruin-0', 'ruin-1', 'wreck', 'tree', 'pole', 'zombie-0', 'zombie-1', 'zombie-2', 'smoke',
      ];
      const handles: number[] = [];
      yield () => {
        for (const h of handles) assets.release(h as never);
      };
      for (const key of keys) {
        const loaded = await assets.load(guidText(voxelGuid(key)), 'mesh');
        if (!loaded.ok) throw loaded.error;
        handles.push(loaded.value as unknown as number);
        // Slot names are palette keys (see voxels.pack.ts), so the material GUID derives from them.
        const asset = assets.read(loaded.value).unwrap() as { materialSlots?: readonly { slotName?: string }[] };
        const materials: number[] = [];
        for (const s of asset.materialSlots ?? []) {
          const guid = guidText(materialGuid(s.slotName ?? 'concrete'));
          let handle = materialCache.get(guid);
          if (handle === undefined) {
            const m = await assets.load(guid, 'material');
            if (!m.ok) throw m.error;
            handle = m.value as unknown as number;
            handles.push(handle);
            materialCache.set(guid, handle);
          }
          materials.push(handle);
        }
        meshes.set(key, { mesh: loaded.value as unknown as number, materials });
      }

      const spawned = new Set<EntityHandle>();
      yield () => {
        for (const e of spawned) world.despawn(e);
        spawned.clear();
      };
      const spawn = (key: string, pos: V3, rot = 0, scale: V3 = [1, 1, 1]): EntityHandle => {
        const m = meshes.get(key)!;
        const e = world.spawn(
          { component: Transform, data: { pos, quat: yaw(rot), scale } },
          { component: MeshFilter, data: { assetHandle: m.mesh } as never },
          { component: MeshRenderer, data: { materials: m.materials } as never },
        ).unwrap();
        spawned.add(e);
        return e;
      };
      const kill = (e: EntityHandle | undefined) => {
        if (e === undefined) return;
        world.despawn(e);
        spawned.delete(e);
      };
      const place = (e: EntityHandle, pos: V3, rot = 0, scale?: V3) =>
        world.set(e, Transform, scale ? { pos, quat: yaw(rot), scale } : { pos, quat: yaw(rot) }).unwrap();

      // --- Static-ish actors -----------------------------------------------------------
      const loco = spawn('loco', [0, 0, 0]);
      let cars: { kind: CarKind; entity: EntityHandle }[] = [];
      const sleepers: Prop[] = [];
      for (let i = 0; i < SPAN / 8; i += 1) sleepers.push({ entity: spawn('sleepers', [0, 0, 0]), base: -SPAN / 2 + i * 8 + 4, y: 0, z: 0, rot: 0 });

      // Deterministic scatter of scenery behind (−z) and a few in front (+z) of the line.
      let seed = 1234567;
      const rand = () => {
        seed = (seed * 16807) % 2147483647;
        return seed / 2147483647;
      };
      const props: Prop[] = [];
      const scenery = ['ruin-0', 'ruin-1', 'wreck', 'tree', 'tree', 'ruin-0', 'wreck', 'ruin-1'];
      for (let i = 0; i < 14; i += 1) {
        const key = scenery[i % scenery.length]!;
        const z = -9 - rand() * 14;
        const base = -SPAN / 2 + (i + rand() * 0.6) * (SPAN / 14);
        const rot = (rand() - 0.5) * 0.8;
        props.push({ entity: spawn(key, [base, 0, z], rot), base, y: 0, z, rot });
      }
      for (let i = 0; i < 11; i += 1) {
        const base = -SPAN / 2 + i * 12 + 3;
        props.push({ entity: spawn('pole', [base, 0, -2.0], 0), base, y: 0, z: -2.0, rot: 0 });
      }
      for (let i = 0; i < 6; i += 1) {
        const z = i < 2 ? 4.0 + rand() * 1.5 : -6.5 - rand() * 4;
        const base = -SPAN / 2 + rand() * SPAN;
        const rot = Math.PI / 2 + (rand() - 0.5) * 1.6;
        props.push({ entity: spawn(`zombie-${i % 3}`, [base, 0, z], rot), base, y: 0, z, rot, zombie: { phase: rand() * 6, speed: 0.8 + rand() * 0.6 } });
      }

      const smoke = Array.from({ length: 8 }, (_, i) => ({ entity: spawn('smoke', [0, -20, 0]), age: i * 0.27, x: 1.75, y: 3.1, z: 0 }));

      // --- View state from the frontend ------------------------------------------------
      let view: ViewMessage | undefined;
      let station: { entity: EntityHandle; key: string } | undefined;
      let incoming: { entity: EntityHandle; key: string } | undefined;
      let travel: { start: number; ms: number; serial: number } | undefined;
      let lastSerial = -1;
      let scroll = 0; // accumulated distance used for wrapping scenery
      let titleSpeed = 0;
      let clock = 0;
      let frames = 0;

      const stationKey = (region: Region, s: number) => `station-${region}-${Math.abs(s) % 2}`;
      const syncCars = (kinds: readonly CarKind[]) => {
        const same = kinds.length === cars.length && kinds.every((k, i) => cars[i]!.kind === k);
        if (same) return;
        for (const c of cars) kill(c.entity);
        cars = kinds.map((kind, i) => ({ kind, entity: spawn(kind === 'storage' ? 'car-storage' : 'car-passenger', [FIRST_CAR_X - i * CAR_SPACING, 0, 0]) }));
      };
      const showStationNow = (region: Region, s: number) => {
        const key = stationKey(region, s);
        if (station?.key === key) return;
        kill(station?.entity);
        station = { entity: spawn(key, [0, 0, 0]), key };
      };

      const camera = resolveGameEntity(world, ctx.gameScene, 'camera');
      const siteEntities: EntityHandle[] = [];
      const surveyMarkers = new Map<string, EntityHandle>();
      let player: EntityHandle | undefined;
      let explorer = { x: TRAIN_EXIT.x, z: TRAIN_EXIT.z, surveyed: [] as string[] };
      let facing = 0;
      let inputReady = false;
      let extractRequested = false;
      let siteSeed = -1;
      const cameraAt = (x: number, z: number, exploring: boolean) => {
        const target: V3 = exploring ? [SITE_ORIGIN + x, 1, z] : [...CAMERA_POSE.target];
        const pos: V3 = exploring ? CAMERA_POSE.pos.map((v, i) => v - CAMERA_POSE.target[i]! + target[i]!) as V3 : [...CAMERA_POSE.pos];
        const rotation = quat.fromLookAt(quat.create(), pos, target, [0, 1, 0]);
        world.set(camera, Transform, { pos, quat: [...rotation] as [number, number, number, number] }).unwrap();
      };
      const clearSite = () => {
        for (const e of siteEntities) kill(e);
        siteEntities.length = 0;
        surveyMarkers.clear();
        player = undefined;
        siteSeed = -1;
        cameraAt(0, 0, false);
      };
      const stageSite = (message: ViewMessage) => {
        clearSite();
        siteSeed = message.stationSeed;
        explorer = { x: message.explorer?.x ?? TRAIN_EXIT.x, z: message.explorer?.z ?? TRAIN_EXIT.z, surveyed: [...message.explorer?.surveyed ?? []] };
        const add = (key: string, x: number, z: number, rot = 0) => {
          const e = spawn(key, [SITE_ORIGIN + x, 0, z], rot);
          siteEntities.push(e); return e;
        };
        add('site-ground', 0, 0);
        add('loco', -17, 7.5);
        add('car-storage', -22.2, 7.5);
        add('exit-marker', TRAIN_EXIT.x, TRAIN_EXIT.z);
        for (const b of stationBuildings(siteSeed)) {
          add('room-' + b.id, b.x, b.z);
          surveyMarkers.set(b.id, add('survey-marker', b.x, b.z));
        }
        for (const [x, z] of [[-15,-19],[-14,-15],[14,-20],[14,-13],[-15,-3],[11,3]] as const) add('tree', x, z);
        add('wreck', -9, -19, 0.4);
        add('ruin-0', 13, -24);
        for (const [x, z] of [[-25,-34],[-13,-39],[0,-40],[15,-38],[28,-30]] as const) {
          add(message.region === 'rich' ? 'ruin-0' : 'ruin-1', x, z);
          add('tree', x + 4, z + 3);
        }
        add('pole', -15, 3);
        player = add('explorer', explorer.x, explorer.z);
        inputReady = false;
        cameraAt(explorer.x, explorer.z, true);
      };

      const receive = (event: MessageEvent) => {
        if (!isViewMessage(event.data)) return;
        const message = event.data;
        const wasExploring = view?.mode === 'exploration';
        view = message;
        if (message.mode === 'exploration') {
          if (!wasExploring || siteSeed !== message.stationSeed) stageSite(message);
          else if (message.explorer) explorer = { ...message.explorer, surveyed: [...message.explorer.surveyed] };
        } else if (wasExploring) clearSite();
        syncCars(message.cars);
        if (message.mode === 'travel' && message.travelSerial !== lastSerial) {
          lastSerial = message.travelSerial;
          travel = { start: clock, ms: message.travelMs, serial: message.travelSerial };
          kill(incoming?.entity);
          const key = stationKey(message.region, message.stationSeed);
          incoming = { entity: spawn(key, [TRAVEL_DISTANCE, 0, 0]), key };
        } else if (message.mode === 'station' && !travel) {
          showStationNow(message.region, message.stationSeed);
        } else if (message.mode === 'title') {
          travel = undefined;
          kill(station?.entity);
          kill(incoming?.entity);
          station = undefined;
          incoming = undefined;
        }
      };
      port?.addEventListener('message', receive);
      port?.start();
      yield () => port?.removeEventListener('message', receive);

      const publish = () => {
        port?.postMessage({
          channel: ENGINE_CHANNEL,
          kind: 'status',
          ready: true,
          mode: view?.mode ?? 'title',
          travelling: !!travel,
          cars: cars.length,
          stationSeed: view?.stationSeed ?? -1,
          frames,
          explorer: view?.mode === 'exploration' ? { ...explorer, building: buildingAt(siteSeed, explorer.x, explorer.z)?.id } : undefined,
          extractRequested,
        } satisfies EngineMessage);
      };

      if (host.gameProjection) yield host.gameProjection.registerRead({
        id: 'doomsday.flow', title: 'Station exploration flow',
        read: () => {
          const game = JSON.parse(view?.simulation ?? 'null');
          if (game?.phase === 'exploration' && game.expedition) {
            game.expedition.x = explorer.x;
            game.expedition.y = explorer.z;
            game.expedition.surveyed = [...explorer.surveyed];
          }
          return {
            game, travelling: view?.mode === 'travel',
            buildings: stationBuildings(view?.stationSeed ?? 0).map(b => ({ ...b })),
            engine: { mode: view?.mode ?? 'title', frames, objects: siteEntities.length,
              building: buildingAt(siteSeed, explorer.x, explorer.z)?.id ?? null },
          };
        },
      });

      world.addSystem(Update, {
        name: 'doomsday-train-stage',
        queries: [],
        fn: (w) => {
          const dt = Math.min(w.getResource(Time).delta || 1 / 60, 0.1);
          clock += dt;
          frames += 1;
          const mode = view?.mode ?? 'title';
          if (mode === 'exploration' && player) {
            const kb = w.getResource<InputSnapshot>(INPUT_SNAPSHOT_RESOURCE_KEY)?.keyboard;
            if (!kb?.downCode('KeyE')) inputReady = true;
            const horizontal = Number(!!(kb?.downCode('KeyD') || kb?.downCode('ArrowRight'))) - Number(!!(kb?.downCode('KeyA') || kb?.downCode('ArrowLeft')));
            const vertical = Number(!!(kb?.downCode('KeyS') || kb?.downCode('ArrowDown'))) - Number(!!(kb?.downCode('KeyW') || kb?.downCode('ArrowUp')));
            if (horizontal || vertical) {
              const ox = CAMERA_POSE.pos[0] - CAMERA_POSE.target[0];
              const oz = CAMERA_POSE.pos[2] - CAMERA_POSE.target[2];
              const norm = Math.hypot(ox, oz) * Math.hypot(horizontal, vertical);
              const dx = (horizontal * oz + vertical * ox) / norm;
              const dz = (-horizontal * ox + vertical * oz) / norm;
              const next = walkSite(siteSeed, explorer.x, explorer.z, dx * dt * EXPLORER_SPEED, dz * dt * EXPLORER_SPEED);
              explorer.x = next.x; explorer.z = next.z;
              facing = Math.atan2(dx, dz);
            }
            const building = buildingAt(siteSeed, explorer.x, explorer.z);
            if (inputReady && kb?.justPressedCode('KeyE')) {
              if (building && !explorer.surveyed.includes(building.id)) explorer.surveyed.push(building.id);
              if (canExtract(explorer.x, explorer.z)) extractRequested = true;
              publish(); extractRequested = false;
            }
            place(player, [SITE_ORIGIN + explorer.x, 0.03, explorer.z], facing);
            for (const [id, marker] of surveyMarkers) {
              const b = stationBuildings(siteSeed).find(b => b.id === id)!;
              place(marker, [SITE_ORIGIN + b.x, explorer.surveyed.includes(id) ? -10 : 0, b.z]);
            }
            cameraAt(explorer.x, explorer.z, true);
            if (frames % 6 === 0) publish();
          }
          let speed = 0; // world units per second, for smoke drift & bob
          let shift = 0; // distance travelled this trip (stations)

          if (travel) {
            const t = Math.min(1, ((clock - travel.start) * 1000) / travel.ms);
            const s = easeInOut(t) * TRAVEL_DISTANCE;
            const prev = (travel as { s?: number }).s ?? 0;
            (travel as { s?: number }).s = s;
            scroll += s - prev;
            speed = (s - prev) / Math.max(dt, 1e-3);
            shift = s;
            if (station) place(station.entity, [-shift, 0, 0]);
            if (incoming) place(incoming.entity, [TRAVEL_DISTANCE - shift, 0, 0]);
            if (t >= 1) {
              kill(station?.entity);
              station = incoming;
              incoming = undefined;
              if (station) place(station.entity, [0, 0, 0]);
              travel = undefined;
              publish();
            }
          } else if (mode === 'title') {
            titleSpeed = Math.min(14, titleSpeed + dt * 6);
            scroll += titleSpeed * dt;
            speed = titleSpeed;
          } else {
            titleSpeed = 0;
          }

          for (const p of sleepers) place(p.entity, [wrap(p.base - scroll), 0, 0]);
          for (const p of props) {
            const x = wrap(p.base - scroll);
            if (p.zombie) {
              const sway = Math.sin(clock * p.zombie.speed * 2 + p.zombie.phase);
              const walk = clock * 0.35 * p.zombie.speed;
              place(p.entity, [wrap(p.base - scroll + walk), Math.abs(sway) * 0.06, p.z], p.rot + sway * 0.18);
            } else place(p.entity, [x, p.y, p.z], p.rot);
          }

          // Train bob while moving.
          const bob = speed > 0.5 ? Math.sin(clock * 22) * 0.025 : 0;
          place(loco, [0, bob, 0]);
          cars.forEach((c, i) => place(c.entity, [FIRST_CAR_X - i * CAR_SPACING, speed > 0.5 ? Math.sin(clock * 22 + i * 1.7) * 0.025 : 0, 0]));

          // Chimney smoke: rises, grows, drifts back with speed.
          for (const puff of smoke) {
            puff.age += dt * (speed > 0.5 ? 1.6 : 0.7);
            if (puff.age > 2.2) {
              puff.age -= 2.2;
              puff.x = 1.75;
              puff.y = 3.1;
              puff.z = 0;
            }
            puff.x -= dt * (0.25 + speed * 0.55);
            puff.y += dt * 0.9;
            const size = 0.35 + puff.age * 0.42;
            const alive = puff.age < 2.0;
            place(puff.entity, alive ? [puff.x, puff.y, puff.z + Math.sin(puff.age * 3) * 0.15] : [0, -30, 0], puff.age, [size, size, size]);
          }
          if (frames % 30 === 0) publish();
        },
      }).unwrap();
      yield () => world.removeSystem(Update, 'doomsday-train-stage').unwrap();
      publish();
    });
  },
};

export default definePack({
  schemaVersion: '2.0.0',
  plugins: true,
  packageId: PACKAGE_IDS.trainPlugin,
  build: () => ok({ 'plugin/train': { kind: 'plugin', module: { specifier: './train.pack.ts', export: 'train' } } }),
});
