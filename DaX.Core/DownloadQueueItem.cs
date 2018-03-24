using AEonAX.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public SimpleCommand CmdDownloadSession { get; set; }
        public SimpleCommand CmdAbortSession { get; set; }
        public SimpleCommand CmdRefreshSession { get; set; }
    }
}
