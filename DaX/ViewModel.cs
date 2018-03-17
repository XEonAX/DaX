using AEonAX.Shared;
using System.ComponentModel;
using System;

namespace DaX
{
    public class ViewModel : NotifyBase
    {
        Core DaXCore = new Core();
        public BindingList<Session> Sessions { get; set; } = new BindingList<Session>();
        Stitcher sticher = new Stitcher();
        public ViewModel()
        {
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
        }

        private void DaXCore_ResponseHeadersAvailable(object sender, SessionEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Sessions.Add(e.Session);
            });
        }

        public SimpleCommand CmdMergeFiles { get; set; }
        public SimpleCommand CmdClear { get; set; }
    }
}