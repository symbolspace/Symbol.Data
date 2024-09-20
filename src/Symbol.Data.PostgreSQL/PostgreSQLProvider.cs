/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using System.Data;
using Npgsql;

[assembly: Symbol.Data.Provider("pgsql", typeof(Symbol.Data.PostgreSQLProvider))]
[assembly: Symbol.Data.Provider("psql", typeof(Symbol.Data.PostgreSQLProvider))]
[assembly: Symbol.Data.Provider("npgsql", typeof(Symbol.Data.PostgreSQLProvider))]
[assembly: Symbol.Data.Provider("postgresql", typeof(Symbol.Data.PostgreSQLProvider))]
namespace Symbol.Data {

    /// <summary>
    /// PostgreSQL数据库提供者(9.x+)
    /// </summary>
    public class PostgreSQLProvider : AdoProvider {

        #region properties

        /// <summary>
        /// 获取数据提供者名称
        /// </summary>
        public override string Name { get { return "postgresql"; } }
        /// <summary>
        /// 获取数据提供者版本
        /// </summary>
        public override string Version { get { return "9.x"; } }

        #endregion

        #region ctor
        /// <summary>
        /// 创建 PostgreSQLProvider 的实例。
        /// </summary>
        public PostgreSQLProvider() {
        }
        #endregion

        #region IDatabaseProvider 成员
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        /// <returns>返回数据库连接。</returns>
        public override IConnection CreateConnection(string connectionString) {
            connectionString = connectionString?.Trim();
            CommonException.CheckArgumentNull(connectionString, nameof(connectionString));
            if (connectionString.StartsWith("{") && connectionString.EndsWith("}")) {
                return CreateConnection((object)connectionString);
            }
            return new PostgreSQLConnection(this,new NpgsqlConnection(connectionString), connectionString);
        }
        /// <summary>
        /// 创建数据库连接。
        /// </summary>
        /// <param name="connectionOptions">连接参数。</param>
        /// <returns>返回数据库连接。</returns>
        public override IConnection CreateConnection(object connectionOptions) {
            {
                if (connectionOptions is string connectionString) {
                    connectionString = connectionString.Trim();
                    if (connectionString.StartsWith("{")) {
                        connectionOptions = JSON.Parse(connectionString);
                        goto lb_Object;
                    }
                    return CreateConnection(connectionString);
                }
                if (connectionOptions is ConnectionOptions connectionOptions2) {
                    connectionOptions = connectionOptions2.ToObject();
                    goto lb_Object;
                }
            }
        lb_Object:
            System.Data.Common.DbConnectionStringBuilder builder
#if NET20 || NET35 || NET40
                = new NpgsqlConnectionStringBuilder()
#else
                = new NpgsqlConnectionStringBuilder(true)
#endif
            ;
            builder["Pooling"] = true;
            builder["MaxPoolSize"] = 1024;
            builder["Port"] = 5432;
            builder["Client Encoding"] = "utf-8";
            Collections.Generic.NameValueCollection<object> values = new Collections.Generic.NameValueCollection<object>(connectionOptions);
            SetBuilderValue(builder, values, "port", "Port");
            SetBuilderValue(builder, values, "host", "Host", p => {
                string p10 = p as string;
                if (string.IsNullOrEmpty(p10))
                    return p;
                if (p10.IndexOf(':') > -1) {
                    string[] pair = p10.Split(':');
                    SetBuilderValue(builder, "Port", pair[1]);
                    return pair[0];
                }
                return p;
            });
            SetBuilderValue(builder, values, "name", "Database");
            SetBuilderValue(builder, values, "account", "Username");
            SetBuilderValue(builder, values, "password", "Password");
            foreach (System.Collections.Generic.KeyValuePair<string, object> item in values) {
                //builder[item.Key] = item.Value;
                SetBuilderValue(builder, item.Key, item.Value);
            }
            return CreateConnection(builder.ConnectionString);
        }
        /// <summary>
        /// 创建数据上下文。
        /// </summary>
        /// <param name="connection">数据库连接。</param>
        /// <returns>返回数据上下文。</returns>
        public override IDataContext CreateDataContext(IConnection connection) {
            return new PostgreSQLDataContext(connection);
        }

        /// <summary>
        /// 创建方言。
        /// </summary>
        /// <returns>返回方言对象。</returns>
        public override IDialect CreateDialect() {
            return new PostgreSQLDialect();
        }

        #endregion

        #region methods

        #region MapTypes
        delegate bool MapTypes_Filter(System.Type type);
        /// <summary>
        /// 映射数据库中的类型、枚举、表
        /// </summary>
        /// <param name="assembly">包含类型的程序集</param>
        /// <param name="perfix">前辍过滤</param>
        /// <param name="connection">连接实例</param>
        public static void MapTypes(System.Reflection.Assembly assembly, string perfix = null, object connection = null) {
            MapTypeHelper helper = new MapTypeHelper(connection);

            MapTypes_Filter filter = null;
            if (string.IsNullOrEmpty(perfix))
                filter = (p1) => true;
            else
                filter = (p1) => p1.Name.StartsWith(perfix, System.StringComparison.OrdinalIgnoreCase);

            foreach (System.Type type in assembly.GetTypes()) {
                if (type.IsAbstract || !filter(type))
                    continue;
                helper.MapType(type);
            }

        }
        #endregion
        #region MapType
        /// <summary>
        /// 映射数据库中的类型、枚举、表
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="connection">连接实例</param>
        /// <returns>返回是否成功</returns>
        public static bool MapType(System.Type type, object connection = null) {
            if (type == null || type.IsAbstract)
                return false;

            MapTypeHelper helper = new MapTypeHelper(connection);
            return helper.MapType(type);
        }
        #endregion


        #endregion

        #region types
        class MapTypeHelper {
            private System.Type _connectionType;
            private System.Reflection.MethodInfo _mapCompositeGlobally;
            private System.Reflection.MethodInfo _mapEnumGlobally;
            private System.Reflection.MethodInfo _mapComposite;
            private System.Reflection.MethodInfo _mapEnum;

            private object _connection = null;

            public MapTypeHelper(object connection = null) {
                _connection = connection;
                _connectionType = typeof(NpgsqlConnection);
                if (_connection == null) {
                    _mapCompositeGlobally = _connectionType.GetMethod("MapCompositeGlobally", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod);
                    _mapEnumGlobally = _connectionType.GetMethod("MapEnumGlobally", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod);
                } else {
                    _mapComposite = _connectionType.GetMethod("MapComposite", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod);
                    _mapEnum = _connectionType.GetMethod("MapEnum", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod);
                }
            }
            public bool MapType(System.Type type, string pgName = null, object nameTranslator = null) {
                if (type == null || type.IsAbstract)
                    return false;

                if (type.IsClass) {
                    if (_connection == null) {
                        _mapCompositeGlobally.MakeGenericMethod(type).Invoke(null, new object[2]);
                    } else {
                        _mapComposite.MakeGenericMethod(type).Invoke(_connection, new object[2]);
                    }
                } else if (type.IsEnum) {
                    if (_connection == null) {
                        _mapEnumGlobally.MakeGenericMethod(type).Invoke(null, new object[] { type.Name.ToLower(), null });
                    } else {
                        _mapEnum.MakeGenericMethod(type).Invoke(_connection, new object[] { type.Name.ToLower(), null });
                    }
                }
                return true;
            }
        }

        #endregion

    }
}

