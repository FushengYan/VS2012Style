using Overlay;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms.Integration;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WindowControls;
using Forms = System.Windows.Forms;

namespace VS2012
{
    /// <summary>
    /// StateIndicator(State.Free, string display on footer);
    /// SetHeaderText(string);
    /// ThemeColorChange(ThemeColor.Blue);
    /// ThemeChange(Theme.Light);
    /// </summary>
    public static class ExtensionMethods
    {
        private static Action EmptyDelegate = delegate() { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }

        public static void RefreshInput(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Input, EmptyDelegate);
        }
    }

    public partial class MainWindow : CustomWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.SetHeaderText("User: " + Environment.UserName);
        }

        public struct MyData
        {
            public int id { set; get; }

            public string colnum { set; get; }

            public int data { set; get; }

            public DateTime lastrun { set; get; }

            public DateTime nextrun { set; get; }
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate()
            {
            }, DispatcherPriority.ApplicationIdle);
            ComboBox box = sender as ComboBox;
            this.ThemeChange((Theme)box.SelectedIndex);
        }

        private void ComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            string val = ((ContentControl)(box.SelectedItem)).Content.ToString();
            this.StateIndicator((State)box.SelectedIndex, "VS2012Style Demo V.0.0.1");
        }

        private void ComboBox_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            string val = ((ContentControl)(box.SelectedItem)).Content.ToString();
            this.ThemeColorChange((ThemeColor)box.SelectedIndex);
        }

        private void CustomWindow_Activated(object sender, EventArgs e)
        {
            this.Opacity = 0;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int tb = TabControls.SelectedIndex;
            if (tb == 2)
            {
                this.Dispatcher.BeginInvoke((Action)delegate()
                {
                    Forms.WebBrowser browser = new Forms.WebBrowser();
                    browser.Name = "browser";
                    browser.ScriptErrorsSuppressed = true;
                    browser.Navigate(new Uri("http://google.com"));
                    new OverlayWindow(BrowserPanel, browser);
                }, DispatcherPriority.ApplicationIdle);
            }
            else
            {
                foreach (Window win in App.Current.Windows)
                {
                    if (!win.IsFocused && win.Name.ToString() == "BrowserWindow")
                    {
                        WindowsFormsHost wh = (WindowsFormsHost)((Overlay.OverlayWindow)(win)).FindName("wfh");
                        Forms.WebBrowser wb = (Forms.WebBrowser)(wh.Child);
                        wb.Navigate(new Uri("about:blank"));
                        win.Close();
                    }
                }
            }

        }
       
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            DataGridTextColumn col2 = new DataGridTextColumn();
            DataGridTextColumn col3 = new DataGridTextColumn();
            DataGridTextColumn col4 = new DataGridTextColumn();
            DataGridTextColumn col5 = new DataGridTextColumn();
            DG.Columns.Add(col1);
            DG.Columns.Add(col2);
            DG.Columns.Add(col3);
            DG.Columns.Add(col4);
            DG.Columns.Add(col5);
            col1.Binding = new Binding("id");
            col2.Binding = new Binding("colnum");
            col3.Binding = new Binding("data");
            col4.Binding = new Binding("lastrun");
            col5.Binding = new Binding("nextrun");
            col1.Header = "ID";
            col2.Header = "Colnum";
            col3.Header = "data";
            col4.Header = "lastrun";
            col5.Header = "nextrun";
            for (int i = 0; i <= 100; i++)
            {
                DG.Items.Add(new MyData { id = i, colnum = "Col - " + i.ToString(), data = i * 2, lastrun = DateTime.Now, nextrun = DateTime.Now });
            }
            this.StateIndicator(State.Busy, "VS2012Style Demo V.0.0.1");
            Application.Current.Dispatcher.BeginInvoke((Action)delegate()
            {
                var th = (this.Opacity == 1) ? 0 : 1;
                var anim = new DoubleAnimation(th, (Duration)TimeSpan.FromMilliseconds(150));
                anim.Completed += (s, _) => StateIndicator(State.Free, "VS2012Style Demo V.0.0.1");
                this.BeginAnimation(UIElement.OpacityProperty, anim);
            }, DispatcherPriority.Render);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            b.IsEnabled = false;
            prg.Maximum = prg.ActualWidth;
            int to = Convert.ToInt32(prg.ActualWidth);
            for (int i = 0; i <= to; i++)
            {
                await Disp(10);
                prg.Value = i;
            }
            b.IsEnabled = true;
        }

        private Task Disp(int i)
        {
            return Task.Run(() =>
            {
                Thread.Sleep(i);
            });
        }

    }
}