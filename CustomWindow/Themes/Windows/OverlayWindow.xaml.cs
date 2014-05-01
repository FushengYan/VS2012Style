using System;
using System.Windows;
using System.Windows.Forms;

namespace Overlay
{
    /// <summary>
    /// Displays a WindowsFormsHost control over a given placement target element in a WPF Window.
    /// The owner window can be transparent, but not this one, due mixing DirectX and GDI drawing.
    /// </summary>
    public partial class OverlayWindow : Window
    {
        private FrameworkElement t;

        public OverlayWindow(FrameworkElement target, Control child)
        {
            InitializeComponent();

            t = target;
            wfh.Child = child;

            Owner = Window.GetWindow(t);

            Owner.LocationChanged += new EventHandler(PositionAndResize);
            t.LayoutUpdated += new EventHandler(PositionAndResize);
            PositionAndResize(null, null);

            if (Owner.IsVisible)
                Show();
            else
                Owner.IsVisibleChanged += delegate
                {
                    if (Owner.IsVisible)
                        Show();
                };
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Owner.LocationChanged -= new EventHandler(PositionAndResize);
            t.LayoutUpdated -= new EventHandler(PositionAndResize);
        }

        private void PositionAndResize(object sender, EventArgs e)
        {
            try
            {
                Point p = t.PointToScreen(new Point());
                Left = p.X;
                Top = p.Y;
                Height = t.ActualHeight;
                Width = t.ActualWidth;
            }
            catch
            {
            }
        }
    };
}