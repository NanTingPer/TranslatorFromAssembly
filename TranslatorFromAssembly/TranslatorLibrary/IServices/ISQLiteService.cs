using TranslatorFormAssembly.Models;

namespace TranslatorFromAssembly.Services.IServices
{
    public interface ISQLiteService
    {
        bool ExesitDatabase { get; }
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <returns></returns>
        Task Initialization();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DatabaseModle>> GetData(string work);


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
