using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;
using TranslatorFromAssembly.AllService;
using TranslatorFromAssembly.Models;
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
        public ISQLiteService? ISQLiteService => _serviceProvider.GetService<ISQLiteService>();

        /// <summary>
        /// 页面切换
        /// </summary>
        public IRootViewCut? IRootViewCut => _serviceProvider.GetService<IRootViewCut>();

        public IILService? IILService => _serviceProvider.GetService<IILService>();
        public IWriteFileService? IWriteFileService => _serviceProvider.GetService<IWriteFileService>();
        public ISQLiteExtract<PreLoadData>? ISQLiteExtractPreData => _serviceProvider.GetService<ISQLiteExtract<PreLoadData>>();
        public ISQLiteExtract<HjsonModel>? ISQLiteExtractHjsonModel => _serviceProvider.GetService<ISQLiteExtract<HjsonModel>>();
        public IHjsonProcess? IHjsonProcess => _serviceProvider.GetService<IHjsonProcess>();    


        public DLLViewModel? DLLViewModel => _serviceProvider.GetService<DLLViewModel>();
        public MainWindowModel? MainWindowModel => _serviceProvider.GetService<MainWindowModel>();
        public MainViewModel? MainViewModel => _serviceProvider.GetService<MainViewModel>();

        public ListViewModel? ListViewModel => _serviceProvider.GetService<ListViewModel>();
        public SaveViewModel? SaveViewModel => _serviceProvider.GetService<SaveViewModel>();
        public HjsonViewModel? HjsonViewModel => _serviceProvider.GetService<HjsonViewModel>();
        public HjsonEditViewModel? HjsonEditViewModel => _serviceProvider.GetService<HjsonEditViewModel>();




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
            _services.AddSingleton<IWriteFileService, WriteFileService>();


            //Hsjon
            _services.AddSingleton<ISQLiteExtract<HjsonModel>, HjsonSQLiteExtract>();
            _services.AddSingleton<IHjsonProcess, HjsonService>();


            //MainViewModel
            _services.AddSingleton<DLLViewModel>();
            _services.AddSingleton<MainWindowModel>();
            _services.AddSingleton<MainViewModel>();
            _services.AddSingleton<ListViewModel>();
            _services.AddSingleton<SaveViewModel>();
            _services.AddSingleton<HjsonViewModel>();
            _services.AddSingleton<HjsonEditViewModel>();
        

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
