using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;

namespace Timer
{
    public class Timer : INotifyPropertyChanged
    {
        /*
         * Creds to WindowsPhoneGeek
         * http://www.geekchamp.com/articles/building-a-windows-phone-countdown-timer-app-step-by-step
         */
        public event PropertyChangedEventHandler PropertyChanged;
        public String id { get; set; }
        public String Name { get; set; }
        private DateTime _endTime;
        private TimeSpan _chosenTime;
        private TimeSpan _value;
        private DispatcherTimer _timer;
        private Boolean _done;

        public TimeSpan Value
        {
            get{ return _value; }
            set{ _value = value;
                OnPropertyChanged("Value");
            }
        }

        private Boolean Done
        {
            set
            {
                _done = value;
                OnPropertyChanged("Finished");
            }
        }

        public void Start()
        {
            this._done = false;

            if(_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(100);
                _timer.Tick += TimerTick;
            }
            if(_endTime == DateTime.MinValue)
            {
                _endTime = DateTime.Now + _chosenTime;
            }

            _timer.Start();
        }

        public void setTimespan(TimeSpan time)
        {
            _chosenTime = time;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var rest = _endTime - DateTime.Now;
            int restSec = (int)rest.TotalSeconds;
            this.Value = TimeSpan.FromSeconds(restSec);

            if (restSec <= 0)
            {
                this.Stop();
                this.Done = true;
            }
        }

        public void Stop()
        {
            if (null != _timer)
            {
                _timer.Stop();
                _endTime = DateTime.MinValue;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
