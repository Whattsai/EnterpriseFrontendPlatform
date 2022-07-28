using ActionEngine.DataClass.DataStructure;
using ActionEngine.DataClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEngine.DataClass.Interface
{
    internal interface IActionModule<OutT, InT>
    {
        void BeforeExecuteCondition(Condition condition, Dictionary<string, object> request);

        OutT ExecuteAction(ExecuteAction action, InT inModel);

        void AfterExecuteCondition(Condition condition, object request);
    }
}
