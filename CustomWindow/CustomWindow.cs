using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Forms = System.Windows.Forms;

[assembly: ThemeInfo(ResourceDictionaryLocation.None, ResourceDictionaryLocation.SourceAssembly)]

namespace WindowControls
{
    public class CustomWindow : Window
    {
        private string LastFooterColor = "#007ACC";

        /// <summary>
        /// StateIndicator(State.Free, string display on footer);
        /// SetHeaderText(string);
        /// ThemeColorChange(ThemeColor.Blue);
        /// ThemeChange(Theme.Light);
        /// </summary>
        static CustomWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWindow),
                new FrameworkPropertyMetadata(typeof(CustomWindow)));
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 30 });
        }

        public enum State { [Description("#007ACC")] Free, [Description("#68217A")] Ready,[Description("#CA5100")] Busy,[Description("#E04343")] Error }

        public enum Theme { [Description("Light")] Light, [Description("Medium")] Medium, [Description("Dark")] Dark }

        public enum ThemeColor { [Description("#0079cb")] Blue, [Description("#CA5100")] Orange,[Description("#77bb44")] Green,[Description("#68217A")] Purple,  [Description("#009999")] OilGreen, }

        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float Alpha = (float)color.A;
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb((byte)Alpha, (byte)red, (byte)green, (byte)blue);
        }

        public override void OnApplyTemplate()
        {
            Button minimizeButton = GetTemplateChild("minimizeButton") as Button;
            if (minimizeButton != null)
                minimizeButton.Click += MinimizeClick;

            Button restoreButton = GetTemplateChild("restoreButton") as Button;
            if (restoreButton != null)
                restoreButton.Click += RestoreClick;

            Button closeButton = GetTemplateChild("closeButton") as Button;
            if (closeButton != null)
                closeButton.Click += CloseClick;

            DockPanel momoveRectangle = GetTemplateChild("Header") as DockPanel;
            if (momoveRectangle != null)
                momoveRectangle.MouseDown += moveRectangle_PreviewMouseDown;

            Window thisWindow = this as Window;
            if (thisWindow != null)
            {
                thisWindow.Activated += ActivateSate;
                thisWindow.Deactivated += DeactivSate;
            }

            DockPanel hd = GetTemplateChild("Header") as DockPanel;
            if (hd != null)
            {
                hd.MouseLeftButtonDown += RestoreClickHeadr;
            }
/*
            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/CustomWindow;component/Themes/Images/icon.png")).Stream;
            Forms.NotifyIcon ni = new Forms.NotifyIcon();
            ni = new Forms.NotifyIcon();
            var bitmap = new System.Drawing.Bitmap(iconStream); // or get it from resource
            var iconHandle = bitmap.GetHicon();
            var icon = System.Drawing.Icon.FromHandle(iconHandle);
            ni.Icon = icon;
            ni.Visible = true;
            ni.DoubleClick += delegate(object sender, EventArgs args)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
*/
            base.OnApplyTemplate();
        }
/*
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }
*/
        public void SetHeaderText(string text)
        {
            Resources["TitleText"] = text;
        }

        public void StateIndicator(Enum st, string text = "", string color = "", bool Chtxt = true)
        {
            LastFooterColor = (Resources["BackgroundAndBorderBrush"] == null) ? "#007ACC" : Resources["BackgroundAndBorderBrush"].ToString();
            FieldInfo fi = st.GetType().GetField(st.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (Chtxt)
            {
                text = (text == "") ? st.ToString() : text;
                Resources["MessageText"] = text;
            }
            color = (color == "") ? attributes[0].Description.ToString() : color;
            Resources["BackgroundAndBorderColor"] = ChangeColorBrightness((Color)(new ColorConverter().ConvertFrom(color)), (float)0.333);
            Resources["BackgroundAndBorderBrush"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
        }

        public void ThemeChange(Enum st)
        {
            FieldInfo fi = st.GetType().GetField(st.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string val = attributes[0].Description.ToString();
            var rDictionary = new ResourceDictionary();
            rDictionary.Source = new Uri(string.Format("pack://application:,,,/CustomWindow;component/Themes/Styles/" + val + ".xaml"), UriKind.Absolute);
            this.Resources.MergedDictionaries.Add(rDictionary);
        }

        public void ThemeColorChange(Enum st)
        {
            FieldInfo fi = st.GetType().GetField(st.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            string color = attributes[0].Description.ToString();
            Resources["ApplicationAccentBrush"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
            Color col = ChangeColorBrightness((Color)(new ColorConverter().ConvertFrom(color)), (float)0.333);
            string lighter = col.ToString();
            Resources["ApplicationAccentBrushSecondary"] = (SolidColorBrush)(new BrushConverter().ConvertFrom(lighter));
        }

        protected void ActivateSate(object sender, System.EventArgs e)
        {
            StateIndicator(State.Ready, "", LastFooterColor, false);
        }

        protected void CloseClick(object sender, RoutedEventArgs e)
        {
            var th = (this.Opacity == 1) ? 0 : 1;
            var anim = new DoubleAnimation(th, (Duration)TimeSpan.FromMilliseconds(150));
            anim.Completed += (s, _) => this.Close();
            this.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        protected void DeactivSate(object sender, System.EventArgs e)
        {
            StateIndicator(State.Ready, "", "#2D2D30", false);
        }

        protected void FadeWindow(string sta)
        {
        }

        protected void MinimizeClick(object sender, RoutedEventArgs e)
        {
                WindowState = WindowState.Minimized;
        }

        protected void moveRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        protected void RestoreClick(object sender, RoutedEventArgs e)
        {
            ResizeGrip ResGrp = GetTemplateChild("ResGrp") as ResizeGrip;
            Border BorderShadow = GetTemplateChild("BorderShadow") as Border;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - (int)BorderShadow.Margin.Bottom + (int)BorderShadow.BorderThickness.Bottom;
            Button SpB = (Button)sender;
            SpB.Content = (WindowState == WindowState.Normal) ? 2 : 1;
            if (WindowState == WindowState.Normal)
            {
                ResGrp.Opacity = 0;
                WindowState = WindowState.Maximized;
            }
            else
            {
                ResGrp.Opacity = 1;
                WindowState = WindowState.Normal;
            };
        }

        protected void RestoreClickHeadr(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Border BorderShadow = GetTemplateChild("BorderShadow") as Border;
                ResizeGrip ResGrp = GetTemplateChild("ResGrp") as ResizeGrip;
                this.MaxHeight = SystemParameters.PrimaryScreenHeight + (int)BorderShadow.Margin.Bottom + (int)BorderShadow.Margin.Top + (int)BorderShadow.BorderThickness.Bottom + (int)BorderShadow.Margin.Top;
                if (WindowState == WindowState.Normal)
                {
                    ResGrp.Opacity = 0;
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    ResGrp.Opacity = 1;
                    WindowState = WindowState.Normal;
                };
            }
        }
    }
}