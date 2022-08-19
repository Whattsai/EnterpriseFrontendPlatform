using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SJ.ObjectMapper.DataClass.Enum;

namespace SJ.ObjectMapper.DataClass
{
    /// <summary>
    /// 物件映射設定檔Model
    /// </summary>
    internal class TreeMappingModel
    {
        /// <summary>
        /// 建構值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="inParameter">inParameter</param>
        /// <param name="transWay">transWay</param>
        public TreeMappingModel(string key, string inParameter, EnumTransWay transWay, object next)
        {
            this.OutKey = key;
            this.InParameter = inParameter;
            this.TransWay = transWay;
            this.Next = next;
        }

        /// <summary>
        /// 輸出object屬性名稱
        /// </summary>
        public string OutKey { get; set; }

        /// <summary>
        /// 輸入object對應參數
        /// </summary>
        public object InParameter { get; set; }

        /// <summary>
        /// 映射轉換方式
        /// </summary>
        public EnumTransWay TransWay { get; set; }


        public object Next { get; set; }
    }
}