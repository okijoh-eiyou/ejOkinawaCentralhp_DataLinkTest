using Dapper;
using Microsoft.AspNetCore.Mvc;
using lw_Confirmation_of_received_telegram.Models;
using lw_Confirmation_of_received_telegram.Services;

namespace lw_Confirmation_of_received_telegram.Controllers;

/// <summary>
/// マスタ保守（プレビュー版・2026-09-18）。
/// 一覧＋行内編集のUIまで実装済み。ただし【DBへの書き込み（INSERT/UPDATE/DELETE）は未実装】。
/// CLAUDE.md の読み取り専用ルール（2026-07-31記録）の改定を上司と合意してから、
/// Save/Delete の「※プレビュー版」ブロックに書き込み処理を実装する。
/// それまでは入力チェックと重複チェック（SELECTのみ）だけが動く。
/// SQLに使うテーブル名・列名は M_View_MasterRegistry の固定定義のみ。値は必ずパラメータ渡し。
/// </summary>
public class MasterController : Controller
{
    private readonly ILogger<MasterController> _logger;
    private readonly Connect_PostgreSQL _db;

    public MasterController(ILogger<MasterController> logger, Connect_PostgreSQL db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>マスタ保守メニュー。対象8マスタをマスタ名のみのボタンで出す（DBアクセスなし）</summary>
    public IActionResult Index()
    {
        return View(M_View_MasterRegistry.All);
    }

    /// <summary>
    /// マスタの一覧・行内編集。editId=その行を入力欄にする / adding=先頭に新規追加行を出す
    /// </summary>
    public IActionResult Edit(string table, int? editId = null, bool adding = false)
    {
        var def = M_View_MasterRegistry.Find(table);
        if (def == null)
        {
            return NotFound();
        }

        var page = BuildEditPage(def, adding ? null : editId, adding);

        // モック保存・削除後のメッセージ（Save/Delete からリダイレクトで戻ってきたとき）
        if (TempData["MasterMessage"] is string msg && msg != "")
        {
            page.Messages.Add(msg);
            page.MessageKind = TempData["MasterMessageKind"] as string ?? "info";
        }

        // 編集開始時は現在値を入力欄に入れる
        if (page.EditId is int targetId)
        {
            var row = page.Rows.FirstOrDefault(r => r.Id == targetId);
            if (row != null)
            {
                page.EditValues = new Dictionary<string, string>(row.Values);
            }
            else
            {
                page.EditId = null;   // 対象行が消えていたら通常表示に戻す
            }
        }

        return View(page);
    }

    /// <summary>
    /// 保存（id=null なら新規追加、あれば変更）。
    /// ※プレビュー版: 入力チェック・重複チェックのみ実施し、DBへは書き込まない
    /// </summary>
    [HttpPost]
    public IActionResult Save(string table, int? id, Dictionary<string, string>? values)
    {
        var def = M_View_MasterRegistry.Find(table);
        if (def == null)
        {
            return NotFound();
        }

        values ??= new Dictionary<string, string>();
        var cleaned = def.Columns.ToDictionary(
            c => c.ColumnName,
            c => (values.GetValueOrDefault(c.ColumnName) ?? "").Trim());

        var errors = Validate(def, cleaned, excludeId: id);
        if (errors.Count > 0)
        {
            // エラー時は入力値を保持したまま同じ編集状態で再表示する
            var page = BuildEditPage(def, id, adding: id == null);
            page.EditValues = cleaned;
            page.Messages = errors;
            page.MessageKind = "danger";
            return View("Edit", page);
        }

        // ※プレビュー版: ここに INSERT / UPDATE を実装する（上司とルール改定合意後）。現時点では書き込まない
        TempData["MasterMessage"] = id == null
            ? "入力チェックOK（追加）。※プレビュー版のため、DBへは保存していません。"
            : "入力チェックOK（変更）。※プレビュー版のため、DBへは保存していません。";
        TempData["MasterMessageKind"] = "warning";
        return RedirectToAction(nameof(Edit), new { table });
    }

    /// <summary>削除。※プレビュー版: DBからは削除しない</summary>
    [HttpPost]
    public IActionResult Delete(string table, int id)
    {
        var def = M_View_MasterRegistry.Find(table);
        if (def == null)
        {
            return NotFound();
        }

        // ※プレビュー版: ここに DELETE を実装する（上司とルール改定合意後）。現時点では書き込まない
        TempData["MasterMessage"] = $"削除対象 id={id} を受け付けました。※プレビュー版のため、DBからは削除していません。";
        TempData["MasterMessageKind"] = "warning";
        return RedirectToAction(nameof(Edit), new { table });
    }

    /// <summary>一覧を読み込んで画面モデルを組む（DB接続断でも画面は返す）</summary>
    private M_View_MasterEditPage BuildEditPage(M_View_MasterDef def, int? editId, bool adding)
    {
        var page = new M_View_MasterEditPage
        {
            Def = def,
            EditId = adding ? null : editId,
            Adding = adding,
        };

        try
        {
            // 列構成がマスタごとに違うため、全列を ::text で v1〜v6 の固定別名に載せ替えて受ける。
            // テーブル名・列名・並び順はレジストリの固定値のみ
            var selectColumns = string.Join(", ",
                def.Columns.Select((c, i) => $"{c.ColumnName}::text AS v{i + 1}"));
            var raws = _db.GetDataList_SQL<M_View_MasterRawRow>(
                $"SELECT id, {selectColumns} FROM {def.TableName} ORDER BY {def.OrderBy}");

            foreach (var raw in raws)
            {
                var row = new M_View_MasterEditRow { Id = raw.id };
                for (var i = 0; i < def.Columns.Count; i++)
                {
                    row.Values[def.Columns[i].ColumnName] = raw.GetV(i + 1) ?? "";
                }
                page.Rows.Add(row);
            }

            page.IsConnected = true;
        }
        catch (Exception ex)
        {
            page.IsConnected = false;
            page.ErrorMessage = ex.Message;
            _logger.LogError(ex, "マスタ一覧の取得でエラー: {Table}", def.TableName);
        }

        return page;
    }

    /// <summary>入力チェック＋キー重複チェック（重複チェックはSELECTのみ・値はパラメータ渡し）</summary>
    private List<string> Validate(M_View_MasterDef def, Dictionary<string, string> values, int? excludeId)
    {
        var errors = new List<string>();

        foreach (var col in def.Columns)
        {
            var value = values.GetValueOrDefault(col.ColumnName, "");
            if (col.Required && value == "")
            {
                errors.Add($"{col.Label}は必須です。");
            }
            else if (col.IsInt)
            {
                if (value != "" && !int.TryParse(value, out _))
                {
                    errors.Add($"{col.Label}は整数で入力してください。");
                }
            }
            else if (value.Length > col.MaxLength)
            {
                errors.Add($"{col.Label}は{col.MaxLength}文字以内で入力してください。");
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        try
        {
            // ユニークキーの重複チェック。列名はレジストリ固定・値は @パラメータ
            var where = string.Join(" AND ", def.KeyColumns.Select((c, i) => $"{c} = @k{i}"));
            var sql = $"SELECT COUNT(*) FROM {def.TableName} WHERE {where}";
            var param = new DynamicParameters();
            for (var i = 0; i < def.KeyColumns.Count; i++)
            {
                param.Add($"k{i}", values[def.KeyColumns[i]]);
            }
            if (excludeId is int ex)
            {
                sql += " AND id <> @exclude_id";
                param.Add("exclude_id", ex);
            }

            if (_db.GetScalar_SQL<long>(sql, param) > 0)
            {
                var keyLabels = string.Join("＋", def.Columns
                    .Where(c => def.KeyColumns.Contains(c.ColumnName)).Select(c => c.Label));
                errors.Add($"同じ{keyLabels}の行が既に存在します。");
            }
        }
        catch (Exception ex2)
        {
            errors.Add("重複チェックが実行できませんでした（DB接続を確認してください）。");
            _logger.LogWarning(ex2, "マスタ重複チェックに失敗: {Table}", def.TableName);
        }

        return errors;
    }
}
