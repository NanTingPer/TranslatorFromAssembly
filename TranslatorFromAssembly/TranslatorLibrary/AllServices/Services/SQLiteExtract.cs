using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllServices.Services
{
    public class SQLiteExtract : ISQLiteExtract
    {
        private string _AllPath = GetAppFilePath.GetPathAndCreate();
        private SQLiteAsyncConnection _connection;
        
        public SQLiteAsyncConnection Connection 
        { 
            get => _connection; 
            set => _connection = value;
        }
        public SQLiteExtract() 
        {

        }
        public void AddData()
        {
            throw new NotImplementedException();
        }

        public void Alter()
        {
            throw new NotImplementedException();
        }

        public void CreateDatabase(string dataBase)
        {
            Connection = new SQLiteAsyncConnection(Path.Combine(_AllPath, dataBase));
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void GetData()
        {
            throw new NotImplementedException();
        }
    }
}
