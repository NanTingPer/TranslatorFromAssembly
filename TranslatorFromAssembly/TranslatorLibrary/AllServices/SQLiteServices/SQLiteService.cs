using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.AllServices.SQLiteServices
{
    public class SQLiteService : ISQLiteService
    {
        private SQLiteConnection _connection;
        
        public Task CreateDatabase()
        {
            throw new NotImplementedException();
        }

        public Task DeleteData()
        {
            throw new NotImplementedException();
        }

        public Task GetData()
        {
            throw new NotImplementedException();
        }

        public Task Initialization()
        {
            throw new NotImplementedException();
        }

        public Task InsertData()
        {
            throw new NotImplementedException();
        }
    }
}
