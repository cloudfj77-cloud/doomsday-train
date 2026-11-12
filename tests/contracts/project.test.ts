import { it } from 'vitest';
import { existsSync, readFileSync, readdirSync, statSync } from 'node:fs';
import { resolve } from 'node:path';
export function checkContract(root: string) {
  const read = (p: string) => readFileSync(resolve(root, p), 'utf8');
  if (Buffer.byteLength(read('AGENTS.md')) >= 20480) throw new Error('agents-size');
  const walk = (dir: string): string[] => readdirSync(resolve(root, dir)).flatMap(n => { const p = dir + '/' + n; return statSync(resolve(root, p)).isDirectory() ? walk(p) : [p]; });
  if (walk('scripts').some(p => /(?:\.test\.[cm]?[jt]s|playthrough\.mjs)$/.test(p))) throw new Error('tests-outside-tests-directory');
  if (readdirSync(resolve(root, 'Documents')).some(p => statSync(resolve(root, 'Documents', p)).isFile())) throw new Error('uncategorized-document');
  if (walk('assets').some(p => p.endsWith('.json') && !/\.(?:meta|pack)\.json$/.test(p))) throw new Error('config-inside-assets');
  for (const p of ['config/gameplay/world.json', 'config/presentation/scene.json', 'config/gameplay/items.json', 'config/gameplay/item-world.json', 'config/presentation/items.json', 'Documents/design/item-system.md', 'Documents/decisions/records.md']) if (!existsSync(resolve(root, p))) throw new Error('missing-classified-file: ' + p);
  for (const file of walk('scripts/logic').filter(p => p.endsWith('.ts') && !p.endsWith('.test.ts'))) if (/document\.|window\.|scripts\/view|@forgeax\/engine\/render/.test(read(file))) throw new Error('logic-view-boundary');
  for (const file of walk('scripts/view').filter(p => p.endsWith('.ts'))) if (/\bstep\(|validateConfig\(/.test(read(file))) throw new Error('view-logic-boundary');
  const forge = JSON.parse(read('forge.json'));
  if (!forge.roots.engine || !forge.roots.frontend) throw new Error('missing-root');
  if (/doc(?:ument)?\.body\.(?:append|replaceChildren|innerHTML)/.test(read('scripts/view/item-ui.ts'))) throw new Error('inventory-outside-host-ui-root');
}
it('enforces the real repository contract', () => checkContract(process.cwd()));
