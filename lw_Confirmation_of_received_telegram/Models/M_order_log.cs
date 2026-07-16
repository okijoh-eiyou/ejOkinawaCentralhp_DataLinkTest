namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// lw_order_log：食事オーダー電文ログデータ（受信した全電文の記録）
    /// </summary>
    public class M_order_log
    {
        public int id { get; set; }

        /// <summary>処理済みフラグ</summary>
        public bool is_processed { get; set; }

        /// <summary>有効フラグ</summary>
        public bool is_active { get; set; }

        /// <summary>追加済みフラグ</summary>
        public bool is_appended { get; set; }

        /// <summary>ファイル名</summary>
        public string file_name { get; set; } = "";

        /// <summary>オーダー電文のJSON形式</summary>
        public string order_items { get; set; } = "";

        /// <summary>更新区分 (A:追加, D:削除, I:一括削除)</summary>
        public string update_type { get; set; } = "";

        /// <summary>患者番号</summary>
        public string patient_number { get; set; } = "";

        /// <summary>開始日</summary>
        public DateTime? start_date { get; set; }

        /// <summary>開始食事区分 (1:朝, 2:昼, 3:夕)</summary>
        public string start_meal_type { get; set; } = "";

        /// <summary>開始食事区分内容</summary>
        public string start_meal_type_text { get; set; } = "";

        /// <summary>理由区分 (A:入院〜K:転室)</summary>
        public string reason_type { get; set; } = "";

        /// <summary>理由区分内容</summary>
        public string reason_type_text { get; set; } = "";

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
