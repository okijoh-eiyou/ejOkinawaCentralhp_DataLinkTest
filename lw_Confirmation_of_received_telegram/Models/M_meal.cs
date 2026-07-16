namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// lw_meal：食事内容データ
    /// </summary>
    public class M_meal
    {
        public int id { get; set; }

        /// <summary>転記元ログID</summary>
        public int log_id { get; set; }

        /// <summary>食事時間区分 (1:朝, 2:昼, 3:夕)</summary>
        public int? meal_time_code { get; set; }

        /// <summary>食種コード</summary>
        public string meal_code { get; set; } = "";

        /// <summary>食事形態コード</summary>
        public string meal_form_code { get; set; } = "";

        /// <summary>配膳区分コード</summary>
        public string serving_category_code { get; set; } = "";

        /// <summary>主食コード</summary>
        public string main_dish_code { get; set; } = "";

        /// <summary>主食量（DB列名の綴りに合わせている）</summary>
        public string main_dish_amout { get; set; } = "";

        /// <summary>特別食加算区分</summary>
        public string addition_type { get; set; } = "";

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
