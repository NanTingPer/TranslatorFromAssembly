using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorFromAssembly.AllService;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.AllServices.SQLiteServices;
using TranslatorLibrary.AllViewModel;

namespace TranslatorFromAssembly
{
    /// <summary>
    /// 依赖注入
    /// </summary>
    public class ServiceLocator
    {
        private static ServiceLocator _getthis;
        //创建依赖注入容器
        private ServiceCollection _services = new ServiceCollection();
        private IServiceProvider _serviceProvider;
       

        /// <summary>
        /// 访问数据库
        /// </summary>
        public ISQLiteService ISQLiteService => _serviceProvider.GetService<ISQLiteService>();

        /// <summary>
        /// 页面切换
        /// </summary>
        public IRootViewCut IRootViewCut => _serviceProvider.GetService<IRootViewCut>();

        public MainViewModel MainViewModel => _serviceProvider.GetService<MainViewModel>();

        public ServiceLocator()
        {
            //SQL访问相关
            _services.AddSingleton<ISQLiteService, SQLiteService>();

            //页面切换
            _services.AddSingleton<IRootViewCut, RootViewCut>();

            //MainViewModel
            _services.AddSingleton<MainViewModel>();

            _serviceProvider = _services.BuildServiceProvider();
        }

        public static ServiceLocator GetThis 
        {
            get
            {
                if (_getthis is null)
                {
                    //从资源中获取指定资源
                    Application.Current.TryGetResource(nameof(ServiceLocator), null, out var value);
                    _getthis = (ServiceLocator)value;
                }

                return _getthis;
            }
            set => _getthis = value;
        }
    }
}
