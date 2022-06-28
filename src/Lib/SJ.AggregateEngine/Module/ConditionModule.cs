using ActionEngine.DataClass.DataStructure;
using Newtonsoft.Json;
using SJ.Convert;

namespace ActionEngine.Module
{
    public class ConditionModule
    {
        public ConditionModule()
        {
            index = -1;
            IOperands = new Dictionary<object, OperandMethod>()
            {
                {"=", new OperandMethod(Equal) },
                {"!", new OperandMethod(NotEqual) },
                {"&", new OperandMethod(And) },
                {"|", new OperandMethod(Or) },
                {">", new OperandMethod(BigThan) },
                {"<", new OperandMethod(LessThan) },
            };
        }

        public int index { get; set; }

        private Dictionary<object, OperandMethod> IOperands;

        private delegate bool OperandMethod(object? left, object? right);
        private bool LessThan(object left, object right)
        {
            return Convert.ToDecimal(left) < Convert.ToDecimal(right);
        }

        private bool BigThan(object left, object right)
        {
            return Convert.ToDecimal(left) > Convert.ToDecimal(right);
        }

        private bool Or(object left, object right)
        {
            return (bool)left || (bool)right;
        }

        private bool And(object left, object right)
        {
            return (bool)left && (bool)right;
        }

        private bool NotEqual(object left, object right)
        {
            return !converType(left).Equals(converType(right));
        }

        private bool Equal(object left, object right)
        {
            return converType(left).Equals(converType(right));
        }

        public bool Go(TreeNode? root, Dictionary<string, object> leftModel, Dictionary<string, object> rightModel)
        {
            if (root.left.left == null && root.right.right == null)
            {
                return new OperandMethod(IOperands[root.val]).Invoke(
                    getData(root.left.val, leftModel),
                    getData(root.right.val, rightModel)
                );
            }

            if (root.left.left == null)
            {
                return new OperandMethod(IOperands[root.val]).Invoke(
                    getData(root.left.val, leftModel),
                    getData(Go(root.right, leftModel, rightModel), rightModel)
                    );
            }

            if (root.right.right == null)
            {
                return new OperandMethod(IOperands[root.val]).Invoke(
                    getData(Go(root.left, leftModel, rightModel), leftModel),
                    getData(root.right.val, rightModel)
                    );
            }

            return new OperandMethod(IOperands[root.val]).Invoke(Go(root.left, leftModel, rightModel), Go(root.right, leftModel, rightModel));
        }

        /// <summary>
        /// 從管理介面設計前敘樹結構，透過BuildTree將資料編譯成TreeNode
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public TreeNode? BuildTree(object[] condition, bool start = true)
        {
            if (start)
            {
                index = -1;
            }
            index++;
            if (condition[index] == null || !IOperands.ContainsKey(condition[index]))
            {
                return new TreeNode(condition[index]);
            }

            TreeNode node = new TreeNode(condition[index]);
            node.left = BuildTree(condition, false);
            node.right = BuildTree(condition, false);
            return node;
        }

        private object? getData(object obj, Dictionary<string, object> model)
        {
            switch (obj?.GetType().Name)
            {
                case "String":
                    return getDataHierarchy((string)obj, model);
                default:
                    return obj;
            }
        }

        private dynamic? getDataHierarchy(string stringHierarchy, dynamic inModelData)
        {
            if (!string.IsNullOrEmpty(stringHierarchy))
            {
                var hierarchyList = stringHierarchy.Split('.');
                foreach (var h in hierarchyList)
                {
                    var imModelDict = DictionaryEx.ToDictionary<object>(inModelData);

                    if (!imModelDict.ContainsKey(h))
                    {
                        return stringHierarchy;
                    }

                    inModelData = imModelDict[h];

                    if (inModelData == null)
                    {
                        return inModelData;
                    }
                }
            }
            else if (stringHierarchy == string.Empty)
            {
                if (inModelData.GetType().Name == "JArray")
                {
                    return JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(inModelData));
                }

                return inModelData;
            }
            else
            {
                return null;
            }

            return inModelData;
        }

        private object converType(object obj)
        {
            switch (obj?.GetType().Name)
            {
                case "Int16":
                case "Int32":
                case "Int64":
                case "Float":
                case "Double":
                    return Convert.ToDecimal(obj);
                default:
                    return obj;
            }
        }
    }
}
