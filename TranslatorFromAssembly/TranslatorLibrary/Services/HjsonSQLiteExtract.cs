using SQLite;
using TranslatorFormAssembly.Models;
using TranslatorLibrary.Tools;
using TranslatorFromAssembly.Services.IServices;

namespace TranslatorFromAssembly.Services.Services
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
            if (connection == null)
                return;

            foreach (var item in values) {
                long id = 0;
                var newValue = await connection.Table<HjsonModel>()
                    .FirstOrDefaultAsync(f => f.Key == item.Key && f.Value != item.Value);

                var oldVlaue = await connection.Table<HjsonModel>()
                    .FirstOrDefaultAsync(f => f.Key == item.Key && f.Value == item.Value);

                //等等null就说明数据第一次进来
                if (oldVlaue == null) {
                    var data = await connection.Table<HjsonModel>().OrderByDescending(f => f.Id).FirstOrDefaultAsync();
                    id = 0;
                    if (data is not null) {
                        id = data.Id + 1;
                    }
                    item.Id = id;
                    item.EditTime = HjsonEditValue.DataOneTo;
                    await connection.InsertAsync(item);
                    continue;
                }

                //不等于空就是这是新的数据
                if (newValue is not null) {
                    var olddata = await connection.Table<HjsonModel>().FirstOrDefaultAsync(f => f.Key == item.Key);
                    olddata.OldValue = olddata.Value;
                    olddata.Value = item.Value;
                    olddata.EditTime = HjsonEditValue.NoDataOneTo;
                    await connection.UpdateAsync(olddata);
                }
            }
            await connection.CloseAsync();
        }

        public async Task AlterAsync(PublicProperty.SaveMode mode, params HjsonModel[] preLoadData)
        {
            if (mode == PublicProperty.SaveMode.Chinese) {
                foreach (HjsonModel item in preLoadData) {
                    try {
                        HjsonModel tempData = await connection.GetAsync<HjsonModel>(item.Id);
                        tempData.OldChinese = tempData.Chinese;
                        tempData.Chinese = item.Chinese;
                        await connection.UpdateAsync(tempData);
                    } catch (Exception e) {
                        continue;
                    }
                }
            }
        }

        public async Task ColseDatabaseAsync()
        {
            if (connection != null) {
                await connection.CloseAsync();
            }
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
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
            connection = new SQLiteAsyncConnection(Path.Combine(newPath, dataBase));
            await connection.CreateTableAsync<HjsonModel>();
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }

        public Task<HjsonModel[]> GetDataAsync(int spik, int taks, string className = "", string methodName = "", string count = "", PublicProperty.SaveMode save = PublicProperty.SaveMode.None, bool isShow = false)
        {
            return connection.Table<HjsonModel>().ToArrayAsync();
        }

        public Task<int> PageCountAsync()
        {
            throw new NotImplementedException();
        }
    }
}
