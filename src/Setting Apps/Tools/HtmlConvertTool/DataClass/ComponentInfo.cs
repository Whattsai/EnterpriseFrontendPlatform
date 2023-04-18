using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlConvertTool.DataClass
{
    public class ComponentInfo
    {
        public ComponentInfo()
        {
            DataClassNames = new List<string>();
        }

        public string Name { get; set; }

        public string MainDataClassName { get; set; }
        public List<string> DataClassNames { get; set; }

        public Queue<AggrPostInfo> InitialInfo { get; set; }

        public Dictionary<string, AggrPostInfo> WatchInfo { get; set; }
    }
}
