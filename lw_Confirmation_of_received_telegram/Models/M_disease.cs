namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// lw_disease：病名データ（1電文あたり最大3）
    /// </summary>
    public class M_disease
    {
        public int id { get; set; }

        /// <summary>転記元ログID</summary>
        public int log_id { get; set; }

        /// <summary>順序</summary>
        public int? row_order { get; set; }

        /// <summary>加算/非加算区分</summary>
        public string addition_type { get; set; } = "";

        /// <summary>病名コード</summary>
        public string disease_code { get; set; } = "";

        /// <summary>漢字病名</summary>
        public string disease_name { get; set; } = "";

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
