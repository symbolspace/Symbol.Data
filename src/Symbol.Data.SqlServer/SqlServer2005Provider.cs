﻿/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

[assembly: Symbol.Data.Provider("mssql", typeof(Symbol.Data.SqlServer2005Provider))]

[assembly: Symbol.Data.Provider("sql2005", typeof(Symbol.Data.SqlServer2005Provider))]
[assembly: Symbol.Data.Provider("mssql2005", typeof(Symbol.Data.SqlServer2005Provider))]
[assembly: Symbol.Data.Provider("mssql.2005", typeof(Symbol.Data.SqlServer2005Provider))]

[assembly: Symbol.Data.Provider("sql2008", typeof(Symbol.Data.SqlServer2005Provider))]
[assembly: Symbol.Data.Provider("mssql2008", typeof(Symbol.Data.SqlServer2005Provider))]
[assembly: Symbol.Data.Provider("mssql.2008", typeof(Symbol.Data.SqlServer2005Provider))]

[assembly: Symbol.Data.Provider("sql2000", typeof(Symbol.Data.SqlServer2005Provider))]
[assembly: Symbol.Data.Provider("mssql2000", typeof(Symbol.Data.SqlServer2005Provider))]
[assembly: Symbol.Data.Provider("mssql.2000", typeof(Symbol.Data.SqlServer2005Provider))]
namespace Symbol.Data {

    /// <summary>
    /// SqlServer 2005数据库提供者
    /// </summary>
    public class SqlServer2005Provider : SqlServerProvider {

        #region properties
        /// <summary>
        /// 获取数据提供者版本
        /// </summary>
        public override string Version { get { return "2005"; } }

        #endregion

        #region IDatabaseProvider 成员

        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        public override IDataContext CreateDataContext(IConnection connection) {
            return new SqlServer2005DataContext(connection);
        }

        #endregion
    }
}

