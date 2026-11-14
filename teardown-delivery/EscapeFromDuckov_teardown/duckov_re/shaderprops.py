import UnityPy, json
D = "/Users/cloud/Volumes/itfs/019fcbbb-1cd0-74cd-a4b9-0ddeebf1d361/Escape from Duckov/Duckov_Data"
env = UnityPy.load(D + "/resources.assets")
want = {'SodaCraft/SodaLit', 'SodaCraft/SodaCharacter', 'SodaCraft/SodaLit_EdgeLight', 'Shader Graphs/SodaDecal', 'SodaSoftParticle', 'SmokeFireFX'}
res = {}
for o in env.objects:
    if o.type.name != "Shader":
        continue
    t = o.read_typetree()
    pf = t.get('m_ParsedForm') or {}
    n = pf.get('m_Name')
    if n in want and n not in res:
        props = [(p['m_Name'], p.get('m_Description'), p.get('m_Type')) for p in pf.get('m_PropInfo', {}).get('m_Props', [])]
        kw = t.get('m_KeywordNames') or pf.get('m_KeywordNames') or []
        subs = pf.get('m_SubShaders', [])
        passes = []
        for s in subs:
            for p in s.get('m_Passes', []):
                passes.append(p.get('m_Name') or (p.get('m_State') or {}).get('m_Name'))
        res[n] = {'props': props, 'keywords': kw[:80], 'passes': passes}
json.dump(res, open('art/shader_props.json', 'w'), ensure_ascii=False, indent=1)
for k, v in res.items():
    print('==', k)
    print(' props:', [p[0] + ('(' + str(p[1]) + ')' if p[1] and p[1] != p[0] else '') for p in v['props']])
    print(' kw:', v['keywords'][:60])
    print(' passes:', v['passes'])
