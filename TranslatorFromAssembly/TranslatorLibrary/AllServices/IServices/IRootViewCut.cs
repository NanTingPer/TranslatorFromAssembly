﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorLibrary.AllServices.IServices
{
    /// <summary>
    /// 用来主页面切换
    /// </summary>
    public interface IRootViewCut
    {
        /// <summary>
        /// 传入ViewModel的名字
        /// </summary>
        /// <param name="viewName"></param>
        void ViewCut(string viewName);
    }

    public static class AllViews
    {
        public const string ListViewModel = nameof(ListViewModel);
        public const string DLLViewModel = nameof(DLLViewModel);
        public const string HjsonEditViewModel = nameof(HjsonEditViewModel);
    }


}
