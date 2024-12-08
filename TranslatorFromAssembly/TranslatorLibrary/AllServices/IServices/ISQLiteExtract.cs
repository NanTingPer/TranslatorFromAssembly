using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.AllServices.IServices
{
    public interface ISQLiteExtract
    {
        void Delete();

        void Alter();

        void GetData();

        void AddData();

        void CreateDatabase(string dataBase);
    }
}
