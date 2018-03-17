using System;

namespace DaX.DesignTime
{
    public class Session : ISession
    {
        public int ID { get; set; }
        public string Method { get; set; }
        public int Size { get; set; }
        public string URL { get; set; }
    }
}