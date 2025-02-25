using SQLite;
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
        private SQLiteAsyncConnection? _connection;

        private string _AllPath = GetAppFilePath.GetPathAndCreate();

        public SQLiteExtract()
        {

        }

        public SQLiteAsyncConnection Connection
        {
            get => _connection!;
            set => _connection = value;
        }

        /// <summary>
        /// 用来添加条目 需要先调用CreateDatabase
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public async Task AddDataAsync(IList<PreLoadData> datas)
        {
            if (ConnectionIsNULL()) return;
            foreach (var item in datas) {
                PreLoadData? whereItem = await Connection.Table<PreLoadData>().Where(f =>
                f.ModName == item.ModName &&
                f.ClassName == item.ClassName &&
                f.MethodName == item.MethodName &&
                f.English == item.English).FirstOrDefaultAsync();

                //降序排序取最大值
                if (whereItem is null) {
                    long? maxid = Connection.Table<PreLoadData>().OrderByDescending(x => x.Id).FirstOrDefaultAsync()?.Id;
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
        /// <para>isShow 为 true 只看隐藏</para>
        /// </summary>
        /// <param name="skip">跳过 </param>
        /// <param name="take">取出数量</param>
        /// <returns></returns>
        public async Task<PreLoadData[]> GetDataAsync(int skip, int take, string className = "", string methodName = "", string counte = "", SaveMode save = SaveMode.None, bool isShow = false)
        {
            if (save == SaveMode.ReallAll)
                return await Connection.Table<PreLoadData>().Where(f => f.IsShow != (int)IsShow.废弃).ToArrayAsync();

            PreLoadData[] pe = [];
            if (ConnectionIsNULL())
                return pe;

            bool cn = false;
            bool mn = false;
            bool en = false;
            if (string.IsNullOrWhiteSpace(className)) cn = true;
            if (string.IsNullOrWhiteSpace(methodName)) mn = true;
            if (string.IsNullOrWhiteSpace(counte)) en = true;


            if (save == SaveMode.Write) {
                return await Connection.Table<PreLoadData>()
                .Where(f => cn || f.ClassName.Contains(className))
                .Where(f => mn || f.MethodName.Contains(methodName)/* == methodName*/)
                .Where(f => en || f.English.Contains(counte)/*f.English == counte*/)
                .Where(f => !string.IsNullOrEmpty(f.Chinese))
                .ToArrayAsync();
            }

            if (save == SaveMode.All) {
                return await Connection.Table<PreLoadData>()
                    .Where(f => !string.IsNullOrEmpty(f.Chinese))
                    .ToArrayAsync();
            }


            if (isShow) {
                return await Connection.Table<PreLoadData>()
                    .Where(f => f.IsShow == 1)
                    .Where(f => cn || f.ClassName.Contains(className))
                    .Where(f => mn || f.MethodName.Contains(methodName)/* == methodName*/)
                    .Where(f => en || f.English.Contains(counte)/*f.English == counte*/)
                    .Skip(skip)
                    .Take(take)
                    .ToArrayAsync();
            }

            return await Connection.Table<PreLoadData>()
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
        public Task<int> PageCountAsync()
        {

            if (ConnectionIsNULL())
                return Task.FromResult(0);

            return Connection.Table<PreLoadData>().CountAsync();
        }

        /// <summary>
        /// 初始化Connection 这是必须的
        /// </summary>
        /// <param name="dataBase"></param>
        public async Task CreateDatabaseAsync(string dataBase)
        {
            Connection = new SQLiteAsyncConnection(Path.Combine(_AllPath, dataBase));
            await Connection.CreateTableAsync<PreLoadData>();
        }


        /// <summary>
        /// 判断Connection是否为空
        /// </summary>
        /// <returns></returns>
        private bool ConnectionIsNULL()
        {
            if (Connection is null) return true;
            return false;
        }

        /// <summary>
        /// 修改数据库内容
        /// <para>需要先调用CreateDatabase</para>
        /// <para>IsShowNo 不显示</para>
        /// <para>IsShowYes 显示</para>
        /// <para>Chinese 中文发生改变</para>
        /// </summary>
        public async Task AlterAsync(SaveMode mode, params PreLoadData[] preLoadData)
        {
            if (ConnectionIsNULL())
                return;
            if (mode == SaveMode.Chinese) {
                foreach (var item in preLoadData) {
                    var data = await Connection.Table<PreLoadData>().Where(f => f.Id == item.Id).FirstOrDefaultAsync();
                    data.Chinese = item.Chinese;
                    await Connection.UpdateAsync(data);
                }
            }
            if (mode == SaveMode.IsShowNo || mode == SaveMode.IsShowYes) {
                foreach (var item in preLoadData) {
                    var data = await Connection.Table<PreLoadData>().Where(f => f.Id == item.Id).FirstOrDefaultAsync();

                    data.IsShow = data.IsShow == 1 ? 0 : 1;
                    await Connection.UpdateAsync(data);
                }
            }
        }

        public Task ColseDatabaseAsync()
        {
            throw new NotImplementedException();
        }

        #region 后加

        /// <summary>
        /// 传入数据库连接，传入一个spd将spd更新入数据库
        /// </summary>
        public static async Task UpdateData(ISQLiteExtract<PreLoadData> sqlConnect, IEnumerable<SimplePreLoadData> spds) /*where T : new()*/
        {
            if(sqlConnect is SQLiteExtract sql) {
                if (!spds.Any())
                    return;
                
                var sqlCon = sql.Connection.Table<PreLoadData>();
                spds = spds.Order(new SimplePreLoadData());
                long? oneId = spds.FirstOrDefault()?.Id;
                if (oneId is null)
                    return;

                var oneData = await sqlCon.FirstOrDefaultAsync(f => f.Id == oneId);
                if (oneData is null)
                    return;

                int isShow = oneData.IsShow;
                var 判断对象 = new 判断对象 { IsShow = isShow, upId = -1L, currId = -1L };
                var 最大ID = await sqlCon.CountAsync();
                foreach (var spd in spds) {
                    PreLoadData pld = await sqlCon.FirstOrDefaultAsync(f => f.Id == spd.Id);
                    判断对象.upId = 判断对象.currId;
                    判断对象.currId = pld.Id;
                    if (pld is not null) {
                        if (pld.ClassName == spd.ClassName) {
                            pld.English = spd.English;
                            pld.Chinese = spd.Chinese;
                            await sql.Connection.UpdateAsync(pld);//更新
                        }
                        if (/*判断对象.upId == -1L || */判断对象.currId == -1L)
                            continue;

                        //跨度更改，将跨过的内容修改为废弃，因为Hjson文件已经不存在此记录
                        var updataDatas = await sqlCon.Where(pld => pld.Id > 判断对象.upId && pld.Id < 判断对象.currId && pld.IsShow == 判断对象.IsShow).ToListAsync();
                        if (updataDatas.Count == 0)
                            continue; //没有内容
                        updataDatas.ForEach(async pld => { pld.IsShow = (int)IsShow.废弃; await sql.Connection.UpdateAsync(pld); });
                    }
                }
            }
        }

        private class 判断对象()
        {
            public int IsShow;
            public long upId;
            public long currId;
        }
        #endregion

        
    }
}
