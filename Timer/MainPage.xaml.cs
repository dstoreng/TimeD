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

namespace Timer
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static readonly String FILENAME = "timers.xml";
        private readonly int CAPASITY = 10;
        private int numStopwatches;
        public static int timers;
        public static String newDescription;
        public static String newTimespan;
        public static Boolean doNotify;

        private Stopwatch stopwatch;
        private Stack<String> stopwatchList;
        private static List<Event> persistentList;
        private static Event[] timerList;
        private static Boolean loadAppState;

        private Thickness marginText;
        private Thickness marginToggle;

        public MainPage()
        {
            InitializeComponent();

            numStopwatches = 0;
            timers = 0;
            timerList = new Event[CAPASITY];
            stopwatchList = new Stack<String>();
            stopwatch = new Stopwatch();

            marginText = new Thickness(0, 18, 0, 0);
            marginToggle = new Thickness(0, 15, 0, 0);

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
                    timerList[i] = obj;
                }
                addTimers(timerList);
               
            } // Navigated from new timer window, add timer
            else if (newTimespan != null)
            {
                addTimer();
                SaveData();
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

        private void addTimer()
        {
            GenerateGridUI();
            Event e = new Event(newTimespan, newDescription, doNotify);
            timerList[timers] = e;
            timers++;
            newDescription = null;
            newTimespan = null;
        }

        private void addTimers(Event[] timerList)
        {
            foreach (Event e in timerList)
            {
                if (null != e)
                {
                    GenerateGridUI(e);
                    timers++;
                }
            }
        }

        private void addTimer_Click(object sender, EventArgs e)
        {
            if (timers < CAPASITY)
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
            Coding4Fun.Toolkit.Controls.TimeSpanPicker time = (Coding4Fun.Toolkit.Controls.TimeSpanPicker)FindName("timerPicker" + senderIndex);

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
            }
        }

        private void Timer_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch s = (ToggleSwitch)sender;
            int senderName = getSenderIndex(s.Name);
            Coding4Fun.Toolkit.Controls.TimeSpanPicker time = (Coding4Fun.Toolkit.Controls.TimeSpanPicker) FindName("timerPicker" + senderName);
            TextBlock block = (TextBlock) FindName("timerBlock" + senderName);

            block.Visibility = Visibility.Collapsed;
            time.Visibility = Visibility.Visible;
            timerList[senderName].Stop();
        }

        void Timer_Tick(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Timer timer = (Timer)sender;

            //Regular tick, update text in correct textblock
            if (e.PropertyName == "Value")
            {
                String targetTextblock = "timerBlock" + timer.id;
                TextBlock block = (TextBlock)FindName(targetTextblock);

                if (block.Visibility != Visibility.Visible)
                    block.Visibility = Visibility.Visible;

                block.Text = Convert.ToString(timer.Value);
            }

            // Timer stopped, reset toggle switch
            if (e.PropertyName == "Finished")
            {
                String targetToggle = "toggle" + timer.id;
                ToggleSwitch ts = (ToggleSwitch)FindName(targetToggle);
                ts.IsChecked = false;
            }

        }

        private void stopwatchStartPressed(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!stopwatch.Running)
            {
                stopwatch = new Stopwatch();
                stopwatch.PropertyChanged += sw_Property_Changed;
                stopwatch.Start();
            }
            if(stopwatch.Paused)
            {
                stopwatch.Resume();
            }
        }

        private void stopwatchStopPressed(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (stopwatch.Running)
            {
                stopwatch.Split();
                numStopwatches++;
                String lap = stopwatch.GetLap().ToString(@"mm\:ss\:ff");
                String total = stopwatch.GetTotal().ToString(@"mm\:ss\:ff");
                stopwatchList.Push(numStopwatches.ToString() + " - " + lap + "  " + total);

                SwElements.ItemsSource = null;
                SwElements.ItemsSource = stopwatchList;
            }
        }

        private void stopwatchClearPressed(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Second press
            if (stopwatch.Running && stopwatch.Paused)
            {
                stopwatch.Stop();
                stopwatchList = new Stack<String>();
                SwElements.ItemsSource = stopwatchList;
                TimeSinceLast.Text = "00:00:00";
                SWText.Text = "00:00:00";
                numStopwatches = 0;
                return;
            }
            // First press
            if (stopwatch.Running)
            {
                stopwatch.Pause();
            }
        }

        void sw_Property_Changed(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                SWText.Text = stopwatch.Value.ToString(@"mm\:ss\:ff");
            }

            if (e.PropertyName == "LastValue")
            {
                TimeSinceLast.Text = stopwatch.LastValue.ToString(@"mm\:ss\:ff");
            }
        }

        private int getSenderIndex(String name)
        {
            Char ch = name[name.Length - 1];
            return (int)char.GetNumericValue(ch);
        }

        private void removeTimer_Click(object sender, EventArgs e)
        {
            if (timers > 0)
            {        
                timerList[timers - 1].Stop();
                timerList[timers - 1] = null;
                stackpanelTimer.Children.RemoveAt(timers - 1);
                timers--;

                SaveData();
            }
        }

        public async static void SaveData()
        {
            await IsolatedStorageOperations.ClearAppData(FILENAME);
            persistentList = new List<Event>();
            foreach (Event e in timerList)
            {
                persistentList.Add(e);
            }
            await persistentList.Save(FILENAME);
        }

        public async static void LoadData()
        {
            loadAppState = true;
            persistentList = await IsolatedStorageOperations.Load<List<Event>>(FILENAME);
        }

        private void GenerateGridUI(Event ev = null)
        {
            Grid grid = new Grid();
            ToggleSwitch toggle = new ToggleSwitch();
            TextBlock txtBlock = new TextBlock();
            TextBlock timeBlock = new TextBlock();
            TimeSpan timespanText = new System.TimeSpan(0, 0, 30);
            Coding4Fun.Toolkit.Controls.TimeSpanPicker picker = new Coding4Fun.Toolkit.Controls.TimeSpanPicker();
        
            //Remove spam text, Event handlers for toggle switch, Set default values
            toggle.Content = "";
            toggle.Checked += Timer_Checked;
            toggle.Unchecked += Timer_Unchecked;
            txtBlock.Text = (null == ev) ? newDescription : ev.Timer.Name;

            picker.Value = (null == ev) ? System.TimeSpan.Parse(newTimespan) : ev.Timespan;

            txtBlock.FontSize = 30;
            timeBlock.FontSize = 30;
            timeBlock.FontWeight = FontWeights.Light;
            txtBlock.Margin = marginText;
            timeBlock.Margin = marginText;
            toggle.Margin = marginToggle;

            //These will be used to identify the (object) senders
            toggle.Name = "toggle" + timers;
            picker.Name = "timerPicker" + timers;
            txtBlock.Name = "txtBlock" + timers;
            timeBlock.Name = "timerBlock" + timers;
            grid.Name = "grid" + timers;

             //Grid definitions
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            ColumnDefinition col3 = new ColumnDefinition();
            col1.Width = new GridLength(1, GridUnitType.Star);
            col2.Width = new GridLength(1, GridUnitType.Star);
            col3.Width = new GridLength(1, GridUnitType.Star);
            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);
            grid.ColumnDefinitions.Add(col3);

            Grid.SetColumn(picker, 1);
            Grid.SetColumn(timeBlock, 1);
            Grid.SetColumn(txtBlock, 0);
            Grid.SetColumn(toggle, 2);
            grid.Children.Add(timeBlock);
            grid.Children.Add(txtBlock);
            grid.Children.Add(toggle);
            grid.Children.Add(picker);
            stackpanelTimer.Children.Add(grid);
        }
    }
}