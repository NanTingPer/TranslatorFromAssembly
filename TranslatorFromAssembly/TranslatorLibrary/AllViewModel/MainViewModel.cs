using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;

namespace TranslatorLibrary.AllViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private int task = 0;
        private int skip = 0;
        private ISQLiteService _sqliteService;
        private IILService _ilService;
        private ISQLiteExtract<PreLoadData> _sQLiteExtract;
        private string _indexText;
        public ICommand CommandGetTranslator { get; }
        public ICommand PitchsCommand { get; }
        public ICommand GetAssemblyStrCommand { get; }
        public ICommand GetAssemblyStrPgDnCommand { get; }
        public ICommand SetSQLiteExtractCommand { get; }

        /// <summary>
        /// 用来显示项目
        /// </summary>
        public ObservableCollection<SimpleTranslator> ViewList { get; set; } = new();

        /// <summary>
        /// 用来显示被ILService提取出来的项目
        /// </summary>
        public ObservableCollection<PreLoadData> ModEnglishList { get; set; } = new();

        /// <summary>
        /// 存储被选中项
        /// </summary>
        public ObservableCollection<PreLoadData> Pitchs { get; set; } = new();

        /// <summary>
        /// 第一个可编辑文本框 未来用来填入 FilePath
        /// </summary>
        public string IndexText { get => _indexText; set => SetProperty(ref _indexText, value); }
        public MainViewModel(ISQLiteService sqliteService,IILService iLService,ISQLiteExtract<PreLoadData> liteExtract) 
        {
            _sqliteService = sqliteService;
            _ilService = iLService;
            _sQLiteExtract = liteExtract;
            CommandGetTranslator = new AsyncRelayCommand(GetTranslator);
            PitchsCommand = new RelayCommand<object>(GetPitchs);
            GetAssemblyStrCommand = new AsyncRelayCommand(GetAssemblyStr);
            SetSQLiteExtractCommand = new RelayCommand(SetSQLiteExtract);
            GetAssemblyStrPgDnCommand = new AsyncRelayCommand(GetAssemblyStrPgDn);

            IndexText = "C:\\Users\\23759\\Documents\\My Games\\Terraria\\tModLoader\\ModReader\\FargowiltasSouls\\FargowiltasSouls.dll";
        }

        /// <summary>
        /// 获取单词翻译
        /// </summary>
        /// <returns></returns>
        private async Task GetTranslator()
        {
            IEnumerable<DatabaseModle> datas = await _sqliteService.GetData(IndexText);
            foreach (var data in datas)
            {
                var st = SimpleTranslator.CreateSimpleTranslator(data);
                if(!ViewList.Contains(st) && data is not null)
                    ViewList.Add(st);
            }
        }


        
        /// <summary>
        /// 用来加载ILService提出的项目 到显示List中 从数据库加载 下一页
        /// </summary>
        private async Task GetAssemblyStr()
        {
            PreLoadData[] datas = await _sQLiteExtract.GetData(skip,10);
            if (datas.Count() <= 0)
                return;
            foreach (var item in datas)
            {
                ModEnglishList.Add(item);
                ModEnglishList.RemoveAt(0);
            }
            skip += 10;
        }

        /// <summary>
        /// 用来加载ILService提出的项目 到显示List中 从数据库加载 上一页
        /// </summary>
        private async Task GetAssemblyStrPgDn()
        {
            skip = skip - 10 <= 0 ? 0 : skip - 10;
            PreLoadData[] datas = await _sQLiteExtract.GetData(skip, 10);
            if(datas.Count() == 0) return;
            foreach (var item in datas)
            {
                ModEnglishList.Add(item);
                ModEnglishList.RemoveAt(0);
            }
        }


        /// <summary>
        /// 项目被选中 触发 并载入(加载) Pitchs
        /// </summary>
        /// <param name="args"></param>
        public void GetPitchs(object args)
        {
            IList tempIEn = (IList)args;
            foreach (var item in tempIEn)
            {
                Pitchs.Add((PreLoadData)item);
            }
        }

        /// <summary>
        /// 将内容插入数据库
        /// </summary>
        private async void SetSQLiteExtract()
        {
            string[] strs = Path.GetFileName(IndexText).Split(".");
            if (strs[1] is "dll")
            {
                ModEnglishList.Clear();
                for (int i = 0; i < 10; i++)
                    ModEnglishList.Add(new());
                CreateSQLiteExtractDataBase(strs[0]);
                List<PreLoadData> tempList = await _ilService.GetAssemblyILString(IndexText);
                await _sQLiteExtract.AddData(tempList);

                await GetAssemblyStr();
            }
            return;
        }

        private void CreateSQLiteExtractDataBase(string name)
        {
            _sQLiteExtract.CreateDatabase(name);
        }
    }
}
