using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlConvertTool.DataClass
{
    public class AggrPostInfo
    {
        /// <summary>
        /// DywebInitial
        /// </summary>
        public AggrPostInfo()
        {
            Perameter = new List<string>();
        }

        public string ExecuteKey { get; set; }

        public List<string> Perameter { get; set; }
    }
}
