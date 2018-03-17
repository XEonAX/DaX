using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using AEonAX.Shared;

namespace DaX
{
    public class Session : NotifyBase, ISession
    {
        private Fiddler.Session fSession;
        private Core score;

        public Session(Fiddler.Session session, Core core)
        {
            fSession = session;
            score = core;
            CmdAbortSession = new SimpleCommand<Session>()
            {
                ExecuteDelegate = (oS) =>
                 {
                     oS.fSession.Abort();
                 },
                CanExecuteDelegate = (oS) =>
                {
                    if (oS==null)
                        return false;
                    return oS.fSession.state != SessionStates.Aborted;
                }
            };
            CmdDownloadSession = new SimpleCommand<Session>()
            {
                ExecuteDelegate = (oS) =>
                {
                    score.SegmentedDownload(oS.fSession);
                },
                CanExecuteDelegate = (oS) =>
                {
                    return true;
                }
            };
            CmdRefreshSession = new SimpleCommand<Session>()
            {
                ExecuteDelegate = (oS) =>
                {

                },
                CanExecuteDelegate = (oS) =>
                {
                    return true;
                }
            };
            CmdXSession = new SimpleCommand<Session>()
            {
                ExecuteDelegate = (oS) => { },
                CanExecuteDelegate = (oS) =>
                {
                    return true;
                }
            };

        }

        public int ID
        {
            get
            {
                return fSession.id;
            }
        }
        public string URL
        {
            get
            {
                return fSession.url;
            }
        }

        public string Method
        {
            get
            {
                return fSession.RequestMethod;
            }
        }
        public int Size
        {
            get
            {
                var size = fSession.ResponseHeaders["Content-Length"];
                if (string.IsNullOrWhiteSpace(size))
                    return -1;
                else
                    return int.Parse(size);
            }
        }



        public SimpleCommand<Session> CmdDownloadSession { get; set; }
        public SimpleCommand<Session> CmdAbortSession { get; set; }
        public SimpleCommand<Session> CmdRefreshSession { get; set; }
        public SimpleCommand<Session> CmdXSession { get; set; }
    }
}
