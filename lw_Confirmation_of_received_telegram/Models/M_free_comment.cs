namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// lw_free_comment：その他フリーコメントデータ
    /// </summary>
    public class M_free_comment
    {
        public int id { get; set; }

        /// <summary>転記元ログID</summary>
        public int log_id { get; set; }

        /// <summary>フリーコメント朝</summary>
        public string free_comment_01 { get; set; } = "";

        /// <summary>フリーコメント昼</summary>
        public string free_comment_02 { get; set; } = "";

        /// <summary>フリーコメント夕</summary>
        public string free_comment_03 { get; set; } = "";

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
