using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.ModelClass;

namespace TranslatorLibrary.AllViewModel
{
    public class ListViewModel : ViewModelBase
    {
        private ListViewAuxModel _auxModel;


        public ObservableCollection<PreLoadData> DataList { get; set; } = new();
        public ListViewModel(ListViewAuxModel listViewAux) 
        {
            _auxModel = listViewAux;
        }
    }
}
