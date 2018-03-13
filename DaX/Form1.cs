using Fiddler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaX
{
    public partial class Form1 : Form
    {
        static Proxy oSecureEndpoint;
        static string sSecureEndpointHostname = "localhost";
        static int iSecureEndpointPort = 7777;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object btnsender, EventArgs e)
        {
            List<Session> oAllSessions = new List<Session>();

            // <-- Personalize for your Application, 64 chars or fewer
            FiddlerApplication.SetAppDisplayName("FiddlerCoreDemoApp");

            #region AttachEventListeners
            //
            // It is important to understand that FiddlerCore calls event handlers on session-handling
            // background threads.  If you need to properly synchronize to the UI-thread (say, because
            // you're adding the sessions to a list view) you must call .Invoke on a delegate on the 
            // window handle.
            // 
            // If you are writing to a non-threadsafe data structure (e.g. List<t>) you must
            // use a Monitor or other mechanism to ensure safety.
            //

            // Simply echo notifications to the console.  Because Fiddler.CONFIG.QuietMode=true 
            // by default, we must handle notifying the user ourselves.
            FiddlerApplication.OnNotification += (sender, oNEA) =>
            {
                Console.WriteLine("** NotifyUser: " + oNEA.NotifyString);
            };
            FiddlerApplication.Log.OnLogString += (sender, oLEA) =>
            {
                Console.WriteLine("** LogString: " + oLEA.LogString);
            };

            FiddlerApplication.BeforeRequest += (oS) =>
           {
               // Console.WriteLine("Before request for:\t" + oS.fullUrl);
               // In order to enable response tampering, buffering mode MUST
               // be enabled; this allows FiddlerCore to permit modification of
               // the response in the BeforeResponse handler rather than streaming
               // the response to the client as the response comes in.
               oS["X-OverrideGateway"] = "127.0.0.1:8888";
               oS.bBufferResponse = false;
               Monitor.Enter(oAllSessions);
               oAllSessions.Add(oS);
               Monitor.Exit(oAllSessions);

               // Set this property if you want FiddlerCore to automatically authenticate by
               // answering Digest/Negotiate/NTLM/Kerberos challenges itself
               // oS["X-AutoAuth"] = "(default)";

               /* If the request is going to our secure endpoint, we'll echo back the response.

               Note: This BeforeRequest is getting called for both our main proxy tunnel AND our secure endpoint, 
               so we have to look at which Fiddler port the client connected to (pipeClient.LocalPort) to determine whether this request 
               was sent to secure endpoint, or was merely sent to the main proxy tunnel (e.g. a CONNECT) in order to *reach* the secure endpoint.

               As a result of this, if you run the demo and visit https://localhost:7777 in your browser, you'll see

               Session list contains...

                   1 CONNECT http://localhost:7777
                   200                                         <-- CONNECT tunnel sent to the main proxy tunnel, port 8877

                   2 GET https://localhost:7777/
                   200 text/html                               <-- GET request decrypted on the main proxy tunnel, port 8877

                   3 GET https://localhost:7777/               
                   200 text/html                               <-- GET request received by the secure endpoint, port 7777
               */

               //if ((oS.oRequest.pipeClient.LocalPort == iSecureEndpointPort) && (oS.hostname == sSecureEndpointHostname))
               //{
               //    oS.utilCreateResponseAndBypassServer();
               //    oS.oResponse.headers.SetStatus(200, "Ok");
               //    oS.oResponse["Content-Type"] = "text/html; charset=UTF-8";
               //    oS.oResponse["Cache-Control"] = "private, max-age=0";
               //    oS.utilSetResponseBody("<html><body>Request for httpS://" + sSecureEndpointHostname + ":" + iSecureEndpointPort.ToString() + " received. Your request was:<br /><plaintext>" + oS.oRequest.headers.ToString());
               //}
           };

            /*
                // The following event allows you to examine every response buffer read by Fiddler. Note that this isn't useful for the vast majority of
                // applications because the raw buffer is nearly useless; it's not decompressed, it includes both headers and body bytes, etc.
                //
                // This event is only useful for a handful of applications which need access to a raw, unprocessed byte-stream
                Fiddler.FiddlerApplication.OnReadResponseBuffer += new EventHandler<RawReadEventArgs>(FiddlerApplication_OnReadResponseBuffer);
            */


            FiddlerApplication.ResponseHeadersAvailable += oS =>
            {

                Console.WriteLine("{0}:HTTP {1} for {2}", oS.id, oS.responseCode, oS.fullUrl);
                var contentlength = oS.ResponseHeaders["Content-Length"];
                if (!string.IsNullOrWhiteSpace(contentlength) && int.Parse(contentlength) > 100000)
                {
                    dgv.BeginInvoke((Action)(() =>
                    {
                        sessionDS.SessionTable.AddSessionTableRow(oS.url, int.Parse(contentlength), oS);
                    }));
                }
            };

            //FiddlerApplication.BeforeResponse += delegate (Fiddler.Session oS)
            //{
            //    Console.WriteLine("{0}:HTTP {1} for {2}", oS.id, oS.responseCode, oS.fullUrl);
            //    var contentlength = oS.ResponseHeaders["Content-Length"];
            //    if (!string.IsNullOrWhiteSpace(contentlength) && int.Parse(contentlength) > 100000)
            //    {
            //        dgv.BeginInvoke((Action)(() =>
            //        {
            //            sessionDS.SessionTable.AddSessionTableRow(oS.url, int.Parse(contentlength), oS);
            //        }));
            //    }
            //    // Uncomment the following two statements to decompress/unchunk the
            //    // HTTP response and subsequently modify any HTTP responses to replace 
            //    // instances of the word "Microsoft" with "Bayden". You MUST also
            //    // set bBufferResponse = true inside the beforeREQUEST method above.
            //    //
            //    //oS.utilDecodeResponse(); oS.utilReplaceInResponse("Microsoft", "Bayden");
            //};

            FiddlerApplication.AfterSessionComplete += delegate (Session oS)
            {
                Console.WriteLine("Finished session:\t" + oS.fullUrl);
                //Console.Title = ("Session list contains: " + oAllSessions.Count.ToString() + " sessions");
            };

            #endregion AttachEventListeners



            string sSAZInfo = "NoSAZ";
            Console.WriteLine(String.Format("Starting {0} ({1})...", FiddlerApplication.GetVersionString(), sSAZInfo));

            // For the purposes of this demo, we'll forbid connections to HTTPS 
            // sites that use invalid certificates. Change this from the default only
            // if you know EXACTLY what that implies.
            CONFIG.IgnoreServerCertErrors = false;

            // ... but you can allow a specific (even invalid) certificate by implementing and assigning a callback...
            // FiddlerApplication.OnValidateServerCertificate += new System.EventHandler<ValidateServerCertificateEventArgs>(CheckCert);

            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);

            // For forward-compatibility with updated FiddlerCore libraries, it is strongly recommended that you 
            // start with the DEFAULT options and manually disable specific unwanted options.
            FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.Default;

            // E.g. If you want to add a flag, start with the .Default and "OR" the new flag on:
            // oFCSF = (oFCSF | FiddlerCoreStartupFlags.CaptureFTP);

            // ... or if you don't want a flag in the defaults, "and not" it out:
            // Uncomment the next line if you don't want FiddlerCore to act as the system proxy
            // oFCSF = (oFCSF & ~FiddlerCoreStartupFlags.RegisterAsSystemProxy);

            // *******************************
            // Important HTTPS Decryption Info
            // *******************************
            // When FiddlerCoreStartupFlags.DecryptSSL is enabled, you must include either
            //
            //     MakeCert.exe
            //
            // *or*
            //
            //     CertMaker.dll
            //     BCMakeCert.dll
            //
            // ... in the folder where your executable and FiddlerCore.dll live. These files
            // are needed to generate the self-signed certificates used to man-in-the-middle
            // secure traffic. MakeCert.exe uses Windows APIs to generate certificates which
            // are stored in the user's \Personal\ Certificates store. These certificates are
            // NOT compatible with iOS devices which require specific fields in the certificate
            // which are not set by MakeCert.exe. 
            //
            // In contrast, CertMaker.dll uses the BouncyCastle C# library (BCMakeCert.dll) to
            // generate new certificates from scratch. These certificates are stored in memory
            // only, and are compatible with iOS devices.

            // Uncomment the next line if you don't want to decrypt SSL traffic.
            // oFCSF = (oFCSF & ~FiddlerCoreStartupFlags.DecryptSSL);

            // NOTE: In the next line, you can pass 0 for the port (instead of 8877) to have FiddlerCore auto-select an available port
            int iPort = 7777;
            oFCSF = FiddlerCoreStartupFlags.Default & ~FiddlerCoreStartupFlags.RegisterAsSystemProxy;
            FiddlerApplication.Startup(iPort, oFCSF);

            FiddlerApplication.Log.LogFormat("Created endpoint listening on port {0}", iPort);

            FiddlerApplication.Log.LogFormat("Starting with settings: [{0}]", oFCSF);
            FiddlerApplication.Log.LogFormat("Gateway: {0}", CONFIG.UpstreamGateway.ToString());

            Console.WriteLine("Hit CTRL+C to end session.");

            // We'll also create a HTTPS listener, useful for when FiddlerCore is masquerading as a HTTPS server
            // instead of acting as a normal CERN-style proxy server.
            oSecureEndpoint = FiddlerApplication.CreateProxyEndpoint(iSecureEndpointPort, true, sSecureEndpointHostname);
            if (null != oSecureEndpoint)
            {
                FiddlerApplication.Log.LogFormat("Created secure endpoint listening on port {0}, using a HTTPS certificate for '{1}'", iSecureEndpointPort, sSecureEndpointHostname);
            }

        }

        private void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                SessionDS.SessionTableRow row = (senderGrid.Rows[e.RowIndex].DataBoundItem as DataRowView).Row as SessionDS.SessionTableRow;
                var cc = (row.colReq as Fiddler.Session);
                var clength = cc.ResponseHeaders["Content-Length"];
                var iclength = int.Parse(clength);
                int lowerlim = 0;
                int deltalim = 1000000;
                List<string> ranges = new List<string>();
                while (((lowerlim + deltalim) < iclength))
                {
                    ranges.Add(lowerlim + " - " + (lowerlim + deltalim));
                    lowerlim += deltalim + 1;
                }
                ranges.Add(lowerlim + " - " + iclength);
                Parallel.ForEach(ranges, r =>
                {
                    var reqh = cc.RequestHeaders.Clone() as HTTPRequestHeaders;
                    reqh["Range"]= "bytes=" + r;
                    var v = FiddlerApplication.oProxy.SendRequest(reqh, cc.RequestBody, new System.Collections.Specialized.StringDictionary());
                    //(x, y)=> { if (y.newState == y.oldState) { } });
                    v.OnStateChanged += (x, y) => { if (y.newState == y.oldState) { } };
                });

            }
        }
    }
}