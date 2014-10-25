using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Timer.Model
{
    class StopwatchInstance
    {
        private int _number;
        private String _split;
        private String _total;

        public int Number
        {
            get { return _number; }
            set
            {
                if (value != _number)
                {
                    _number = value;
                    OnPropertyChanged("number");
                }
            }
        }

        public String Split
        {
            get { return _split; }
            set
            {
                if (value != _split)
                {
                    _split = value;
                    OnPropertyChanged("split");
                }
            }
        }

        public String Total
        {
            get { return _total; }
            set
            {
                if (value != _total)
                {
                    _total = value;
                    OnPropertyChanged("total");
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
