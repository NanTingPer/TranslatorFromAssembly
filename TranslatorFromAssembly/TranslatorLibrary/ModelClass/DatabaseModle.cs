using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.ModelClass
{
    [SQLite.Table("stardict")]
    public class DatabaseModle
    {

        /// <summary>
        /// 唯一ID
        /// </summary>
        [SQLite.Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 英文单词
        /// </summary>
        [SQLite.Column("word")]
        public string Word { get; set; } = string.Empty;

        /// <summary>
        /// 分词
        /// </summary>
        [SQLite.Column("sw")]
        public string sw { get; set; } = string.Empty;


        [SQLite.Column("phonetic")]
        public string Phonetic { get; set; } = string.Empty;

        [SQLite.Column("definition")]
        public string Definition { get; set; } = string.Empty;

        /// <summary>
        /// 汉化
        /// </summary>
        [SQLite.Column("translation")]
        public string Translation { get; set; } = string.Empty;

        [SQLite.Column("pos")]
        public string Pos { get; set; } = string.Empty;

        [SQLite.Column("collins")]
        public string Collins { get; set; } = string.Empty;

        [SQLite.Column("oxford")]
        public string Oxford { get; set; } = string.Empty;


        [SQLite.Column("tag")]
        public string Tag { get; set; } = string.Empty;

        [SQLite.Column("bnc")]
        public string Bnc { get; set; } = string.Empty;

        [SQLite.Column("frq")]
        public string Frq { get; set; } = string.Empty;

        [SQLite.Column("exchange")]
        public string Exchange { get; set; } = string.Empty;

        [SQLite.Column("detail")]
        public string Detail { get; set; } = string.Empty;

        [SQLite.Column("audio")]
        public string Audio { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            var data = (DatabaseModle)obj;
            if(data is null) return false;
            if (this.Id.Equals(data.Id)) return true;
            return false;
        }
    }

}
