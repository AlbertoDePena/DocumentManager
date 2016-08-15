namespace DocumentManager.ViewModel
{
    using System;
    using System.Windows;
    using GalaSoft.MvvmLight.Messaging;
    using Helper;
    using Microsoft.Win32;
    using Model;

    public class ImportViewModel : BaseImportViewModel, IDocumentViewModel
    {
        private bool _convertToPdf;
        private string _description;
        private string _error;
        private string _name;

        public ImportViewModel(IDocumentService service) : base(service)
        {
        }

        public bool ConvertToPdf
        {
            get { return _convertToPdf; }
            set
            {
                if (_convertToPdf == value) return;

                _convertToPdf = value;
                RaisePropertyChanged(App.ConvertToPdfPropertyName);
            }
        }

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

        protected override bool CanImport(Window window)
        {
            Error = string.Empty;

            if (string.IsNullOrEmpty(Name)) Error = "Name is required.";
            else if (DocumentType == null) Error = "Document Type is required.";
            else if (string.IsNullOrEmpty(FilePath)) Error = "File Path is required.";

            return string.IsNullOrEmpty(Error);
        }

        protected override void ExecuteFileDialog()
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true) FilePath = dialog.FileName;
        }

        protected override void ExecuteImport(Window window)
        {
            try
            {
                var item = new Document
                {
                    Id = DocumentService.GetDocumentId(),
                    Name = Name,
                    Description = Description,
                    FilePath = FilePath,
                    DocumentTypeId = DocumentType.Id
                };

                item.FilePath = DocumentService.AddDocument(item, ConvertToPdf);

                Messenger.Default.Send(new DocumentViewModel(item, DocumentTypeLookups), App.DocumentImport);
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