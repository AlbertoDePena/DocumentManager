namespace DocumentManager.ViewModel
{
    using System;
    using System.Linq;
    using System.Windows;
    using GalaSoft.MvvmLight.Messaging;
    using Helper;
    using Microsoft.Win32;
    using Model;

    public class BatchImportViewModel : BaseImportViewModel
    {
        public BatchImportViewModel(IDocumentService service) : base(service)
        {
        }

        private string[] FileNames { get; set; }

        protected override bool CanImport(Window window)
        {
            return !string.IsNullOrEmpty(FilePath);
        }

        protected override void ExecuteFileDialog()
        {
            var dialog = new OpenFileDialog { Multiselect = true, Filter = "All Files (*.*)|*.*" };

            if (dialog.ShowDialog() != true) return;

            FilePath = string.Join(", ", dialog.FileNames.ToList());
            FileNames = dialog.FileNames;
        }

        protected override void ExecuteImport(Window window)
        {
            try
            {
                var items = DocumentService.BatchImport(FileNames);

                Messenger.Default.Send(items, App.DocumentBatchImport);
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