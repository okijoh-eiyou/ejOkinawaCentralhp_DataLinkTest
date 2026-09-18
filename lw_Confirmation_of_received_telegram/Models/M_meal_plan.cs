namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// lw_meal_plan：食事計画（先輩作成。「この日・この食事に誰が載るか」の目次）
    /// ※Disp_Json(列) は仕様確定待ちのため、当面このモデルでは扱わない
    /// </summary>
    public class M_meal_plan
    {
        public int plan_id { get; set; }

        /// <summary>転記元ログID（その時点で有効な電文）</summary>
        public int log_id { get; set; }

        /// <summary>有効フラグ</summary>
        public bool? is_active { get; set; }

        /// <summary>処理済みフラグ</summary>
        public bool? is_process { get; set; }

        /// <summary>日付（配膳する日）</summary>
        public DateOnly? meal_date { get; set; }

        /// <summary>時間区分 (1:朝, 2:昼, 3:夕)</summary>
        public int? meal_type { get; set; }

        /// <summary>患者番号</summary>
        public string patient_number { get; set; } = "";
    }
}
