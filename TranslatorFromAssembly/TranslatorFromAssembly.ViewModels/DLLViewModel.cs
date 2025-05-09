using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using TranslatorFormAssembly.Models;
using TranslatorLibrary.Tools;
using static TranslatorLibrary.Tools.HjsonSerializer;
using TranslatorFromAssembly.Services.IServices;

namespace TranslatorFromAssembly.ViewModels
{
    public partial class DLLViewModel : ViewModelBase
    {
        private const string LastDLLConfKey = "DLLPath";

        /// <summary>
        /// 拿出数据条数
        /// </summary>
        private int task = 0;

        /// <summary>
        /// 跳过数据条数
        /// </summary>
        private int skip = 0;

        /// <summary>
        /// 数据总条数
        /// </summary>
        private int pageCount = 0;

        /// <summary>
        /// 单页数据大小
        /// </summary>
        private int pageSize = 10;

        /// <summary>
        /// 第几页
        /// </summary>
        private int pageNum = 0;

        //public int Skip { get => skip ; set => SetProperty(ref skip,value); }
        private ISQLiteService _sqliteService;
        private IILService _ilService;
        private ISQLiteExtract<PreLoadData> _sQLiteExtract;
        private IRootViewCut _rootViewCut;
        private string _indexText = string.Empty;
        public ICommand CommandGetTranslator { get; }
        public ICommand PitchsCommand { get; }
        public ICommand GetAssemblyStrCommand { get; }
        public ICommand GetAssemblyStrPgDnCommand { get; }
        public ICommand SetSQLiteExtractCommand { get; }
        public ICommand GotSaveViewCommand { get; }
        public ICommand OpenFilePathCommand { get; }

        /// <summary>
        /// 用来显示项目
        /// </summary>
        public ObservableCollection<SimpleTranslator> ViewList { get; set; } = new();

        /// <summary>
        /// 用来显示被ILService提取出来的项目
        /// </summary>
        public ObservableCollection<PreLoadData> ModEnglishList = PublicProperty.DataList;

        /// <summary>
        /// 存储被选中项
        /// </summary>
        public ObservableCollection<PreLoadData> Pitchs { get; set; } = new();

        /// <summary>
        /// 第一个可编辑文本框 未来用来填入 FilePath
        /// </summary>
        public string IndexText { get => _indexText; set { SetProperty(ref _indexText, value); Config.SetConf(LastDLLConfKey, _indexText); } }

        public int PageNum { get => pageNum; set => SetProperty(ref pageNum, value); }
        public DLLViewModel(ISQLiteService sqliteService, IILService iLService, ISQLiteExtract<PreLoadData> liteExtract, IRootViewCut viewcut)
        {
            _sqliteService = sqliteService;
            _ilService = iLService;
            _sQLiteExtract = liteExtract;
            _rootViewCut = viewcut;
            CommandGetTranslator = new AsyncRelayCommand(GetTranslator);
            PitchsCommand = new RelayCommand<object>(GetPitchs!);
            GetAssemblyStrCommand = new AsyncRelayCommand(GetAssemblyStr);
            SetSQLiteExtractCommand = new AsyncRelayCommand(SetSQLiteExtract);
            GetAssemblyStrPgDnCommand = new AsyncRelayCommand(GetAssemblyStrPgDn);
            OpenFilePathCommand = new RelayCommand(OpenFilePath);
            GotSaveViewCommand = new RelayCommand(GoToSaveView);
            if (Config.GetConf(LastDLLConfKey) is null) {
                Config.SetConf(LastDLLConfKey, "C:\\Users\\用户名\\Documents\\My Games\\Terraria\\tModLoader\\ModReader\\目标模组名\\目标.dll");
            }
            IndexText = Config.GetConf(LastDLLConfKey)!;
        }



