using SJ.Convert;
using SJ.ObjectMapper.DataClass;
using System.Text.Json;
using static SJ.ObjectMapper.DataClass.Enum;

namespace SJ.ObjectMapper.Module
{
    /// <summary>
        /// 動態物件映射邏輯
        /// </summary>
    public class Mapper
    {
        /// <summary>
        /// 委派時做邏輯，可自行客製修改TransWay邏輯
        /// </summary>
        private Dictionary<EnumTransWay, transMethod> ITrans;

        private dynamic _inModel;

        /// <summary>
        /// 建構值
        /// </summary>
        public Mapper()
        {
            ITrans = new Dictionary<EnumTransWay, transMethod>()
            {
            { EnumTransWay.Default, new transMethod(Default) },
            { EnumTransWay.SetNull, new transMethod(setNull) },
            { EnumTransWay.TwoListToDictionary, new transMethod(twoListToDictionary) },
            { EnumTransWay.SetDateTimeByNow, new transMethod(SetDateTimeByNow) },
            { EnumTransWay.SetByInParameter, new transMethod(SetByInParameter) },
            { EnumTransWay.setNewGuid, new transMethod(setNewGuid) },
            { EnumTransWay.ListObjectToListObject, new transMethod(ListObjectToListObject) },
            };
        }

        private delegate object transMethod(TreeMappingModel map);

        /// <summary>
        /// 樹狀設定結構取的結果
        /// </summary>
        /// <typeparam name="T">要轉換的強行別</typeparam>
        /// <param name="jsonString">jsonString</param>
        /// <param name="inModel">要被轉換的物件</param>
        /// <param name="outModel">outModel原始資料</param>
        /// <returns>T</returns>
        public T GetTreeMapResult<T>(string jsonString, object inModel, object outModel)
        {
            var treeMap = JsonTrans.ToModelOrDefault<Dictionary<string, dynamic>>(jsonString);
            _inModel = inModel;

            var jsonModelT = JsonSerializer.Serialize(treeRecursion(treeMap, DictionaryEx.ToDictionary<object>(outModel)));
            return JsonTrans.ToModelOrDefault<T>(jsonModelT);
        }

        /// <summary>
        /// 樹狀設定方式
        /// </summary>
        /// <param name="treeMapping">樹狀設定檔</param>
        /// <param name="outModel">outModel原始資料</param>
        /// <returns>Dictionary結構的資料</returns>
        private Dictionary<string, object> treeRecursion(Dictionary<string, dynamic> treeMapping, Dictionary<string, dynamic> outModel)
        {
            foreach (var sub in treeMapping)
            {
                if (sub.Value["InParameter"] == null && sub.Value["TransWay"] == null)
                {
                    var tmp = DictionaryEx.ToDictionary<dynamic>(outModel[sub.Key]);

                    if (!outModel.ContainsKey(sub.Key))
                    {
                        outModel.Add(sub.Key, new object());
                    }

                    outModel[sub.Key] = treeRecursion(DictionaryEx.ToDictionary<dynamic>(sub.Value), tmp);
                }
                else
                {
                    EnumTransWay transway = (EnumTransWay)(sub.Value["TransWay"]?? EnumTransWay.Default);
                    var next = sub.Value["Next"] == null ? null : sub.Value["Next"].ToObject<object>();
                    var mapping = new TreeMappingModel(sub.Key, (string)sub.Value["InParameter"], transway, next);
                    if (!outModel.ContainsKey(sub.Key))
                    {
                        outModel.Add(sub.Key, null);
                    }

                    outModel[sub.Key] = new transMethod(ITrans[transway]).Invoke(mapping);
                }
            }

            return outModel;
        }

        private object twoListToDictionary(TreeMappingModel obj)
        {
            var parameterDict = DictionaryEx.ToDictionary<string>(obj.InParameter);

            var keyListTmp = getDataHierarchy(parameterDict["KeyList"], _inModel);
            var valueListTmp = getDataHierarchy(parameterDict["ValueList"], _inModel);

            var keyList = JsonTrans.ToModelOrDefault<List<string>>(keyListTmp?.ToString());
            var valueList = JsonTrans.ToModelOrDefault<List<string>>(valueListTmp?.ToString());

