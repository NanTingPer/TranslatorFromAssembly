using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorFromAssembly.AllService;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.AllServices.Services;
using TranslatorLibrary.AllServices.Services.SQLiteServices;
using TranslatorLibrary.AllViewModel;
using TranslatorLibrary.ModelClass;

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

        public IILService IILService => _serviceProvider.GetService<IILService>();

        public ISQLiteExtract<PreLoadData> ISQLiteExtractPreData => _serviceProvider.GetService<ISQLiteExtract<PreLoadData>>();
        public MainViewModel MainViewModel => _serviceProvider.GetService<MainViewModel>();
        public MainWindowModel MainWindowModel => _serviceProvider.GetService<MainWindowModel>();
        public DataViewModel DataViewModel => _serviceProvider.GetService<DataViewModel>();


        

        public ServiceLocator()
        {
            //SQL访问相关
            _services.AddSingleton<ISQLiteService, SQLiteService>();

            //提取内容的数据库
            _services.AddSingleton<ISQLiteExtract<PreLoadData>, SQLiteExtract>();

            //页面切换
            _services.AddSingleton<IRootViewCut, RootViewCut>();

            //IL
            _services.AddSingleton<IILService, ILService>();

            //MainViewModel
            _services.AddSingleton<MainViewModel>();

            _services.AddSingleton<MainWindowModel>();
            _services.AddSingleton<DataViewModel>();

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
