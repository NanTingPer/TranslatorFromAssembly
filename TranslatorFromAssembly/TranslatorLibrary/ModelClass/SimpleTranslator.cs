using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.ModelClass
{

    /// <summary>
    /// 简单汉化类
    /// </summary>
    public class SimpleTranslator
    {
        private SimpleTranslator() { }
        public long Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;


        /// <summary>
        /// 使用复制汉化类创建简单汉化类
        /// <para>复杂汉化类为 DatabaseModle</para>
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        public static SimpleTranslator CreateSimpleTranslator(DatabaseModle database)
        {
            if (database is null)
                return null;
            return new SimpleTranslator() { Word = database.Word, Translation = database.Translation, Id = database.Id };
        }

        public override bool Equals(object? obj)
        {
            var data = (SimpleTranslator)obj;
            if (data is null)
                return false;
            if (this.Id.Equals(data.Id))
                return true;
            return false;
        }
    }
}
