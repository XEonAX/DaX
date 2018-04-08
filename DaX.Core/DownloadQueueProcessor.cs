using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DaX
{
    public static class DownloadQueueProcessor
    {
        //string dax_id;
        //int tot = 0;
        //int comp = 0;
        //private int MaxParallel;
        //Session Session;
        //public List<Tuple<int, int>> Ranges { get; set; } = new List<Tuple<int, int>>();
        public static void Initialize(Session session)
        {
            //Session = dsession;
            //MaxParallel = parallelcount;
            long rangeLower = 0;
            long incre = 1;

            session.dax_id = Guid.NewGuid().ToString();
            long rangeDelta = session.Size / session.Config.DeltaDivider;//500
            //List<string> ranges = new List<string>();
            //List<int> deltas = new List<int>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                session.DownloadQueue.Clear();
            });
            while (((rangeLower + (rangeDelta * incre)) < session.Size))
            {
                var rangeUpper = (rangeLower + (rangeDelta * incre));
                Application.Current.Dispatcher.Invoke(() =>
                {
                    session.DownloadQueue.Add(new DownloadQueueItem()
                    {
                        RangeStart = rangeLower,
                        RangeEnd = rangeUpper,
                        DSession = session
                    });
                });
                rangeLower += (rangeDelta * incre) + 1;
                //deltas.Add((rangeDelta * incre));
                incre += 1 * session.Config.IncrementMultiplier;//1
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                session.DownloadQueue.Add(new DownloadQueueItem()
                {
                    RangeStart = rangeLower,
                    RangeEnd = session.Size,
                    DSession=session
                });
            });
        }

        public static void Start(Session session,int howmany)
        {
            var rangestostart = session.DownloadQueue.Where(y => y.Processed == false).Take(howmany);
            foreach (var r in rangestostart)
            {
                Start(session,r);
            }
        }

        public static void Start(Session session,DownloadQueueItem r)
        {
            r.Processed = null;
            session.tot = session.DownloadQueue.Count;
            var oS = session.fSession;
            var requestHeaders = oS.RequestHeaders.Clone() as HTTPRequestHeaders;
            requestHeaders["Range"] = "bytes=" + r.RangeStart + " - " + r.RangeEnd; //+ ranges[r];
            var newflags = new System.Collections.Specialized.StringDictionary { { "dax_id", session.dax_id } };

            //Interlocked.Increment(ref tot);
            //tot++;
            r.Session = FiddlerApplication.oProxy.SendRequest(requestHeaders,
                 oS.RequestBody,
                 newflags,
                 (sender, evtstatechangeargs) =>
                 QueueItemStateChange(session, evtstatechangeargs, r)
                 );
        }

        private static void QueueItemStateChange(Session session, StateChangeEventArgs evtstatechangeargs, DownloadQueueItem r)
        {
            if (evtstatechangeargs.newState == SessionStates.Done)
            {
                r.Processed = true;
                Interlocked.Increment(ref session.comp);
                Start(session,1);
                //comp++;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    session.Progress = Convert.ToInt32((session.comp * 100.0) / (session.tot));
                });
                var tempDQ = session.DownloadQueue.ToList();
                //lock (Session.LockObject)
                foreach (var dqitem in tempDQ)
                {
                    lock (dqitem.LockObject)
                    {
                        //Console.WriteLine(session.Session);
                        var a = dqitem;
                        var b = a.Session;
                        var c = b.state;
                        if (c != SessionStates.Done)
                        {
                            return;
                        }
                    }
                }
                if (session.Progress != 100) return;
                lock (session.LockObject)
                {
                    Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXDL"));

                    var idfiles = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXCaps"), Path.GetFileName(session.dax_id) + "_DaX_" + "*" + "_XaD_" + "*");
                    if (idfiles.Length > 0)
                    {

                        var destname = idfiles[0];
                        destname = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXDL", destname.Substring(destname.IndexOf("_XaD_") + "_XaD_".Length));

                        using (Stream destStream = File.OpenWrite(destname))
                        {
                            foreach (string srcFileName in idfiles.OrderBy(x => x, new AlphanumComparatorFast()))
                            {
                                try
                                {
                                    using (Stream srcStream = File.OpenRead(srcFileName))
                                    {
                                        srcStream.CopyTo(destStream);
                                    }
                                    File.Delete(srcFileName);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
                        }
                    }
                }
                //Application.Current.Dispatcher.Invoke(() =>
                //{
                //    //Session.SubSessions.Clear();// = new ConcurrentBag<Fiddler.Session>();
                //});
                GC.Collect();
            }

        }
    }
}
