import os, random
from PIL import Image, ImageDraw, ImageFont
import numpy as np
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d.art3d import Poly3DCollection

A = 'art'
os.makedirs(A + '/preview', exist_ok=True)
try:
    FONT = ImageFont.truetype('/System/Library/Fonts/Supplemental/Arial.ttf', 11)
except Exception:
    FONT = ImageFont.load_default()


def sheet(files, out, cell=128, cols=10, bg=(245, 245, 240), label=True, title=None):
    rows = (len(files) + cols - 1) // cols
    top = 36 if title else 0
    W, H = cols * cell, rows * (cell + (16 if label else 0)) + top
    img = Image.new('RGB', (W, H), bg)
    d = ImageDraw.Draw(img)
    if title:
        d.text((10, 10), title, fill=(30, 30, 30), font=ImageFont.truetype('/System/Library/Fonts/Supplemental/Arial.ttf', 18) if FONT else None)
    for i, f in enumerate(files):
        try:
            im = Image.open(f).convert('RGBA')
        except Exception:
            continue
        im.thumbnail((cell - 8, cell - 8))
        x = (i % cols) * cell
        y = top + (i // cols) * (cell + (16 if label else 0))
        # checker bg for alpha
        tile = Image.new('RGBA', (cell - 4, cell - 4), (232, 232, 226, 255))
        img.paste(tile, (x + 2, y + 2))
        img.paste(im, (x + (cell - im.width) // 2, y + (cell - im.height) // 2), im)
        if label:
            n = os.path.splitext(os.path.basename(f))[0][:20]
            d.text((x + 4, y + cell), n, fill=(60, 60, 60), font=FONT)
    img.save(out, quality=88)
    print(out, img.size)


random.seed(7)
sp = sorted(os.listdir(A + '/sprites'))
# item icons: larger sprites tend to be item icons
sized = []
for f in sp:
    p = f'{A}/sprites/{f}'
    try:
        w, h = Image.open(p).size
    except Exception:
        continue
    sized.append((p, w, h))
icons = [p for p, w, h in sized if 120 <= max(w, h) <= 600 and 0.6 < w / max(h, 1) < 1.7]
ui = [p for p, w, h in sized if max(w, h) < 120]
print('icons', len(icons), 'ui', len(ui))
sheet(random.sample(icons, min(80, len(icons))), A + '/preview/sprites_icons.jpg', cell=120, cols=10, title='Sprites - item / UI icons (sample)')
sheet(random.sample(ui, min(60, len(ui))), A + '/preview/sprites_ui_small.jpg', cell=72, cols=15, title='Sprites - small UI glyphs (sample)')

tx = sorted(os.listdir(A + '/tex'))
cats = {
    'char': [f for f in tx if any(k in f.lower() for k in ('duck', 'beak', 'hair', 'eye', 'face', 'clothes', 'helmet', 'body', 'head'))],
    'weapon': [f for f in tx if any(k in f.lower() for k in ('wpn', 'weapon', 'muzzle', 'scope', 'stock', 'gun', 'rifle', 'pfb_'))],
}
cats['env'] = [f for f in tx if f not in cats['char'] and f not in cats['weapon']]
for k, v in cats.items():
    print(k, len(v))
    sel = v if len(v) <= 40 else random.sample(v, 40)
    sheet([f'{A}/tex/{f}' for f in sel], A + f'/preview/tex_{k}.jpg', cell=150, cols=8, title=f'Textures - {k} (albedo sample)')


def load_obj(p):
    V, F = [], []
    for l in open(p):
        if l.startswith('v '):
            V.append([float(x) for x in l.split()[1:4]])
        elif l.startswith('f '):
            F.append([int(x.split('/')[0]) - 1 for x in l.split()[1:]])
    return np.array(V), F


meshes = ['DuckBody', 'Duck_Beak', 'DuckFoot', 'WPN_AK47', 'WPN_Mosin', 'WPN_Watergun', 'Vechile_Island_Moto1', 'Crown', 'Item_Crop_1_Banana', 'SM_MedicalBag_02_1', 'Grenade03', 'Knife01']
fig = plt.figure(figsize=(16, 9), facecolor='#f7f7f3')
light = np.array([0.4, -0.5, 0.8]); light /= np.linalg.norm(light)
for i, m in enumerate(meshes):
    p = f'{A}/mesh/{m}.obj'
    if not os.path.exists(p):
        continue
    V, F = load_obj(p)
    if len(V) == 0:
        continue
    V = V[:, [0, 2, 1]]
    V[:, 0] *= -1
    tris = [V[f] for f in F if len(f) >= 3 and max(f) < len(V)]
    cols = []
    for t in tris:
        n = np.cross(t[1] - t[0], t[2] - t[0]); nn = np.linalg.norm(n)
        s = 0.35 + 0.65 * abs(np.dot(n / nn, light)) if nn > 0 else 0.5
        cols.append((0.95 * s, 0.78 * s, 0.35 * s) if 'Duck' in m else (0.55 * s, 0.62 * s, 0.72 * s))
    ax = fig.add_subplot(3, 4, i + 1, projection='3d')
    ax.add_collection3d(Poly3DCollection(tris, facecolors=cols, edgecolor=(0, 0, 0, 0.08), linewidths=0.2))
    mn, mx = V.min(0), V.max(0); c = (mn + mx) / 2; r = (mx - mn).max() / 2
    ax.set_xlim(c[0] - r, c[0] + r); ax.set_ylim(c[1] - r, c[1] + r); ax.set_zlim(c[2] - r, c[2] + r)
    ax.view_init(18, -60); ax.set_axis_off(); ax.set_facecolor('#f7f7f3')
    ax.set_title(f'{m}\n{len(V)} verts / {len(tris)} tris', fontsize=9)
plt.tight_layout()
plt.savefig(A + '/preview/meshes.jpg', dpi=110)
print('meshes ok')
