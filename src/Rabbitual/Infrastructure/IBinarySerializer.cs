using System;

namespace Rabbitual.Infrastructure
{
    public interface IBinarySerializer
    {
        byte[] ToBytes<T>(T o);
        T FromBytes<T>(byte[] o);
        object FromBytes(byte[] bytes, Type t);
    }
}