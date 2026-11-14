"""Reconstruct a Unity built scene (levelN) into a textured GLB.
Usage: python3 build_scene.py <levelIndex> <outName> [maxTex]
"""
import UnityPy, os, sys, json, struct, io, time, math, collections
import numpy as np
from PIL import Image
from UnityPy.helpers.MeshHelper import MeshHandler

D = "/Users/cloud/Volumes/itfs/019fcbbb-1cd0-74cd-a4b9-0ddeebf1d361/Escape from Duckov/Duckov_Data"
lv = int(sys.argv[1]); name = sys.argv[2]
TEX = int(sys.argv[3]) if len(sys.argv) > 3 else 256
OUT = 'viewer'; os.makedirs(OUT, exist_ok=True)
t0 = time.time()
env = UnityPy.load(f"{D}/level{lv}")
objs = {o.path_id: o for o in env.objects}

# ---------- hierarchy ----------
tr, go_active, go_tr, go_name, go_layer = {}, {}, {}, {}, {}
mf, mr, terrains = {}, {}, []
for o in env.objects:
    tn = o.type.name
    if tn in ('Transform', 'RectTransform'):
        t = o.read_typetree()
        p, q, s = t['m_LocalPosition'], t['m_LocalRotation'], t['m_LocalScale']
        tr[o.path_id] = (t['m_Father']['m_PathID'], (p['x'], p['y'], p['z']), (q['x'], q['y'], q['z'], q['w']), (s['x'], s['y'], s['z']))
        go_tr[t['m_GameObject']['m_PathID']] = o.path_id
    elif tn == 'GameObject':
        t = o.read_typetree(); go_active[o.path_id] = t['m_IsActive']; go_name[o.path_id] = t['m_Name']; go_layer[o.path_id] = t['m_Layer']
    elif tn == 'MeshFilter':
        t = o.read_typetree(); mf[t['m_GameObject']['m_PathID']] = o
    elif tn == 'MeshRenderer':
        t = o.read_typetree(); mr[t['m_GameObject']['m_PathID']] = (t, o)
    elif tn == 'Terrain':
        terrains.append(o)
tr_go = {v: k for k, v in go_tr.items()}


def qmat(q):
    x, y, z, w = q
    return np.array([[1 - 2 * (y * y + z * z), 2 * (x * y - z * w), 2 * (x * z + y * w)],
                     [2 * (x * y + z * w), 1 - 2 * (x * x + z * z), 2 * (y * z - x * w)],
                     [2 * (x * z - y * w), 2 * (y * z + x * w), 1 - 2 * (x * x + y * y)]])


wcache, acache = {}, {}


def world(tid):
    if tid in wcache: return wcache[tid]
    f, p, q, s = tr[tid]
    M = np.eye(4); M[:3, :3] = qmat(q) * np.array(s); M[:3, 3] = p
    if f and f in tr: M = world(f) @ M
    wcache[tid] = M; return M


def active(tid):
    if tid in acache: return acache[tid]
    g = tr_go.get(tid); a = go_active.get(g, 1) == 1
    f = tr[tid][0]
    if a and f and f in tr: a = active(f)
    acache[tid] = a; return a


sys.setrecursionlimit(100000)

# ---------- mesh / material caches ----------
mesh_cache, mat_cache, tex_cache = {}, {}, {}


def get_mesh(pptr):
    key = (pptr.assetsfile.name if hasattr(pptr, 'assetsfile') else '', pptr.file_id, pptr.path_id)
    if key in mesh_cache: return mesh_cache[key]
    r = None
    try:
        m = pptr.read()
        h = MeshHandler(m); h.process()
        V = np.array(h.m_Vertices, dtype=np.float32).reshape(-1, 3)
        nv = len(V)
        uv = np.array(h.m_UV0, dtype=np.float32).reshape(nv, -1)[:, :2] if h.m_UV0 is not None and len(h.m_UV0) else np.zeros((nv, 2), np.float32)
        N = np.array(h.m_Normals, dtype=np.float32).reshape(nv, -1)[:, :3] if h.m_Normals is not None and len(h.m_Normals) else None
        subs = [np.array(t, dtype=np.uint32).reshape(-1, 3) for t in h.get_triangles()]
        if len(uv) != len(V): uv = np.zeros((len(V), 2), np.float32)
        if N is not None and len(N) != len(V): N = None
        r = (m.m_Name, V, uv, N, subs)
    except Exception as e:
        r = None
    mesh_cache[key] = r; return r


