using Hjson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;

namespace TranslatorLibrary.AllServices.Services
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
            JsonValue jsonValue = Hjson.HjsonValue.Load(path);
            HjsonObjectKV(jsonValue, KeyValueList);
            await _sqLiteExtract.CreateDatabaseAsync(Path.GetFileName(path));
            await _sqLiteExtract.AddDataAsync(KeyValueList);
        }

        public Task SaveHjsonAsync(string path)
        {
            throw new NotImplementedException();
        }

        private void HjsonObjectKV(JsonValue jsonValue,List<HjsonModel> list,string key="")
        {
            if(jsonValue is JsonObject)
            {
                foreach(KeyValuePair<string,JsonValue> item in jsonValue)
                {
                    HjsonObjectKV(item.Value, list, key + "." + item.Key);
                }
            }
            else
            {
                list.Add(new HjsonModel() { Key = key.Substring(1), Value = jsonValue.ToValue().ToString() });
            }
            
        }
    }
}
