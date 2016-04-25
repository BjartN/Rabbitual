using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Rabbitual.Infrastructure
{
    public static class StringExtentions
    {
        public static string[] Split(this string s, string separator)
        {
            return s.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string FromTemplate(this string template, IDictionary<string, string> d)
        {
            if (string.IsNullOrWhiteSpace(template))
                return string.Empty;

            var text = template;
            foreach (var s in d)
            {
                text = text.ReplaceInsensitive("{" + s.Key + "}", s.Value);
            }
            return text;
        }

        public static double ToDouble(this string s)
        {
            return double.Parse(s, CultureInfo.InvariantCulture);
        }

        public static string ReplaceInsensitive(this string str, string from, string to)
        {
            str = Regex.Replace(str, from, to, RegexOptions.IgnoreCase);
            return str;
        }
    }
}