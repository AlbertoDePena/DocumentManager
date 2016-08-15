namespace DocumentManager.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;
    using Helper;
    using Model;

    public class DocumentTypeListViewModel : ViewModelBase
    {
        private readonly IDocumentService _documentService;
        private readonly List<int> _documentsToDelete;
        private DocumentTypeViewModel _currentViewModel;
        private string _error;

        public DocumentTypeListViewModel(IDocumentService service)
        {
            try
            {
                _documentService = service;

                _documentsToDelete = new List<int>();

                AddCommand = new RelayCommand(ExecuteAdd);
                SaveCommand = new RelayCommand<Window>(ExecuteSave, CanExecuteSave);
                DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
                CancelCommand = new RelayCommand<Window>(ExecuteCancel);

                var viewModels = new ObservableCollection<DocumentTypeViewModel>();

                foreach (var documentType in _documentService.GetDocumentTypes())
                {
                    viewModels.Add(new DocumentTypeViewModel(documentType));
                }

                ViewModels = viewModels;

                _currentViewModel = ViewModels.FirstOrDefault();
            }
            catch (Exception ex)
            {
                App.DisplayErrorMessage(ex.ToMessage());
            }
        }

        public RelayCommand AddCommand { get; private set; }

        public RelayCommand<Window> CancelCommand { get; private set; }

        public RelayCommand<Window> SaveCommand { get; private set; }

        public RelayCommand DeleteCommand { get; private set; }

        public DocumentTypeViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                if (_currentViewModel == value) return;

                _currentViewModel = value;
                RaisePropertyChanged(App.CurrentViewModelPropertyName);
            }
        }

        public string Error
        {
            get { return _error; }
            set
            {
                if (_error == value) return;

                _error = value;

                RaisePropertyChanged(App.ErrorPropertyName);
            }
        }

        public ObservableCollection<DocumentTypeViewModel> ViewModels { get; private set; }

        private bool CanExecuteDelete()
        {
            if (CurrentViewModel == null) return false;

            return (CurrentViewModel.Model.Id != (int) DocumentTypeEnum.Imported &&
                    _documentService.CanDeleteDocumentType(CurrentViewModel.Model.Id));
        }

        private bool CanExecuteSave(Window window)
        {
            Error = string.Empty;

            var item = ViewModels.GroupBy(vm => vm.Name).FirstOrDefault(grp => grp.Count() > 1);

            if (item != null) Error = string.Format("No duplicate names ({0})", item.Key);
            else if (ViewModels.Any(vm => string.IsNullOrEmpty(vm.Name))) Error = "Name is required.";

            return string.IsNullOrEmpty(Error);
        }

        private void ExecuteAdd()
        {
            var id = ViewModels.Max(vm => vm.Model.Id) + 1;

            var newVm = new DocumentTypeViewModel(new DocumentType {Id = id, Name = "New document type"});

            ViewModels.Add(newVm);

            CurrentViewModel = newVm;
        }

        private static void ExecuteCancel(Window window)
        {
            window.Close();
        }

        private void ExecuteDelete()
        {
            _documentsToDelete.Add(CurrentViewModel.Model.Id);

            ViewModels.Remove(CurrentViewModel);

            CurrentViewModel = ViewModels.FirstOrDefault();
        }

        private void ExecuteSave(Window window)
        {
            try
            {
                _documentsToDelete.ForEach(doc => _documentService.DeleteDocumentType(doc));

                foreach (var vm in ViewModels)
                {
                    vm.OnPreSave();
                }

                _documentService.SaveDocumentTypes(ViewModels.Where(vm => vm.HasChanges).Select(vm => vm.Model).ToList());
            }
            catch (Exception ex)
            {
                App.DisplayErrorMessage(ex.ToMessage());
            }
            finally
            {
                window.Close();
            }
        }
    }
}