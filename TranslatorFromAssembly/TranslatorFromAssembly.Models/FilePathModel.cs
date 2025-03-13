namespace TranslatorFormAssembly.Models
{
    /// <summary>
    /// 文件路径模型类，包含文件路径和文件名称
    /// </summary>
    public class FilePathModel
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 根据传入对象的路径, 设置文件FilePathModel的名字
        /// </summary>
        /// <param name="fpm"></param>
        public static void SetName(FilePathModel fpm)
        {
            fpm.FileName = Path.GetFileName(fpm.FilePath);
        }

        public FilePathModel Clone(FilePathModel filePathModel)
        {
            return new FilePathModel() { FileName = filePathModel.FileName, FilePath = filePathModel.FilePath };
        }
    }
}
