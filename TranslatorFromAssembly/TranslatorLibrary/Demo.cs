//using FargoCP.Systems;
//using System.Collections.Generic;
//using Terraria.ModLoader;
//namespace FargoCP.Expmale
//{
//    internal class GodMode_Lang : ModSystem
//    {
//        //创建空类 类的名称为模组的内部名称
//        private class GodMode { }
//        //必须写 确保在改模组之前本类被加载从而报错
//        [ExtendsFromMod("GodMode"), JITWhenModsEnabled("GodMode")]
//        //ForL是自定义的                         上面那个空类的名称,类名是什么就传什么
//        private class ForL : ForceLocalizeSystem<GodMode, ForL>
//        {
//        }
//        //在加载的最后阶段进行汉化注入
//        public override void PostSetupContent()
//        {
//            //判断这个模组有没有被加载 "GodMode" => 这个是模组名称
//            if (ModLoader.TryGetMod("GodMode", out var mod))
//            {
//                //上面写的ForL这里就用ForL调用
//                //GodMode.World 是要汉化的内容在模组的哪个类里面
//                //PostDrawFullscrreenMap 是要汉化的内容在这个类的哪个方法里面
//                //有其他类的内容要汉化就加一个这个
//                ForL.LocalizeByTypeFullName("GodMode.World", "PostDrawFullscreenMap", new()
//                {
//                    {"Release to teleport","右键以传送"},
//                });
//                //例如 在XXXX.XXX类下的ZZZ方法里面有一条硬编码"Hello"
//                /*
//                 ForL.LocalizeByTypeFullName("XXXX.XXX","ZZZ", new ()
//                {
//                    {"Hello","你好"},
//                }); 
                
//                */
//                base.Load();
//            }
//            base.PostSetupContent();
//        }
//    }
//}