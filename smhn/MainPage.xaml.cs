using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using SmhnLib;

namespace smhn
{
    public partial class MainPage : PhoneApplicationPage
    {
        // コンストラクター
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            GetNews();
        }

        // ViewModel Items のデータを読み込みます
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            /*if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }*/
        }

        private void GetNews()
        {
            var url = new Uri("http://feeds.feedburner.com/AlphaDevice?format=xml");
            WebClient wc = new WebClient();
            
            Observable.FromEvent<DownloadStringCompletedEventArgs>(wc, "DownloadStringCompleted")
                .ObserveOn(Scheduler.ThreadPool)
                .Select(e => 
                    XElement.Parse(e.EventArgs.Result))
                .Select(x => x.Descendants("item").Select(xe => new NewsItemClass(xe)))
                .ObserveOnDispatcher() // UIスレッドに戻す
                .Subscribe(list =>
                {
                    newsList.ItemsSource = list;
                });

            wc.DownloadStringAsync(url);
        }

        private void newsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            /*PhoneApplicationService.Current.State["title"] = item.Title;
            PhoneApplicationService.Current.State["link"] = item.Link;*/
            if (newsList.SelectedIndex == -1)
                return;
            var item = newsList.Items[newsList.SelectedIndex];
            PhoneApplicationService.Current.State["item"] = item;
            newsList.SelectedIndex = -1;
            NavigationService.Navigate(new Uri("/Article.xaml", UriKind.RelativeOrAbsolute));
            /*
            //仮のWebブラウザで開くアレ
            var task = new Microsoft.Phone.Tasks.WebBrowserTask();
            task.Uri = item.Link;
            task.Show();
            */
        }
    }
}