using System.Text.Json.Serialization;

namespace TranslatorFormAssembly.Models
{
    public class SimplePreLoadData : IComparer<SimplePreLoadData>
    {
        [JsonPropertyName("Id")]
        public long Id { get; set; } = 0L;
        [JsonPropertyName("ClassName")]
        public string ClassName { get; set; } = string.Empty;
        [JsonPropertyName("English")]
        public string English { get; set; } = string.Empty;
        [JsonPropertyName("Chinese")]
        public string Chinese { get; set; } = string.Empty;

        [JsonPropertyName("HongKong")]
        public string HongKong { get; set; } = string.Empty;

        [JsonPropertyName("TaiWan")]
        public string TaiWan { get; set; } = string.Empty;

        [JsonPropertyName("PCR")]
        public string PCR { get; set; } = string.Empty;

        [JsonPropertyName("CSOW")]
        public string CSOW { get; set; } = string.Empty;

        public static SimplePreLoadData ToSimplePreLoadData(PreLoadData r)
        {
            return new SimplePreLoadData()
            {
                Chinese = r.Chinese,
                English = r.English,
                ClassName = r.ClassName,
                Id = r.Id,
                TaiWan = r.TaiWan,
                HongKong = r.HongKong,
                PCR = r.PCR,
                CSOW = r.CSOW
            };
        }

        public int Compare(SimplePreLoadData? x, SimplePreLoadData? y)
        {
            if (x is null)
                return 1;
            if (y is null)
                return -1;
            return (int)x.Id - (int)y.Id;
        }
    }

    public class SimplePreLoadDataIComparer : IComparer<SimplePreLoadData>
    {
        public int Compare(SimplePreLoadData? x, SimplePreLoadData? y)
        {
            if (x is null)
                return 1;
            if (y is null)
                return -1;
            return (int)x.Id - (int)y.Id;
        }

    }
}
