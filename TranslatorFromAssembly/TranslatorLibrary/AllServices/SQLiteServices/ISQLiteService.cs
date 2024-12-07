using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.AllServices.SQLiteServices
{
    public interface ISQLiteService
    {
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <returns></returns>
        Task Initialization();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        Task GetData();

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <returns></returns>
        Task CreateDatabase();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task DeleteData();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <returns></returns>
        Task InsertData();
    }
}
