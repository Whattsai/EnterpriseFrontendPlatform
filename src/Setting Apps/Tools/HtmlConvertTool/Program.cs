using HtmlAgilityPack;
using HtmlConvertTool;
using HtmlConvertTool.DataClass;

Dictionary<string, List<string>> _dataClassNameByWebInitial = new Dictionary<string, List<string>>();
Dictionary<string, Queue<AggrPostInfo>> _initialDataByWebInitial = new Dictionary<string, Queue<AggrPostInfo>>();
Dictionary<string, Dictionary<string, AggrPostInfo>> _watchDataByWebInitial = new Dictionary<string, Dictionary<string, AggrPostInfo>>();

string mainDataClassName = "";

//使用預設編碼讀入 HTML
HtmlDocument doc = new HtmlDocument();
doc.Load(@"Example\e-HR 玉山人園地.html");

//產生 Detail Vue 檔
OACommunicate.CreateDetailVue(doc);

//依 dyweb-initial 分別取得底下 Node
var initialNodes = doc.DocumentNode.SelectNodes($"//*[@dyweb-initial]");

for (int i = 0; i < initialNodes.Count; i++)
{
    HtmlNode? node = initialNodes[i];

    //產生DataClassList
    _dataClassNameByWebInitial.Add($"dyweb_{i.IntToLetters()}", OACommunicate.CreateDataClassInfoByWebInitial(node, out mainDataClassName));

    //產生InitialData
    _initialDataByWebInitial.Add($"dyweb_{i.IntToLetters()}", OACommunicate.CreateDywebInitialData(node));

    //產生watchData
    _watchDataByWebInitial.Add($"dyweb_{i.IntToLetters()}", OACommunicate.CreateDywebWatchData(node));

    //產生Detail Vue檔的Ts內容
    OACommunicate.AppendTypeScriptToVue($"dyweb_{i.IntToLetters()}", _initialDataByWebInitial[$"dyweb_{i.IntToLetters()}"], _watchDataByWebInitial[$"dyweb_{i.IntToLetters()}"], _dataClassNameByWebInitial[$"dyweb_{i.IntToLetters()}"], mainDataClassName);
}

//產生index.html檔 & App.Vue
OACommunicate.CreateIndex(doc);

Console.ReadLine();


public static class OACommunicate
{
    /// <summary>
    /// 準備 dyweb-initial、dyweb-selectinitial 觸發方法的資料
    /// </summary>
    /// <param name="node">HtmlNode</param>
    /// <returns></returns>
    public static Queue<AggrPostInfo> CreateDywebInitialData(HtmlNode node)
    {
        Queue<AggrPostInfo> InitialData = new Queue<AggrPostInfo>();

        AggrPostInfo dywebInitial = new AggrPostInfo();

        string[] perameter = node.GetAttributeValue($"dyweb-initial", "default").Replace("Aggr_Post(", "").Replace(")", "").Split(",");

        dywebInitial.ExecuteKey = perameter[0];

        for (int j = 1; j < perameter.Length; j++)
        {
            dywebInitial.Perameter.Add(perameter[j]);
        }

        InitialData.Enqueue(dywebInitial);

        foreach (var selectinitialNode in node.SelectNodes($".//*[@dyweb-selectinitial]"))
        {
            AggrPostInfo dywebInitial2 = new AggrPostInfo();

            string[] perameter2 = selectinitialNode.GetAttributeValue($"dyweb-selectinitial", "default").Replace("Aggr_Post(", "").Replace(")", "").Split(",");

            dywebInitial2.ExecuteKey = perameter2[0];

            for (int j = 1; j < perameter2.Length; j++)
            {
                dywebInitial2.Perameter.Add(perameter2[j]);
            }

            InitialData.Enqueue(dywebInitial2);
        }

        return InitialData;
    }

