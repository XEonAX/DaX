using Fiddler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DaX
{
    public class Core
    {
        static Proxy oSecureEndpoint;
        static string sSecureEndpointHostname = "localhost";

        //public List<Fiddler.Session> oAllSessions = new List<Fiddler.Session>();

        public event EventHandler<SessionEventArgs> ResponseHeadersAvailable;
        public Core() : this(7777, 7777) { }

        public Core(int unsecurePort, int securePort)
        {
            FiddlerApplication.SetAppDisplayName("DaX.Core");

            FiddlerApplication.OnNotification += FiddlerApplication_OnNotification;
            FiddlerApplication.Log.OnLogString += Log_OnLogString;
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            FiddlerApplication.OnReadResponseBuffer += FiddlerApplication_OnReadResponseBuffer;
            FiddlerApplication.ResponseHeadersAvailable += FiddlerApplication_ResponseHeadersAvailable;
            FiddlerApplication.BeforeResponse += FiddlerApplication_BeforeResponse;
            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;


            Console.WriteLine(String.Format("Starting {0} ...", FiddlerApplication.GetVersionString()));

            CONFIG.IgnoreServerCertErrors = false;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);

            FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.Default;
            oFCSF = FiddlerCoreStartupFlags.Default & ~FiddlerCoreStartupFlags.RegisterAsSystemProxy;
            FiddlerApplication.Startup(unsecurePort, oFCSF);

            FiddlerApplication.Log.LogFormat("Created endpoint listening on port {0}", unsecurePort);

            FiddlerApplication.Log.LogFormat("Starting with settings: [{0}]", oFCSF);
            FiddlerApplication.Log.LogFormat("Gateway: {0}", CONFIG.UpstreamGateway.ToString());
            oSecureEndpoint = FiddlerApplication.CreateProxyEndpoint(securePort, true, sSecureEndpointHostname);
            if (null != oSecureEndpoint)
            {
                FiddlerApplication.Log.LogFormat("Created secure endpoint listening on port {0}, using a HTTPS certificate for '{1}'", securePort, sSecureEndpointHostname);
            }

        }

        private void FiddlerApplication_AfterSessionComplete(Fiddler.Session oS)
        {
            Console.WriteLine("Finished session: (" + DateTime.Now + ")\t" + oS.fullUrl);
        }

        private void FiddlerApplication_BeforeResponse(Fiddler.Session oS)
        {
            if (oS.oFlags.ContainsKey("dax_id"))
            {
                oS.utilDecodeResponse();
                oS.SaveResponseBody(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXCaps", oS.oFlags["dax_id"] + "_DaX_" + oS.RequestHeaders["Range"].Substring("bytes=".Length) + "_XaD_" + oS.SuggestedFilename));
                oS.ResponseBody = null;
                GC.Collect();
            }
        }

        private void FiddlerApplication_ResponseHeadersAvailable(Fiddler.Session oS)
        {
            Console.WriteLine("{0}:HTTP {1} for {2}", oS.id, oS.responseCode, oS.fullUrl);
            var contentlength = oS.ResponseHeaders["Content-Length"];
            if (!oS.oFlags.ContainsKey("dax_id"))
            {
                ResponseHeadersAvailable?.Invoke(this, new SessionEventArgs(oS, this));
            }
        }

        private void FiddlerApplication_OnReadResponseBuffer(object sender, RawReadEventArgs e)
        {
        }



        private void FiddlerApplication_BeforeRequest(Fiddler.Session oS)
        {
            oS["X-OverrideGateway"] = "127.0.0.1:8888";
            oS.bBufferResponse = false;
            //Monitor.Enter(oAllSessions);
            //oAllSessions.Add(oS);
            //Monitor.Exit(oAllSessions);
        }

        private void Log_OnLogString(object sender, LogEventArgs e)
        {
            //Console.WriteLine("** LogString: " + e.LogString);
        }

        private void FiddlerApplication_OnNotification(object sender, NotificationEventArgs e)
        {
            //Console.WriteLine("** NotifyUser: " + e.NotifyString);
        }


        //public void SegmentedDownload(Session dsession)
        //{
        //    var oS = dsession.fSession;
        //    var strContentLength = oS.ResponseHeaders["Content-Length"];
        //    var intContentLength = int.Parse(strContentLength);
        //    int rangeLower = 0;
        //    int incre = 1;
        //    int rangeDelta = intContentLength / 500;
        //    List<string> ranges = new List<string>();
        //    List<int> deltas = new List<int>();
        //    while (((rangeLower + (rangeDelta * incre)) < intContentLength))
        //    {
        //        ranges.Add(rangeLower + " - " + (rangeLower + (rangeDelta * incre)));
        //        rangeLower += (rangeDelta * incre) + 1;
        //        deltas.Add((rangeDelta * incre));
        //        incre++;
        //    }
        //    ranges.Add(rangeLower + " - " + intContentLength);
        //    var rangep = Partitioner.Create(0, intContentLength);
        //    var dax_id = Guid.NewGuid().ToString();
        //    dsession.DownloadQueue.Clear();// = new ConcurrentBag<Fiddler.Session>();
        //    int tot = 0;
        //    int comp = 0;
        //    dsession.Progress = 0;
        //    //Parallel.For(0, ranges.Count, r =>
        //    //foreach (var r in ranges)
        //    Parallel.ForEach(rangep, new ParallelOptions { MaxDegreeOfParallelism = 3 }, r =>
        //    {
        //        var requestHeaders = oS.RequestHeaders.Clone() as HTTPRequestHeaders;
        //        requestHeaders["Range"] = "bytes=" + r.Item1 + " - " + r.Item2; //+ ranges[r];
        //        var newflags = new System.Collections.Specialized.StringDictionary { { "dax_id", dax_id } };

        //        Interlocked.Increment(ref tot);
        //        //tot++;
        //        var segmentedSession = FiddlerApplication.oProxy.SendRequestAndWait(requestHeaders,
        //             oS.RequestBody,
        //             newflags,
        //             (sender, evtstatechangeargs) =>
        //             {
        //                 if (evtstatechangeargs.newState == SessionStates.Done)
        //                 {
        //                     Interlocked.Increment(ref comp);
        //                     //comp++;
        //                     dsession.Progress = Convert.ToInt32((comp * 100.0) / (tot));
        //                     foreach (var session in dsession.DownloadQueue)
        //                     {
        //                         if (session.fSession.state != SessionStates.Done)
        //                         {
        //                             return;
        //                         }
        //                     }
        //                     if (dsession.Progress != 100) return;
        //                     Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXDL"));

        //                     var idfiles = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXCaps"), Path.GetFileName(dax_id) + "_DaX_" + "*" + "_XaD_" + "*");
        //                     var destname = idfiles[0];
        //                     destname = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXDL", destname.Substring(destname.IndexOf("_XaD_") + "_XaD_".Length));

        //                     using (Stream destStream = File.OpenWrite(destname))
        //                     {
        //                         foreach (string srcFileName in idfiles.OrderBy(x => x, new AlphanumComparatorFast()))
        //                         {
        //                             try
        //                             {
        //                                 using (Stream srcStream = File.OpenRead(srcFileName))
        //                                 {
        //                                     srcStream.CopyTo(destStream);
        //                                 }
        //                                 File.Delete(srcFileName);
        //                             }
        //                             catch (Exception ex)
        //                             {
        //                                 Console.WriteLine(ex);
        //                             }
        //                         }
        //                     }
        //                     Application.Current.Dispatcher.Invoke(() =>
        //                     {
        //                         //dsession.SubSessions.Clear();// = new ConcurrentBag<Fiddler.Session>();
        //                     });
        //                     GC.Collect();
        //                 }
        //             });
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            dsession.DownloadQueue.Add(new Session(segmentedSession, this));
        //        });
        //    }
        //    );
        //}
    }



    public class SessionEventArgs : System.EventArgs
    {
        public Session Session { get; set; }
        public SessionEventArgs(Fiddler.Session oS, Core core)
        {
            Session = new Session(oS, core);
        }
    }
}
