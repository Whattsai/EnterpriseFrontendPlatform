using HtmlAgilityPack;
using HtmlConvertTool.DataClass;
using System.Linq;
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
            //取得 dy-component 底下所有 Node
            HtmlNodeCollection htmlNodeCollection = _doc.DocumentNode.SelectNodes($"//*[@dy-component]");

            foreach (var node in htmlNodeCollection)
            {
                ComponentInfo componentInfo = new ComponentInfo();

                //ComponentVue 檔名
                componentInfo.Name = GetComponentName(node);

                //建立DataClass.ts & 準備DataClassName
                componentInfo.DataClassNames = CreateDataClassInfo(node, out var mainDataClassName);
                componentInfo.MainDataClassName = mainDataClassName;

                //準備InitialData
                componentInfo.InitialInfo = CreateInitialData(node);

                //準備watchData
                componentInfo.WatchInfo = CreateWatchData(node);

                //建立ComponentVue檔案
                CreateComponentVue(node, componentInfo);
            }

            //產生index.html檔 & App.Vue & View.vue
            CreateRouterTs(_doc.DocumentNode);
            CreateAppVue();
            CreateViewVue(_doc.DocumentNode);
            CreateIndex(_doc);
        }

        private static string GetComponentName(HtmlNode node)
        {
            string nodeString = node.GetAttributeValue($"dy-component", "default");

            return nodeString.Split(",")[0].Replace("'", "");
        }

        private static string GetViewName(HtmlNode node)
        {
            var viewNode = node.SelectSingleNode($"//*[@dy-view]");
            string nodeString = viewNode.GetAttributeValue($"dy-view", "default");

            return nodeString.Split(",")[0].Replace("'", "");
        }

        private static string GetDetailViewName(HtmlNode node)
        {
            var viewNode = node.SelectSingleNode($"//*[@dy-detailview]");
            string nodeString = viewNode.GetAttributeValue($"dy-detailview", "default");

            return nodeString.Split(",")[0].Replace("'", "");
        }

        /// <summary>
        /// 準備 dy-component、dy-selectinitial 觸發方法的資料
        /// </summary>
        /// <param name="node">HtmlNode</param>
        /// <returns></returns>
        private static Queue<AggrPostInfo> CreateInitialData(HtmlNode node)
        {
            Queue<AggrPostInfo> InitialData = new Queue<AggrPostInfo>();

            AggrPostInfo dyInitialInfo = GetAggrPostParameterByAttribute(node, $"dy-component");

            InitialData.Enqueue(dyInitialInfo);

            foreach (var selectInitialNode in node.SelectNodes($".//*[@dy-selectinitial]"))
            {
                AggrPostInfo selectinitialInfo = GetAggrPostParameterByAttribute(selectInitialNode, $"dy-selectinitial");

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

            var htmlNodes = node.SelectNodes($".//*[@dy-watch]");

            foreach (var item in htmlNodes)
            {
                AggrPostInfo dyInitial = new AggrPostInfo();

                string nodeString = item.GetAttributeValue($"dy-watch", "default");

                string key = nodeString.Split(",")[0];

                string aggrPostString = nodeString.Substring(nodeString.IndexOf(',') + 1);

                string[] perameter = aggrPostString.Replace("Aggr_Post(", "").Replace(")", "").Split(",");

                dyInitial.ExecuteKey = perameter[0];

                for (int i = 1; i < perameter.Length; i++)
                {
                    dyInitial.Perameter.Add(perameter[i]);
                }

                watchData.Add(key, dyInitial);
            }

            return watchData;
        }

        /// <summary>
        /// 準備 DataClass 資料並建立檔案
        /// </summary>
        /// <param name="node"></param>
        /// <param name="mainDataClassName"></param>
        /// <returns>DataClassName</returns>
        private static List<string> CreateDataClassInfo(HtmlNode node, out string mainDataClassName)
        {

            List<DataClassInfo> dataClassInfos = DyWebConvert.AnalyticalNode(node);
            mainDataClassName = dataClassInfos[0].ParentClassName;
            CreateFolderIfNotExist($@"src\dataclass\");

            //參數最前面加【.】才會限縮在子階層內
            //var htmlNodes = node.SelectNodes($".//*[@dy-model]");

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
        /// 建立 Component Vue 檔案
        /// </summary>
        /// <param name="vueName"></param>
        /// <param name="initialInfos"></param>
        /// <param name="watchData"></param>
        /// <param name="dataClasses"></param>
        /// <param name="mainDataClassName"></param>
        private static void CreateComponentVue(HtmlNode node, ComponentInfo componentInfo)
        {
            CreateFolderIfNotExist($@"src\components\");

            using (StreamWriter file = new StreamWriter(Path.Combine(AppSettings.OutputFilePath, $@"src\components\{componentInfo.Name}.vue"), false))
            {
                file.WriteLine("<template>");
                file.WriteLine(node.InnerHtml);
                file.WriteLine("</template>");

                file.WriteLine("<script lang='ts'>");
                file.WriteLine("import { watch, reactive, defineComponent } from 'vue';");
                file.WriteLine("import 'reflect-metadata';");
                file.WriteLine("import 'es6-shim';");
                file.WriteLine("import { classToPlain, deserialize, plainToClassFromExist } from 'class-transformer';");
                file.WriteLine("import axios from 'axios';");
                foreach (var item in componentInfo.DataClassNames)
                {
                    file.WriteLine($"import {{{ item }}} from '../dataclass/{item}';");
                }

                file.WriteLine("");

                file.WriteLine($"export default defineComponent ({{");
                file.WriteLine("setup() {");

                file.WriteLine("");

                file.WriteLine($"const {componentInfo.MainDataClassName.FirstCharTolower()} = reactive(new {componentInfo.MainDataClassName.FirstCharToUpper()}());");

                file.WriteLine("");

                /** dy-Watch */
                foreach (var item in componentInfo.WatchInfo)
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
                file.WriteLine($"const key = perameters[i] as keyof {componentInfo.MainDataClassName.FirstCharToUpper()};");
                file.WriteLine($"jsonstring += '\"' + perameters[i] + '\":\"' + {componentInfo.MainDataClassName.FirstCharTolower()}[key] + '\"';");
                file.WriteLine("}}");
                file.WriteLine("jsonstring += \"} \";");
                file.WriteLine("var postData = JSON.parse(jsonstring);");
                file.WriteLine(" const automapp = (jsonData: any) => {");
                file.WriteLine(" const multiJson = classToPlain(jsonData);");
                file.WriteLine($" plainToClassFromExist({componentInfo.MainDataClassName.FirstCharTolower()}, multiJson);");
                file.WriteLine(" }");
                file.WriteLine(" const postRequest = {Service: serviceName, ID: executeKey, Data: postData }");
                file.WriteLine($" axios.post('{AppSettings.AxiosPost}', postRequest)");
                file.WriteLine($" .then((response) => automapp(response.data));");
                file.WriteLine("}");

                file.WriteLine("");

                /** dy-component & SelectInitial */
                while (componentInfo.InitialInfo.Any())
                {
                    AggrPostInfo initialInfo = componentInfo.InitialInfo.Dequeue();
                    string postPerameter = String.Join(", ", initialInfo.Perameter);
                    file.WriteLine($"Aggr_Post({initialInfo.ExecuteKey},{postPerameter})");
                    file.WriteLine("");
                }

                file.WriteLine("");

                /** returm */
                file.WriteLine($"return {{ {componentInfo.MainDataClassName.FirstCharTolower()}, Aggr_Post }}");
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

            htmlNode.InnerHtml = "";

            using (StreamWriter file = new StreamWriter(Path.Combine(AppSettings.OutputFilePath, $@"public\index.html"), false))
            {
                file.WriteLine(htmlDocument.DocumentNode.InnerHtml);
            }
        }

        /// <summary>
        /// 建立 View.Vue
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <param name="attributeName"></param>oooooo
        private static void CreateViewVue(HtmlNode htmlNode)
        {
            List<string> componentNameList = new List<string>();
            List<string> detailViewNameList = new List<string>();

            CreateFolderIfNotExist(@"src\views\");

            //取得每個dy-view
            var viewNode = htmlNode.SelectSingleNode($"//*[@dy-view]");

            var detailViewNodes = viewNode.SelectNodes($"//*[@dy-view=\"'{GetViewName(viewNode)}'\"]//@dy-detailview");

            if (detailViewNodes.Any())
            {
                foreach (var detailViewNode in detailViewNodes)
                {
                    string detailViewPath = Path.Combine(AppSettings.OutputFilePath, $@"src\views\{GetDetailViewName(detailViewNode)}.vue");

                    var deatilViewComponentNodes = viewNode.SelectNodes($"//*[@dy-detailview=\"'{GetDetailViewName(detailViewNode)}'\"]//@dy-component");

                    if (deatilViewComponentNodes != null)
                    {
                        foreach (var node in deatilViewComponentNodes)
                        {
                            componentNameList.Add($"{GetComponentName(node)}");
                            HtmlNode newChild3 = HtmlNode.CreateNode($"<{GetComponentName(node)} />");
                            node.ParentNode.ReplaceChild(newChild3, node);
                        }
                    }
                    using (StreamWriter file = new StreamWriter(detailViewPath, false))
                    {
                        file.WriteLine("<template>");
                        file.WriteLine(detailViewNode.InnerHtml);
                        file.WriteLine("</template>");
                        file.WriteLine("<script lang='ts'>");
                        file.WriteLine("import { defineComponent } from 'vue';");

                        foreach (var item in componentNameList)
                        {
                            file.WriteLine($"import {item} from './components/{item}.vue';");
                        }

                        file.WriteLine("export default defineComponent({");
                        file.WriteLine("name: 'App',");
                        file.WriteLine("components: {");

                        foreach (var item in componentNameList)
                        {
                            file.WriteLine($"\"{ item}\": {item},");
                        }

                        file.WriteLine(" },");
                        file.WriteLine("})");
                        file.WriteLine("</script>");
                    }

                    //除去主view裡的DetailView內容
                    HtmlNode newChild2 = HtmlNode.CreateNode($"<{GetDetailViewName(detailViewNode)} />");
                    viewNode.ReplaceChild(newChild2, detailViewNode);

                }
            }

            var componentNodes = viewNode.SelectNodes($"//*[@dy-component]");

            string path = Path.Combine(AppSettings.OutputFilePath, $@"src\views\{GetViewName(viewNode)}.vue");

            if (componentNodes != null)
            {
                foreach (var node in componentNodes)
                {
                    componentNameList.Add($"{GetComponentName(node)}");
                    HtmlNode newChild = HtmlNode.CreateNode($"<{GetComponentName(node)} />");
                    node.ParentNode.ReplaceChild(newChild, node);
                }

                using (StreamWriter file = new StreamWriter(path, false))
                {
                    file.WriteLine("<template>");
                    file.WriteLine(viewNode.InnerHtml);
                    file.WriteLine("</template>");
                    file.WriteLine("<script lang='ts'>");
                    file.WriteLine("import { defineComponent } from 'vue';");

                    foreach (var item in componentNameList)
                    {
                        file.WriteLine($"import {item} from './components/{item}.vue';");
                    }

                    file.WriteLine("export default defineComponent({");
                    file.WriteLine("name: 'App',");
                    file.WriteLine("components: {");

                    foreach (var item in componentNameList)
                    {
                        file.WriteLine($"\"{ item}\": {item},");
                    }

                    file.WriteLine(" },");
                    file.WriteLine("})");
                    file.WriteLine("</script>");
                }
            }


        }

        /// <summary>
        /// 建立 App.Vue
        /// </summary>
        private static void CreateAppVue()
        {
            string path = Path.Combine(AppSettings.OutputFilePath, $@"src\App.vue");

            using (StreamWriter file = new StreamWriter(path, false))
            {
                file.WriteLine("<template>");
                file.WriteLine("<router-view/>");
                file.WriteLine("</template>");
            }
        }

        /// <summary>
        /// 建立 App.Vue
        /// </summary>
        private static void CreateRouterTs(HtmlNode htmlNode)
        {
            string path = Path.Combine(AppSettings.OutputFilePath, $@"src\router\index.ts");

            CreateFolderIfNotExist(path);

            var viewNode = htmlNode.SelectSingleNode($"//*[@dy-view]");

            var detailViewNodes = viewNode.SelectNodes($"//*[@dy-view=\"'{GetViewName(viewNode)}'\"]//@dy-detailview");



            using (StreamWriter file = new StreamWriter(path, false))
            {
                file.WriteLine("import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';");
                file.WriteLine($"import {GetViewName(viewNode)} from '../views/{GetViewName(viewNode)}.vue';");

                foreach (var node in detailViewNodes)
                {
                    file.WriteLine($"import {GetDetailViewName(node)} from '../views/{GetDetailViewName(node)}.vue';");
                }

                file.WriteLine("import NotFound from '../views/NotFound.vue';");
                file.WriteLine("import axios from 'axios';");
                file.WriteLine("");
                file.WriteLine("const routes: Array<RouteRecordRaw> = [");
                file.WriteLine("{");
                file.WriteLine("path: '/',");
                file.WriteLine($"name: '{GetViewName(viewNode)}',");
                file.WriteLine($"component: () => import('../views/{GetViewName(viewNode)}.vue'),");

                file.WriteLine("children:[");
                foreach (var node in detailViewNodes)
                {
                    file.WriteLine("{name: '"+$"{GetDetailViewName(node)}"+ "',path: '" + $"{GetDetailViewName(node)}" + "',component: " + $"{GetDetailViewName(node)}" + ",},");
                }
                file.WriteLine("],");

                file.WriteLine("},");
                file.WriteLine("{");
                file.WriteLine("path: '/:pathMatch(.*)*',");
                file.WriteLine("name: 'NotFound',");
                file.WriteLine("component: NotFound,");
                file.WriteLine("},");
                file.WriteLine("]");
                file.WriteLine("const router = createRouter({");
                file.WriteLine("history: createWebHistory(process.env.BASE_URL),");
                file.WriteLine("routes");
                file.WriteLine("})");
                file.WriteLine("export default router");
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
                TSClassType type = Extension.GetValueFromDescription<TSClassType>(detailNode.GetAttributeValue($"dy-model", "default"));

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
                                type = Extension.GetValueFromDescription<TSClassType>(detailNode.GetAttributeValue($"dy-vfor-model", "string"));
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
                                    type = Extension.GetValueFromDescription<TSClassType>(detailNode.GetAttributeValue($"dy-vfor-model", "string"));
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
            if (!Directory.Exists(Path.GetDirectoryName(Path.Combine(AppSettings.OutputFilePath, path))))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(AppSettings.OutputFilePath, path)));
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

            AggrPostInfo dyInitialAggrPostInfo = new AggrPostInfo();

            dyInitialAggrPostInfo.ExecuteKey = parameter[0];

            for (int j = 1; j < parameter.Length; j++)
            {
                dyInitialAggrPostInfo.Perameter.Add(parameter[j]);
            }
            return dyInitialAggrPostInfo;
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
