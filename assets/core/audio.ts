class SoundEngine {
  private ctx: AudioContext | null = null;
  public muted = false;
  private ensure(): AudioContext | null {
    if (this.muted || typeof window === 'undefined') return null;
    if (!this.ctx) {
      const C = window.AudioContext || (window as unknown as { webkitAudioContext?: typeof AudioContext }).webkitAudioContext;
      if (!C) return null;
      this.ctx = new C();
    }
    if (this.ctx.state === 'suspended') void this.ctx.resume().catch(() => {});
    return this.ctx;
  }
  play(kind: string): void {
    const ctx = this.ensure(); if (!ctx) return; const t = ctx.currentTime;
    const tone = (a: number, b: number, d: number, ty: OscillatorType, g: number, at = 0) => {
      const o = ctx.createOscillator(), gn = ctx.createGain();
      o.type = ty; o.frequency.setValueAtTime(a, t + at);
      if (a !== b) o.frequency.exponentialRampToValueAtTime(Math.max(20, b), t + at + d);
      gn.gain.setValueAtTime(0.0001, t + at); gn.gain.linearRampToValueAtTime(g, t + at + 0.008);
      gn.gain.exponentialRampToValueAtTime(0.0001, t + at + d);
      o.connect(gn); gn.connect(ctx.destination); o.start(t + at); o.stop(t + at + d + 0.02);
    };
    const noise = (d: number, f: number, g: number) => {
      const len = Math.max(1, Math.floor(ctx.sampleRate * d)), buf = ctx.createBuffer(1, len, ctx.sampleRate), ch = buf.getChannelData(0);
      for (let i = 0; i < len; i++) ch[i] = (Math.random() * 2 - 1) * (1 - i / len);
      const s = ctx.createBufferSource(); s.buffer = buf;
      const fl = ctx.createBiquadFilter(); fl.type = 'bandpass'; fl.frequency.value = f; fl.Q.value = 1.1;
      const gn = ctx.createGain(); gn.gain.setValueAtTime(g, t); gn.gain.exponentialRampToValueAtTime(0.0001, t + d);
      s.connect(fl); fl.connect(gn); gn.connect(ctx.destination); s.start(t); s.stop(t + d + 0.02);
    };
    switch (kind) {
      case 'click': tone(660, 520, .05, 'triangle', .07); break;
      case 'shutter': noise(.22, 320, .12); tone(140, 260, .2, 'sawtooth', .07); break;
      case 'bell': tone(1174, 1174, .25, 'sine', .1); tone(1760, 1760, .3, 'sine', .07, .04); break;
      case 'inspect_ok': tone(587, 880, .14, 'sine', .09); break;
      case 'inspect_expose': tone(880, 440, .12, 'square', .1); tone(622, 311, .18, 'sawtooth', .09, .09); break;
      case 'relabel': noise(.07, 900, .11); tone(190, 110, .09, 'square', .1, .02); break;
      case 'cash': tone(1318, 1318, .15, 'sine', .11); tone(1760, 1760, .26, 'sine', .11, .07); break;
      case 'fail': tone(280, 165, .22, 'sawtooth', .1); break;
      case 'siren': tone(520, 780, .2, 'sawtooth', .1); tone(780, 520, .22, 'sawtooth', .1, .2); break;
      case 'shoot': noise(.07, 1400, .14); tone(180, 58, .08, 'sawtooth', .13); break;
      case 'shoot_electric': tone(1280, 340, .09, 'sawtooth', .11); noise(.05, 2800, .08); break;
      case 'headshot': tone(1980, 2450, .13, 'sine', .13); break;
      case 'hurt': noise(.1, 480, .14); tone(150, 75, .14, 'sawtooth', .12); break;
      case 'dash': noise(.09, 1800, .07); break;
      case 'loot_gold': tone(523, 523, .13, 'triangle', .11); tone(659, 659, .15, 'triangle', .11, .08); tone(784, 1046, .3, 'triangle', .12, .16); break;
      case 'extract': tone(440, 660, .16, 'sine', .11); tone(660, 880, .2, 'sine', .11, .13); tone(880, 1320, .32, 'sine', .12, .26); break;
      default: tone(500, 500, .04, 'sine', .04);
    }
  }
}
export const sfx = new SoundEngine();
