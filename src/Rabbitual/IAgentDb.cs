using System;
using System.Data;
using System.Linq;
using Dapper;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public interface IAgentDb
    {
        object GetOptions(int agentId, Type t);
        void InsertOrReplaceOptions(int agentId, object options);

        void UpdateAgent(AgentConfigDto a);
        int InsertAgent(AgentConfigDto a);
        AgentConfigDto[] GetAgents();

        T GetState<T>(int agentId);
        object GetState(int agentId, Type t);
        void InsertOrReplaceState(int agentId, object state);
    }

    public class AgentDb : IAgentDb
    {
        private readonly Func<IDbConnection> _c;
        private readonly IJsonSerializer _s;

        public AgentDb(Func<IDbConnection> c, IJsonSerializer s)
        {
            _c = c;
            _s = s;
        }

        public object GetOptions(int agentId, Type t)
        {
            var q = @"SELECT * FROM Options WHERE AgentId=@AgentId";
            using (var c = _c())
            {
                var r = c.Query<OptionsDto>(q, new
                {
                    AgentId = agentId
                }).FirstOrDefault();

                if (r == null)
                    return null;

                return _s.Deserialize(r.Json, t);
            }
        }

        public T GetState<T>(int agentId)
        {
            return (T) GetState(agentId, typeof(T));
        }

        public object GetState(int agentId, Type t)
        {
            var q = @"SELECT * FROM State WHERE AgentId=@AgentId";
            using (var c = _c())
            {
                var r = c.Query<StateDto>(q, new
                {
                    AgentId = agentId
                }).FirstOrDefault();

                if (r == null)
                    return null;

                return _s.Deserialize(r.Json, t);
            }
        }


        public void InsertOrReplaceOptions(int agentId, object options)
        {
            var q = "INSERT OR REPLACE INTO Options(AgentId,Json) VALUES (@AgentId, @Json)";
            using (var c = _c())
            {
                c.Execute(q, new
                {
                    AgentId = agentId,
                    Json = _s.Serialize(options)
                });
            }

        }


        public void InsertOrReplaceState(int agentId, object state)
        {
            var q = "INSERT OR REPLACE INTO State(AgentId,Json) VALUES (@AgentId, @Json)";
            using (var c = _c())
            {
                c.Execute(q, new
                {
                    AgentId = agentId,
                    Json = _s.Serialize(state)
                });
            }

        }

        public void UpdateAgent(AgentConfigDto a)
        {
            using (var c = _c())
            {
                c.Query("UPDATE Agent SET Name=@Name, Schedule=@Schedule, Type=@Type WHERE Id=@Id",
                    new
                    {
                        a.Name,
                        a.Type,
                        a.Schedule,
                        a.Id
                    });

                c.Query("DELETE FROM AgentSource WHERE TargetAgentId=@Id", new {a.Id});

                foreach (var s in a.SourceIds)
                {
                    c.Query("INSERT INTO AgentSource(SourceAgentId,TargetAgentId) VALUES (@S,@T)", new
                    {
                        T = a.Id,
                        S = s
                    });
                }

            }
        }
        
        public int InsertAgent(AgentConfigDto a)
        {
            using (var c = _c())
            {
                var agentId = c.Query<int>(@"
                            INSERT INTO Agent(Name,Schedule,Type) VALUES (@Name,@Schedule,@Type);
                            SELECT last_insert_rowid();",
                    new
                    {
                        a.Name,
                        a.Type,
                        a.Schedule
                    });


                foreach (var s in a.SourceIds)
                {
                    c.Query("INSERT INTO AgentSource(SourceAgentId,TargetAgentId) VALUES (@S,@T)", new
                    {
                        T=agentId,S=s
                    });
                }

                return agentId.First();
            }
        }

        public AgentConfigDto[] GetAgents()
        {
            using (var c = _c())
            {
                var agents = c.Query<AgentConfigBaseDto>("SELECT * FROM Agent");
                var sources = c
                    .Query<AgentSourceDto>("SELECT * FROM AgentSource")
                    .GroupBy(x => x.TargetAgentId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.SourceAgentId).Distinct().ToArray());

                return agents
                    .Select(x => x.ToConfig(sources.ContainsKey(x.Id) ? sources[x.Id] : new int[0]))
                    .ToArray();
            }
        }

        public void CreateSchema()
        {
            var qAgent = @"CREATE TABLE IF NOT EXISTS Agent (
                Id INTEGER  PRIMARY KEY AUTOINCREMENT, 
                Name TEXT, 
                Schedule INTEGER , 
                Type TEXT)";

            var qAgentSource = @"CREATE TABLE IF NOT EXISTS AgentSource (
                Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                SourceAgentId INTEGER , 
                TargetAgentId INTEGER )";

            var qOptions = @"CREATE TABLE IF NOT EXISTS Options (
                AgentId INTEGER PRIMARY KEY,
                Json TEXT)";

            var qState = @"CREATE TABLE IF NOT EXISTS State (
                AgentId INTEGER  PRIMARY KEY,
                Json TEXT)";

            var qIndex = "CREATE INDEX IF NOT EXISTS TargetIdx ON AgentSource (TargetAgentId)";

            var qWebServer = @"
                INSERT INTO Agent (Name,Schedule,Type) VALUES ('WebServer',null,'WebServerAgent');
                SELECT last_insert_rowid();";


            using (var c = _c())
            {
                c.Execute(qAgent);
                c.Execute(qAgentSource);
                c.Execute(qOptions);
                c.Execute(qState);
                c.Execute(qIndex);

                c.Query<int>(qWebServer);
            }
        }

    }

    public class OptionsDto
    {
        public int AgentId { get; set; }
        public string Json { get; set; }
    }

    public class StateDto
    {
        public int AgentId { get; set; }
        public string Json { get; set; }
    }

    public class AgentSourceDto
    {
        public int Id { get; set; }
        public int SourceAgentId { get; set; }
        public int TargetAgentId { get; set; }
    }
}
