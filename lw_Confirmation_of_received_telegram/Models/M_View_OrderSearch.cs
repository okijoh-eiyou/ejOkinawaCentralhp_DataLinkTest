namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// 患者検索画面（画面2）の表示用。DBテーブルには対応しない
    /// </summary>
    public class M_View_OrderSearch
    {
        /// <summary>検索した患者番号（前後の空白は除去済み）。数字以外もあり得るため string のまま扱う</summary>
        public string PatientNumber { get; set; } = "";

        /// <summary>無効データ（is_active = FALSE）も表示するか</summary>
        public bool ShowInactive { get; set; }

        /// <summary>検索を実行したか（false なら検索フォームのみ表示）</summary>
        public bool Searched { get; set; }

        /// <summary>DB接続に成功したか（Searched = true のときのみ意味を持つ）</summary>
        public bool IsConnected { get; set; }

        /// <summary>接続失敗時のエラー内容</summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>検索結果の電文一覧（新しい順）</summary>
        public List<M_order_log> OrderLogs { get; set; } = new();

        /// <summary>画面上部に常時表示する最新の受信電文（全患者・無効含む・最新300件）</summary>
        public List<M_order_log> LatestLogs { get; set; } = new();

        /// <summary>最新電文一覧の取得に失敗したときのエラー内容（成功時は空）</summary>
        public string LatestLogsError { get; set; } = "";

        /// <summary>一覧で選択中の電文ID（未選択なら null）</summary>
        public int? SelectedId { get; set; }

        /// <summary>選択中の電文の展開データ（画面下部に表示。未選択なら null）</summary>
        public M_View_OrderDetail? Detail { get; set; }
    }
}
