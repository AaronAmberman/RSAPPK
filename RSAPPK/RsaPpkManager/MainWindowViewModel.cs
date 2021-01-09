using RSAPPK;
using RSAPPK.Database;
using SNORM.ORM;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RsaPpkManager
{
    /// <summary>View model for the main window.</summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private ICommand connectDatabaseCommand;
        //private string connectionString;
        private string connectionString = "Server=DESKTOP-AJJCI25\\SQLEXPRESS;Initial Catalog=RsaPpk;Integrated Security=true;";
        private ICommand createPpkCommand;
        private string creationPpkName;
        private SqlDatabase database;
        private Visibility databaseOverlayVisibility = Visibility.Visible;
        private ICommand deletePpkCommand;
        private string deletionPpkName;
        private bool hasConnectionBeenTestedSuccessfully;
        private Visibility overlayVisibility = Visibility.Collapsed;
        private ICommand showDatabaseScriptCommand;
        private string overlayMessage;
        private SolidColorBrush statusBackground = Brushes.Orange;
        private string statusMessage = "Database not connected (this is optional)";

        #endregion

        #region Properties

        public ICommand ConnectDatabaseCommand =>
            connectDatabaseCommand ?? (connectDatabaseCommand = new RelayCommand(ConnectDatabase, CanConnectDatabase));

        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                connectionString = value;

                if (database?.ConnectionState == System.Data.ConnectionState.Open)
                    database.Disconnect();

                try
                {
                    database = new SqlDatabase(value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occurred attempting to connect to database.{Environment.NewLine}{ex}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                OnPropertyChanged();
            }
        }

        public ICommand CreatePpkCommand =>
            createPpkCommand ?? (createPpkCommand = new RelayCommand(CreatePpk, CanCreatePpk));

        public string CreationPpkName
        {
            get { return creationPpkName; }
            set
            {
                creationPpkName = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeletePpkCommand =>
            deletePpkCommand ?? (deletePpkCommand = new RelayCommand(DeletePpk, CanDeletePpk));

        public string DeletionPpkName
        {
            get { return deletionPpkName; }
            set
            {
                deletionPpkName = value;
                OnPropertyChanged();
            }
        }

        public Visibility DatabaseOverlayVisibility
        {
            get { return databaseOverlayVisibility; }
            set
            {
                databaseOverlayVisibility = value;
                OnPropertyChanged();
            }
        }

        public Dispatcher Dispatcher { get; set; }

        public bool HasConnectionBeenTestedSuccessfully
        {
            get { return hasConnectionBeenTestedSuccessfully; }
            set
            {
                hasConnectionBeenTestedSuccessfully = value;
                OnPropertyChanged();
            }
        }

        public string OverlayMessage
        {
            get { return overlayMessage; }
            set
            {
                overlayMessage = value;
                OnPropertyChanged();
            }
        }

        public Visibility OverlayVisibility
        {
            get { return overlayVisibility; }
            set
            {
                overlayVisibility = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowDatabaseScriptCommand =>
            showDatabaseScriptCommand ?? (showDatabaseScriptCommand = new RelayCommand(ShowDatabaseScript));

        public SolidColorBrush StatusBackground 
        {
            get { return statusBackground; }
            set 
            { 
                statusBackground = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get { return statusMessage; }
            set
            {
                statusMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        #endregion

        #region Methods

        private bool CanCreatePpk()
        {
            return !string.IsNullOrWhiteSpace(creationPpkName);
        }

        private bool CanConnectDatabase()
        {
            return !string.IsNullOrWhiteSpace(connectionString);
        }

        private bool CanDeletePpk()
        {
            return !string.IsNullOrWhiteSpace(deletionPpkName);
        }

        private void ConnectDatabase()
        {
            if (database == null)
            {
                MessageBox.Show("Please enter a connection string. Any valid MS SQL connection string will work.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Task.Run(() =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    OverlayVisibility = Visibility.Visible;
                    OverlayMessage = "Attempting to connect to database...";
                }), DispatcherPriority.Normal);

                if (database.Connect())
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        HasConnectionBeenTestedSuccessfully = true;

                        OverlayVisibility = Visibility.Collapsed;
                        OverlayMessage = "";

                        StatusBackground = Brushes.Green;
                        StatusMessage = "Connected to database";

                        DatabaseOverlayVisibility = Visibility.Collapsed;
                    }), DispatcherPriority.Normal);

                    database.Disconnect();
                }
                else
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        HasConnectionBeenTestedSuccessfully = false;

                        OverlayVisibility = Visibility.Collapsed;
                        OverlayMessage = "";

                        StatusBackground = Brushes.Red;
                        StatusMessage = "Failed to connect to database";

                        DatabaseOverlayVisibility = Visibility.Visible;
                    }), DispatcherPriority.Normal);
                }
            });
        }

        private void CreatePpk()
        {
            string result = RsaPpkManagementService.CreateRsaPpkContainer(CreationPpkName);
        }

        private void DeletePpk()
        {
            string result = RsaPpkManagementService.DeleteRsaPpkContainer(DeletionPpkName);
        }

        private void ShowDatabaseScript()
        {
            string databaseCreationString = DatabaseCreator.GetCreateDatabaseScript();

            TextWindow textWindow = new TextWindow();
            textWindow.Text = databaseCreationString;
            textWindow.ShowDialog();
        }

        #endregion
    }
}
