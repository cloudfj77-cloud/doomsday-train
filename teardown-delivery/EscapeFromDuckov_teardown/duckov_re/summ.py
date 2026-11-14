import json
d = json.load(open('art/art_summary.json'))
for k in ('tex_count', 'tex_prefix', 'tex_fmt', 'tex_size', 'mat_shader', 'mesh_count', 'mesh_vert_median'):
    print(k, d[k])
print('shaders', len(d['shaders']), d['shaders'][:150])
print('mesh_top', d['mesh_top'][:15])
scenes = d.get('scenes') or []
print('scenes', len(scenes))
print([x.split('/')[-1] for x in scenes])
for f in ('DuckBody', 'Duck_Beak', 'WPN_AK47', 'Vechile_Island_Moto1'):
    L = open(f'art/mesh/{f}.obj').read().splitlines()
    print(f, sum(1 for l in L if l.startswith('v ')), 'verts', sum(1 for l in L if l.startswith('f ')), 'faces')
