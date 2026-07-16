namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// lw_contra_comment：禁止コメントデータ（禁止指示、朝昼夕ごとに最大20）
    /// </summary>
    public class M_contra_comment
    {
        public int id { get; set; }

        /// <summary>転記元ログID</summary>
        public int log_id { get; set; }

        /// <summary>時間区分 (1:朝, 2:昼, 3:夕)</summary>
        public int? meal_time_code { get; set; }

        /// <summary>並び順</summary>
        public int? row_order { get; set; }

        /// <summary>禁止コメントコード</summary>
        public string contra_code { get; set; } = "";

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
