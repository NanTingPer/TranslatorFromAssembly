using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.ModelClass;

namespace TranslatorLibrary.Tools
{
    /// <summary>
    /// 全局类 全局属性
    /// </summary>
    public static class PublicProperty 
    {
        public static ObservableCollection<PreLoadData> DataList { get; set; } = new();

        /// <summary>
        /// 全部可选数据库
        /// </summary>
        public static ObservableCollection<DataFilePath> DataFilePaths { get; set; } = new();

        public enum SaveMode
        {
            Chinese,
            IsShowNo,
            IsShowYes
        }
        public static void LoadAllDataFileToDataFilePaths()
        {
            DataFilePaths.Clear();
            string path = GetAppFilePath.GetPath();
            if (!Directory.Exists(path))
                return;
            string[] dataPaths = Directory.GetFiles(path);
            foreach (var dataPath in dataPaths)
            {
                DataFilePaths.Add(new DataFilePath() { FilePath = dataPath, FileName = Path.GetFileName(dataPath) });
            }

        }

        public class PublicDataList : ObservableCollection<PreLoadData>
        { 
        }
    }
}
