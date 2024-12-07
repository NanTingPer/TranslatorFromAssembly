using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.AllServices.SQLiteServices;

namespace TranslatorLibrary.AllViewModel
{
    public class MainViewModel
    {
        private ISQLiteService _sqliteService;
        public MainViewModel(ISQLiteService sqliteService) 
        {
            _sqliteService = sqliteService;
        }
    }
}
