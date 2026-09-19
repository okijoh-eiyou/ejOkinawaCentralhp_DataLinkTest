using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using lw_Confirmation_of_received_telegram.Models;
using lw_Confirmation_of_received_telegram.Services;

namespace lw_Confirmation_of_received_telegram.Controllers;

/// <summary>
/// 食事一覧画面（トップ画面）。
/// 2026-09-18から【Disp_Json直表示版】: lw_meal_plan の disp_json をパースし、
/// キーを列ラベル・値をセルにして、jsonbが返す順のまま動的に表を組む（先輩のキー設計をそのまま映す）。
/// ※旧方式（展開テーブルから肉付け: IMealPlanProvider / MealPlanProviderBase）はコードを温存してあり、
/// 　戻す場合はこのコントローラの旧Indexコメントを復活させる
/// </summary>
public class MealPlanController : Controller
{
    private readonly ILogger<MealPlanController> _logger;
    private readonly Connect_PostgreSQL _db;
    private readonly IMealPlanProvider _provider;   // 旧方式用に温存（現在未使用）

    public MealPlanController(ILogger<MealPlanController> logger, Connect_PostgreSQL db, IMealPlanProvider provider)
    {
        _logger = logger;
        _db = db;
        _provider = provider;
    }

    /// <summary>
    /// 日付×食事区分（1:朝 2:昼 3:夕）の患者一覧。既定は今日の昼。
    /// 列は disp_json のキーから動的に決まる（全行を上から走査した出現順）
    /// </summary>
    public IActionResult Index(string? date, int mealType = 2, bool hideCodes = true,
        string[]? wardCode = null, string[]? mealCode = null, string[]? mainDishCode = null)
    {
        var page = new M_View_MealPlanJsonPage
        {
            TargetDate = DateOnly.TryParse(date, out var d) ? d : DateOnly.FromDateTime(DateTime.Today),
            MealType = mealType is >= 1 and <= 3 ? mealType : 2,
            HideCodes = hideCodes,
            // フィルタはチェックボックスの複数選択（?wardCode=001&wardCode=002 のように繰り返しで届く）
            WardCodes = (wardCode ?? Array.Empty<string>()).Where(c => c != "").ToList(),
            MealCodes = (mealCode ?? Array.Empty<string>()).Where(c => c != "").ToList(),
            MainDishCodes = (mainDishCode ?? Array.Empty<string>()).Where(c => c != "").ToList(),
        };

        // DBが落ちていても画面は必ず表示する
        try
        {
            // DateOnly はこのDapperのバージョンではパラメータにできないため、文字列で渡して ::date にキャスト。
            // is_active = true の行だけ表示する（2026-09-18 ルール変更。false は無効化された計画）
            var plans = _db.GetDataList_SQL<M_meal_plan>(
                "SELECT plan_id, patient_number, disp_json::text AS disp_json_text FROM lw_meal_plan " +
                "WHERE meal_date = @meal_date::date AND meal_type = @meal_type AND is_active = true " +
                "ORDER BY plan_id",
                new { meal_date = page.TargetDate.ToString("yyyy-MM-dd"), meal_type = page.MealType });

            foreach (var plan in plans)
            {
                var row = new M_View_MealPlanJsonRow
                {
                    PatientNumber = plan.patient_number,
                    DisplayPatientNumber = TrimLeadingZeros(plan.patient_number),
                };

                if (!string.IsNullOrEmpty(plan.disp_json_text))
                {
                    row.HasJson = true;
                    using var doc = JsonDocument.Parse(plan.disp_json_text);
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        // 「:」の左＝ラベル、右＝データ。null は空欄にする
                        var value = prop.Value.ValueKind switch
                        {
                            JsonValueKind.String => prop.Value.GetString() ?? "",
                            JsonValueKind.Null => "",
                            _ => prop.Value.ToString(),
                        };

                        // 表示用の整形: 患者番号は先頭ゼロを外す（検索用の原値は row.PatientNumber に保持）、
                        // 開始日は yyyyMMdd → yyyy/MM/dd(曜) にする（2026-09-19 ミーティング）
                        row.Values[prop.Name] = prop.Name switch
                        {
                            "患者番号" => TrimLeadingZeros(value),
                            "開始日" => FormatStartDate(value),
                            _ => value,
                        };

                        // 列ラベルは初出順（＝jsonbが返す順）に採用
                        if (!page.Columns.Contains(prop.Name))
                        {
                            page.Columns.Add(prop.Name);
                        }
                    }
                }

                page.Rows.Add(row);
            }

            // フィルタの選択肢は「この日付・時間帯の一覧に実際に出ている値」だけにする（2026-09-19 ミーティング）。
            // 名前も disp_json 内の値をそのまま使う（マスタ参照はしない）。絞り込み前の全行から作ること
            // （絞り込み後の行から作ると、チェックを広げ直せなくなる）
            page.WardOptions = BuildOptionsFromRows(page.Rows, "病棟コード", "病棟名");
            page.MealOptions = BuildOptionsFromRows(page.Rows, "食種コード", "食種名");
            page.MainDishOptions = BuildOptionsFromRows(page.Rows, "主食コード", "主食名");

            // 別の日付から引き継いだチェック済みコードが当日の一覧に無い場合も、外せるように選択肢へ残す
            AddMissingChecked(page.WardOptions, page.WardCodes);
            AddMissingChecked(page.MealOptions, page.MealCodes);
            AddMissingChecked(page.MainDishOptions, page.MainDishCodes);

            // フィルタ（disp_json内のコード値と突き合わせ。disp_json未生成の行はフィルタ時は対象外になる）。
            // 同じリスト内の複数チェックは「どれかに一致（OR）」、リスト同士は掛け合わせ（AND）
            if (page.FilterActive)
            {
                page.Rows = page.Rows.Where(r =>
                    (page.WardCodes.Count == 0 || page.WardCodes.Contains(r.Values.GetValueOrDefault("病棟コード") ?? "")) &&
                    (page.MealCodes.Count == 0 || page.MealCodes.Contains(r.Values.GetValueOrDefault("食種コード") ?? "")) &&
                    (page.MainDishCodes.Count == 0 || page.MainDishCodes.Contains(r.Values.GetValueOrDefault("主食コード") ?? ""))).ToList();
            }

            page.IsConnected = true;
        }
        catch (Exception ex)
        {
            page.IsConnected = false;
            page.ErrorMessage = ex.Message;
            _logger.LogError(ex, "食事一覧（Disp_Json）の取得でエラー（{Date} 区分{MealType}）", page.TargetDate, page.MealType);
        }

