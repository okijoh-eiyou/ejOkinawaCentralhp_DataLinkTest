namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// 受信モニタ画面（画面1）の表示用。DBテーブルには対応しない
    /// </summary>
    public class M_View_ReceiveMonitor
    {
        /// <summary>DB接続に成功したか</summary>
        public bool IsConnected { get; set; }

        /// <summary>接続失敗時のエラー内容</summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>lw_order_log の総件数</summary>
        public long TotalCount { get; set; }

        /// <summary>本日受信した電文の件数</summary>
        public long TodayCount { get; set; }

        /// <summary>最新50件の受信電文（新しい順）。接続失敗時は空リスト</summary>
        public List<M_order_log> LatestLogs { get; set; } = new();
    }
}
