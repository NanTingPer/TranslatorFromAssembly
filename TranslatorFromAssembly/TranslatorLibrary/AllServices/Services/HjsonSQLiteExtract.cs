using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllServices.Services
{
    public class HjsonSQLiteExtract : ISQLiteExtract<HjsonService>
    {
        public Task AddDataAsync(IList<HjsonService> values)
        {
            throw new NotImplementedException();
        }

        public Task AlterAsync(PublicProperty.SaveMode mode, params PreLoadData[] preLoadData)
        {
            throw new NotImplementedException();
        }

        public Task CreateDatabaseAsync(string dataBase)
        {
            throw new NotImplementedException();
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }

        public Task<HjsonService[]> GetDataAsync(int spik, int taks, string className = "", string methodName = "", string count = "", PublicProperty.SaveMode save = PublicProperty.SaveMode.None, bool isShow = false)
        {
            throw new NotImplementedException();
        }

        public Task<int> PageCountAsync()
        {
            throw new NotImplementedException();
        }
    }
}
