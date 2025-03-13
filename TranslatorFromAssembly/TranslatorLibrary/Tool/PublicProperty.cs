using System.Collections.ObjectModel;
using TranslatorFormAssembly.Models;

namespace TranslatorLibrary.Tools
{
    /// <summary>
    /// 全局类 全局属性
    /// </summary>
    public static class PublicProperty
    {
        /// <summary>
        /// 公共的 用来存储要被输出的
        /// <para>层级</para>
        /// <para>类名</para>
        /// <para>方法名</para>
        /// <para>内容</para>
        /// </summary>
        //public static Dictionary<Tuple<string,string>, List<Tuple<string,string>>> WriteMap = new();


        /// <summary>
        /// 用来存储要被写入文件的全部内容
        /// <para>第一个Kv对中，Key是类名</para>
        /// <para>第二个Kv对中，Key是方法名，List是该方法的全部内容</para>
        /// </summary>
        public static Dictionary<string, Dictionary<string, List<英汉台港>>> WriteMap { get; set; } = new();

        /// <summary>
        /// 公共的 用来存储要被GridData显示的项目
        /// </summary>
        public static ObservableCollection<PreLoadData> DataList { get; set; } = new();

        /// <summary>
        /// 全部可选数据库
        /// </summary>
        public static ObservableCollection<DataFilePath> DataFilePaths { get; set; } = new();

        /// <summary>
        /// 对数据库进行修改时的模式
        /// <para>Chinese 汉化文本更改</para>
        /// <para>IsShowNo 不显示</para>
        /// <para>IsShowYes 显示</para>
        /// <para>Write 输出</para>
        /// </summary>
        public enum SaveMode
        {
            None,
            Chinese,
            IsShowNo,
            IsShowYes,
            Write,
            All,
            ReallAll
        }
        public static async Task LoadAllDataFileToDataFilePaths()
        {
            await Task.Delay(1);
            //var Count = DataFilePaths.Count;
            //var yesOnNo = 0;
            DataFilePaths.Clear();

            string path = GetAppFilePath.GetPath();
            if (!Directory.Exists(path))
                return;
            string[] dataPaths = Directory.GetFiles(path);
            foreach (var dataPath in dataPaths) {
                //if (Count != 0 && yesOnNo < Count)
                //{
                //    DataFilePaths.RemoveAt(0);
                //    yesOnNo++;
                //}
                DataFilePaths.Add(new DataFilePath() { FilePath = dataPath, FileName = Path.GetFileName(dataPath) });
            }

        }

        public class PublicDataList : ObservableCollection<PreLoadData>
        {
        }
    }
}
