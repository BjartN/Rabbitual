using System.IO;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public class FileObjectDb : IObjectDb
    {
        private readonly ISerializer _s;
        private readonly string _folder;

        public FileObjectDb(ISerializer s, IConfiguration cfg)
        {
            _s = s;
            _folder = cfg.Get("rabbitual.filedb.folder");
        }

        public void Save(object o, string id)
        {
            if (o == null)
                return;

            var file = getFile(id);
            var bytes = _s.ToBytes(o);
            File.WriteAllBytes(file, bytes);
        }

        private string getFile(string id)
        {
            return Path.Combine(_folder, id + ".dat");
        }

        public T Get<T>(string id)
        {
            var file = getFile(id);
            if (!File.Exists(file))
                return default(T);

            var bytes = File.ReadAllBytes(file);
            return _s.FromBytes<T>(bytes);
        }
    }
}