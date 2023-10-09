using System.Timers;
using System.Windows;
using System.Windows.Controls;
using WPFUtilsLib.Enums;

namespace WPFUtilsLib.UserControls.IOs
{
    /// <summary>
    /// Interakční logika pro ClientStatusDot.xaml
    /// </summary>
    public partial class OnlineStatusDot : UserControl
    {
        private static readonly Timer _timer = new();

        public Status Status
        {
            get { return (Status)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(
                nameof(Status), typeof(Status), typeof(OnlineStatusDot),
                new PropertyMetadata(Status.Offline, OnStatusChanged));

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OnlineStatusDot statusDot = (OnlineStatusDot)d;

            statusDot.UpdateStatusDot();
        }

        public OnlineStatusDot()
        {
            InitializeComponent();

            _timer.Interval = 1000;
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(UpdateStatusDot);
        }

        private void UpdateStatusDot()
        {
            if (Status == Status.Offline)
            {
                SetRed();
            }
            else if (Status == Status.Waiting)
            {
                SetWhite();
            }
            else
            {
                ToggleGreen();
            }
        }

        private void ToggleGreen()
        {
            if (PART_greenDot.Visibility == Visibility.Visible)
            {
                SetWhite();
            }
            else
            {
                SetGreen();
            }
        }

        private void SetRed()
        {
            PART_redDot.Visibility = Visibility.Visible;
            PART_whiteDot.Visibility = Visibility.Hidden;
            PART_greenDot.Visibility = Visibility.Hidden;
        }

        private void SetWhite()
        {
            PART_redDot.Visibility = Visibility.Hidden;
            PART_whiteDot.Visibility = Visibility.Visible;
            PART_greenDot.Visibility = Visibility.Hidden;
        }

        private void SetGreen()
        {
            PART_redDot.Visibility = Visibility.Hidden;
            PART_whiteDot.Visibility = Visibility.Hidden;
            PART_greenDot.Visibility = Visibility.Visible;
        }
    }
}
