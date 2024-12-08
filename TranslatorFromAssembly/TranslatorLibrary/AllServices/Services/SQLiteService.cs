using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using TranslatorLibrary.Tools;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.AllServices.IServices;

namespace TranslatorLibrary.AllServices.Services.SQLiteServices
{
    public class SQLiteService : ISQLiteService
    {
        private bool _exesitDatabase { get => File.Exists(_databasePath); }
        public bool ExesitDatabase { get => _exesitDatabase; }
        private string _databaseName;
        private string _databasePath;
        private SQLiteAsyncConnection _connection;
        public SQLiteAsyncConnection ConnectionAsync { get => _connection ?? new SQLiteAsyncConnection(_databasePath); }

        public SQLiteService()
        {
            _databaseName = "stardict.db";
            _databasePath = GetAppFilePath.GetPathAndCreate(_databaseName, 2);
        }

        
        
        public Task CreateDatabase()
        {
            throw new NotImplementedException();
        }

        public Task DeleteData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回一组数据
        /// </summary>
        /// <param name="word"> 英文内容 </param>
        /// <returns> 可枚举类型 自行枚举 </returns>
        public async Task<IEnumerable<DatabaseModle>> GetData(string word)
        {
            if (_exesitDatabase == true)
            {
                string words = word.Trim().ToLower();
                string[] strs = words.Split(" ");
                List<DatabaseModle> ie = new List<DatabaseModle>();
                foreach (string work in strs)
                {
                    //var str = work.Trim();
                    ie.Add(await ConnectionAsync.Table<DatabaseModle>().FirstOrDefaultAsync(f => f.Word.Equals(work)));
                }
                return ie;
            }
            return null;
        }

        /// <summary>
        /// 数据库迁移
        /// 会报找不到的错误
        /// </summary>
        /// <returns></returns>
        public async Task Initialization()
        {
            if (_exesitDatabase == false)
            {
                try
                {
                    //Assembly assembly = Assembly.GetExecutingAssembly();
                    Assembly assembly = typeof(SQLiteService).Assembly;
                    //string resourceName = assembly.GetManifestResourceNames().FirstOrDefault(f => f.Contains("stardict"));

                    using Stream database = assembly.GetManifestResourceStream(_databaseName);
                    using Stream fromStram = new FileStream(GetAppFilePath.GetPathAndCreate(_databaseName), FileMode.Open);
                    await database.CopyToAsync(fromStram);
                }
                catch
                {
                    
                }
            }
        }

        public Task InsertData()
        {
            throw new NotImplementedException();
        }
        
    }
}
