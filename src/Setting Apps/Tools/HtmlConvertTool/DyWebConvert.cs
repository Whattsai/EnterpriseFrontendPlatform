using HtmlAgilityPack;
using HtmlConvertTool.DataClass;
using System.Runtime.InteropServices;

namespace HtmlConvertTool
{
    public class DyWebConvert
    {
        HtmlDocument _doc = new HtmlDocument();

        /// <summary>
        /// 建構子
        /// </summary>
        public DyWebConvert()
        {
            //使用預設編碼讀入 HTML
            _doc.Load(AppSettings.LoadHtmlFilePath);
        }

        /// <summary>
        /// 執行轉換
        /// </summary>
        public void ExecuteConvert()
        {
            //取得 dyweb-initial 底下所有 Node
            HtmlNodeCollection htmlNodeCollection = _doc.DocumentNode.SelectNodes($"//*[@dyweb-initial]");

            foreach (var node in htmlNodeCollection)
            {
                //DetailVue  檔名
                string fileName = GetVueName(node);

                //產生DataClass.ts
                var dataClassNames = CreateDataClassInfo(node, out var mainDataClassName);

                //產生InitialData
                var initialInfoQueue = CreateInitialData(node);

                //產生watchData
                var watchInfo = CreateWatchData(node);

                //產生 Detail Vue 檔
                CreateDetailVue(node, fileName);

                //產生Detail Vue檔的Ts內容
                AppendTypeScriptToVue(fileName, dataClassNames, initialInfoQueue, watchInfo, mainDataClassName);
            }

            //產生index.html檔 & App.Vue
            DyWebConvert.CreateIndex(_doc);
        }

        private static string GetVueName(HtmlNode node)
        {
            string nodeString = node.GetAttributeValue($"dyweb-initial", "default");

            return nodeString.Split(",")[0].Replace("'", "");
        }

        /// <summary>
        /// 準備 dyweb-initial、dyweb-selectinitial 觸發方法的資料
        /// </summary>
        /// <param name="node">HtmlNode</param>
        /// <returns></returns>
        private static Queue<AggrPostInfo> CreateInitialData(HtmlNode node)
        {
            Queue<AggrPostInfo> InitialData = new Queue<AggrPostInfo>();

            AggrPostInfo dywebInitialInfo = GetAggrPostParameterByAttribute(node, $"dyweb-initial");

            InitialData.Enqueue(dywebInitialInfo);

            foreach (var selectInitialNode in node.SelectNodes($".//*[@dyweb-selectinitial]"))
            {
                AggrPostInfo selectinitialInfo = GetAggrPostParameterByAttribute(selectInitialNode, $"dyweb-selectinitial");

                InitialData.Enqueue(selectinitialInfo);
            }

            return InitialData;
        }

        /// <summary>
        /// 準備 dy-watch 觸發方法的資料
        /// </summary>
        /// <param name="node">HtmlNode</param>
        /// <returns></returns>
        private static Dictionary<string, AggrPostInfo> CreateWatchData(HtmlNode node)
        {
            Dictionary<string, AggrPostInfo> watchData = new Dictionary<string, AggrPostInfo>();

            var htmlNodes = node.SelectNodes($".//*[@dyweb-watch]");

            foreach (var item in htmlNodes)
            {
                AggrPostInfo dywebInitial = new AggrPostInfo();

                string nodeString = item.GetAttributeValue($"dyweb-watch", "default");

                string key = nodeString.Split(",")[0];

                string aggrPostString = nodeString.Substring(nodeString.IndexOf(',') + 1);

                string[] perameter = aggrPostString.Replace("Aggr_Post(", "").Replace(")", "").Split(",");

                dywebInitial.ExecuteKey = perameter[0];

                for (int i = 1; i < perameter.Length; i++)
                {
                    dywebInitial.Perameter.Add(perameter[i]);
                }

                watchData.Add(key, dywebInitial);
            }

            return watchData;
        }

