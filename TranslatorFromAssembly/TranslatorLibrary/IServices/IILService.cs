using TranslatorFormAssembly.Models;

namespace TranslatorFromAssembly.Services.IServices
{
    public interface IILService
    {
        /// <summary>
        /// 提取内容
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        Task<List<PreLoadData>> GetAssemblyILStringAsync(string dllPath);
    }
}
