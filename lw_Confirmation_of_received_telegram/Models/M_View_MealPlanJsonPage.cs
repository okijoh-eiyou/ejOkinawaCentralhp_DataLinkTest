namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// 食事一覧（Disp_Json直表示版）の画面全体。
    /// 列は固定せず、disp_json に現れたキーを出現順（jsonbが返す順）にそのまま列にする
    /// </summary>
    public class M_View_MealPlanJsonPage
    {
        /// <summary>表示対象の日付</summary>
        public DateOnly TargetDate { get; set; }

        /// <summary>表示対象の食事区分（1:朝 2:昼 3:夕）</summary>
        public int MealType { get; set; } = 2;

        /// <summary>列ラベル（disp_jsonのキー。全行を上から走査した出現順）</summary>
        public List<string> Columns { get; set; } = new();

        /// <summary>一覧の行（患者ごと）</summary>
        public List<M_View_MealPlanJsonRow> Rows { get; set; } = new();

        /// <summary>コード系の列（列名に「コード」を含むもの）を非表示にするか（既定: 非表示＝本番の見た目）</summary>
        public bool HideCodes { get; set; } = true;

        // ---- フィルタ（フィルタ表示ボタンで開閉。チェックなし=絞り込みなし） ----
        // 各リストはチェックボックスの複数選択（2026-09-19）。
        // 同じリスト内の複数チェックは「どれかに一致（OR）」、リスト同士は掛け合わせ（AND）

        /// <summary>病棟フィルタ（チェックされた病棟コード）</summary>
        public List<string> WardCodes { get; set; } = new();

        /// <summary>食種フィルタ（チェックされた食種コード）</summary>
        public List<string> MealCodes { get; set; } = new();

        /// <summary>主食フィルタ（チェックされた主食コード）</summary>
        public List<string> MainDishCodes { get; set; } = new();

        /// <summary>いずれかのフィルタが有効か（フィルタ表示ボタンの青塗り判定にも使う）</summary>
        public bool FilterActive => WardCodes.Count > 0 || MealCodes.Count > 0 || MainDishCodes.Count > 0;

        /// <summary>病棟プルダウンの選択肢（lw_m_ward から）</summary>
        public List<M_CodeName> WardOptions { get; set; } = new();

        /// <summary>食種プルダウンの選択肢（lw_m_meal から）</summary>
        public List<M_CodeName> MealOptions { get; set; } = new();

        /// <summary>主食プルダウンの選択肢（lw_m_main_dish から）</summary>
        public List<M_CodeName> MainDishOptions { get; set; } = new();

        /// <summary>データ取得に成功したか</summary>
        public bool IsConnected { get; set; }

        /// <summary>取得失敗時のエラー内容</summary>
        public string ErrorMessage { get; set; } = "";
    }

    /// <summary>食事一覧（Disp_Json直表示版）の1行分</summary>
    public class M_View_MealPlanJsonRow
    {
        /// <summary>患者番号（lw_meal_planの列から。先頭ゼロ付きの原値＝履歴ボタンの検索に使う）</summary>
        public string PatientNumber { get; set; } = "";

        /// <summary>表示用の患者番号（先頭ゼロを除去。例: 0009900354 → 9900354）</summary>
        public string DisplayPatientNumber { get; set; } = "";

        /// <summary>disp_json が入っているか（空の行は全セル空欄で表示）</summary>
        public bool HasJson { get; set; }

        /// <summary>キー→表示文字列（null は空文字にして格納）</summary>
        public Dictionary<string, string> Values { get; set; } = new();
    }
}
