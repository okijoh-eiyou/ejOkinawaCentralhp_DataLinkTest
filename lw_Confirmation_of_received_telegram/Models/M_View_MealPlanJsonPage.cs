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

        // ---- フィルタ（フィルタ表示ボタンで開閉。空文字=絞り込みなし） ----

        /// <summary>病棟フィルタ（選択された病棟コード）</summary>
        public string WardCode { get; set; } = "";

        /// <summary>食種フィルタ（選択された食種コード）</summary>
        public string MealCode { get; set; } = "";

        /// <summary>主食フィルタ（選択された主食コード）</summary>
        public string MainDishCode { get; set; } = "";

        /// <summary>いずれかのフィルタが有効か（フィルタ行を開いた状態で表示する判定にも使う）</summary>
        public bool FilterActive => WardCode != "" || MealCode != "" || MainDishCode != "";

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
