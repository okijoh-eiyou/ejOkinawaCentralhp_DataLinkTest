using Microsoft.AspNetCore.Mvc;
using lw_Confirmation_of_received_telegram.Models;
using lw_Confirmation_of_received_telegram.Services;

namespace lw_Confirmation_of_received_telegram.Controllers;

/// <summary>
/// 画面2（患者検索）と画面3（電文詳細）。手本は屋宜原版 VM_MainWindow.cs の
/// SearchPatient() / SelectChanged() だが、SQLはすべてパラメータ渡しに置き換えている
/// </summary>
public class OrderLogController : Controller
{
    private readonly ILogger<OrderLogController> _logger;
    private readonly Connect_PostgreSQL _db;

    public OrderLogController(ILogger<OrderLogController> logger, Connect_PostgreSQL db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// 患者検索（/OrderLog?patientNumber=）。患者番号未指定なら検索フォームのみ表示。
    /// 既定では無効データ（is_active = FALSE）を隠し、showInactive = true で全件表示。
    /// selectedId 指定時は、その電文の展開データを同じ画面の下部に表示する（屋宜原版の行選択に相当）
    /// </summary>
    public IActionResult Index(string? patientNumber, bool showInactive = false, int? selectedId = null)
    {
        var search = new M_View_OrderSearch
        {
            PatientNumber = (patientNumber ?? "").Trim(),
            ShowInactive = showInactive,
        };

        if (search.PatientNumber == "")
        {
            return View(search);
        }

        search.Searched = true;
        try
        {
            var sql = showInactive
                ? "SELECT * FROM lw_order_log WHERE patient_number = @patient_number ORDER BY id DESC"
                : "SELECT * FROM lw_order_log WHERE is_active = TRUE AND patient_number = @patient_number ORDER BY id DESC";
            search.OrderLogs = _db.GetDataList_SQL<M_order_log>(sql, new { patient_number = search.PatientNumber });
            search.IsConnected = true;
        }
        catch (Exception ex)
        {
            search.IsConnected = false;
            search.ErrorMessage = ex.Message;
            _logger.LogError(ex, "患者検索でDBエラー（患者番号: {PatientNumber}）", search.PatientNumber);
        }

        if (selectedId.HasValue && search.IsConnected)
        {
            search.SelectedId = selectedId;
            search.Detail = BuildDetail(selectedId.Value);
        }

        return View(search);
    }

    /// <summary>
    /// 電文詳細（/OrderLog/Detail/{id}）。生JSONと展開8テーブルを単独ページで表示
    /// </summary>
    public IActionResult Detail(int id)
    {
        return View(BuildDetail(id));
    }

    /// <summary>
    /// 電文1件の展開データ一式を組み立てる（患者検索の下部表示と詳細ページで共用）
    /// </summary>
    private M_View_OrderDetail BuildDetail(int id)
    {
        var detail = new M_View_OrderDetail();

        try
        {
            detail.OrderLog = _db.GetDataList_SQL<M_order_log>(
                "SELECT * FROM lw_order_log WHERE id = @id", new { id }).FirstOrDefault();
            detail.IsConnected = true;

            if (detail.OrderLog != null)
            {
                var param = new { log_id = id };
                detail.PatientContexts = _db.GetDataList_SQL<M_patient_context>(
                    "SELECT * FROM lw_patient_context WHERE log_id = @log_id ORDER BY id", param);
                detail.Meals = _db.GetDataList_SQL<M_meal>(
                    "SELECT * FROM lw_meal WHERE log_id = @log_id ORDER BY meal_time_code, id", param);
                detail.Comments = _db.GetDataList_SQL<M_comment>(
                    "SELECT * FROM lw_comment WHERE log_id = @log_id ORDER BY meal_time_code, row_order, id", param);
                detail.ContraComments = _db.GetDataList_SQL<M_contra_comment>(
                    "SELECT * FROM lw_contra_comment WHERE log_id = @log_id ORDER BY meal_time_code, row_order, id", param);
                detail.OtherComments = _db.GetDataList_SQL<M_other_comment>(
                    "SELECT * FROM lw_other_comment WHERE log_id = @log_id ORDER BY meal_time_code, row_order, id", param);
                detail.Diseases = _db.GetDataList_SQL<M_disease>(
                    "SELECT * FROM lw_disease WHERE log_id = @log_id ORDER BY row_order, id", param);
                detail.TubeFeedings = _db.GetDataList_SQL<M_tube_feeding>(
                    "SELECT * FROM lw_tube_feeding WHERE log_id = @log_id ORDER BY meal_time_code, row_order, id", param);
                detail.FreeComments = _db.GetDataList_SQL<M_free_comment>(
                    "SELECT * FROM lw_free_comment WHERE log_id = @log_id ORDER BY id", param);

                // マスタ名前解決（列構成は ejOkinawaCentralhp リポジトリの database/tables を正とする）
                detail.WardNames = LoadMaster("SELECT ward_code AS code, COALESCE(ward_name, '') AS name FROM lw_m_ward");
                detail.RoomNames = LoadMaster("SELECT room_code AS code, COALESCE(room_name, '') AS name FROM lw_m_room");
                detail.MealNames = LoadMaster("SELECT meal_code AS code, COALESCE(meal_name, '') AS name FROM lw_m_meal");
                detail.MainDishNames = LoadMaster("SELECT main_dish_code AS code, COALESCE(main_dish_name, '') AS name FROM lw_m_main_dish");
                detail.CommentNames = LoadMaster("SELECT comment_code AS code, COALESCE(comment_name, '') AS name FROM lw_m_comment");
                detail.ContraNames = LoadMaster("SELECT contra_code AS code, COALESCE(contra_name, '') AS name FROM lw_m_contra_comment");
                detail.OtherCommentNames = LoadMaster("SELECT other_comment_code AS code, COALESCE(other_comment_name, '') AS name FROM lw_m_other_comment");
                detail.TubeProductNames = LoadMaster("SELECT tube_feeding_product_code AS code, COALESCE(tube_feeding_product_name, '') AS name FROM lw_m_tube_feeding_product");
            }
        }
        catch (Exception ex)
        {
            detail.IsConnected = false;
            detail.ErrorMessage = ex.Message;
            _logger.LogError(ex, "電文詳細でDBエラー（id: {Id}）", id);
        }

        return detail;
    }

    /// <summary>
    /// マスタ1つをコード→名前の辞書にして返す。
    /// マスタが空なら空辞書、テーブル未作成などで失敗しても空辞書を返して画面は出す（コード素通し表示になる）
    /// </summary>
    private Dictionary<string, string> LoadMaster(string sql)
    {
        var dic = new Dictionary<string, string>();
        try
        {
            foreach (var row in _db.GetDataList_SQL<M_CodeName>(sql))
            {
                if (!string.IsNullOrEmpty(row.code))
                {
                    // 病室マスタは病棟をまたいで同じ room_code があり得るため、重複キーは先勝ちで無視する
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
