using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.AllServices.IServices;

namespace TranslatorLibrary.AllViewModel
{
    public class MainWindowModel : ViewModelBase
    {
        private ViewModelBase _view;
        private IRootViewCut _rootViewCut;
        public ViewModelBase View { get => _view; set => SetProperty(ref _view, value); }

        public MainWindowModel(IRootViewCut rootViewCut)
        {
            _rootViewCut = rootViewCut;
        }
    }
}
