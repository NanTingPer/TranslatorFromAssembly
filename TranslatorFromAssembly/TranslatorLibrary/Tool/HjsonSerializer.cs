using Hjson;
using System.Text.Json;
using TranslatorFormAssembly.Models;
using HJsonValue = Hjson.HjsonValue;
using JsonObject = System.Text.Json.Nodes.JsonObject;
using JsonValue = System.Text.Json.Nodes.JsonValue;
using TranslatorFromAssembly.Services.Services;
using TranslatorFromAssembly.Services.IServices;

namespace TranslatorLibrary.Tools
{
    public static class HjsonSerializer
    {
        #region Hjson
        /// <summary>
        /// 给定模组名称将Hjson更新载入SQLite
        /// </summary>
        public static async Task<bool> HjsonToSQLite(string modName, ISQLiteExtract<PreLoadData> _sQLiteExtract)
        {
            string iLHjsonDic = Path.Combine(GetAppFilePath.GetPathAndCreate(), "ILHjsoh");
            string ilHjsonShow = Path.Combine(iLHjsonDic, modName + "Show.hjson");
            string ilHjsonNoShow = Path.Combine(iLHjsonDic, modName + "NoShow.hjson");
            if (File.Exists(ilHjsonShow) && File.Exists(ilHjsonNoShow)) {//存在
                await 更新载入(ilHjsonShow, _sQLiteExtract);
                await 更新载入(ilHjsonNoShow, _sQLiteExtract);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 给定Hjson路径将数据更新载入SQLite
        /// </summary>
        private static async Task 更新载入(string path, ISQLiteExtract<PreLoadData> _sQLiteExtract)
        {
            List<SimplePreLoadData> PLDS = [];
            Hjson.JsonValue showJsonValue = HJsonValue.Parse(File.ReadAllText(path));
            foreach (var oneValue in showJsonValue) { //遍历
                var rrr = (KeyValuePair<string, Hjson.JsonValue>)oneValue;
                SimplePreLoadData spd = JsonSerializer.Deserialize<SimplePreLoadData>(rrr.Value.ToString())!;
                SPLDTaiWanOrHongKong(spd);
                PLDS.Add(spd);
            }
            await SQLiteExtract.UpdateData(_sQLiteExtract, PLDS);
        }

        public static void SaveToHjson(PreLoadData[] plds, string modName)
        {
            string iLHjsonDic = Path.Combine(GetAppFilePath.GetPathAndCreate(), "ILHjsoh");
            string ilHjsonShow = Path.Combine(iLHjsonDic, modName + "Show.hjson");
            string ilHjsonNoShow = Path.Combine(iLHjsonDic, modName + "NoShow.hjson");
            var noShow = plds.Where(pld => pld.IsShow == (int)IsShow.不显示);//不显示
            var Show = plds.Where(pld => pld.IsShow == (int)IsShow.显示);//显示
            给定数据保存到Hjson(noShow, ilHjsonNoShow);
            给定数据保存到Hjson(Show, ilHjsonShow);
        }

        /// <summary>
        /// 将给定内容，以Hjson的文件格式保存到path
        /// </summary>
        private static void 给定数据保存到Hjson(IEnumerable<PreLoadData> plds, string path)
        {
            JsonObject jsonObject = new JsonObject();
            foreach (var pld in plds) {
                jsonObject.Add(pld.Id.ToString(), JsonValue.Parse(JsonSerializer.Serialize(SimplePreLoadData.ToSimplePreLoadData(pld), typeof(SimplePreLoadData), new JsonSerializerOptions() { WriteIndented = false })));
            }
            var objectValue = jsonObject.ToString();
            //objectValue = Convert.ToString(objectValue);
            Hjson.JsonValue value = HjsonValue.Parse(jsonObject.ToString());
            var direct = Path.GetDirectoryName(path);
            if (direct is not null) {
                if (!Directory.Exists(direct)) {
                    Directory.CreateDirectory(direct);
                }
                value.Save(path, Stringify.Hjson);
            }
        }
        #endregion Hjson

        /// <summary>
        /// 判断spld对象的TaiWan与HongKong是否为空串 是的话就替换为普通中文
        /// </summary>
        private static void SPLDTaiWanOrHongKong(SimplePreLoadData spld)
        {
            if (string.IsNullOrEmpty(spld.TaiWan))
                spld.TaiWan = spld.Chinese;
            if (string.IsNullOrEmpty(spld.HongKong))
                spld.HongKong = spld.Chinese;
        }
    }
}
