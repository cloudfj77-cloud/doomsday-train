import assert from 'node:assert/strict';
export default async function(play) {
  const before = await play.state();
  await play.screenshot('minimal-3d-scene');
  await play.keyboard.hold('KeyD', 500);
  assert.ok((await play.state()).x > before.x, 'real keyboard must move player');
  await play.screenshot('keyboard-moves-player');
  await play.reload();
  const reset = await play.state();
  assert.equal(reset.x, before.x);
  await play.keyboard.hold('KeyW', 500);
  assert.ok((await play.state()).z < reset.z);
  await play.screenshot('reload-retains-input');
}
