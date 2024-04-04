/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;

namespace Symbol.Data {

    /// <summary>
    /// 接口：ADO.NET 数据查询（泛型）。
    /// </summary>
    /// <typeparam name="T">任意类型。</typeparam>
    public interface IAdoDataQuery<T> 
        : IDataQuery<T> {

    }
}