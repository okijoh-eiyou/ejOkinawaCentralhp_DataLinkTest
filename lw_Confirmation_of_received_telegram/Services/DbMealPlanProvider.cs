using lw_Confirmation_of_received_telegram.Models;

namespace lw_Confirmation_of_received_telegram.Services
{
    /// <summary>
    /// 食事一覧の【本物】供給元。
    /// 目次を先輩作成の lw_meal_plan(テーブル) から取得する（SELECTのみ・DB更新は一切しない）。
    /// 肉付け（患者名・食種名・コメント等）は基底クラスが展開8テーブル＋マスタから行う。
    /// ※Disp_Json(列) のキー設計が確定したら、肉付けをDisp_Json読み取りへ切り替える選択肢あり
    /// </summary>
    public class DbMealPlanProvider : MealPlanProviderBase
    {
        public DbMealPlanProvider(Connect_PostgreSQL db, ILogger<DbMealPlanProvider> logger)
            : base(db, logger)
        {
        }

        protected override List<(string PatientNumber, int LogId)> GetIndex(DateOnly date, int mealType)
        {
            // DateOnly はこのDapperのバージョンではパラメータにできないため、
            // 文字列で渡してSQL側で ::date にキャストする（パラメータ渡しは維持）
            var plans = _db.GetDataList_SQL<M_meal_plan>(
                "SELECT plan_id, log_id, patient_number FROM lw_meal_plan " +
                "WHERE meal_date = @meal_date::date AND meal_type = @meal_type ORDER BY patient_number",
                new { meal_date = date.ToString("yyyy-MM-dd"), meal_type = mealType });

            return plans.Select(p => (p.patient_number, p.log_id)).ToList();
        }
    }
}
