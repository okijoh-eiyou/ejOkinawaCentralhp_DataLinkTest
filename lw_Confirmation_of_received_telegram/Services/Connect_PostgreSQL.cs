using Dapper;
using Npgsql;
using System.Data;

namespace lw_Confirmation_of_received_telegram.Services
{
    /// <summary>
    /// PostgreSQL接続とクエリ実行（屋宜原版 Modules/Connect_PostgreSQL.cs のMVC版）
    /// 接続文字列は appsettings(.Development).json の ConnectionStrings:Default から取得
    /// </summary>
    public class Connect_PostgreSQL
    {
        private readonly string ConnectionString;

        public Connect_PostgreSQL(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("接続文字列 ConnectionStrings:Default が設定されていません。appsettings.Development.json を確認してください。");
        }

        /// <summary>
        /// SELECT結果をリストで返す（パラメータ付き。SQLには @名前 で埋め込む）
        /// </summary>
        public List<T> GetDataList_SQL<T>(string vSQL, object? vParam = null) where T : class
        {
            using (IDbConnection db = new NpgsqlConnection(ConnectionString))
            {
                return db.Query<T>(vSQL, vParam).ToList();
            }
        }

        /// <summary>
        /// 単一値（件数など）を返す
        /// </summary>
        public T GetScalar_SQL<T>(string vSQL, object? vParam = null)
        {
            using (IDbConnection db = new NpgsqlConnection(ConnectionString))
            {
                return db.ExecuteScalar<T>(vSQL, vParam)!;
            }
        }
    }
}
