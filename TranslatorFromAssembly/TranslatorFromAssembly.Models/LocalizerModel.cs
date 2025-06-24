namespace TranslatorFromAssembly.Models;

[Serializable]
public class LocalizerModel
{
    public string TypeName { get; set; }

    public string MethodName { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }
}
