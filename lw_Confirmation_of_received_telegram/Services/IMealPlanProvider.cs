using lw_Confirmation_of_received_telegram.Models;

namespace lw_Confirmation_of_received_telegram.Services
{
    /// <summary>
    /// 食事一覧（日付×食事区分の患者一覧）の供給元との約束事。
    /// 画面（MealPlanController）はこのインターフェイスしか見ない。
    /// 現在は仮実装（FakeMealPlanProvider）。上司の meal_plan 相当テーブルが
    /// できたら本物実装（DbMealPlanProvider）を作り、Program.cs の登録を差し替える
    /// </summary>
    public interface IMealPlanProvider
    {
        /// <summary>指定した日付・食事区分（1:朝 2:昼 3:夕）の患者一覧を返す</summary>
        List<M_View_MealPlanRow> GetDailyPlan(DateOnly date, int mealType);
    }
}
