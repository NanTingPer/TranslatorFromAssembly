using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.ModelClass
{
    /// <summary>
    /// Hjson数据模型
    /// </summary>
    public class HjsonModel
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        [SQLite.Column("id"),SQLite.AutoIncrement,SQLite.PrimaryKey]
        public long Id { get; set; } = 0;

        /// <summary>
        /// key
        /// </summary>
        [SQLite.Column("key")]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// 英文值
        /// </summary>
        [SQLite.Column("value")]
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 上一次的英文值
        /// </summary>
        [SQLite.Column("oldvalue")]
        public string OldValue { get; set; } = string.Empty;

        /// <summary>
        /// 中文值
        /// </summary>
        [SQLite.Column("chinese")]
        public string Chinese { get; set; } = string.Empty;

        /// <summary>
        /// 上一次的中文值
        /// </summary>
        [SQLite.Column("oldchinese")]
        public string OldChinese { get; set; } = string.Empty;
    }
}