            parameterDict = new Dictionary<string, string>();
            for (int i = 0; i < keyList.Count; i++)
            {
                parameterDict.Add(keyList[i], valueList[i]);
            }

            return parameterDict;
        }

        /// <summary>
        /// 產生新Guid
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private object setNewGuid(TreeMappingModel map)
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// 設NULL
        /// </summary>
        /// <param name="obj">設定物件</param>
        /// <returns>object</returns>
        private object Default(TreeMappingModel obj)
        {
            return getDataHierarchy(obj.InParameter?.ToString(), _inModel);
        }

        /// <summary>
        /// 回傳當下日期時間
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private object SetDateTimeByNow(TreeMappingModel map)
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 依InParameter回傳
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private object SetByInParameter(TreeMappingModel map)
        {
            return map.InParameter;
        }

        /// <summary>
        /// 轉成OAO單值單屬資料型態
        /// </summary>
        /// <param name="obj">物件</param>
        /// <returns>object</returns>
        private object toOAOValueSingle(TreeMappingModel obj)
        {
            var tmp = getDataHierarchy(obj.InParameter?.ToString(), _inModel);

            Dictionary<string, Dictionary<string, object>> ans = new Dictionary<string, Dictionary<string, object>>();
            ans.Add(obj.OutKey, new Dictionary<string, object>());
            ans[obj.OutKey].Add("ContainerKey", obj.OutKey);
            ans[obj.OutKey].Add("ContainerType", "11"); //OAO的Text 欄位型態
            ans[obj.OutKey].Add("ValueSingle", tmp?.ToString());
            ans[obj.OutKey].Add("ComponentDataList", new List<Dictionary<string, object>>());
            ans[obj.OutKey].Add("ValueArray", new List<string>());
            ans[obj.OutKey].Add("ValueDictionary", new Dictionary<string, string>());

            return ans;
        }

        /// <summary>
        /// 將Inkey的資料轉為Oukey的StringToString邏輯
        /// </summary>
        /// <param name="obj">設定物件</param>
        /// <returns>NULL</returns>
        private object? setNull(TreeMappingModel obj)
        {
            return null;
        }

        private object ListObjectToListObject(TreeMappingModel map)
        {
            if (map.Next == null)
            {
                throw new Exception("未設定Array內物件");
            }

            var data = getDataHierarchy(map.InParameter?.ToString(), _inModel);

            return toDictionary<object>(data, map);
        }

        private List<Dictionary<string, T>> toDictionary<T>(dynamic objList, TreeMappingModel map)
        {
            List<Dictionary<string, T>> ans = new List<Dictionary<string, T>>();
            if (objList == null)
            {
                return ans;
            }

            var tmpModel = _inModel;
            foreach (var obj in objList)
            {
                _inModel = obj;
                ans.Add(DictionaryEx.ToDictionary<T>(treeRecursion(DictionaryEx.ToDictionary<dynamic>(map.Next), new Dictionary<string, dynamic>())));
            }

            _inModel = tmpModel;
            return ans;
        }

        /// <summary>
        /// 深度model取值
        /// </summary>
        /// <param name="stringHierarchy">物件路徑</param>
        /// <param name="inModelData">要被轉換的Model資訊</param>
        /// <returns>object</returns>
        private dynamic getDataHierarchy(string stringHierarchy, dynamic inModelData)
        {
            if (!string.IsNullOrEmpty(stringHierarchy))
            {
                var hierarchyList = stringHierarchy.Split('.');
                foreach (var h in hierarchyList)
                {
                    if (inModelData == null)
                    {
                        return null;
                    }

                    var imModelDict = DictionaryEx.ToDictionary<object>(inModelData);

                    if (!imModelDict.ContainsKey(h))
                    {
                        return null;
                    }

                    inModelData = imModelDict[h];

                    // TODO 遇到裡面是Array不知道怎麼處理拉，直接通通丟回去
                    //if (inModelData.GetType().Name == "JArray")
                    //{
                    //    return inModelData;
                    //}
                }
            }
            else if (stringHierarchy == string.Empty)
            {
                if (_inModel.GetType().Name == "JArray")
                {
                    return JsonSerializer.Deserialize<List<dynamic>>(JsonSerializer.Serialize(_inModel));
                }

                return inModelData;
            }
            else
            {
                return null;
            }

            return inModelData;
        }
    }
}
