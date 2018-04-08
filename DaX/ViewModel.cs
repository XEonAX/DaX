using AEonAX.Shared;
using System.ComponentModel;
using System;

namespace DaX
{
    public class ViewModel : NotifyBase
    {
        Core DaXCore = new Core();
        public BindingList<Session> Sessions { get; set; } = new BindingList<Session>();
        private Session _DownloadDetail;

        public Session DownloadDetail
        {
            get { return _DownloadDetail; }
            set
            {
                if (_DownloadDetail != value)
                {
                    _DownloadDetail = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Config _Config;

        public Config Config
        {
            get { return _Config; }
            set
            {
                if (_Config != value)
                {
                    _Config = value;
                    NotifyPropertyChanged();
                }
            }
        }





        Stitcher sticher = new Stitcher();
        public ViewModel()
        {
            Config = new Config();
            DaXCore.ResponseHeadersAvailable += DaXCore_ResponseHeadersAvailable;
            CmdMergeFiles = new SimpleCommand
            {
                CanExecuteDelegate = (o) =>
                {
                    return true;
                },
                ExecuteDelegate = (o) =>
                {
                    sticher.StitchAll();
                }
            };


            CmdClear = new SimpleCommand
            {
                CanExecuteDelegate = (o) =>
                {
                    return true;
                },
                ExecuteDelegate = (o) =>
                {
                    Sessions.Clear();
                    GC.Collect();
                }
            };

            CmdViewDetails = new SimpleCommand<Session>
            {
                CanExecuteDelegate = (o) =>
                {
                    return true;
                },
                ExecuteDelegate = (o) =>
                {
                    DownloadDetail = o;
                }
            };
        }

        private void DaXCore_ResponseHeadersAvailable(object sender, SessionEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (e.Session.Size > 1000)
                {
                    e.Session.Config = Config;
                    Sessions.Add(e.Session);
                }
            });
        }

        public SimpleCommand CmdMergeFiles { get; set; }
        public SimpleCommand CmdClear { get; set; }
        public SimpleCommand<Session> CmdViewDetails { get; set; }
    }
}