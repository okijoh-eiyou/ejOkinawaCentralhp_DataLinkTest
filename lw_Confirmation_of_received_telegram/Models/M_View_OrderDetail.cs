using System.Text.Encodings.Web;
using System.Text.Json;

namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// 電文詳細画面（画面3）の表示用。lw_order_log 1件と、そこから展開された8テーブルの内容を持つ
    /// </summary>
    public class M_View_OrderDetail
    {
        /// <summary>DB接続に成功したか</summary>
        public bool IsConnected { get; set; }

        /// <summary>接続失敗時のエラー内容</summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>対象の電文。指定IDが存在しないときは null</summary>
        public M_order_log? OrderLog { get; set; }

        // 展開8テーブル（log_id で引いた結果）
        public List<M_patient_context> PatientContexts { get; set; } = new();
        public List<M_meal> Meals { get; set; } = new();
        public List<M_comment> Comments { get; set; } = new();
        public List<M_contra_comment> ContraComments { get; set; } = new();
        public List<M_other_comment> OtherComments { get; set; } = new();
        public List<M_disease> Diseases { get; set; } = new();
        public List<M_tube_feeding> TubeFeedings { get; set; } = new();
        public List<M_free_comment> FreeComments { get; set; } = new();

        // マスタ名前解決用の辞書（コード→名前）。マスタが空・未投入なら空辞書のままで動く
        public Dictionary<string, string> WardNames { get; set; } = new();
        public Dictionary<string, string> RoomNames { get; set; } = new();
        public Dictionary<string, string> BedNames { get; set; } = new();
        public Dictionary<string, string> DepartmentNames { get; set; } = new();
        public Dictionary<string, string> MealNames { get; set; } = new();
        public Dictionary<string, string> MainDishNames { get; set; } = new();
        public Dictionary<string, string> CommentNames { get; set; } = new();
        public Dictionary<string, string> ContraNames { get; set; } = new();
        public Dictionary<string, string> OtherCommentNames { get; set; } = new();
        public Dictionary<string, string> TubeProductNames { get; set; } = new();

        public string WardName(string? code) => Resolve(WardNames, code);
        public string RoomName(string? code) => Resolve(RoomNames, code);
        public string BedName(string? code) => Resolve(BedNames, code);
        public string DepartmentName(string? code) => Resolve(DepartmentNames, code);
        public string MealName(string? code) => Resolve(MealNames, code);
        public string MainDishName(string? code) => Resolve(MainDishNames, code);
        public string CommentName(string? code) => Resolve(CommentNames, code);
        public string ContraName(string? code) => Resolve(ContraNames, code);
        public string OtherCommentName(string? code) => Resolve(OtherCommentNames, code);
        public string TubeProductName(string? code) => Resolve(TubeProductNames, code);

        /// <summary>
        /// コードをマスタで名前に引く。引けなければ空文字を返す
        /// （コード自体は画面の別列に常に表示するので、マスタが空でもコードは見える）
        /// </summary>
        private static string Resolve(Dictionary<string, string> master, string? code)
            => code != null && master.TryGetValue(code, out var name) ? name : "";

        /// <summary>食事時間区分 (1:朝, 2:昼, 3:夕) の表示名。未知の値はそのまま数字で見せる</summary>
        public static string MealTimeText(int? code) => code switch
        {
            1 => "朝",
            2 => "昼",
            3 => "夕",
            null => "",
            _ => code.Value.ToString(),
        };

        /// <summary>
        /// 生JSON（order_items）をインデント付きに整形して返す。
        /// JSONとして壊れている場合も確認対象なので、例外にせず原文のまま返す
        /// </summary>
        public string FormattedJson()
        {
            if (OrderLog == null || string.IsNullOrWhiteSpace(OrderLog.order_items))
            {
                return "";
            }

            try
            {
                using var doc = JsonDocument.Parse(OrderLog.order_items);
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    // 日本語キーを \uXXXX にエスケープさせない
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                });
            }
            catch (JsonException)
            {
                return OrderLog.order_items;
            }
        }
    }
}
