namespace DocumentManager.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Helper;
    using Model;
    using Application = System.Windows.Application;
    using MessageBox = System.Windows.MessageBox;

    public class MainViewModel : BaseViewModel
    {
        private DocumentViewModel _currentViewModel;

        private string _message;

        public MainViewModel(IDocumentService service) : base(service)
        {
            ViewModels = new ObservableCollection<DocumentViewModel>();

            DocumentTypeCommand = new RelayCommand(ExecuteDocumentType);
            SearchCommand = new RelayCommand(ExecuteSearch);
            ExitCommand = new RelayCommand(ExecuteExit);
            ImportCommand = new RelayCommand(ExecuteImport);
            DeleteCommand = new RelayCommand(ExecuteDelete, HasViewModel);
            ExportCommand = new RelayCommand(ExecuteExport, HasViewModel);
            ViewDocumentCommand = new RelayCommand(ExecuteViewDocument, CanViewDocument);
            EditDocumentCommand = new RelayCommand(ExecuteEditDocument, HasViewModel);
            BatchImportCommand = new RelayCommand(ExecuteBatchExport);

            Messenger.Default.Register<DocumentViewModel>(this, App.DocumentImport, OnDocumentImported);
            Messenger.Default.Register<List<Document>>(this, App.DocumentBatchImport, OnDocumentsAdded);
            Messenger.Default.Register<Document>(this, App.DocumentEdit, OnDocumentEdited);
            Messenger.Default.Register<List<Document>>(this, App.DocumentSearch, OnDocumentsAdded);

            Application.Current.MainWindow.Closing += MainWindow_Closing;
        }

        public RelayCommand BatchImportCommand { get; private set; }

        public bool HasViewModel()
        {
            return CurrentViewModel != null;
        }

        public DocumentViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                if (_currentViewModel == value) return;

                _currentViewModel = value;
                RaisePropertyChanged(App.CurrentViewModelPropertyName);

                if (!File.Exists(value.FilePath)) App.DisplayErrorMessage(string.Format("{0} does not exists.", value.Name));

                Messenger.Default.Send(new KeyValuePair<bool, string>(value.IsPdf, value.FilePath), App.DocumentView);
            }
        }

        public RelayCommand DeleteCommand { get; private set; }

        public RelayCommand DocumentTypeCommand { get; private set; }

        public RelayCommand EditDocumentCommand { get; private set; }

        public RelayCommand ExitCommand { get; private set; }

        public RelayCommand ExportCommand { get; private set; }

        public RelayCommand ImportCommand { get; private set; }

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message == value) return;

                _message = value;
                RaisePropertyChanged(App.MessagePropertyName);
            }
        }

        public RelayCommand SearchCommand { get; private set; }

        public RelayCommand ViewDocumentCommand { get; private set; }

        public ObservableCollection<DocumentViewModel> ViewModels { get; private set; }

        private bool CanViewDocument()
        {
            return CurrentViewModel != null && !CurrentViewModel.IsPdf;
        }

        public override void Cleanup()
        {
            Messenger.Default.Unregister(this);
            Messenger.Default.Unregister(Application.Current.MainWindow);

            Application.Current.MainWindow.Closing -= MainWindow_Closing;
        }

        private void ExecuteBatchExport()
        {
            var dialog = new BatchImportWindow {DataContext = new BatchImportViewModel(DocumentService)};

            dialog.ShowDialog();

            dialog.DataContext = null;
        }

        private void ExecuteDelete()
        {
            try
            {
                var dialogResult = MessageBox.Show(Application.Current.MainWindow,
                    "Are you sure you want to delete this document (this cannot be undone)?", "Document Manager",
                    MessageBoxButton.YesNo);

                if (dialogResult != MessageBoxResult.Yes) return;

                Messenger.Default.Send(new NotificationMessageAction(App.DocumentDelete, () =>
                {
                    DocumentService.DeleteDocument(CurrentViewModel.Id);

                    Message = string.Format("Deleted document ({0})", CurrentViewModel.Name);

                    ViewModels.Remove(CurrentViewModel);

                    CurrentViewModel = ViewModels.FirstOrDefault();
                }), App.DocumentDelete);
            }
            catch (Exception ex)
            {
                App.DisplayErrorMessage(ex.ToMessage());
            }
        }

        private void ExecuteDocumentType()
        {
            var dialog = new DocumentTypeWindow {DataContext = new DocumentTypeListViewModel(DocumentService)};

            dialog.ShowDialog();

            dialog.DataContext = null;
        }

        private void ExecuteEditDocument()
        {
            var dialog = new EditWindow {DataContext = new EditViewModel(DocumentService, CurrentViewModel.Model)};

            dialog.ShowDialog();

            dialog.DataContext = null;
        }

        private static void ExecuteExit()
        {
            Application.Current.MainWindow.Close();
        }

        private void ExecuteExport()
        {
            try
            {
                var directory = GetDestinationDirectory();

                if (string.IsNullOrEmpty(directory)) return;

                DocumentService.ExportDocument(CurrentViewModel.Model, directory);

                Message = string.Format(@"Selected document was exported to {0}.", directory);
            }
            catch (Exception ex)
            {
                App.DisplayErrorMessage(ex.ToMessage());
            }
        }

        private void ExecuteImport()
        {
            var dialog = new ImportWindow {DataContext = new ImportViewModel(DocumentService)};

            dialog.ShowDialog();

            dialog.DataContext = null;
        }

        private void ExecuteSearch()
        {
            var dialog = new SearchWindow {DataContext = new SearchViewModel(DocumentService)};

            dialog.ShowDialog();

            dialog.DataContext = null;
        }

        private void ExecuteViewDocument()
        {
            try
            {
                Process.Start(CurrentViewModel.FilePath);
            }
            catch (Exception ex)
            {
                App.DisplayErrorMessage(ex.ToMessage());
            }
        }

        private static string GetDestinationDirectory()
        {
            var dialog = new FolderBrowserDialog();

            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }

        private static void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            var dialogResult = MessageBox.Show(Application.Current.MainWindow,
                "Are you sure you want to close this application?", "Document Manager", MessageBoxButton.YesNo);

            if (dialogResult == MessageBoxResult.Yes) ViewModelLocator.Cleanup();
            else e.Cancel = true;
        }

        private void OnDocumentEdited(Document document)
        {
            var item = ViewModels.FirstOrDefault(vm => vm.Model.Id == document.Id);

            if (item == null) return;

            ViewModels.Remove(item);

            var editedItem = new DocumentViewModel(document, DocumentTypeLookups);

            ViewModels.Add(editedItem);

            CurrentViewModel = editedItem;

            Message = "Edited selected document.";
        }

        private void OnDocumentImported(DocumentViewModel vm)
        {
            ViewModels.Add(vm);

            RaisePropertyChanged(App.ViewModelsPropertyName);

            CurrentViewModel = vm;

            Message = string.Format("Added document ({0}).", vm.Name);
        }

        private void OnDocumentsAdded(List<Document> documents)
        {
            ViewModels.Clear();

            foreach (var document in documents)
            {
                ViewModels.Add(new DocumentViewModel(document, DocumentTypeLookups));
            }

            Message = string.Format("{0} document(s).", ViewModels.Count);

            if (CurrentViewModel == null) CurrentViewModel = ViewModels.FirstOrDefault();
        }
    }
}