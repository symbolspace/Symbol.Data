/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using KdbndpTypes;

namespace Symbol.Data.Kingbase {

    /// <summary>
    /// Kingbase 命令参数列表
    /// </summary>
    public class KingbaseCommandParameterList : CommandParameterList {

        #region fields
        #endregion

        #region ctor
        /// <summary>
        /// 创建KingbaseCommandParameterList实例。
        /// </summary>
        /// <param name="provider">提供者。</param>
        public KingbaseCommandParameterList(IProvider provider)
            : base(provider) {
        }
        #endregion


        #region methods

        /// <summary>
        /// 创建参数回调
        /// </summary>
        /// <param name="item">参数对象</param>
        protected override void OnCreate(CommandParameter item) {
            if (item.Value == null) {
                return;
            }
            if (item.Value.GetType() != item.RealType) {
                item.Value = TypeExtensions.Convert(item.Value, item.RealType);
            }

            if (item.RealType.IsEnum) {
                item.RealType = typeof(long);
                item.Value = TypeExtensions.Convert<long>(item.Value);
                return;
            }
            if (item.RealType.IsArray) {
                var elementType = item.RealType.GetElementType();
                if (elementType == typeof(string)) {
                    item.Properties["KdbndpDbType"] = KdbndpDbType.Array | KdbndpDbType.Text;
                    return;
                }
                if(elementType== typeof(byte)) {
                    item.Properties["KdbndpDbType"] = KdbndpDbType.Bytea;
                    return;
                }
                item.Properties["KdbndpDbType"] = KdbndpDbType.Json;
                item.RealType = typeof(object);
                item.Value = item.Value == null ? null : JSON.ToJSON(item.Value);
                return;
            }
            if (
                   item.RealType == typeof(object)
                || TypeExtensions.IsAnonymousType(item.RealType)
                || TypeExtensions.IsInheritFrom(item.RealType, typeof(System.Collections.Generic.IDictionary<string, object>))
                || (item.RealType.IsClass && !TypeExtensions.IsSystemBaseType(item.RealType))
            ) {
                item.Properties["KdbndpDbType"] = KdbndpDbType.Json;
                item.RealType = typeof(object);
                item.Value = item.Value == null ? null : JSON.ToJSON(item.Value);
                return;
            }

        }

        #endregion
    }

}

