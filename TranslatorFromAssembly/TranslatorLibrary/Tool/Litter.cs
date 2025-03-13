using TranslatorFormAssembly.Models;
using TranslatorFromAssembly.Services.IServices;

namespace TranslatorLibrary.Tools
{
    /// <summary>
    /// 由于 SQLiteService获取的数据是杂乱的 需要进行处理
    /// </summary>
    public class Litter
    {
        public static async Task<string> TranEnglish(string URL, ISQLiteService _sqliteService)
        {
            return string.Empty;
            //if (_sqliteService.ExesitDatabase == true)  return string.Empty;
            //IEnumerable<DatabaseModle> works = await _sqliteService.GetData(URL);
            //StringBuilder stb = new StringBuilder();
            //foreach (DatabaseModle work in works)
            //{
            //    if (work is not null)
            //    {
            //        string translation = work.Translation;
            //        if (translation.Split(".").Length <= 1)
            //        {
            //            stb.Append(translation.Trim().Split(";")[0].Split("；")[0].Split(",")[0].Split("\n")[0].Replace("\n", ""));
            //        }
            //        else
            //        {
            //            stb.Append(translation.Split(".")[1].Trim().Split(";")[0].Split("；")[0].Split(",")[0].Split("\n")[0].Replace("\n", ""));
            //        }
            //    }

            //}
            //if (string.IsNullOrWhiteSpace(stb.ToString()))
            //    return string.Empty;
            //return stb.ToString();
        }

        /// <summary>
        /// 判断两个PreLoadData对象是否相等
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static bool PreLoadDataIsEqual(PreLoadData p1, PreLoadData p2)
        {
            if (p1 == null && p2 == null)
                return true;

            if (p1 == null || p2 == null)
                return false;

            if (p1.ModName == p2.ModName &&
                p1.ClassName == p2.ClassName &&
                p1.MethodName == p2.MethodName &&
                p1.English == p2.English)
                return true;

            return false;
        }
    }
}
