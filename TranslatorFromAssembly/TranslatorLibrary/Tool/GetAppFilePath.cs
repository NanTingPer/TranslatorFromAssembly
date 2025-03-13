namespace TranslatorLibrary.Tools
{
    public class GetAppFilePath
    {
        private static string _appFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TranslatorFromAssembly");

        /// <summary>
        /// 获取应用存储位置 不会创建文件夹
        /// </summary>
        /// <returns></returns>
        public static string GetPath()
        {
            return _appFilePath;
        }

        /// <summary>
        /// 获取应用存储位置 会创建文件夹
        /// </summary>
        /// <returns></returns>
        public static string GetPathAndCreate()
        {

            if (Directory.Exists(_appFilePath) == false) Directory.CreateDirectory(_appFilePath);
            return _appFilePath;
        }

        /// <summary>
        /// <para>获取该文件应该被存储的位置</para>
        /// <para>选择是否创建文件夹</para>
        /// <para>1创建 2不创建</para>
        /// </summary>
        /// <param name="fileName"> 目标文件名 </param>
        /// <param name="mode"> 1创建 2创文件夹 3不创建 </param>
        /// <returns></returns>
        public static string GetPathAndCreate(string fileName, int mode = 1)
        {
            string filePath = Path.Combine(_appFilePath, fileName);
            switch (mode) {
                case 1:
                    GetPathAndCreate();
                    File.Create(filePath).Close();
                    return filePath;
                case 2:
                    GetPathAndCreate();
                    return filePath;
                default:
                    return filePath;
            }
        }

        /// <summary>
        /// 删除全部应用数据
        /// </summary>
        /// <returns>true成功，false失败</returns>
        public static bool DeleteAppFile()
        {
            try {
                Directory.Delete(_appFilePath);
            } catch {
                return false;
            }
            return true;
        }
    }
}
