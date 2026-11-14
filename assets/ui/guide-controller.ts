import type { GameHost } from '@forgeax/engine/app';
import { mountUi, type UiAsset, type UiInstance } from '@forgeax/engine/ui';
import { mountGame } from './game-app.ts';

/** Duckov Pawnshop UI controller: mounts the full game into the presentation shadow root. */
export async function createGuideUi(
  asset: UiAsset,
  host: Pick<GameHost, 'uiRoot' | 'controlMode' | 'port' | 'presentationPort'>,
): Promise<UiInstance> {
  const root = host.uiRoot;
  if (!root) throw new Error('Duckov Pawnshop UI requires GameHost.uiRoot');
  const mounted = mountUi(asset, { root, layer: 50, onAction: () => {} });
  if (!mounted.ok) throw mounted.error;
  const ui = mounted.value;
  const shadow = ui.host.shadowRoot;
  if (!shadow) throw new Error('Duckov Pawnshop UI requires a shadow root');
  const game = mountGame(shadow);
  let disposed = false;
  const dispose = () => {
    if (disposed) return;
    disposed = true;
    game.dispose();
    ui.dispose();
  };
  return { host: ui.host, signal: ui.signal, resourceUrl: ui.resourceUrl, dispose };
}
