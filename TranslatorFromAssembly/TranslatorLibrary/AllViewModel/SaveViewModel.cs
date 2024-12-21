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
        public ICommand SaveToSwitchCommand { get; }
        public SaveViewModel(IWriteFileService writeFileService,ISQLiteExtract<PreLoadData> sQLiteExtract) 
        { 
            _writeFileService = writeFileService;
            _sQLiteExtract = sQLiteExtract;

            ClickListOptionCommand = new RelayCommand(ClickListOption);
            WriteOutputCommand = new AsyncRelayCommand(WriteOutput);
            LoadDataPathToListCommand = new RelayCommand(LoadDataPathToList);
            SaveToSwitchCommand = new RelayCommand(SaveToSwitch);
        }

        private async Task WriteOutput()
        {
            if(string.IsNullOrWhiteSpace(MyModName) ||
                string.IsNullOrWhiteSpace(TarGetModName) ||
                string.IsNullOrWhiteSpace(MyModPath)) { return; }
            await _writeFileService.CreateWriteMap(_sQLiteExtract, listBoxOption.FileName);
            _writeFileService.WriteFile(MyModPath,TarGetModName,MyModName);
        }

        private void ClickListOption()
        {
            _sQLiteExtract.CreateDatabaseAsync(ListBoxOption.FileName);
            TarGetModName = ListBoxOption.FileName;
            MyModPath = "C:\\Users\\23759\\Documents\\My Games\\Terraria\\tModLoader\\ModSources\\";
        }

        private void LoadDataPathToList()
        {
            PublicProperty.LoadAllDataFileToDataFilePaths();
        }

        //保持到临时文件
        private async void SaveToSwitch()
        {
            string savePath = Path.Combine(Path.GetTempPath(),TarGetModName+".txt");

            await Task.Run(async () => 
            {
                PreLoadData[] preLoadDatas = await _sQLiteExtract.GetDataAsync(0, 0, save: PublicProperty.SaveMode.All);
                using (FileStream file = new FileStream(savePath, FileMode.Create))
                {
                    file.Write(StrToBytes("switch (str)"));
                    file.Write(StrToBytes("{"));
               
                    foreach (var item in preLoadDatas)
                    {
                        file.Write(StrToBytes($"\tcase \"{item.English}\":"));
                        file.Write(StrToBytes($"\t\treturn \"{item.Chinese}\";"));
                        file.Flush();
                    }
                    file.Write(StrToBytes("\tdefault: return str;"));
                    file.Write(StrToBytes("}"));
                    file.Flush();
                }
            });

        }

        private static byte[] StrToBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str + "\r\n");
        }
    }
}
