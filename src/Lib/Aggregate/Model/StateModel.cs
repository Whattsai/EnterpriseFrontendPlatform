using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aggregate.Model
{
    public class StateModel
    {
        public StateModel()
        {

        }

        public StateModel(bool isSuccess, object responseData)
        {
            IsSuccess = isSuccess;
            ResponseData = responseData;
        }

        /// <summary>
        /// 顯示名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 回傳的物件(限定物件)
        /// </summary>
        public object ResponseData { get; set; }
    }
}
