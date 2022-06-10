using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlConvertTool.DataClass
{
    public enum TSClassType
    {
        [Description("default")]
        Default = 0,
        [Description("string")]
        String = 1,
        [Description("string[]")]
        StringArray = 2,
        [Description("number")]
        Number = 3,
        [Description("number[]")]
        NumberArray = 4,
        [Description("boolean")]
        Boolean = 5,
        [Description("boolean[]")]
        BooleanArray = 6,
        [Description("object")]
        Object = 7,
        [Description("object[]")]
        ObjectArray = 8,

    }
}
