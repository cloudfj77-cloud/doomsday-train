import UnityPy, os, re, json
D = "/Users/cloud/Volumes/itfs/019fcbbb-1cd0-74cd-a4b9-0ddeebf1d361/Escape from Duckov/Duckov_Data"
O = "art/scene"
os.makedirs(O + "/minimap", exist_ok=True)
os.makedirs(O + "/keyart", exist_ok=True)
scan = json.load(open('art/scene_tex_scan.json'))
key = re.compile(r'^(Map_|Title$|Splash|Cover|MainMenuBackground|Background$|TagillaPoster|SodaTeam|Logo_|BossScene_Albedo|MapCrop|D_SnowTownHall_Logo|level2-4 wal)', re.I)
done = set()
meta = []
for f, items in scan['match'].items():
    want = {pid for (t, n, pid) in items if t == 'Texture2D' and (n.startswith('MiniMap_') or key.match(n))}
    if not want:
        continue
    env = UnityPy.load(os.path.join(D, f))
    for o in env.objects:
        if o.path_id not in want or o.type.name != 'Texture2D':
            continue
        t = o.read()
        n = t.m_Name
        if n in done:
            continue
        done.add(n)
        sub = 'minimap' if n.startswith('MiniMap_') else 'keyart'
        try:
            img = t.image
            img.save(f"{O}/{sub}/{n}.png")
            meta.append({'name': n, 'file': f, 'w': t.m_Width, 'h': t.m_Height, 'kind': sub})
            print(sub, n, t.m_Width, t.m_Height)
        except Exception as e:
            print('fail', n, e)
json.dump(meta, open(O + '/meta.json', 'w'), ensure_ascii=False, indent=1)
