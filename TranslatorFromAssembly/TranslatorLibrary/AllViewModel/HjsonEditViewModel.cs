using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllViewModel
{
    /// <summary>
    /// Hjson编辑视图
    /// </summary>
    public class HjsonEditViewModel : ViewModelBase
    {
        public HjsonEditViewModel() 
        {
            ListBoxTappedCommand = new RelayCommand(ListBoxTapped);
            ListBoxBackspaceCommand = new RelayCommand(ListBoxBackspace);
            ListBoxSelectInitalizedCommand = new RelayCommand(ListBoxSelectInitalized);
        }

        private FilePathModel _selectItem;

        public ICommand ListBoxTappedCommand { get; }
        public ICommand ListBoxBackspaceCommand { get; }
        public ICommand ListBoxSelectInitalizedCommand { get; }
        //存储文件列表
        public ObservableCollection<FilePathModel> FileList { get; set; } = new ObservableCollection<FilePathModel>();

        //存储显示值
        public ObservableCollection<HjsonModel> ValueList { get; set; } = new ObservableCollection<HjsonModel>();

        //存储上下级
        public StackObservableList<string> DirectoryList { get; set; } = new StackObservableList<string>();

        //ListBox当前选择项
        public FilePathModel SelectItem { get => _selectItem; set => SetProperty(ref _selectItem, value); }

        //主加载
        private void RootLoadToFileList(string path)
        {
            foreach (var item in LoadFilePathToFileList(path))
            {
                FileList.Add(item);
            }
        }


        //加载文件路径到FileList
        private IEnumerable<FilePathModel> LoadFilePathToFileList(string path)
        {
            FileList.Clear();
            if (Directory.Exists(path))
            {
                string[] FilesPtah = Directory.GetFiles(path);
                string[] directors = Directory.GetDirectories(path);

                foreach (string item in directors)
                {
                    FilePathModel fpm = new FilePathModel() { FilePath = item };
                    fpm.FileName = Path.GetFileName(item) ?? "什么";
                    yield return fpm;
                    //FileList.Add(fpm);
                }

                foreach (string item in FilesPtah)
                {
                    FilePathModel fpm = new FilePathModel() { FilePath = item };
                    FilePathModel.SetName(fpm);
                    yield return fpm;
                    //FileList.Add(fpm);
                }
            }
        }

        /// <summary>
        /// 弹出数据
        /// </summary>
        /// <returns></returns>
        private string DirectoryListPop()
        {
            string e = "";
            if (DirectoryList.Count > 1)
            {
                DirectoryList.RemoveAt(0);
                e = DirectoryList[0];
                RootLoadToFileList(e);
            }
            return e;
        }

        /// <summary>
        /// 插入数据
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
        private void ListBoxTapped()
        {
            if (SelectItem is not null)
            {
                if (Directory.Exists(SelectItem.FilePath))
                {
                    DirectoryListPush(SelectItem.FilePath);
                    return;
                }
            }
        }

        //单机退格触发
        private void ListBoxBackspace()
        {
            if (DirectoryList.Count > 0)
            {
                DirectoryListPop();
            }
        }

        //初始化
        private void ListBoxSelectInitalized()
        {
            string initpath = Path.Combine(GetAppFilePath.GetPathAndCreate(), "Hjson");
            RootLoadToFileList(initpath);
            DirectoryListPush(initpath);
        }
    }
}