        /// <summary>
        /// 将内容插入数据库
        /// </summary>
        private async Task SetSQLiteExtract()
        {
            PageNum = 0;
            string[] strs = Path.GetFileName(IndexText).Split(".");
            if (strs[1] is "dll") {
                ModEnglishList.Clear();

                //载入空数据 防止无法加载
                for (int i = 0; i < 10; i++)
                    ModEnglishList.Add(new());

                await CreateSQLiteExtractDataBase(strs[0]);
                List<PreLoadData> tempList = await _ilService.GetAssemblyILStringAsync(IndexText);
                if (tempList == null)
                    return;

                //寻找是否存在Hjson文件,如果存在就把Hjson数据导入到数据库
                await HjsonToSQLite(strs[0], _sQLiteExtract);
                //将程序集提取出来的数据添加到数据库
                await _sQLiteExtract.AddDataAsync(tempList);
                //将数据保存为Hjson
                PreLoadData[] plds = await _sQLiteExtract.GetDataAsync(0,0,save: PublicProperty.SaveMode.ReallAll);
                SaveToHjson(plds, strs[0]);

                await GetAssemblyStr();//TODO 这个可以不要了
            }
            pageCount = await _sQLiteExtract.PageCountAsync();
            PageNum = 1;
            InitaDataList();
            return;
        }

        /// <summary>
        /// 跳转到报错视图
        /// </summary>
        private void GoToSaveView()
        {
            _rootViewCut.ViewCut("SaveViewModel");
        }

        /// <summary>
        /// 打开文件资源路径
        /// </summary>
        private void OpenFilePath()
        {
            Process.Start(new ProcessStartInfo() 
            { 
                FileName = "explorer.exe", 
                Arguments = GetAppFilePath.GetPath(), 
                UseShellExecute = true 
            });
        }
        #region 无什么大用了
        /// <summary>
        /// 获取单词翻译
        /// </summary>
        /// <returns></returns>
        private async Task GetTranslator()
        {
            IEnumerable<DatabaseModle> datas = await _sqliteService.GetData(IndexText);
            foreach (var data in datas) {
                var st = SimpleTranslator.CreateSimpleTranslator(data);
                if (!ViewList.Contains(st) && data is not null)
                    ViewList.Add(st);
            }
        }

        /// <summary>
        /// 用来加载ILService提出的项目 到显示List中 从数据库加载 下一页
        /// </summary>
        private async Task GetAssemblyStr()
        {
            if (++PageNum * pageSize >= pageCount) {
                PageNum = pageCount / pageSize - 1;
            }
            if (pageNum <= 0) pageNum = 1;
            PreLoadData[] datas = await _sQLiteExtract.GetDataAsync(PageNum * pageSize, pageSize);
            if (datas.Length <= 0)
                return;

            foreach (var item in datas) {
                ModEnglishList.Add(item);
                ModEnglishList.RemoveAt(0);
            }
        }

        /// <summary>
        /// 用来加载ILService提出的项目 到显示List中 从数据库加载 上一页
        /// </summary>
        private async Task GetAssemblyStrPgDn()
        {
            if (--PageNum < 0) {
                PageNum = 0;
            }

            PreLoadData[] datas = await _sQLiteExtract.GetDataAsync(PageNum * pageSize, pageSize);
            if (datas.Length == 0) return;
            foreach (var item in datas) {
                ModEnglishList.Add(item);
                ModEnglishList.RemoveAt(0);
            }

        }


        /// <summary>
        /// 项目被选中 触发 并载入(加载) Pitchs
        /// </summary>
        /// <param name="args"></param>
        private void GetPitchs(object args)
        {
            IList tempIEn = (IList)args;
            foreach (var item in tempIEn) {
                Pitchs.Add((PreLoadData)item);
            }
        }


        /// <summary>
        /// 初始化显示视图
        /// </summary>
        private async void InitaDataList()
        {
            PageNum++;
            PreLoadData[] datas = await _sQLiteExtract.GetDataAsync(0, pageSize);
            foreach (var item in datas) {
                ModEnglishList.Add(item);
                ModEnglishList.RemoveAt(0);
            }
        }
        #endregion
        private async Task CreateSQLiteExtractDataBase(string name)
        {
            await _sQLiteExtract.CreateDatabaseAsync(name);
        }
    }
}
