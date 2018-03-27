using AEonAX.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace DaX
{
    public class DownloadQueueItem : NotifyBase
    {
        private string _Range;
        static PropertyInfo _PeekDownloadProgress = typeof(Fiddler.ServerChatter).GetProperty("_PeekDownloadProgress", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        public string Range
        {
            get { return _Range; }
            set
            {
                if (_Range != value)
                {
                    _Range = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _RangeStart;

        public int RangeStart
        {
            get { return _RangeStart; }
            set
            {
                if (_RangeStart != value)
                {
                    _RangeStart = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _RangeEnd;

        public int RangeEnd
        {
            get { return _RangeEnd; }
            set
            {
                if (_RangeEnd != value)
                {
                    _RangeEnd = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Fiddler.Session _Session;

        public Fiddler.Session Session
        {
            get { return _Session; }
            set
            {
                if (_Session != value)
                {
                    _Session = value;
                    _Session.OnStateChanged += (sender, e) => State = e.newState;
                    NotifyPropertyChanged();
                }
            }
        }

        private Fiddler.SessionStates _State = (Fiddler.SessionStates)(-1);

        public Fiddler.SessionStates State
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

        private bool? _Processed = false;

        public bool? Processed
        {
            get { return _Processed; }
            set
            {
                if (_Processed != value)
                {
                    _Processed = value;
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


        public DownloadQueueItem()
        {
            ProgressTimer = new DispatcherTimer();
            ProgressTimer.Interval = new TimeSpan(200);
            ProgressTimer.Tick += ProgressTimerCallback;

            PropertyChanged += DownloadQueueItem_PropertyChanged;
        }

        private void ProgressTimerCallback(object sender, EventArgs e)
        {
            if (Session == null)
            {
                return;
            }
            long dprogress = (long)_PeekDownloadProgress.GetValue(Session.oResponse);
            if (dprogress < 0)
            {
                dprogress = 0;
            }
            long clength = default(long);
            if (Session.oResponse["Content-Length"].Length > 0 && long.TryParse(Session.oResponse["Content-Length"], out clength))
            {
                if (clength == 0)
                {
                    clength = long.MaxValue;
                }
                var prog = Convert.ToInt32((dprogress * 100.0) / (clength));
                Progress = Progress < prog ? prog : Progress;
            }
            else
            {
                //ProgressTimer.Stop();
            }
        }

        private void DownloadQueueItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Processed))
            {
                if (Processed == null && !ProgressTimer.IsEnabled)
                {
                    ProgressTimer.Start();
                }
                else if (Processed == true)
                {
                    ProgressTimer.Stop();
                    Progress = 100;
                }
            }
        }

        public SimpleCommand CmdDownloadSession { get; set; }
        public SimpleCommand CmdAbortSession { get; set; }
        public SimpleCommand CmdRefreshSession { get; set; }

        DispatcherTimer ProgressTimer;

    }
}
