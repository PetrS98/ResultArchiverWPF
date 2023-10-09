using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interakční logika pro NumberBox.xaml
    /// </summary>
    public partial class NumberBox : UserControl
    {
        public event Action? ValueChanged;

        #region ValueIncrementProperty
        public int ValueIncrement
        {
            get { return (int)GetValue(ValueIncrementProperty); }
            set { SetValue(ValueIncrementProperty, value); }
        }

        public static readonly DependencyProperty ValueIncrementProperty =
            DependencyProperty.Register(
                nameof(ValueIncrement), typeof(int), typeof(NumberBox), 
                new PropertyMetadata(1));
        #endregion

        #region ValueDecrementProperty
        public int ValueDecrement
        {
            get { return (int)GetValue(ValueDecrementProperty); }
            set { SetValue(ValueDecrementProperty, value); }
        }

        public static readonly DependencyProperty ValueDecrementProperty =
            DependencyProperty.Register(
                nameof(ValueDecrement), typeof(int), typeof(NumberBox),
                new PropertyMetadata(1));
        #endregion

        #region ButtonsVisibleDependencyProperty
        public bool ButtonsVisible
        {
            get { return (bool)GetValue(ButtonsVisibleProperty); }
            set { SetValue(ButtonsVisibleProperty, value); }
        }

        public static readonly DependencyProperty ButtonsVisibleProperty =
            DependencyProperty.Register(
                nameof(ButtonsVisible), typeof(bool), typeof(NumberBox), 
                new PropertyMetadata(true, OnButtonsVisibleChanged));

        private static void OnButtonsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox nb = (NumberBox)d;
            bool visible = (bool)e.NewValue;

            nb.PART_buttons.Visibility = visible ?
                Visibility.Visible :
                Visibility.Collapsed;
        }
        #endregion

        #region MinValueDependencyProperty
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                nameof(MinValue), typeof(int), typeof(NumberBox), 
                new PropertyMetadata(int.MinValue, OnMinValueChanged));

        private static void OnMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox nb = (NumberBox)d;

            if (nb.MinValue > nb.Value)
                nb.Value = nb.MinValue;

            nb.UpdateButtonsEnabled();
        }
        #endregion

        #region MaxValueDependencyProperty
        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                nameof(MaxValue), typeof(int), typeof(NumberBox), 
                new PropertyMetadata(int.MaxValue, OnMaxValueChanged));

        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox nb = (NumberBox)d;

            if (nb.MaxValue < nb.Value)
                nb.Value = nb.MaxValue;

            nb.UpdateButtonsEnabled();
        }
        #endregion

        #region ValueDependencyProperty
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumberBox),
                new PropertyMetadata(0, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox nb = (NumberBox)d;
            int value = (int)e.NewValue;

            if (value < nb.MinValue)
            {
                nb.Value = nb.MinValue;
                return;
            }
            if (value > nb.MaxValue)
            {
                nb.Value = nb.MaxValue;
                return;
            }

            nb.PART_textBox.Text = e.NewValue.ToString();
            nb.UpdateButtonsEnabled();
            nb.ValueChanged?.Invoke();
        }
        #endregion

        public NumberBox()
        {
            InitializeComponent();

            MouseWheel += Border_MouseWheel;
            PART_textBox.TextChanged += TextBox_OnTextChanged;
            PART_textBox.LostFocus += TextBox_OnLostFocus;
            PART_textBox.PreviewKeyDown += OnTextBoxKeyDown;
            PART_incrementButton.Click += (s, e) => IncrementValue(1);
            PART_decrementButton.Click += (s, e) => DecrementValue(1);

            UpdateButtonsEnabled();
        }

        private void UpdateTextBox()
        {
            bool parsed = int.TryParse(PART_textBox.Text, out int result);
            if (parsed) Value = result;
            PART_textBox.Text = Value.ToString();
        }

        private void UpdateButtonsEnabled()
        {
            PART_incrementButton.IsEnabled = Value < MaxValue;
            PART_decrementButton.IsEnabled = Value > MinValue;
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateTextBox();
            }
            else if (e.Key == Key.Escape)
            {
                PART_textBox.Text = Value.ToString();
            }
            else if (e.Key == Key.Up)
            {
                UpdateTextBox();
                IncrementValue(1);
            }
            else if (e.Key == Key.PageUp)
            {
                UpdateTextBox();
                IncrementValue(ValueIncrement);
            }
            else if (e.Key == Key.Down)
            {
                UpdateTextBox();
                DecrementValue(1);
            }
            if (e.Key == Key.PageDown)
            {
                UpdateTextBox();
                DecrementValue(ValueDecrement);
            }
        }

        private void IncrementValue(int amount)
        {
            if (Value < MaxValue) Value += amount;
        }

        private void DecrementValue(int amount)
        {
            if (Value > MinValue) Value -= amount;
        }

        private void Border_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!PART_textBox.IsFocused) return;

            UpdateTextBox();
            if (e.Delta > 0) IncrementValue(ValueIncrement);
            else if (e.Delta < 0) DecrementValue(ValueDecrement);
            e.Handled = true;
        }

        private void TextBox_OnTextChanged(object sender, RoutedEventArgs e)
        {
            bool parsed = int.TryParse(PART_textBox.Text, out int result);
            if (parsed) Value = result;
        }

        private void TextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            UpdateTextBox();
        }
    }
}
