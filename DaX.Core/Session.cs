using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using AEonAX.Shared;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace DaX
{
    public class Session : NotifyBase, ISession
    {
        internal Fiddler.Session fSession;
        private Core score;

        public Session(Fiddler.Session session, Core core)
        {
            fSession = session;
            fSession.OnStateChanged += (x, stateChangeEventArgs) =>
            {
                State = (int)stateChangeEventArgs.newState;
            };
            score = core;
            CmdAbortSession = new SimpleCommand<Session>()
            {
                ExecuteDelegate = (oS) =>
                 {
                     oS.fSession.Abort();
                 },
                CanExecuteDelegate = (oS) =>
                {
                    if (oS == null)
                        return false;
                    return oS.fSession.state != SessionStates.Aborted;
                }
            };
            CmdDownloadSession = new AwaitableDelegateCommand<int>(

                executeMethod: (MaxParallel) =>
               {
                   return Task.Run(() =>
                   {
                       DownloadQueueProcessor dq = new DownloadQueueProcessor();
                       dq.Initialize(this);
                       dq.Start(MaxParallel);
                       //score.SegmentedDownload(oS);
                   });
               },
                 canExecuteMethod: (oS) =>
                {
                    return true;
                }
            );
            CmdRefreshSession = new SimpleCommand<Session>()
            {
                ExecuteDelegate = (oS) =>
                {
                    oS.NotifyPropertyChanged("Size");
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

        private BindingList<DownloadQueueItem> _DownloadQueue = new BindingList<DownloadQueueItem>();

        public BindingList<DownloadQueueItem> DownloadQueue
        {
            get { return _DownloadQueue; }
            set
            {
                if (_DownloadQueue != value)
                {
                    _DownloadQueue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _Progress = 0;
        public int Progress
        {
            get { return _Progress; }
            set
            {
                if (_Progress != value)
                {
                    _Progress = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _State;

        public int State
        {
            get { return _State; }
            set
            {
                if (_State != value)
                {
                    _State = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public AwaitableDelegateCommand<int> CmdDownloadSession { get; set; }
        public SimpleCommand<Session> CmdAbortSession { get; set; }
        public SimpleCommand<Session> CmdRefreshSession { get; set; }
        public SimpleCommand<Session> CmdXSession { get; set; }

    }
}
