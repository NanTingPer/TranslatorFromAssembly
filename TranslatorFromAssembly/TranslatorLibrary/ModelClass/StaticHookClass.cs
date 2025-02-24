namespace TranslatorLibrary.ModelClass
{
    /// <summary>
    /// 挂钩子 静态构造内
    /// </summary>
    public class StaticHookClass
    {
        public StaticHookClass(string fileName, string className, string methodName) 
        { 
            FileName = fileName;
            ClassName = className;
            MethodName = methodName;
        }

        public string ClassName { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;
        
        public string ModName;

        /// <summary>
        /// 该模组汉化文件所在的目录
        /// </summary>
        public string DirectoryName;
    }
}
