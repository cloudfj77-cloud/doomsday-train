import os, base64, json
R = '/Users/cloud/WorkBuddy/spaces/2026_11_07_15-28-09/duckov_re'
os.chdir(R)
os.makedirs('viewer/js', exist_ok=True)
for f in sorted(os.listdir('viewer')):
    if f.endswith('.glb'):
        n = f[:-4]
        b = base64.b64encode(open(f'viewer/{f}', 'rb').read()).decode()
        open(f'viewer/js/{n}.glb.js', 'w').write(f'(window.__GLB=window.__GLB||{{}})[{json.dumps(n)}]="{b}";\n')
        print(n, round(len(b) / 1e6, 1), 'MB')
for f in sorted(os.listdir('art/mesh')):
    if f.endswith('.obj'):
        n = f[:-4]
        open(f'viewer/js/{n}.obj.js', 'w').write(f'(window.__OBJ=window.__OBJ||{{}})[{json.dumps(n)}]={json.dumps(open("art/mesh/" + f).read())};\n')
print('ok')
