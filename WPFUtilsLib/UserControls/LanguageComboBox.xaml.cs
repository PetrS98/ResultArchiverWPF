using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFUtilsLib.UserControls
{
    /// <summary>
    /// Interakční logika pro LanguageComboBox.xaml
    /// </summary>
    public partial class LanguageComboBox : UserControl
    {
        private static readonly string _defaultLanguage = "en-GB";
        private static readonly Dictionary<string, ComboBoxItem> _flags = new();
        private static readonly Dictionary<ComboBoxItem, string> _items = new();

        public string? SelectedLanguage
        {
            get { return (string?)GetValue(SelectedLanguageProperty); }
            set { SetValue(SelectedLanguageProperty, value); }
        }

        public static readonly DependencyProperty SelectedLanguageProperty =
            DependencyProperty.Register(nameof(SelectedLanguage), typeof(string), typeof(LanguageComboBox), new PropertyMetadata(null, OnLanguageChanged));

        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (LanguageComboBox)d;

            control.UpdateComboBox();
        }

        public LanguageComboBox()
        {
            InitializeComponent();

            _flags.Add("en-GB", PART_item_enGB);
            _flags.Add("cs-CZ", PART_item_csCZ);

            foreach (var flagKeyValue in _flags)
            {
                _items.Add(flagKeyValue.Value, flagKeyValue.Key);
            }

            SelectedLanguage = _defaultLanguage;
        }

        private void UpdateComboBox()
        {
            if (string.IsNullOrWhiteSpace(SelectedLanguage) ||
                !_flags.ContainsKey(SelectedLanguage))
            {
                SelectedLanguage = _defaultLanguage;
                return;
            }

            PART_comboBox.SelectedItem = _flags[SelectedLanguage];
        }

        private void PART_comboBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            SelectedLanguage = _items[(ComboBoxItem)PART_comboBox.SelectedItem];
        }
    }
}
