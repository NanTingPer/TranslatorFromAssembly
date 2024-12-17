﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslatorLibrary.ModelClass;
using TranslatorLibrary.Tools;

namespace TranslatorLibrary.AllServices.IServices
{
    public interface ISQLiteExtract<T>
    {
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        Task Delete();

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="preLoadData"></param>
        /// <returns></returns>
        Task AlterAsync(PublicProperty.SaveMode mode , params T[] preLoadData);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="spik"></param>
        /// <param name="taks"></param>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="count"></param>
        /// <param name="save"></param>
        /// <param name="isShow"></param>
        /// <returns></returns>
        Task<T[]> GetDataAsync(int spik, int taks,string className="",string methodName="",string count="",PublicProperty.SaveMode save = PublicProperty.SaveMode.None,bool isShow = false);

        Task<int> PageCountAsync();

        Task AddDataAsync(IList<T> values);

        Task CreateDatabaseAsync(string dataBase);
    }
}
