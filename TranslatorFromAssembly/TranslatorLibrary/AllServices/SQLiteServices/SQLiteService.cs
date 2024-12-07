using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllServices.SQLiteServices
{
    public class SQLiteService : ISQLiteService
    {
        private bool _exesitDatabase { get => File.Exists(_databasePath); }
        private string _databaseName;
        private string _databasePath;
        private SQLiteAsyncConnection _connection;
        public SQLiteService()
        {
            _databasePath = "stardict.db";
            _databasePath = GetAppFilePath.GetPathAndCreate(_databasePath, 2);
        }

        
        
        public SQLiteAsyncConnection ConnectionAsync { get => _connection ?? new SQLiteAsyncConnection(_databasePath); }
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

        /// <summary>
        /// 数据库迁移
        /// </summary>
        /// <returns></returns>
        public async Task Initialization()
        {
            if (_exesitDatabase == false)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string? resourceName = assembly.GetManifestResourceNames().FirstOrDefault(f => f.Contains("stardict.db"));
                using Stream database = assembly.GetManifestResourceStream(resourceName);
                using Stream fromStram = new FileStream(GetAppFilePath.GetPathAndCreate(_databaseName), FileMode.Open);
                await database.CopyToAsync(fromStram);
            }
        }

        public Task InsertData()
        {
            throw new NotImplementedException();
        }
        
    }
}
