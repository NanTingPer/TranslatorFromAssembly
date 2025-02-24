using TranslatorLibrary.ModelClass;

namespace TranslatorLibrary.AllServices.IServices
{
    public interface IWriteFileService
    {
        /// <summary>
        /// 给定模组根目录还有模组名称 开始生成
        /// </summary>
        /// <param name="ModRootPath"> 模组根目录 </param>
        /// <param name="ModName"> 要被汉化的模组名称 </param>
        /// <param name="YouModName">你的模组名称</param>
        void WriteFile(string ModRootPath, string ModName, string MyModName);

        /// <summary>
        /// 构造 WriteMap 需要传入一个dataBase的名称
        /// </summary>
        /// <param name="dataBaseName"></param>
        Task CreateWriteMap(ISQLiteExtract<PreLoadData> sQLiteExtract, string dataBaseName);
    }
}
