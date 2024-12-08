using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCollections = Mono.Collections.Generic.Collection<Mono.Cecil.Cil.Instruction>;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;
using System.Reflection.Emit;
using MMethodBody = Mono.Cecil.Cil.MethodBody;
using MOpenCodes = Mono.Cecil.Cil.OpCodes;
using TranslatorLibrary.Tools;

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
            IEnumerable<TypeDefinition> types = DLLModule.GetTypes();
            foreach (var type in types)
            {
                IEnumerable<MethodDefinition> methods = type.GetMethods();
                foreach (var method in methods)
                {
                    MMethodBody methodBody  = method.Body;
                    if (methodBody is not null)
                    {
                        MCollections ilALL = methodBody.Instructions;
                        foreach (var il in ilALL)
                        {
                            if (il.OpCode.Equals(MOpenCodes.Ldstr))
                            {
                                string english = il.Operand.ToString();
                                if (!string.IsNullOrWhiteSpace(english))
                                {
                                    string chinese = await Litter.TranEnglish(english, _sqliteService);
                                    TempList.Add(new PreLoadData()
                                    {
                                        Id = tempNume++,
                                        English = english,
                                        ModName = ModName,
                                        MethodName = method.Name,
                                        ClassName = type.Name,
                                        AutoChinese = chinese
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return TempList;
        }
    }
}
