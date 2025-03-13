using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TranslatorFromAssembly.Models;
using TranslatorFromAssembly.Services.IServices;
using TranslatorFromAssembly.ViewModels;

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
            _mainViewModel ??= _serviceLocator.MainViewModel;//获取主视图

            PropertyInfo[] properties = typeof(ServiceLocator).GetProperties();//获取依赖注入容器全部属性
            PropertyInfo view = null;
            foreach (var property in properties) {
                if (property.Name.Equals(viewName)) {//如果属性名称等于跳转的页面
                    view = property;
                    break;
                }
            }
            if (view is not null)
                _mainViewModel.ContentView = (ViewModelBase)view.GetValue(_serviceLocator);//设置显示页面

            _mainViewModel.AllViewInfo = AllViewInfo.AllViewInfos.FirstOrDefault(f => f.ViewName == viewName);

        }
    }

    public class AllViewInfo
    {
        public string ViewName { get; private set; } = string.Empty;
        public string ViewTitle { get; private set; } = string.Empty;
        private AllViewInfo() { }

        private static AllViewInfo DLLViewModel = new AllViewInfo() { ViewName = nameof(DLLViewModel), ViewTitle = "从程序集提取硬编码" };
        //private static AllViewInfo ListViewModel = new AllViewInfo() { ViewName = nameof(ListViewModel), ViewTitle = "编辑硬编码文件" };
        private static AllViewInfo SaveViewModel = new AllViewInfo() { ViewName = nameof(SaveViewModel), ViewTitle = "导出硬编码" };
        private static AllViewInfo HjsonViewModel = new AllViewInfo() { ViewName = nameof(HjsonViewModel), ViewTitle = "导入Hjson" };
        private static AllViewInfo HjsonEditViewModel = new AllViewInfo() { ViewName = nameof(HjsonEditViewModel), ViewTitle = "编辑Hjson" };
        public static ObservableCollection<AllViewInfo> AllViewInfos { get; private set; } = new ObservableCollection<AllViewInfo>()
        {
            DLLViewModel,/*ListViewModel,*/SaveViewModel,HjsonViewModel,HjsonEditViewModel
        };
    }
}
