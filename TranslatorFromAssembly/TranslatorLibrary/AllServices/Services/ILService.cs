using Mono.Cecil;
using Mono.Cecil.Rocks;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;
using MMethodBody = Mono.Cecil.Cil.MethodBody;
using MOpenCodes = Mono.Cecil.Cil.OpCodes;
using MCollections = Mono.Collections.Generic.Collection<Mono.Cecil.Cil.Instruction>;

using TranslatorLibrary.Tools;
using Mono.Cecil.Cil;

namespace TranslatorLibrary.AllServices.Services
{
    public class ILService : IILService
    {
        private ISQLiteService _sqliteService;
        public ILService(ISQLiteService liteService)
        {
            _sqliteService = liteService;
        }
        /// <summary>
        /// 获取此Assembly内的全部 Ldstr
        /// </summary>
        /// <param name="dllPath"></param>
        /// <returns></returns>
        public async Task<List<PreLoadData>> GetAssemblyILString(string dllPath)
        {
            string ModName = Path.GetFileName(dllPath).Split(".")[0];
            long tempNume = 0L;
            List<PreLoadData> TempList = new List<PreLoadData>();
            ModuleDefinition DLLModule = ModuleDefinition.ReadModule(dllPath);
            foreach (var type in DLLModule.GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    MMethodBody methodBody = method.Body;
                    if (methodBody is null)
                        continue;

                    foreach (var il in methodBody.Instructions)
                    {
                        if (!il.OpCode.Equals(MOpenCodes.Ldstr))
                            continue;

                        string? english = il.Operand.ToString();

                        if (string.IsNullOrWhiteSpace(english))
                            continue;
                        int isShow = 0;
                        string chinese = await Litter.TranEnglish(english, _sqliteService);

                        if (type.Name.Contains("<") || 
                            type.Name.Contains(">") ||
                            english.Contains("/") || 
                            //english.Count(f => f == '.') >= 2 ||
                            //english.StartsWith('.') ||
                            english.StartsWith("_") ||
                            english.Length <= 2 ||
                            english.Contains(".") ||
                            method.Name.Contains("AddRecipe") ||
                            method.Name.Contains("AI") ||
                            method.Name.Contains("Draw") ||
                            method.Name.Contains("HitEffect") ||
                            method.Name.Contains("_") ||
                            method.Name.Contains("ToString") ||
                            (method.Name.Contains("Load") && english.Contains(":")) ||
                            (method.Name.Contains("Update") && english.Contains(":")))
                            isShow = 1;

                        TempList.Add(new PreLoadData()
                        {
                            Id = tempNume++,
                            English = english,
                            ModName = ModName,
                            MethodName = method.Name,
                            ClassName = type.FullName,
                            AutoChinese = chinese,
                            IsShow = isShow
                        });
                    }
                }
            }
            return TempList;
        }


        private void GetILs(MethodDefinition method)
        {
            MCollections Instruction =  method.Body.Instructions;
        }
        private IEnumerable<MethodDefinition> GetMethod(TypeDefinition type)
        {
            return type.GetMethods();
        }

        private IEnumerable<TypeDefinition> GetTypes(ModuleDefinition module)
        {
            return module.GetTypes();
        }
    }
}
