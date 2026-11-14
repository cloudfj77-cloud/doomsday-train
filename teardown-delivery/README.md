# 两款游戏完整逆向/拆解包（内部）

- `EscapeFromDuckov_teardown/` — 《逃离鸭科夫》拆解（含 decompiled 源码、GLB 地图、OBJ 等）
- `ProbablyStolen_teardown/` — 《可能被偷了》拆解（含 decompiled 源码与美术）

仅限内部使用，勿外传。未收录重复的 `.zip`（与解压目录内容相同；Duckov zip 超 GitHub 单文件 100MB 限制）。

本地跑鸭科夫黑店 3D 夜袭时，可将本目录放到 `/data/workspace/teardown-delivery/`，或改 `scripts/sync-art.sh` 中的 `DUCKOV_RE` 指向：
`teardown-delivery/EscapeFromDuckov_teardown/duckov_re`
