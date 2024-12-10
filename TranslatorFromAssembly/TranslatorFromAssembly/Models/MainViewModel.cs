using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TranslatorFromAssembly.Views;
using TranslatorLibrary.AllViewModel;

namespace TranslatorFromAssembly.Models
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel() 
        {
            IsPaneOpenCommand = new RelayCommand(IsPaneOpenMethod);
            
        }

        private bool _isPaneOpen;
        private int _PaneSize = 200;

        public ICommand IsPaneOpenCommand { get; }
        public bool IsPaneOpen { get => _isPaneOpen; set => SetProperty(ref _isPaneOpen, value); }
        public int PaneSize { get => _PaneSize; set => SetProperty(ref _PaneSize, value); }
        
        /// <summary>
        /// 控制侧边栏开关
        /// </summary>
        private void IsPaneOpenMethod()
        {
            IsPaneOpen = !IsPaneOpen;
        }


        /// <summary>
        /// 获取主窗体
        /// </summary>
        /// <returns></returns>
        private Window? GetWindow()
        {
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            return null;
        }

        //private void SetPan
    }
}
