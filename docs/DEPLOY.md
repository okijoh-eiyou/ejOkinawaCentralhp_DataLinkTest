# デプロイ・本番移設メモ

最終更新: 2026-07-28
※接続情報（パスワード等）はこのファイルに書かない（CLAUDE.mdのルール）。実値はサーバー上の `~/.lw-denbun.env` と `appsettings.Development.json`（ともにGit管理外）のみ。

## 環境の入れ子構造

```
実機PC（開発時: 名嘉さんの机のUbuntuデスクトップ）← ホスト
└ VirtualBox（アプリ。VMを作って動かすソフト）
   └ VM「OIS_マイヘルスEJ_電子カルテ連携svr」（仮想のPC1台）
      └ Ubuntu Server 26.04（VMの中のOS。hostname: ubuntutest）
         ├ PostgreSQL（okicenhp-db。OSに直接インストール＝ネイティブ稼働）
         ├ 電文取込（order_text_importer）
         └ Docker
            └ コンテナ lw-denbun-check（本アプリ。※Docker不使用への切替を検討中・後述）
```

- VMはブリッジ接続（LAN上の独立した1台として見える）。IPは現状DHCP → **本番では固定IP推奨**
- 利用者はブラウザで `http://<VMのIP>/` を開くだけ（ポート80）
- WebサーバーはASP.NET Core内蔵の Kestrel（Apache/Nginx不使用）

## 現在の稼働状態（開発サーバー 192.168.1.96）

- **systemdサービス `lw-denbun-check` で稼働中**（2026-07-29切替・動作確認済み。Dockerは廃止・アンインストール済み）
- 配置: アプリ本体 `/opt/lw-denbun-check/`（self-contained発行＝.NETランタイム同梱、サーバーへの.NETインストール不要）
- ユニット: `/etc/systemd/system/lw-denbun-check.service`（自動起動enabled・落ちたら5秒後に自動再起動・ポート80は CAP_NET_BIND_SERVICE で許可）
- 接続文字列: `/home/administrator/.lw-denbun.env` を EnvironmentFile として読込
- DB接続先は `Host=localhost`（同一VM内のPostgreSQL）→ **VMを移設してIPが変わっても設定変更不要**
- 状態確認: `systemctl status lw-denbun-check` ／ ログ: `journalctl -u lw-denbun-check`

## 本番移設の方式（上司の方針）

**VMをovaファイルにエクスポートし、ユーザー先PCのVirtualBoxで復元（インポート）する。**

- ova = VMまるごと（Ubuntu＋PostgreSQL＋データ＋アプリ＋設定一式）を1ファイルに固めたエクスポート形式
- つまり「エクスポートした瞬間のVMの中身」がそのまま本番になる
- 移設先の要件:
  - VirtualBox本体がインストールされていること（ovaは再生データ、VirtualBoxはプレイヤーの関係）
  - ネットワーク割り当てをブリッジ接続にすること
  - VMのIPを固定すること（ブラウザで開くURLが変わらないように）
- 復元後のアプリ側作業: **なし**（DB接続がlocalhostのため）。利用者に新IPのURLを周知するだけ

## 決定事項（2026-07-28）

1. **Docker不使用へ切替する。方式は案2**: 自己完結型発行（`dotnet publish -r linux-x64 --self-contained`）＋ systemd サービス化（サーバーに.NETをインストールしない）
   - 自動起動・自動再起動は systemd、接続情報は `~/.lw-denbun.env` を流用、ポート80は systemd の権限付与（CAP_NET_BIND_SERVICE）で対応
   - **切替はovaエクスポートの前に済ませる**（ovaは「その時点の中身」を持っていくため）
2. 切替完了後、docker.io はアンインストールする
3. （未決）移設先での固定IPの値

## 切替手順（案2・5段階）

1. **SSH接続** — 前回作った鍵認証をそのまま流用（作業なし）
2. **ビルド（発行）** — Windows側で `dotnet publish -r linux-x64 --self-contained` → アプリ＋.NETランタンイムを1フォルダに固める
3. **成果物を転送** — 圧縮してサーバーへ → `/opt/lw-denbun-check/` に展開
4. **サービス登録（systemd）** — 自動起動・自動再起動・localhost接続・ポート80待受をユニットファイルに定義（sudo必要はここだけ）
5. **起動と後片付け** — サービス起動→ブラウザ確認→旧コンテナ停止・削除→docker.ioアンインストール

## 更新手順（切替後・systemd方式）

1. Windowsで修正 → `dotnet publish -r linux-x64 --self-contained`
2. 成果物を `scp` でサーバーの配置先へ差し替え
3. `sudo systemctl restart lw-denbun-check`

### 旧・Docker方式（参考。切替後は使わない）
1. ソースを `tar`＋`scp` で転送 → サーバーで `docker build -t lw-denbun-check .`
2. `docker rm -f lw-denbun-check` → `docker run -d ...` で差し替え

## 経緯メモ

- 2026-07-27: SSH開通（鍵認証設定）、Dockerfile作成、ソース転送
- 2026-07-28: Docker導入・ビルド・起動、http://192.168.1.96/ で動作確認（DB接続・検索10件表示OK）
- 2026-07-28: 上司方針の確認 — Docker不使用希望・ova方式での移設。案2に決定
- 2026-07-29: **systemd方式へ切替完了**（self-contained発行→/opt配置→サービス登録→docker.io削除→HTTP 200・検索10件で確認）
