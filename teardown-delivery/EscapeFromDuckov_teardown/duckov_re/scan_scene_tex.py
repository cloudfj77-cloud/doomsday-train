import UnityPy, os, re, json, collections
D = "/Users/cloud/Volumes/itfs/019fcbbb-1cd0-74cd-a4b9-0ddeebf1d361/Escape from Duckov/Duckov_Data"
files = sorted([f for f in os.listdir(D) if (f.startswith('sharedassets') or f == 'resources.assets') and f.endswith('.assets')], key=lambda x: (len(x), x))
pat = re.compile(r'(map|minimap|loading|cover|splash|title|logo|cg|scene|level|bg|background|poster|ending|key ?art|lightmap)', re.I)
res = collections.defaultdict(list)
big = []
for f in files:
    env = UnityPy.load(os.path.join(D, f))
    for o in env.objects:
        if o.type.name not in ('Texture2D', 'Sprite'):
            continue
        try:
            d = o.peek_name() if hasattr(o, 'peek_name') else None
        except Exception:
            d = None
        if d is None:
            try:
                d = o.read().m_Name
            except Exception:
                continue
        if pat.search(d or ''):
            res[f].append((o.type.name, d, o.path_id))
        if o.type.name == 'Texture2D':
            try:
                t = o.read()
                if max(t.m_Width, t.m_Height) >= 1024:
                    big.append((f, t.m_Name, t.m_Width, t.m_Height, o.path_id))
            except Exception:
                pass
json.dump({'match': res, 'big': big}, open('art/scene_tex_scan.json', 'w'), ensure_ascii=False, indent=1)
for f, v in res.items():
    print(f, len(v), [x[1] for x in v][:40])
print('BIG', len(big))
for b in big[:200]:
    print(b)