        return View(page);
    }

    /// <summary>患者番号の先頭ゼロを表示用に除去する（例: 0009900354 → 9900354。全部ゼロなら 0）</summary>
    private static string TrimLeadingZeros(string value)
    {
        var trimmed = value.TrimStart('0');
        return trimmed == "" && value != "" ? "0" : trimmed;
    }

    /// <summary>開始日（yyyyMMdd）を「yyyy/MM/dd(曜)」にする（例: 20260919 → 2026/09/19(土)）。形式外の値はそのまま返す</summary>
    private static string FormatStartDate(string value)
    {
        if (DateOnly.TryParseExact(value, "yyyyMMdd", out var date))
        {
            return $"{date:yyyy/MM/dd}({"日月火水木金土"[(int)date.DayOfWeek]})";
        }
        return value;
    }

    /// <summary>
    /// フィルタの選択肢を一覧の行（disp_json由来の値）から作る。
    /// コードの昇順。同じコードに複数の名前があった場合は最初に見つかった空でない名前を使う
    /// </summary>
    private static List<M_CodeName> BuildOptionsFromRows(List<M_View_MealPlanJsonRow> rows, string codeKey, string nameKey)
    {
        return rows
            .Where(r => r.HasJson)
            .Select(r => new
            {
                code = r.Values.GetValueOrDefault(codeKey) ?? "",
                name = r.Values.GetValueOrDefault(nameKey) ?? "",
            })
            .Where(x => x.code != "")
            .GroupBy(x => x.code)
            .OrderBy(g => g.Key)
            .Select(g => new M_CodeName
            {
                code = g.Key,
                name = g.Select(x => x.name).FirstOrDefault(n => n != "") ?? "",
            })
            .ToList();
    }

    /// <summary>チェック済みのコードが選択肢に無ければ末尾に足す（チェックを外せなくなるのを防ぐ）</summary>
    private static void AddMissingChecked(List<M_CodeName> options, List<string> checkedCodes)
    {
        foreach (var code in checkedCodes.Where(c => options.All(o => o.code != c)))
        {
            options.Add(new M_CodeName { code = code, name = code });
        }
    }
}
