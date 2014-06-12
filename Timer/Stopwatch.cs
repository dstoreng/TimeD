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
        private readonly int UPDATE_FREQ = 50;
        private DispatcherTimer _timer;
        private TimeSpan _value;
        private TimeSpan _lastValue;
        private DateTime _startTime;
        private DateTime _pauseStart;
        private DateTime _pauseStop;
        private Boolean _running;
        private Boolean _paused;
        private TimeSpan _prevValue;
        private TimeSpan _lapValue;
        public Boolean Paused { get { return _paused; } }

        public TimeSpan Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public TimeSpan LastValue
        {
            get { return _lastValue; }
            set
            {
                _lastValue = value;
                OnPropertyChanged("LastValue");
            }
        }

        public Boolean Running
        {
            get
            {
                return _running;
            }
        }

        public void Start()
        {
            if (_timer == null)
            {
                _running = true;
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(UPDATE_FREQ);
                _timer.Tick += TimerTick;               
            }
            _paused = false;
            _startTime = DateTime.Now;
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var start = DateTime.Now - _startTime;
            decimal startSec = (decimal)start.TotalSeconds;
            this.Value = TimeSpan.FromSeconds(Convert.ToDouble(startSec));
            this.LastValue = _value - _prevValue;
        }

        public void Resume()
        {
            _paused = false;
            _pauseStop = DateTime.Now;
            _startTime = _startTime + (_pauseStop - _pauseStart);
            _timer.Start();
        }

        public void Pause()
        {
            _paused = true;
            _pauseStart = DateTime.Now;
            _timer.Stop();
        }

        public void Stop()
        {
            _timer.Stop();
            _running = false;
            _startTime = DateTime.MinValue;
        }

        public void Split()
        {
            _lapValue = _value - _prevValue;
            _prevValue = _value;
        }

        public TimeSpan GetLap()
        {
            return _lapValue;
        }

        public TimeSpan GetTotal()
        {
            return _prevValue;
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
