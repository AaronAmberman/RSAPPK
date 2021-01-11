using Microsoft.Win32;
using RSAPPK;
using RSAPPK.Cryptography;
using RSAPPK.Database;
using SNORM.ORM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private string connectionString;
        private ICommand createPpkCommand;
        private string creationPpkName;
        private SqlDatabase database;
        private string databaseLog;
        private Visibility databaseLoggingVisibility = Visibility.Collapsed;
        private Visibility databaseOverlayVisibility = Visibility.Visible;
        private string dataDecryptString;
        private string dataDecryptStringOutput;
        private ICommand dataDecryptCommand;
        private string dataEncryptString;
        private string dataEncryptStringOutput;
        private ICommand dataEncryptCommand;
        private string dataRsaPpkName;
        private ICommand deletePpkCommand;
        private string deletionPpkName;
        private ICommand exportBrowseCommand;
        private string exportFileName;
        private ICommand exportPpkCommand;
        private string exportPpkName;
        private bool hasConnectionBeenTestedSuccessfully;
        private ICommand importBrowseCommand;
        private ICommand importDatabasePpkCommand;
        private string importFileName;
        private ICommand importPpkCommand;
        private string importPpkName;
        private Visibility overlayVisibility = Visibility.Collapsed;
        private ICommand refreshKeysCommand;
        private ICommand removeKeysCommand;
        private ICommand showDatabaseScriptCommand;
        private string overlayMessage;
        private ExtendedObservableCollection<RsaPpks> rsaPpks = new ExtendedObservableCollection<RsaPpks>();
        private SolidColorBrush statusBackground = Brushes.Orange;
        private string statusMessage = "Database not connected (this is optional)";

        #endregion

        #region Properties

        public ICommand ConnectDatabaseCommand =>
            connectDatabaseCommand ?? (connectDatabaseCommand = new RelayCommand(ConnectDatabase));

        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                connectionString = value;

                if (database?.ConnectionState == System.Data.ConnectionState.Open)
                    database.Disconnect();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    try
                    {
                        database = new SqlDatabase(value)
                        {
                            Log = DatabaseLogging
                        };
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error occurred attempting to initialize database.{Environment.NewLine}{ex}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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

        public string DatabaseLog
        {
            get { return databaseLog; }
            set
            {
                databaseLog = value;
                OnPropertyChanged();
            }
        }

        public Visibility DatabaseLoggingVisibility
        {
            get { return databaseLoggingVisibility; }
            set
            {
                databaseLoggingVisibility = value;
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

        public string DataDecryptString
        {
            get { return dataDecryptString; }
            set
            {
                dataDecryptString = value;
                OnPropertyChanged();
            }
        }

        public string DataDecryptStringOutput
        {
            get { return dataDecryptStringOutput; }
            set
            {
                dataDecryptStringOutput = value;
                OnPropertyChanged();
            }
        }

        public ICommand DataDecryptCommand =>
            dataDecryptCommand ?? (dataDecryptCommand = new RelayCommand(DecryptData, CanDecryptData));

        public string DataEncryptString
        {
            get { return dataEncryptString; }
            set
            {
                dataEncryptString = value;
                OnPropertyChanged();
            }
        }

        public string DataEncryptStringOutput
        {
            get { return dataEncryptStringOutput; }
            set
            {
                dataEncryptStringOutput = value;
                OnPropertyChanged();
            }
        }

        public ICommand DataEncryptCommand =>
            dataEncryptCommand ?? (dataEncryptCommand = new RelayCommand(EncryptData, CanEncryptData));

        public string DataRsaPpkName
        {
            get { return dataRsaPpkName; }
            set
            {
                dataRsaPpkName = value;
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

        public ICommand ImportDatabasePpkCommand =>
            importDatabasePpkCommand ?? (importDatabasePpkCommand = new RelayCommand(ImportDatabasePpk, CanImportDatabasePpk));

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

        public ListView RsaKeysViewSql { get; set; }
        public ListView RsaKeysViewImport { get; set; }

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

        public ExtendedObservableCollection<RsaPpks> RsaPpks 
        {
            get { return rsaPpks; }
            set 
            { 
                rsaPpks = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshKeysCommand =>
            refreshKeysCommand ?? (refreshKeysCommand = new RelayCommand(RefreshKeys));

        public ICommand RemoveKeysCommand =>
            removeKeysCommand ?? (removeKeysCommand = new RelayCommand(RemoveKeys, CanRemoveKeys));

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

        private bool CanDecryptData()
        {
            return !string.IsNullOrWhiteSpace(dataDecryptString) && !string.IsNullOrWhiteSpace(dataRsaPpkName);
        }

        private bool CanDeletePpk()
        {
            return !string.IsNullOrWhiteSpace(deletionPpkName);
        }

        private bool CanEncryptData()
        {
            return !string.IsNullOrWhiteSpace(dataEncryptString) && !string.IsNullOrWhiteSpace(dataRsaPpkName);
        }

        private bool CanExportPpk()
        {
            return !string.IsNullOrWhiteSpace(exportFileName) && !string.IsNullOrWhiteSpace(exportPpkName);
        }

        private bool CanImportDatabasePpk()
        {
            return RsaKeysViewImport?.SelectedItems.Count > 0;
        }

        private bool CanImportPpk()
        {
            return !string.IsNullOrWhiteSpace(importFileName) && !string.IsNullOrWhiteSpace(importPpkName);
        }

        private bool CanRemoveKeys()
        {
            return RsaKeysViewSql?.SelectedItems.Count > 0;
        }

        private void ConnectDatabase()
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                MessageBox.Show("Please enter a valid MS SQL Server connection, any valid string will work.", "Need Connection String", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

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
                    DatabaseLoggingVisibility = Visibility.Visible;
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

                        DatabaseLog += "Connected to database" + Environment.NewLine;

                        DatabaseOverlayVisibility = Visibility.Collapsed;
                    }), DispatcherPriority.Normal);

                    List<RsaPpks> allKeys = database.Select<RsaPpks>();

                    Dispatcher.BeginInvoke((Action)(() => 
                    {
                        RsaPpks.AddRange(allKeys);
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

                        DatabaseLog += "Failed to connect to database" + Environment.NewLine;

                        DatabaseOverlayVisibility = Visibility.Visible;
                    }), DispatcherPriority.Normal);
                }
            });
        }

        private void CreatePpk()
        {
            string result = RsaPpkManagementService.CreateRsaPpkContainer(CreationPpkName);

            MessageBox.Show(result, "Result of Create Operation");

            // we don't want to do anything with the database if the creation failed
            if (result.EndsWith("Failed!", StringComparison.Ordinal)) return;

            if (hasConnectionBeenTestedSuccessfully)
            {
                MessageBoxResult userResult = MessageBox.Show("There is a connected database, would you like to save this PPK pair in the database?", "Database Storage", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (userResult == MessageBoxResult.Yes)
                {
                    RsaPpks match = RsaPpks.FirstOrDefault(key => key.Name.Equals(CreationPpkName, StringComparison.OrdinalIgnoreCase));

                    if (match != null)
                    {
                        userResult = MessageBox.Show("There is already a key by that name in the database, would you like to replace it? Please be warned that this change is permanent and cannot be undone. So if there is data encrypted by this key it will no longer be decryptable because you are replacing key. The data is lost forever.", 
                            "Database Name Clash", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (userResult == MessageBoxResult.No)
                        {
                            MessageBox.Show("Please note that you have not chosen to insert this key into the database and now the key on your system and the key in the database are different. This can cause seriously problems. We suggest fixing the issue by decrypting the data by using the key in the database and then re-encrypting the data with the key you just created, then inserting the key you just created into the database.", "No Replacement Warning");

                            return;
                        }
                    }

                    Task.Run(() =>
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            OverlayVisibility = Visibility.Visible;
                            OverlayMessage = "Inserting into database...";
                        }), DispatcherPriority.Normal);

                        string tempFile = Path.Combine(Path.GetTempPath(), $"{CreationPpkName}.xml");

                        RsaPpkManagementService.ExportRsaPpkContainer(CreationPpkName, tempFile);

                        string text = File.ReadAllText(tempFile);

                        File.Delete(tempFile);

                        if (database.Connect())
                        {
                            // if we have a match and they chose to replace it then update the existing, if not insert a new one
                            if (match == null)
                            {
                                RsaPpks rsaPpk = new RsaPpks
                                {
                                    Name = CreationPpkName,
                                    RsaPpkXml = text
                                };

                                int insertResult = database.Insert(new List<RsaPpks>() { rsaPpk });

                                if (insertResult > -1)
                                {
                                    Dispatcher.BeginInvoke((Action)(() =>
                                    {
                                        RsaPpks.Add(rsaPpk);
                                    }), DispatcherPriority.Normal);
                                }
                            }
                            else
                            {
                                match.RsaPpkXml = text;

                                database.Update(new List<RsaPpks>() { match });
                            }

                            database.Disconnect();
                        }

                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            OverlayVisibility = Visibility.Collapsed;
                            OverlayMessage = "";
                            DatabaseLog += "RSA PPK successfully inserted into the database";
                        }), DispatcherPriority.Normal);
                    });
                }
            }
        }

        private void DatabaseLogging(string message)
        {
            DatabaseLog += message += Environment.NewLine;
        }

        private void DecryptData()
        {
            try
            {
                TwoStageCryptographer tsc = new TwoStageCryptographer(DataRsaPpkName);

                DataDecryptStringOutput = tsc.Decrypt(DataDecryptString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Decrypt Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeletePpk()
        {
            string result = RsaPpkManagementService.DeleteRsaPpkContainer(DeletionPpkName);

            MessageBox.Show(result, "Result of Delete Operation");
        }

        private void EncryptData()
        {
            try
            {
                TwoStageCryptographer tsc = new TwoStageCryptographer(DataRsaPpkName);

                DataEncryptStringOutput = tsc.Encrypt(DataEncryptString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Encrypt Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void ExportPpk()
        {
            string result = RsaPpkManagementService.ExportRsaPpkContainer(ExportPpkName, ExportFileName);

            MessageBox.Show(result, "Result of Export Operation");
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

        private void ImportDatabasePpk()
        {
            Task.Run(() =>
            {
                List<RsaPpks> selectedItems = new List<RsaPpks>();

                Dispatcher.Invoke(() =>
                {
                    OverlayVisibility = Visibility.Visible;
                    OverlayMessage = "Inserting into database...";

                    selectedItems = RsaKeysViewImport?.SelectedItems.OfType<RsaPpks>().ToList();
                }, DispatcherPriority.Normal);

                foreach (RsaPpks rsaPpk in selectedItems)
                {
                    string tempFile = Path.Combine(Path.GetTempPath(), "temp.xml");

                    File.WriteAllText(tempFile, rsaPpk.RsaPpkXml);

                    string result = RsaPpkManagementService.ImportRsaPpkContainer(rsaPpk.Name, tempFile);

                    File.Delete(tempFile);

                    Dispatcher.BeginInvoke((Action)(() => 
                    {
                        // let the user know the import failed
                        if (result.EndsWith("Failed!", StringComparison.Ordinal))
                        {
                            MessageBox.Show($"The import failed for key: {rsaPpk.Name}. Result:{Environment.NewLine}{result}", "Import Failure");
                        }
                        else
                        {
                            MessageBox.Show($"The import was successful for key: {rsaPpk.Name}", "Import Success");
                        }
                    }), DispatcherPriority.Normal);
                }

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    OverlayVisibility = Visibility.Collapsed;
                    OverlayMessage = "";
                    DatabaseLog += "RSA PPKs successfully inserted into the database" + Environment.NewLine;
                }), DispatcherPriority.Normal);
            }).ContinueWith(task => 
            {
                if (task.IsFaulted || task.Exception != null)
                {
                    Dispatcher.BeginInvoke((Action)(() => 
                    {
                        MessageBox.Show("An error occurred attempting to import data from the database.");
                    }), DispatcherPriority.Normal);
                }
            });
        }

        private void ImportPpk()
        {
            string result = RsaPpkManagementService.ImportRsaPpkContainer(ImportPpkName, ImportFileName);

            MessageBox.Show(result, "Result of Import Operation");

            // we don't want to do anything with the database if the import failed
            if (result.EndsWith("Failed!", StringComparison.Ordinal)) return;

            if (hasConnectionBeenTestedSuccessfully)
            {
                MessageBoxResult userResult = MessageBox.Show("There is a connected database, would you like to save this PPK pair in the database?", "Database Storage", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (userResult == MessageBoxResult.Yes)
                {
                    RsaPpks match = RsaPpks.FirstOrDefault(key => key.Name.Equals(ImportPpkName, StringComparison.OrdinalIgnoreCase));

                    if (match != null)
                    {
                        userResult = MessageBox.Show("There is already a key by that name in the database, would you like to replace it? Please be warned that this change is permanent and cannot be undone. So if there is data encrypted by this key it will no longer be decryptable because you are replacing key. The data is lost forever.",
                            "Database Name Clash", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (userResult == MessageBoxResult.No)
                        {
                            MessageBox.Show("Please note that you have not chosen to insert this key into the database and now the key on your system and the key in the database are different. This can cause seriously problems. We suggest fixing the issue by decrypting the data by using the key in the database and then re-encrypting the data with the key you just created, then inserting the key you just created into the database.", "No Replacement Warning");

                            return;
                        }
                    }

                    Task.Run(() =>
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            OverlayVisibility = Visibility.Visible;
                            OverlayMessage = "Inserting into database...";
                        }), DispatcherPriority.Normal);

                        string tempFile = Path.Combine(Path.GetTempPath(), $"{ImportPpkName}.xml");

                        RsaPpkManagementService.ExportRsaPpkContainer(ImportPpkName, tempFile);

                        string text = File.ReadAllText(tempFile);

                        File.Delete(tempFile);

                        if (database.Connect())
                        {
                            // if we have a match and they chose to replace it then update the existing, if not insert a new one
                            if (match == null)
                            {
                                RsaPpks rsaPpk = new RsaPpks
                                {
                                    Name = ImportPpkName,
                                    RsaPpkXml = text
                                };

                                int insertResult = database.Insert(new List<RsaPpks>() { rsaPpk });

                                if (insertResult > -1)
                                {
                                    Dispatcher.BeginInvoke((Action)(() =>
                                    {
                                        RsaPpks.Add(rsaPpk);
                                    }), DispatcherPriority.Normal);
                                }
                            }
                            else
                            {
                                match.RsaPpkXml = text;

                                database.Update(new List<RsaPpks>() { match });
                            }

                            database.Disconnect();
                        }

                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            OverlayVisibility = Visibility.Collapsed;
                            OverlayMessage = "";
                            DatabaseLog += "RSA PPK successfully inserted into the database";
                        }), DispatcherPriority.Normal);
                    });
                }
            }
        }

        private void RefreshKeys()
        {
            if (database.Connect())
            {
                List<RsaPpks> allKeys = database.Select<RsaPpks>();

                RsaPpks.Clear();
                RsaPpks.AddRange(allKeys);

                database.Disconnect();
            }
        }

        private void RemoveKeys()
        {
            List<RsaPpks> selectedItems = RsaKeysViewSql?.SelectedItems.OfType<RsaPpks>().ToList();

            if (database.Connect())
            {
                int result = database.Delete(selectedItems);

                if (result == -1)
                {
                    MessageBox.Show("There was an issue removing the selected items from the database. See database log below for details.", "Removal Error");
                }
                else
                {
                    RsaPpks.RemoveRange(selectedItems);
                }

                database.Disconnect();
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
