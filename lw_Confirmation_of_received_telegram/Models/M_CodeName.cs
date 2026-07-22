namespace lw_Confirmation_of_received_telegram.Models
{
    /// <summary>
    /// マスタ名前解決用の汎用コード・名前ペア。
    /// 各マスタ（lw_m_ward / lw_m_meal など）を「SELECT xxx_code AS code, xxx_name AS name」で読むときの受け皿
    /// </summary>
    public class M_CodeName
    {
        public string code { get; set; } = "";
        public string name { get; set; } = "";
    }
}
