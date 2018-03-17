using Fiddler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            List<Fiddler.Session> oAllSessions = new List<Fiddler.Session>();

            // <-- Personalize for your Application, 64 chars or fewer
            FiddlerApplication.SetAppDisplayName("FiddlerDaX");

            #region AttachEventListeners
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

            Fiddler.FiddlerApplication.OnReadResponseBuffer += FiddlerApplication_OnReadResponseBuffer;



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
                else
                {
                    dgv.BeginInvoke((Action)(() =>
                    {
                        sessionDS.SessionTable.AddSessionTableRow(oS.url, -1, oS);
                    }));
                }
            };
            FiddlerApplication.BeforeResponse += (oS) =>
            {
                if (oS.oFlags.ContainsKey("dax_id"))
                {
                    oS.utilDecodeResponse();
                    oS.SaveResponseBody(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DaXCaps", oS.oFlags["dax_id"] + "_DaX_" + oS.RequestHeaders["Range"].Substring("bytes=".Length) + "_XaD_" + oS.SuggestedFilename));
                }
            };

            FiddlerApplication.AfterSessionComplete += (oS) =>
           {
               Console.WriteLine("Finished session:\t" + oS.fullUrl);
           };

            #endregion AttachEventListeners



            string sSAZInfo = "NoSAZ";
            Console.WriteLine(String.Format("Starting {0} ({1})...", FiddlerApplication.GetVersionString(), sSAZInfo));

            CONFIG.IgnoreServerCertErrors = false;
            FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);

            FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.Default;
            int iPort = 7777;
            oFCSF = FiddlerCoreStartupFlags.Default & ~FiddlerCoreStartupFlags.RegisterAsSystemProxy;
            FiddlerApplication.Startup(iPort, oFCSF);

            FiddlerApplication.Log.LogFormat("Created endpoint listening on port {0}", iPort);

            FiddlerApplication.Log.LogFormat("Starting with settings: [{0}]", oFCSF);
            FiddlerApplication.Log.LogFormat("Gateway: {0}", CONFIG.UpstreamGateway.ToString());

            oSecureEndpoint = FiddlerApplication.CreateProxyEndpoint(iSecureEndpointPort, true, sSecureEndpointHostname);
            if (null != oSecureEndpoint)
            {
                FiddlerApplication.Log.LogFormat("Created secure endpoint listening on port {0}, using a HTTPS certificate for '{1}'", iSecureEndpointPort, sSecureEndpointHostname);
            }

        }

        private void FiddlerApplication_OnReadResponseBuffer(object sender, RawReadEventArgs e)
        {
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
                int deltalim = iclength / 10;
                List<string> ranges = new List<string>();
                while (((lowerlim + deltalim) < iclength))
                {
                    ranges.Add(lowerlim + " - " + (lowerlim + deltalim));
                    lowerlim += deltalim + 1;
                }
                ranges.Add(lowerlim + " - " + iclength);
                var dax_id = Guid.NewGuid().ToString();
                Parallel.ForEach(ranges, r =>
                {
                    var reqh = cc.RequestHeaders.Clone() as HTTPRequestHeaders;
                    reqh["Range"] = "bytes=" + r;
                    var newflags = new System.Collections.Specialized.StringDictionary { { "dax_id", dax_id } };
                    var v = FiddlerApplication.oProxy.SendRequest(reqh, cc.RequestBody, newflags);
                    //(x, y)=> { if (y.newState == y.oldState) { } });
                    v.OnStateChanged += (x, y) => { if (y.newState == y.oldState) { } };

                });

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DaXCaps"), "*" + "_DaX_" + "*" + "_XaD_" + "*");

            var ids = files.Select(x => x.Substring(0, x.IndexOf("_DaX_"))).Distinct();

            foreach (var id in ids)
            {
                var idfiles = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(id)), Path.GetFileName(id) + "_DaX_" + "*" + "_XaD_" + "*");
                var destname = idfiles[0];
                destname = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DaXDL", destname.Substring(destname.IndexOf("_XaD_") + "_XaD_".Length));

                using (Stream destStream = File.OpenWrite(destname))
                {
                    foreach (string srcFileName in idfiles.OrderBy(x => x, new AlphanumComparatorFast()))
                    {
                        using (Stream srcStream = File.OpenRead(srcFileName))
                        {
                            srcStream.CopyTo(destStream);
                        }
                    }
                }
            }
        }
    }
}

