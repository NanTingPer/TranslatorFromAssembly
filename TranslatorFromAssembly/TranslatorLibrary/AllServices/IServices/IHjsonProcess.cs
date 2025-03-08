using TranslatorLibrary.ModelClass;

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

        /// <summary>
        /// 保存Hjson到给定路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sQLiteExtract"></param>
        /// <returns></returns>
        Task SaveHjsonAsync(string path,ISQLiteExtract<HjsonModel> sQLiteExtract);
    }
}
