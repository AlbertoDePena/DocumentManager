namespace DocumentManager.ViewModel
{
    using System;
    using System.Linq;
    using System.Windows;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Helper;
    using Model;

    public class EditViewModel : BaseViewModel, IDocumentViewModel
    {
        private string _description;
        private string _error;
        private string _name;

        public EditViewModel(IDocumentService service, Document model) : base(service)
        {
            Model = model;

            _name = model.Name;
            _description = model.Description;

            DocumentType = DocumentTypeLookups.FirstOrDefault(dt => dt.Id == model.DocumentTypeId);

            SaveCommand = new RelayCommand<Window>(ExecuteSave, CanSave);
        }

        private Document Model { get; set; }

        public RelayCommand<Window> SaveCommand { get; private set; }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;

                _description = value;
                RaisePropertyChanged(App.DescriptionPropertyName);
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

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;

                _name = value;
                RaisePropertyChanged(App.NamePropertyName);
            }
        }

        private bool CanSave(Window window)
        {
            Error = string.Empty;

            if (string.IsNullOrEmpty(Name)) Error = "Name is required.";
            else if (DocumentType == null) Error = "Document Type is required.";

            return string.IsNullOrEmpty(Error);
        }

        private void ExecuteSave(Window window)
        {
            try
            {
                Model.Name = Name;
                Model.Description = Description;
                Model.DocumentTypeId = DocumentType.Id;

                DocumentService.EditDocument(Model);

                Messenger.Default.Send(Model, App.DocumentEdit);
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