using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Timer.Model;
using System.Diagnostics;

namespace Timer
{
    class ViewModel
    {
        public ObservableCollection<Event> Timers;
        public ObservableCollection<StopwatchInstance> Stopwatches;
        public Stopwatch Stopwatch;
        public static ViewModel instance;
        
        private ViewModel()
        {
            Stopwatches = new ObservableCollection<StopwatchInstance>();
            Stopwatch = new Stopwatch();
            Timers = new ObservableCollection<Event>();
            
            Timers.CollectionChanged += Timers_CollectionChanged;
        }

        void Timers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("Collection changed, no handler..");
        }
        
        public static ViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new ViewModel();
            }

            return instance;
        }

        public void StopwatchStart()
        {
            if (Stopwatch != null)
            {
                if (Stopwatch.Paused)
                {
                    Stopwatch.Resume();
                    return;
                }
                if (!Stopwatch.Running)
                {
                    Stopwatch.Start();
                }
                else
                {
                    Stopwatch.Pause();
                }
            }
            
        }

        public void StopwatchSplit()
        {
            if (Stopwatch.Running)
            {
                Stopwatch.Split();

                StopwatchInstance splitTime = new StopwatchInstance();
                splitTime.Number = Stopwatches.Count + 1;
                splitTime.Split = Stopwatch.GetSplit().ToString(@"mm\:ss\:ff");
                splitTime.Total = Stopwatch.GetTotal().ToString(@"mm\:ss\:ff");

                Stopwatches.Add(splitTime);
            }
        }
        /// <summary>
        /// Pauses or completely stops the stopwatch depending on current state.
        /// @return true if the stopwatch was cleared, false if paused
        /// </summary>
        public bool StopwatchStopOrClear()
        {
            // First press
            if (Stopwatch.Running && !Stopwatch.Paused)
            {
                Stopwatch.Pause();
                return false;
            }
            else
            {
                Stopwatch = null;
                Stopwatch = new Stopwatch();

                Stopwatches.Clear();
                return true;
            }
        }

        public void AddTimer(String name, String timeSpan, Boolean notify)
        {
            var obj = new Event(timeSpan, name, notify);
            Timers.Add(obj);
        }

        public void AddTimers(List<Event> list)
        {
            foreach (Event ev in list)
            {
                Timers.Add(ev);
            }
        }

        public void PopLastTimer()
        {
            if(Timers.Count > 0)
                Timers.RemoveAt(Timers.Count - 1);
        }


    }
}
