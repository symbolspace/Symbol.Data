/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 接口：事务。
    /// </summary>
    public interface IAdoTransaction : ITransaction {

        /// <summary>
        /// 获取Ado事务对象。
        /// </summary>
        IDbTransaction DbTransaction { get; }

    }
}