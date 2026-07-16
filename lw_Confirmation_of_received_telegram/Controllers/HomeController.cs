using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using lw_Confirmation_of_received_telegram.Models;
using lw_Confirmation_of_received_telegram.Services;

namespace lw_Confirmation_of_received_telegram.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly Connect_PostgreSQL _db;

    public HomeController(ILogger<HomeController> logger, Connect_PostgreSQL db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        var status = new M_View_DbStatus();

        // DBが落ちていても画面は必ず表示する（屋宜原版が起動時に落ちた反省）
        try
        {
            status.TotalCount = _db.GetScalar_SQL<long>("SELECT COUNT(*) FROM lw_order_log");
            status.TodayCount = _db.GetScalar_SQL<long>("SELECT COUNT(*) FROM lw_order_log WHERE created_at >= CURRENT_DATE");
            status.IsConnected = true;
        }
        catch (Exception ex)
        {
            status.IsConnected = false;
            status.ErrorMessage = ex.Message;
            _logger.LogError(ex, "DB接続に失敗しました");
        }

        return View(status);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
