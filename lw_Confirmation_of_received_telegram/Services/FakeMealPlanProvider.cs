namespace lw_Confirmation_of_received_telegram.Services
{
    /// <summary>
    /// 食事一覧の【仮】供給元（lw_meal_plan が使えない環境向けのバックアップ。通常は DbMealPlanProvider を使う）。
    /// 「その日その食事に誰が載るか」の目次だけをハードコードし、肉付けは基底クラスが実DBから行う。
    /// ※日付は見ないため、どの日を選んでも同じ顔ぶれを返す
    /// </summary>
    public class FakeMealPlanProvider : MealPlanProviderBase
    {
        // 仮の目次: 患者番号×電文ID（2026-07-31時点の開発用DB(.96)に実在した値）
        private static readonly (string PatientNumber, int LogId)[] FakeIndex =
        {
            ("0009900354", 9),
            ("0009900788", 19),
            ("0009911234", 23),
            ("0009922345", 27),
            ("0009933456", 33),
            ("0009944567", 40),
            ("0009990000", 43),
            ("0009990100", 46),
            ("0009990200", 54),
            ("0009990300", 60),
            ("0009990400", 64),
            ("0009990500", 68),
        };

        public FakeMealPlanProvider(Connect_PostgreSQL db, ILogger<FakeMealPlanProvider> logger)
            : base(db, logger)
        {
        }

        protected override List<(string PatientNumber, int LogId)> GetIndex(DateOnly date, int mealType)
            => FakeIndex.ToList();
    }
}