    /// <summary>
    /// 準備 dy-watch 觸發方法的資料
    /// </summary>
    /// <param name="node">HtmlNode</param>
    /// <returns></returns>
    public static Dictionary<string, AggrPostInfo> CreateDywebWatchData(HtmlNode node)
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
    public static void CreateDetailVue(HtmlDocument htmlDocument)
    {
        List<string> InnerHTMLs = new List<string>();

        var htmlNodes = htmlDocument.DocumentNode.SelectNodes($"//*[@dyweb-initial]");

        for (int i = 0; i < htmlNodes.Count; i++)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\src\components\dyweb_{i.IntToLetters()}.vue", false))
            {
                file.WriteLine("<template>");
                file.WriteLine(htmlNodes[i].InnerHtml);
                file.WriteLine("</template>");
            }
        }
    }

    /// <summary>
    /// 準備 DataClass 資料並建立檔案
    /// </summary>
    /// <param name="node"></param>
    /// <param name="mainDataClassName"></param>
    /// <returns></returns>
    public static List<string> CreateDataClassInfoByWebInitial(HtmlNode node, out string mainDataClassName)
    {
        mainDataClassName = "";
        Dictionary<string, List<string>> dataClassDic = new Dictionary<string, List<string>>();

        List<DataClass> dataClassInfos = new List<DataClass>();

        //參數最前面加【.】才會限縮在子階層內
        var htmlNodes = node.SelectNodes($".//*[@dyweb-model]");

        if (htmlNodes == null) return null;

        foreach (var item in htmlNodes)
        {
            string stringType = item.GetAttributeValue($"dyweb-model", "default");

            string[] levelArray;

            if (stringType == "object[]")
            {
                levelArray = item.GetAttributeValue($"v-for", "default").Split("in").LastOrDefault().Trim().Split(".");
            }
            else
            {
                levelArray = item.InnerHtml.Replace("{{", "").Replace("}}", "").Split(".");
            }

            var objectLevel = levelArray.Length;

            while (objectLevel >= 2)
            {
                DataClass dataclass = new DataClass();

                dataclass.PropertyInfo.Type = stringType;

                dataclass.PropertyInfo.Name = levelArray[objectLevel - 1];

                dataclass.ParentClassName = levelArray[objectLevel - 2].FirstCharToUpper(); ;

                if (!dataClassInfos.Contains(dataclass))
                {
                    dataClassInfos.Add(dataclass);
                }

                objectLevel -= 1;

                stringType = "object";
            }
            mainDataClassName = levelArray[0];
        }

        //依據此標籤判斷parent層級及型別後寫入dataClassList

        htmlNodes = node.SelectNodes($".//*[@dyweb-model-vfor]");

        if (htmlNodes == null) return null;

        foreach (var item in htmlNodes)
        {
            string stringType = item.GetAttributeValue($"dyweb-model-vfor", "default");

            DataClass dataclass = new DataClass();

            dataclass.PropertyInfo.Type = stringType.Split(",")[1].Trim();

            dataclass.PropertyInfo.Name = item.InnerHtml.Replace("{{", "").Replace("}}", "").Split(".")[1].Trim();

            dataclass.ParentClassName = stringType.Split(",")[0].Trim();

            if (!dataClassInfos.Contains(dataclass))
            {
                dataClassInfos.Add(dataclass);
            }
        }

        Dictionary<string, List<DataClass>> typeScriptClassInfo = dataClassInfos.GroupBy(o => o.ParentClassName).ToDictionary(o => o.Key, o => o.ToList());

        foreach (var item in typeScriptClassInfo)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\src\dataclass\{item.Key}.ts", false))
            {

                foreach (var objectItem in item.Value.FindAll(o => o.PropertyInfo.Type == "object" || o.PropertyInfo.Type == "object[]"))
                {
                    file.WriteLine($"import {{ {objectItem.PropertyInfo.Name} }} from \"./{ objectItem.PropertyInfo.Name}\";");
                }

                file.WriteLine($"export class {item.Key} " + "{");

                foreach (var value in item.Value)
                {

                    switch (value.PropertyInfo.Type)
                    {

                        case "string":

                            file.WriteLine($"{value.PropertyInfo.Name} : string = '';");

                            break;

                        case "string[]":

                            file.WriteLine($"{value.PropertyInfo.Name} : string[] = new Array<string>();");

                            break;

                        case "number":

                            file.WriteLine($"{value.PropertyInfo.Name} : number = 0;");

                            break;

                        case "number[]":

                            file.WriteLine($"{value.PropertyInfo.Name} : number[] = new Array<number>();");

                            break;

                        case "boolean":

                            file.WriteLine($"{value.PropertyInfo.Name} : boolean = false;");

                            break;

                        case "boolean[]":

                            file.WriteLine($"{value.PropertyInfo.Name} : boolean[] = new Array<boolean>();");

                            break;

                        case "object":

                            file.WriteLine($"{value.PropertyInfo.Name} : {value.PropertyInfo.Name} = new {value.PropertyInfo.Name}();");

                            break;

                        case "object[]":

                            file.WriteLine($"{value.PropertyInfo.Name} : {value.PropertyInfo.Name}[] = new Array<{value.PropertyInfo.Name}>();");

                            break;

                        default:

                            break;

                    }
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
    public static void AppendTypeScriptToVue(string vueName, Queue<AggrPostInfo> initialInfos, Dictionary<string, AggrPostInfo> watchData, List<string> dataClasses, string mainDataClassName)
    {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\src\components\{vueName}.vue", true))
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


            file.WriteLine($"const {mainDataClassName} = reactive(new {mainDataClassName.FirstCharToUpper()}());");

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
            file.WriteLine("const Aggr_Post = (executeKey: string, ...perameters: string[]) => {");
            file.WriteLine("var jsonstring = \"{ \";");
            file.WriteLine("for (var i = 0; i < perameters.length; i++) {");
            file.WriteLine("if (i > 0) { jsonstring += ',' }");
            file.WriteLine("if (perameters[i].includes('url:')) {");
            file.WriteLine("const paramInfo = perameters[i].replace(\"url: \", \"\").split(':');");
            file.WriteLine("const urlParams = new URLSearchParams(window.location.search);");
            file.WriteLine("const paramValue = urlParams.get(paramInfo[0]);");
            file.WriteLine("jsonstring += '\"' + paramInfo[1] + '\":\"' + paramValue + '\"';");
            file.WriteLine("} else {");
            file.WriteLine($"const key = perameters[i] as keyof {mainDataClassName.FirstCharToUpper()};");
            file.WriteLine($"jsonstring += '\"' + perameters[i] + '\":\"' + {mainDataClassName}[key] + '\"';");
            file.WriteLine("}}");
            file.WriteLine("jsonstring += \"} \";");
            file.WriteLine("var postData = JSON.parse(jsonstring);");
            file.WriteLine(" const automapp = (jsonData: any) => {");
            file.WriteLine(" const multiJson = classToPlain(jsonData);");
            file.WriteLine($" plainToClassFromExist({mainDataClassName}, multiJson);");
            file.WriteLine(" }");
            file.WriteLine(" axios.post('https://localhost:7080/api/Values/BonusInfo', postData)");
            file.WriteLine($" .then((response) => automapp(response.data));");
            file.WriteLine("}");

            file.WriteLine("");

            /** dyweb-Initial & SelectInitial */
            while (initialInfos.Any())
            {
                AggrPostInfo initialInfo = initialInfos.Dequeue();
                string postPerameter = String.Join(", ", initialInfo.Perameter);
                file.WriteLine($"Aggr_Post(\"{initialInfo.ExecuteKey}\",{postPerameter})");

                file.WriteLine("");
            }



            file.WriteLine("");

            /** returm */
            file.WriteLine($"return {{ {mainDataClassName}, Aggr_Post }}");
            file.WriteLine("}");
            file.WriteLine("})");

            file.WriteLine("</script>");
        }

    }

    /// <summary>
    /// 建立Index.html
    /// </summary>
    /// <param name="htmlDocument"></param>
    public static void CreateIndex(HtmlDocument htmlDocument)
    {
        var htmlNode = htmlDocument.GetElementbyId("app");

        CreateAppVue(htmlNode, "dyweb-initial");

        htmlNode.InnerHtml = "";

        using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\public\index.html", false))
        {
            file.WriteLine(htmlDocument.DocumentNode.InnerHtml);
        }
    }

    /// <summary>
    /// 建立 App.Vue
    /// </summary>
    /// <param name="htmlNode"></param>
    /// <param name="attributeName"></param>
    private static void CreateAppVue(HtmlNode htmlNode, string attributeName)
    {
        List<string> detailVueNames = new List<string>();
        var htmlNodes = htmlNode.SelectNodes($"//*[@{attributeName}]");
        using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\src\App.vue", false))
        {
            file.WriteLine(htmlNode.InnerHtml);
        }
        for (int i = 0; i < htmlNodes.Count(); i++)
        {
            detailVueNames.Add($"dyweb_{i.IntToLetters()}");
            HtmlNode newChild = HtmlNode.CreateNode($"<dyweb_{i.IntToLetters()} />");
            htmlNodes[i].ParentNode.ReplaceChild(newChild, htmlNodes[i]);
        }

        using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\src\App.vue", false))
        {
            file.WriteLine("<template>");
            file.WriteLine(htmlNode.InnerHtml);
            file.WriteLine("</template>");
            file.WriteLine("<script lang='ts'>");
            file.WriteLine("import { defineComponent } from 'vue';");
            foreach (var item in detailVueNames)
            {
                file.WriteLine($"import {item} from './components/{item}.vue';");
            }
            file.WriteLine("export default defineComponent({");
            file.WriteLine("name: 'App',");
            file.WriteLine("components: {");

            foreach (var item in detailVueNames)
            {
                file.WriteLine($"\"{ item}\": {item},");
            }
            file.WriteLine(" },");
            file.WriteLine("})");
            file.WriteLine("</script>");
        }

    }
}







