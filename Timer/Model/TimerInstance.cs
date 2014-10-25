using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Timer.Model
{
    class TimerInstance
    {
        private String _name;
        private TimeSpan _timespan;
        private Boolean _doNotify;

        public String Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public TimeSpan Timespan
        {
            get { return _timespan; }
            set
            {
                if (value != _timespan)
                {
                    _timespan = value;
                    OnPropertyChanged("Timespan");
                }
            }
        }

        public Boolean DoNotify
        {
            get { return _doNotify; }
            set
            {
                if (value != _doNotify)
                {
                    _doNotify = value;
                    OnPropertyChanged("DoNotify");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
