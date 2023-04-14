/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

[assembly: Symbol.Data.Provider("sql2012", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql2012", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql.2012", typeof(Symbol.Data.SqlServer2012Provider))]

[assembly: Symbol.Data.Provider("sql2014", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql2014", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql.2014", typeof(Symbol.Data.SqlServer2012Provider))]

[assembly: Symbol.Data.Provider("sql2016", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql2016", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql.2016", typeof(Symbol.Data.SqlServer2012Provider))]

[assembly: Symbol.Data.Provider("sql2017", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql2017", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql.2017", typeof(Symbol.Data.SqlServer2012Provider))]

[assembly: Symbol.Data.Provider("sql2019", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql2019", typeof(Symbol.Data.SqlServer2012Provider))]
[assembly: Symbol.Data.Provider("mssql.2019", typeof(Symbol.Data.SqlServer2012Provider))]
namespace Symbol.Data {

    /// <summary>
    /// SqlServer 2012数据库提供者
    /// </summary>
    public class SqlServer2012Provider : SqlServerProvider {

        #region properties
        /// <summary>
        /// 获取数据提供者版本
        /// </summary>
        public override string Version { get { return "2012"; } }

        #endregion


        #region IDatabaseProvider 成员

        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        public override IDataContext CreateDataContext(IConnection connection) {
            return new SqlServer2012DataContext(connection);
        }

        #endregion
    }
}

