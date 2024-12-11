using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllServices.IServices
{
    public interface ISQLiteExtract<T>
    {
        Task Delete();

        Task Alter(PublicProperty.SaveMode mode , params PreLoadData[] preLoadData);

        Task<T[]> GetData(int spik, int taks,string className="",string methodName="",string count="",PublicProperty.SaveMode save = PublicProperty.SaveMode.None);

        Task<int> PageCount();

        Task AddData(IList<T> values);

        void CreateDatabase(string dataBase);
    }
}
