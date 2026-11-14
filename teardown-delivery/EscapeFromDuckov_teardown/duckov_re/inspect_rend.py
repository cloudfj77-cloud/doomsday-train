import UnityPy, sys, collections, numpy as np
from UnityPy.helpers.MeshHelper import MeshHandler
D = "/Users/cloud/Volumes/itfs/019fcbbb-1cd0-74cd-a4b9-0ddeebf1d361/Escape from Duckov/Duckov_Data"
lv = int(sys.argv[1])
env = UnityPy.load(f"{D}/level{lv}")
go = {}; mf = {}
for o in env.objects:
    if o.type.name == 'GameObject':
        t = o.read_typetree(); go[o.path_id] = (t['m_Name'], t['m_Layer'], t['m_IsActive'])
    elif o.type.name == 'MeshFilter':
        t = o.read_typetree(); mf[t['m_GameObject']['m_PathID']] = o
rows = []
shc = collections.Counter(); layc = collections.Counter(); cast = collections.Counter()
for o in env.objects:
    if o.type.name != 'MeshRenderer': continue
    t = o.read_typetree(); g = t['m_GameObject']['m_PathID']
    if g not in mf: continue
    r = o.read()
    m = mf[g].read().m_Mesh
    try:
        me = m.read(); mn = me.m_Name; b = me.m_LocalAABB.m_Extent; ext = max(b.x, b.y, b.z)
    except Exception:
        mn = '?'; ext = 0
    sh = []
    mats = []
    for mp in r.m_Materials:
        try:
            ma = mp.read(); mats.append(ma.m_Name)
            s = ma.m_Shader.read(); sh.append(s.m_ParsedForm.m_Name if s.m_ParsedForm else s.m_Name)
        except Exception: sh.append('?')
    for s in sh: shc[s] += 1
    layc[go[g][1]] += 1; cast[t['m_CastShadows']] += 1
    rows.append((go[g][0], mn, round(ext, 1), go[g][1], t['m_CastShadows'], mats[:2], sh[:2]))
print('shaders', shc.most_common(40))
print('layers', layc.most_common()); print('castShadows', cast)
for r in rows:
    if any(k in (r[0] + r[1]).lower() for k in ('light', 'fog', 'dark', 'cover', 'roof', 'ceil', 'black', 'shadow', 'volume', 'mask')) or r[2] > 40:
        print(r)
