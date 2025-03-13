using Mono.Cecil;
using Mono.Cecil.Rocks;
using TranslatorFormAssembly.Models;
using MMethodBody = Mono.Cecil.Cil.MethodBody;
using MOpenCodes = Mono.Cecil.Cil.OpCodes;
using MCollections = Mono.Collections.Generic.Collection<Mono.Cecil.Cil.Instruction>;

using TranslatorLibrary.Tools;
using Mono.Cecil.Cil;
using System.Text.RegularExpressions;
using System.Reflection;
using TranslatorFromAssembly.Services.IServices;

namespace TranslatorFromAssembly.Services.Services
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
        public async Task<List<PreLoadData>> GetAssemblyILStringAsync(string dllPath)
        {
            if (!File.Exists(dllPath))
                return null;
            string ModName = Path.GetFileName(dllPath).Split(".")[0];
            long tempNume = 0L;
            List<PreLoadData> TempList = new List<PreLoadData>();
            ModuleDefinition DLLModule = ModuleDefinition.ReadModule(dllPath);
            List<MethodDefinition> AllMethod = [];
            foreach (var type in DLLModule.GetTypes()) {
                AllMethod.AddRange(type.GetMethods().AsEnumerable());
            }
            foreach (var method in AllMethod) {
                TypeDefinition type = method.DeclaringType;
                var count = 0;
                MMethodBody methodBody = method.Body;
                if (methodBody is null)
                    continue;

                foreach (var il in methodBody.Instructions) {
                    count++;
                    if (!il.OpCode.Equals(MOpenCodes.Ldstr))
                        continue;

                    string? english = il.Operand.ToString();

                    if (string.IsNullOrWhiteSpace(english))
                        continue;
                    int isShow = 0;
                    string chinese = await Litter.TranEnglish(english, _sqliteService);

                    var allIL = methodBody.Instructions;

                    if (english == "AntisocialBuff")
                        isShow = 0;

                    if (MethodCount(type, method.Name) ||
                        ExitsIL(/*ilOffset,*/ allIL, count) ||
                        type.Name.Contains("<") ||
                        type.Name.Contains(">") ||
                        ContentValue(english) ||

                        method.Name.Contains("Load") && english.Contains(":") ||
                        method.Name.Contains("Update") && english.Contains(":"))
                        isShow = 1;//show = 1 就是不显示

                    TempList.Add(new PreLoadData(){ Id = tempNume++,English = english,ModName = ModName,MethodName = method.Name,ClassName = type.FullName,AutoChinese = chinese,IsShow = isShow});
                }
            }
            return TempList;
        }

        /// <summary>
        /// 判断此类型是否显示
        /// </summary>
        //private bool ViewTypeBool(TypeDefinition typeD)
        //{
        //    string typeFullName = typeD.FullName;
        //    if (typeFullName.Contains(""))
        //    return 
        //}

        /// <summary>
        /// 判断内容中的值
        /// </summary>
        /// <param name="content"> english </param>
        /// <returns></returns>
        private bool ContentValue(string content)
        {
            if (content.Length <= 2) return true;
            if (content.Contains("/")) return true;
            if (content.StartsWith("_")) return true;
            if (content.StartsWith(".")) return true;
            if (content == "MapEntry") return true;

            #region Boss列表
            //Boss列表
            if (content == "LogMiniBoss") return true;
            if (content == "LogBoss") return true;
            if (content == "LogEvent") return true;
            if (content == "GetBossInfoDictionary") return true;
            if (content.Contains("AddToBossLoot")) return true;
            if (content.Contains("AddToBossCollection")) return true;
            if (content.Contains("AddToBossSpawnItems")) return true;
            if (content.Contains("AddToEventNPCs")) return true;

            //存疑
            if (content.Contains("AddBoss")) return true;
            if (content.Contains("AddMiniBoss")) return true;
            if (content.Contains("AddEvent")) return true;


            /*
             * https://github.com/JavidPack/BossChecklist/blob/4eadcfa39f5e390923c5ee27856fee2d7a429702/BossChecklistIntegrationExample.cs#L67
             */
            if (content == "modSource") return true;
            if (content == "displayName") return true;
            if (content == "progression") return true;
            if (content == "downed") return true;
            if (content == "isBoss") return true;
            if (content == "isMiniboss") return true;
            if (content == "isEvent") return true;
            if (content == "npcIDs") return true;
            if (content == "spawnInfo") return true;
            if (content == "spawnItems") return true;
            if (content == "treasureBag") return true;
            if (content == "relic") return true;
            if (content == "dropRateInfo") return true;
            if (content == "loot") return true;
            if (content == "collectibles") return true;


            #endregion

            var reg = new Regex(@"\d$");
            if (reg.IsMatch(content))
                return true;

            return false;
        }

        /// <summary>
        /// 判断指定类型中的继承和方法名
        /// </summary>
        /// <param name="type"> 类型的Definition </param>
        /// <param name="methodName"> 方法的名称 </param>
        /// <returns></returns>
        private bool MethodCount(TypeDefinition type, string methodName)
        {
            if (methodName.Contains("_")) return true;
            //if (methodName.Contains("AI")) return true;
            if (methodName.Contains("Draw")) return true;
            if (methodName.Contains("ToString")) return true;
            if (methodName.Contains("AddRecipe")) return true;
            //if (methodName.Contains("HitEffect")) return true;
            if (methodName.Contains("LoadWorldData")) return true;
            if (methodName.Contains("SaveWorldData")) return true;


            if (type.BaseType.ToString().Contains("ModPlayer")) {

                if (methodName.Contains("SaveData"))
                    return true;

                if (methodName.Contains("LoadData"))
                    return true;

            }
            return false;
        }

        /// <summary>
        /// offset从零开始计算
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="ILs"></param>
        /// <param name="count"> 第几条 </param>
        /// <returns></returns>
        private bool ExitsIL(/*int offset, */MCollections ILs, int count)
        {
            if (count + 1 < ILs.Count) {
                var opc = ILs[/*offset*/count + 1];
                var opCode = opc.OpCode;
                object? operand = opc.Operand;
                if (opCode == MOpenCodes.Ret) return true;
                if (opCode == MOpenCodes.Stelem_Ref) return true;
                if (opCode == MOpenCodes.Newobj) {
                    if(operand is not null) {
                        bool? str = operand?.ToString()?.Contains("Graphics.Shaders.ArmorShaderData");
                        if (str != null && str.Value) return true;
                    }
                }
                if(operand is not null) {
                    bool? str = operand?.ToString()?.Contains("Terraria.Main::npcChatText");
                    if (str != null && str.Value) return false;
                }

            }

            int num = 0;
            //存储当前第几条
            //int conuts = count;
            for (int i = count; i < ILs.Count; i++) {
                var instruction = ILs[i];
                if (num == 2) break;
                if (instruction.OpCode == MOpenCodes.Call || instruction.OpCode == MOpenCodes.Callvirt) {
                    num++;
                    string? value = instruction?.Operand.ToString();
                    if (value is not null) {
                        if (value.Contains("ModLoader.Mod::Find")) return true;
                        if (value.Contains("ModLoader.NPCShop::")) return true;
                        if (value.Contains("ModLoader.IO.TagCompound")) return true;
                        if (value.Contains("ModLoader.ModContent::Find")) return true;
                        if (value.Contains("ModLoader.ModContent::Request")) return true;
                        if (value.Contains("ModLoader.ModContent::TryFind")) return true;
                        if (value.Contains("ModLoader.ModType::get_Name")) return true;
                        if (value.Contains("ModLoader.ModLoader::GetMod")) return true;
                        if (value.Contains("ModLoader.ModLoader::HasMod")) return true;

                        if (value.Contains("ModLoader.ILocalizedModTypeExtensions::GetLocalization")) return true;
                        if (value.Contains("ModLoader.ILocalizedModTypeExtensions::GetLocalizationKey")) return true;
                        if (value.Contains("ModLoader.ILocalizedModTypeExtensions::GetLocalizedValue")) return true;

                        if (value.Contains("Localization.Language::GetText")) return true;
                        if (value.Contains("Localization.Language::GetTextValue")) return true;
                        if (value.Contains("Graphics.ShaderManager::GetShader")) return true;
                        if (value.Contains("Microsoft.Xna.Framework")) return true;
                        if (value.Contains("Luminance.Core.Graphics")) return true;

                        if (value.Contains("ModLoader.MusicLoader::GetMusicSlot")) return true;
                        if (value.Contains("ModLoader.ModLoader::TryGetMod")) return true;
                        if (value.Contains("Graphics.Effects.EffectManager")) return true;

                        if (value.Contains("Terraria.Audio.SoundStyle")) return true;
                        if (value.Contains("Terraria.Graphics.Shaders.MiscShaderData")) return true;
                        if (value.Contains("Terraria.Recipe::AddIngredient")) return true;
                        if (value.Contains("Terraria.Player::ManageSpecialBiomeVisuals")) return true;
                        if (value.Contains("ReLogic.Content.AssetRepository::Request")) return true;

                        if (value.Contains("System.Type::GetMethod")) return true;
                        if (value.Contains("System.Type::get_Assembly")) return true;
                        if (value.Contains("System.Reflection.Assembly::GetType")) return true;
                        if (value.Contains("System.Collections.Generic.Dictionary")) return true;
                        if (value.Contains("System.Object::GetType")) return true;
                        if (value.Contains("System.Reflection")) return true;


                        //对于穹月的特别优化
                        if (value.Contains("Stellamod.Helpers.LangText")) return true;
                    }
                }
            }

            return false;
        }
    }
}
