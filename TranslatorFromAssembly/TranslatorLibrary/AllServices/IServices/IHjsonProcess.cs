using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.AllServices.IServices
{
    /// <summary>
    /// 用来处理Hjson
    /// </summary>
    public interface IHjsonProcess
    {
        /// <summary>
        /// 加载数据并存入数据库 传入文件全路径
        /// </summary>
        /// <param name="path"> 全路径 </param>
        /// <returns></returns>
        Task LoadHjsonAsync(string path);

        Task SaveHjsonAsync(string path);
    }
}
