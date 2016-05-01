using System;
using System.IO;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public class FileObjectDb : IObjectDb
    {
        private readonly IJsonSerializer _s;
        private readonly string _folder;

        public FileObjectDb(IJsonSerializer s, IAppConfiguration cfg)
        {
            _s = s;
            _folder = cfg.Get("rabbitual.filedb.folder");
        }

        public void Save(object o, string id)
        {
            if (o == null)
                return;

            var file = getFile(id);
            var json = _s.Serialize(o);
            File.WriteAllText(file, json);
        }

        private string getFile(string id)
        {
            return Path.Combine(_folder, id + ".json");
        }

        public object Get(Type t, string id)
        {
            var file = getFile(id);
            if (!File.Exists(file))
                return null;

            var bytes = File.ReadAllText(file);
            return _s.Deserialize(bytes, t);
        }

        public T Get<T>(string id)
        {
            var file = getFile(id);
            if (!File.Exists(file))
                return default(T);

            var json = File.ReadAllText(file);
            return _s.Deserialize<T>(json);
        }
    }
}