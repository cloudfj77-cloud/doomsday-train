#!/bin/bash
set -e
cd "$(dirname "$0")/.."
rm -rf dist/art
mkdir -p dist/art
cp -r web-art/portraits web-art/items web-art/scene web-art/wanted web-art/maps web-art/chars dist/art/
cp web-art/manifest.json dist/art/ 2>/dev/null || true
# 挂接拆解包自带的 3D GLB 场景浏览器（9 张 GLB 大地图 + 16 个 OBJ 网格，软链零拷贝）
DUCKOV_RE="/data/workspace/teardown-delivery/EscapeFromDuckov_teardown/duckov_re"
if [ -d "$DUCKOV_RE/viewer" ]; then
  rm -rf dist/explorer3d
  mkdir -p dist/explorer3d
  cp "$DUCKOV_RE/explorer.html" dist/explorer3d/index.html
  ln -sfn "$DUCKOV_RE/viewer" dist/explorer3d/viewer
  ln -sfn "$DUCKOV_RE/art" dist/explorer3d/art
  ln -sfn "$DUCKOV_RE/shots" dist/explorer3d/shots
fi
# 3D 夜袭用的 three.js（本地托管，不依赖外网 CDN）+ 原版鸭子/武器 OBJ
THREE_SRC="/data/workspace/forgeax-sdk-0.4.19/.forgeax/cli-runtime/node_modules/.pnpm/three@0.184.0/node_modules/three"
rm -rf dist/vendor/three && mkdir -p dist/vendor/three/build dist/vendor/three/jsm
cp "$THREE_SRC/build/three.module.js" "$THREE_SRC/build/three.core.js" dist/vendor/three/build/
cp -r "$THREE_SRC/examples/jsm/loaders" "$THREE_SRC/examples/jsm/utils" dist/vendor/three/jsm/
grep -rl "from 'three'" dist/vendor/three/jsm | xargs sed -i "s#from 'three'#from '/vendor/three/build/three.module.js'#g"
cat > dist/vendor/three/duckov-three.js <<'JS'
import * as THREE from '/vendor/three/build/three.module.js';
import { GLTFLoader } from '/vendor/three/jsm/loaders/GLTFLoader.js';
import { OBJLoader } from '/vendor/three/jsm/loaders/OBJLoader.js';
globalThis.__duckovThree = { THREE, GLTFLoader, OBJLoader };
JS
mkdir -p dist/art/mesh
cp "$DUCKOV_RE"/art/mesh/*.obj dist/art/mesh/
echo "art synced: $(find dist/art -type f | wc -l) files + explorer3d linked"
