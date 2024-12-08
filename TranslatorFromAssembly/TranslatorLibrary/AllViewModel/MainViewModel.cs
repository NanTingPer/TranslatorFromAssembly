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
        private ISQLiteService _sqliteService;
        private IILService _ilService;
        private string _indexText;
        public ICommand CommandGetTranslator { get; }
        public ICommand PitchsCommand { get; }

        public ICommand GetAssemblyStrCommand { get; }
        
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
        public MainViewModel(ISQLiteService sqliteService,IILService iLService) 
        {
            _sqliteService = sqliteService;
            _ilService = iLService;
            IndexText = "C:\\Users\\23759\\Documents\\My Games\\Terraria\\tModLoader\\ModReader\\FargowiltasSouls\\FargowiltasSouls.dll";
            CommandGetTranslator = new AsyncRelayCommand(GetTranslator);
            PitchsCommand = new RelayCommand<object>(GetPitchs);
            GetAssemblyStrCommand = new AsyncRelayCommand(GetAssemblyStr);
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
        /// 用来加载ILService提出的项目 到显示List中
        /// </summary>
        private async Task GetAssemblyStr()
        {
            ModEnglishList.Clear();
            List<PreLoadData> tempList = await _ilService.GetAssemblyILString(IndexText);
            foreach (var item in tempList)
            {
                ModEnglishList.Add(item);
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
    }
}
