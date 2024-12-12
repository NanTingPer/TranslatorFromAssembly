using CommunityToolkit.Mvvm.ComponentModel;
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
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllViewModel
{
    public partial class DLLViewModel : ViewModelBase
    {
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
        public ObservableCollection<PreLoadData> ModEnglishList = PublicProperty.DataList;

        /// <summary>
        /// 存储被选中项
        /// </summary>
        public ObservableCollection<PreLoadData> Pitchs { get; set; } = new();

        /// <summary>
        /// 第一个可编辑文本框 未来用来填入 FilePath
        /// </summary>
        public string IndexText { get => _indexText; set => SetProperty(ref _indexText, value); }

        public int PageNum { get => pageNum; set => SetProperty(ref pageNum, value); }
        public DLLViewModel(ISQLiteService sqliteService,IILService iLService,ISQLiteExtract<PreLoadData> liteExtract) 
        {
            _sqliteService = sqliteService;
            _ilService = iLService;
            _sQLiteExtract = liteExtract;
            CommandGetTranslator = new AsyncRelayCommand(GetTranslator);
            PitchsCommand = new RelayCommand<object>(GetPitchs);
            GetAssemblyStrCommand = new AsyncRelayCommand(GetAssemblyStr);
            SetSQLiteExtractCommand = new AsyncRelayCommand(SetSQLiteExtract);
            GetAssemblyStrPgDnCommand = new AsyncRelayCommand(GetAssemblyStrPgDn);

            IndexText = "C:\\Users\\23759\\Documents\\My Games\\Terraria\\tModLoader\\ModReader\\Stellamod\\Stellamod.dll";
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
            if (++PageNum * pageSize >= pageCount)
            { 
                PageNum = pageCount / pageSize - 1;
            }
            if(pageNum <= 0) pageNum = 1;
            PreLoadData[] datas = await _sQLiteExtract.GetData((PageNum * pageSize), pageSize);
            if (datas.Count() <= 0)
                return;

            foreach (var item in datas)
            {
                ModEnglishList.Add(item);
                ModEnglishList.RemoveAt(0);
            }
        }

        /// <summary>
        /// 用来加载ILService提出的项目 到显示List中 从数据库加载 上一页
        /// </summary>
        private async Task GetAssemblyStrPgDn()
        {
            if (--PageNum < 0)
            {
                PageNum = 0;
            }
                
            PreLoadData[] datas = await _sQLiteExtract.GetData((PageNum * pageSize),pageSize);
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
        private async Task SetSQLiteExtract()
        {
            PageNum = 0;
            string[] strs = Path.GetFileName(IndexText).Split(".");
            if (strs[1] is "dll")
            {
                ModEnglishList.Clear();

                //载入空数据 防止无法加载
                for (int i = 0; i < 10; i++)
                    ModEnglishList.Add(new());

                await CreateSQLiteExtractDataBase(strs[0]);
                List<PreLoadData> tempList = await _ilService.GetAssemblyILString(IndexText);
                await _sQLiteExtract.AddData(tempList);

                await GetAssemblyStr();
            }
            pageCount = await _sQLiteExtract.PageCount();
            InitaDataList();
            return;
        }

        /// <summary>
        /// 初始化显示视图
        /// </summary>
        private async void InitaDataList()
        {
            PageNum++;
            PreLoadData[] datas = await _sQLiteExtract.GetData(0, pageSize);
            foreach (var item in datas)
            {
                ModEnglishList.Add(item);
                ModEnglishList.RemoveAt(0);
            }
        }

        private async Task CreateSQLiteExtractDataBase(string name)
        {
            await _sQLiteExtract.CreateDatabase(name);
        }
    }
}
