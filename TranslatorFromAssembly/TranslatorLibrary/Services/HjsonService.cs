using Hjson;
using TranslatorFormAssembly.Models;
using TranslatorFromAssembly.Services.IServices;

namespace TranslatorFromAssembly.Services.Services
{
    /// <summary>
    /// 用来处理Hjson的
    /// </summary>
    public class HjsonService : IHjsonProcess
    {
        private ISQLiteExtract<HjsonModel> _sqLiteExtract;
        public HjsonService(ISQLiteExtract<HjsonModel> sQLiteExtract)
        {
            _sqLiteExtract = sQLiteExtract;
        }

        /// <summary>
        /// 加载数据并存入数据库
        /// </summary>
        /// <param name="path"> 全路径 </param>
        /// <returns></returns>
        public async Task LoadHjsonAsync(string path)
        {
            List<HjsonModel> KeyValueList = new();
            JsonValue jsonValue = HjsonValue.Load(path);
            HjsonObjectKV(jsonValue, KeyValueList);
            await _sqLiteExtract.CreateDatabaseAsync(Path.GetFileName(path));
            await _sqLiteExtract.AddDataAsync(KeyValueList);
            await _sqLiteExtract.ColseDatabaseAsync();
        }

        /// <summary>
        /// 给定一个数据库链接
        /// <para> 将此链接的数据全部导出为Hjson </para>
        /// </summary>
        public async Task SaveHjsonAsync(string path, ISQLiteExtract<HjsonModel> sQLiteExtract)
        {
            var data = await sQLiteExtract.GetDataAsync(0, 0);
            JsonObject hjson = new JsonObject();
            await Task.Run(() => {
                foreach (var item in data) {
                    //为空返true
                    hjson.Add(item.Key, string.IsNullOrWhiteSpace(item.Chinese) ? item.Value : item.Chinese);
                }
            });

            await Task.Run(() => {
                using (FileStream stream = new FileStream(path, FileMode.Create)) {
                    hjson.Save(stream, Stringify.Hjson);
                }

            });
        }

        private void HjsonObjectKV(JsonValue jsonValue, List<HjsonModel> list, string key = "")
        {
            if (jsonValue is JsonObject) {
                foreach (KeyValuePair<string, JsonValue> item in jsonValue) {
                    HjsonObjectKV(item.Value, list, key + "." + item.Key);
                }
            } else {
                list.Add(new HjsonModel() { Key = key.Substring(1), Value = jsonValue.ToValue().ToString()! });
            }

        }
    }
}
