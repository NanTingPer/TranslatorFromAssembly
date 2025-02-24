using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllViewModel
{
    /// <summary>
    /// Hjson编辑视图
    /// </summary>
    public class HjsonEditViewModel : ViewModelBase
    {
        public HjsonEditViewModel(ISQLiteExtract<HjsonModel> sQLiteExtract, IHjsonProcess hjsonProcess)
        {
            SaveFilePathCommand = new RelayCommand(SaveFilePath);
            ListBoxTappedCommand = new RelayCommand(ListBoxTapped);
            ListBoxBackspaceCommand = new RelayCommand(ListBoxBackspace);
            EditEndedMethodCommand = new RelayCommand<object>(EditEndedMethod);
            ListBoxSelectInitalizedCommand = new RelayCommand(ListBoxSelectInitalized);

            _sQLiteExtract = sQLiteExtract;
            _hjsonProcess = hjsonProcess;
        }
        private string _saveFile;
        private FilePathModel _selectItem;
        private HjsonModel _dataGridSelect;
        private IHjsonProcess _hjsonProcess;
        private ISQLiteExtract<HjsonModel> _sQLiteExtract;

        public ICommand SaveFilePathCommand { get; }
        public ICommand ListBoxTappedCommand { get; }
        public ICommand EditEndedMethodCommand { get; }
        public ICommand ListBoxBackspaceCommand { get; }
        public ICommand ListBoxSelectInitalizedCommand { get; }
        //存储上下级
        public StackObservableList<string> DirectoryList { get; set; } = new StackObservableList<string>();

        //存储显示值
        public ObservableCollection<HjsonModel> ValueList { get; set; } = new ObservableCollection<HjsonModel>();

        //存储文件列表
        public ObservableCollection<FilePathModel> FileList { get; set; } = new ObservableCollection<FilePathModel>();




        //保存位置
        public string SaveFile { get => _saveFile; set => SetProperty(ref _saveFile, value); }
        //ListBox当前选择项
        public FilePathModel SelectItem { get => _selectItem; set => SetProperty(ref _selectItem, value); }
        //DataGrid当前选中项
        public HjsonModel DataGridSelect { get => _dataGridSelect; set => SetProperty(ref _dataGridSelect, value); }


        //侧边目录主加载
        private void RootLoadToFileList(string path)
        {
            foreach (var item in LoadFilePathToFileList(path)) {
                FileList.Add(item);
            }
        }


        //加载文件路径到FileList(侧边栏)
        private IEnumerable<FilePathModel> LoadFilePathToFileList(string path)
        {
            FileList.Clear();
            if (Directory.Exists(path)) {
                string[] FilesPtah = Directory.GetFiles(path);
                string[] directors = Directory.GetDirectories(path);

                foreach (string item in directors) {
                    FilePathModel fpm = new FilePathModel() { FilePath = item };
                    fpm.FileName = Path.GetFileName(item) ?? "什么";
                    yield return fpm;
                    //FileList.Add(fpm);
                }

                foreach (string item in FilesPtah) {
                    if (Path.GetFileName(item) != "conf") {
                        FilePathModel fpm = new FilePathModel() { FilePath = item };
                        FilePathModel.SetName(fpm);
                        yield return fpm;
                    } else {
                        //读取配置
                        SaveFile = File.ReadAllText(item);
                    }
                    //FileList.Add(fpm);
                }
            }
        }

        /// <summary>
        /// 弹出数据 (侧边栏)
        /// </summary>
        /// <returns></returns>
        private string DirectoryListPop()
        {
            string e = "";
            if (DirectoryList.Count > 1) {
                DirectoryList.RemoveAt(0);
                e = DirectoryList[0];
                RootLoadToFileList(e);
            }
            return e;
        }

        /// <summary>
        /// 插入数据 (侧边栏)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string DirectoryListPush(string path)
        {
            DirectoryList.Push(path);
            RootLoadToFileList(path);
            return path;
        }

        //单机ListBox选项触发
        //这里会进行数据载入逻辑
        private async void ListBoxTapped()
        {
            await _sQLiteExtract.ColseDatabaseAsync();

            if (SelectItem is not null) {
                if (Directory.Exists(SelectItem.FilePath)) {
                    DirectoryListPush(SelectItem.FilePath);
                    return;
                }
                if (File.Exists(SelectItem.FilePath)) {
                    await _sQLiteExtract.CreateDatabaseAsync(SelectItem.FileName);
                    ValueList.Clear();
                    LoadDataToValueList();
                }
            }
        }

        //单机退格触发(侧边栏)
        private void ListBoxBackspace()
        {
            if (DirectoryList.Count > 0) {
                DirectoryListPop();
            }
        }

        //初始化(侧边栏显示)
        private void ListBoxSelectInitalized()
        {
            string initpath = Path.Combine(GetAppFilePath.GetPathAndCreate(), "Hjson");
            RootLoadToFileList(initpath);
            DirectoryListPush(initpath);
        }

        private async void LoadDataToValueList()
        {
            await foreach (var item in GetData()) {
                ValueList.Add(item);
            }
        }

        private async IAsyncEnumerable<HjsonModel> GetData()
        {
            HjsonModel[] hjsonData = await _sQLiteExtract.GetDataAsync(0, 10);
            foreach (HjsonModel data in hjsonData) {
                yield return data;
            }
        }

        private async void EditEndedMethod(object? r)
        {
            await _sQLiteExtract.AlterAsync(PublicProperty.SaveMode.Chinese, DataGridSelect);
            string tempFilePath = Path.Combine(SaveFile, SelectItem.FileName);
            if (!string.IsNullOrEmpty(SaveFile)) {
                await _hjsonProcess.SaveHjsonAsync(tempFilePath, _sQLiteExtract);
            }
        }

        private void SaveFilePath()
        {
            string path = DirectoryList[0];
            string confPath = Path.Combine(path, "conf");
            if (File.Exists(confPath)) {
                File.WriteAllText(confPath, SaveFile);
            } else {
                File.Create(confPath).Close();
                File.WriteAllText(confPath, SaveFile);
            }
        }
    }
}
