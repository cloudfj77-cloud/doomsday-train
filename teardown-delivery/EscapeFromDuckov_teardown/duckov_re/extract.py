import UnityPy, os, json, collections, re
D="/Users/cloud/Volumes/itfs/019fcbbb-1cd0-74cd-a4b9-0ddeebf1d361/Escape from Duckov/Duckov_Data"
O="art"
for s in ("sprites","tex","text","mesh"): os.makedirs(f"{O}/{s}",exist_ok=True)
out={}
g=UnityPy.load(D+"/globalgamemanagers")
for o in g.objects:
    try:
        if o.type.name=="BuildSettings": out['scenes']=o.read_typetree().get('scenes')
    except Exception as e: print('bs',e)
env=UnityPy.load(D+"/resources.assets")
texinfo=[]; prefix=collections.Counter(); fmt=collections.Counter(); size=collections.Counter()
shaders=[]; matshader=collections.Counter(); meshes=[]
want_tex=re.compile(r'^(T_|IG_|Pfb_|WeaponParts|Duck|Character|Head|Body|Face|Beak|Hair|Eye)',re.I)
want_mesh={'WPN_AK47','WPN_Mosin','WPN_AK74U','Crown','Knife01','Vechile_Island_Moto1','Item_Crop_1_Banana','WPN_Watergun','Grenade03','SM_MedicalBag_02_1','WPN_CubeGun'}
nt=0; errs=collections.Counter()
for o in env.objects:
    t=o.type.name
    try:
        if t=="Texture2D":
            d=o.read(); w,h=d.m_Width,d.m_Height; n=d.m_Name or ''
            texinfo.append((n,w,h,str(d.m_TextureFormat)))
            m=re.match(r'^([A-Za-z]+_)',n) or re.match(r'^([A-Za-z]{2,})',n)
            prefix[m.group(1) if m else '?']+=1
            fmt[str(d.m_TextureFormat).split('.')[-1]]+=1; size[max(w,h)]+=1
            if nt<300 and want_tex.match(n) and not re.search(r'_(N|M|MS|R|AO|Normal)$',n):
                d.image.convert('RGBA').save(f"{O}/tex/{n}.png"); nt+=1
        elif t=="Sprite":
            d=o.read(); d.image.save(f"{O}/sprites/{d.m_Name}.png")
        elif t=="Shader":
            d=o.read_typetree(); shaders.append((d.get('m_ParsedForm') or {}).get('m_Name') or d.get('m_Name'))
        elif t=="Material":
            d=o.read_typetree(); pid=d['m_Shader']['m_PathID']; fid=d['m_Shader']['m_FileID']
            matshader[(fid,pid)]+=1
        elif t=="Mesh":
            d=o.read(); n=d.m_Name
            vc=d.m_VertexData.m_VertexCount if hasattr(d,'m_VertexData') else 0
            meshes.append((n,vc))
            if n in want_mesh or n.lower().startswith('duck'):
                open(f"{O}/mesh/{n}.obj","w").write(d.export())
        elif t=="TextAsset":
            d=o.read(); s=d.m_Script
            s=s.encode('utf-8','surrogateescape') if isinstance(s,str) else bytes(s)
            open(f"{O}/text/{d.m_Name}_{o.path_id}.txt","wb").write(s)
    except Exception as e:
        errs[(t,type(e).__name__)]+=1
# resolve material shader names (same-file only)
sid={}
for o in env.objects:
    if o.type.name=="Shader":
        try: sid[o.path_id]=(o.read_typetree().get('m_ParsedForm') or {}).get('m_Name')
        except Exception: pass
ms=collections.Counter()
for (fid,pid),c in matshader.items(): ms[sid.get(pid, f'ext:{fid}') if fid==0 else f'ext_file{fid}']+=c
out['tex_count']=len(texinfo); out['tex_prefix']=prefix.most_common(40); out['tex_fmt']=fmt.most_common(); out['tex_size']=sorted(size.items())
out['shaders']=sorted(set(s for s in shaders if s)); out['mat_shader']=ms.most_common(60)
meshes.sort(key=lambda x:-(x[1] or 0)); out['mesh_top']=meshes[:40]; out['mesh_count']=len(meshes)
vs=sorted(m[1] for m in meshes if m[1]); out['mesh_vert_median']=vs[len(vs)//2] if vs else 0
out['errors']={str(k):v for k,v in errs.items()}
json.dump(out,open(O+"/art_summary.json","w"),ensure_ascii=False,indent=1,default=str)
print('done',nt,dict(errs))
