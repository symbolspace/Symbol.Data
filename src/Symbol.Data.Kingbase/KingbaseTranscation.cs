/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.Kingbase {

    /// <summary>
    /// Kingbase 事务
    /// </summary>
    public class KingbaseTransaction : AdoTransaction {

        #region ctor
        /// <summary>
        /// 创建实例。
        /// </summary>
        /// <param name="connection">连接对象。</param>
        public KingbaseTransaction(AdoConnection connection)
            : base(connection) {
        }
        #endregion
    }
}

