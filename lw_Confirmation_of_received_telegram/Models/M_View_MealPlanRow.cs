namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// 食事一覧画面（屋宜原版 Meal_Information トップ画面相当）の1行分。DBテーブルには対応しない。
    /// この形が「meal_plan相当のテーブルに最終的に必要な情報」の一覧を兼ねる
    /// </summary>
    public class M_View_MealPlanRow
    {
        /// <summary>理由区分の内容（入院・食事変更など）</summary>
        public string ReasonText { get; set; } = "";

        /// <summary>変更マーク（前食との差分。meal_plan相当が本物になってから実装するため当面は空）</summary>
        public string ChangeMark { get; set; } = "";

        public string WardCode { get; set; } = "";
        public string WardName { get; set; } = "";
        public string RoomCode { get; set; } = "";
        public string RoomName { get; set; } = "";

        public string PatientNumber { get; set; } = "";
        public string KanjiName { get; set; } = "";
        public string KanaName { get; set; } = "";

        /// <summary>食事区分の表示名（朝/昼/夕）</summary>
        public string MealTimeText { get; set; } = "";

        public string MealCode { get; set; } = "";
        public string MealName { get; set; } = "";
        public string MainDishCode { get; set; } = "";
        public string MainDishName { get; set; } = "";

        /// <summary>特別指示コメント（複数件を「、」区切りで連結）</summary>
        public string CommentText { get; set; } = "";

        /// <summary>禁止コメント（複数件を「、」区切りで連結）</summary>
        public string ContraText { get; set; } = "";

        /// <summary>フリーコメント（選択中の食事区分のもの）</summary>
        public string FreeComment { get; set; } = "";
    }
}
