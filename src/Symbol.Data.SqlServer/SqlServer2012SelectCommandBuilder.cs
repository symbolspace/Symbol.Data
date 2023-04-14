/*  
 *  author：symbolspace
 *  e-mail：symbolspace@outlook.com
 */

using Symbol.Text;

namespace Symbol.Data {

    /// <summary>
    /// SqlServer 查询命令构造器基类
    /// </summary>
    public class SqlServer2012SelectCommandBuilder : Symbol.Data.SelectCommandBuilder, ISelectCommandBuilder {

        #region ctor
        /// <summary>
        /// 创建SqlServer2012SelectCommandBuilder实例。
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="tableName"></param>
        /// <param name="commandText"></param>
        public SqlServer2012SelectCommandBuilder(IDataContext dataContext, string tableName, string commandText)
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
            commandText= base.PreParse(commandText);
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

            //select 
            var beginIndex = 0;
            int endIndex;
            {
                var content = StringExtractHelper.StringsStartEnd(commandText, "select ", new string[] { " from " }, beginIndex, false, false, false, out endIndex);
                if (endIndex != -1) {
                    ParseFields(content);
                    beginIndex = endIndex;
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

        }
        #endregion

        #endregion

    }
}

