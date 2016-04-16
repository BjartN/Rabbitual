namespace Rabbitual
{
    public interface IConsumer
    {
        bool CanConsume(object o);
        void Consume(object o);
    }
}