        /// <summary>
        /// 初始化 DetailVue 檔案
        /// </summary>
        /// <param name="htmlDocument"></param>
        private static void CreateDetailVue(HtmlNode node, string fileName)
        {
            CreateFolderIfNotExist($@"src\components\");

            using (StreamWriter file = new StreamWriter(Path.Combine(AppSettings.OutputFilePath, $@"src\components\{fileName}.vue"), false))
            {
                file.WriteLine("<template>");
                file.WriteLine(node.InnerHtml);
                file.WriteLine("</template>");
            }

        }

        /// <summary>
        /// 準備 DataClass 資料並建立檔案
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mainDataClassName"></param>
        /// <returns></returns>
        private static List<string> CreateDataClassInfo(HtmlNode node, out string mainDataClassName)
        {

            List<DataClassInfo> dataClassInfos = DyWebConvert.AnalyticalNode(node);
            mainDataClassName = dataClassInfos[0].ParentClassName;
            CreateFolderIfNotExist($@"src\dataclass\");

            //參數最前面加【.】才會限縮在子階層內
            //var htmlNodes = node.SelectNodes($".//*[@dyweb-model]");

            //產出DataClass檔
            Dictionary<string, List<DataClassInfo>> typeScriptClassInfo = dataClassInfos.GroupBy(o => o.ParentClassName).ToDictionary(o => o.Key, o => o.ToList());

            foreach (var item in typeScriptClassInfo)
            {
                using (StreamWriter file = new StreamWriter(Path.Combine(AppSettings.OutputFilePath, $@"src\dataclass\{item.Key}.ts"), false))
                {
                    foreach (var objectItem in item.Value.FindAll(o => o.PropertyInfo.Type == TSClassType.Object.Description() || o.PropertyInfo.Type == TSClassType.ObjectArray.Description()))
                    {
                        file.WriteLine($"import {{ {objectItem.PropertyInfo.Name} }} from \"./{ objectItem.PropertyInfo.Name}\";");
                    }

                    file.WriteLine($"export class {item.Key} " + "{");

                    foreach (var dataClass in item.Value)
                    {
                        file.WriteLine(TsPropertyInitialStringCreater(dataClass));
                    }

                    file.WriteLine("}");
                }
            }

            return typeScriptClassInfo.Select(o => o.Key).ToList();
        }

        /// <summary>
        /// 附加 TypeScript 程式碼至 DetailVue 檔案
        /// </summary>
        /// <param name="vueName"></param>
        /// <param name="initialInfos"></param>
        /// <param name="watchData"></param>
        /// <param name="dataClasses"></param>
        /// <param name="mainDataClassName"></param>
        private static void AppendTypeScriptToVue(string vueName, List<string> dataClasses, Queue<AggrPostInfo> initialInfoQueue, Dictionary<string, AggrPostInfo> watchData, string mainDataClassName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Path.Combine(AppSettings.OutputFilePath, $@"src\components\{vueName}.vue"), true))
            {
                file.WriteLine("<script lang='ts'>");
                file.WriteLine("import { watch, reactive, defineComponent } from 'vue';");
                file.WriteLine("import 'reflect-metadata';");
                file.WriteLine("import 'es6-shim';");
                file.WriteLine("import { classToPlain, deserialize, plainToClassFromExist } from 'class-transformer';");
                file.WriteLine("import axios from 'axios';");
                foreach (var item in dataClasses)
                {
                    file.WriteLine($"import {{{ item }}} from '../dataclass/{item}';");
                }

                file.WriteLine("");

                file.WriteLine($"export default defineComponent ({{");
                file.WriteLine("setup() {");

                file.WriteLine("");

                file.WriteLine($"const {mainDataClassName.FirstCharTolower()} = reactive(new {mainDataClassName.FirstCharToUpper()}());");

                file.WriteLine("");

                /** dyweb-Watch */
                foreach (var item in watchData)
                {
                    string postPeram = String.Join(", ", item.Value.Perameter);
                    file.WriteLine($"watch(() =>{item.Key}, (newValue, oldValue) => {{");
                    file.WriteLine($"Aggr_Post({item.Value.ExecuteKey},{postPeram})");
                    file.WriteLine("});");
                }

                file.WriteLine("");

                file.WriteLine("");

                /** Aggr_Post */
                file.WriteLine("const Aggr_Post = (serviceName:string, executeKey: string, ...perameters: string[]) => {");
                file.WriteLine("var jsonstring = \"{ \";");
                file.WriteLine("for (var i = 0; i < perameters.length; i++) {");
                file.WriteLine("if (i > 0) { jsonstring += ',' }");
                file.WriteLine("if (perameters[i].includes('url:')) {");
                file.WriteLine("const paramInfo = perameters[i].replace(\"url: \", \"\").split(':');");
                file.WriteLine("const urlParams = new URLSearchParams(window.location.search);");
                file.WriteLine("const paramValue = urlParams.get(paramInfo[1]);");
                file.WriteLine("jsonstring += '\"' + paramInfo[1] + '\":\"' + paramValue + '\"';");
                file.WriteLine("} else {");
                file.WriteLine($"const key = perameters[i] as keyof {mainDataClassName.FirstCharToUpper()};");
                file.WriteLine($"jsonstring += '\"' + perameters[i] + '\":\"' + {mainDataClassName.FirstCharTolower()}[key] + '\"';");
                file.WriteLine("}}");
                file.WriteLine("jsonstring += \"} \";");
                file.WriteLine("var postData = JSON.parse(jsonstring);");
                file.WriteLine(" const automapp = (jsonData: any) => {");
                file.WriteLine(" const multiJson = classToPlain(jsonData);");
                file.WriteLine($" plainToClassFromExist({mainDataClassName.FirstCharTolower()}, multiJson);");
                file.WriteLine(" }");
                file.WriteLine(" const postRequest = {Service: serviceName, ID: executeKey, Data: postData }");
                file.WriteLine($" axios.post('{AppSettings.AxiosPost}', postRequest)");
                file.WriteLine($" .then((response) => automapp(response.data));");
                file.WriteLine("}");

                file.WriteLine("");

                /** dyweb-Initial & SelectInitial */
                while (initialInfoQueue.Any())
                {
                    AggrPostInfo initialInfo = initialInfoQueue.Dequeue();
                    string postPerameter = String.Join(", ", initialInfo.Perameter);
                    file.WriteLine($"Aggr_Post({initialInfo.ExecuteKey},{postPerameter})");
                    file.WriteLine("");
                }



                file.WriteLine("");

                /** returm */
                file.WriteLine($"return {{ {mainDataClassName.FirstCharTolower()}, Aggr_Post }}");
                file.WriteLine("}");
                file.WriteLine("})");

                file.WriteLine("</script>");
            }
        }

