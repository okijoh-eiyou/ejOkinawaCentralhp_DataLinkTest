# CLAUDE.md

このリポジトリで作業するときは、以下の指示に必ず従うこと。

## プロジェクト概要

沖縄中央病院・栄養課向けの**受信電文確認Webアプリ**（ASP.NET Core MVC / .NET 8）。
電子カルテ「Live S」（株式会社ライブワークス）からTCP/IPソケット連携（給食I/F Bタイプ仕様）で届く食事オーダー電文が、
正しくDBに保存・展開されているかを目視確認する読み取り専用ツール。

- 屋宜原病院版（WPF: `C:\git-file_eiyou\smax_Confirmation_of_received_telegram`）の載せ替え版
- 本番はUbuntu VM上のDockerで稼働予定。栄養課PCはブラウザでアクセスする
- データを閲覧するのみ。業務テーブルへのINSERT/UPDATE/DELETEは実装しない

## 構成

```
ejOkinawaCentralhp_DataLinkTest.sln
└── lw_Confirmation_of_received_telegram\   ← 唯一のプロジェクト（MVC）
    ├── Models\          M_xxx = lw_xxxテーブル対応クラス / M_View_xxx = 画面表示用（テーブル対応なし）
    ├── Services\        Connect_PostgreSQL.cs（Dapper + Npgsql、DI登録済み）
    ├── Controllers\
    └── Views\
```

- 実行: `dotnet run`（またはVSでF5）
- DBスキーマ定義（DDL）: `C:\git-file_eiyou\ejOkinawaCentralhp\database\tables\` の `lw_*` テーブル群（上司管理・別リポジトリ）
- 開発用DB: `appsettings.Development.json` に接続文字列あり（後述）

## 絶対に守る設計ルール

1. **SQLは必ずパラメータ渡し。** `Connect_PostgreSQL.GetDataList_SQL<T>(sql, param)` に `@名前` + 匿名オブジェクトで渡す。
   文字列連結（`"... = '" + 値 + "'"`）と `SQL.Replace("@1", ...)` は禁止（屋宜原版の悪い例。真似しない）。
2. **患者番号は string のまま扱う。** int変換しない（DB列はVARCHAR(10)、I/F仕様も文字型。屋宜原版のInt32.Parseクラッシュを持ち込まない）。
3. **マスタテーブルが空でも動くこと。** 名前解決できない場合はコードをそのまま表示する（マスタデータは未投入の期間がある）。
4. **DB接続断でも画面は必ず表示する。** DBアクセスはtry/catchで包み、失敗時はエラー内容を画面に出す（HomeController.Index() のパターンを踏襲）。
5. **接続情報は `appsettings.Development.json` のみに書く。** このファイルは.gitignore対象。接続文字列・パスワードを
   appsettings.json、ソースコード、このCLAUDE.mdに書くこと・コミットすることは禁止。

## ドメイン知識（前提）

- `lw_order_log`(テーブル) が電文の原本。`order_items`(列・JSONB) に電文全体、GENERATED列で検索用の値を自動展開
- 電文のライフサイクルは3フラグ: `is_processed`(判定済み) → `is_active`(有効/無効) → `is_appended`(展開済み)。画面ではこの3つをセットで見せる
- 展開先は8テーブル: `lw_patient_context` / `lw_meal` / `lw_comment` / `lw_contra_comment` / `lw_other_comment` / `lw_disease` / `lw_tube_feeding` / `lw_free_comment`（すべて `log_id`(列) で原本に紐づく）
- 更新区分: A=追加 / D=削除 / I=一括削除（指定日時以降を全削除）。更新は「I+A」のセットで届く。削除対象がなくてもI電文が来るのは正常（I/F仕様）
- コメント類は朝昼夕ごとに最大20件（屋宜原版の「全体で10件」とは違う）
- JSONキーは「開始**時**区分」（屋宜原版の「開始時間区分」とは異なる。コピペ注意）

## 解説・レビュー時の書き方

- 識別子には種類を付記する: `lw_meal`(テーブル)、`GetDataList_SQL`(メソッド)、`patient_number`(列) のように書く
- 説明は日本語で行う

## 進行状況の記録

- 承認済み実装計画: `C:\Users\t.asato\.claude\plans\misty-pondering-sunrise.md`
  （Step 0疎通・Step 1雛形は完了。残り: Step 2画面実装＝受信モニタ/患者検索/電文詳細、Step 3検証）
- 画面実装は t.asato さんが自分で書く方針。Claudeは手順案内・質問対応・コードレビューを担当する
