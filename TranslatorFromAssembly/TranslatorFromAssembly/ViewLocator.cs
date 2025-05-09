using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using TranslatorFromAssembly.ViewModels;

namespace TranslatorFromAssembly
{
    public class ViewLocator : IDataTemplate
    {

        public Control? Build(object? param)
        {
            if (param is null)
                return null;

            var name2 = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal).Replace("All","").Replace("View","Views");
            var name = name2.Substring(0, name2.Length - 1).Replace("TranslatorLibrary","TranslatorFromAssembly").Replace("Models","Views");
            var type = Type.GetType(name.Replace("Viewss", "Views"));

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
