import UnityPy, os, json, collections, sys, glob
UnityPy.config.FALLBACK_UNITY_VERSION = None
D="/Users/cloud/Volumes/itfs/019fcbbb-1cd0-74cd-a4b9-0ddeebf1d361/Escape from Duckov/Duckov_Data"
files=sorted([f for f in os.listdir(D) if (f.startswith('level') or f.startswith('sharedassets') or f in('resources.assets','globalgamemanagers','globalgamemanagers.assets')) and not f.endswith('.resS')])
tot=collections.Counter(); per={}; names=collections.defaultdict(list); ver=None
for f in files:
    try:
        env=UnityPy.load(os.path.join(D,f))
    except Exception as e:
        print('ERR',f,e);continue
    c=collections.Counter()
    for o in env.objects:
        t=o.type.name; c[t]+=1
        if f=='resources.assets' and t in('Texture2D','Sprite','Mesh','Shader','Material','AudioClip','Font','AnimationClip','GameObject','MonoBehaviour','TextAsset','VideoClip'):
            try:
                if t=='MonoBehaviour': continue
                n=o.peek_name() if hasattr(o,'peek_name') else o.read().m_Name
                names[t].append(n)
            except Exception: pass
    for sf in env.files.values():
        if ver is None and hasattr(sf,'unity_version'): ver=sf.unity_version
    per[f]=dict(c); tot.update(c); print(f, sum(c.values()), flush=True)
json.dump({'unity':str(ver),'total':dict(tot.most_common()),'per':per,'res_names':names},open('/Users/cloud/WorkBuddy/spaces/2026_11_07_15-28-09/duckov_re/art/inventory.json','w'),ensure_ascii=False,indent=1)
print('UNITY',ver); print(tot.most_common(40))
