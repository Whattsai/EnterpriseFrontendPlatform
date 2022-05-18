using System.Diagnostics.CodeAnalysis;
using HtmlAgilityPack;
using HtmlConvertTool.DataClass;


List<string> DataclassList = new List<string>();
string mainDataClassName = "";

//使用預設編碼讀入 HTML
HtmlDocument doc = new HtmlDocument();
doc.Load(@"Example\e-HR 玉山人園地.html");

//產生DataClassList
DataclassList = OACommunicate.CreateDataClassList(doc, out mainDataClassName);

//產生Detail Vue檔
OACommunicate.CreateDetailVue(doc, "dyweb-initial");

//產生InitialData
var initialData = OACommunicate.CreateDywebInitialData(doc, "dyweb-initial");

//產生SelectInitialData
var selectInitialData = OACommunicate.CreateDywebInitialData(doc, "dyweb-selectinitial");

//產生watchData
var watchData = OACommunicate.CreateDywebWatchData(doc, "dyweb-watch");

//產生Detail Vue檔的Ts內容
OACommunicate.AppendTypeScriptToVue(initialData, selectInitialData, watchData, DataclassList, mainDataClassName);

//產生index.html檔 & App.Vue
OACommunicate.CreateIndex(doc, "dyweb-initial");

Console.ReadLine();


public static class OACommunicate
{
    public static List<AggrPostInfo> CreateDywebInitialData(HtmlDocument htmlDocument, string attributeName)
    {
        //oa-initial 會觸發的方法
        List<AggrPostInfo> InitialData = new List<AggrPostInfo>();

        var htmlNodes = htmlDocument.DocumentNode.SelectNodes($"//*[@{attributeName}]");

        foreach (var item in htmlNodes)
        {
            AggrPostInfo dywebInitial = new AggrPostInfo();

            string[] perameter = item.GetAttributeValue($"{attributeName}", "default").Replace("Aggr_Post(", "").Replace(")", "").Split(",");

            dywebInitial.ExecuteKey = perameter[0];

            for (int i = 1; i < perameter.Length; i++)
            {
                dywebInitial.Perameter.Add(perameter[i]);
            }

            InitialData.Add(dywebInitial);
        }

        return InitialData;
    }

    public static Dictionary<string, AggrPostInfo> CreateDywebWatchData(HtmlDocument htmlDocument, string attributeName)
    {
        Dictionary<string, AggrPostInfo> watchData = new Dictionary<string, AggrPostInfo>();

        var htmlNodes = htmlDocument.DocumentNode.SelectNodes($"//*[@{attributeName}]");

        foreach (var item in htmlNodes)
        {
            AggrPostInfo dywebInitial = new AggrPostInfo();

            string nodeString = item.GetAttributeValue($"{attributeName}", "default");

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

    public static void CreateDetailVue(HtmlDocument htmlDocument, string attributeName)
    {
        //oa-initial 會觸發的方法
        List<string> InnerHTMLs = new List<string>();

        var htmlNodes = htmlDocument.DocumentNode.SelectNodes($"//*[@{attributeName}]");

        for (int i = 0; i < htmlNodes.Count; i++)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\src\components\dyweb_{IntToLetters(i)}.vue", false))
            {
                file.WriteLine("<template>");
                file.WriteLine(htmlNodes[i].InnerHtml);
                file.WriteLine("</template>");
            }
        }
    }

    public static List<string> CreateDataClassList(HtmlDocument htmlDocument, out string mainDataClassName)
    {
        mainDataClassName = "";
        List<DataClass> dataClassList = new List<DataClass> { };

        var htmlNodes = htmlDocument.DocumentNode.SelectNodes($"//*[@dyweb-model]");

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

                if (!dataClassList.Contains(dataclass))
                {
                    dataClassList.Add(dataclass);
                }

                objectLevel -= 1;

                stringType = "object";
            }
            mainDataClassName = levelArray[0];
        }



        //SELECT  var htmlNodes = htmlDocument.DocumentNode.SelectNodes($"//*[@oa-data-vfor]");
        //依據此標籤判斷parent層級及型別後寫入dataClassList

        htmlNodes = htmlDocument.DocumentNode.SelectNodes($"//*[@dyweb-model-vfor]");

        if (htmlNodes == null) return null;

        foreach (var item in htmlNodes)
        {
            string stringType = item.GetAttributeValue($"dyweb-model-vfor", "default");

            DataClass dataclass = new DataClass();

            dataclass.PropertyInfo.Type = stringType.Split(",")[1].Trim();

            dataclass.PropertyInfo.Name = item.InnerHtml.Replace("{{", "").Replace("}}", "").Split(".")[1].Trim();

            dataclass.ParentClassName = stringType.Split(",")[0].Trim();

            if (!dataClassList.Contains(dataclass))
            {
                dataClassList.Add(dataclass);
            }
        }



        Dictionary<string, List<PropertyInfo>> typeScriptClassInfo = new Dictionary<string, List<PropertyInfo>>();

