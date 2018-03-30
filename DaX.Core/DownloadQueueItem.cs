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

        private long _RangeStart;

        public long RangeStart
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

        private long _RangeEnd;

        public long RangeEnd
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
                    NotifyPropertyChanged();
                    _Session.OnStateChanged += (sender, e) => State = e.newState;
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
            CmdAbortQItem = new SimpleCommand
            {
                CanExecuteDelegate = (x) => { return true; },
                ExecuteDelegate = (x) =>
                 {
                     Session.Abort();
                 }
            };
        }

        private void ProgressTimerCallback(object sender, EventArgs e)
        {
            if (Session == null)
            {
                return;
            }
            long dprogress = (long)FiddlerPropertyInfo._PeekDownloadProgress.GetValue(Session.oResponse);
            if (dprogress < 0)
            {
                dprogress = 0;
            }
            if (contentlength == default(long))
            {
                if (Session.oResponse["Content-Length"].Length > 0 && long.TryParse(Session.oResponse["Content-Length"], out contentlength))
                {
                    if (contentlength == 0)
                    {
                        contentlength = long.MaxValue;
                    }
                    var newProgress = Convert.ToInt32((dprogress * 100.0) / (contentlength));
                    Progress = Math.Max(Progress, newProgress);
                }
            }
            else
            {
                var newProgress = Convert.ToInt32((dprogress * 100.0) / (contentlength));
                Progress = Math.Max(Progress, newProgress);
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

        public SimpleCommand CmdDownloadQItem { get; set; }
        public SimpleCommand CmdAbortQItem { get; set; }

        DispatcherTimer ProgressTimer;

        public object LockObject = new object();
        private long contentlength = default(long);
    }


    internal static class FiddlerPropertyInfo
    {
        internal static PropertyInfo _PeekDownloadProgress = typeof(Fiddler.ServerChatter).GetProperty("_PeekDownloadProgress", BindingFlags.NonPublic | BindingFlags.Instance);
    }
}
