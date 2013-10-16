using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace TwitchTV_JSON
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<TwitchStream> streamlist = new List<TwitchStream>();
        public MainWindow()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            streamlist.Clear();
            streamview.Items.Clear();
            string uri = "http://api.justin.tv/api/stream/list.xml?meta_game=Dota+2";
            var xmlDocument = XDocument.Load(uri);
            foreach (XElement stream in xmlDocument.Descendants("stream"))
            {
                try
                {
                    TwitchStream temp = new TwitchStream();
                    if (stream.Element("title").Value != null)
                        temp.Title = stream.Element("title").Value;
                    if (stream.Element("channel").Element("login").Value != null)
                        temp.ChannelOwner = stream.Element("channel").Element("login").Value;
                    if (stream.Element("channel_count").Value != null)
                        temp.ViewerCount = Convert.ToInt32(stream.Element("channel_count").Value);
                    if (stream.Element("channel").Element("screen_cap_url_large").Value != null)
                        temp.image = stream.Element("channel").Element("screen_cap_url_large").Value;
                    streamlist.Add(temp);
                }
                catch
                {

                }
            }
            List<TwitchStream> SortedStreams = streamlist.OrderByDescending(o => o.ViewerCount).ToList<TwitchStream>();
            foreach (TwitchStream stream in SortedStreams)
            {
                Button temp = new Button();
                temp.Content = stream.ChannelOwner;
                temp.ToolTip = stream.Title + Environment.NewLine + stream.ViewerCount.ToString();
                temp.Height = 50;
                temp.Width = 150;
                temp.Click += temp_Click;
                streamview.Items.Add(temp);
            }
        }

        void temp_Click(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            TwitchStream stream = streamlist[0];
            foreach (TwitchStream get in streamlist)
                if (get.ChannelOwner == name)
                    stream = get;
            text_owner.Text = "Owner: " + stream.ChannelOwner;
            text_title.Text = "Title: " + stream.Title;
            text_title.ToolTip = stream.Title;
            text_viewer.Text = "Viewers: " + stream.ViewerCount.ToString();
            this.DataContext = stream.image;
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        public class TwitchStream
        {
            public string Title { get; set; }
            public string ChannelOwner { get; set; }
            public int ViewerCount { get; set; }
            public string image { get; set; }
        }

        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            string quality = "mobile_high";
            foreach (RadioButton button in Quality.Children)
            {
                if ((bool)button.IsChecked)
                    quality = button.Content.ToString();
            }
            Process.Start("CMD", "/C livestreamer.exe -url twitch.tv/" + text_owner.Text.Remove(0, 7) + " " + quality);
        }
    }
}