        foreach (var item in dataClassList)
        {
            List<PropertyInfo> propertyInfos;

            if (typeScriptClassInfo.TryGetValue(item.ParentClassName, out propertyInfos))
            {
                propertyInfos.Add(item.PropertyInfo);
            }
            else
            {
                propertyInfos = new List<PropertyInfo>() { item.PropertyInfo };

                typeScriptClassInfo.Add(item.ParentClassName, propertyInfos);
            }
        }

        foreach (var item in typeScriptClassInfo)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\src\dataclass\{item.Key}.ts", false))
            {

                foreach (var objectItem in item.Value.FindAll(o => o.Type == "object" || o.Type == "object[]"))
                {
                    file.WriteLine($"import {{ {objectItem.Name} }} from \"./{ objectItem.Name}\";");
                }

                file.WriteLine($"export class {item.Key} " + "{");

                foreach (var propertyInfo in item.Value)
                {

                    switch (propertyInfo.Type)
                    {

                        case "string":

                            file.WriteLine($"{propertyInfo.Name} : string = '';");

                            break;

                        case "string[]":

                            file.WriteLine($"{propertyInfo.Name} : string[] = new Array<string>();");

                            break;

                        case "number":

                            file.WriteLine($"{propertyInfo.Name} : number = 0;");

                            break;

                        case "number[]":

                            file.WriteLine($"{propertyInfo.Name} : number[] = new Array<number>();");

                            break;

                        case "boolean":

                            file.WriteLine($"{propertyInfo.Name} : boolean = false;");

                            break;

                        case "boolean[]":

                            file.WriteLine($"{propertyInfo.Name} : boolean[] = new Array<boolean>();");

                            break;

                        case "object":

                            file.WriteLine($"{propertyInfo.Name} : {propertyInfo.Name} = new {propertyInfo.Name}();");

                            break;

                        case "object[]":

                            file.WriteLine($"{propertyInfo.Name} : {propertyInfo.Name}[] = new Array<{propertyInfo.Name}>();");

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

    public static void AppendTypeScriptToVue(List<AggrPostInfo> initialInfos, List<AggrPostInfo> selectInitialInfos, Dictionary<string, AggrPostInfo> watchData, List<string> dataClasses, string mainDataClassName)
    {
        for (int i = 0; i < initialInfos.Count(); i++)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\src\components\dyweb_{IntToLetters(i)}.vue", true))
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

                /** dyweb-Model */
                //foreach (var item in dataClasses)
                //{
                //    file.WriteLine($"const {item} = reactive(new {item}());");
                //}

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

                /** dyweb-Initial */
                string postPerameter = String.Join(", ", initialInfos[i].Perameter);
                file.WriteLine($"Aggr_Post(\"{initialInfos[i].ExecuteKey}\",{postPerameter})");

                file.WriteLine("");

                /** dyweb-SelectInitial */
                string postPeram2 = String.Join(", ", selectInitialInfos[i].Perameter);
                file.WriteLine($"Aggr_Post(\"{selectInitialInfos[i].ExecuteKey}\",{postPeram2})");


                file.WriteLine("");

                /** returm */
                file.WriteLine($"return {{ {mainDataClassName}, Aggr_Post }}");
                file.WriteLine("}");
                file.WriteLine("})");
            
                file.WriteLine("</script>");
            }
        }
    }

    public static void CreateIndex(HtmlDocument htmlDocument, string attributeName)
    {
        var htmlNode = htmlDocument.GetElementbyId("app");

        CreateAppVue(htmlNode, "dyweb-initial");

        htmlNode.InnerHtml = "";

        using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"D:\GitProject\EnterpriseFrontendPlatform\webapp\public\index.html", false))
        {
            file.WriteLine(htmlDocument.DocumentNode.InnerHtml);
        }
    }

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
            detailVueNames.Add($"dyweb_{ IntToLetters(i)}");
            HtmlNode newChild = HtmlNode.CreateNode($"<dyweb_{IntToLetters(i)} />");
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

    /// <summary>
    /// 數字轉英文字母
    /// </summary>
    /// <param name="value">數字</param>
    /// <returns></returns>
    private static string IntToLetters(int value)
    {
        value += 1;
        string result = string.Empty;
        while (--value >= 0)
        {
            result = (char)('a' + value % 26) + result;
            value /= 26;
        }
        return result;
    }

    /// <summary>
    /// 字首轉大寫
    /// </summary>
    private static string FirstCharToUpper(this string input) =>
    input switch
    {
        null => throw new ArgumentNullException(nameof(input)),
        "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
        _ => input.First().ToString().ToUpper() + input.Substring(1)
    };

    /// <summary>
    /// 字首轉小寫
    /// </summary>
    private static string FirstCharTolower(this string input) =>
   input switch
   {
       null => throw new ArgumentNullException(nameof(input)),
       "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
       _ => input.First().ToString().ToUpper() + input.Substring(1)
   };

}







