import { readFileSync, writeFileSync } from 'node:fs';
import { spawnSync } from 'node:child_process';
const run = () => spawnSync(process.execPath, ['node_modules/vitest/vitest.mjs', 'run', 'scripts/logic/world.test.ts', 'tools/contracts.test.ts'], { encoding: 'utf8' });
const cases = [
  ['assets/world.json', s => s.replace('"speed": 4', '"speed": -1')],
  ['scripts/logic/world.ts', s => s.replace('state.x + input[0]', 'state.x - input[0]')],
  ['scripts/logic/world.ts', s => s.replace('Object.freeze({ ...state })', '({ ...state })')],
  ['scripts/logic/world.ts', s => s + '\n// document. forbidden boundary\n'],
  ['scripts/view/scene.ts', s => s + '\n// step( forbidden boundary\n'],
  ['AGENTS.md', s => s + 'x'.repeat(20480)],
  ['forge.json', s => { const f = JSON.parse(s); delete f.roots.engine; return JSON.stringify(f); }],
];
if (run().status !== 0) throw new Error('baseline must pass');
for (const [file, mutate] of cases) {
  const original = readFileSync(file, 'utf8');
  try {
    const broken = mutate(original);
    if (broken === original) throw new Error('mutation did not apply: ' + file);
    writeFileSync(file, broken);
    if (run().status === 0) throw new Error('mutation escaped: ' + file);
    console.log('caught broken premise:', file);
  } finally { writeFileSync(file, original); }
  if (run().status !== 0) throw new Error('restored checks failed: ' + file);
}
