﻿using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Scheduler;
using System.IO;
using System.IO.IsolatedStorage;
using System.Diagnostics;

namespace Timer
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static readonly String FILENAME = "timers.xml";
        private readonly int CAPASITY = 10;

        private static List<Event> persistentList;
        private static Boolean loadAppState;

        private ViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();

            viewModel = ViewModel.GetInstance();
            viewModel.Stopwatch.PropertyChanged += sw_Property_Changed;
            ItemsControlStopwatches.ItemsSource = viewModel.Stopwatches;
            ItemsControlTimers.ItemsSource = viewModel.Timers;
         }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            /*
             * Load saved application state
             */
            if (loadAppState)
            {
                loadAppState = false;

                for (int i = 0; i < persistentList.Count; i++)
                {
                    var obj = persistentList.ElementAt(i);
                    if (null != obj)
                        obj.SerializationDone();
                }
                viewModel.AddTimers(persistentList);
               
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    ApplicationBar.IsVisible = false;
                    break;

                case 1:
                    ApplicationBar.IsVisible = true;
                    break;
            }
        }

        private void addTimer_Click(object sender, EventArgs e)
        {
            if (viewModel.Timers.Count < CAPASITY)
            {
                NavigationService.Navigate(new Uri("/AddTimer.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Cannot add any more timers.");
            }

        }

        private void Timer_Checked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch s = (ToggleSwitch)sender;
            int senderIndex = getSenderIndex(s.Name);

            Event clickedEvent = ((ToggleSwitch)sender).Tag as Event;
            /*
            if (time != null)
            { 
                // Hide timespan and set id for the event so we can access it later
                // Create event handler
                time.Visibility = Visibility.Collapsed;
                timerList[senderIndex].id = senderIndex.ToString();
                timerList[senderIndex].Timer.PropertyChanged += Timer_Tick;
                
                 //  Timespan can change between creating it and starting the alarm
                 //  Check for updates
                TimeSpan span = (TimeSpan)time.Value;
                timerList[senderIndex].Timer.setTimespan(span);
                timerList[senderIndex].setDetails(span, senderIndex.ToString());
                timerList[senderIndex].Start();
            }*/
        }

        private void Timer_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch s = (ToggleSwitch)sender;
            Event targetEvent = ((ToggleSwitch)sender).Tag as Event;
            int senderName = getSenderIndex(s.Name);
            Coding4Fun.Toolkit.Controls.TimeSpanPicker time = (Coding4Fun.Toolkit.Controls.TimeSpanPicker) FindName("timerPicker" + senderName);
            TextBlock block = (TextBlock) FindName("timerBlock" + senderName);

            block.Visibility = Visibility.Collapsed;
            time.Visibility = Visibility.Visible;
            targetEvent.Stop();
        }

        private void stopwatchStartPressed(object sender, System.Windows.Input.GestureEventArgs e)
        {
            viewModel.StopwatchStart();
        }

        private void stopwatchStopPressed(object sender, System.Windows.Input.GestureEventArgs e)
        {
            viewModel.StopwatchSplit();
        }

        private void stopwatchClearPressed(object sender, System.Windows.Input.GestureEventArgs e)
        {
            bool clear = viewModel.StopwatchStopOrClear();
            Debug.WriteLine("Reset stopwatch: " + clear);
            if (clear)
            {       
                TimeSinceLast.Text = "00:00:00";
                SWText.Text = "00:00:00";

                viewModel.Stopwatch.PropertyChanged += sw_Property_Changed;
                
            }
        }

        void sw_Property_Changed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Debug.WriteLine(viewModel.Stopwatch.Value.ToString(@"mm\:ss\:ff"));
                SWText.Text = viewModel.Stopwatch.Value.ToString(@"mm\:ss\:ff");
            }

            if (e.PropertyName == "LastValue")
            {
                TimeSinceLast.Text = viewModel.Stopwatch.LastValue.ToString(@"mm\:ss\:ff");
            }
        }

        private int getSenderIndex(String name)
        {
            Char ch = name[name.Length - 1];
            return (int)char.GetNumericValue(ch);
        }

        private void removeTimer_Click(object sender, EventArgs e)
        {
            if (viewModel.Timers.Count > 0)
            {
                viewModel.PopLastTimer();
            }
        }

        public async static void SaveData()
        {
            try 
            { 
                await IsolatedStorageOperations.ClearAppData(FILENAME);
                //await viewModel.Timers.ToArray<Event>().Save(FILENAME);
            }
            catch (Exception) { }
            
        }

        public async static void LoadData()
        {
            loadAppState = true;
            persistentList = await IsolatedStorageOperations.Load<List<Event>>(FILENAME);
        }

        private void AlarmSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            Event ev = ((ToggleSwitch)sender).Tag as Event;
            ev.Start();
        }

        private void AlarmSwitch_Checked(object sender, RoutedEventArgs e)
        {
            Event ev = ((ToggleSwitch)sender).Tag as Event;
            ev.Start();
        }

    }
}