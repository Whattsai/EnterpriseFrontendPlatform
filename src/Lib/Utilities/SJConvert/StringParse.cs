using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SJ.Convert
{
    public static class StringParse
    {
        public static string GetCombinedString(string target, object obj)
        {
            var match = Regex.Matches(target, @"(?<=\{)[\w\d.]+(?=\})");

            foreach (Match m in match)
            {
                target = target.Replace("{" + m.Value + "}", GetDataHierarchy(m.Value, obj));
            }

            return target;
        }

        public static string? GetDataHierarchy(string hierarachyString, object obj)
        {
            string[] valuePath = hierarachyString.Split('.');

            foreach(var eachPar in valuePath)
            {
                var tmp = DictionaryEx.ToDictionary<object>(obj);

                if(!tmp.ContainsKey(eachPar))
                {
                    return null;
                }
                obj = tmp[eachPar];
            }

            return obj?.ToString();
        }
    }
}
