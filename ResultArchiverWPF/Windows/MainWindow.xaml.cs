using System.Windows;

namespace ResultArchiverWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            topBar.Window = this;
        }
    }
}
