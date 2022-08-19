using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class EFPapiResponse
    {
        public EFPapiResponse() { }

        public EFPapiResponse(EnumEFPResultCode code, string message, dynamic data)
        {
            ResultCode = code;
            Message = message;
            Data = data;
        }

        public EnumEFPResultCode ResultCode { get; set; }

        public string Message { get; set; }

        public dynamic Data { get; set; }

    }

    public enum EnumEFPResultCode
    {
        Non = 0,
        Success = 1,
        Fail = 2,
    }
}
