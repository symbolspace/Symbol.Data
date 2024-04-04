/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 接口：ADO.NET 连接
    /// </summary>
    public interface IAdoConnection : IConnection {
        /// <summary>
        /// 获取Ado连接对象。
        /// </summary>
        IDbConnection DbConnection { get; }
        /// <summary>
        /// 获取Ado事务对象。
        /// </summary>
        IDbTransaction DbTransaction { get; }
    }
}