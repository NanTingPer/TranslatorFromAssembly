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

        Task AlterAsync(PublicProperty.SaveMode mode , params PreLoadData[] preLoadData);

        Task<T[]> GetDataAsync(int spik, int taks,string className="",string methodName="",string count="",PublicProperty.SaveMode save = PublicProperty.SaveMode.None,bool isShow = false);

        Task<int> PageCountAsync();

        Task AddDataAsync(IList<T> values);

        Task CreateDatabaseAsync(string dataBase);
    }
}