def tex_img(pptr):
    key = (pptr.file_id, pptr.path_id, id(pptr.assetsfile))
    try:
        t = pptr.read()
    except Exception:
        return None
    k2 = (t.assets_file.name, t.object_reader.path_id) if hasattr(t, 'object_reader') else t.m_Name
    if k2 in tex_cache: return tex_cache[k2]
    try:
        im = t.image.convert('RGB'); im.thumbnail((TEX, TEX))
        b = io.BytesIO(); im.save(b, 'JPEG', quality=82)
        arr = np.asarray(im.resize((16, 16))).reshape(-1, 3).mean(0) / 255
        r = (k2, b.getvalue(), arr)
    except Exception:
        r = None
    tex_cache[k2] = r; return r


def get_mat(pptr):
    key = (pptr.file_id, pptr.path_id, id(pptr.assetsfile))
    if key in mat_cache: return mat_cache[key]
    res = {'name': '?', 'color': (0.7, 0.7, 0.7, 1), 'tex': None, 'shader': '', 'transparent': False}
    try:
        m = pptr.read()
        res['name'] = m.m_Name
        try:
            sh = m.m_Shader.read(); res['shader'] = sh.m_ParsedForm.m_Name if sh.m_ParsedForm else sh.m_Name
        except Exception: pass
        props = m.m_SavedProperties
        cols = dict(props.m_Colors) if isinstance(props.m_Colors, list) else props.m_Colors
        texs = dict(props.m_TexEnvs) if isinstance(props.m_TexEnvs, list) else props.m_TexEnvs
        for k in ('_Tint', '_BaseColor', '_Color'):
            if k in cols:
                c = cols[k]; res['color'] = (c.r, c.g, c.b, c.a); break
        for k in ('_MainTex', '_BaseMap', '_Albedo', '_MainTexture'):
            if k in texs and texs[k].m_Texture and texs[k].m_Texture.path_id:
                ti = tex_img(texs[k].m_Texture)
                if ti:
                    res['tex'] = ti; res['st'] = (texs[k].m_Scale.x, texs[k].m_Scale.y, texs[k].m_Offset.x, texs[k].m_Offset.y)
                break
        s = res['shader'].lower()
        if any(x in s for x in ('particle', 'decal', 'fx', 'lazer', 'overlay', 'billboard', 'hidden', 'ring', 'range', 'smoke', 'zeropoint', 'buildinggrid', 'worldtile', 'unlit', 'teleporter', 'mosaic', 'sound', 'map')):
            res['skip'] = True
        if 'water' in s:
            res['tex'] = None; res['color'] = (0.36, 0.56, 0.68, 1)
    except Exception:
        pass
    mat_cache[key] = res; return res


# ---------- bake ----------
groups = collections.defaultdict(lambda: {'P': [], 'N': [], 'U': [], 'I': [], 'n': 0})
S = np.diag([-1, 1, 1, 1]).astype(np.float64)  # unity LH -> gltf RH
stats = collections.Counter(); names = collections.Counter(); records = []
INST_MIN = 3
for g, (rt, ro) in mr.items():
    if not rt.get('m_Enabled', True): stats['disabled'] += 1; continue
    if g not in mf or g not in go_tr: continue
    tid = go_tr[g]
    if not active(tid): stats['inactive'] += 1; continue
    mfo = mf[g].read()
    mesh = get_mesh(mfo.m_Mesh)
    if not mesh: stats['nomesh'] += 1; continue
    mname, V, uv, N, subs = mesh
    W = world(tid)
    mats = ro.read().m_Materials
    names[mname] += 1; stats['renderers'] += 1
    mkey = id(mesh)
    for si in range(len(subs)):
        if si >= len(mats) or not mats[si] or not mats[si].path_id: continue
        mat = get_mat(mats[si])
        if mat.get('skip'): stats['skipfx'] += 1; continue
        records.append((mkey, si, id(mat), W, mesh, mat))

