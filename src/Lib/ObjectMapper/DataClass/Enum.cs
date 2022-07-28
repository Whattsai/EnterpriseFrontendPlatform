using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ.ObjectMapper.DataClass
{
    internal class Enum
    {
        /// <summary>
        /// 映射轉換方式
        /// </summary>
        public enum EnumTransWay
        {
            /// <summary>
            /// 預設指派指標
            /// </summary>
            Default = 0,

            /// <summary>
            /// 建立新Guid
            /// </summary>
            setNewGuid = 11,

            /// <summary>
            /// 設定NULL
            /// </summary>
            SetNull = 101,

            /// <summary>
            /// 新Guid
            /// </summary>
            SetDateTimeByNow = 111,

            /// <summary>
            /// 依照InParameter設定參數
            /// </summary>
            SetByInParameter = 201,

            /// <summary>
            /// List<object> to List<object>
            /// </summary>
            ListObjectToListObject = 205,

            /// <summary>
            /// List to Dictionary
            /// </summary>
            TwoListToDictionary = 211,

        }
    }
}
