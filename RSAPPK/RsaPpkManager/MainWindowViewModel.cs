using Microsoft.Win32;
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
        private ICommand exportBrowseCommand;
        private string exportFileName;
        private ICommand exportPpkCommand;
        private string exportPpkName;
        private bool hasConnectionBeenTestedSuccessfully;
        private ICommand importBrowseCommand;
        private string importFileName;
        private ICommand importPpkCommand;
        private string importPpkName;
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

        public ICommand ExportBrowseCommand =>
            exportBrowseCommand ?? (exportBrowseCommand = new RelayCommand(ExportBrowse));

        public string ExportFileName
        {
            get { return exportFileName; }
            set
            {
                exportFileName = value;
                OnPropertyChanged();
            }
        }

        public ICommand ExportPpkCommand =>
            exportPpkCommand ?? (exportPpkCommand = new RelayCommand(ExportPpk, CanExportPpk));

        public string ExportPpkName
        {
            get { return exportPpkName; }
            set
            {
                exportPpkName = value;
                OnPropertyChanged();
            }
        }

        public bool HasConnectionBeenTestedSuccessfully
        {
            get { return hasConnectionBeenTestedSuccessfully; }
            set
            {
                hasConnectionBeenTestedSuccessfully = value;
                OnPropertyChanged();
            }
        }

        public ICommand ImportBrowseCommand =>
            importBrowseCommand ?? (importBrowseCommand = new RelayCommand(ImportBrowse));

        public string ImportFileName
        {
            get { return importFileName; }
            set
            {
                importFileName = value;
                OnPropertyChanged();
            }
        }

        public ICommand ImportPpkCommand =>
            importPpkCommand ?? (importPpkCommand = new RelayCommand(ImportPpk, CanImportPpk));

        public string ImportPpkName
        {
            get { return importPpkName; }
            set
            {
                importPpkName = value;
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

        private bool CanExportPpk()
        {
            return !string.IsNullOrWhiteSpace(exportFileName) && !string.IsNullOrWhiteSpace(exportPpkName);
        }

        private bool CanImportPpk()
        {
            return !string.IsNullOrWhiteSpace(importFileName) && !string.IsNullOrWhiteSpace(importPpkName);
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

        private void ExportPpk()
        {
            string result = RsaPpkManagementService.ExportRsaPpkContainer(ExportPpkName, ExportFileName);
        }

        private void ExportBrowse()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                AddExtension = true,
                CheckFileExists = false,
                Filter = "XML file (*.xml)|*.xml",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                OverwritePrompt = true,
                Title = "Export File"
            };

            bool? result = sfd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                ExportFileName = sfd.FileName;
                ExportPpkName = Path.GetFileNameWithoutExtension(sfd.SafeFileName);
            }
        }

        private void ImportPpk()
        {
            string result = RsaPpkManagementService.ImportRsaPpkContainer(ImportPpkName, ImportFileName);
        }

        private void ImportBrowse()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = false,
                Filter = "XML file (*.xml)|*.xml",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                Multiselect = false,
                Title = "Export File"                
            };

            bool? result = ofd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                ImportFileName = ofd.FileName;
                ImportPpkName = Path.GetFileNameWithoutExtension(ofd.SafeFileName);
            }
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
