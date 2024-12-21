using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllServices.Services
{
    public class WriteFileService : IWriteFileService
    {
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
        /// <param name="ModRootPath"> 模组根目录 </param>
        /// <param name="ModName"> 要被汉化的模组名称 </param>
        /// <param name="YouModName">你的模组名称</param>
        public void WriteFile(string ModRootPath,string ModName,string MyModName)
        {
            this.ModName = ModName;
            this.MyModName = MyModName;

            //目标根目录 使用目标模组名称 + Translator
            string rootPath = ModName + "Translator";

            //目标文件路径 自己模组所在目录 + 目标根目录
            string filePath = Path.Combine(ModRootPath, rootPath);
            Directory.CreateDirectory(filePath);

            if (PublicProperty.WriteMap.Count == 0)
                return;

            Stream? dll = typeof(WriteFileService).Assembly.GetManifestResourceStream("ForceLocalizeSystem");
            if(dll is not null)
            {
                string tempDric = Path.Combine(ModRootPath, "Systems");
                //文件夹不存在就创建
                if(!Directory.Exists(tempDric)) Directory.CreateDirectory(tempDric);
                string tempFile  = Path.Combine(ModRootPath, "Systems", "ForceLocalizeSystem.cs");
                //资源没有转移就转移
                if (!File.Exists(tempFile))
                {
                    Stream newStream = new FileStream(tempFile, FileMode.OpenOrCreate);
                    dll.CopyTo(newStream);
                    newStream.Close();
                }
            }
            dll?.Close();

            int fileTail = 0;
            var map = PublicProperty.WriteMap;

            //TODO 第一层循环 类名 方法名称 内容
            foreach (var item in map)
            {
                ClassNaem = item.Key;
                var Temp = ClassNaem.Split('.');

                //文件名称的开头 现在是类名
                var FileName = Temp[Temp.Length - 1];
                FileName = FileName.Replace("<", "x").Replace(">", "d").Replace(".","j").Replace("/","g");
                

                //使用类名 + 数字来命名
                string fileSystemPath = Path.Combine(filePath, FileName + fileTail+".cs");

                if(File.Exists(fileSystemPath)) File.Delete(fileSystemPath);


                //写
                FileStream Write = File.Create(fileSystemPath);

                WriteTo(Write, MyModName, rootPath, FileName, fileTail, ModName);

                //方法名称 内容
                var Translat = item.Value;
                foreach (var value in Translat)
                {
                    MethodName = value.Key;
                    var 内容 = value.Value;
                    //填入全类名 还有方法名
                    Write.Write(StringToByte($"\t\t\t\tTranslatorLoad.LocalizeByTypeFullName(\"{ClassNaem.Replace("/","+")}\", \"{value.Key}\", new ()"));
                    Write.Write(StringToByte("\t\t\t\t{"));
                    Write.Flush();
                    foreach (Tuple<String, String> tpule in 内容)
                    {
                        Write.Write(StringToByte("\t\t\t\t\t{" + "\"" + tpule.Item1.Replace("\n","\\n") + "\"" + "," + "\"" + tpule.Item2.Replace("\n","\\n") + "\"" + "},"));
                        Write.Flush();
                    }
                    Write.Write(StringToByte("\t\t\t\t});"));
                }


                Write.Write(StringToByte("\t\t\t}"));
                Write.Write(StringToByte("\t\t\tbase.Load();"));
                Write.Write(StringToByte("\t\t}"));
                Write.Write(StringToByte("\t}"));
                Write.Write(StringToByte("}"));
                Write.Flush();
                Write.Dispose();
                Write.Close();
            }
            fileTail++;
            PublicProperty.WriteMap.Clear();
        }

        /// <summary>
        /// 构造 WriteMap 需要传入一个dataBase的名称
        /// </summary>
        /// <param name="dataBaseName"></param>
        public async Task CreateWriteMap(ISQLiteExtract<PreLoadData> sQLiteExtract,string dataBaseName)
        {
            await sQLiteExtract.CreateDatabaseAsync(dataBaseName);
            PreLoadData[] data = await sQLiteExtract.GetDataAsync(0, 0, save : PublicProperty.SaveMode.Write);

            var e = PublicProperty.WriteMap;

            foreach (var item in data)
            {
                
                if (e.ContainsKey(item.ClassName))
                {

                    //获取值 存在是一定有值的
                    if(e.TryGetValue(item.ClassName,out var value))
                    {

                        //判断是否存在指定方法
                        if (value.ContainsKey(item.MethodName))
                        {
                            if(value.TryGetValue(item.MethodName,out var 内容))
                            {
                                内容.Add(Tuple.Create(item.English, item.Chinese));
                            }
                        } 
                        //不存在指定方法 那就创建 同时赋值
                        else
                        {
                            var method = item.MethodName;
                            var tuple2 = Tuple.Create(item.English, item.Chinese);
                            var list = new List<Tuple<string,string>>();
                            value.Add(method, list);
                        }
                    }
                }
                //不存在指定类
                else
                {
                    var calssname = item.ClassName;
                    var methodname = item.MethodName;
                    var 内容 = Tuple.Create(item.English, item.Chinese);
                    var list = new List<Tuple<string, string>>();
                    list.Add(内容);

                    var map = new Dictionary<string,List<Tuple<string,string>>>();
                    map.Add(methodname, list);

                    e.Add(calssname, map);

                }
            }
        }

        private static byte[] StringToByte(String str)
        {
            String str2 = str + "\r\n";
            return Encoding.UTF8.GetBytes(str2);/*Convert.FromBase64String(str2);*/
        }

        /// <summary>
        /// 公共内容
        /// </summary>
        /// <param name="Write"> 文件流 </param>
        /// <param name="MyModName"> 你的模组名称 </param>
        /// <param name="rootPath"> 目标根目录 </param>
        /// <param name="FileName"> 文件名的开头 </param>
        /// <param name="fileTail"> 文件名称的结尾</param>
        /// <param name="ModName"> 目标模组名称 </param>
        private static void WriteTo(FileStream Write,string MyModName,string rootPath,string FileName,int fileTail,string ModName)
        {
            Write.Write(StringToByte($"using {MyModName}.Systems;"));
            Write.Write(StringToByte("using System.Collections.Generic;"));
            Write.Write(StringToByte("using Terraria.ModLoader;"));
            //名称空间
            Write.Write(StringToByte($"namespace {MyModName}.{rootPath}"));
            Write.Write(StringToByte("{"));
            //使用要被汉化的模组的名称+数字来定义类型名
            Write.Write(StringToByte($"\tinternal class {FileName}{fileTail} : ModSystem"));
            Write.Write(StringToByte("\t{"));
            //要被汉化的模组的名称
            Write.Write(StringToByte($"\t\tprivate class {ModName}" + "{}"));

            //延迟加载
            Write.Write(StringToByte($"\t\t[ExtendsFromMod(\"{ModName}\"), JITWhenModsEnabled(\"{ModName}\")]"));

            Write.Write(StringToByte($"\t\tprivate class TranslatorLoad : ForceLocalizeSystem<{ModName}, TranslatorLoad>" + "{}"));
            Write.Write(StringToByte("\t\tpublic override void Load()"));
            Write.Write(StringToByte("\t\t{"));

            //获取模组判断同时能判断是不是存在
            Write.Write(StringToByte($"\t\t\tif(ModLoader.TryGetMod(\"{ModName}\",out var mod))"));
            Write.Write(StringToByte("\t\t\t{"));

        }
    }
}
