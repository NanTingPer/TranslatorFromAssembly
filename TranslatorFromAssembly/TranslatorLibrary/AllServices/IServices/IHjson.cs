using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.AllServices.IServices
{
    public interface IHjson
    {
        Task LoadHjsonAsync(string path);

        Task SaveHjsonAsync(string path);
    }
}
