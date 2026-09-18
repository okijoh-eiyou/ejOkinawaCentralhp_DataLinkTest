using lw_Confirmation_of_received_telegram.Models;

namespace lw_Confirmation_of_received_telegram.Services
{
    /// <summary>
    /// 食事一覧の共通処理（肉付け担当）。
    /// 「その日その食事に誰が載るか（目次）」の取得方法だけを派生クラスに任せ、
    /// 患者名・食種名・コメント等を実DB（展開8テーブル＋マスタ）から組み立てる部分を共通化する。
    /// 派生: FakeMealPlanProvider（目次ハードコード） / DbMealPlanProvider（lw_meal_plan から取得）
    /// </summary>
    public abstract class MealPlanProviderBase : IMealPlanProvider
    {
        protected readonly Connect_PostgreSQL _db;
        protected readonly ILogger _logger;

        protected MealPlanProviderBase(Connect_PostgreSQL db, ILogger logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>目次（患者番号×有効電文ID）を返す。出どころは派生クラスが決める</summary>
        protected abstract List<(string PatientNumber, int LogId)> GetIndex(DateOnly date, int mealType);

        public List<M_View_MealPlanRow> GetDailyPlan(DateOnly date, int mealType)
        {
            var index = GetIndex(date, mealType);
            if (index.Count == 0)
            {
                return new List<M_View_MealPlanRow>();
            }

            var logIds = index.Select(f => f.LogId).ToArray();
            var param = new { log_ids = logIds, meal_time = mealType };

            // 肉付けデータ（屋宜原版 VM_Meal_Information と同じく ANY でまとめ引き）
            var orderLogs = _db.GetDataList_SQL<M_order_log>(
                "SELECT * FROM lw_order_log WHERE id = ANY(@log_ids)", param);
            var contexts = _db.GetDataList_SQL<M_patient_context>(
                "SELECT * FROM lw_patient_context WHERE log_id = ANY(@log_ids)", param);
            var meals = _db.GetDataList_SQL<M_meal>(
                "SELECT * FROM lw_meal WHERE log_id = ANY(@log_ids) AND meal_time_code = @meal_time", param);
            var comments = _db.GetDataList_SQL<M_comment>(
                "SELECT * FROM lw_comment WHERE log_id = ANY(@log_ids) AND meal_time_code = @meal_time ORDER BY row_order", param);
            var contras = _db.GetDataList_SQL<M_contra_comment>(
                "SELECT * FROM lw_contra_comment WHERE log_id = ANY(@log_ids) AND meal_time_code = @meal_time ORDER BY row_order", param);
            var frees = _db.GetDataList_SQL<M_free_comment>(
                "SELECT * FROM lw_free_comment WHERE log_id = ANY(@log_ids)", param);

            // マスタ名前解決（マスタ空・未作成ならコード素通し）
            var wardNames = LoadMaster("SELECT ward_code AS code, COALESCE(ward_name, '') AS name FROM lw_m_ward");
            var roomNames = LoadMaster("SELECT room_code AS code, COALESCE(room_name, '') AS name FROM lw_m_room");
            var mealNames = LoadMaster("SELECT meal_code AS code, COALESCE(meal_name, '') AS name FROM lw_m_meal");
            var mainDishNames = LoadMaster("SELECT main_dish_code AS code, COALESCE(main_dish_name, '') AS name FROM lw_m_main_dish");
            var commentNames = LoadMaster("SELECT comment_code AS code, COALESCE(comment_name, '') AS name FROM lw_m_comment");
            var contraNames = LoadMaster("SELECT contra_code AS code, COALESCE(contra_name, '') AS name FROM lw_m_contra_comment");

            var rows = new List<M_View_MealPlanRow>();
            foreach (var (patientNumber, logId) in index)
            {
                var log = orderLogs.FirstOrDefault(o => o.id == logId);
                var pc = contexts.FirstOrDefault(c => c.log_id == logId);
                var meal = meals.FirstOrDefault(m => m.log_id == logId);
                var free = frees.FirstOrDefault(f => f.log_id == logId);

                rows.Add(new M_View_MealPlanRow
                {
                    ReasonText = log?.reason_type_text ?? "",
                    ChangeMark = "",    // 前食との差分。meal_plan相当の仕様確定後に実装
                    WardCode = pc?.ward_code ?? "",
                    WardName = Resolve(wardNames, pc?.ward_code),
                    RoomCode = pc?.room_code ?? "",
                    RoomName = Resolve(roomNames, pc?.room_code),
                    PatientNumber = patientNumber,
                    KanjiName = pc?.kanji_name ?? "",
                    KanaName = pc?.kana_name ?? "",
                    MealTimeText = M_View_OrderDetail.MealTimeText(mealType),
                    MealCode = meal?.meal_code ?? "",
                    MealName = Resolve(mealNames, meal?.meal_code),
                    MainDishCode = meal?.main_dish_code ?? "",
                    MainDishName = Resolve(mainDishNames, meal?.main_dish_code),
                    CommentText = JoinCodes(comments.Where(c => c.log_id == logId).Select(c => c.comment_code), commentNames),
                    ContraText = JoinCodes(contras.Where(c => c.log_id == logId).Select(c => c.contra_code), contraNames),
                    FreeComment = mealType switch
                    {
                        1 => free?.free_comment_01 ?? "",
                        2 => free?.free_comment_02 ?? "",
                        3 => free?.free_comment_03 ?? "",
                        _ => "",
                    },
                });
            }

            // 病棟→病室→患者番号の順に整列（屋宜原版トップ画面の並びに寄せる）
            return rows.OrderBy(r => r.WardCode).ThenBy(r => r.RoomCode).ThenBy(r => r.PatientNumber).ToList();
        }

        /// <summary>コード列を「コード 名前、コード 名前…」の形に連結（名前が引けなければコードのみ）</summary>
        protected static string JoinCodes(IEnumerable<string> codes, Dictionary<string, string> master)
        {
            var parts = codes
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c => string.IsNullOrEmpty(Resolve(master, c)) ? c : $"{c} {Resolve(master, c)}");
            return string.Join("、", parts);
        }

        protected static string Resolve(Dictionary<string, string> master, string? code)
            => code != null && master.TryGetValue(code, out var name) ? name : "";

        /// <summary>マスタをコード→名前の辞書にする。テーブル未作成でも空辞書で続行</summary>
        protected Dictionary<string, string> LoadMaster(string sql)
        {
            var dic = new Dictionary<string, string>();
            try
            {
                foreach (var row in _db.GetDataList_SQL<M_CodeName>(sql))
                {
                    if (!string.IsNullOrEmpty(row.code))
                    {
                        dic.TryAdd(row.code, row.name ?? "");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "マスタ取得に失敗（名前解決なしで続行）: {Sql}", sql);
            }
            return dic;
        }
    }
}
