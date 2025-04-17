using System.Text;
using TranslatorFormAssembly.Models;
using TranslatorLibrary.Tools;
using TranslatorFromAssembly.Services.IServices;

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
        /// <param name="ModRootPath"> 模组根目录 </param>
        /// <param name="ModName"> 要被汉化的模组名称 </param>
        /// <param name="YouModName">你的模组名称</param>
        public void WriteFile(string ModRootPath, string ModName, string MyModName)
        {
            this.ModName = ModName;
            this.MyModName = MyModName;
            string ModRootClassPath = Path.Combine(ModRootPath, $"{MyModName}.cs"); //模组主类(继承Mod)
            string rootPath = ModName + "Translator";                               //目标根目录 使用目标模组名称 + Translator
            string filePath = Path.Combine(ModRootPath, rootPath);                  //目标文件路径 自己模组所在目录 + 目标根目录
            Directory.CreateDirectory(filePath);                                    //创建目标文件夹(汉化文件存储的主目录)

            if (PublicProperty.WriteMap.Count == 0)             //如果没有内容就返回了
                return;

            #region 备份模组主类
            //获取模组根目录下的全部
            string[] files = Directory.GetFiles(ModRootPath);                                   //获取跟目录下的全部文件
            string? tarGetFile = files.FirstOrDefault(f => f.Contains(MyModName + ".cs"));//寻找模组主类
            //备份原始 模组.cs文件
            if (tarGetFile != null) {
                int a = 0;
                string bakFile = tarGetFile + ".bak";
                string newBakFile = string.Empty;

                do {
                    newBakFile = bakFile + a;
                    a++;
                } while (File.Exists(newBakFile));

                if (!File.Exists(newBakFile)) {
                    File.Create(newBakFile).Dispose();
                }
                //将tarGetFile替换给bakFile 备份为他自己
                //File.Replace(tarGetFile, bakFile, tarGetFile,false);
                File.Copy(tarGetFile, newBakFile, true);
            }
            #endregion 备份模组主类

            #region 资源转移
            Stream? dll = typeof(WriteFileService).Assembly.GetManifestResourceStream("ForceLocalizeSystem");
            if (dll is not null) {
                string tempDric = Path.Combine(ModRootPath, "Systems");
                //文件夹不存在就创建
                if (!Directory.Exists(tempDric)) Directory.CreateDirectory(tempDric);
                string tempFile = Path.Combine(ModRootPath, "Systems", "ForceLocalizeSystem.cs");
                //资源没有转移就转移
                if (!File.Exists(tempFile)) {
                    Stream newStream = new FileStream(tempFile, FileMode.OpenOrCreate);
                    dll.CopyTo(newStream);
                    newStream.Close();
                }
            }
            dll?.Close();
            #endregion

            var map = PublicProperty.WriteMap;

            #region 构建汉化内容存储的文件 并创建头内容
            var tarGetFileName = ModName + "Translator.cs"; //LocalTextTranslator.cs
            var HongKongHjsonFileName = "IL_" + ModName + "hk.hjson";
            var TawiWanHjsonFileName = "IL_" + ModName + "tw.hjson";
            var CnZhHjsonFileName = "IL_" + ModName + "zh.hjson";
            var PCRHjsonFileName = "IL_" + ModName + "pcr.hjson";
            var CSOWHjsonFileName = "IL_" + ModName + "csow.hjson";

            var writeTarGet = Path.Combine(filePath, tarGetFileName); // LocalText/LocalTextTranslator/LocalTextTranslator.cs
            var HKHjson = Path.Combine(filePath, HongKongHjsonFileName);
            var TWHjson = Path.Combine(filePath, TawiWanHjsonFileName);
            var ZHHjson = Path.Combine(filePath, CnZhHjsonFileName);
            var PCRHjson = Path.Combine(filePath, PCRHjsonFileName);
            var CSOWHjson = Path.Combine(filePath, CSOWHjsonFileName);


            if (File.Exists(writeTarGet)) File.Delete(writeTarGet);
            if (File.Exists(HKHjson)) File.Delete(HKHjson);
            if (File.Exists(TWHjson)) File.Delete(TWHjson);
            if (File.Exists(ZHHjson)) File.Delete(ZHHjson);
            if (File.Exists(PCRHjson)) File.Delete(PCRHjson);
            if (File.Exists(CSOWHjson)) File.Delete(CSOWHjson);

            using var Write = File.Create(writeTarGet);
            using var HKWrite = File.Create(HKHjson);
            using var TWWrite = File.Create(TWHjson);
            using var ZHWrite = File.Create(ZHHjson);
            using var PCRWrite = File.Create(PCRHjson);
            using var CSOWWrite = File.Create(CSOWHjson);

            WriteTo(Write, MyModName, rootPath, tarGetFileName.Split(".")[0], ModName);
            #endregion 构建汉化内容存储的文件

            #region 构建staticHookList
            staticHookList.Add(new StaticHookClass(
                    ModName + "Translator",
                    ModName + "Translator." + tarGetFileName.Split(".")[0],
                    "LoadTranslator")
            { ModName = MyModName });
            #endregion


            foreach (var item in map) {
                ClassNaem = item.Key;
                Write.Write(StringToByte($"\t\t\t\t#region {ClassNaem}"));
                //方法名称 内容
                var Translat = item.Value;
                #region 开始输出
                foreach (var value in Translat) {
                    MethodName = value.Key;
                    var 内容 = value.Value;
                    //填入全类名 还有方法名
                    //ClassName 全类名 value.Key 方法名
                    string classname = ClassNaem.Replace("/", "+");
                    Write.Write(StringToByte($"\t\t\t\tTranslatorLoad.LocalizeByTypeFullName(\"{classname}\", \"{value.Key}\", new ()"));
                    Write.Write(StringToByte("\t\t\t\t{"));
                    Write.Flush();
                    //Item1是英文内容，Item2是中文内容
                    foreach (英汉台港 英汉台港 in 内容) {
                        //这里需要改成本地化键的键，内容是英文内容
                        //Write.Write(StringToByte("\t\t\t\t\t{" + "\"" + 英汉台港.英文.Replace("\n", "\\n") + "\"" + "," + "\"" + 英汉台港.中文.Replace("\n", "\\n") + "\"" + "},"));
                        //{"ModName.classname.methodname.english","ENGLISH"},
                        Write.Write(StringToByte("\t\t\t\t\t{\"" + ModName + "." + classname + "." + MethodName + "." + 英汉台港.id + "\"," + "\"" + 英汉台港.英文 + "\"},"));
                        HKWrite.Write(StringToByte(ModName + "." + classname + "." + MethodName + "." + 英汉台港.id + ": " + 英汉台港.香港));
                        TWWrite.Write(StringToByte(ModName + "." + classname + "." + MethodName + "." + 英汉台港.id + ": " + 英汉台港.台湾));
                        ZHWrite.Write(StringToByte(ModName + "." + classname + "." + MethodName + "." + 英汉台港.id + ": " + 英汉台港.中文));
                        PCRWrite.Write(StringToByte(ModName + "." + classname + "." + MethodName + "." + 英汉台港.id + ": " + 英汉台港.害人));
                        CSOWWrite.Write(StringToByte(ModName + "." + classname + "." + MethodName + "." + 英汉台港.id + ": " + 英汉台港.旧文));

                        Write.Flush();
                        HKWrite.Flush();
                        TWWrite.Flush();
                        PCRWrite.Flush();
                        CSOWWrite.Flush();
                    }
                    Write.Write(StringToByte("\t\t\t\t});"));
                }
                Write.Write(StringToByte($"\t\t\t\t#endregion {ClassNaem}"));
                //类名间换行
                Write.Write(StringToByte("\r\n"));

                Write.Flush();
                #endregion
            }
            //fileTail++;
            Write.Write(StringToByte("\t\t\t}"));
            Write.Write(StringToByte("\t\t}"));
            Write.Write(StringToByte("\t}"));
            Write.Write(StringToByte("}"));
            #region 关闭流
            Write.Dispose();
            HKWrite.Dispose();
            TWWrite.Dispose();
            ZHWrite.Dispose();
            PCRWrite.Dispose();
            CSOWWrite.Dispose();
            Write.Close();
            HKWrite.Close();
            TWWrite.Close();
            ZHWrite.Close();
            PCRWrite.Close();
            CSOWWrite.Close();
            #endregion
            //构建Mod主类文件
            BuildModFile(ModRootClassPath);

            PublicProperty.WriteMap.Clear();
        }

        #region oldWriteFile
        /*
        public void WriteFile(string ModRootPath, string ModName, string MyModName)
        {
            this.ModName = ModName;
            this.MyModName = MyModName;
            //模组主类(继承Mod)
            string ModRootClassPath = Path.Combine(ModRootPath, $"{MyModName}.cs");

            //目标根目录 使用目标模组名称 + Translator
            string rootPath = ModName + "Translator";

            //目标文件路径 自己模组所在目录 + 目标根目录
            string filePath = Path.Combine(ModRootPath, rootPath);
            Directory.CreateDirectory(filePath);

            if (PublicProperty.WriteMap.Count == 0)
                return;

            //获取模组根目录下的全部
            string[] files = Directory.GetFiles(ModRootPath);

            string? tarGetFile = files.FirstOrDefault(f => f.Contains(MyModName + ".cs"));
            //备份原始 模组.cs文件
            if (tarGetFile != null)
            {
                int a = 0;
                string bakFile = tarGetFile + ".bak";
                string newBakFile = string.Empty;

                do
                {
                    newBakFile = bakFile + a;
                    a++;
                } while (File.Exists(newBakFile));

                if (!File.Exists(newBakFile))
                {
                    File.Create(newBakFile).Dispose();
                }
                //将tarGetFile替换给bakFile 备份为他自己
                //File.Replace(tarGetFile, bakFile, tarGetFile,false);
                File.Copy(tarGetFile, newBakFile, true);
            }

            #region 资源转移
            Stream? dll = typeof(WriteFileService).Assembly.GetManifestResourceStream("ForceLocalizeSystem");
            if (dll is not null)
            {
                string tempDric = Path.Combine(ModRootPath, "Systems");
                //文件夹不存在就创建
                if (!Directory.Exists(tempDric))
                    Directory.CreateDirectory(tempDric);
                string tempFile = Path.Combine(ModRootPath, "Systems", "ForceLocalizeSystem.cs");
                //资源没有转移就转移
                if (!File.Exists(tempFile))
                {
                    Stream newStream = new FileStream(tempFile, FileMode.OpenOrCreate);
                    dll.CopyTo(newStream);
                    newStream.Close();
                }
            }
            dll?.Close();
            #endregion

            int fileTail = 0;//防止名称重复
            var map = PublicProperty.WriteMap;

            //TODO 第一层循环 类名 方法名称 内容
            foreach (var item in map)
            {
                ClassNaem = item.Key;
                var Temp = ClassNaem.Split('.');

                //文件名称的开头 现在是类名
                var FileName = Temp[Temp.Length - 1];
                FileName = FileName.Replace("<", "x").Replace(">", "d").Replace(".", "j").Replace("/", "g");


                //使用类名 + 数字来命名
                string fileSystemPath = Path.Combine(filePath, FileName + fileTail + ".cs");

                if (File.Exists(fileSystemPath))
                    File.Delete(fileSystemPath);

                #region 构建staticHookList
                staticHookList.Add(new StaticHookClass(
                        ModName + "Translator",
                        ModName + "Translator." + FileName + fileTail,
                        "LoadTranslator")
                { ModName = MyModName });
                #endregion


                //写
                FileStream Write = File.Create(fileSystemPath);

                WriteTo(Write, MyModName, rootPath, FileName, fileTail, ModName);

                //方法名称 内容
                var Translat = item.Value;
                #region 开始输出
                foreach (var value in Translat)
                {
                    MethodName = value.Key;
                    var 内容 = value.Value;
                    //填入全类名 还有方法名
                    //ClassName 全类名 value.Key 方法名
                    Write.Write(StringToByte($"\t\t\t\tTranslatorLoad.LocalizeByTypeFullName(\"{ClassNaem.Replace("/", "+")}\", \"{value.Key}\", new ()"));
                    Write.Write(StringToByte("\t\t\t\t{"));
                    Write.Flush();
                    foreach (Tuple<String, String> tpule in 内容)
                    {
                        Write.Write(StringToByte("\t\t\t\t\t{" + "\"" + tpule.Item1.Replace("\n", "\\n") + "\"" + "," + "\"" + tpule.Item2.Replace("\n", "\\n") + "\"" + "},"));
                        Write.Flush();
                    }
                    Write.Write(StringToByte("\t\t\t\t});"));
                }


                Write.Write(StringToByte("\t\t\t}"));
                Write.Write(StringToByte("\t\t}"));
                Write.Write(StringToByte("\t}"));
                Write.Write(StringToByte("}"));
                Write.Flush();
                Write.Dispose();
                Write.Close();
                #endregion
            }
            fileTail++;

            //构建Mod主类文件
            BuildModFile(ModRootClassPath);

            PublicProperty.WriteMap.Clear();
        }
        */
        #endregion oldWriteFile

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

        /// <summary>
        /// 公共内容
        /// <para> Write 文件流 </para>
        /// <para> MyModName 自己的模组名称</para>
        /// <para> rootPath 汉化文件所在目录名称 </para>
        /// <para> ModName 要被汉化的模组名字 </para>
        /// </summary>
        /// <param name="Write"> 文件流 </param>
        /// <param name="MyModName"> 你的模组名称 </param>
        /// <param name="rootPath"> 目标根目录 </param>
        /// <param name="FileName"> 文件名的开头 </param>
        /// <param name="fileTail"> 文件名称的结尾</param>
        /// <param name="ModName"> 目标模组名称 </param>
        private static void WriteTo(FileStream Write, string MyModName, string rootPath, string FileName, string ModName, int fileTail = 1)
        {
            Write.Write(StringToByte($"using {MyModName}.Systems;"));
            Write.Write(StringToByte("using System.Collections.Generic;"));
            Write.Write(StringToByte("using Terraria.ModLoader;"));
            //名称空间
            Write.Write(StringToByte($"namespace {MyModName}.{rootPath}"));
            Write.Write(StringToByte("{"));
            //使用要被汉化的模组的名称+数字来定义类型名
            Write.Write(StringToByte($"\tpublic class {FileName}"));//{fileTail}
            Write.Write(StringToByte("\t{"));
            //要被汉化的模组的名称
            Write.Write(StringToByte($"\t\tprivate class {ModName}" + "{}"));

            //选择性加载与JIT
            Write.Write(StringToByte($"\t\t[ExtendsFromMod(\"{ModName}\"), JITWhenModsEnabled(\"{ModName}\")]"));

            Write.Write(StringToByte($"\t\tprivate class TranslatorLoad : ForceLocalizeSystem<{ModName}, TranslatorLoad>" + "{}"));
            Write.Write(StringToByte("\t\tpublic static void LoadTranslator()"));
            Write.Write(StringToByte("\t\t{"));

            //获取模组判断同时能判断是不是存在
            Write.Write(StringToByte($"\t\t\tif(ModLoader.TryGetMod(\"{ModName}\",out var mod))"));
            Write.Write(StringToByte("\t\t\t{"));

        }


        #region 公共输出 2024/12/17 bak
        /*
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
        */
        #endregion
        private static void BuildModFile(string filePath)
        {
            using FileStream write = File.Open(filePath, FileMode.Create);
            #region 固定输出
            write.Write(StringToByte("using System.Linq;"));
            write.Write(StringToByte("using System.Reflection;"));
            write.Write(StringToByte("using System.Threading;"));
            write.Write(StringToByte("using Terraria;"));
            write.Write(StringToByte("using Terraria.ModLoader;"));
            write.Flush();
            write.Write(StringToByte($"namespace {staticHookList[0].ModName}"));
            write.Write(StringToByte("{"));
            write.Write(StringToByte($"\tpublic class {staticHookList[0].ModName} : Mod"));
            write.Write(StringToByte("\t{"));

            #region 挂钩子
            /*
            write.Write(StringToByte($"\t\tstatic {staticHookList[0].ModName}()"));
            write.Write(StringToByte("\t\t{"));
            write.Write(StringToByte("\t\t\tvar tModLoaderDLL = typeof(Main);"));
            write.Write(StringToByte("\t\t\tvar ModLoaderType = tModLoaderDLL.Assembly.GetTypes().FirstOrDefault(f => f.Name == \"ModContent\");"));
            write.Write(StringToByte("\t\t\tMethodInfo LoadMethod = ModLoaderType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic).FirstOrDefault(f => f.Name == \"Load\");"));
            write.Write(StringToByte("\t\t\tMonoModHooks.Add(LoadMethod, LanguageHook);"));
            write.Write(StringToByte("\t\t}"));
            write.Write(StringToByte(""));
            write.Write(StringToByte("\t\tpublic delegate void LanguageDelegate(CancellationToken token);"));
            write.Write(StringToByte(""));
            write.Write(StringToByte("\t\tpublic static void LanguageHook(LanguageDelegate orig, CancellationToken token)"));
            write.Write(StringToByte("\t\t{"));
            */
            #endregion 挂钩子

            write.Write(StringToByte("\t\tpublic override void Load()"));
            write.Write(StringToByte("\t\t{"));

            #endregion  固定输出
            write.Flush();

            foreach (var item in staticHookList) {
                write.Write(StringToByte($"\t\t\t{item.ClassName}.{item.MethodName}();"));
                write.Flush();
            }
            staticHookList.Clear();
            //write.Write(StringToByte("\t\t\torig.Invoke(token);")); //挂钩子
            write.Write(StringToByte("\t\t\tbase.Load();"));
            write.Write(StringToByte("\t\t}"));
            write.Write(StringToByte("\t}"));
            write.Write(StringToByte("}"));
            write.Flush();
            write.Dispose();
        }

        /// <summary>
        /// 写文件模型类 含类名
        /// </summary>
        public class WriteFileModelOne(string Key, Dictionary<string, List<Tuple<string, string>>> keyValues)
        {
            /// <summary>
            /// 类名
            /// </summary>
            public string ClassName = Key;

            /// <summary>
            /// 全部内容，其中Key是方法名称，Value是英文与中文的元组
            /// </summary>
            public Dictionary<string, List<Tuple<string, string>>> Content = keyValues;

            //public WriteFileModelOne(string Key, Dictionary<string, List<Tuple<string, string>>> keyValues)
            //{
            //    ClassName = Key;
            //    Content = keyValues;
            //}
        }
    }
}
