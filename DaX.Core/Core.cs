using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaX
{
    public class Core
    {
        static Proxy oSecureEndpoint;
        static string sSecureEndpointHostname = "localhost";

        public List<Session> oAllSessions = new List<Session>();

        public event SessionStateHandler ResponseHeadersAvailable;
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

        private void FiddlerApplication_AfterSessionComplete(Session oS)
        {
            Console.WriteLine("Finished session:\t" + oS.fullUrl);
        }

        private void FiddlerApplication_BeforeResponse(Session oS)
        {
            if (oS.oFlags.ContainsKey("dax_id"))
            {
                oS.utilDecodeResponse();
                oS.SaveResponseBody(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DaXCaps", oS.oFlags["dax_id"] + "_DaX_" + oS.RequestHeaders["Range"].Substring("bytes=".Length) + "_XaD_" + oS.SuggestedFilename));
            }
        }

        private void FiddlerApplication_ResponseHeadersAvailable(Session oS)
        {
            Console.WriteLine("{0}:HTTP {1} for {2}", oS.id, oS.responseCode, oS.fullUrl);
            var contentlength = oS.ResponseHeaders["Content-Length"];
            ResponseHeadersAvailable?.Invoke(oS);
        }

        private void FiddlerApplication_OnReadResponseBuffer(object sender, RawReadEventArgs e)
        {
        }



        private void FiddlerApplication_BeforeRequest(Session oS)
        {
            oS["X-OverrideGateway"] = "127.0.0.1:8888";
            oS.bBufferResponse = false;
            Monitor.Enter(oAllSessions);
            oAllSessions.Add(oS);
            Monitor.Exit(oAllSessions);
        }

        private void Log_OnLogString(object sender, LogEventArgs e)
        {
            Console.WriteLine("** LogString: " + e.LogString);
        }

        private void FiddlerApplication_OnNotification(object sender, NotificationEventArgs e)
        {
            Console.WriteLine("** NotifyUser: " + e.NotifyString);
        }


        public void SegmentedDownload(Session oS)
        {
            var strContentLength = oS.ResponseHeaders["Content-Length"];
            var intContentLength = int.Parse(strContentLength);
            int rangeLower = 0;
            int rangeDelta = intContentLength / 10;
            List<string> ranges = new List<string>();
            while (((rangeLower + rangeDelta) < intContentLength))
            {
                ranges.Add(rangeLower + " - " + (rangeLower + rangeDelta));
                rangeLower += rangeDelta + 1;
            }
            ranges.Add(rangeLower + " - " + intContentLength);
            var dax_id = Guid.NewGuid().ToString();
            Parallel.ForEach(ranges, r =>
            {
                var requestHeaders = oS.RequestHeaders.Clone() as HTTPRequestHeaders;
                requestHeaders["Range"] = "bytes=" + r;
                var newflags = new System.Collections.Specialized.StringDictionary { { "dax_id", dax_id } };
                var segmentedSession = FiddlerApplication.oProxy.SendRequest(requestHeaders, oS.RequestBody, newflags);
            });
        }
    }

}
