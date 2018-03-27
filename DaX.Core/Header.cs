using AEonAX.Shared;
using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaX
{
    public class Header : NotifyBase
    {
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _Value;

        public string Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public Header(HTTPHeaderItem item) : this(item.Name, item.Value)
        {

        }
    }
}
