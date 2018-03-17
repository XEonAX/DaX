namespace DaX
{
    public interface ISession
    {
        int ID { get; }
        string Method { get; }
        int Size { get; }
        string URL { get; }
    }
}