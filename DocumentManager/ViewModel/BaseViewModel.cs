namespace DocumentManager.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using Model;

    public abstract class BaseViewModel : ViewModelBase
    {
        protected readonly IDocumentService DocumentService;
        private ItemLookup _documentType;

        protected BaseViewModel(IDocumentService service)
        {
            DocumentService = service;

            CancelCommand = new RelayCommand<Window>(ExecuteCancel);
        }

        public ObservableCollection<ItemLookup> DocumentTypeLookups
        {
            get
            {
                return new ObservableCollection<ItemLookup>(DocumentService.GetDocumentTypeLookup());
            }
        }

        public RelayCommand<Window> CancelCommand { get; private set; }

        public ItemLookup DocumentType
        {
            get { return _documentType; }
            set
            {
                if (Equals(_documentType, value)) return;

                _documentType = value;
                RaisePropertyChanged(App.DocumentTypePropertyName);
            }
        }

        private static void ExecuteCancel(Window window)
        {
            window.Close();
        }
    }
}