import { definePack, definePackageId } from '@forgeax/engine/pack/source';
import { ok } from '@forgeax/engine/types';
import type { Plugin } from '@forgeax/engine/plugin';
import './runtime/zaohua-platform-init.js';
export const frontend: Plugin = { name: 'preview-frontend', inject: ['gameHost'], async apply(ctx) { await ctx.effect(async function* () { ctx.gameHost!.setPointerLockAllowed?.(false); }); } };
export default definePack({ schemaVersion: '2.0.0', plugins: true, packageId: definePackageId('019fb7ce-3d00-7000-8000-000000000000'), build: () => ok({ 'plugin/frontend': { kind: 'plugin', module: { specifier: './frontend.pack.ts', export: 'frontend' }, config: {} } }) });




