/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data {

    /// <summary>
    /// PostgreSQL 事务
    /// </summary>
    public class PostgreSQLTransaction : AdoTransaction {

        #region properties
        /// <summary>
        /// 获取是否在事务中。
        /// </summary>
        public override bool Working {
            get {
                var transaction = (Npgsql.NpgsqlTransaction)DbTransaction;
                return transaction != null
#if !net20 && !net35
                    && !transaction.IsCompleted
#endif
                    ;
            }
        }

        #endregion

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

