﻿using Newtonsoft.Json;
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

        public StateModel(bool isSuccess, Dictionary<string, object>? responseData)
        {
            IsSuccess = isSuccess;
            ResponseData = responseData;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 回傳的物件(限定物件)
        /// </summary>
        public Dictionary<string, object>? ResponseData { get; set; }
    }
}
