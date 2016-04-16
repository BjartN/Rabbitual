namespace Rabbitual.Infrastructure
{
    public interface IObjectDb
    {
        void Save(object o, string id);
        object Get(string id);
    }
}