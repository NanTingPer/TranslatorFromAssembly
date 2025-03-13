using Avalonia.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TranslatorFormAssembly.Models;
using TranslatorFromAssembly.Services.IServices;

namespace TranslatorFromAssembly.ViewModels
{
    public class HjsonViewModel : ViewModelBase
    {
        public HjsonViewModel(ISQLiteExtract<HjsonModel> sQLite, IHjsonProcess hjsonProcess, IRootViewCut rootViewCut)
        {
            FileDragInCommand = new RelayCommand<object?>(FileDragIn);
            _sQLiteExtract = sQLite;
            _hjsonProcess = hjsonProcess;
            _rootViewCut = rootViewCut;
        }

        private ISQLiteExtract<HjsonModel> _sQLiteExtract;
        private IHjsonProcess _hjsonProcess;
        private IRootViewCut _rootViewCut;


        /// <summary>
        /// 存储拖入的文件
        /// </summary>
        public ObservableCollection<FilePathModel> FilePath { get; set; } = new();


        public ICommand FileDragInCommand { get; }


        public async void FileDragIn(object? e)
        {
            if (e is DragEventArgs value) {
                var list = value.Data.GetFiles();
                foreach (var item in list) {
                    var path = item.Path;
                    var pathstring = path.LocalPath;
                    if (File.Exists(pathstring) && Path.GetFileName(pathstring).EndsWith(".hjson")) {
                        FilePathModel fpm = new FilePathModel() { FilePath = pathstring };
                        FilePathModel.SetName(fpm);
                        FilePath.Add(fpm);
                    }
                }
            }
            await SetDataToSQLite();
            _rootViewCut.ViewCut(AllViews.HjsonEditViewModel);
        }

        //加载数据并存入数据库
        private async Task SetDataToSQLite()
        {
            if (FilePath.Count > 0) {
                foreach (var item in FilePath) {
                    await _hjsonProcess.LoadHjsonAsync(item.FilePath);
                }
                FilePath.Clear();
            }
        }
    }
}
