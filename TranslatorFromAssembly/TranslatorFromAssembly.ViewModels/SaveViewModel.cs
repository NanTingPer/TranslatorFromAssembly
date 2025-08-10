using CommunityToolkit.Mvvm.Input;
using System.Text;
using static TranslatorLibrary.Tools.HjsonSerializer;
using TranslatorFromAssembly.Services.IServices;
using TranslatorFormAssembly.Models;
using TranslatorLibrary.Tools;

namespace TranslatorFromAssembly.ViewModels;
public class SaveViewModel : ViewModelBase
{
    //private const string defaultModPath = "C:\\Users\\用户名\\Documents\\My Games\\Terraria\\tModLoader\\ModSources\\";

    private readonly static string defaultModPath = $"C:\\Users\\{Environment.UserName}\\Documents\\My Games\\Terraria\\tModLoader\\ModSources\\";

    private const string MyModDicPath = "MyModPath";
    private const string MyModNameConf = ".TarGetModName";


    private IWriteFileService _writeFileService;
    private ISQLiteExtract<PreLoadData> _sQLiteExtract;

    private string myModPath = string.Empty;
    private string tarGetModName = string.Empty;
    private string myModName = string.Empty;
    private DataFilePath? listBoxOption;

    public string MyModPath { get => myModPath; set => SetProperty(ref myModPath, value); }
    public string TarGetModName { get => tarGetModName; set => SetProperty(ref tarGetModName, value); }
    public string MyModName { get => myModName; set => SetProperty(ref myModName, value); }

    public DataFilePath ListBoxOption { get => listBoxOption!; set => SetProperty(ref listBoxOption, value); }

    public IRelayCommand ClickListOptionCommand { get; }
    public IRelayCommand WriteOutputCommand { get; }
    public IRelayCommand LoadDataPathToListCommand { get; }
    public IRelayCommand SaveToSwitchCommand { get; }
    public SaveViewModel(IWriteFileService writeFileService, ISQLiteExtract<PreLoadData> sQLiteExtract)
    {
        _writeFileService = writeFileService;
        _sQLiteExtract = sQLiteExtract;

        ClickListOptionCommand = new RelayCommand(ClickListOption);
        WriteOutputCommand = new AsyncRelayCommand(WriteOutput);
        LoadDataPathToListCommand = new AsyncRelayCommand(LoadDataPathToList);
        SaveToSwitchCommand = new RelayCommand(SaveToSwitch);
    }

    private async Task WriteOutput()
    {
        if (string.IsNullOrWhiteSpace(MyModName) ||
            string.IsNullOrWhiteSpace(TarGetModName) ||
            string.IsNullOrWhiteSpace(MyModPath)) { return; }
        await _writeFileService.CreateWriteMap(_sQLiteExtract, listBoxOption!.FileName); //这FileName在不改文件名的情况下就是ModName
        _writeFileService.WriteFile(MyModPath, TarGetModName, MyModName);
        #region 导出为Hjson
        var datas = await _sQLiteExtract.GetDataAsync(0, 0, save: PublicProperty.SaveMode.ReallAll);
        SaveToHjson(datas, listBoxOption.FileName);
        #endregion
        Config.SetConf(listBoxOption.FileName + MyModNameConf, MyModName); //设置自己的模组名称与自己的模组名称匹配
        string? path = Path.GetDirectoryName(MyModPath);//ModSource文件夹目录
        if (path is not null)
            Config.SetConf(MyModDicPath, path);
        //想法001 能否通过创建Hjson存储Key与ILIndex的对应关系，这样在进行IL编辑时速度会更快
    }

    private void ClickListOption()
    {
        if (ListBoxOption is null || ListBoxOption.FileName == string.Empty)
            return;

        _sQLiteExtract.CreateDatabaseAsync(ListBoxOption.FileName);
        TarGetModName = ListBoxOption.FileName;

        string? modDicPath = Config.GetConf(MyModDicPath); //获取存储目录
        MyModPath = modDicPath is null ? defaultModPath : Config.GetConf(MyModDicPath)!;

        if (Config.GetConf(listBoxOption!.FileName + MyModNameConf) is string mymodname) { //获取指定模组的指定目录
            MyModPath = Path.Combine(MyModPath, mymodname);
            MyModName = mymodname;
        }

    }

    /// <summary>
    /// 加载全部数据库到显示集合
    /// </summary>
    private async Task LoadDataPathToList()
    {
        await Task.Run(() => PublicProperty.LoadAllDataFileToDataFilePaths());
    }

    #region 保持到临时文件
    private async void SaveToSwitch()
    {
        string savePath = Path.Combine(Path.GetTempPath(), TarGetModName + ".txt");

        await Task.Run(async () => {
            PreLoadData[] preLoadDatas = await _sQLiteExtract.GetDataAsync(0, 0, save: PublicProperty.SaveMode.All);
            #region Json解析
            /*
            string hjsonCount = File.ReadAllText("C:/Users/23759/Documents/My Games/Terraria/tModLoader/ModSources/LibTest/Localization/BACK_Mods.LibTest.hjson");
            JsonValue jsv = HjsonValue.Load("C:/Users/23759/Documents/My Games/Terraria/tModLoader/ModSources/LibTest/Localization/BACK_Mods.LibTest.hjson");
            var r= jsv["3001"].ToString();


            var jsonObj = new JsonObject();
            foreach (var pd in preLoadDatas) {
                jsonObj.Add(pd.Id.ToString(), JsonValue.Parse(JsonSerializer.Serialize(pd)));
            }

            var jsonValue = HjsonValue.Parse(jsonObj.ToString());
            jsonValue.Save("C:/Users/23759/Documents/My Games/Terraria/tModLoader/ModSources/LibTest/Localization/BACK_Mods.LibTest.hjson", Stringify.Hjson);
            */
            #endregion
            #region 保存为sw
            using (FileStream file = new FileStream(savePath, FileMode.Create)) {
                file.Write(StrToBytes("switch (str)"));
                file.Write(StrToBytes("{"));

                foreach (var item in preLoadDatas) {
                    file.Write(StrToBytes($"\tcase \"{item.English}\":"));
                    file.Write(StrToBytes($"\t\treturn \"{item.Chinese}\";"));
                    file.Flush();
                }
                file.Write(StrToBytes("\tdefault: return str;"));
                file.Write(StrToBytes("}"));
                file.Flush();
            }
            #endregion
        });

    }
    #endregion

    private static byte[] StrToBytes(string str)
    {
        return Encoding.UTF8.GetBytes(str + "\r\n");
    }
}
