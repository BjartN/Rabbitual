using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Rabbitual.Agents.CsvAgent;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual.IntegrationTests
{
    [TestFixture]
    public class when_accessing_database
    {
        private AgentDb _db;

        [SetUp]
        public void setup_each_test()
        {
            var f = @"c:\temp\rabbitual-repo\rabbitual-test.sqlite";
            if (File.Exists(f))
                File.Delete(f);

            SQLiteConnection.CreateFile(f);

            Func<IDbConnection> factory = () =>
            {
                var sqliteConn = new SQLiteConnection($"Data Source={f};Version=3;");
                sqliteConn.Open();
                return sqliteConn;
            };

            _db = new AgentDb(factory, new JsonSerializer());
            _db.CreateSchema();
        }


        [Test]
        public void shold_insert_agent_and_get_it_back_out()
        {
            var agent = new AgentConfigDto
            {
                Name = "WebServer1",
                Type = "WebServer"
            };

            var agentId = _db.InsertAgent(agent);
            var agentFromDb = _db.GetAgents().FirstOrDefault(x => x.Id == agentId);

            Assert.IsNotNull(agentFromDb);
            Assert.AreEqual("WebServer1", agentFromDb.Name);
            Assert.AreEqual("WebServer", agentFromDb.Type);
        }


        [Test]
        public void should_insert_options_and_get_it_back_out()
        {
            var o = new CsvOptions
            {
                Separator = "|"
            };

            _db.InsertOrReplaceOptions(100, o);
            var dbOptions = _db.GetOptions(100, typeof(CsvOptions));

            Assert.AreEqual("|", ((CsvOptions) dbOptions).Separator);
        }


        [Test]
        public void should_insert_state_and_get_it_back_out()
        {
            var o = new CsvOptions
            {
                Separator = "|"
            };

            _db.InsertOrReplaceState(100, o);
            var dbState = _db.GetState(100, typeof(CsvOptions));

            Assert.AreEqual("|", ((CsvOptions)dbState).Separator);
        }
    }
}
