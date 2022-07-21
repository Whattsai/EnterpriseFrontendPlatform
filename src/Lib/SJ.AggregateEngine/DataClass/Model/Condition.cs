using ActionEngine.DataClass.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ActionEngine.DataClass.Model
{
    public class Condition
    {
        [JsonConstructor]
        public Condition() { }

        public Condition(string mapper, TreeNode? tree)
        {
            MapperKey = mapper;
            ConditionTree = tree;
        }

        public string MapperKey { get; set; }

        public TreeNode? ConditionTree { get; set; }

        /// <summary>
        /// 是否成功(執行成功: true, 失敗: false, 未執行: null)
        /// </summary>
        public bool? IsOK { get; set; }

        /// <summary>
        /// Mapping完的物件
        /// </summary>
        public Dictionary<string, object> OutModel { get; set; }
    }


    public class ConditionSettingModel
    {
        public string MapperKey { get; set; }

        public IList<object> ConditionSetting { get; set; }
    }
}
