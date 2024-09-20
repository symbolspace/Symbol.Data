/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
 
namespace Symbol.Data.Kingbase {

    /// <summary>
    /// Kingbase 更新命令构造器
    /// </summary>
    public class KingbaseUpdateCommandBuilder : Symbol.Data.UpdateCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建KingbaseUpdateCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        public KingbaseUpdateCommandBuilder(IDataContext dataContext, string tableName)
            : base(dataContext, tableName) {
        }
        #endregion

      
    }
}

