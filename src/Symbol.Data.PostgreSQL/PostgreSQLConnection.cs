/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// PostgreSQL 连接
    /// </summary>
    public class PostgreSQLConnection : AdoConnection {


        #region ctor
        /// <summary>
        /// 创建AdoConnection实例。
        /// </summary>
        /// <param name="provider">提供者。</param>
        /// <param name="connection">连接对象。</param>
        /// <param name="connectionString">连接字符串</param>
        public PostgreSQLConnection(IProvider provider, IDbConnection connection, string connectionString)
            : base(provider, connection, connectionString) {
        }
        #endregion

        #region methods

        /// <summary>
        /// 创建事务对象。
        /// </summary>
        /// <returns>返回事务对象。</returns>
        protected override ITransaction CreateTranscation() {
            var _transaction = new PostgreSQLTransaction(this);
            return _transaction;
        }

        #endregion
    }
}

