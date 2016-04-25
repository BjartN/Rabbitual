using System.ComponentModel;

namespace Rabbitual.Agents.CsvAgent
{
    public class CsvOptions
    {
        public CsvOptions()
        {
            FieldNames = new string[0];
            Separator = ",";
        }

        [Description("True if first line of csv file contains field names")]
        public bool FieldNamesAtFirstLine { get; set; }

        [Description("Names of columns. If first line contains field names, these will replace the field names if set.")]
        public string[] FieldNames { get; set; }

        [Description("If set, only this many rows will be converted to events")]
        public int? RowCount { get; set; }

        [Description("Apply row count starting at the end of the feed")]
        public bool StartAtEnd { get; set; }

        public string Separator { get; set; }

        [Description("Url of CSV")]
        public string Url { get; set; }
    }
}