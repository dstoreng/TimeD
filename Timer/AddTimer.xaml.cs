﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Timer
{
    public partial class AddTimer : PhoneApplicationPage
    {
        private ViewModel viewModel;
        public AddTimer()
        {
            InitializeComponent();

            viewModel = ViewModel.GetInstance();

            TimespanPicker.Value = new System.TimeSpan(0, 1, 0);
            Textbox.Text = "Timer";
        }

        private void saveClick(object sender, EventArgs e)
        {
            String timespan = TimespanPicker.Value.ToString();
            String description = Textbox.Text;
            Boolean notify = (Boolean)ToggleswitchNotify.IsChecked;

            viewModel.AddTimer(description, timespan, notify);

            if(NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

    }
}