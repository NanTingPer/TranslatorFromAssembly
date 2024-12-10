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

        public class PublicDataList : ObservableCollection<PreLoadData>
        { 
        }
    }
}
