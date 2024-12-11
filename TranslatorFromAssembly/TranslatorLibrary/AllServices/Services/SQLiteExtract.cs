using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;
using static TranslatorLibrary.Tools.PublicProperty;

namespace TranslatorLibrary.AllServices.Services
{
    /// <summary>
    /// 对提取出来的数据 进行数据库化管理
    /// </summary>
    public class SQLiteExtract : ISQLiteExtract<PreLoadData>
    {
        private SQLiteAsyncConnection _connection;

        private string _AllPath = GetAppFilePath.GetPathAndCreate();
        
        public SQLiteExtract() 
        {

        }

        public SQLiteAsyncConnection Connection 
        { 
            get => _connection; 
            set => _connection = value;
        }

        /// <summary>
        /// 用来添加条目 需要先调用CreateDatabase
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public async Task AddData(IList<PreLoadData> datas)
        {
            if(ConnectionIsNULL()) return;
            foreach (var item in datas)
            {
                PreLoadData? whereItem = await Connection.Table<PreLoadData>().Where(f =>
                f.ModName == item.ModName &&
                f.ClassName == item.ClassName &&
                f.MethodName == item.MethodName &&
                f.English == item.English).FirstOrDefaultAsync();

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
        /// 用来获取数据库中的内容
        /// <para>需要先调用CreateDatabase</para>
        /// <para>skip 跳过的条目数</para>
        /// <para>take 取出的条目数</para>
        /// </summary>
        /// <param name="skip">跳过 </param>
        /// <param name="take">取出数量</param>
        /// <returns></returns>
        public Task<PreLoadData[]> GetData(int skip,int take,string className="", string methodName="", string counte="",SaveMode save = SaveMode.None)
        {
            PreLoadData[] pe = [];
            if (ConnectionIsNULL())
                return Task.FromResult(pe);

            bool cn = false;
            bool mn = false;
            bool en = false;
            if (string.IsNullOrEmpty(className))cn = true;
            if (string.IsNullOrEmpty(methodName))mn = true;
            if (string.IsNullOrEmpty(counte))en = true;


            if (save == SaveMode.Write)
            {
                return Connection.Table<PreLoadData>()
                .Where(f => cn || f.ClassName.Contains(className))
                .Where(f => mn || f.MethodName.Contains(methodName)/* == methodName*/)
                .Where(f => en || f.English.Contains(counte)/*f.English == counte*/)
                .Where(f => !string.IsNullOrEmpty(f.Chinese))
                .ToArrayAsync();
            }


            return Connection.Table<PreLoadData>()
                .Where(f => f.IsShow == 0)
                .Where(f => cn || f.ClassName.Contains(className))
                .Where(f => mn || f.MethodName.Contains(methodName)/* == methodName*/)
                .Where(f => en || f.English.Contains(counte)/*f.English == counte*/)
                .Skip(skip)
                .Take(take)
                .ToArrayAsync();
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取数据条目数量
        /// <para>需要先调用CreateDatabase</para>
        /// </summary>
        /// <returns></returns>
        public Task<int> PageCount()
        {

            if(ConnectionIsNULL())
                return Task.FromResult(0);
            
            return Connection.Table<PreLoadData>().CountAsync();
        }

        /// <summary>
        /// 初始化Connection 这是必须的
        /// </summary>
        /// <param name="dataBase"></param>
        public void CreateDatabase(string dataBase)
        {
            Connection = new SQLiteAsyncConnection(Path.Combine(_AllPath, dataBase));
            Connection.CreateTableAsync<PreLoadData>();
        }


        /// <summary>
        /// 判断Connection是否为空
        /// </summary>
        /// <returns></returns>
        private bool ConnectionIsNULL()
        {
            if(Connection is null) return true;
            return false;
        }

        /// <summary>
        /// 修改数据库内容
        /// <para>需要先调用CreateDatabase</para>
        /// <para>IsShowNo 不显示</para>
        /// <para>IsShowYes 显示</para>
        /// <para>Chinese 中文发生改变</para>
        /// </summary>
        public async Task Alter(SaveMode mode, params PreLoadData[] preLoadData)
        {
            if (ConnectionIsNULL())
                return;
            if(mode == SaveMode.Chinese)
            {
                foreach (var item in preLoadData)
                {
                    var data = await Connection.Table<PreLoadData>().Where(f => f.Id == item.Id).FirstOrDefaultAsync();
                    data.Chinese = item.Chinese;
                    await Connection.UpdateAsync(data);
                }
            }
            if(mode == SaveMode.IsShowNo)
            {
                foreach (var item in preLoadData)
                {
                    var data = await Connection.Table<PreLoadData>().Where(f => f.Id == item.Id).FirstOrDefaultAsync();
                    data.IsShow = 1;
                    await Connection.UpdateAsync(data);
                }
            }

            if (mode == SaveMode.IsShowYes)
            {
                foreach (var item in preLoadData)
                {
                    var data = await Connection.Table<PreLoadData>().Where(f => f.Id == item.Id).FirstOrDefaultAsync();
                    data.IsShow = 0;
                    await Connection.UpdateAsync(data);
                }
            }

        }
    }
}
