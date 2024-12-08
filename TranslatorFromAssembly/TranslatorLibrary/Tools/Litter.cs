using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;

namespace TranslatorLibrary.Tools
{
    /// <summary>
    /// 由于 SQLiteService获取的数据是杂乱的 需要进行处理
    /// </summary>
    public class Litter
    {
        public static async Task<string> TranEnglish(string URL,ISQLiteService _sqliteService)
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
    }
}
