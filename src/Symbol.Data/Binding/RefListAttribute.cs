﻿/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */
using System;

namespace Symbol.Data.Binding {

    /// <summary>
    /// 引用列表数据绑定特性（支持List&lt;T&gt;和T[]，推荐前者）。
    /// </summary>
    public class RefListAttribute : DataBinderAttribute {

        #region properties
        /// <summary>
        /// 获取或设置源字段。
        /// </summary>
        public string SourceField { get; set; }
        /// <summary>
        /// 获取或设置目标名称。
        /// </summary>
        public string TargetName { get; set; }
        /// <summary>
        /// 获取或设置目标字段。
        /// </summary>
        public string TargetField { get; set; }

        #endregion

        #region ctor
        /// <summary>
        /// 创建RefListAttribute实例。
        /// </summary>
        /// <param name="targetName">目标名称。</param>
        /// <param name="targetField">目标字段。</param>
        /// <param name="sourceName">源名称。</param>
        /// <param name="sourceField">源字段。</param>
        /// <param name="condition">过虑规则。</param>
        /// <param name="field">输出字段，输出为单值时。</param>
        /// <param name="sort">排序规则。</param>
        public RefListAttribute(string targetName, string targetField, string sourceName, string sourceField, string condition, string field = "*", string sort = "{}")
            : base(sourceName, condition, field, sort) {
            SourceField = sourceField;
            TargetName = targetName;
            TargetField = targetField;
        }
        #endregion

        #region methods

        #region Bind
        /// <summary>
        /// 绑定数据。
        /// </summary>
        /// <param name="dataContext">数据上下文对象。</param>
        /// <param name="reader">数据查询读取器。</param>
        /// <param name="entity">当前实体对象。</param>
        /// <param name="field">当前字段。</param>
        /// <param name="type">实体中字段的类型。</param>
        /// <param name="cache">缓存。</param>
        /// <returns>返回绑定的数据。</returns>
        public override object Bind(IDataContext dataContext, IDataQueryReader reader, object entity, string field, Type type, IDataBinderObjectCache cache) {
            var elementType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
            bool isSingleValue = (elementType == typeof(string) || elementType.IsValueType || TypeExtensions.IsNullableType(elementType));

            if (isSingleValue && (string.IsNullOrEmpty(Field) || Field == "*"))
                Field = "id";

            string fix = "__";
            using (var builder = dataContext.CreateSelect(SourceName)) {
                //PreSelectBuilder(dataContext, dataReader, entity, builder, cache);
                if (isSingleValue) {
                    builder.Select(builder.Dialect.PreName(new string[] { fix, Field }));
                } else {
                    builder.Select(builder.Dialect.PreName(fix) + ".*");
                }
                builder.Refer(NoSQL.Refer.Begin().Refence(fix, TargetName, TargetField, SourceName, SourceField).Json());
                var conditiion = MapObject(Condition, dataContext, entity, reader);
                builder.Query(conditiion).Sort(Sorter);
                return CacheFunc(cache, builder, "list.ref", type, () => {
                    var list = (System.Collections.IList)FastWrapper.CreateInstance(type);
                    using var q = builder.CreateQuery(elementType);
                    q.Command.AllowNoTransaction = true;
                    q.DataBinderObjectCache = cache;
                    foreach (var item in q) {
                        list.Add(item);
                    }
                    if (type.IsArray) {
                        var array = Array.CreateInstance(elementType, list.Count);
                        list.CopyTo(array, 0);
                        return array;
                    }
                    return list;
                });
            }
        }
        #endregion

        #endregion
    }
}