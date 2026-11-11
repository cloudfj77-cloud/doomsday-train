import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';

const config = JSON.parse(readFileSync(new URL('../config/gameplay/world.json', import.meta.url), 'utf8'));

async function walk(play, key, reached) {
  const arrival = await play.keyboard.hold(key, reached, { timeoutMs: 12000 });
  assert.ok(reached(arrival), `route waypoint under ${key}`);
  return arrival;
}

async function route(play, size) {
  const shot = label => play.screenshot(`${size}-${label}`);
  const start = await play.state();
  assert.deepEqual([start.x, start.z], config.start);
  await shot('railway-spawn');

  await play.step('Cross the buried tracks from the west', async () => {
    await walk(play, 'KeyA', s => s.x <= -config.limit);
    await walk(play, 'KeyW', s => s.z <= 2.1);
    await walk(play, 'KeyD', s => s.x >= -3.35 && s.moving && s.facingX > 0);
    await shot('crossing-sleepers');
    await walk(play, 'KeyD', s => s.x >= -0.2);
    const stopped = await play.waitFor(s => !s.moving);
    const after = await play.advance(0.25);
    assert.deepEqual([after.x, after.z], [stopped.x, stopped.z]);
    assert.ok(after.walkDistance > start.walkDistance);
  });

  await play.step('Meet the outcrop and take its shaded eastern edge', async () => {
    await walk(play, 'KeyW', s => s.z < 1.3 && !s.moving);
    const front = (await play.state()).z;
    assert.ok(Math.abs(front) < 3, 'contact occurs at a scene obstacle');
    await play.keyboard.hold('KeyW', 250);
    assert.equal((await play.state()).z, front);
    await walk(play, 'KeyD', s => s.x >= 1.5);
    await play.page.keyboard.down('KeyW');
    try {
      await play.waitFor(s => s.z < front - 0.1 && s.moving && s.facingZ < 0);
      await shot('walking-in-rock-shadow');
    } finally {
      await play.page.keyboard.up('KeyW');
    }
  });

  await play.step('Reach the signal and stop at the supply platform', async () => {

    if ((await play.state()).z < -1.7) await walk(play, 'KeyS', s => s.z >= -1.6);
    await walk(play, 'KeyD', s => s.x >= 2.45);
    await walk(play, 'KeyW', s => s.z <= -1.7);
    await shot('signal-and-supplies');
    await walk(play, 'KeyW', s => s.z < -2 && !s.moving);
    const front = (await play.state()).z;
    assert.ok(Math.abs(front) < 3, 'contact occurs at a scene obstacle');
    await play.keyboard.hold('KeyW', 250);
    assert.equal((await play.state()).z, front);
  });

  await play.step('Return around the other side and restore the scene', async () => {
    await walk(play, 'KeyA', s => s.x <= start.x);
    await walk(play, 'KeyS', s => s.z >= start.z && s.facingZ > 0);
    await play.reload();
    const reset = await play.state();
    assert.deepEqual([reset.x, reset.z, reset.walkDistance], [start.x, start.z, start.walkDistance]);
    await walk(play, 'KeyD', s => s.x > reset.x + 0.5);
    await shot('reload-retains-movement');
  });
}

export default async function(play) {
  for (const height of [720, 600]) {
    await play.page.setViewportSize({ width: 1280, height });
    await play.reload();
    await play.step(`1280x${height} railway route`, () => route(play, `1280x${height}`));
  }
  await play.page.setViewportSize({ width: 1280, height: 720 });
}
