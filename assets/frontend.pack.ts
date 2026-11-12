import { definePack, definePackageId } from '@forgeax/engine/pack/source';
import { ok } from '@forgeax/engine/types';
import type { Plugin } from '@forgeax/engine/plugin';
import './runtime/zaohua-platform-init.js';
import { subscribeBrowserFrameSubmitted } from '@forgeax/engine/app';
import { computeDisplayScreenRay } from '@forgeax/engine/picking';
import type { BarrelDistortionMapping } from '@forgeax/engine/render';
export const frontend: Plugin = { name: 'preview-frontend', inject: ['gameHost'], async apply(ctx) {
  await ctx.effect(async function* () {
    const host=ctx.gameHost!;
    host.setPointerLockAllowed?.(false);
    const canvas=host.canvas;
    const port=host.presentationPort??host.port;
    if(!(canvas instanceof HTMLCanvasElement)||!port) throw Error('missing-controller-presentation-host');
    const doc=canvas.ownerDocument, win=doc.defaultView!;
    let point: {x:number;y:number}|undefined, mapping:BarrelDistortionMapping|undefined;
    const publish=()=>{
      const rect=canvas.getBoundingClientRect();
      const inside=point&&point.x>=rect.left&&point.x<rect.right&&point.y>=rect.top&&point.y<rect.bottom;
      const focused=doc.hasFocus();
      const ray=inside&&focused&&mapping&&rect.width>0&&rect.height>0?computeDisplayScreenRay((point!.x-rect.left)/rect.width*mapping.width,(point!.y-rect.top)/rect.height*mapping.height,mapping,mapping.width,mapping.height)?.ray:undefined;
      port.postMessage({type:'controller-pointer',focused,ray:ray?Array.from(ray):undefined});
    };
    const move=(event:PointerEvent)=>{point={x:event.clientX,y:event.clientY};publish();};
    const leave=()=>{point=undefined;publish();};
    const resize=()=>{mapping=undefined;publish();};
    canvas.addEventListener('pointermove',move);canvas.addEventListener('pointerleave',leave);
    win.addEventListener('blur',leave);win.addEventListener('focus',publish);win.addEventListener('resize',resize);
    yield subscribeBrowserFrameSubmitted(canvas,event=>{mapping=event.barrelDistortion;publish();});
    yield ()=>{canvas.removeEventListener('pointermove',move);canvas.removeEventListener('pointerleave',leave);win.removeEventListener('blur',leave);win.removeEventListener('focus',publish);win.removeEventListener('resize',resize);};
    publish();
  });
} };
export default definePack({ schemaVersion: '2.0.0', plugins: true, packageId: definePackageId('019fb7ce-3d00-7000-8000-000000000000'), build: () => ok({ 'plugin/frontend': { kind: 'plugin', module: { specifier: './frontend.pack.ts', export: 'frontend' }, config: {} } }) });




