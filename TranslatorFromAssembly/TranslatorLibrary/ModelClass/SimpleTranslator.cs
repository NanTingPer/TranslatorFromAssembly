using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.ModelClass
{
    public class SimpleTranslator
    {
        private SimpleTranslator() { }
        public long Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;


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
