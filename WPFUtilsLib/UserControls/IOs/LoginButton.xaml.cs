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
using System.Xml.Linq;
using WPFUtilsLib.Services.Enums;
using WPFUtilsLib.UserControls.PopUpDialogs;

namespace WPFUtilsLib.UserControls.IOs
{
    /// <summary>
    /// Interakční logika pro LoginButton.xaml
    /// </summary>
    public partial class LoginButton : UserControl
    {
        private LoginDialog loginDialog = new LoginDialog();

        private static readonly ImageSource LoginImage = new BitmapImage(new Uri("pack://application:,,,/WPFUtilsLib;component/Resources/login.png"));
        private static readonly ImageSource LogoutImage = new BitmapImage(new Uri("pack://application:,,,/WPFUtilsLib;component/Resources/logout.png"));

        public event EventHandler<string> Error;
        public event EventHandler<LoginStates> LoginChanged;

        private LoginStates _loginStatus = LoginStates.NoLogged;
        private LoginStates _LoginStatus
        {
            get => _loginStatus;
            set
            {
                if (_loginStatus != value)
                {
                    _loginStatus = value;
                    LoginChanged?.Invoke(this, value);
                }
            }
        }

        private LanguageEnum _reqLanguage = LanguageEnum.ENG;

        public LanguageEnum ReqLanguage
        {
            get { return _reqLanguage; }
            set
            {
                _reqLanguage = value;
                SetTextsLanguage(value);
                loginDialog.ReqLanguage = value;
            }
        }

        public string[] LoginData_UserNames { get; set; }
        public string[] LoginData_Passwords { get; set; }

        private string[] ErrorTexts = new string[2];

        public LoginButton()
        {
            InitializeComponent();
            loginDialog.Login_Click += LoginCmd;

            LoginChanged += ChangeImage;
        }

        private void ChangeImage(object sender, LoginStates e)
        {
            if (e == LoginStates.Logged)
            {
                image.Source = LogoutImage;
            }
            else if (e == LoginStates.NoLogged)
            {
                image.Source = LoginImage;
            }
        }

        private void LoginCmd(object sender, bool e)
        {
            if (CheckIfLoginIsCorrect())
            {
                _LoginStatus = LoginStates.Logged;
                loginDialog.Hide();
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenLoginDialogOrLogOut();
        }

        public void OpenLoginDialogOrLogOut()
        {
            if (_LoginStatus == LoginStates.Logged)
            {
                image.Source = LogoutImage;
                _LoginStatus = LoginStates.NoLogged;
            }
            else if (_LoginStatus == LoginStates.NoLogged)
            {
                loginDialog.ShowPopUp();
            }
        }

        private bool CheckIfLoginIsCorrect()
        {
            if( LoginData_UserNames is null || LoginData_Passwords is null)
            {
                Error?.Invoke(this, ErrorTexts[0]);
                return false;
            }

            int UserDataLen = LoginData_UserNames.Length;
            int PasswordDataLen = LoginData_Passwords.Length;
            int DataLen;

            if (UserDataLen == PasswordDataLen || UserDataLen >= PasswordDataLen) DataLen = UserDataLen;
            else DataLen = PasswordDataLen;

            for (int i = 0; i < DataLen; i++)
            {
                if (LoginData_UserNames[i] == loginDialog.UserName && LoginData_Passwords[i] == loginDialog.Password) return true;
            }

            Error?.Invoke(this, ErrorTexts[1]);
            return false;
        }

        private void SetTextsLanguage(LanguageEnum Language)
        {
            if (Language == LanguageEnum.ENG)
            {
                ErrorTexts[0] = "User name or password data is not setting. Login not possible.";
                ErrorTexts[1] = "User name or password is not correct.";
            }
            else if (Language == LanguageEnum.CZ)
            {
                ErrorTexts[0] = "Uživatelsé jméno nebo heslo není nastaveno v konfiguraci. Přihlášení není možné.";
                ErrorTexts[1] = "Nesprávné uživatelské jméno nebo heslo.";
            }
        }
    }
}
