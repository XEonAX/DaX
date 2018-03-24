using System.Collections.Generic;
using System.ComponentModel;

namespace DaX
{
    public interface ISession
    {
        int ID { get; }
        string Method { get; }
        int Size { get; }
        int Progress { get; }
        int State { get; }
        string URL { get; }
        //List<ISession> SubSessions { get; }
    }
}