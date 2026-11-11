import { expect, it } from 'vitest';
import { readFileSync, readdirSync, statSync } from 'node:fs';
import { resolve } from 'node:path';
export function checkContract(root: string) {
  const read = (p: string) => readFileSync(resolve(root, p), 'utf8');
  if (Buffer.byteLength(read('AGENTS.md')) >= 20480) throw new Error('agents-size');
  const walk = (dir: string): string[] => readdirSync(resolve(root, dir)).flatMap(n => { const p = dir + '/' + n; return statSync(resolve(root, p)).isDirectory() ? walk(p) : [p]; });
  for (const file of walk('scripts/logic').filter(p => p.endsWith('.ts') && !p.endsWith('.test.ts'))) if (/document\.|window\.|scripts\/view|@forgeax\/engine\/render/.test(read(file))) throw new Error('logic-view-boundary');
  for (const file of walk('scripts/view').filter(p => p.endsWith('.ts'))) if (/\bstep\(|validateConfig\(/.test(read(file))) throw new Error('view-logic-boundary');
  const forge = JSON.parse(read('forge.json'));
  if (!forge.roots.engine || !forge.roots.frontend) throw new Error('missing-root');
}
it('enforces the real repository contract', () => checkContract(process.cwd()));