# count instances per (mesh, submesh, material)
cnt = collections.Counter((r[0], r[1], r[2]) for r in records)
inst = collections.defaultdict(list)
for mkey, si, matid, W, mesh, mat in records:
    k = (mkey, si, matid)
    if cnt[k] >= INST_MIN:
        inst[k].append((W, mesh, mat)); continue
    mname, V, uv, N, subs = mesh
    tri = subs[si]
    M = S @ W
    Vw = (np.c_[V, np.ones(len(V))] @ M.T)[:, :3].astype(np.float32)
    flip = np.linalg.det(M[:3, :3]) < 0
    Nw = None
    if N is not None:
        Nm = np.linalg.inv(M[:3, :3]).T
        Nw = (N @ Nm.T); Nw /= (np.linalg.norm(Nw, axis=1, keepdims=True) + 1e-9); Nw = Nw.astype(np.float32)
    key = (mat['tex'][0] if mat['tex'] else None, tuple(round(x, 3) for x in mat['color'][:3]))
    G = groups[key]; G['mat'] = mat
    used = np.unique(tri)
    remap = np.full(len(V), -1, np.int64); remap[used] = np.arange(len(used))
    t2 = remap[tri]
    if not flip: t2 = t2[:, [0, 2, 1]]
    st = mat.get('st', (1, 1, 0, 0))
    G['P'].append(Vw[used]); G['U'].append(uv[used] * np.array(st[:2]) + np.array(st[2:]))
    G['N'].append(Nw[used] if Nw is not None else np.zeros((len(used), 3), np.float32))
    G['I'].append((t2 + G['n']).astype(np.uint32)); G['n'] += len(used)
stats['instanced_groups'] = len(inst); stats['instances'] = sum(len(v) for v in inst.values())

