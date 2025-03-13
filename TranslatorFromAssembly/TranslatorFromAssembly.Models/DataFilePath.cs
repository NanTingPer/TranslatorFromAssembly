namespace TranslatorFormAssembly.Models 
{ 
    /// <summary>
    /// 目录列表类型
    /// <para>FilePath用来给SQLit开启数据库</para>
    /// <para>FileName用来显示</para>
    /// </summary>
    public class DataFilePath
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            DataFilePath? e;
            try {
                e = (DataFilePath?)obj;
            } catch {
                return false;
            }
            if (e is null) return true;
            if (FilePath == e.FilePath && FileName == e.FileName) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
