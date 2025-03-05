namespace TranslatorLibrary.ModelClass
{
    public class 英汉台港
    {
        public string 英文 { get; set; } = string.Empty;
        public string 中文 { get; set; } = string.Empty;
        public string 台湾 { get; set; } = string.Empty;
        public string 香港 { get; set; } = string.Empty;
        public long id { get; set; }

        public 英汉台港(string 英文, string 中文, string 台湾, string 香港, long id)
        {
            this.英文 = 英文;
            this.中文 = 中文;
            this.台湾 = 台湾;
            this.香港 = 香港;
            this.id = id;
        }

        public static 英汉台港 Create(string 英文, string 中文, string 台湾, string 香港, long id)
        {
            return new 英汉台港(英文, 中文, 台湾, 香港, id);
        }
    }
}
