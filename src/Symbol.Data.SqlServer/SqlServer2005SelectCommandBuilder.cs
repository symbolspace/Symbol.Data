/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using Symbol.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Symbol.Data {

    /// <summary>
    /// SqlServer 查询命令构造器基类
    /// </summary>
    public class SqlServer2005SelectCommandBuilder : Symbol.Data.SelectCommandBuilder, ISelectCommandBuilder {

        #region fields
        private bool _sql2012 = false;
        #endregion

        #region ctor
        /// <summary>
        /// 创建SqlServer2005SelectCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        /// <param name="commandText"></param>
        public SqlServer2005SelectCommandBuilder(IDataContext dataContext, string tableName, string commandText)
            : base(dataContext, tableName, commandText) {
        }
        #endregion

        #region methods

        #region Parse
        /// <summary>
        /// 预处理：解析命令脚本
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        protected override string PreParse(string commandText) {
            commandText = base.PreParse(commandText);
            //top
            commandText = CommandTextGrammarReplace(commandText, "top");
            //rows
            commandText = CommandTextGrammarReplace(commandText, "rows");
            //next
            commandText = CommandTextGrammarReplace(commandText, "next");
            return commandText;
        }

        /// <summary>
        /// 解析命令脚本。
        /// </summary>
        /// <param name="commandText">命令脚本。</param>
        protected override void Parse(string commandText) {
            commandText = PreParse(commandText);

            var beginIndex = 0;
            int endIndex;

            //传统分页解析（兼容性差，只能做到尽可能识别）
            bool hasRowNumber = commandText.IndexOf("row_number()", System.StringComparison.OrdinalIgnoreCase) > -1;
            if (hasRowNumber) {
                //select * from( select row_number() over(order by [t_Row_Number]) as [Row_Number],* from (
                //    sql 
                //) t ) tt where [Row_Number]>3


                //跨首层 row_number() 和 from 
                {
                    StringExtractHelper.StringsStartEnd(commandText, "row_number()", new string[] { " from " }, beginIndex, false, false, false, out endIndex);
                    if (endIndex > -1) {
                        beginIndex = endIndex;

                        //跨(
                        endIndex = commandText.IndexOf("(", beginIndex);
                        if (endIndex > -1) {
                            beginIndex = endIndex + 1;
                        }
                        //抹除开头
                        commandText = commandText.Substring(beginIndex);
                        beginIndex = 0;
                    }
                }

                //抹除末尾，并解析skip
                {
                    endIndex = commandText.LastIndexOf(") t ) tt where ", System.StringComparison.OrdinalIgnoreCase);
                    if (endIndex > -1) {
                        var content = commandText.Substring(endIndex);
                        commandText = commandText.Substring(beginIndex, endIndex);
                        content = StringExtractHelper.StringsStartEnd(content, ">", "[*]$");
                        var match = System.Text.RegularExpressions.Regex.Match(content, "(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        SkipCount = TypeExtensions.Convert(match.Groups[1].Value, 0);
                    }
                }
            }

            //select 
            {
                var content = StringExtractHelper.StringsStartEnd(commandText, "select ", new string[] { " from " }, beginIndex, false, false, false, out endIndex);
                if (endIndex != -1) {
                    ParseFields(content);
                    beginIndex = endIndex;
                    //传统分页时
                    if (hasRowNumber) {
                        //清理row number字段
                        _fields.RemoveWhere(p => {
                            if (p.IndexOf("Row_Number", System.StringComparison.OrdinalIgnoreCase) > -1)
                                return true;
                            var find = "row[*]number";
                            return StringExtractHelper.StringIndexOf(p, ref find, false) > -1;
                        });

                        //此处的TakeCount要减去SkipCount
                        if (TakeCount > 0 && SkipCount > 0) {
                            TakeCount -= SkipCount;
                        }
                    }
                    
                } else {
                    throw new System.InvalidOperationException("没有“select ”：" + commandText);
                }
            }

            //where before
            {
                var content = StringExtractHelper.StringsStartEnd(commandText, " from ", new string[] { " where ", " group by ", " order by ", " offset ", "[*]$" }, beginIndex, false, false, false, out endIndex);
                if (endIndex != -1) {
                    ParseWhereBefore(content);
                    beginIndex = endIndex;
                }
            }
            //where
            {
                var content = StringExtractHelper.StringsStartEnd(commandText, " where ", new string[] { " group by ", " order by ", " offset ", "[*]$" }, beginIndex, false, false, false, out endIndex);
                if (endIndex != -1) {
                    ParseWhere(content);
                    beginIndex = endIndex;
                }
            }
            //group by
            bool hasGroupBy = false;
            {
                var content = StringExtractHelper.StringsStartEnd(commandText, " group by ", new string[] { " having ", " order by ", " offset ", "[*]$" }, beginIndex, false, false, false, out endIndex);
                if (endIndex != -1) {
                    ParseGroupBy(content);
                    beginIndex = endIndex;
                    hasGroupBy = true;
                }
            }
            //having
            if (hasGroupBy) {
                var content = StringExtractHelper.StringsStartEnd(commandText, " having ", new string[] { " order by ", " offset ", "[*]$" }, beginIndex, false, false, false, out endIndex);
                if (endIndex != -1) {
                    ParseHaving(content);
                    beginIndex = endIndex;
                }
            }
            //order by
            {
                var content = StringExtractHelper.StringsStartEnd(commandText, " order by ", new string[] { " offset ", "[*]$" }, beginIndex, false, false, false, out endIndex);
                if (endIndex != -1) {
                    ParseOrderBy(content);
                    beginIndex = endIndex;
                }
            }
            //offset
            bool hasOffset = false;
            {
                var content = StringExtractHelper.StringsStartEnd(commandText, " offset ", new string[] { "rows" }, beginIndex, false, false, false, out endIndex);
                if (endIndex != -1) {
                    ParseOffset(content);
                    beginIndex = endIndex;
                    hasOffset = true;
                    _sql2012 = true;
                }
            }
            //next
            if (hasOffset) {
                var content = StringExtractHelper.StringsStartEnd(commandText, " next ", new string[] { "rows" }, beginIndex, false, false, false, out endIndex);
                if (endIndex != -1) {
                    ParseNext(content);
                    beginIndex = endIndex;
                }
            }
        }
       
       
        void ParseWhereBefore(string text) {
            if (string.IsNullOrEmpty(text))
                return;
            bool b = false;
            int i = -1;
            if (text[0] == '(') {
                int x = text.IndexOf(')', 1);
                if (x > -1) {
                    b = true;
                    _tableName = text.Substring(0, x + 1);
                    text = text.Substring(x + 1);
                    i = 0;
                }
            }
            if (!b) {
                int i1 = text.IndexOf(' ');
                int i2 = text.IndexOf("]");
                i = System.Math.Max(i1, i2);
                if (i == -1) {
                    _tableName = text;
                    return;
                }
                i++;
                _tableName = text.Substring(0, i).Trim();
            }
            if (!IsCustomTable)
                _tableName = _tableName.Trim('[', ']');
            if (i == text.Length)
                return;
            text = text.Substring(i)?.Trim('\r', '\n')?.Trim();
            if (!string.IsNullOrEmpty(text))
                _whereBefores.Add(text);
        }
        void ParseGroupBy(string text) {
            text = text?.Trim('\r', '\n')?.Trim();
            if (string.IsNullOrEmpty(text))
                return;
            GroupByKeys.Add(text);
        }
        void ParseHaving(string text) {
            text = text?.Trim('\r', '\n')?.Trim();
            if (!string.IsNullOrEmpty(text))
                Having(WhereOperators.And, text);
        }
        bool ParseFields(string fields) {
            fields = fields.Trim();
            if (string.IsNullOrEmpty(fields))
                return false;
            int i = fields.IndexOf("top ", System.StringComparison.OrdinalIgnoreCase);
            bool top = false;
            if (i != -1) {
                i += "top ".Length;
                int j = fields.IndexOf(' ', i);
                if (j != -1) {
                    TakeCount = TypeExtensions.Convert<int>(fields.Substring(i, j - i), 0);
                    top = true;
                    fields = fields.Substring(j + 1);
                } else {
                    fields = fields.Substring(i);
                }
            }
            foreach (var field in fields.Split(',')) {
                var name = field.Trim('\r', '\n')?.Trim();
                if (string.IsNullOrEmpty(name))
                    continue;
                _fields.Add(name);
            }
            return top;
        }
        private void ParseWhere(string text) {
            text = text?.Trim('\r', '\n')?.Trim();
            if (!string.IsNullOrEmpty(text))
                Where(WhereOperators.And, text);
        }
        private void ParseOrderBy(string text) {
            text = text?.Trim('\r', '\n')?.Trim();
            if (string.IsNullOrEmpty(text))
                return;
            foreach (var item in text.Split(',')) {
                var name = item.Trim('\r', '\n')?.Trim();
                if (string.IsNullOrEmpty(name))
                    continue;
                _orderbys.Add(name);
            }
        }
        void ParseOffset(string text) {
            var match = System.Text.RegularExpressions.Regex.Match(text, "(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            SkipCount = TypeExtensions.Convert(match.Groups[1].Value, 0);
        }
        void ParseNext(string text) {
            var match = System.Text.RegularExpressions.Regex.Match(text, "(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            TakeCount = TypeExtensions.Convert(match.Groups[1].Value, 0);
        }
        
        #endregion

        #region BuilderCommandText
        /// <summary>
        /// 构造命令脚本。
        /// </summary>
        /// <returns>返回命令脚本。</returns>
        protected override string BuilderCommandText() {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            BuildSelect(builder);
            BuildWhereBefore(builder);
            BuildWhere(builder);
            BuildGroupBy(builder);
            BuildHaving(builder);
            BuildOrderBy(builder);
            BuildSkip(builder);
            return builder.ToString();
        }
        void BuildSkip(System.Text.StringBuilder builder) {

            if (_sql2012) {
                if (TakeCount < 1 && SkipCount < 1)
                    return;
                if (OrderBys.Count == 0) {
                    builder.AppendLine().AppendLine(" order by 1 ");
                }
                if (TakeCount < 1) {
                    builder.AppendLine().AppendFormat(" offset {0} rows", SkipCount);
                    return;
                }

                builder.AppendLine().AppendFormat(" offset {0} rows ", SkipCount < 0 ? 0 : SkipCount)
                       .AppendLine().AppendFormat(" fetch next {0} rows only", TakeCount);
            } else {
                if (SkipCount == 0)
                    return;
                builder.Insert(0, "select * from( select row_number() over(order by [t_Row_Number]) as [Row_Number],* from (\r\n\r\n");
                builder.AppendLine().AppendLine()
                       .Append(") t ) tt where [Row_Number]>").Append(SkipCount);
            }

        }
        /// <summary>
        /// 构造select脚本
        /// </summary>
        /// <param name="builder">构造缓存。</param>
        protected override void BuildSelect(System.Text.StringBuilder builder) {
            builder.AppendLine(" select ");
            if (!_sql2012) {
                int top = TakeCount + SkipCount;
                if (top > 0) {
                    builder.Append("    top ").Append(top).AppendLine(" ");
                }
            }

            if (!_sql2012) {
                if (SkipCount > 0) {
                    builder.AppendLine("    [t_Row_Number]=0,");
                }
            }
            BuildSelectFields(builder);
            BuildFrom(builder);
        }
        #endregion

        #endregion
    }
}

