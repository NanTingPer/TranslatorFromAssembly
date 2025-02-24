using System.Collections.ObjectModel;

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
