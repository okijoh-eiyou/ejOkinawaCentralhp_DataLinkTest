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

    /// <summary>
    /// 画面1: 受信モニタ。最新50件の電文一覧＋本日受信件数＋患者番号検索フォーム
    /// </summary>
    public IActionResult Index()
    {
        var monitor = new M_View_ReceiveMonitor();

        // DBが落ちていても画面は必ず表示する（屋宜原版が起動時に落ちた反省）
        try
        {
            monitor.TotalCount = _db.GetScalar_SQL<long>("SELECT COUNT(*) FROM lw_order_log");
            monitor.TodayCount = _db.GetScalar_SQL<long>("SELECT COUNT(*) FROM lw_order_log WHERE created_at >= CURRENT_DATE");
            monitor.LatestLogs = _db.GetDataList_SQL<M_order_log>("SELECT * FROM lw_order_log ORDER BY id DESC LIMIT 50");
            monitor.IsConnected = true;
        }
        catch (Exception ex)
        {
            monitor.IsConnected = false;
            monitor.ErrorMessage = ex.Message;
            _logger.LogError(ex, "DB接続に失敗しました");
        }

        return View(monitor);
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
