using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFUtilsLib.Services.Enums;

namespace WPFUtilsLib.UserControls.IOs 
{
    /// <summary>
    /// Interakční logika pro LanguageSwitcher.xaml
    /// </summary>
    public partial class LanguageSwitcher : UserControl
    {
        private static readonly ImageSource CZImage = new BitmapImage(new Uri("pack://application:,,,/WPFUtilsLib;component/Resources/CZFlag.jpg"));
        private static readonly ImageSource GBImage = new BitmapImage(new Uri("pack://application:,,,/WPFUtilsLib;component/Resources/GBFlag.jpg"));

        public event EventHandler<LanguageEnum>? LanguageChanged;

        private LanguageEnum _language;
        private LanguageEnum _Language
        {
            get => _language;
            set
            {
                if (_language != value)
                {
                    _language = value;
                    LanguageChanged?.Invoke(this, value);
                }
            }
        }

        public LanguageSwitcher()
        {
            InitializeComponent();
            _Language = LanguageEnum.ENG;
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(_Language == LanguageEnum.ENG)
            {
                image.Source = GBImage;
                _Language = LanguageEnum.CZ;
            }
            else if(_Language == LanguageEnum.CZ)
            {
                image.Source= CZImage;
                _Language = LanguageEnum.ENG;
            }
        }
    }
}
