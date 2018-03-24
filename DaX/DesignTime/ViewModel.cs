using AEonAX.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaX.DesignTime
{
    public class ViewModel
    {
        public List<Session> Sessions { get; set; } = new List<Session>();
        public Session DownloadDetail { get; set; }
        public ViewModel()
        {
            int i = 0;
            Sessions.Add(new Session
            {
                ID = ++i,
                Method = "GET",
                URL = @"https://api.nuget.org/v3/registration3-gz/mahapps.metro/page/1.1.3-alpha143/1.1.3-alpha210.json",
                Size = 105239,
                Progress=50,
                State=5
            });
            Sessions.Add(new Session
            {
                ID = ++i,
                Method = "POST",
                URL = @"http://811092dcc98648d790a1730d08245b4b.monitor-eqatec.com/monitor.ashx?pv=4&mt=dotnet&mb=3.3.43&cv=28e28a5aa7794bf18a1b5b232e5e249a&av=4.6.20172.35715&pi=811092dcc98648d790a1730d08245b4b&ms=0&rs=0&tm=0&lr=0",
                Size = 44239,
                Progress = 50,
                State = 5
            });
            Sessions.Add(new Session
            {
                ID = ++i,
                Method = "GET",
                URL = @"https://jar.web.io/static/LargeFile.txt",
                Size = 2105239,
                Progress = 50,
                State = 5
            });
            Sessions.Add(new Session
            {
                ID = ++i,
                Method = "GET",
                URL = @"http://r3---sn-b51vo-2o9e.gvt1.com/edgedl/release2/chrome_component/dHDIrKyxeSE_4344/4344_all_crl-set-16726194787281141118.data.crx3?cms_redirect=yes&ip=43.241.24.63&ipbits=0&mm=28&mn=sn-b51vo-2o9e&ms=nvh&mt=1521284308&mv=m&pcm2cms=yes&pl=24&shardbypass=yes",
                Size = 639,
                Progress = 50,
                State = 5
            });
            i = 0;
            //Sessions[0].DownloadQueue.Add(
            //    new Session
            //    {
            //        ID = ++i,
            //        Method = "GET",
            //        URL = @"https://api.nuget.org/v3/registration3-gz/mahapps.metro/page/1.1.3-alpha143/1.1.3-alpha210.json",
            //        Size = 105239
            //    });
            //Sessions[0].DownloadQueue.Add(
            //    new Session
            //    {
            //        ID = ++i,
            //        Method = "GET",
            //        URL = @"https://api.nuget.org/v3/registration3-gz/mahapps.metro/page/1.1.3-alpha143/1.1.3-alpha210.json",
            //        Size = 105239
            //    });
            //Sessions[0].DownloadQueue.Add(
            //    new Session
            //    {
            //        ID = ++i,
            //        Method = "GET",
            //        URL = @"https://api.nuget.org/v3/registration3-gz/mahapps.metro/page/1.1.3-alpha143/1.1.3-alpha210.json",
            //        Size = 105239
            //    });
            //Sessions[0].DownloadQueue.Add(
            //    new Session
            //    {
            //        ID = ++i,
            //        Method = "GET",
            //        URL = @"https://api.nuget.org/v3/registration3-gz/mahapps.metro/page/1.1.3-alpha143/1.1.3-alpha210.json",
            //        Size = 105239
            //    });
            //Sessions[0].DownloadQueue.Add(
            //    new Session
            //    {
            //        ID = ++i,
            //        Method = "GET",
            //        URL = @"https://api.nuget.org/v3/registration3-gz/mahapps.metro/page/1.1.3-alpha143/1.1.3-alpha210.json",
            //        Size = 105239
            //    });
            Sessions[0].DownloadQueue.Add(new DownloadQueueItem { RangeStart = 0, RangeEnd = 100, Processed = true });
            Sessions[0].DownloadQueue.Add(new DownloadQueueItem { RangeStart = 101, RangeEnd = 200, Processed = true });
            Sessions[0].DownloadQueue.Add(new DownloadQueueItem { RangeStart = 201, RangeEnd = 300, Processed = null });
            Sessions[0].DownloadQueue.Add(new DownloadQueueItem { RangeStart = 301, RangeEnd = 350, Processed = false });

            DownloadDetail = Sessions[0];
        }
    }
}
