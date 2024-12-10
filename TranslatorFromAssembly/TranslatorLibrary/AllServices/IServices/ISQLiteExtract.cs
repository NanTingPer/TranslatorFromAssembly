using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.AllServices.IServices
{
    public interface ISQLiteExtract<T>
    {
        Task Delete();

        Task Alter();

        Task<T[]> GetData(int spik, int taks);

        Task<int> PageCount();

        Task AddData(IList<T> values);

        void CreateDatabase(string dataBase);
    }
}
