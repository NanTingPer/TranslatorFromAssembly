using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllViewModel
{
    public class ListViewModel : ViewModelBase
    {
        /// <summary>
        /// 单页数据大小
        /// </summary>
        private int pageSize = 10;

        /// <summary>
        /// 第几页
        /// </summary>
        private int pageNum = 0;

        private int pageCount = 0;

        private int _paneSize = 200;

        private bool _isPaneOpen;

        private bool _oneLoad = false;

        private string _className = string.Empty;

        private string _methodName = string.Empty;
        private string _isShow = "不显示隐藏";

        /// <summary>
        /// 决定是否显示隐藏 false 不显示
        /// </summary>
        private bool fineIsShow = false;

        private string _english = string.Empty;
        private PreLoadData _selectItem;

        private ISQLiteExtract<PreLoadData> _SQLiteExtract;

        private DataFilePath _dataFilePath;


        public ICommand GetAssemblyStr_PgDnCommand { get; }
        public ICommand GetAssemblyStr_PgUpCommand { get; }
        public ICommand IsPaneOpenMethodCommand { get; }
        public ICommand LoadDataPathToListCommand { get; }
        public ICommand InitialTableListCommand { get; }
        public ICommand EditEndedMethodCommand { get; }
        public ICommand SelectContCommand { get; }
        public ICommand SetDataIsNoShowCommand { get; }
        public ICommand SelectCommand { get; }
        public ICommand ClearSelectCommand { get; }
        public ICommand IsShowCommand { get; }

        public int PaneSize { get => _paneSize; set => SetProperty(ref _paneSize, value); }
        public bool IsPaneOpen { get => _isPaneOpen; set => SetProperty(ref _isPaneOpen, value); }
        public string ClassName { get => _className; set => SetProperty(ref _className, value); }
        public string MethodName { get => _methodName; set => SetProperty(ref _methodName, value); }
        public string English { get => _english; set => SetProperty(ref _english, value); }
        public string IsShow { get => _isShow; set => SetProperty(ref _isShow, value); }

        /// <summary>
        /// 列表中被选中项
        /// </summary>
        public PreLoadData SelectItem { get => _selectItem; set => SetProperty(ref _selectItem, value); }

        /// <summary>
        /// 存储被选中项 多选
        /// </summary>
        public List<PreLoadData> SelectData { get; set; } = new();

        public ObservableCollection<PreLoadData> DataList { get; set; } = PublicProperty.DataList;

        /// <summary>
        /// ListBox的被选中项
        /// </summary>
        public DataFilePath DataFilePath { get => _dataFilePath; set => SetProperty(ref _dataFilePath, value); }
        public ListViewModel(ISQLiteExtract<PreLoadData> sQLiteExtract)
        {
            _SQLiteExtract = sQLiteExtract;
            GetAssemblyStr_PgDnCommand = new AsyncRelayCommand(GetAssemblyStr_PgDn);
            GetAssemblyStr_PgUpCommand = new AsyncRelayCommand(GetAssemblyStr_PgUp);
            IsPaneOpenMethodCommand = new RelayCommand(IsPaneOpenMethod);
            LoadDataPathToListCommand = new RelayCommand(LoadDataPathToList);
            InitialTableListCommand = new AsyncRelayCommand(InitialTableList);
            EditEndedMethodCommand = new RelayCommand(EditEndedMethod);
            SelectContCommand = new RelayCommand<object>(SelectCont);
            SetDataIsNoShowCommand = new RelayCommand(SetDataIsNoShow);
            SelectCommand = new AsyncRelayCommand(Select);
            ClearSelectCommand = new RelayCommand(ClearSelect);
            IsShowCommand = new RelayCommand(IsShow_);
        }


        /// <summary>
        /// 用来加载ILService提出的项目 到显示List中 从数据库加载 下一页
        /// </summary>
        private async Task GetAssemblyStr_PgDn()
        {
            initTextOrderBy();
            if (DataFilePath is null)
                return;
            pageCount = await _SQLiteExtract.PageCountAsync();
            if (++pageNum * pageSize >= pageCount) {
                pageNum = pageCount / pageSize - 1;
            }
            PreLoadData[] datas = await _SQLiteExtract.GetDataAsync((pageNum * pageSize), pageSize, ClassName, MethodName, English, isShow: fineIsShow);
            if (datas.Count() <= 0) { pageNum -= 1; return; }


            foreach (var item in datas) {
                if (DataList.Contains(item))
                    continue;

                DataList.Add(item);
                if (DataList.Count >= 10) {
                    DataList.RemoveAt(0);
                }
            }

            控制显示();
        }

        /// <summary>
        /// 用来加载ILService提出的项目 到显示List中 从数据库加载 上一页
        /// </summary>
        private async Task GetAssemblyStr_PgUp()
        {
            initTextOrderBy();
            if (DataFilePath is null)
                return;
            if (--pageNum < 0) {
                pageNum = 0;
            }

            PreLoadData[] datas = await _SQLiteExtract.GetDataAsync((pageNum * pageSize), pageSize, ClassName, MethodName, English, isShow: fineIsShow);
            if (datas.Count() == 0) { pageNum += 1; return; }
            foreach (var item in datas) {
                if (DataList.Contains(item))
                    continue;

                DataList.Add(item);
                if (DataList.Count >= 10) {
                    DataList.RemoveAt(0);
                }
            }

            控制显示();

        }

        /// <summary>
        /// 控制侧边栏开关
        /// </summary>
        private void IsPaneOpenMethod()
        {
            IsPaneOpen = !IsPaneOpen;
        }

        /// <summary>
        /// 加载文件列表到侧边栏
        /// </summary>
        private void LoadDataPathToList()
        {
            PublicProperty.LoadAllDataFileToDataFilePaths();
        }

        /// <summary>
        /// 每次点击都初始化
        /// </summary>
        private async Task InitialTableList()
        {
            if (DataFilePath is null || DataFilePath.FilePath is null || DataFilePath.FileName is null)
                return;

            if (string.IsNullOrWhiteSpace(DataFilePath.FileName) || string.IsNullOrWhiteSpace(DataFilePath.FilePath))
                return;

            pageNum = 0;

            initTextOrderBy();

            DataList.Clear();
            await _SQLiteExtract.CreateDatabaseAsync(DataFilePath.FileName);
            PreLoadData[] data = await _SQLiteExtract.GetDataAsync(0, 10);
            foreach (var item in data) {
                DataList.Add(item);
            }
            IsPaneOpenMethod();
            控制显示();
        }

        /// <summary>
        /// 判断各个字符筛选条件是否为空
        /// </summary>
        private void initTextOrderBy()
        {
            if (ClassName is null) ClassName = "";
            if (MethodName is null) MethodName = "";
            if (MethodName is null) MethodName = "";
        }

        /// <summary>
        /// 保存修改内容
        /// </summary>
        private void EditEndedMethod()
        {
            _SQLiteExtract.AlterAsync(PublicProperty.SaveMode.Chinese, SelectItem);
        }

        /// <summary>
        /// 多选触发
        /// </summary>
        /// <param name="list"></param>
        private void SelectCont(object? list)
        {
            SelectData.Clear();
            if (list is IList datas) {
                foreach (var data in datas) {
                    SelectData.Add((PreLoadData)data);
                }
            }
        }

        /// <summary>
        /// 可见性更改为不可见 / 可见
        /// </summary>
        private async void SetDataIsNoShow()
        {
            await _SQLiteExtract.AlterAsync(PublicProperty.SaveMode.IsShowNo, SelectData.ToArray());
            await GetAssemblyStr_PgDn();
            await GetAssemblyStr_PgUp();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        private async Task Select()
        {
            await GetAssemblyStr_PgUp();
        }

        /// <summary>
        /// 清空查询
        /// </summary>
        private void ClearSelect()
        {
            English = "";
            MethodName = "";
            ClassName = "";
        }

        /// <summary>
        /// 决定显示/不显示
        /// </summary>
        private async void IsShow_()
        {
            fineIsShow = !fineIsShow;
            if (fineIsShow == true)
                IsShow = "显示隐藏";

            if (fineIsShow == false)
                IsShow = "不显示隐藏";

            pageNum = 0;
            await GetAssemblyStr_PgUp();
            控制显示();
        }

        private void 控制显示()
        {
            for (int i = 0; i < DataList.Count; i++) {
                //如果 不显示隐藏  但是数据是隐藏的 删掉
                if (fineIsShow == false && DataList[i].IsShow == 1)
                    DataList.RemoveAt(i);

                //如果 显示隐藏   但是数据是不隐藏的 删掉
                if (fineIsShow == true && DataList[i].IsShow == 0)
                    DataList.RemoveAt(i);
            }
        }

    }
}
