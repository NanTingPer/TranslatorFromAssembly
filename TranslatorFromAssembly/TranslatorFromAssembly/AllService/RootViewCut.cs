using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TranslatorFromAssembly.Models;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.AllViewModel;

namespace TranslatorFromAssembly.AllService
{
    /// <summary>
    /// 用来切换主页面的实现类
    /// </summary>
    public class RootViewCut : IRootViewCut
    {
        private ServiceLocator _serviceLocator;
        private MainViewModel? _mainViewModel;
        public RootViewCut() 
        {
            _serviceLocator = ServiceLocator.GetThis;
            
        }
        public void ViewCut(string viewName)
        {
            _mainViewModel ??=  _serviceLocator.MainViewModel;

            PropertyInfo[] properties = typeof(ServiceLocator).GetProperties();
            PropertyInfo view = null;
            foreach (var property in properties)
            {
                if (property.Name.Equals(viewName))
                {
                    view = property;
                    break;
                }
            }
            if(view is not null)
                _mainViewModel.ContentView = (ViewModelBase)view.GetValue(_serviceLocator);

        }
    }

    public class AllViewInfo
    {
        public string ViewName { get; private set; } = string.Empty;
        public string ViewTitle { get; private set; } = string.Empty;
        private AllViewInfo() { }

        private static AllViewInfo DLLViewModel = new AllViewInfo() { ViewName = nameof(DLLViewModel), ViewTitle = "从程序集" };
        private static AllViewInfo ListViewModel = new AllViewInfo() { ViewName = nameof(ListViewModel), ViewTitle = "根据本地" };
        private static AllViewInfo SaveViewModel = new AllViewInfo() { ViewName = nameof(SaveViewModel), ViewTitle = "导出" };
        public static ObservableCollection<AllViewInfo> AllViewInfos { get; private set; } = new ObservableCollection<AllViewInfo>()
        {
            DLLViewModel,ListViewModel,SaveViewModel
        };
    }
}
