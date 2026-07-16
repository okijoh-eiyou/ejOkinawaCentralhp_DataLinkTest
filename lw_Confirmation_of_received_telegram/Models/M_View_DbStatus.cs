namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// 接続確認（スモークテスト）画面の表示用。DBテーブルには対応しない
    /// </summary>
    public class M_View_DbStatus
    {
        /// <summary>DB接続に成功したか</summary>
        public bool IsConnected { get; set; }

        /// <summary>接続失敗時のエラー内容</summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>lw_order_log の総件数</summary>
        public long TotalCount { get; set; }

        /// <summary>本日受信した電文の件数</summary>
        public long TodayCount { get; set; }
    }
}