# ---------- terrain ----------
for o in terrains:
    try:
        t = o.read_typetree()
        td = o.read().m_TerrainData.read()
        tt = td.read_typetree() if hasattr(td, 'read_typetree') else td.object_reader.read_typetree()
        hm = tt['m_Heightmap']; res = hm['m_Resolution']; sc = hm['m_Scale']
        H = np.array(hm['m_Heights'], dtype=np.float32).reshape(res, res) / 32766.0
        step = max(1, (res - 1) // 256); H = H[::step, ::step]; r = H.shape[0]
        gid = tr_go and go_tr.get(t['m_GameObject']['m_PathID'])
        M = S @ world(gid)
        xs = np.arange(r) * step * sc['x']; zs = np.arange(r) * step * sc['z']
        X, Z = np.meshgrid(xs, zs)
        P = np.c_[X.ravel(), (H * sc['y']).ravel(), Z.ravel(), np.ones(r * r)] @ M.T
        P = P[:, :3].astype(np.float32)
        ii = np.arange(r * r).reshape(r, r)
        a, b, c, d = ii[:-1, :-1].ravel(), ii[:-1, 1:].ravel(), ii[1:, :-1].ravel(), ii[1:, 1:].ravel()
        I = np.r_[np.c_[a, c, b], np.c_[b, c, d]].astype(np.uint32)
        I = I[:, [0, 2, 1]]
        # color from splat layers
        col = (0.45, 0.6, 0.3, 1); tex = None
        try:
            layers = tt['m_SplatDatabase'].get('m_TerrainLayers', [])
            if layers:
                L = env.objects[0].assets_file  # noqa
        except Exception: pass
        try:
            lay = td.m_SplatDatabase.m_TerrainLayers[0].read()
            ti = tex_img(lay.m_DiffuseTexture)
            if ti: tex = ti; col = (1, 1, 1, 1)
            tsz = lay.m_TileSize
        except Exception:
            tsz = None
        U = np.c_[X.ravel(), Z.ravel()].astype(np.float32) / (np.array([tsz.x, tsz.y]) if tsz else 10)
        mat = {'name': 'Terrain', 'color': col, 'tex': tex}
        G = groups[('terrain', id(o))]; G['mat'] = mat
        G['P'].append(P); G['U'].append(U.astype(np.float32)); G['N'].append(np.zeros_like(P)); G['I'].append(I + G['n']); G['n'] += len(P)
        stats['terrain'] += 1
        print('terrain', res, sc)
    except Exception as e:
        print('terrain fail', type(e).__name__, e)

# ---------- write GLB ----------
bin_ = bytearray(); bufferViews = []; accessors = []; meshes_prims = []; images = []; textures = []; materials = []; img_idx = {}


def add_view(b, target=None):
    while len(bin_) % 4: bin_.append(0)
    off = len(bin_); bin_.extend(b)
    bv = {'buffer': 0, 'byteOffset': off, 'byteLength': len(b)}
    if target: bv['target'] = target
    bufferViews.append(bv); return len(bufferViews) - 1


def add_acc(arr, typ, ctype, target, mm=False):
    v = add_view(arr.tobytes(), target)
    a = {'bufferView': v, 'componentType': ctype, 'count': int(len(arr) if arr.ndim == 1 else arr.shape[0]), 'type': typ}
    if mm: a['min'] = arr.min(0).tolist(); a['max'] = arr.max(0).tolist()
    accessors.append(a); return len(accessors) - 1


tv = ttri = 0; utv = 0
allP = []; mat_idx = {}; nodes = []; meshes = []


def mat_index(mat):
    k = id(mat)
    if k in mat_idx: return mat_idx[k]
    m = {'name': mat['name'][:40], 'pbrMetallicRoughness': {'baseColorFactor': [float(max(0, min(1, x))) for x in mat['color'][:3]] + [1.0], 'metallicFactor': 0.0, 'roughnessFactor': 0.85}, 'doubleSided': True}
    if mat['tex']:
        k2, jpg, _ = mat['tex']
        if k2 not in img_idx:
            images.append({'bufferView': add_view(jpg), 'mimeType': 'image/jpeg'}); textures.append({'source': len(images) - 1, 'sampler': 0}); img_idx[k2] = len(textures) - 1
        m['pbrMetallicRoughness']['baseColorTexture'] = {'index': img_idx[k2]}
    materials.append(m); mat_idx[k] = len(materials) - 1; return mat_idx[k]


def prim(P, U, N, I, mat):
    attrs = {'POSITION': add_acc(P, 'VEC3', 5126, 34962, True), 'TEXCOORD_0': add_acc(U.astype(np.float32), 'VEC2', 5126, 34962)}
    if N is not None and not (np.abs(N).sum(1) == 0).any():
        attrs['NORMAL'] = add_acc(N.astype(np.float32), 'VEC3', 5126, 34962)
    return {'attributes': attrs, 'indices': add_acc(I.astype(np.uint32).ravel(), 'SCALAR', 5125, 34963), 'material': mat_index(mat)}


# merged (unique) geometry
merged_prims = []
for key, G in groups.items():
    if not G['P']: continue
    P = np.concatenate(G['P']).astype(np.float32); U = np.concatenate(G['U']); N = np.concatenate(G['N']); I = np.concatenate(G['I'])
    allP.append(P)
    merged_prims.append(prim(P, U, N, I, G['mat']))
    tv += len(P); ttri += len(I); utv += len(P)
meshes.append({'primitives': merged_prims, 'name': 'merged'}); nodes.append({'mesh': 0, 'name': 'merged'})


def decompose(M):
    A = M[:3, :3].copy(); s = np.linalg.norm(A, axis=0); s[s == 0] = 1e-6
    if np.linalg.det(A) < 0: s[0] = -s[0]
    R = A / s
    u, _, vt = np.linalg.svd(R); R = u @ vt
    if np.linalg.det(R) < 0: R[:, 0] *= -1; s[0] = -s[0]
    w = math.sqrt(max(0, 1 + R[0, 0] + R[1, 1] + R[2, 2])) / 2
    x = math.sqrt(max(0, 1 + R[0, 0] - R[1, 1] - R[2, 2])) / 2
    y = math.sqrt(max(0, 1 - R[0, 0] + R[1, 1] - R[2, 2])) / 2
    z = math.sqrt(max(0, 1 - R[0, 0] - R[1, 1] + R[2, 2])) / 2
    x = math.copysign(x, R[2, 1] - R[1, 2]); y = math.copysign(y, R[0, 2] - R[2, 0]); z = math.copysign(z, R[1, 0] - R[0, 1])
    return M[:3, 3], (x, y, z, w), s


inst_pos = []
for (mkey, si, matid), L in inst.items():
    W0, mesh, mat = L[0]
    mname, V, uv, N, subs = mesh
    tri = subs[si]
    used = np.unique(tri); remap = np.full(len(V), -1, np.int64); remap[used] = np.arange(len(used))
    t2 = remap[tri][:, [0, 2, 1]]
    P = V[used].copy(); P[:, 0] *= -1
    Nl = None
    if N is not None:
        Nl = N[used].copy(); Nl[:, 0] *= -1
    st = mat.get('st', (1, 1, 0, 0))
    U = uv[used] * np.array(st[:2]) + np.array(st[2:])
    meshes.append({'primitives': [prim(P.astype(np.float32), U, Nl, t2, mat)], 'name': mname[:40]})
    T, Rq, Sc = [], [], []
    for W, _, _ in L:
        Mp = S @ W @ S
        t, q, s = decompose(Mp); T.append(t); Rq.append(q); Sc.append(s)
    T = np.array(T, np.float32); inst_pos.append(T)
    nodes.append({'mesh': len(meshes) - 1, 'name': mname[:40], 'extensions': {'EXT_mesh_gpu_instancing': {'attributes': {
        'TRANSLATION': add_acc(T, 'VEC3', 5126, None), 'ROTATION': add_acc(np.array(Rq, np.float32), 'VEC4', 5126, None), 'SCALE': add_acc(np.array(Sc, np.float32), 'VEC3', 5126, None)}}}})
    tv += len(P) * len(L); ttri += len(t2) * len(L); utv += len(P)

gltf = {'asset': {'version': '2.0', 'generator': 'duckov_re'}, 'scene': 0, 'scenes': [{'nodes': list(range(len(nodes)))}],
        'nodes': nodes, 'meshes': meshes, 'extensionsUsed': ['EXT_mesh_gpu_instancing'],
        'materials': materials, 'buffers': [{'byteLength': 0}], 'bufferViews': bufferViews, 'accessors': accessors,
        'samplers': [{'magFilter': 9729, 'minFilter': 9987, 'wrapS': 10497, 'wrapT': 10497}]}
if images: gltf['images'] = images; gltf['textures'] = textures
while len(bin_) % 4: bin_.append(0)
gltf['buffers'][0]['byteLength'] = len(bin_)
js = json.dumps(gltf, separators=(',', ':')).encode()
while len(js) % 4: js += b' '
with open(f'{OUT}/{name}.glb', 'wb') as f:
    f.write(struct.pack('<III', 0x46546C67, 2, 12 + 8 + len(js) + 8 + len(bin_)))
    f.write(struct.pack('<II', len(js), 0x4E4F534A)); f.write(js)
    f.write(struct.pack('<II', len(bin_), 0x004E4942)); f.write(bin_)
AP = np.concatenate(allP + inst_pos)
obj = np.array([(S @ r[3])[:3, 3] for r in records]) if records else AP
focus = [np.percentile(obj, 3, axis=0).tolist(), np.percentile(obj, 97, axis=0).tolist()]
info = {'name': name, 'level': lv, 'renderers': stats['renderers'], 'verts': int(tv), 'tris': int(ttri), 'unique_verts': int(utv), 'focus': focus, 'draw_groups': len(meshes), 'materials': len(materials), 'textures': len(textures),
        'bbox': [AP.min(0).tolist(), AP.max(0).tolist()], 'stats': dict(stats), 'top_meshes': names.most_common(25), 'size_mb': round(os.path.getsize(f'{OUT}/{name}.glb') / 1e6, 1), 'sec': round(time.time() - t0, 1)}
json.dump(info, open(f'{OUT}/{name}.json', 'w'), ensure_ascii=False, indent=1)
print(json.dumps({k: v for k, v in info.items() if k != 'top_meshes'}, ensure_ascii=False))
print(names.most_common(15))
