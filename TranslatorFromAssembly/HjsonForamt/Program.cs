using Hjson;
using System.Text;
using System.Text.Json;
using TranslatorLibrary.ModelClass;
using static TranslatorLibrary.Tools.PublicProperty;

namespace HjsonForamt
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var datas = TranslatorFromAssembly.ServiceLocator.GetThis.ISQLiteExtractPreData!.GetDataAsync(0,0,save: SaveMode.All).Result;
            //var sb = new StringBuilder();
            //foreach (var pd in datas) {
            //    sb.Append(JsonSerializer.Serialize(pd));
            //}

            //var jsonValue = HjsonValue.Parse(sb.ToString());
            //jsonValue.Save("C:/Users/23759/Documents/My Games/Terraria/tModLoader/ModSources/LibTest/Localization/BACK_Mods.LibTest.hjson", Stringify.Hjson);


            //string allStr = File.ReadAllText("C:/Users/23759/Documents/My Games/Terraria/tModLoader/ModSources/LibTest/Localization/en-US_Mods.LibTest.hjson");
            //JsonValue jsv = HjsonValue.Parse(allStr);
            //Console.WriteLine("Hello, World!");
        }
    }
}
