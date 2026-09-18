namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// マスタ保守: 1マスタ分の定義。
    /// テーブル名・列名はこの固定定義（ホワイトリスト）だけをSQLに使い、ユーザー入力は絶対に混ぜない。
    /// 定義の出典: C:\git-file_eiyou\ejOkinawaCentralhp\database\tables の DDL（No は DDLファイル番号）
    /// </summary>
    public class M_View_MasterDef
    {
        /// <summary>URL用の識別子（例: contra_comment → /Master/Edit?table=contra_comment）</summary>
        public string Slug { get; init; } = "";

        /// <summary>DDLファイル番号（11〜21。依頼・DDLとの突き合わせ用）</summary>
        public int No { get; init; }

        /// <summary>物理テーブル名（lw_m_xxx）</summary>
        public string TableName { get; init; } = "";

        /// <summary>画面表示名（DDLの COMMENT ON TABLE と同じ）</summary>
        public string DisplayName { get; init; } = "";

        /// <summary>優先度表示（★=最優先 / 〇=優先 / 空=通常）</summary>
        public string Priority { get; init; } = "";

        /// <summary>編集対象の列（id・created_at・updated_at は含めない）</summary>
        public List<M_View_MasterColumnDef> Columns { get; init; } = new();

        /// <summary>ユニークキーを構成する列（DDLのユニークインデックスと同じ。重複チェックに使う）</summary>
        public List<string> KeyColumns { get; init; } = new();

        /// <summary>一覧の並び順（固定の列名のみ）</summary>
        public string OrderBy { get; init; } = "";
    }

    /// <summary>マスタ保守: 編集対象1列分の定義</summary>
    public class M_View_MasterColumnDef
    {
        public M_View_MasterColumnDef(string columnName, string label, int maxLength = 0, bool isInt = false, bool required = false)
        {
            ColumnName = columnName;
            Label = label;
            MaxLength = maxLength;
            IsInt = isInt;
            Required = required;
        }

        /// <summary>物理列名</summary>
        public string ColumnName { get; }

        /// <summary>画面表示名（DDLの COMMENT ON COLUMN と同じ）</summary>
        public string Label { get; }

        /// <summary>文字列の最大文字数（IsInt=true のときは使わない）</summary>
        public int MaxLength { get; }

        /// <summary>整数列か（sort_order / color_number）</summary>
        public bool IsInt { get; }

        /// <summary>必須入力か（DDLの NOT NULL コード列）</summary>
        public bool Required { get; }
    }

    /// <summary>
    /// マスタ保守の対象マスタ一覧（固定登録）。
    /// 並び順は依頼の優先度: ★17禁止コメント → ★21その他コメント → 〇14食種 → 残りはNo順
    /// </summary>
    public static class M_View_MasterRegistry
    {
        public static readonly List<M_View_MasterDef> All = new()
        {
            new M_View_MasterDef
            {
                Slug = "contra_comment", No = 17, TableName = "lw_m_contra_comment", DisplayName = "禁止コメントマスタ", Priority = "★",
                Columns = new()
                {
                    new("contra_code", "禁止コメントコード", 4, required: true),
                    new("contra_name", "禁止コメント名", 50),
                    new("category_name", "分類名", 50),
                    new("sort_order", "並び順", isInt: true),
                    new("color_number", "色番号", isInt: true),
                },
                KeyColumns = new() { "contra_code" },
                OrderBy = "contra_code",
            },
            new M_View_MasterDef
            {
                Slug = "other_comment", No = 21, TableName = "lw_m_other_comment", DisplayName = "その他コメントマスタ", Priority = "★",
                Columns = new()
                {
                    new("other_comment_code", "その他コメントコード", 4, required: true),
                    new("other_comment_name", "その他コメント名", 50),
                    new("category_name", "分類名", 50),
                    new("sort_order", "並び順", isInt: true),
                    new("color_number", "色番号", isInt: true),
                },
                KeyColumns = new() { "other_comment_code" },
                OrderBy = "other_comment_code",
            },
            new M_View_MasterDef
            {
                Slug = "meal", No = 14, TableName = "lw_m_meal", DisplayName = "食種マスタ", Priority = "〇",
                Columns = new()
                {
                    new("meal_code", "食種コード", 5, required: true),
                    new("meal_name", "食種名", 50),
                    new("category_name", "分類名", 50),
                    new("diet_group", "食種区分", 50),
                    new("sort_order", "並び順", isInt: true),
                    new("color_number", "色番号", isInt: true),
                },
                KeyColumns = new() { "meal_code" },
                OrderBy = "meal_code",
            },
            new M_View_MasterDef
            {
                Slug = "ward", No = 11, TableName = "lw_m_ward", DisplayName = "病棟マスタ",
                Columns = new()
                {
                    new("ward_code", "病棟コード", 3, required: true),
                    new("ward_name", "病棟名", 50),
                },
                KeyColumns = new() { "ward_code" },
                OrderBy = "ward_code",
            },
            new M_View_MasterDef
            {
                Slug = "room", No = 12, TableName = "lw_m_room", DisplayName = "病室マスタ",
                Columns = new()
                {
                    new("ward_code", "病棟コード", 3, required: true),
                    new("room_code", "病室コード", 4, required: true),
                    new("room_name", "病室名", 50),
                },
                KeyColumns = new() { "ward_code", "room_code" },
                OrderBy = "ward_code, room_code",
            },
            new M_View_MasterDef
            {
                Slug = "bed", No = 13, TableName = "lw_m_bed", DisplayName = "ベッドマスタ",
                Columns = new()
                {
                    new("ward_code", "病棟コード", 3, required: true),
                    new("room_code", "病室コード", 4, required: true),
                    new("bed_code", "ベッドコード", 2, required: true),
                    new("bed_name", "ベッド名", 50),
                },
                // DDLのユニークインデックスは room_code + bed_code（ward_code は含まれない）に合わせる
                KeyColumns = new() { "room_code", "bed_code" },
                OrderBy = "ward_code, room_code, bed_code",
            },
            new M_View_MasterDef
            {
                Slug = "main_dish", No = 15, TableName = "lw_m_main_dish", DisplayName = "主食マスタ",
                Columns = new()
                {
                    new("main_dish_code", "主食コード", 2, required: true),
                    new("main_dish_name", "主食名", 50),
                    new("category_name", "分類名", 50),
                    new("sort_order", "並び順", isInt: true),
                    new("color_number", "色番号", isInt: true),
                },
                KeyColumns = new() { "main_dish_code" },
                OrderBy = "main_dish_code",
            },
            new M_View_MasterDef
            {
                Slug = "department", No = 18, TableName = "lw_m_department", DisplayName = "診療科マスタ",
                Columns = new()
                {
                    new("department_code", "診療科コード", 2, required: true),
                    new("department_name", "診療科名", 50),
                },
                KeyColumns = new() { "department_code" },
                OrderBy = "department_code",
            },
        };

        public static M_View_MasterDef? Find(string? slug) => All.FirstOrDefault(d => d.Slug == slug);
    }

    /// <summary>マスタ保守の一覧・行内編集画面（/Master/Edit）の画面全体</summary>
    public class M_View_MasterEditPage
    {
        public M_View_MasterDef Def { get; set; } = new();

        public List<M_View_MasterEditRow> Rows { get; set; } = new();

        /// <summary>行内編集中の行の id（null=編集していない）</summary>
        public int? EditId { get; set; }

        /// <summary>新規追加行を表示中か（EditId とは同時に立たない）</summary>
        public bool Adding { get; set; }

        /// <summary>編集フォームの表示値（編集開始時は現在値、検証エラー時は入力値をそのまま返す）</summary>
        public Dictionary<string, string> EditValues { get; set; } = new();

        /// <summary>画面上部に出すメッセージ（検証エラー一覧、モック保存の結果など）</summary>
        public List<string> Messages { get; set; } = new();

        /// <summary>メッセージの種類（Bootstrapのalert色: success / warning / danger / info）</summary>
        public string MessageKind { get; set; } = "info";

        public bool IsConnected { get; set; }

        public string ErrorMessage { get; set; } = "";
    }

    /// <summary>マスタ保守一覧の1行分（列名→表示文字列）</summary>
    public class M_View_MasterEditRow
    {
        public int Id { get; set; }

        public Dictionary<string, string> Values { get; set; } = new();
    }

    /// <summary>
    /// マスタ保守のSELECT受け皿。列構成がマスタごとに違うため、
    /// 全列を ::text にキャストして v1〜v6 の固定別名で受ける（最大6列＝食種マスタ）
    /// </summary>
    public class M_View_MasterRawRow
    {
        public int id { get; set; }
        public string? v1 { get; set; }
        public string? v2 { get; set; }
        public string? v3 { get; set; }
        public string? v4 { get; set; }
        public string? v5 { get; set; }
        public string? v6 { get; set; }

        public string? GetV(int index) => index switch
        {
            1 => v1, 2 => v2, 3 => v3, 4 => v4, 5 => v5, 6 => v6,
            _ => null,
        };
    }
}
