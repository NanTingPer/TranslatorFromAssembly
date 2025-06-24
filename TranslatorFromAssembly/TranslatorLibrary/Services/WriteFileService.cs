using System.Text;
using TranslatorFormAssembly.Models;
using TranslatorLibrary.Tools;
using TranslatorFromAssembly.Services.IServices;
using TranslatorFromAssembly.Models;
using System.Text.Json;

namespace TranslatorFromAssembly.Services.Services
{
    public class WriteFileService : IWriteFileService
    {
        /// <summary>
        /// 存储哪些方法 类
        /// </summary>
        private static List<StaticHookClass> staticHookList = [];

        /// <summary>
        /// 类名
        /// </summary>
        private string ClassNaem = string.Empty;

        /// <summary>
        /// 你自己的模组名称
        /// </summary>
        private string MyModName = string.Empty;

        /// <summary>
        /// 要被汉化的模组的名称
        /// </summary>
        private string ModName = string.Empty;

        /// <summary>
        /// 当前方法的名称
        /// </summary>
        private string MethodName = string.Empty;

        /// <summary>
        /// 给定模组根目录还有模组名称 开始生成
        /// </summary>
        /// <param name="modRootPath"> 模组根目录 </param>
        /// <param name="tarModName"> 要被汉化的模组名称 </param>
        /// <param name="YouModName">你的模组名称</param>
        public void WriteFile(string modRootPath, string tarModName, string MyModName)
        {
            string filePath = Path.Combine(modRootPath, "IL_" + tarModName + ".hjson");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);                                    //创建目标文件夹(汉化文件存储的主目录)

            if (PublicProperty.WriteMap.Count == 0)             //如果没有内容就返回了
                return;

            var map = PublicProperty.WriteMap;
            var list = new List<LocalizerModel>();
            foreach (var type in map) {
                var typeName = type.Key;
                foreach (var method in type.Value) {
                    var methodName = method.Key;
                    foreach (var localizer in method.Value) {
                        list.Add(new LocalizerModel()
                        {
                            MethodName = methodName,
                            OldValue = localizer.英文,
                            NewValue = localizer.中文,
                            TypeName = typeName
                        });
                    }
                }
            }
            string jsonString = JsonSerializer.Serialize(list);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            string str = Encoding.UTF8.GetString(bytes);
            using var sw = File.CreateText(str);
            sw.Write(jsonString);
            sw.Flush();
            sw.Close();

            PublicProperty.WriteMap.Clear();
        }


/// <summary>
/// 构造 WriteMap 需要传入一个dataBase的名称
/// </summary>
/// <param name="dataBaseName"></param>
public async Task CreateWriteMap(ISQLiteExtract<PreLoadData> sQLiteExtract, string dataBaseName)
        {
            await sQLiteExtract.CreateDatabaseAsync(dataBaseName);
            await HjsonSerializer.HjsonToSQLite(dataBaseName, sQLiteExtract);
            PreLoadData[] data = await sQLiteExtract.GetDataAsync(0, 0, save: PublicProperty.SaveMode.Write);

            var e = PublicProperty.WriteMap;

            foreach (var item in data) {
                if (e.ContainsKey(item.ClassName)) {

                    //获取值 存在是一定有值的
                    if (e.TryGetValue(item.ClassName, out var value)) {

                        //判断是否存在指定方法
                        if (value.ContainsKey(item.MethodName)) {
                            if (value.TryGetValue(item.MethodName, out List<英汉台港>? 内容)) {
                                内容.Add(CreateYHTG(item))/*Tuple.Create(item.English, item.Chinese)*/;
                            }
                        }
                        //不存在指定方法 那就创建 同时赋值
                        else {
                            var method = item.MethodName;
                            var tuple2 = CreateYHTG(item);
                            List<英汉台港> list = [];
                            list.Add(tuple2);
                            value.Add(method, list);
                        }
                    }
                }
                //不存在指定类
                else {
                    var calssname = item.ClassName;
                    var methodname = item.MethodName;
                    var 内容 = CreateYHTG(item);
                    List<英汉台港> list = [];
                    list.Add(内容);

                    Dictionary<string, List<英汉台港>> map = [];
                    map.Add(methodname, list);

                    e.Add(calssname, map);

                }
            }
        }

        private static 英汉台港 CreateYHTG(PreLoadData item)
        {
            return new 英汉台港(转义(item.English), 转义(item.Chinese), 转义(item.TaiWan), 转义(item.HongKong), 转义(item.PCR), 转义(item.CSOW),item.Id);
        }
        private static string 转义(string str) => str.Replace("\n", @"\n");
        private static byte[] StringToByte(string str)
        {
            string str2 = str + "\r\n";
            return Encoding.UTF8.GetBytes(str2);/*Convert.FromBase64String(str2);*/
        }
    }
}
