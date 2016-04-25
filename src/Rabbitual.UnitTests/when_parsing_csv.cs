using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rabbitual.Agents.CsvAgent;

namespace Rabbitual.UnitTests
{
    [TestFixture]
    public class when_parsing_csv
    {
        private string _csvWithCols = @"
a,b,c
1,2,3
4,5,6
";
        private string _csvWithoutCols =@"
1,2,3
4,5,6
";

        [Test]
        public void should_pick_from_end_when_configured()
        {
            var r = CsvParser.Parse(_csvWithCols, new CsvOptions { FieldNamesAtFirstLine = true, RowCount = 1,StartAtEnd=true});
            Assert.AreEqual(r.Length,1);
            Assert.AreEqual("4", r[0]["a"]);
            Assert.AreEqual("5", r[0]["b"]);
            Assert.AreEqual("6", r[0]["c"]);
        }

        [Test]
        public void should_contain_correct_keys()
        {
            var r = CsvParser.Parse(_csvWithCols, new CsvOptions { FieldNamesAtFirstLine = true });
            Assert.AreEqual("a", r[0].Keys.Skip(0).First());
            Assert.AreEqual("b", r[0].Keys.Skip(1).First());
            Assert.AreEqual("c", r[0].Keys.Skip(2).First());


            var r2 = CsvParser.Parse(_csvWithoutCols, new CsvOptions { FieldNamesAtFirstLine = false, FieldNames = new [] {"e","f","g"}});
            Assert.AreEqual("e", r2[0].Keys.Skip(0).First());
            Assert.AreEqual("f", r2[0].Keys.Skip(1).First());
            Assert.AreEqual("g", r2[0].Keys.Skip(2).First());

        }

        [Test]
        public void number_of_lines_should_match()
        {
         
            var r = CsvParser.Parse(_csvWithCols, new CsvOptions{FieldNamesAtFirstLine = true});
            Assert.AreEqual(2, r.Length);

            var r2 = CsvParser.Parse(_csvWithoutCols, new CsvOptions { FieldNamesAtFirstLine = false });
            Assert.AreEqual(2, r2.Length);

        }
    }
}
