import { expect, it } from 'vitest';
import { sceneConfig as scene } from '../../scripts/config/game.ts';
import { validatePresentation } from '../../scripts/view/presentation.ts';

it('accepts the shipped camera, actor and desert art configuration', () => {
  expect(validatePresentation(scene)).toBe(scene);
});
