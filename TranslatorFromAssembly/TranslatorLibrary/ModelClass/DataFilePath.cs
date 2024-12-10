using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.ModelClass
{
    public class DataFilePath
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            DataFilePath e = null;
            try
            { 
                e = (DataFilePath)obj;
            }
            catch
            {
                return false;
            }
            if (e is null) return true;
            if(FilePath == e.FilePath && FileName == e.FileName) return true;
            return false;
        }
    }
}
