using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEngine.DataClass.Model.Service
{
    public class OutResponse
    {
        public EnumReturnCode RetrunCode { get; set; }

        public string Message { get; set; }

        public Dictionary<string, object> Detail { get; set; }
    }

    public enum EnumReturnCode
    {
        UnException = 0,
        OK = 1,
        Fail = 2,
    }
}
