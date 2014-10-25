using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Scheduler;
using System.Xml;

namespace Timer
{
    public class Event : INotifyPropertyChanged
    {
        private Timer _timer;
        private Alarm _alarm;
        public String id { get; set; }
        public String Name { get { return _alarm.Content; } }
        public TimeSpan Timespan { get; set; }
        public Boolean Notify { get; set; }
        public Boolean Enabled { get; set; }
        public String TimespanSerialisable
        {
            get { return XmlConvert.ToString(Timespan); }
            set
            {
                Timespan = string.IsNullOrEmpty(value) ?
                    TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
            }
        }

        public String NotifyConverter
        {
            get 
            { 
                String msg = (Notify) ? "On" : "Off";
                return msg; 
            }
        }

        public Alarm Alarm
        {
            get { return _alarm; }
        }
        public Timer Timer
        {
            get { return _timer; }
            set { _timer = value; }
        }

        public Event() { }

        public Event(String timespan, String description, Boolean notify)
        {
            Timespan = System.TimeSpan.Parse(timespan);
            _alarm = new Alarm(Guid.NewGuid().ToString());
            _alarm.Content = description;
            _alarm.BeginTime = DateTime.Now + Timespan;
            _alarm.RecurrenceType = RecurrenceInterval.None;

            _timer = new Timer();
            _timer.Name = description;

            Notify = notify;
        }

        // This will cause nullpointer if not updated at every toggleswitch
        public void setDetails(TimeSpan time, String id)
        {
            _alarm.BeginTime = DateTime.Now + time;
            _timer.id = id;
        }

        public void Stop()
        {
            _timer.Stop();
            this.Enabled = false;
            if (Notify)
            {
                ScheduledActionService.Remove(this.Name);
            }
        }

        public void Start()
        {
            _timer.Start();
            this.Enabled = true;
            if (Notify)
            {
                ScheduledActionService.Add(_alarm);
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

        // Får ikke serialisert alarm objektet, så blir litt =GER=
        public void SerializationDone()
        {
            _alarm = new Alarm(Name);
            _alarm.Content = Timer.Name;
            _alarm.BeginTime = DateTime.Now + Timespan;
            _alarm.RecurrenceType = RecurrenceInterval.None;
        }
    }
}
