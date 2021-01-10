using System.Windows;

namespace RsaPpkManager
{
    /// <summary>MainWindow to the application.</summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel
            {
                Dispatcher = Dispatcher,
                RsaKeysViewSql = sqlTabListView,
                RsaKeysViewImport = importTabListView
            };
        }

        #endregion
    }
}
