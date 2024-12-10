using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.AllServices.Services;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllViewModel
{
    public class ListViewModel : ViewModelBase
    {

        /// <summary>
        /// 单页数据大小
        /// </summary>
        private int pageSize = 10;

        /// <summary>
        /// 第几页
        /// </summary>
        private int pageNum = 0;

        private int pageCount = 0;

        private int _paneSize = 200;

        private bool _isPaneOpen;

        private bool _oneLoad = false;

        private ISQLiteExtract<PreLoadData> _SQLiteExtract;

        public ICommand GetAssemblyStr_PgDnCommand { get; }
        public ICommand GetAssemblyStr_PgUpCommand { get; }
        public ICommand IsPaneOpenMethodCommand { get; }


        public int PaneSize { get => _paneSize; set => SetProperty(ref _paneSize, value); }
        public bool IsPaneOpen { get => _isPaneOpen; set => SetProperty(ref _isPaneOpen, value); }
        public ObservableCollection<PreLoadData> DataList { get; set; } = PublicProperty.DataList;
        public ListViewModel(ISQLiteExtract<PreLoadData> sQLiteExtract) 
        {
            _SQLiteExtract = sQLiteExtract;
            GetAssemblyStr_PgDnCommand = new AsyncRelayCommand(GetAssemblyStr_PgDn);
            GetAssemblyStr_PgUpCommand = new AsyncRelayCommand(GetAssemblyStr_PgUp);
            IsPaneOpenMethodCommand = new RelayCommand(IsPaneOpenMethod);
        }


        /// <summary>
        /// 用来加载ILService提出的项目 到显示List中 从数据库加载 下一页
        /// </summary>
        private async Task GetAssemblyStr_PgDn()
        {
            pageCount = await _SQLiteExtract.PageCount();
            if (++pageNum * pageSize >= pageCount)
            {
                pageNum = pageCount / pageSize - 1;
            }
            PreLoadData[] datas = await _SQLiteExtract.GetData((pageNum * pageSize), pageSize);
            if (datas.Count() <= 0)
                return;

            foreach (var item in datas)
            {
                DataList.Add(item);
                DataList.RemoveAt(0);
            }
        }

        /// <summary>
        /// 用来加载ILService提出的项目 到显示List中 从数据库加载 上一页
        /// </summary>
        private async Task GetAssemblyStr_PgUp()
        {
            if (--pageNum < 0)
            {
                pageNum = 0;
            }

            PreLoadData[] datas = await _SQLiteExtract.GetData((pageNum * pageSize), pageSize);
            if (datas.Count() == 0)
                return;
            foreach (var item in datas)
            {
                DataList.Add(item);
                DataList.RemoveAt(0);
            }
        }

        /// <summary>
        /// 控制侧边栏开关
        /// </summary>
        private void IsPaneOpenMethod()
        {
            IsPaneOpen = !IsPaneOpen;
        }

    }
}
