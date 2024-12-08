using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.ModelClass
{
    public class PreLoadData
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SQLite.Column("id"),SQLite.AutoIncrement,SQLite.PrimaryKey]
        public long Id { get; set; }

        /// <summary>
        /// 模组名称
        /// </summary>
        [SQLite.Column("modname")]
        public string ModName { get; set; } = string.Empty;

        /// <summary>
        /// 类名称
        /// </summary>
        [SQLite.Column("classname")]
        public string ClassName { get; set; } = string.Empty;

        /// <summary>
        /// 方法名称
        /// </summary>
        [SQLite.Column("methodname")]
        public string MethodName { get; set; } = string.Empty;

        /// <summary>
        /// 英文
        /// </summary>
        [SQLite.Column("english")]
        public string English { get; set; } = string.Empty;

        /// <summary>
        /// 中文
        /// </summary>
        [SQLite.Column("chinese")]
        public string Chinese {  get; set; } = string.Empty;
        
        /// <summary>
        /// 纯机翻
        /// </summary>
        [SQLite.Column("autochinese")]
        public string AutoChinese { get; set; } = string.Empty;
    }
}
