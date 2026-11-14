// 3D 夜袭：原版《逃离鸭科夫》GLB 战区 + 原版鸭子/武器 OBJ，55° 俯角 / -30° 偏航 / 20° FOV 的鸭科夫镜头。
// three.js 由 scripts/sync-art.sh 托管在 /vendor/three，运行时动态加载（不进引擎打包）。
import { GRADE_LABELS, ITEM_DEFS, RAID_ZONES, RARITY_COLORS, RARITY_NAMES } from '../core/catalog.ts';
import { clamp, computeArmorDamage, computeEffectiveWeaponStats, computeLootInspectDuration, rollRaidLootItem } from '../core/formulas.ts';
import { sfx } from '../core/audio.ts';
import type { GameStore } from '../core/store.ts';
import type { DuckItem } from '../core/types.ts';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
type Any = any;
type Zone = (typeof RAID_ZONES)[number];
type WeaponStats = ReturnType<typeof computeEffectiveWeaponStats>;
export type RaidFinish = (extracted: boolean, reason: string, usedMeds: string[], loot: DuckItem[], helmDur: number, armorDur: number) => void;

const esc = (s: unknown) => String(s).replace(/[&<>"']/g, c => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]!));
const gradeName = (g: number) => GRADE_LABELS[g as keyof typeof GRADE_LABELS] ?? String(g);
const itemIcon = (defId: string) => `/art/items/${defId}.png`;

const MAP_FILE: Record<string, string> = { ground_zero: 'GroundZero', warehouse: 'HiddenWarehouse', farm_town: 'Farm', jlab_37: 'JLab' };
const MAP_MB: Record<string, number> = { GroundZero: 13, HiddenWarehouse: 18, Farm: 66, JLab: 20 };
const REGION_SPAN = 170;
const SHOT_H = 1.1;
const PITCH = (55 * Math.PI) / 180, YAW = (30 * Math.PI) / 180;
const CAM_DIR = { x: -Math.sin(YAW) * Math.cos(PITCH), y: Math.sin(PITCH), z: -Math.cos(YAW) * Math.cos(PITCH) };
const FWD = { x: Math.sin(YAW), z: Math.cos(YAW) };
const RIGHT = { x: -FWD.z, z: FWD.x };

interface ThreeLib { THREE: Any; GLTFLoader: Any; OBJLoader: Any }
let threePromise: Promise<ThreeLib> | null = null;
// 引擎插件打包只允许字面量 import，three 通过 <script type="module"> 注入，由 duckov-three.js 挂到 window。
function loadThree(): Promise<ThreeLib> {
  threePromise ??= new Promise<ThreeLib>((resolve, reject) => {
    const g = globalThis as unknown as { __duckovThree?: ThreeLib };
    if (g.__duckovThree) { resolve(g.__duckovThree); return; }
    const s = document.createElement('script'); s.type = 'module'; s.src = '/vendor/three/duckov-three.js';
    s.onload = () => (g.__duckovThree ? resolve(g.__duckovThree) : reject(new Error('three.js 未就绪')));
    s.onerror = () => { threePromise = null; reject(new Error('three.js 加载失败')); };
    document.head.appendChild(s);
  });
  return threePromise;
}

// ---------- 导航网格：GPU 俯视/仰视四次渲染 → 地面高度、可走区域、遮挡 ----------
interface Nav { minX: number; maxZ: number; cell: number; N: number; W: Float32Array; ok: Uint8Array; block: Uint8Array; top: Float32Array; dist: Int32Array; spawn: number; cells: number[] }
interface MapEntry { name: string; root: Any; nav: Nav }
let mapCache: MapEntry | null = null;
let rendererSingleton: Any = null;
const duckParts: Record<string, Any> = {};

const NAV_VERT = `varying vec3 vW; varying vec3 vN;
void main(){ mat4 m = modelMatrix;
#ifdef USE_INSTANCING
m = m * instanceMatrix;
#endif
vec4 w = m * vec4(position,1.0); vW = w.xyz; vN = normalize(mat3(m) * normal); gl_Position = projectionMatrix * viewMatrix * w; }`;
const NAV_FRAG = (body: string) => `uniform sampler2D ref; varying vec3 vW; varying vec3 vN;
void main(){ vec4 r = texelFetch(ref, ivec2(gl_FragCoord.xy), 0); ${body} }`;

function buildNav(T: Any, renderer: Any, root: Any, info: { focus: number[][]; bbox: number[][] }): Nav {
  const [a, b] = info.focus as [number[], number[]];
  const span = Math.min(REGION_SPAN, Math.max(b[0]! - a[0]!, b[2]! - a[2]!, 60));
  const cx = (a[0]! + b[0]!) / 2, cz = (a[2]! + b[2]!) / 2, half = span / 2;
  const yMin = info.bbox[0]![1]! - 5, yMax = info.bbox[1]![1]! + 5;
  const N = clamp(Math.ceil(span / 0.3), 256, 768);
  const cell = span / N;
  const rt = new T.WebGLRenderTarget(N, N, { type: T.FloatType, format: T.RGBAFormat, minFilter: T.NearestFilter, magFilter: T.NearestFilter, depthBuffer: true, generateMipmaps: false });
  const mkCam = (fromBelow: boolean) => {
    const c = new T.OrthographicCamera(-half, half, half, -half, 0.1, yMax - yMin + 20);
    c.up.set(0, 0, -1); c.position.set(cx, fromBelow ? yMin - 10 : yMax + 10, cz); c.lookAt(cx, fromBelow ? yMax : yMin, cz); c.updateMatrixWorld(); return c;
  };
  const topCam = mkCam(false), botCam = mkCam(true);
  const scene = new T.Scene(); scene.add(root);
  const blank = new T.DataTexture(new Float32Array(4), 1, 1, T.RGBAFormat, T.FloatType); blank.needsUpdate = true;
  const pass = (cam: Any, body: string, ref: Any) => {
    const mat = new T.ShaderMaterial({ vertexShader: NAV_VERT, fragmentShader: NAV_FRAG(body), uniforms: { ref: { value: ref } }, side: T.DoubleSide });
    scene.overrideMaterial = mat;
    renderer.setRenderTarget(rt); renderer.setClearColor(0x000000, 0); renderer.clear(); renderer.render(scene, cam);
    const out = new Float32Array(N * N * 4); renderer.readRenderTargetPixels(rt, 0, 0, N, N, out);
    renderer.setRenderTarget(null); mat.dispose(); return out;
  };
  const toTex = (d: Float32Array) => { const t = new T.DataTexture(d, N, N, T.RGBAFormat, T.FloatType); t.minFilter = t.magFilter = T.NearestFilter; t.needsUpdate = true; return t; };
  const prevClip = renderer.clippingPlanes; renderer.clippingPlanes = [];
  // 1) 仰视最低的水平面 = 地面（仰视相机左右镜像，回读时翻转 x）
  const gRaw = pass(botCam, 'if (abs(vN.y) < 0.5) discard; gl_FragColor = vec4(vW.y, 1.0, 0.0, 1.0);', blank);
  const G = new Float32Array(N * N * 4);
  for (let j = 0; j < N; j++) for (let i = 0; i < N; i++) { const s = (j * N + (N - 1 - i)) * 4, d = (j * N + i) * 4; G[d] = gRaw[s]!; G[d + 1] = gRaw[s + 1]!; }
  // 2) 俯视最高点（判断头顶有没有屋顶）
  const top = pass(topCam, 'gl_FragColor = vec4(vW.y, 1.0, 0.0, 1.0);', blank);
  // 3) 地面以上 2.4m 内最高的水平面 = 可站立面
  const gTex = toTex(G);
  const Wr = pass(topCam, 'if (abs(vN.y) < 0.5 || r.g < 0.5 || vW.y > r.r + 2.4) discard; gl_FragColor = vec4(vW.y, 1.0, 0.0, 1.0);', gTex);
  // 4) 站立面上方 0.4~1.9m 有东西 = 挡路/挡子弹
  const wTex = toTex(Wr);
  const O = pass(topCam, 'if (r.g < 0.5 || vW.y < r.r + 0.4 || vW.y > r.r + 1.9) discard; gl_FragColor = vec4(1.0);', wTex);
  renderer.clippingPlanes = prevClip; scene.remove(root); scene.overrideMaterial = null;
  gTex.dispose(); wTex.dispose(); blank.dispose(); rt.dispose();

  const NN = N * N, W = new Float32Array(NN), topH = new Float32Array(NN), walk = new Uint8Array(NN), block = new Uint8Array(NN);
  for (let k = 0; k < NN; k++) {
    const has = Wr[k * 4 + 1]! > 0.5, blocked = O[k * 4 + 1]! > 0.5;
    W[k] = has ? Wr[k * 4]! : -999; topH[k] = top[k * 4 + 1]! > 0.5 ? top[k * 4]! : -999;
    walk[k] = has && !blocked ? 1 : 0; block[k] = !has || blocked ? 1 : 0;
  }
  // 腐蚀一格，给鸭子留出身位
  const walkE = new Uint8Array(NN);
  for (let j = 1; j < N - 1; j++) for (let i = 1; i < N - 1; i++) {
    const k = j * N + i; if (!walk[k]) continue;
    let ok = 1; for (let dj = -1; dj <= 1 && ok; dj++) for (let di = -1; di <= 1; di++) if (!walk[k + dj * N + di]) { ok = 0; break; }
    walkE[k] = ok;
  }
  // 连通分量：台阶落差 ≤0.45m 可走，取最大的一块
  const comp = new Int32Array(NN).fill(-1); let best = -1, bestSize = 0, cid = 0; const q = new Int32Array(NN);
  for (let s = 0; s < NN; s++) {
    if (!walkE[s] || comp[s] !== -1) continue;
    let h = 0, t = 0; q[t++] = s; comp[s] = cid;
    while (h < t) { const k = q[h++]!; const i = k % N; for (const n of [k - 1, k + 1, k - N, k + N]) { if (n < 0 || n >= NN || (n === k - 1 && i === 0) || (n === k + 1 && i === N - 1)) continue; if (walkE[n] && comp[n] === -1 && Math.abs(W[n]! - W[k]!) <= 0.45) { comp[n] = cid; q[t++] = n; } } }
    if (t > bestSize) { bestSize = t; best = cid; } cid++;
  }
  const ok = new Uint8Array(NN); const cells: number[] = [];
  for (let k = 0; k < NN; k++) if (comp[k] === best) { ok[k] = 1; cells.push(k); }
  // 出生点：最靠近西南角（镜头左下）的可走格；再算到各格的步数距离
  let spawn = cells[0] ?? 0, sd = Infinity;
  for (const k of cells) { const i = k % N, j = Math.floor(k / N); const d = i * i + j * j; if (d < sd) { sd = d; spawn = k; } }
  const dist = new Int32Array(NN).fill(-1); { let h = 0, t = 0; q[t++] = spawn; dist[spawn] = 0; while (h < t) { const k = q[h++]!; const i = k % N; for (const n of [k - 1, k + 1, k - N, k + N]) { if (n < 0 || n >= NN || (n === k - 1 && i === 0) || (n === k + 1 && i === N - 1)) continue; if (ok[n] && dist[n] === -1 && Math.abs(W[n]! - W[k]!) <= 0.45) { dist[n] = dist[k]! + 1; q[t++] = n; } } } }
  return { minX: cx - half, maxZ: cz + half, cell, N, W, ok, block, top: topH, dist, spawn, cells };
}

const navIdx = (n: Nav, x: number, z: number) => { const i = Math.floor((x - n.minX) / n.cell), j = Math.floor((n.maxZ - z) / n.cell); return i < 0 || j < 0 || i >= n.N || j >= n.N ? -1 : j * n.N + i; };
const navPos = (n: Nav, k: number) => ({ x: n.minX + ((k % n.N) + 0.5) * n.cell, z: n.maxZ - (Math.floor(k / n.N) + 0.5) * n.cell, y: n.W[k]! });

// ---------- 实体 ----------
interface Enemy { obj: Any; x: number; z: number; y: number; hp: number; maxHp: number; speed: number; boss: boolean; alert: number; shootCd: number; name: string; dmg: number; tx: number; tz: number; stuck: number; face: number; bar: HTMLElement; dead: boolean }
interface Crate { id: number; obj: Any; ring: Any; x: number; z: number; y: number; opened: boolean; tier: string }
interface Bullet { mesh: Any; x: number; y: number; z: number; vx: number; vz: number; dmg: number; fromPlayer: boolean; left: number; elec?: boolean }
interface Float { el: HTMLElement; x: number; y: number; z: number; life: number }
interface Extract { x: number; z: number; y: number; obj: Any }

export interface RaidOptions { tutorial?: boolean; onTutorialDone?: () => void }

const TUT_STEPS = [
  { title: '第 1 步 · 走路', text: '按 <b>W A S D</b> 移动（W 是屏幕上方）。走到前面 <b style="color:#ffd23e">黄色光圈</b> 里。' },
  { title: '第 2 步 · 开枪', text: '一只敌鸭发现你了！<b>鼠标</b>对准它，<b>按住左键</b>射击。头顶红条是它的血量。打空了按 <b>R</b> 换弹。' },
  { title: '第 3 步 · 开箱', text: '走到 <b style="color:#ffcf5e">发光的物资箱</b> 旁边，按 <b>E</b> 打开。' },
  { title: '第 4 步 · 搜刮', text: '点「🔎 搜刮」等读条（东西越值钱读条越久），再点「放入背包」，最后点「继续战斗」。' },
  { title: '第 5 步 · 翻滚', text: '按 <b>空格</b> 或 <b>Shift</b> 翻滚一次——被围攻时用它躲子弹。受伤了按 <b>Q</b> 吃药。' },
  { title: '第 6 步 · 撤离', text: '跟着屏幕上的 <b style="color:#3dff8c">绿色箭头</b> 走到绿色光柱里，<b>站着别动 3 秒</b>。撤出去，战利品才算你的！' },
];

export class RaidGame3D {
  private T: Any = null; private renderer: Any = null; private scene: Any = null; private camera: Any = null; private nav!: Nav;
  private raf = 0; private last = 0; private keys = new Set<string>(); private firing = false; private lastShot = 0;
  private mouse = { x: 0, y: 0 }; private aim = { x: 0, z: 1 };
  private px = 0; private pz = 0; private py = 0; private face = 0; private hp: number; private maxHp = 100;
  private ammo: number; private reloading = 0; private dashCd = 0; private dashT = 0; private dashDir = { x: 0, z: 0 };
  private enemies: Enemy[] = []; private crates: Crate[] = []; private bullets: Bullet[] = []; private floats: Float[] = []; private extracts: Extract[] = [];
  private timeLeft: number; private kills = 0; private finished = false; private started = false; private ready = false;
  private helmDur: number; private armorDur: number; private bleeding = false; private bleedT = 0; private extractT = 0;
  private usedMeds: string[] = []; private loot: DuckItem[] = []; private lootOpen: Crate | null = null;
  private weather: 'clear_night' | 'storm_rain';
  private disposers: Array<() => void> = []; private transient: Any[] = [];
  private host!: HTMLElement; private hud!: HTMLElement; private lootEl!: HTMLElement; private labels!: HTMLElement; private coach!: HTMLElement; private arrow!: HTMLElement; private dist!: HTMLElement; private mini!: HTMLCanvasElement; private miniBg: ImageData | null = null; private hudT = 0;
  private player: Any = null; private aimLine: Any = null; private flash: Any = null; private flashT = 0; private spot: Any = null; private clip: Any = null; private rain: Any = null;
  private camT = { x: 0, y: 0, z: 0 }; private camDist = 34; private walkPhase = 0;
  private tutStep = 0; private tutMarker: Any = null; private tutLooted = false;

  constructor(private store: GameStore, private zone: Zone, private ws: WeaponStats, private meds: DuckItem[], private onFinish: RaidFinish, private opts: RaidOptions = {}) {
    this.maxHp = store.upgrades.has('upg_duck_conditioning') ? 135 : 100;
    this.hp = this.maxHp; this.ammo = ws.magSize;
    this.timeLeft = opts.tutorial ? 900 : Math.round(zone.timeLimitSec * 1.5);
    this.weather = opts.tutorial ? 'clear_night' : zone.weatherPool[Math.floor(Math.random() * zone.weatherPool.length)]!;
    const helm = store.equippedHelmet(), arm = store.equippedArmor();
    this.helmDur = helm?.durability ?? 0; this.armorDur = arm?.durability ?? 0;
  }

  start(host: HTMLElement) {
    if (this.started) return;
    this.started = true; this.host = host;
    host.innerHTML = `<div class="r3-stage"></div><div class="r3-labels"></div><div id="raid-hud"></div>
      <canvas class="r3-mini" width="180" height="180"></canvas><div class="r3-arrow">➤</div><div class="r3-dist"></div>
      <div class="r3-coach" style="display:none"></div><div id="raid-loot"></div>
      <div class="raidhelp">WASD 移动 · 鼠标瞄准 · 左键射击 · 空格/Shift 翻滚 · R 换弹 · E 开箱 · Q 医疗 · 滚轮缩放 · Esc 放弃</div>
      <div class="r3-loading"><div class="r3-loadbox"><b>正在空投到「${esc(this.zone.name)}」</b><div class="r3-bar"><i></i></div><small>载入原版鸭科夫 3D 战区…</small></div></div>`;
    this.hud = host.querySelector('#raid-hud')!; this.lootEl = host.querySelector('#raid-loot')!; this.labels = host.querySelector('.r3-labels')!;
    this.coach = host.querySelector('.r3-coach')!; this.arrow = host.querySelector('.r3-arrow')!; this.dist = host.querySelector('.r3-dist')!; this.mini = host.querySelector('.r3-mini')!;
    this.boot().catch(err => { console.error(err); const l = host.querySelector('.r3-loading small'); if (l) l.innerHTML = `3D 战区加载失败：${esc((err as Error)?.message ?? err)}<br><button class="r3-btn" data-r3-abort>返回店铺</button>`; host.querySelector('[data-r3-abort]')?.addEventListener('click', () => this.finish(false, '战区加载失败')); });
  }

  private setLoad(p: number, txt: string) { const bar = this.host.querySelector<HTMLElement>('.r3-bar i'); const s = this.host.querySelector<HTMLElement>('.r3-loading small'); if (bar) bar.style.width = `${Math.round(p * 100)}%`; if (s) s.textContent = txt; }

  private async boot() {
    this.setLoad(0.02, '载入 3D 渲染器…');
    const { THREE: T, GLTFLoader, OBJLoader } = await loadThree(); this.T = T;
    if (!rendererSingleton) {
      rendererSingleton = new T.WebGLRenderer({ antialias: true, powerPreference: 'high-performance' });
      rendererSingleton.setPixelRatio(Math.min(devicePixelRatio, 1.75));
      rendererSingleton.toneMapping = T.ACESFilmicToneMapping; rendererSingleton.toneMappingExposure = 1.15;
      rendererSingleton.localClippingEnabled = false;
    }
    const renderer = this.renderer = rendererSingleton;
    if (!renderer.extensions.has('EXT_color_buffer_float')) throw new Error('浏览器不支持浮点渲染（请用新版 Chrome / Edge）');
    this.host.querySelector('.r3-stage')!.appendChild(renderer.domElement);
    const name = MAP_FILE[this.zone.id] ?? 'GroundZero';
    if (!Object.keys(duckParts).length) {
      this.setLoad(0.05, '载入原版鸭子与武器模型…');
      const ol = new OBJLoader();
      await Promise.all(['DuckBody', 'Duck_Beak', 'DuckFoot2', 'DuckGlove', 'Crown', 'WPN_AK74U', 'WPN_AK47', 'WPN_Mosin', 'WPN_CubeGun', 'Knife01'].map(async n => {
        const g = await ol.loadAsync(`/art/mesh/${n}.obj`); let geo: Any = null; g.traverse((o: Any) => { if (o.isMesh && !geo) geo = o.geometry; });
        if (n === 'DuckBody' || n === 'Duck_Beak') geo.rotateX(-Math.PI / 2);
        geo.computeVertexNormals(); duckParts[n] = geo;
      }));
    }
    if (!mapCache || mapCache.name !== name) {
      if (mapCache) { mapCache.root.traverse((o: Any) => { if (o.isMesh) { o.geometry.dispose(); (Array.isArray(o.material) ? o.material : [o.material]).forEach((m: Any) => { m.map?.dispose(); m.dispose(); }); } }); mapCache = null; }
      const info = await (await fetch(`/explorer3d/viewer/${name}.json`)).json();
      const gltf: Any = await new Promise((res, rej) => new GLTFLoader().load(`/explorer3d/viewer/${name}.glb`, res, (ev: Any) => {
        const total = ev.total || (MAP_MB[name] ?? 20) * 1e6; this.setLoad(0.08 + 0.8 * Math.min(1, ev.loaded / total), `下载原版战区模型 ${name}.glb … ${(ev.loaded / 1e6).toFixed(0)} / ${(total / 1e6).toFixed(0)} MB`);
      }, rej));
      const root = gltf.scene;
      root.traverse((o: Any) => { if (o.isMesh) { (Array.isArray(o.material) ? o.material : [o.material]).forEach((m: Any) => { m.side = T.DoubleSide; if (m.map) m.map.anisotropy = 4; }); } });
      root.updateMatrixWorld(true);
      this.setLoad(0.92, '扫描地形：计算可走区域与掩体…');
      await new Promise(r => setTimeout(r, 30));
      mapCache = { name, root, nav: buildNav(T, renderer, root, info) };
    }
    this.nav = mapCache.nav;
    if (this.nav.cells.length < 500) throw new Error('这张图没扫到足够的可走区域');
    this.setupScene(mapCache.root);
    this.populate();
    this.bindInput();
    this.host.querySelector('.r3-loading')?.remove();
    this.ready = true; this.last = performance.now();
    if (this.opts.tutorial) this.setTutStep(0);
    const loop = (t: number) => { if (this.finished) return; const dt = Math.min(0.05, (t - this.last) / 1000); this.last = t; this.update(dt); this.draw(); this.raf = requestAnimationFrame(loop); };
    this.raf = requestAnimationFrame(loop);
    this.renderHud();
  }

  private setupScene(mapRoot: Any) {
    const T = this.T, storm = this.weather === 'storm_rain';
    const scene = this.scene = new T.Scene();
    const sky = new T.Color(storm ? 0x0d1520 : 0x141d2e);
    scene.background = sky; scene.fog = new T.Fog(sky, 55, 140);
    scene.add(mapRoot);
    scene.add(new T.HemisphereLight(storm ? 0x7d8db3 : 0x9fb2e0, 0x2a241e, storm ? 0.75 : 0.9));
    const moon = new T.DirectionalLight(0xbcd0ff, storm ? 0.55 : 0.8); moon.position.set(-0.6, 1, 0.45).multiplyScalar(500); scene.add(moon);
    scene.add(new T.AmbientLight(0xffffff, 0.18));
    this.spot = new T.SpotLight(0xfff0d0, 260, 34, 0.55, 0.55, 1.3); scene.add(this.spot, this.spot.target);
    const glow = new T.PointLight(0xffd59a, 26, 12, 1.6); glow.position.set(0, 2.2, 0); this.flash = new T.PointLight(0xffc860, 0, 9, 2);
    this.player = this.makeDuck(0xf4f0e4, this.weaponMesh(), false); this.player.add(glow, this.flash); this.flash.position.set(0.2, 0.9, 0.9);
    const ring = new T.Mesh(new T.RingGeometry(0.55, 0.72, 32), new T.MeshBasicMaterial({ color: 0xffd23e, transparent: true, opacity: 0.8, depthWrite: false })); ring.rotation.x = -Math.PI / 2; ring.position.y = 0.06; this.player.add(ring);
    scene.add(this.player);
    this.aimLine = new T.Line(new T.BufferGeometry().setFromPoints([new T.Vector3(), new T.Vector3()]), new T.LineBasicMaterial({ color: 0xffd23e, transparent: true, opacity: 0.55 }));
    scene.add(this.aimLine);
    this.camera = new T.PerspectiveCamera(20, 1, 1, 600);
    this.clip = new T.Plane(new T.Vector3(0, -1, 0), 1e4); this.renderer.clippingPlanes = [this.clip];
    if (storm) {
      const n = 1800, pos = new Float32Array(n * 3); for (let i = 0; i < n; i++) { pos[i * 3] = (Math.random() - 0.5) * 60; pos[i * 3 + 1] = Math.random() * 25; pos[i * 3 + 2] = (Math.random() - 0.5) * 60; }
      const g = new T.BufferGeometry(); g.setAttribute('position', new T.BufferAttribute(pos, 3));
      this.rain = new T.Points(g, new T.PointsMaterial({ color: 0x9fc8ff, size: 0.09, transparent: true, opacity: 0.6, depthWrite: false })); scene.add(this.rain);
    }
    this.resize();
    const ro = new ResizeObserver(() => this.resize()); ro.observe(this.host); this.disposers.push(() => ro.disconnect());
  }

  private resize() {
    if (!this.renderer || !this.camera) return;
    const w = this.host.clientWidth || 1280, h = this.host.clientHeight || 720;
    this.renderer.setSize(w, h); this.camera.aspect = w / h; this.camera.updateProjectionMatrix();
  }

  private weaponMesh(): string {
    const a = this.ws.archetype;
    return a === 'melee' ? 'Knife01' : a === 'smg' || a === 'pistol' ? 'WPN_AK74U' : a === 'sniper' ? 'WPN_Mosin' : a === 'pulse' ? 'WPN_CubeGun' : 'WPN_AK47';
  }

  private makeDuck(bodyColor: number, weapon: string | null, crown: boolean, scale = 1.35): Any {
    const T = this.T, g = new T.Group(), inner = new T.Group(); g.add(inner); inner.scale.setScalar(scale);
    const m = (c: number, metal = 0) => new T.MeshStandardMaterial({ color: c, roughness: 0.65, metalness: metal });
    inner.add(new T.Mesh(duckParts.DuckBody, m(bodyColor)));
    const beak = new T.Mesh(duckParts.Duck_Beak, m(0xf39a2b)); beak.position.set(0, 0.84, 0.0); inner.add(beak);
    for (const s of [-1, 1]) {
      const f = new T.Mesh(duckParts.DuckFoot2, m(0xf39a2b)); f.position.set(0.12 * s, 0, 0.02); f.name = s < 0 ? 'footL' : 'footR'; inner.add(f);
      const gl = new T.Mesh(duckParts.DuckGlove, m(0x2b3036)); gl.scale.setScalar(0.8); gl.position.set(0.3 * s, 0.48, 0.12); inner.add(gl);
    }
    if (weapon) { const w = new T.Mesh(duckParts[weapon], m(0x2a2d31, 0.5)); w.position.set(0.24, 0.52, 0.3); inner.add(w); }
    if (crown) inner.add(new T.Mesh(duckParts.Crown, m(0xffc93b, 0.8)));
    return g;
  }

  private place(minD: number, maxD: number, avoid: Array<{ x: number; z: number }>, sep: number): { x: number; z: number; y: number } {
    const n = this.nav;
    for (const widen of [1, 1.5, 2.5, 100]) {
      const lo = minD / widen, hi = maxD * widen;
      const pool = n.cells.filter(k => { const d = n.dist[k]! * n.cell; return d >= lo && d <= hi; });
      const open = pool.filter(k => n.top[k]! <= n.W[k]! + 6);
      for (const list of [open, pool]) for (let tries = 0; tries < 200 && list.length; tries++) {
        const p = navPos(n, list[Math.floor(Math.random() * list.length)]!);
        if (!avoid.some(a => Math.hypot(a.x - p.x, a.z - p.z) < sep)) return p;
      }
    }
    return navPos(n, n.cells[Math.floor(Math.random() * n.cells.length)]!);
  }

  private populate() {
    const T = this.T, n = this.nav, z = this.zone, tut = !!this.opts.tutorial;
    const sp = navPos(n, n.spawn); this.px = sp.x; this.pz = sp.z; this.py = sp.y;
    this.camT = { x: sp.x, y: sp.y, z: sp.z };
    let maxD = 0; for (const k of n.cells) maxD = Math.max(maxD, n.dist[k]!); maxD *= n.cell;
    const used: Array<{ x: number; z: number }> = [sp];
    // 撤离点
    const exCount = tut ? 1 : 2;
    for (let i = 0; i < exCount; i++) {
      const p = tut ? this.place(30, 48, used, 0) : this.place(maxD * 0.55, maxD * 0.95, used, 60);
      used.push(p);
      const grp = new T.Group(); grp.position.set(p.x, p.y, p.z);
      const col = new T.Mesh(new T.CylinderGeometry(2.4, 2.4, 7, 40, 1, true), new T.MeshBasicMaterial({ color: 0x3dff8c, transparent: true, opacity: 0.18, side: T.DoubleSide, depthWrite: false, blending: T.AdditiveBlending })); col.position.y = 3.5;
      const ring = new T.Mesh(new T.RingGeometry(2.1, 2.5, 48), new T.MeshBasicMaterial({ color: 0x3dff8c, transparent: true, opacity: 0.9, depthWrite: false })); ring.rotation.x = -Math.PI / 2; ring.position.y = 0.08;
      const l = new T.PointLight(0x3dff8c, 40, 14, 1.5); l.position.y = 2.5;
      grp.add(col, ring, l); this.scene.add(grp); this.transient.push(grp);
      this.extracts.push({ x: p.x, z: p.z, y: p.y, obj: grp });
    }
    // 物资箱
    const crateN = tut ? 1 : z.lootCrateCount;
    for (let i = 0; i < crateN; i++) {
      const p = tut ? this.place(13, 20, used, 6) : this.place(14, maxD, used, 16); used.push(p);
      const tier = i === 0 ? 'military' : 'civilian';
      const grp = new T.Group(); grp.position.set(p.x, p.y, p.z); grp.rotation.y = Math.random() * Math.PI;
      const box = new T.Mesh(new T.BoxGeometry(1.1, 0.7, 0.75), new T.MeshStandardMaterial({ color: tier === 'military' ? 0x56603a : 0x7a5a3a, roughness: 0.8 })); box.position.y = 0.35;
      const edge = new T.LineSegments(new T.EdgesGeometry(box.geometry), new T.LineBasicMaterial({ color: 0xffcf5e })); edge.position.y = 0.35;
      const ring = new T.Mesh(new T.RingGeometry(0.9, 1.1, 32), new T.MeshBasicMaterial({ color: 0xffcf5e, transparent: true, opacity: 0.7, depthWrite: false })); ring.rotation.x = -Math.PI / 2; ring.position.y = 0.06;
      const l = new T.PointLight(0xffc860, 10, 6, 1.6); l.position.y = 1.4;
      grp.add(box, edge, ring, l); this.scene.add(grp); this.transient.push(grp);
      this.crates.push({ id: i, obj: grp, ring, x: p.x, z: p.z, y: p.y, opened: false, tier });
    }
    // 敌鸭
    const enemyN = tut ? 1 : z.enemyCount;
    for (let i = 0; i < enemyN; i++) {
      const boss = !tut && i === 0;
      const p = tut ? this.place(9, 16, [sp], 0) : boss ? this.place(maxD * 0.7, maxD, used, 10) : this.place(30, maxD, used, 12);
      used.push(p);
      const hp = tut ? 40 : boss ? 260 + z.dangerLevel * 60 : 55 + z.dangerLevel * 18;
      const obj = this.makeDuck(boss ? 0x8d8a80 : 0xa4553f, boss ? 'WPN_AK47' : 'WPN_AK74U', boss, boss ? 1.7 : 1.35);
      const ring = new T.Mesh(new T.RingGeometry(0.55, 0.7, 32), new T.MeshBasicMaterial({ color: boss ? 0xffb62e : 0xff4d4d, transparent: true, opacity: 0.8, depthWrite: false })); ring.rotation.x = -Math.PI / 2; ring.position.y = 0.06; obj.add(ring);
      obj.position.set(p.x, p.y, p.z); this.scene.add(obj); this.transient.push(obj);
      const bar = document.createElement('div'); bar.className = `r3-hp${boss ? ' boss' : ''}`; bar.innerHTML = `<i></i><span>${esc(boss ? z.bossName : tut ? '练习靶鸭' : '封锁区敌鸭')}</span>`; this.labels.appendChild(bar);
      this.enemies.push({ obj, x: p.x, z: p.z, y: p.y, hp, maxHp: hp, speed: tut ? 1.6 : boss ? 2.4 : 2.8 + Math.random() * 1.2, boss, alert: 0, shootCd: 1.5 + Math.random(), name: boss ? z.bossName : '封锁区敌鸭', dmg: tut ? 4 : 11 + z.dangerLevel * 3, tx: p.x, tz: p.z, stuck: 0, face: Math.random() * 6, bar, dead: false });
      if (tut) obj.visible = false;
    }
    if (tut) {
      const p = this.place(7, 10, [sp], 0);
      this.tutMarker = new T.Group(); this.tutMarker.position.set(p.x, p.y, p.z);
      const r = new T.Mesh(new T.RingGeometry(1.2, 1.6, 40), new T.MeshBasicMaterial({ color: 0xffd23e, transparent: true, opacity: 0.9, depthWrite: false })); r.rotation.x = -Math.PI / 2; r.position.y = 0.08;
      const c = new T.Mesh(new T.CylinderGeometry(1.4, 1.4, 4, 32, 1, true), new T.MeshBasicMaterial({ color: 0xffd23e, transparent: true, opacity: 0.15, side: T.DoubleSide, depthWrite: false, blending: T.AdditiveBlending })); c.position.y = 2;
      this.tutMarker.add(r, c); this.scene.add(this.tutMarker); this.transient.push(this.tutMarker);
    }
    this.buildMinimap();
  }

  private buildMinimap() {
    const n = this.nav, S = this.mini.width, img = new ImageData(S, S);
    let lo = Infinity, hi = -Infinity; for (const k of n.cells) { lo = Math.min(lo, n.W[k]!); hi = Math.max(hi, n.W[k]!); }
    for (let y = 0; y < S; y++) for (let x = 0; x < S; x++) {
      const k = Math.floor((S - 1 - y) / S * n.N) * n.N + Math.floor(x / S * n.N); const o = (y * S + x) * 4;
      if (n.ok[k]) { const t = (n.W[k]! - lo) / Math.max(1, hi - lo); img.data[o] = 40 + t * 50; img.data[o + 1] = 70 + t * 60; img.data[o + 2] = 80 + t * 40; img.data[o + 3] = 235; }
      else if (n.block[k] && n.W[k]! > -900) { img.data[o] = 18; img.data[o + 1] = 24; img.data[o + 2] = 32; img.data[o + 3] = 235; }
      else { img.data[o + 3] = 120; }
    }
    this.miniBg = img;
  }

  private bindInput() {
    const cv = this.renderer.domElement as HTMLCanvasElement;
    const kd = (e: KeyboardEvent) => {
      if (!this.ready) return; const k = e.key.toLowerCase(); this.keys.add(k);
      if (k === 'r') this.reload(); if (k === 'q') this.useMed(); if (k === 'e') this.interact();
      if (e.key === 'Escape') this.finish(false, '主动放弃');
      if (e.key === ' ' || k === 'shift') { e.preventDefault(); this.dash(); }
    };
    const ku = (e: KeyboardEvent) => this.keys.delete(e.key.toLowerCase());
    const mm = (e: MouseEvent) => { const r = cv.getBoundingClientRect(); this.mouse.x = ((e.clientX - r.left) / r.width) * 2 - 1; this.mouse.y = -((e.clientY - r.top) / r.height) * 2 + 1; };
    const md = (e: MouseEvent) => { if (e.button !== 0) return; this.firing = true; if (!this.lootOpen && this.reloading <= 0) this.fire(); }; const mu = () => { this.firing = false; };
    const wh = (e: WheelEvent) => { e.preventDefault(); this.camDist = clamp(this.camDist + Math.sign(e.deltaY) * 3, 24, 60); };
    const cm = (e: Event) => e.preventDefault();
    const blur = () => { this.keys.clear(); this.firing = false; };
    window.addEventListener('keydown', kd); window.addEventListener('keyup', ku); window.addEventListener('mouseup', mu); window.addEventListener('blur', blur);
    cv.addEventListener('mousemove', mm); cv.addEventListener('mousedown', md); cv.addEventListener('wheel', wh, { passive: false }); cv.addEventListener('contextmenu', cm);
    this.disposers.push(() => { window.removeEventListener('keydown', kd); window.removeEventListener('keyup', ku); window.removeEventListener('mouseup', mu); window.removeEventListener('blur', blur); cv.removeEventListener('mousemove', mm); cv.removeEventListener('mousedown', md); cv.removeEventListener('wheel', wh); cv.removeEventListener('contextmenu', cm); });
  }

  dispose() { if (this.finished) return; this.finished = true; this.cleanup(); }

  private cleanup() {
    cancelAnimationFrame(this.raf); this.disposers.forEach(d => d()); this.disposers = [];
    if (this.scene) {
      for (const o of [...this.transient, ...this.bullets.map(b => b.mesh), this.player, this.aimLine, this.rain]) if (o) { this.scene.remove(o); o.traverse?.((c: Any) => { if (c.isMesh || c.isLine || c.isPoints) { if (!Object.values(duckParts).includes(c.geometry)) c.geometry.dispose(); c.material.dispose?.(); } }); }
      if (mapCache) this.scene.remove(mapCache.root);
    }
    if (this.renderer) { this.renderer.clippingPlanes = []; this.renderer.domElement.remove(); }
  }

  private finish(extracted: boolean, reason: string) {
    if (this.finished) return; this.finished = true; this.cleanup();
    if (this.opts.tutorial && extracted) this.opts.onTutorialDone?.();
    this.onFinish(extracted, reason, this.usedMeds, this.loot, this.helmDur, this.armorDur);
  }

  // ---------- 地形查询 ----------
  private groundAt(x: number, z: number): number { const k = navIdx(this.nav, x, z); return k < 0 ? -999 : this.nav.W[k]!; }
  private canStand(x: number, z: number, fromY: number): boolean { const k = navIdx(this.nav, x, z); return k >= 0 && this.nav.ok[k] === 1 && Math.abs(this.nav.W[k]! - fromY) <= 0.6; }
  private lineClear(ax: number, az: number, ay: number, bx: number, bz: number, by: number): boolean {
    const d = Math.hypot(bx - ax, bz - az), steps = Math.ceil(d / (this.nav.cell * 1.5));
    for (let s = 1; s < steps; s++) { const t = s / steps, k = navIdx(this.nav, ax + (bx - ax) * t, az + (bz - az) * t); if (k < 0) return false; const h = ay + (by - ay) * t; const w = this.nav.W[k]!; if (w > h || (this.nav.block[k] && h > w + 0.4 && h < w + 1.9)) return false; }
    return true;
  }
  private moveEntity(e: { x: number; z: number; y: number }, dx: number, dz: number): boolean {
    let moved = false;
    if (this.canStand(e.x + dx, e.z, e.y)) { e.x += dx; moved = true; }
    if (this.canStand(e.x, e.z + dz, e.y)) { e.z += dz; moved = true; }
    const g = this.groundAt(e.x, e.z); if (g > -900) e.y += (g - e.y) * 0.5;
    return moved;
  }

  // ---------- 动作 ----------
  private reload() { if (this.ws.archetype !== 'melee' && this.reloading <= 0 && this.ammo < this.ws.magSize) { this.reloading = this.ws.reloadTime; sfx.play('click'); } }
  private useMed() { const med = this.meds.find(m => !this.usedMeds.includes(m.uid)); if (!med) return; const def = ITEM_DEFS[med.defId]; this.usedMeds.push(med.uid); this.hp = Math.min(this.maxHp, this.hp + (def?.med?.healAmount ?? 25)); if (def?.med?.stopsBleed) this.bleeding = false; sfx.play('inspect_ok'); this.addFloat(this.px, this.py + 2.2, this.pz, `+${def?.med?.healAmount ?? 25} HP`, '#52d681'); }
  private dash() {
    if (this.dashCd > 0 || this.lootOpen) return;
    this.dashCd = this.store.upgrades.has('upg_duck_conditioning') ? 1.8 : 2.3; this.dashT = 0.25; sfx.play('dash');
    const mv = this.moveInput(); this.dashDir = mv.x || mv.z ? mv : { ...this.aim };
    if (this.opts.tutorial && this.tutStep === 4) this.setTutStep(5);
  }
  private moveInput() {
    let f = 0, r = 0; const k = this.keys;
    if (k.has('w') || k.has('arrowup')) f += 1; if (k.has('s') || k.has('arrowdown')) f -= 1;
    if (k.has('d') || k.has('arrowright')) r += 1; if (k.has('a') || k.has('arrowleft')) r -= 1;
    const x = FWD.x * f + RIGHT.x * r, z = FWD.z * f + RIGHT.z * r, l = Math.hypot(x, z);
    return l ? { x: x / l, z: z / l } : { x: 0, z: 0 };
  }
  private nearCrate(): Crate | undefined { return this.crates.find(c => !c.opened && Math.hypot(c.x - this.px, c.z - this.pz) < 2.2); }
  private inExtract(): Extract | undefined { return this.extracts.find(e => Math.hypot(e.x - this.px, e.z - this.pz) < 2.4); }

  private interact() {
    if (this.lootOpen) return;
    const c = this.nearCrate(); if (!c) return;
    if (this.opts.tutorial && this.tutStep < 2) return;
    c.opened = true; this.lootOpen = c; c.ring.visible = false; c.obj.children[0].material.color.multiplyScalar(0.5);
    if (this.opts.tutorial && this.tutStep === 2) this.setTutStep(3);
    const n = this.opts.tutorial ? 2 : 2 + Math.floor(Math.random() * 2);
    const slots = Array.from({ length: n }, () => { const it = rollRaidLootItem(this.zone.id, Math.random); const dur = computeLootInspectDuration(ITEM_DEFS[it.defId]?.rarity ?? 0); return { it, dur, revealed: false }; });
    this.lootEl.innerHTML = `<div class="lootbox"><h3>📦 ${c.tier === 'military' ? '军用物资箱' : '民用物资箱'} · 点击逐个搜刮（品质越高读条越久）</h3>
    ${slots.map((s, i) => `<div class="lootslot" id="loot-${i}"><button data-loot="${i}">🔎 搜刮（约 ${s.dur.toFixed(1)}s）</button><span class="lootname"></span><span class="lootbar"><i></i></span></div>`).join('')}
    <button data-loot-close="1">继续战斗</button></div>`;
    this.lootEl.querySelectorAll<HTMLButtonElement>('[data-loot]').forEach(btn => btn.addEventListener('click', () => {
      const idx = Number(btn.dataset.loot); const s = slots[idx]!; if (s.revealed) return; s.revealed = true; btn.disabled = true;
      const bar = this.lootEl.querySelector<HTMLElement>(`#loot-${idx} .lootbar i`)!;
      const nameEl = this.lootEl.querySelector<HTMLElement>(`#loot-${idx} .lootname`)!;
      const t0 = performance.now();
      const tick = () => {
        const p = Math.min(1, (performance.now() - t0) / (s.dur * 1000)); bar.style.width = `${p * 100}%`;
        if (p < 1) { requestAnimationFrame(tick); return; }
        const def = ITEM_DEFS[s.it.defId]!;
        nameEl.innerHTML = `<img src="${itemIcon(s.it.defId)}" style="width:34px;height:34px;object-fit:contain;vertical-align:middle" onerror="this.style.display='none'"> <b style="color:${RARITY_COLORS[def.rarity]}">${esc(def.name)}</b> ${RARITY_NAMES[def.rarity]} · ${gradeName(s.it.actualGrade)} <button data-take="${idx}">放入背包</button>`;
        sfx.play(def.rarity >= 3 ? 'loot_gold' : 'inspect_ok');
        nameEl.querySelector('button')?.addEventListener('click', () => { this.loot.push(s.it); this.tutLooted = true; nameEl.textContent = '已放入背包 ✔'; this.renderHud(); });
      };
      requestAnimationFrame(tick);
    }));
    this.lootEl.querySelector('[data-loot-close]')?.addEventListener('click', () => {
      if (this.opts.tutorial && this.tutStep === 3 && !this.tutLooted) { this.flashCoach('先把至少一件东西「放入背包」，再点继续。'); return; }
      this.lootOpen = null; this.lootEl.innerHTML = '';
      if (this.opts.tutorial && this.tutStep === 3) this.setTutStep(4);
    });
  }

  private fire() {
    const now = performance.now() / 1000;
    if (now - this.lastShot < 1 / this.ws.fireRate) return;
    this.lastShot = now;
    const melee = this.ws.archetype === 'melee';
    if (!melee && this.ammo <= 0) { this.reload(); return; }
    if (!melee) this.ammo--;
    const ang = Math.atan2(this.aim.x, this.aim.z);
    const range = melee ? 2.4 : this.ws.range * 1.8;
    for (let p = 0; p < this.ws.pelletCount; p++) {
      const a = ang + (Math.random() - 0.5) * this.ws.spreadDeg * Math.PI / 180;
      this.spawnBullet(this.px + Math.sin(ang) * 0.6, this.py + SHOT_H, this.pz + Math.cos(ang) * 0.6, Math.sin(a), Math.cos(a), melee ? 18 : 48, range, this.ws.damage, true, this.ws.element === 'electric');
    }
    sfx.play(this.ws.element === 'electric' ? 'shoot_electric' : 'shoot');
    this.flashT = 0.06;
    for (const e of this.enemies) if (!e.dead && Math.hypot(e.x - this.px, e.z - this.pz) < this.ws.soundRadius * 1.3) e.alert = 6;
  }

  private spawnBullet(x: number, y: number, z: number, dx: number, dz: number, speed: number, range: number, dmg: number, fromPlayer: boolean, elec?: boolean) {
    const T = this.T;
    const mesh = new T.Mesh(new T.BoxGeometry(0.07, 0.07, 1.1), new T.MeshBasicMaterial({ color: fromPlayer ? (elec ? 0x7df9ff : 0xffe07a) : 0xff6a6a, transparent: true, opacity: 0.95, blending: T.AdditiveBlending, depthWrite: false }));
    mesh.position.set(x, y, z); mesh.rotation.y = Math.atan2(dx, dz); this.scene.add(mesh);
    this.bullets.push({ mesh, x, y, z, vx: dx * speed, vz: dz * speed, dmg, fromPlayer, left: range, elec });
  }

  private addFloat(x: number, y: number, z: number, text: string, color: string) {
    const el = document.createElement('div'); el.className = 'r3-float'; el.style.color = color; el.textContent = text; this.labels.appendChild(el);
    this.floats.push({ el, x, y, z, life: 1 });
  }

  // ---------- 教学 ----------
  private setTutStep(s: number) {
    this.tutStep = s;
    if (s === 1) { const e = this.enemies[0]; if (e) { e.obj.visible = true; e.alert = 99; } if (this.tutMarker) this.tutMarker.visible = false; }
    const st = TUT_STEPS[s]; if (!st) return;
    this.coach.style.display = '';
    this.coach.innerHTML = `<img src="/art/portraits/serpico.png" alt=""><div><small>师傅 · 塞尔皮科　${s + 1}/${TUT_STEPS.length}</small><b>${st.title}</b><p>${st.text}</p></div>`;
    sfx.play('bell');
  }
  private flashCoach(msg: string) { const p = this.coach.querySelector('p'); if (p) { p.innerHTML = `<b style="color:#ff8a5c">${esc(msg)}</b>`; } this.coach.classList.remove('r3-shake'); void this.coach.offsetWidth; this.coach.classList.add('r3-shake'); }

  // ---------- 主循环 ----------
  private update(dt: number) {
    if (this.lootOpen) { this.hudT += dt; if (this.hudT > 0.25) { this.hudT = 0; this.renderHud(); } return; }
    const tut = !!this.opts.tutorial;
    this.timeLeft -= dt; if (this.timeLeft <= 0) { this.finish(false, '超时未撤离'); return; }
    if (this.dashCd > 0) this.dashCd -= dt;
    if (this.reloading > 0) { this.reloading -= dt; if (this.reloading <= 0) this.ammo = this.ws.magSize; }
    // 瞄准：鼠标射线与胸口高度平面求交
    const T = this.T, ray = new T.Raycaster(); ray.setFromCamera(new T.Vector2(this.mouse.x, this.mouse.y), this.camera);
    const hit = new T.Vector3(); const aimY = this.py + SHOT_H;
    if (ray.ray.intersectPlane(new T.Plane(new T.Vector3(0, 1, 0), -aimY), hit)) { const dx = hit.x - this.px, dz = hit.z - this.pz, l = Math.hypot(dx, dz); if (l > 0.2) this.aim = { x: dx / l, z: dz / l }; this.aimDist = clamp(l, 1, 30); }
    // 移动
    const mv = this.moveInput(); const p = { x: this.px, z: this.pz, y: this.py };
    if (this.dashT > 0) { this.dashT -= dt; this.moveEntity(p, this.dashDir.x * 13 * dt, this.dashDir.z * 13 * dt); }
    else if (mv.x || mv.z) { this.moveEntity(p, mv.x * 5.2 * dt, mv.z * 5.2 * dt); this.walkPhase += dt * 11; }
    this.px = p.x; this.pz = p.z; this.py = p.y;
    if (this.bleeding) { this.bleedT += dt; if (this.bleedT > 1.2) { this.bleedT = 0; this.hp -= 2; this.checkDeath(); } }
    if (this.firing && this.reloading <= 0) this.fire();
    // 教学进度
    if (tut && this.tutStep === 0 && this.tutMarker && Math.hypot(this.tutMarker.position.x - this.px, this.tutMarker.position.z - this.pz) < 1.8) this.setTutStep(1);
    // 敌鸭 AI
    const helm = this.store.equippedHelmet(), arm = this.store.equippedArmor();
    for (const e of this.enemies) {
      if (e.dead || !e.obj.visible) continue;
      const dx = this.px - e.x, dz = this.pz - e.z, dist = Math.hypot(dx, dz);
      const vision = this.weather === 'storm_rain' ? 13 : 17;
      const los = dist < 26 && this.lineClear(e.x, e.z, e.y + SHOT_H, this.px, this.pz, this.py + SHOT_H);
      if (dist < vision && los) e.alert = Math.max(e.alert, 5);
      let mx = 0, mz = 0;
      if (e.alert > 0) {
        e.alert -= dt; e.face = Math.atan2(dx, dz);
        if (dist > 8 || !los) { mx = dx / dist; mz = dz / dist; }
        e.shootCd -= dt;
        if (e.shootCd <= 0 && los && dist < 20) {
          e.shootCd = tut ? 2.4 : e.boss ? 0.85 : 1.25 + Math.random() * 0.5;
          const a = e.face + (Math.random() - 0.5) * 0.12;
          this.spawnBullet(e.x + Math.sin(a) * 0.7, e.y + SHOT_H, e.z + Math.cos(a) * 0.7, Math.sin(a), Math.cos(a), 30, 26, e.dmg, false);
        }
      } else {
        const tdx = e.tx - e.x, tdz = e.tz - e.z, td = Math.hypot(tdx, tdz);
        if (td < 0.6 || e.stuck > 1.2) { e.stuck = 0; const a = Math.random() * Math.PI * 2, r = 4 + Math.random() * 8; e.tx = e.x + Math.sin(a) * r; e.tz = e.z + Math.cos(a) * r; }
        else { mx = (tdx / td) * 0.45; mz = (tdz / td) * 0.45; e.face = Math.atan2(tdx, tdz); }
      }
      if (mx || mz) { if (!this.moveEntity(e, mx * e.speed * dt, mz * e.speed * dt)) { e.stuck += dt; if (e.alert > 0 && e.stuck > 0.6) { e.stuck = 0; const s = Math.random() < 0.5 ? 1 : -1; this.moveEntity(e, -mz * s * e.speed * dt * 4, mx * s * e.speed * dt * 4); } } }
    }
    // 子弹
    for (const b of [...this.bullets]) {
      const step = Math.hypot(b.vx, b.vz) * dt, nx = b.x + b.vx * dt, nz = b.z + b.vz * dt;
      let dead = b.left <= 0 || !this.lineClear(b.x, b.z, b.y, nx, nz, b.y);
      b.left -= step;
      if (!dead && b.fromPlayer) {
        for (const e of this.enemies) {
          if (e.dead || !e.obj.visible || segDist(b.x, b.z, nx, nz, e.x, e.z) > (e.boss ? 0.8 : 0.6)) continue;
          const h = computeArmorDamage({ baseDamage: b.dmg, critRate: this.ws.critRate, critMult: this.ws.critMult, armorPiercing: this.ws.armorPiercing, armorBreak: this.ws.armorBreak, element: this.ws.element, bleedChance: this.ws.bleedChance, weather: this.weather, helmetLevel: 0, helmetDurability: 0, bodyArmorLevel: tut ? 0 : Math.min(6, this.zone.dangerLevel + (e.boss ? 2 : 1)), bodyArmorDurability: 80, critRoll: Math.random(), bleedRoll: Math.random() });
          e.hp -= h.finalDamage; e.alert = 6; dead = true;
          this.addFloat(e.x, e.y + 2, e.z, `${h.finalDamage}${h.isHeadshotCrit ? ' 爆头!' : ''}`, h.isHeadshotCrit ? '#ffb62e' : '#fff');
          if (h.isHeadshotCrit) sfx.play('headshot');
          if (e.hp <= 0) this.killEnemy(e);
          break;
        }
      }
      if (!dead && !b.fromPlayer && segDist(b.x, b.z, nx, nz, this.px, this.pz) < 0.5 && this.dashT <= 0) {
        const h = computeArmorDamage({ baseDamage: b.dmg, critRate: tut ? 0 : 0.18, critMult: 1.7, armorPiercing: this.zone.dangerLevel * 0.7, armorBreak: 4, element: 'physical', bleedChance: tut ? 0 : 0.14, weather: this.weather, helmetLevel: helm ? (ITEM_DEFS[helm.defId]?.armor?.armorLevel ?? 0) : 0, helmetDurability: this.helmDur, bodyArmorLevel: arm ? (ITEM_DEFS[arm.defId]?.armor?.armorLevel ?? 0) : 0, bodyArmorDurability: this.armorDur, critRoll: Math.random(), bleedRoll: Math.random() });
        this.hp -= h.finalDamage; this.helmDur = h.newHelmetDurability; this.armorDur = h.newBodyArmorDurability;
        if (tut) this.hp = Math.max(this.hp, 30);
        if (h.inflictedBleed) this.bleeding = true;
        this.addFloat(this.px, this.py + 2.2, this.pz, `-${h.finalDamage}`, '#ff5d5d'); sfx.play('hurt'); dead = true; this.checkDeath();
      }
      if (dead) { this.scene.remove(b.mesh); b.mesh.geometry.dispose(); b.mesh.material.dispose(); this.bullets = this.bullets.filter(x => x !== b); }
      else { b.x = nx; b.z = nz; b.mesh.position.set(nx, b.y, nz); }
    }
    for (const f of this.floats) { f.y += 1.2 * dt; f.life -= dt; if (f.life <= 0) f.el.remove(); }
    this.floats = this.floats.filter(f => f.life > 0);
    // 撤离
    const ex = this.inExtract();
    if (ex && (!tut || this.tutStep >= 5)) { this.extractT += dt; if (this.extractT > 3) { this.finish(true, '成功撤离'); return; } } else this.extractT = 0;
    if (ex && tut && this.tutStep < 5 && this.hudT === 0) this.flashCoach('先完成上面的步骤，再来撤离。');
    this.hudT += dt; if (this.hudT > 0.15) { this.hudT = 0; this.renderHud(); }
  }
  private aimDist = 8;

  private killEnemy(e: Enemy) {
    e.dead = true; e.bar.remove(); this.kills++;
    e.obj.rotation.z = Math.PI / 2; e.obj.position.y = e.y + 0.3;
    if (e.boss) { const bonus = rollRaidLootItem(this.zone.id, Math.random); this.loot.push(bonus); this.addFloat(e.x, e.y + 2.8, e.z, `首领击杀! 掉落 ${ITEM_DEFS[bonus.defId]?.name}`, '#ffb62e'); sfx.play('loot_gold'); }
    if (this.opts.tutorial && this.tutStep === 1) this.setTutStep(2);
  }
  private checkDeath() { if (this.hp <= 0) this.finish(false, '阵亡'); }

  private draw() {
    const T = this.T, t = performance.now() / 1000;
    // 鸭子姿态
    const face = Math.atan2(this.aim.x, this.aim.z);
    this.player.position.set(this.px, this.py, this.pz); this.player.rotation.y = face;
    const moving = this.moveInput().x !== 0 || this.moveInput().z !== 0;
    animateDuck(this.player, moving ? this.walkPhase : 0, this.dashT > 0);
    this.flash.intensity = this.flashT > 0 ? 60 : 0; if (this.flashT > 0) this.flashT -= 1 / 60;
    for (const e of this.enemies) { if (e.dead) continue; e.obj.position.set(e.x, e.y, e.z); e.obj.rotation.y = e.face; animateDuck(e.obj, t * 9 + e.face, false); }
    for (const c of this.crates) if (!c.opened) c.ring.material.opacity = 0.45 + 0.35 * Math.sin(t * 4);
    for (const x of this.extracts) x.obj.children[0].rotation.y = t * 0.6;
    if (this.tutMarker?.visible) this.tutMarker.children[0].scale.setScalar(1 + 0.08 * Math.sin(t * 5));
    // 瞄准线 + 手电
    const ax = this.px + this.aim.x * this.aimDist, az = this.pz + this.aim.z * this.aimDist, ay = this.py + SHOT_H;
    const pos = this.aimLine.geometry.attributes.position; pos.setXYZ(0, this.px + this.aim.x * 0.7, ay, this.pz + this.aim.z * 0.7); pos.setXYZ(1, ax, ay, az); pos.needsUpdate = true; this.aimLine.geometry.computeBoundingSphere();
    this.spot.position.set(this.px, this.py + 1.6, this.pz); this.spot.target.position.set(this.px + this.aim.x * 12, this.py, this.pz + this.aim.z * 12);
    // 镜头：鸭科夫 55° 俯角，略微偏向准星
    const lead = Math.min(this.aimDist, 10) * 0.3;
    const tx = this.px + this.aim.x * lead, tz = this.pz + this.aim.z * lead, ty = this.py + 0.8;
    const k = 1 - Math.pow(0.0015, 1 / 60 * 3); this.camT.x += (tx - this.camT.x) * k; this.camT.y += (ty - this.camT.y) * k; this.camT.z += (tz - this.camT.z) * k;
    this.camera.position.set(this.camT.x + CAM_DIR.x * this.camDist, this.camT.y + CAM_DIR.y * this.camDist, this.camT.z + CAM_DIR.z * this.camDist);
    this.camera.lookAt(this.camT.x, this.camT.y, this.camT.z);
    // 室内：切掉头顶屋顶（鸭科夫式剖切）
    const pk = navIdx(this.nav, this.px, this.pz); const indoor = pk >= 0 && this.nav.top[pk]! > this.py + 3.2;
    const want = indoor ? this.py + 2.9 : this.py + 60; this.clip.constant += (want - this.clip.constant) * 0.25;
    if (this.rain) { this.rain.position.set(this.camT.x, this.camT.y, this.camT.z); const a = this.rain.geometry.attributes.position; for (let i = 0; i < a.count; i++) { let y = a.getY(i) - 0.7; if (y < 0) y += 25; a.setY(i, y); } a.needsUpdate = true; }
    this.renderer.render(this.scene, this.camera);
    this.drawOverlays();
  }

  private project(x: number, y: number, z: number): { x: number; y: number; vis: boolean } {
    const v = new this.T.Vector3(x, y, z).project(this.camera); const w = this.host.clientWidth, h = this.host.clientHeight;
    return { x: (v.x * 0.5 + 0.5) * w, y: (-v.y * 0.5 + 0.5) * h, vis: v.z < 1 && Math.abs(v.x) < 1.1 && Math.abs(v.y) < 1.1 };
  }

  private drawOverlays() {
    for (const e of this.enemies) {
      if (e.dead) continue; const s = this.project(e.x, e.y + (e.boss ? 2.4 : 1.9), e.z);
      e.bar.style.display = s.vis && e.obj.visible ? '' : 'none'; e.bar.style.transform = `translate(${s.x - 30}px, ${s.y}px)`;
      (e.bar.firstElementChild as HTMLElement).style.width = `${Math.max(0, e.hp / e.maxHp) * 100}%`; e.bar.classList.toggle('alert', e.alert > 0);
    }
    for (const f of this.floats) { const s = this.project(f.x, f.y, f.z); f.el.style.transform = `translate(${s.x}px, ${s.y}px) translate(-50%,-50%)`; f.el.style.opacity = String(Math.min(1, f.life * 1.5)); }
    // 指向目标（教学目标 / 最近撤离点）的屏幕边缘箭头
    let tgt: { x: number; y: number; z: number } | null = null; let color = '#3dff8c';
    if (this.opts.tutorial) {
      if (this.tutStep === 0 && this.tutMarker) { tgt = this.tutMarker.position; color = '#ffd23e'; }
      else if (this.tutStep === 1) { const e = this.enemies.find(x => !x.dead); if (e) { tgt = { x: e.x, y: e.y, z: e.z }; color = '#ff5d5d'; } }
      else if (this.tutStep === 2) { const c = this.crates.find(x => !x.opened); if (c) { tgt = c; color = '#ffcf5e'; } }
      else if (this.tutStep >= 4) tgt = this.nearestExtract();
    } else tgt = this.nearestExtract();
    if (tgt) {
      const s = this.project(tgt.x, tgt.y + 1, tgt.z), w = this.host.clientWidth, h = this.host.clientHeight;
      const onScreen = s.vis && s.x > 40 && s.x < w - 40 && s.y > 90 && s.y < h - 60;
      const cx = w / 2, cy = h / 2; const ang = Math.atan2(s.y - cy, s.x - cx);
      const ex = onScreen ? s.x : cx + Math.cos(ang) * (Math.min(w, h) / 2 - 50), ey = onScreen ? s.y - 40 : cy + Math.sin(ang) * (Math.min(w, h) / 2 - 50);
      const d = Math.round(Math.hypot(tgt.x - this.px, tgt.z - this.pz));
      this.arrow.style.display = ''; this.arrow.style.color = color;
      this.arrow.style.transform = `translate(${ex}px, ${ey}px) translate(-50%,-50%) rotate(${onScreen ? 90 : (ang * 180) / Math.PI}deg)`;
      this.dist.style.display = ''; this.dist.textContent = `${d}m`; this.dist.style.transform = `translate(${ex}px, ${ey + 22}px) translate(-50%,0)`;
    } else { this.arrow.style.display = 'none'; this.dist.style.display = 'none'; }
    this.drawMinimap();
  }

  private nearestExtract() { let best: Extract | null = null, bd = Infinity; for (const e of this.extracts) { const d = Math.hypot(e.x - this.px, e.z - this.pz); if (d < bd) { bd = d; best = e; } } return best; }

  private drawMinimap() {
    const ctx = this.mini.getContext('2d'); if (!ctx || !this.miniBg) return;
    const S = this.mini.width, n = this.nav, span = n.N * n.cell;
    ctx.putImageData(this.miniBg, 0, 0);
    const m = (x: number, z: number) => [((x - n.minX) / span) * S, ((n.maxZ - z) / span) * S] as const;
    for (const x of this.extracts) { const [u, v] = m(x.x, x.z); ctx.fillStyle = '#3dff8c'; ctx.beginPath(); ctx.arc(u, v, 5, 0, 7); ctx.fill(); }
    for (const c of this.crates) if (!c.opened) { const [u, v] = m(c.x, c.z); ctx.fillStyle = '#ffcf5e'; ctx.fillRect(u - 2.5, v - 2.5, 5, 5); }
    for (const e of this.enemies) if (!e.dead && e.alert > 0 && e.obj.visible) { const [u, v] = m(e.x, e.z); ctx.fillStyle = e.boss ? '#ffb62e' : '#ff4d4d'; ctx.beginPath(); ctx.arc(u, v, 3, 0, 7); ctx.fill(); }
    const [u, v] = m(this.px, this.pz); ctx.fillStyle = '#ffd23e'; ctx.beginPath(); ctx.arc(u, v, 4, 0, 7); ctx.fill();
    ctx.strokeStyle = '#ffd23e'; ctx.beginPath(); ctx.moveTo(u, v); ctx.lineTo(u + this.aim.x * 10, v - this.aim.z * 10); ctx.stroke();
  }

  private renderHud() {
    if (!this.hud) return;
    const nc = this.nearCrate(), ex = this.inExtract(), ne = this.nearestExtract();
    const d = ne ? Math.round(Math.hypot(ne.x - this.px, ne.z - this.pz)) : 0;
    this.hud.innerHTML = `<div class="raidstats">
      <span class="zonename">${esc(this.opts.tutorial ? '新手教学 · ' + this.zone.name : this.zone.name)} ${this.weather === 'storm_rain' ? '⛈ 暴雨（火伤-15% 电伤+20%）' : '🌙 晴夜'}</span>
      <span>⏱ ${Math.ceil(this.timeLeft)}s</span><span class="${this.hp < 35 ? 'danger' : ''}">❤️ ${Math.max(0, Math.ceil(this.hp))}/${this.maxHp}${this.bleeding ? ' 🩸流血!按Q止血' : ''}</span>
      <span>🔫 ${this.ws.archetype === 'melee' ? '近战' : `${this.ammo}/${this.ws.magSize}`}${this.reloading > 0 ? ' 换弹中…' : ''}</span><span>💨 翻滚 ${this.dashCd > 0 ? this.dashCd.toFixed(1) + 's' : '就绪'}</span>
      <span>💊 医疗 ${this.meds.length - this.usedMeds.length}</span><span>🎒 战利品 ${this.loot.length}</span><span>☠ 击杀 ${this.kills}</span>
      <span>🪖盔 ${Math.round(this.helmDur)} 🛡甲 ${Math.round(this.armorDur)}</span>
      ${ex && (!this.opts.tutorial || this.tutStep >= 5) ? `<span class="extracting">撤离中 ${Math.round(this.extractT / 3 * 100)}% …别动!</span>` : `<span>🟢 最近撤离点 ${d}m（跟绿色箭头走）</span>`}
      ${nc ? `<span class="interact">按 E 开箱</span>` : ''}
    </div>`;
    this.host.classList.toggle('lowhp', this.hp < 35);
  }
}

function segDist(ax: number, az: number, bx: number, bz: number, px: number, pz: number): number {
  const dx = bx - ax, dz = bz - az, l2 = dx * dx + dz * dz; const t = l2 ? clamp(((px - ax) * dx + (pz - az) * dz) / l2, 0, 1) : 0;
  return Math.hypot(ax + dx * t - px, az + dz * t - pz);
}

function animateDuck(g: Any, phase: number, rolling: boolean) {
  const inner = g.children[0]; if (!inner) return;
  inner.position.y = phase ? Math.abs(Math.sin(phase)) * 0.06 : 0;
  inner.rotation.x = rolling ? 0.5 : 0;
  for (const c of inner.children) { if (c.name === 'footL') c.position.z = 0.02 + Math.sin(phase) * 0.09; if (c.name === 'footR') c.position.z = 0.02 - Math.sin(phase) * 0.09; }
}
