/**
 * Language rules shared by the HUD and the loading cover (web/head.html mirrors them):
 * first visit follows the browser (zh* → Chinese, otherwise English); afterwards the
 * player's manual choice, saved locally, wins. `?lang=zh|en` overrides for QA.
 */
export type GameLanguage = 'zh' | 'en';
export const LANG_KEY = 'doomsday.lang';

export function detectBrowserLanguage(languages: readonly (string | undefined)[]): GameLanguage {
  const first = languages.find((value) => typeof value === 'string' && value.length > 0);
  return first !== undefined && first.toLowerCase().startsWith('zh') ? 'zh' : 'en';
}

export function initialLanguage(options: {
  readonly saved: string | null;
  readonly browser: readonly (string | undefined)[];
  readonly override?: string | null;
}): { readonly lang: GameLanguage; readonly source: 'browser' | 'saved' | 'override' } {
  if (options.override === 'zh' || options.override === 'en') return { lang: options.override, source: 'override' };
  if (options.saved === 'zh' || options.saved === 'en') return { lang: options.saved, source: 'saved' };
  return { lang: detectBrowserLanguage(options.browser), source: 'browser' };
}
