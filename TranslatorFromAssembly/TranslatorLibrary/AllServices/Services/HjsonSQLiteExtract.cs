using SQLite;
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
    /// <summary>
    /// 用来获取和修改Hjson的数据库
    /// </summary>
    public class HjsonSQLiteExtract : ISQLiteExtract<HjsonModel>
    {
        private SQLiteAsyncConnection connection;
        private string DataPath = Path.Combine(GetAppFilePath.GetPathAndCreate(), "Hjson");
        public async Task AddDataAsync(IList<HjsonModel> values)
        {
            foreach (var item in values)
            {
                long id = 0;
                var newVlaue = connection.Table<HjsonModel>()
                    .Where(f => f.Key == item.Key && f.Value != item.Value);

                var oldVlaue = connection.Table<HjsonModel>()
                    .Where(f => f.Key == item.Key && f.Value == item.Value);

                //等等null就说明数据第一次进来
                if (oldVlaue == null)
                {
                    id = (await connection.Table<HjsonModel>().OrderByDescending(f => f.Id).FirstOrDefaultAsync()).Id;
                    item.Id = id;
                    await connection.InsertAsync(item);
                    continue;
                }

                //不等于空就是这是新的数据
                if(newVlaue != null)
                {
                    var olddata = await connection.Table<HjsonModel>().FirstOrDefaultAsync(f => f.Key == item.Key);
                    olddata.OldValue = olddata.Value;
                    olddata.Value = item.Value;
                    await connection.UpdateAsync(olddata);
                }

            }
            await connection.CloseAsync();
        }

        public Task AlterAsync(PublicProperty.SaveMode mode, params HjsonModel[] preLoadData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public async Task CreateDatabaseAsync(string dataBase)
        {
            string modName = dataBase.Split(".")[1];
            string newPath = Path.Combine(DataPath, modName);
            if(!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
            connection = new SQLiteAsyncConnection(Path.Combine(newPath, dataBase));
            await connection.CreateTableAsync<HjsonModel>();
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }

        public Task<HjsonModel[]> GetDataAsync(int spik, int taks, string className = "", string methodName = "", string count = "", PublicProperty.SaveMode save = PublicProperty.SaveMode.None, bool isShow = false)
        {
            throw new NotImplementedException();
        }

        public Task<int> PageCountAsync()
        {
            throw new NotImplementedException();
        }
    }
}
