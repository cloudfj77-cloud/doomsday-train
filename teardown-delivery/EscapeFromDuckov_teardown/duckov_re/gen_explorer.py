import os, json, glob
R = '/Users/cloud/WorkBuddy/spaces/2026_11_07_15-28-09/duckov_re'
os.chdir(R)
CUT = {'Base': 2.4, 'JLab': 3.0}
SC = [
    ('Base', ['Base', 'BaseSub'], '地堡（基地）', 'MiniMap_Base_SceneV2'),
    ('GroundZero', ['GroundZero'], '零号区', 'MiniMap_Level_GroundZero_1'),
    ('HiddenWarehouse', ['HiddenWarehouse'], '仓库区', 'MiniMap_Level_HiddenWarehouse'),
    ('Farm', ['Farm'], '农场镇', 'MiniMap_Level_Farm_01'),
    ('JLab', ['JLab'], 'J-Lab 实验室', 'MiniMap_Level_JLab_1'),
    ('Desert', ['Desert'], '沙漠', 'MiniMap_Level_Desert'),
    ('StormZone', ['StormZone'], '风暴区', 'MiniMap_Level_StormZone_1'),
    ('SnowMilitaryBase', ['SnowMilitaryBase'], '37 号实验区（雪地）', 'MiniMap_Level_SnowMilitaryBase'),
]
scenes = []
for key, parts, cn, mm in SC:
    infos = [json.load(open(f'viewer/{p}.json')) for p in parts if os.path.exists(f'viewer/{p}.json') and os.path.exists(f'viewer/{p}.glb')]
    if not infos: continue
    f0 = infos[0]['focus']
    scenes.append({'key': key, 'cn': cn, 'glbs': [f'viewer/{i["name"]}.glb' for i in infos], 'minimap': f'art/scene/minimap/{mm}.png',
                   'renderers': sum(i['renderers'] for i in infos), 'tris': sum(i['tris'] for i in infos), 'mb': round(sum(i['size_mb'] for i in infos), 1),
                   'textures': sum(i['textures'] for i in infos), 'instances': sum(i['stats'].get('instances', 0) for i in infos),
                   'focus': f0, 'top': infos[0]['top_meshes'][:8]})
    if CUT.get(key) is not None: scenes[-1]['cut'] = CUT[key]


def lst(d, ext=('.png', '.jpg')):
    return sorted(f for f in os.listdir(d) if f.lower().endswith(ext))


from PIL import Image
def dims(p):
    try: return Image.open(p).size
    except Exception: return (0, 0)

minimaps = [{'f': f'art/scene/minimap/{f}', 'n': f[:-4].replace('MiniMap_', ''), 'wh': dims(f'art/scene/minimap/{f}')} for f in lst('art/scene/minimap')]
keyart = [{'f': f'art/scene/keyart/{f}', 'n': f[:-4], 'wh': dims(f'art/scene/keyart/{f}')} for f in lst('art/scene/keyart') if dims(f'art/scene/keyart/{f}')[0] >= 200]
sprites = [{'f': f'art/sprites/{f}', 'n': f[:-4]} for f in lst('art/sprites')]
texs = [{'f': f'art/tex/{f}', 'n': f[:-4]} for f in lst('art/tex')]
models = []
for f in lst('art/mesh', ('.obj',)):
    L = open(f'art/mesh/{f}').read().splitlines()
    models.append({'f': f'art/mesh/{f}', 'n': f[:-4], 'v': sum(1 for l in L if l.startswith('v ')), 't': sum(1 for l in L if l.startswith('f '))})
manifest = {'scenes': scenes, 'minimaps': minimaps, 'keyart': keyart, 'sprites': sprites, 'tex': texs, 'models': models}

html = open('explorer_template.html', encoding='utf-8').read().replace('/*__MANIFEST__*/null', json.dumps(manifest, ensure_ascii=False))
open('explorer.html', 'w', encoding='utf-8').write(html)
print('scenes', [(s['key'], s['mb']) for s in scenes], 'sprites', len(sprites), 'tex', len(texs), 'models', len(models), 'keyart', len(keyart))
