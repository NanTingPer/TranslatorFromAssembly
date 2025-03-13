using System.Text.Json.Serialization;

namespace TranslatorFormAssembly.Models
{
    /// <summary>
    /// 最后入库的类型 也是读取的类型
    /// </summary>
    public class PreLoadData
    {
        /// <summary>
        /// 唯一ID
        /// </summary>
        [SQLite.Column("id"), SQLite.AutoIncrement, SQLite.PrimaryKey]
        public long Id { get; set; }

        /// <summary>
        /// 模组名称
        /// </summary>
        [SQLite.Column("modname")]
        [JsonPropertyName("ModName")]
        public string ModName { get; set; } = string.Empty;

        /// <summary>
        /// 类名称
        /// </summary>
        [SQLite.Column("classname")]
        [JsonPropertyName("ClassName")]
        public string ClassName { get; set; } = string.Empty;

        /// <summary>
        /// 方法名称
        /// </summary>
        [SQLite.Column("methodname")]
        [JsonPropertyName("MethodName")]
        public string MethodName { get; set; } = string.Empty;

        /// <summary>
        /// 英文
        /// </summary>
        [SQLite.Column("english")]
        [JsonPropertyName("English")]
        public string English { get; set; } = string.Empty;

        /// <summary>
        /// 中文
        /// </summary>
        [SQLite.Column("chinese")]
        [JsonPropertyName("Translator")]
        public string Chinese { get; set; } = string.Empty;

        [SQLite.Column("hongkong")]
        [JsonPropertyName("HongKong")]
        public string HongKong { get; set; } = string.Empty;

        [SQLite.Column("taiwan")]
        [JsonPropertyName("TaiWan")]
        public string TaiWan { get; set; } = string.Empty;
        /// <summary>
        /// 纯机翻
        /// </summary>
        [SQLite.Column("autochinese")]
        public string AutoChinese { get; set; } = string.Empty;

        /// <summary>
        /// 是否显示 0显示 1不显示
        /// </summary>
        [SQLite.Column("isshow")]
        public int IsShow { get; set; } = 0;

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is PreLoadData r) {
                if (r.ModName.Equals(ModName) &&
                    r.ClassName.Equals(ClassName) &&
                    r.MethodName.Equals(MethodName) &&
                    r.English.Equals(English)) return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public enum IsShow
    {
        显示 = 0,
        不显示 = 1,
        废弃 = 3,
    }
}