        /// <summary>
        /// 建立 Index.html
        /// </summary>
        /// <param name="htmlDocument"></param>
        private static void CreateIndex(HtmlDocument htmlDocument)
        {
            CreateFolderIfNotExist($@"public\");

            var htmlNode = htmlDocument.GetElementbyId("app");

            CreateAppVue(htmlNode);

            htmlNode.InnerHtml = "";

            using (StreamWriter file = new StreamWriter(Path.Combine(AppSettings.OutputFilePath, $@"public\index.html"), false))
            {
                file.WriteLine(htmlDocument.DocumentNode.InnerHtml);
            }
        }

        /// <summary>
        /// 建立 App.Vue
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <param name="attributeName"></param>oooooo
        private static void CreateAppVue(HtmlNode htmlNode)
        {
            List<string> detailVueNameList = new List<string>();

            //取得每個dyweb-initial節點
            var htmlNodes = htmlNode.SelectNodes($"//*[@{"dyweb-initial"}]");

            foreach (var node in htmlNodes)
            {
                detailVueNameList.Add($"{GetVueName(node)}");
                HtmlNode newChild = HtmlNode.CreateNode($"<{GetVueName(node)} />");
                node.ParentNode.ReplaceChild(newChild, node);
            }

            using (StreamWriter file = new StreamWriter(Path.Combine(AppSettings.OutputFilePath, $@"src\App.vue"), false))
            {
                file.WriteLine("<template>");
                file.WriteLine(htmlNode.InnerHtml);
                file.WriteLine("</template>");
                file.WriteLine("<script lang='ts'>");
                file.WriteLine("import { defineComponent } from 'vue';");

                foreach (var item in detailVueNameList)
                {
                    file.WriteLine($"import {item} from './components/{item}.vue';");
                }

                file.WriteLine("export default defineComponent({");
                file.WriteLine("name: 'App',");
                file.WriteLine("components: {");

                foreach (var item in detailVueNameList)
                {
                    file.WriteLine($"\"{ item}\": {item},");
                }

                file.WriteLine(" },");
                file.WriteLine("})");
                file.WriteLine("</script>");
            }

        }

        /// <summary>
        /// 遞迴解析 HtmlNode 產生 DataClass 資訊
        /// </summary>
        /// <param name="node">HtmlNode</param>
        /// <param name="dataClassInfos">存放DataClassInfo</param>
        /// <param name="vforItemName">vFor的item</param>
        /// <param name="vforItemValue">item實際的值</param>
        /// <returns></returns>
        private static List<DataClassInfo> AnalyticalNode(HtmlNode node, [Optional] List<DataClassInfo> dataClassInfos, [Optional] string vforItemName, [Optional] string vforItemValue)
        {
            if (dataClassInfos == null)
            {
                dataClassInfos = new List<DataClassInfo>();
            }

            string[] classHierarchy;
            foreach (var detailNode in node.ChildNodes)
            {
                TSClassType type = Extension.GetValueFromDescription<TSClassType>(detailNode.GetAttributeValue($"dyweb-model", "default"));

                //處理<tr>標記裡的v-for
                if (detailNode.InnerHtml.Contains("</"))
                {
                    switch (type)
                    {
                        case TSClassType.ObjectArray:
                            GetVforItem(out vforItemName, out vforItemValue, detailNode);
                            AnalyticalClassHierarchy(ref dataClassInfos, type, GetVforClassHierarchy(detailNode));
                            AnalyticalNode(detailNode, dataClassInfos, vforItemName, vforItemValue);
                            break;
                        case TSClassType.StringArray:
                            AnalyticalClassHierarchy(ref dataClassInfos, type, GetVforClassHierarchy(detailNode));
                            AnalyticalNode(detailNode, dataClassInfos);
                            break;
                        default:
                            AnalyticalNode(detailNode, dataClassInfos);
                            break;
                    }
                }
                else
                {
                    if (detailNode.InnerText.Contains("{{"))
                    {
                        //處理<option>標記裡的v-for

                        switch (type)
                        {
                            case TSClassType.ObjectArray:
                                AnalyticalClassHierarchy(ref dataClassInfos, type, GetVforClassHierarchy(detailNode));
                                type = Extension.GetValueFromDescription<TSClassType>(detailNode.GetAttributeValue($"dyweb-vfor-model", "string"));
                                GetVforItem(out vforItemName, out vforItemValue, detailNode);
                                if (vforItemName != null)
                                {
                                    classHierarchy = detailNode.InnerText.Replace(vforItemName, vforItemValue).Replace("{{", "").Replace("}}", "").Trim().Split(".");
                                    AnalyticalClassHierarchy(ref dataClassInfos, type, classHierarchy);

                                    if (!string.IsNullOrEmpty(detailNode.GetAttributeValue($"v-bind:value", "default")))
                                    {
                                        classHierarchy = detailNode.GetAttributeValue($"v-bind:value", "default").Replace(vforItemName, vforItemValue).Split(".");
                                        AnalyticalClassHierarchy(ref dataClassInfos, TSClassType.String, classHierarchy);
                                    }
                                    continue;
                                }
                                break;
                            case TSClassType.StringArray:
                                classHierarchy = GetVforClassHierarchy(detailNode);
                                AnalyticalClassHierarchy(ref dataClassInfos, type, classHierarchy);
                                continue;
                            //處理<option>標記裡的v-for 細項
                            case TSClassType.Default:
                                if (vforItemName != null)
                                {
                                    classHierarchy = detailNode.InnerText.Replace(vforItemName, vforItemValue).Replace("{{", "").Replace("}}", "").Trim().Split(".");
                                    type = Extension.GetValueFromDescription<TSClassType>(detailNode.GetAttributeValue($"dyweb-vfor-model", "string"));
                                    AnalyticalClassHierarchy(ref dataClassInfos, type, classHierarchy);
                                    continue;
                                }
                                break;
                            default:
                                break;
                        }

                        classHierarchy = detailNode.InnerText.Replace("{{", "").Replace("}}", "").Split(".");
                        AnalyticalClassHierarchy(ref dataClassInfos, type, classHierarchy);
                    }
                }
            }

            return dataClassInfos;
        }

        /// <summary>
        /// 解析 Class 階層
        /// </summary>
        /// <param name="dataClassInfos"></param>
        /// <param name="stringType"></param>
        /// <param name="levelArray"></param>
        private static void AnalyticalClassHierarchy(ref List<DataClassInfo> dataClassInfos, TSClassType type, string[] classHierarchy)
        {
            int objectLevel = classHierarchy.Length;
            while (objectLevel >= 2)
            {
                DataClassInfo dataclass = new DataClassInfo();

                dataclass.PropertyInfo.Type = type.Description();

                dataclass.PropertyInfo.Name = classHierarchy[objectLevel - 1];

                dataclass.ParentClassName = classHierarchy[objectLevel - 2].FirstCharToUpper(); ;

                if (!dataClassInfos.Contains(dataclass))
                {
                    dataClassInfos.Add(dataclass);
                }

                objectLevel -= 1;

                type = TSClassType.Object;
            }
        }

        /// <summary>
        /// 產生 ts dataclass 的 initial 語法
        /// </summary>
        /// <param name="dataClass">DataClass</param>
        /// <returns></returns>
        private static string TsPropertyInitialStringCreater(DataClassInfo dataClass)
        {
            switch (dataClass.PropertyInfo.Type)
            {
                case "string":
                    return $"{dataClass.PropertyInfo.Name} : string = '';";
                case "string[]":
                    return $"{dataClass.PropertyInfo.Name} : string[] = new Array<string>();";
                case "number":
                    return $"{dataClass.PropertyInfo.Name} : number = 0;";
                case "number[]":
                    return $"{dataClass.PropertyInfo.Name} : number[] = new Array<number>();";
                case "boolean":
                    return $"{dataClass.PropertyInfo.Name} : boolean = false;";
                case "boolean[]":
                    return $"{dataClass.PropertyInfo.Name} : boolean[] = new Array<boolean>();";
                case "object":
                    return $"{dataClass.PropertyInfo.Name} : {dataClass.PropertyInfo.Name} = new {dataClass.PropertyInfo.Name}();";
                case "object[]":
                    return $"{dataClass.PropertyInfo.Name} : {dataClass.PropertyInfo.Name}[] = new Array<{dataClass.PropertyInfo.Name}>();";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 取得 Vfor 的 itemName 名稱及值
        /// </summary>
        /// <param name="vforItemName"></param>
        /// <param name="vforItemValue"></param>
        /// <param name="item"></param>
        private static void GetVforItem(out string vforItemName, out string vforItemValue, HtmlNode item)
        {
            vforItemName = item.GetAttributeValue($"v-for", "default").Split(" in ").FirstOrDefault().Trim();
            vforItemValue = item.GetAttributeValue($"v-for", "default").Split(" in ").LastOrDefault().Split(".").LastOrDefault().Trim();
        }

        /// <summary>
        /// 檢查資料夾是否存在，不存在則建立
        /// </summary>
        /// <param name="path"></param>
        private static void CreateFolderIfNotExist(string path)
        {
            if (!Directory.Exists(Path.Combine(AppSettings.OutputFilePath, path)))
            {
                Directory.CreateDirectory(Path.Combine(AppSettings.OutputFilePath, path));
            }
        }

        /// <summary>
        /// 取得指定 AttributeValue 的 Aggr_Post 參數
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static AggrPostInfo GetAggrPostParameterByAttribute(HtmlNode node, string attribute)
        {
            var parameter = node.GetAttributeValue(attribute, "default").Replace("Aggr_Post(", "").Replace(")", "").Split(",");

            AggrPostInfo dywebInitialAggrPostInfo = new AggrPostInfo();

            dywebInitialAggrPostInfo.ExecuteKey = parameter[0];

            for (int j = 1; j < parameter.Length; j++)
            {
                dywebInitialAggrPostInfo.Perameter.Add(parameter[j]);
            }
            return dywebInitialAggrPostInfo;
        }

        /// <summary>
        /// 取得 VforAttributeValue 並回傳 Class 階層
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string[] GetVforClassHierarchy(HtmlNode node)
        {
            return node.GetAttributeValue($"v-for", "default").Split(" in ").LastOrDefault().Replace("{{", "").Replace("}}", "").Trim().Split(".");
        }
    }
}
