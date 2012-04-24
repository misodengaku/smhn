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
using Microsoft.Phone.Reactive;
using SmhnLib;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace smhn
{
    public partial class Article : PhoneApplicationPage
    {
        NewsItemClass item;

        public Article()
        {
            InitializeComponent();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            item = (NewsItemClass)PhoneApplicationService.Current.State["item"];
            PageTitle.Text = item.Title;
            GetArticle(item.Link);
        }

        private void GetArticle(Uri url)
        {
            //var url = new Uri(_url);
            WebClient wc = new WebClient();

            Observable.FromEvent<DownloadStringCompletedEventArgs>(wc, "DownloadStringCompleted")
                .ObserveOn(Scheduler.ThreadPool)
                .Select(e => e.EventArgs.Result)
                .ObserveOnDispatcher() // UIスレッドに戻す
                .Subscribe(doc =>
                {
                    Color AccentColor = (Color)Application.Current.Resources["PhoneAccentColor"];
                    Color BackColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
                    Color TextColor = (Color)Application.Current.Resources["PhoneForegroundColor"];
                    /*string[] del1 = { "mycontent" };
                                        //"<br class=\"wp_social_bookmarking_light_clear\">" };
                    string[] del2 = { "<a href=\"http://www.amazon.co.jp" };*/
                    var html = System.Text.RegularExpressions.Regex.Split(doc, "<br class='wp_social_bookmarking_light_clear' />").Last();
                    //html = System.Text.RegularExpressions.Regex.Split(doc, "").Last();
                    //"<div class='wp_social_bookmarking_light'><div>.*/></a></p>").Last();

                    html = System.Text.RegularExpressions.Regex.Split(html, "(<a href=\"http://www.amazon.co.jp|<div class=\"clear\">)").First();
                    html = ConvertExtendedASCII(html);
                    var code = "<html><style type=\"text/css\"><!--\n";
                    code += "body {background-color: " + ColorToHtml(BackColor) + "}\n";
                    code += "* {color: " + ColorToHtml(TextColor) + "}\n";
                    code += "a {color: " + ColorToHtml(AccentColor) + "}\n";
                    code += "img {border-style: none}\n";
                    code += "--></style><body>\n";
                    code += html;
                    webBrowser1.NavigateToString(code);
                    webBrowser1.Navigating += ((sender, e) =>
                    {

                        if (MessageBox.Show("InternetExplorerで別ページを表示します。\n「キャンセル」でページ移動を中止します。", "ページ移動", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            Dispatcher.BeginInvoke(() =>
                            {
                                var task = new Microsoft.Phone.Tasks.WebBrowserTask();
                                task.Uri = e.Uri;
                                task.Show();
                            });
                        }
                        e.Cancel = true;
                    });
                }, ex =>
                {
                    MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK);
                });

            wc.DownloadStringAsync(url);
        }

        private static string ConvertExtendedASCII(string HTML)
        {
            string retVal = "";
            char[] s = HTML.ToCharArray();

            foreach (char c in s)
            {
                if (Convert.ToInt32(c) > 127)
                    retVal += "&#" + Convert.ToInt32(c) + ";";
                else
                    retVal += c;
            }

            return retVal;
        }

        private string ColorToHtml(Color c)
        {
            string html = "#";

            html += ((int)(c.R)).ToString("X2");
            html += ((int)(c.G)).ToString("X2");
            html += ((int)(c.B)).ToString("X2");

            return html;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ShareStatusTask shareStatusTask = new ShareStatusTask();

            shareStatusTask.Status = item.Title + ": "+item.Link;

            shareStatusTask.Show();
        }

    }
}