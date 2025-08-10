using CommunityToolkit.Mvvm.Input;
using TranslatorFromAssembly.AllService;
using TranslatorFromAssembly.Services.IServices;
using TranslatorFromAssembly.ViewModels;

namespace TranslatorFromAssembly.Models
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IRootViewCut rootViewCut)
        {
            IsPaneOpenCommand = new RelayCommand(IsPaneOpenMethod);
            SetPaneSizeCommand = new RelayCommand(SetPaneSize);
            InitViewCommand = new RelayCommand(InitView);
            ClickOptionCommand = new RelayCommand(ClickOption);
            _rootViewCut = rootViewCut;
            //ContentView = ServiceLocator.DLLViewModel;
        }

        private IRootViewCut _rootViewCut;
        private bool _isPaneOpen;
        private int _PaneSize = 200;
        private bool _oneLoad = false;
        private ViewModelBase _contentView;
        private AllViewInfo _allViewInfo = AllViewInfo.AllViewInfos[0];

        public IRelayCommand IsPaneOpenCommand { get; }
        public IRelayCommand SetPaneSizeCommand { get; }
        public IRelayCommand InitViewCommand { get; }
        public IRelayCommand ClickOptionCommand { get; }
        public bool IsPaneOpen { get => _isPaneOpen; set => SetProperty(ref _isPaneOpen, value); }
        public int PaneSize { get => _PaneSize; set => SetProperty(ref _PaneSize, value); }
        public AllViewInfo AllViewInfo { get => _allViewInfo; set => SetProperty(ref _allViewInfo, value); }
        public ViewModelBase ContentView { get => _contentView; set => SetProperty(ref _contentView, value); }
        public ServiceLocator ServiceLocator = ServiceLocator.GetThis;

        /// <summary>
        /// 控制侧边栏开关
        /// </summary>
        private void IsPaneOpenMethod()
        {
            IsPaneOpen = !IsPaneOpen;
        }


        /// <summary>
        /// 修改PaneSize的大小
        /// </summary>
        private void SetPaneSize()
        {
            if (_oneLoad)
                App.PaneSize = (int)App.GetWindow().Width / 5;
            PaneSize = App.PaneSize;
            _oneLoad = true;
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        private void InitView()
        {
            _rootViewCut.ViewCut(AllViews.DLLViewModel);
        }

        /// <summary>
        /// 点击选项
        /// </summary>
        private void ClickOption()
        {
            _rootViewCut.ViewCut(AllViewInfo.ViewName);
            IsPaneOpenMethod();
        }
    }
}
