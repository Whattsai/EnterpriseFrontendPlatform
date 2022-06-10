using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HtmlConvertTool
{
    public static class Extension
    {
        public static string IntToLetters(this int value)
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
        public static string FirstCharToUpper(this string input) =>
           input switch
           {
               null => throw new ArgumentNullException(nameof(input)),
               "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
               _ => input.First().ToString().ToUpper() + input.Substring(1)
           };

        /// <summary>
        /// 字首轉小寫
        /// </summary>
        public static string FirstCharTolower(this string input) =>
           input switch
           {
               null => throw new ArgumentNullException(nameof(input)),
               "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
               _ => input.First().ToString().ToUpper() + input.Substring(1)
           };

        /// <summary>
        /// 取得 Enum 列舉 Attribute Description 設定值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Description<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }
}
