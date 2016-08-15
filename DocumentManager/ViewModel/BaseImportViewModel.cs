namespace DocumentManager.ViewModel
{
    using System.Windows;
    using GalaSoft.MvvmLight.Command;
    using Model;

    public abstract class BaseImportViewModel : BaseViewModel
    {
        private string _filePath;

        protected BaseImportViewModel(IDocumentService service) : base(service)
        {
            ImportCommand = new RelayCommand<Window>(ExecuteImport, CanImport);
            FileDialogCommand = new RelayCommand(ExecuteFileDialog);
        }

        public RelayCommand FileDialogCommand { get; private set; }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath == value) return;

                _filePath = value;
                RaisePropertyChanged(App.FilePathPropertyName);
            }
        }

        public RelayCommand<Window> ImportCommand { get; private set; }

        protected virtual bool CanImport(Window window)
        {
            return true;
        }

        protected virtual void ExecuteFileDialog()
        {
        }

        protected virtual void ExecuteImport(Window window)
        {
        }
    }
}