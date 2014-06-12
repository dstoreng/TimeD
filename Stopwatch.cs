using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;

namespace Timer
{
    public class Stopwatch : INotifyPropertyChanged
    {
        /*
         * Creds to WindowsPhoneGeek
         * http://www.geekchamp.com/articles/building-a-windows-phone-countdown-timer-app-step-by-step
         */

        private DispatcherTimer _timer;
        private TimeSpan _value;
        public TimeSpan Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private DateTime _startTime;

        public void Start(TimeSpan chosenTime)
        {
            if (_timer == null)
            {
                _chosenTime = chosenTime;
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(100);
                _timer.Tick += TimerTick;
            }

            _startTime = DateTime.Now;
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var start = _endTime - DateTime.Now;
            int startSec = (int)start.TotalSeconds;
            this.Value = TimeSpan.FromSeconds(restSec);
        }

        public void Stop()
        {
            _timer.Stop();
            _startTime = DateTime.MinValue;
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
