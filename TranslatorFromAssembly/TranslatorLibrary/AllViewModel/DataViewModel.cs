using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TranslatorLibrary.AllServices.IServices;
using TranslatorLibrary.ModelClass;

namespace TranslatorLibrary.AllViewModel
{
    public class DataViewModel : ViewModelBase
    {
        private ISQLiteExtract<PreLoadData> _sQLiteExtract;

        /// <summary>
        /// 跳过
        /// </summary>
        private int skip = 0;
        private int take = 10;
        /// <summary>
        /// 存储需要显示的数据
        /// </summary>
        public ObservableCollection<PreLoadData> ViewDataList { get; set; } = new();


        public ICommand GetDataToViewData_PgDnCommand { get; }
        public DataViewModel(ISQLiteExtract<PreLoadData> sQLiteExtract)
        {
            _sQLiteExtract = sQLiteExtract;

            GetDataToViewData_PgDnCommand = new AsyncRelayCommand(GetDataToViewData_PgDn);
        }

        /// <summary>
        /// 获取数据到ViewData 下一页
        /// </summary>
        private async Task GetDataToViewData_PgDn()
        {
            //防止数据为空 无法插入
            if(ViewDataList.Count == 0)
            {
                for(int i = 0; i< take;i++)
                {
                    ViewDataList.Add(new PreLoadData());
                }
            }
            PreLoadData[] datas = await _sQLiteExtract.GetData(skip, take);

            //加入一条删除一条可以不会闪屏
            foreach (PreLoadData data in datas)
            {
                ViewDataList.Add(data);
                ViewDataList.RemoveAt(0);
            }
            skip += 10;
        }

        private async Task GetDataToViewData_PgUp()
        {
            if(skip - 10 >= 0)
            {
                PreLoadData[] datas = await _sQLiteExtract.GetData(skip, take);
                foreach (PreLoadData data in datas)
                {
                    ViewDataList.Add(data);
                    ViewDataList.RemoveAt(0);
                }
                skip -= 10;
            }
        }

       
    }
}
