import { existsSync, readFileSync, writeFileSync, unlinkSync } from 'node:fs';
import { spawnSync } from 'node:child_process';
const run = () => spawnSync(process.execPath, ['node_modules/vitest/vitest.mjs', 'run', 'tests/logic/world.test.ts', 'tests/contracts/project.test.ts', 'tests/view/presentation.test.ts'], { encoding: 'utf8' });
const cases = [
  ['config/presentation/scene.json', s => s.replace('"gravelCount": 195', '"gravelCount": -2')],
  ['scripts/logic/world.ts', s => s.replace('ids.has(o.id)', 'false')],
  ['scripts/logic/world.ts', s => s.replace('if (c.start.every', 'if (false && c.start.every')],
  ['scripts/logic/world.ts', s => s.replace('enter < time && enter < exit', 'false')],
  ['scripts/logic/world.ts', s => s.replace('if (hit.axis >= 0)', 'if (false)')],
  ['scripts/logic/world.ts', s => s.replace('moving: traveled > 0', 'moving: true')],
  ['scripts/logic/world.ts', s => s.replace('walkDistance: state.walkDistance + traveled', 'walkDistance: 0')],
  ['config/gameplay/world.json', s => s.replace('"size": [2, 1.5]', '"size": [5, 1.5]')],
  ['scripts/logic/layout.test.ts', () => '// misplaced test'],
  ['Documents/uncategorized.md', () => '# misplaced document'],
  ['assets/world.json', () => '{}'],
  ['config/gameplay/world.json', s => s.replace('"speed": 4', '"speed": -1')],
  ['scripts/logic/world.ts', s => s.replace('facingX * distance', '-facingX * distance')],
  ['scripts/logic/world.ts', s => s.replace('Object.freeze({ ...state })', '({ ...state })')],
  ['scripts/logic/world.ts', s => s + '\n// document. forbidden boundary\n'],
  ['scripts/view/scene.ts', s => s + '\n// step( forbidden boundary\n'],
  ['AGENTS.md', s => s + 'x'.repeat(20480)],
  ['forge.json', s => { const f = JSON.parse(s); delete f.roots.engine; return JSON.stringify(f); }],
];
if (run().status !== 0) throw new Error('baseline must pass');
for (const [file, mutate] of cases) {
  const existed = existsSync(file);
  const original = existed ? readFileSync(file, 'utf8') : '';
  try {
    const broken = mutate(original);
    if (broken === original) throw new Error('mutation did not apply: ' + file);
    writeFileSync(file, broken);
    if (run().status === 0) throw new Error('mutation escaped: ' + file);
    console.log('caught broken premise:', file);
  } finally { if (existed) writeFileSync(file, original); else unlinkSync(file); }
  if (run().status !== 0) throw new Error('restored checks failed: ' + file);
}

