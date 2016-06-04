using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace TaiwanInvoice
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainViewModel viewModel = null;
        public MainViewModel ViewModel
        {
            get
            {
                // Delay creation of the view model until necessary
                if (viewModel == null)
                {
                    viewModel = new MainViewModel();
                }

                return viewModel;
            }
        }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = ViewModel;
            ViewModel.LoadDataStarted += OnViewModelLoadDataStarted;
            ViewModel.LoadDataError += OnViewModelLoadDataError;
            ViewModel.LoadDataCompleted += OnViewModelLoadDataCompleted;
        }

        private void OnViewModelLoadDataCompleted()
        {
            MainProgressBar.Visibility = Visibility.Collapsed;
        }

        private void OnViewModelLoadDataError()
        {
            MessageBox.Show("資料查詢發生錯誤，請稍候再試，謝謝。");
        }

        private void OnViewModelLoadDataStarted()
        {
            MainProgressBar.Visibility = Visibility.Visible;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsDataLoaded)
            {
                ViewModel.LoadData();
            }
        }

        private void OnInfoListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = ((ListBox)sender).SelectedIndex;
            if (idx >= 0)
            {
                switch (idx)
                {
                    case 0:
                        ViewModel.LoadData(true);
                        break;
                    case 1:
                        {
                            WebBrowserTask webBrowserTask = new WebBrowserTask();
                            webBrowserTask.Uri = new Uri("http://invoice.etax.nat.gov.tw/etaxinfo_3.htm", UriKind.Absolute);
                            webBrowserTask.Show();
                        }
                        break;
                    case 2:
                        {
                            WebBrowserTask webBrowserTask = new WebBrowserTask();
                            webBrowserTask.Uri = new Uri("http://invoice.etax.nat.gov.tw/etaxinfo_1.htm", UriKind.Absolute);
                            webBrowserTask.Show();
                        }
                        break;
                    case 3:
                        {
                            WebBrowserTask webBrowserTask = new WebBrowserTask();
                            webBrowserTask.Uri = new Uri("http://invoice.etax.nat.gov.tw/etaxinfo_2.htm", UriKind.Absolute);
                            webBrowserTask.Show();
                        }
                        break;
                    case 4:
                        {
                            PhoneCallTask phoneCallTask = new PhoneCallTask();
                            phoneCallTask.PhoneNumber = "0223961651";
                            phoneCallTask.Show();
                        }
                        break;
                }

                ((ListBox)sender).SelectedItem = null;
            }
        }
    }
}