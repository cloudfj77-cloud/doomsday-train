import { expect, it } from 'vitest';
import scene from '../../config/presentation/scene.json';
import { validatePresentation } from '../../scripts/view/presentation.ts';

it('accepts the shipped camera, actor and desert art configuration', () => {
  expect(validatePresentation(scene)).toBe(scene);
});
