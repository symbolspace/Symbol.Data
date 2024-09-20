/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

namespace Symbol.Data.Kingbase {

    /// <summary>
    /// Kingbase 插入命令构造器
    /// </summary>
    public class KingbaseInsertCommandBuilder : Symbol.Data.InsertCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建KingbaseInsertCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public KingbaseInsertCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion

    }
}

