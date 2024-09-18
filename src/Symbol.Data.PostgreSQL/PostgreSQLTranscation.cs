/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// PostgreSQL 事务
    /// </summary>
    public class PostgreSQLTransaction : AdoTransaction {

        #region ctor
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="connection">连接对象。</param>
        public PostgreSQLTransaction(AdoConnection connection)
            : base(connection) {
        }
        #endregion
    }
}

