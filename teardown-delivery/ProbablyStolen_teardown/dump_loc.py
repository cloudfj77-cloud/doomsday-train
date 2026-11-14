import UnityPy, json, sys, os, glob
base="Probably Stolen_Data/StreamingAssets/aa/StandaloneWindows64/"
out="_teardown/loc/"
# shared: key ids
env=UnityPy.load(base+"localization-assets-shared_assets_all.bundle")
keys={}
for obj in env.objects:
    if obj.type.name=="MonoBehaviour":
        try:
            t=obj.read_typetree()
        except Exception as e:
            print("err",e);continue
        name=t.get("m_Name") or t.get("m_TableCollectionName")
        entries=t.get("m_Entries")
        if entries:
            keys[t.get("m_TableCollectionName",name)]={e["m_Id"]:e["m_Key"] for e in entries}
print("shared tables:",{k:len(v) for k,v in keys.items()})
for lang in ["english(en)","chinese(simplified)(zh)"]:
    env=UnityPy.load(base+f"localization-string-tables-{lang}_assets_all.bundle")
    res={}
    for obj in env.objects:
        if obj.type.name=="MonoBehaviour":
            t=obj.read_typetree()
            nm=t.get("m_Name","")
            ent=t.get("m_TableData")
            if ent is None: continue
            coll=nm.rsplit("_",1)[0]
            km=keys.get(coll,{})
            res[nm]={km.get(e["m_Id"],str(e["m_Id"])):e["m_Localized"] for e in ent}
    json.dump(res,open(out+lang.split("(")[0]+".json","w"),ensure_ascii=False,indent=1)
    print(lang,{k:len(v) for k,v in res.items()})
