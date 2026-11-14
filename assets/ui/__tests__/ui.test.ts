import { readFile } from 'node:fs/promises';
import { describe, expect, it } from 'vitest';

// Product UI tests for Duckov Pawnshop: the template's vase/pointer-lock UI was
// replaced wholesale, so these pin the new game's authored UI contract instead.
describe('Duckov Pawnshop UI assets', () => {
  it('html provides the game mount point', async () => {
    const html = await readFile(new URL('../../guide.ui.html', import.meta.url), 'utf8');
    expect(html).toContain('id="duckov-root"');
  });

  it('css covers shop, afterhours and raid layouts', async () => {
    const css = await readFile(new URL('../../guide.ui.css', import.meta.url), 'utf8');
    for (const marker of ['.career-grid', '.shop', '.raidstats', '.raidwrap.r3', '.lootbox', '.logbar', '.coach', '.tut-focus']) {
      expect(css).toContain(marker);
    }
  });

  it('controller mounts the game app and disposes it', async () => {
    const src = await readFile(new URL('../guide-controller.ts', import.meta.url), 'utf8');
    expect(src).toContain('mountGame');
    expect(src).toContain('game.dispose()');
  });

  it('game app implements both day shop and night raid', async () => {
    const src = await readFile(new URL('../game-app.ts', import.meta.url), 'utf8');
    for (const marker of ['CAREER_SELECT', 'start-raid', 'inspection-submit', 'wholesale-all', 'RaidGame3D', 'tut-start']) {
      expect(src).toContain(marker);
    }
  });

  it('3D raid uses the original Duckov maps, duck models, combat formula and Duckov camera', async () => {
    const src = await readFile(new URL('../raid3d.ts', import.meta.url), 'utf8');
    for (const marker of ['GroundZero', 'HiddenWarehouse', 'DuckBody', 'computeArmorDamage', '55 * Math.PI', 'PerspectiveCamera(20']) {
      expect(src).toContain(marker);
    }
  });
});
