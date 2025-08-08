using System.Text;
using TranslatorFormAssembly.Models;
using TranslatorLibrary.Tools;
using TranslatorFromAssembly.Services.IServices;
using System.Threading.Tasks;

namespace TranslatorFromAssembly.Services.Services;

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
    /// <param name="modRootPath"> 模组根目录 </param>
    /// <param name="modName"> 要被汉化的模组名称 </param>
    /// <param name="YouModName">你的模组名称</param>
    public void WriteFile(string modRootPath, string modName, string myModName)
    {
        this.ModName = modName;
        this.MyModName = myModName;
        string ModRootClassPath = Path.Combine(modRootPath, $"{myModName}.cs"); //模组主类(继承Mod)
        string rootPath = modName + "Translator";                               //目标根目录 使用目标模组名称 + Translator
        string filePath = Path.Combine(modRootPath, rootPath);                  //目标文件路径 自己模组所在目录 + 目标根目录
        Directory.CreateDirectory(filePath);                                    //创建目标文件夹(汉化文件存储的主目录)

        if (PublicProperty.WriteMap.Count == 0)             //如果没有内容就返回了
            return;

        #region 资源转移
        string[] resValues = typeof(WriteFileService).Assembly.GetManifestResourceNames();
        foreach (var resName in resValues) {
            _ = Task.Run(() => 资源转移(resName, modRootPath));
        }
        #endregion

        var map = PublicProperty.WriteMap;

        #region 构建汉化内容存储的文件 并创建头内容
        var tarGetFileName = modName + "Translator.cs"; //LocalTextTranslator.cs

        var writeTarGet = Path.Combine(filePath, tarGetFileName); // LocalText/LocalTextTranslator/


        if (File.Exists(writeTarGet)) File.Delete(writeTarGet);

        using var Write = File.Create(writeTarGet);

        WriteTo(Write, myModName, rootPath, tarGetFileName.Split(".")[0], modName);
        #endregion 构建汉化内容存储的文件


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

                //构造函数和静态构造
                classname = ReplaceNotChar(classname);
                MethodName = ReplaceNotChar(MethodName);
                //Item1是英文内容，Item2是中文内容
                foreach (英汉台港 英汉台港 in 内容) {
                    Write.Write(StringToByte("\t\t\t\t\t{" + 英汉台港.中文 + ","  + 英汉台港.英文 + "},"));

                    Write.Flush();
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
        #endregion
        //构建Mod主类文件
        //BuildModFile(ModRootClassPath);

        PublicProperty.WriteMap.Clear();
    }

    private static string ReplaceNotChar(string str)
    {
        return str.Replace('<', 'x').Replace('>', 'd').Replace('.', 'j').Replace('/', 'g').Replace('+', 'j');
    }

    /// <param name="localName">资源名称</param>
    /// <param name="modRootPath"></param>
    /// <returns></returns>
    private async Task 资源转移(string localName, string modRootPath)
    {
        if (MyModName == string.Empty)
            return;
        var local = GetType().Assembly.GetManifestResourceStream(localName);
        if (local is not null) {
            string tempDric = Path.Combine(modRootPath, "Systems");
            //文件夹不存在就创建
            if (!Directory.Exists(tempDric)) Directory.CreateDirectory(tempDric);
            string tempFile = Path.Combine(modRootPath, "Systems", localName + ".cs");
            //资源没有转移就转移
            if (!File.Exists(tempFile)) {
                FileStream newStream = new FileStream(tempFile, FileMode.OpenOrCreate);
                var sr = new StreamReader(local);
                var values = await sr.ReadToEndAsync();
                var newValues = values.Replace("@${ModName}$@", MyModName);
                newStream.Write(Encoding.UTF8.GetBytes(newValues));
                newStream.Flush();
                newStream.Dispose();
            }
        }
        local?.Dispose();
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
                    } else {
                        var method = item.MethodName;
                        var tuple2 = CreateYHTG(item);
                        List<英汉台港> list = [];
                        list.Add(tuple2);
                        value.Add(method, list);
                    }
                }
            } else {
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
        return new 英汉台港(转义(item.English), 转义(item.Chinese), 转义(item.TaiWan), 转义(item.HongKong), 转义(item.PCR), 转义(item.CSOW), item.Id);
    }
    private static string 转义(string str) => str.Replace("\n", @"\n");
    private static byte[] StringToByte(string str)
    {
        string str2 = str + "\r\n";
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
    private static void WriteTo(FileStream Write, string MyModName, string rootPath, string FileName, string ModName, int fileTail = 1)
    {
        Write.Write(StringToByte("#pragma warning disable CA2255"));
        Write.Write(StringToByte($"using {MyModName}.Systems;"));
        Write.Write(StringToByte("using System.Collections.Generic;"));
        Write.Write(StringToByte("using Terraria.ModLoader;"));
        Write.Write(StringToByte("using System.Runtime.CompilerServices;"));
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
        //[ModuleInitializer]
        Write.Write(StringToByte("\t\t[ModuleInitializer]"));
        Write.Write(StringToByte("\t\tpublic static void LoadTranslator()"));
        Write.Write(StringToByte("\t\t{"));

        //获取模组判断同时能判断是不是存在
        Write.Write(StringToByte($"\t\t\tif(LoadModAssembly.LoadModContext.TryGetValue(\"{ModName}\",out _))"));
        Write.Write(StringToByte("\t\t\t{"));

    }
}
