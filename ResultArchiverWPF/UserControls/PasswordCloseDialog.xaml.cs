using ResultArchiverWPF.Classes;
using System.Windows;
using System.Windows.Input;

namespace ResultArchiverWPF.UserControls
{
    /// <summary>
    /// Interakční logika pro PasswordCloseDialog.xaml
    /// </summary>
    public partial class PasswordCloseDialog : Window
    {
        public PasswordCloseDialog()
        {
            InitializeComponent();

            TopBar.Window = this;
            TopBar.ClosingAction = Enums.ClosingAction.HideWindow;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            CheckPasswordCloseApp();
        }

        public void ShowPopup(string Title)
        {
            TopBar.TitleText_Text = Title;
            Show();
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckPasswordCloseApp();
            }
        }

        private void CheckPasswordCloseApp()
        {
            lblError.Content = "";

            if (Constants.PASSWORDS is null)
            {
                lblError.Content = "Error. Passwords is not set in constant class.";
                Application.Current.Shutdown();
                return;
            }

            foreach (string password in Constants.PASSWORDS)
            {
                if (password == passwordBox.Password)
                {
                    Application.Current.Shutdown();
                    return;
                }
            }

            lblError.Content = "Error. Passwords is not correct.";
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible) 
            {
                lblError.Content = "";
                passwordBox.Password = "";
                passwordBox.Focus();
            }
        }
    }
}
