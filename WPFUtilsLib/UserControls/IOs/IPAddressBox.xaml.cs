using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFUtilsLib.UserControls.IOs
{
    /// <summary>
    /// Interakční logika pro IPAddressBox.xaml
    /// </summary>
    public partial class IPAddressBox : UserControl
    {
        private static readonly Brush ForegroundValid = new SolidColorBrush(Colors.White);
        private static readonly Brush ForegroundInvalid = new SolidColorBrush(Color.FromRgb(192, 0, 0));
        private static readonly Regex IPAddressPortRegex = new(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}(:([0-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9]|[1-5][0-9][0-9][0-9][0-9]|6[0-4][0-9][0-9][0-9]|65[0-4][0-9][0-9]|655[0-2][0-9]|6553[0-5]))?$");
        private static readonly Regex IPAddressRegex = new(@"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$");

        #region PortAllowedDependencyProperty
        public bool PortAllowed
        {
            get { return (bool)GetValue(PortAllowedProperty); }
            set { SetValue(PortAllowedProperty, value); }
        }

        public static readonly DependencyProperty PortAllowedProperty =
            DependencyProperty.Register(
                nameof(PortAllowed), typeof(bool), typeof(IPAddressBox),
                new PropertyMetadata(false, OnPortAllowedChanged));

        private static void OnPortAllowedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IPAddressBox ipAddressBox = (IPAddressBox)d;
            ipAddressBox.UpdateValidity();
        }
        #endregion

        #region IPAddressValidDependencyProperty
        public bool IPAddressValid
        {
            get { return (bool)GetValue(IPAddressValidProperty); }
            set { SetValue(IPAddressValidProperty, value); }
        }

        private static readonly DependencyProperty IPAddressValidProperty =
            DependencyProperty.Register(nameof(IPAddressValid), 
                typeof(bool), typeof(IPAddressBox), 
                new PropertyMetadata(true, OnIPAddressValidChanged));

        private static void OnIPAddressValidChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IPAddressBox ipAddressBox = (IPAddressBox)d;
            bool newValue = (bool)e.NewValue;

            if (newValue)
            {
                ipAddressBox.PART_textBox.Foreground = ForegroundValid;
            }
            else
            {
                ipAddressBox.PART_textBox.Foreground = ForegroundInvalid;
            }
        }
        #endregion

        #region IPAddressDependencyProperty
        public string? IPAddress
        {
            get { return (string?)GetValue(IPAddressProperty); }
            set { SetValue(IPAddressProperty, value); }
        }

        public static readonly DependencyProperty IPAddressProperty =
            DependencyProperty.Register(
                nameof(IPAddress), typeof(string), typeof(IPAddressBox), 
                new PropertyMetadata("127.0.0.1", OnIPAddressChanged));

        private static void OnIPAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IPAddressBox ipAddressBox = (IPAddressBox)d;
            string? newValue = (string?)e.NewValue;

            ipAddressBox.PART_textBox.Text = newValue;
            ipAddressBox.UpdateValidity();
        }
        #endregion

        public IPAddressBox()
        {
            InitializeComponent();

            PART_textBox.TextChanged += OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            IPAddress = PART_textBox.Text;
        }

        private void UpdateValidity()
        {
            if (PortAllowed)
            {
                UpdateIPAddressPortValidity();
            }
            else
            {
                UpdateIPAddressOnlyValidity();
            }
        }

        private void UpdateIPAddressOnlyValidity()
        {
            IPAddressValid = IPAddress is null ? false : IPAddressRegex.IsMatch(IPAddress);
        }

        private void UpdateIPAddressPortValidity()
        {
            IPAddressValid = IPAddress is null ? false : IPAddressPortRegex.IsMatch(IPAddress);
        }
    }
}
