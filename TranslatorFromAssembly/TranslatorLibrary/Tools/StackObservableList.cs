using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.Tools
{
    public class StackObservableList<T> : ObservableCollection<T>
    {
        /// <summary>
        /// 插入顶部数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T Push(T value)
        {
            this.Insert(0, value);
            return value;
        }
    }
}
