using System.Text.Json.Serialization;

namespace ActionEngine.DataClass.Model
{
    public class ExecuteAction
    {
        [JsonConstructor]
        public ExecuteAction() { }

        public ExecuteAction(string name, EnumActionType actionType, string key)
        {
            Name = name;
            ActionType = actionType;
            Key = key;
        }

        public string Name { get; set; }

        public EnumActionType ActionType { set; get; }

        public string Key { get; set; }
    }

    public enum EnumActionType
    {
        ApiGet = 11,
    }
}