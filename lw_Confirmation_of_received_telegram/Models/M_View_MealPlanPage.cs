namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// 食事一覧画面（画面全体）の表示用。DBテーブルには対応しない
    /// </summary>
    public class M_View_MealPlanPage
    {
        /// <summary>表示対象の日付</summary>
        public DateOnly TargetDate { get; set; }

        /// <summary>表示対象の食事区分（1:朝 2:昼 3:夕）</summary>
        public int MealType { get; set; } = 2;

        /// <summary>一覧の行（患者ごと）</summary>
        public List<M_View_MealPlanRow> Rows { get; set; } = new();

        /// <summary>データ取得に成功したか</summary>
        public bool IsConnected { get; set; }

        /// <summary>取得失敗時のエラー内容</summary>
        public string ErrorMessage { get; set; } = "";
    }
}
