namespace TranslatorLibrary.ModelClass
{
    public class FilePathModel
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

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
