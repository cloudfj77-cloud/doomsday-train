import UnityPy, os, sys, time, json, collections
D = "/Users/cloud/Volumes/itfs/019fcbbb-1cd0-74cd-a4b9-0ddeebf1d361/Escape from Duckov/Duckov_Data"
scenes = json.load(open('art/art_summary.json'))['scenes']
for i in [3, 5, 6, 11, 12, 14, 19, 34, 48]:
    print(i, scenes[i].split('/')[-1], os.path.getsize(f"{D}/level{i}") // 1024, 'KB')
lv = int(sys.argv[1]) if len(sys.argv) > 1 else 14
t0 = time.time()
env = UnityPy.load(f"{D}/level{lv}")
c = collections.Counter(o.type.name for o in env.objects)
print('level', lv, scenes[lv], c.most_common(12))
sf = list(env.files.values())[0]
print('externals', [e.path for e in sf.externals][:10])
n = 0
for o in env.objects:
    if o.type.name == 'MeshFilter':
        d = o.read()
        try:
            m = d.m_Mesh.read()
            print('mesh ok', m.m_Name, d.m_Mesh.file_id, d.m_Mesh.path_id)
        except Exception as e:
            print('mesh fail', d.m_Mesh.file_id, d.m_Mesh.path_id, type(e).__name__, e)
        n += 1
        if n > 5:
            break
print('t', time.time() - t0)
