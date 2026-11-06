#!/usr/bin/env bash
# 一键发布到 GitHub Pages：按子路径打包 → 推送 site/ 到 gh-pages 分支（强制覆盖）。
# 在线地址：https://cloudfj77-cloud.github.io/doomsday-train/
set -euo pipefail
cd "$(dirname "$0")/.."
export PATH="$HOME/.local/bin:$PATH"
BASE=/doomsday-train/ OUT=site bash scripts/build.sh
tmp=$(mktemp -d)
cp -R site/. "$tmp/"
touch "$tmp/.nojekyll"
rev=$(git rev-parse --short HEAD)
(
  cd "$tmp"
  git init -q -b gh-pages
  git add -A
  git -c user.name=cloudfj77-cloud -c user.email=cloudfj77-cloud@users.noreply.github.com commit -qm "发布：源码 $rev"
  git push -qf git@github.com:cloudfj77-cloud/doomsday-train.git gh-pages
)
rm -rf "$tmp"
echo "已发布源码 $rev，约 1 分钟后生效：https://cloudfj77-cloud.github.io/doomsday-train/"
