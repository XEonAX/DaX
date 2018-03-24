using System;
using System.Collections.Generic;

namespace DaX.DesignTime
{
    public class Session : ISession
    {
        public int ID { get; set; }
        public string Method { get; set; }
        public int Size { get; set; }
        public int Progress { get; set; }
        public int State { get; set; }
        public string URL { get; set; }
        public List<DownloadQueueItem> DownloadQueue { get; set; } = new List<DownloadQueueItem>();
    }
}