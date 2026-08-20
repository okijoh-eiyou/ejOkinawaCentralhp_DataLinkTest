using Microsoft.AspNetCore.Mvc;
using lw_Confirmation_of_received_telegram.Models;
using lw_Confirmation_of_received_telegram.Services;

namespace lw_Confirmation_of_received_telegram.Controllers;

/// <summary>
/// 食事一覧画面（屋宜原版 Meal_Information トップ画面のWeb版）。
/// データの出どころは IMealPlanProvider しか知らない（現在は仮実装が刺さっている）
/// </summary>
public class MealPlanController : Controller
{
    private readonly ILogger<MealPlanController> _logger;
    private readonly IMealPlanProvider _provider;

    public MealPlanController(ILogger<MealPlanController> logger, IMealPlanProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    /// <summary>
    /// 日付×食事区分（1:朝 2:昼 3:夕）の患者一覧。既定は今日の昼
    /// </summary>
    public IActionResult Index(string? date, int mealType = 2)
    {
        var page = new M_View_MealPlanPage
        {
            TargetDate = DateOnly.TryParse(date, out var d) ? d : DateOnly.FromDateTime(DateTime.Today),
            MealType = mealType is >= 1 and <= 3 ? mealType : 2,
        };

        // DBが落ちていても画面は必ず表示する
        try
        {
            page.Rows = _provider.GetDailyPlan(page.TargetDate, page.MealType);
            page.IsConnected = true;
        }
        catch (Exception ex)
        {
            page.IsConnected = false;
            page.ErrorMessage = ex.Message;
            _logger.LogError(ex, "食事一覧の取得でエラー（{Date} 区分{MealType}）", page.TargetDate, page.MealType);
        }

        return View(page);
    }
}
