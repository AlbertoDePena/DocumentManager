namespace DocumentManager.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Helper;
    using Model;

    public class SearchViewModel : BaseViewModel
    {
        private string _message;
        private string _searchCriteria;

        public SearchViewModel(IDocumentService service) : base(service)
        {
            ViewModels = new ObservableCollection<DocumentViewModel>();

            SearchCommand = new RelayCommand(ExecuteSearch, CanSearch);
            ClearFieldsCommand = new RelayCommand(ExecuteClearFields, CanClearFields);
            ViewDocumentsCommand = new RelayCommand<Window>(ExecuteViewDocument, CanViewDocument);
        }

        public RelayCommand ClearFieldsCommand { get; private set; }

        public RelayCommand SearchCommand { get; private set; }

        public RelayCommand<Window> ViewDocumentsCommand { get; private set; }

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

        public string SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria == value) return;

                _searchCriteria = value;
                RaisePropertyChanged(App.SearchCriteriaPropertyName);
            }
        }

        public ObservableCollection<DocumentViewModel> ViewModels { get; private set; }

        private bool CanClearFields()
        {
            return DocumentType != null || !string.IsNullOrEmpty(SearchCriteria) || ViewModels.Any();
        }

        private bool CanSearch()
        {
            return !string.IsNullOrEmpty(SearchCriteria) || DocumentType != null;
        }

        private bool CanViewDocument(Window e)
        {
            return ViewModels.Any(vm => vm.IsSelected);
        }

        private void ExecuteClearFields()
        {
            SearchCriteria = string.Empty;
            DocumentType = null;
            ViewModels.Clear();
            Message = string.Empty;
        }

        private void ExecuteSearch()
        {
            try
            {
                ViewModels.Clear();

                var results = DocumentType != null
                    ? DocumentService.Search(SearchCriteria ?? string.Empty, DocumentType.Id)
                    : DocumentService.Search(SearchCriteria ?? string.Empty);

                foreach (var document in results)
                {
                    ViewModels.Add(new DocumentViewModel(document, DocumentTypeLookups));
                }

                Message = string.Format("{0} document(s) found.", ViewModels.Count);

                RaisePropertyChanged(App.ViewModelsPropertyName);
            }
            catch (Exception ex)
            {
                App.DisplayErrorMessage(ex.ToMessage());
            }
        }

        private void ExecuteViewDocument(Window e)
        {
            var items = ViewModels.Where(vm => vm.IsSelected).Select(vm => vm.Model).ToList();

            Messenger.Default.Send(items, App.DocumentSearch);

            e.Close();
        }
    }
}