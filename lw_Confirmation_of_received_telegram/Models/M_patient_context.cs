namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// lw_patient_context：患者状況データ
    /// </summary>
    public class M_patient_context
    {
        public int id { get; set; }

        /// <summary>転記元ログID</summary>
        public int log_id { get; set; }

        /// <summary>患者番号</summary>
        public string patient_number { get; set; } = "";

        /// <summary>患者名（カナ）</summary>
        public string kana_name { get; set; } = "";

        /// <summary>患者名（漢字）</summary>
        public string kanji_name { get; set; } = "";

        /// <summary>患者性別 (1:男, 2:女)</summary>
        public string gender { get; set; } = "";

        /// <summary>患者性別内容</summary>
        public string gender_text { get; set; } = "";

        /// <summary>患者生年月日（DB列はDATE型。NpgsqlがDateOnlyで返すため型を合わせる）</summary>
        public DateOnly? birth_date { get; set; }

        /// <summary>患者身長(cm)</summary>
        public float? height_cm { get; set; }

        /// <summary>患者体重(kg)</summary>
        public float? weight_kg { get; set; }

        /// <summary>病棟コード</summary>
        public string ward_code { get; set; } = "";

        /// <summary>病室コード</summary>
        public string room_code { get; set; } = "";

        /// <summary>ベッドコード</summary>
        public string bed_code { get; set; } = "";

        /// <summary>科コード</summary>
        public string clinical_department_code { get; set; } = "";

        /// <summary>主治医コード</summary>
        public string doctor_code { get; set; } = "";

        /// <summary>主治医氏名</summary>
        public string doctor_name { get; set; } = "";

        /// <summary>食事加算区分</summary>
        public string addition_type { get; set; } = "";

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
