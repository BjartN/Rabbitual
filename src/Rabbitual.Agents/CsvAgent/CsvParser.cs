using System;
using System.Collections.Generic;
using System.Linq;
using Rabbitual.Core.Infrastructure;

namespace Rabbitual.Agents.CsvAgent
{
    public class CsvParser
    {
        public static IDictionary<string, string>[] Parse(string csv, CsvOptions options)
        {
            var lines = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var cols = resolveCols(lines, options);
            var l = new List<IDictionary<string, string>>();

            var dataLines = lines.Skip(options.FieldNamesAtFirstLine ? 1 : 0).ToArray();
            var rowCount = options.RowCount.HasValue ? Math.Min(options.RowCount.Value, dataLines.Length) : dataLines.Length;

            dataLines = options.StartAtEnd
                ? dataLines.TakeLast(rowCount).ToArray()
                : dataLines.Take(rowCount).ToArray();

            foreach (var line in dataLines)
            {
                var vals = line.Split(options.Separator);
                var d = new Dictionary<string, string>();
                for (var i = 0; i < cols.Length; i++)
                {
                    var col = cols[i];
                    if (i >= vals.Length)
                        break;
                    if (string.IsNullOrWhiteSpace(col))
                        continue;
                    d[col] = vals[i];
                }
                l.Add(d);
            }
            return l.ToArray();
        }

        private static string[] resolveCols(string[] lines, CsvOptions options)
        {
            if (!options.FieldNamesAtFirstLine)
                return options.FieldNames;

            if (lines.Length == 0)
                return new string[0];

            return lines[0].Split(options.Separator);
        }
    }
}