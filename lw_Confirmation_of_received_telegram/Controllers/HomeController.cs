using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using lw_Confirmation_of_received_telegram.Models;

namespace lw_Confirmation_of_received_telegram.Controllers;

/// <summary>
/// エラーページ専用（画面の入口は OrderLogController.Index の患者検索）
/// </summary>
public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
