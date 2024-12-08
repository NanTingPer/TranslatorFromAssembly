using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.AllServices.IServices;

namespace TranslatorFromAssembly.AllService
{
    /// <summary>
    /// 用来切换主页面的实现类
    /// </summary>
    public class RootViewCut : IRootViewCut
    {
        public void ViewCut(string viewName)
        {
            ServiceLocator.GetThis.MainWindowModel.View = ServiceLocator.GetThis.MainViewModel;
        }
    }
}
