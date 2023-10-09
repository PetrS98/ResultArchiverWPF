using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFUtilsLib.Services.Enums;

namespace WPFUtilsLib.UserControls.PopUpDialogs
{
    /// <summary>
    /// Interakční logika pro LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public EventHandler<bool> Login_Click;
        public EventHandler<bool> Cancel_Click;

        public string UserName { get; private set; } = "";
        public string Password { get; private set; } = "";

        private LanguageEnum _reqLanguage = LanguageEnum.ENG;

        public LanguageEnum ReqLanguage
        {
            get { return _reqLanguage; }
            set 
            { 
                _reqLanguage = value;
                SetTextsLanguage(value);
            }
        }

        public LoginDialog()
        {
            InitializeComponent();
            SetTextsLanguage(LanguageEnum.ENG);
            TopBar.Window = this;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            UserName = tbUserName.Text;
            Password = tbPassword.Password;
            Login_Click?.Invoke(this, true);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel_Click?.Invoke(this, true);
            DeleteInputs();
        }

        private void SetTextsLanguage(LanguageEnum Language)
        {
            if(Language == LanguageEnum.ENG)
            {
                TopBar.TitleText_Text =     "Login Dialog";
                tblUserName.Text =          "User Name:";
                tblPassword.Text =          "Password:";
                btnLogin.Content =          "Login";
                btnCancel.Content =         "Cancel";
            }
            else if(Language == LanguageEnum.CZ)
            {
                TopBar.TitleText_Text =     "Přihlašovací Okno";
                tblUserName.Text =          "Uživatelské Jméno:";
                tblPassword.Text =          "Heslo:";
                btnLogin.Content =          "Přihlásit";
                btnCancel.Content =         "Zrušit";
            }
        }

        private void DeleteInputs()
        {
            tbUserName.Text = "";
            tbPassword.Password = "";
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Visibility == Visibility.Hidden)
            {
                DeleteInputs();
            }
        }

        public void ShowPopUp()
        {
            if (Visibility == Visibility.Hidden) Visibility = Visibility.Visible;
            else Show();
        }
    }
}
