namespace Rabbitual.Infrastructure
{
    public interface ISerializer
    {
        byte[] ToBytes<T>(T o);
        T FromBytes<T>(byte[] o);
    }
}