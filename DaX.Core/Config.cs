using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEonAX.Shared;
namespace DaX
{
    public class Config : NotifyBase
    {

        private int _MaxParallel = 5;

        public int MaxParallel
        {
            get { return _MaxParallel; }
            set
            {
                if (_MaxParallel != value)
                {
                    _MaxParallel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private long _DeltaDivider = 500L;

        public long DeltaDivider
        {
            get { return _DeltaDivider; }
            set
            {
                if (_DeltaDivider != value)
                {
                    _DeltaDivider = value;
                    NotifyPropertyChanged();
                }
            }
        }


        private long _IncrementMultiplier = 1L;

        public long IncrementMultiplier
        {
            get { return _IncrementMultiplier; }
            set
            {
                if (_IncrementMultiplier != value)
                {
                    _IncrementMultiplier = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }
}
