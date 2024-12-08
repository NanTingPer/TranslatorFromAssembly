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
    /// 对提取出来的数据 进行数据库化管理
    /// </summary>
    public class SQLiteExtract : ISQLiteExtract<PreLoadData>
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
        public async Task AddData(IList<PreLoadData> datas)
        {
            foreach (var item in datas)
            {
                PreLoadData? whereItem = await Connection.Table<PreLoadData>().Where(f => f.MethodName == item.MethodName).FirstOrDefaultAsync();

                //降序排序取最大值
                long? maxid = Connection.Table<PreLoadData>().OrderByDescending(x => x.Id).FirstOrDefaultAsync()?.Id;
                if (whereItem is null)
                {
                    item.Id = maxid is null ? 0 : maxid.Value + 1;
                    await Connection.InsertAsync(item);
                }
            }
        }

        /// <summary>
        /// 修改数据库内容
        /// </summary>
        public Task Alter()
        {
            throw new NotImplementedException();
        }

        public void CreateDatabase(string dataBase)
        {
            Connection = new SQLiteAsyncConnection(Path.Combine(_AllPath, dataBase));
            Connection.CreateTableAsync<PreLoadData>();
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 用来获取数据库中的内容
        /// </summary>
        /// <param name="skip">跳过 </param>
        /// <param name="take">取出数量</param>
        /// <returns></returns>
        public Task<PreLoadData[]> GetData(int skip,int take)
        {
            return Connection.Table<PreLoadData>().Skip(skip).Take(take).ToArrayAsync();
        }
    }
}
