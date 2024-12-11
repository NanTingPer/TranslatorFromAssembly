using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllViewModel
{
    public class SaveViewModel : ViewModelBase
    {
        private IWriteFileService _writeFileService;
        private ISQLiteExtract<PreLoadData> _sQLiteExtract;

        private string myModPath;
        private string tarGetModName;
        private string myModName;
        private DataFilePath listBoxOption;

        public string MyModPath { get => myModPath; set => SetProperty(ref myModPath, value); }
        public string TarGetModName { get => tarGetModName; set => SetProperty(ref tarGetModName, value); }
        public string MyModName { get => myModName; set => SetProperty(ref myModName, value); }

        public DataFilePath ListBoxOption { get => listBoxOption; set => SetProperty(ref listBoxOption, value); }


        public ICommand ClickListOptionCommand { get; }
        public ICommand WriteOutputCommand { get; }
        public ICommand LoadDataPathToListCommand { get; }
        public SaveViewModel(IWriteFileService writeFileService,ISQLiteExtract<PreLoadData> sQLiteExtract) 
        { 
            _writeFileService = writeFileService;
            _sQLiteExtract = sQLiteExtract;

            ClickListOptionCommand = new RelayCommand(ClickListOption);
            WriteOutputCommand = new RelayCommand(WriteOutput);
            LoadDataPathToListCommand = new RelayCommand(LoadDataPathToList);
        }

        private void WriteOutput()
        {
            if(string.IsNullOrWhiteSpace(MyModName) ||
                string.IsNullOrWhiteSpace(TarGetModName) ||
                string.IsNullOrWhiteSpace(MyModPath)) { return; }
            _writeFileService.CreateWriteMap(_sQLiteExtract, listBoxOption.FileName);
            _writeFileService.WriteFile(MyModPath,TarGetModName,MyModName);
        }

        private void ClickListOption()
        {
            TarGetModName = ListBoxOption.FileName;
            MyModPath = "C:\\Users\\23759\\Documents\\My Games\\Terraria\\tModLoader\\ModSources\\";
        }

        private void LoadDataPathToList()
        {
            PublicProperty.LoadAllDataFileToDataFilePaths();
        }
    }
}
