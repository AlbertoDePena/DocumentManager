namespace DocumentManager
{
    using System.Windows;
    using GalaSoft.MvvmLight.Threading;

    public partial class App
    {
        public const string DocumentDelete = "DocumentManager.DocumentDelete";
        public const string DocumentView = "DocumentManager.DocumentView";
        public const string DocumentSearch = "DocumentManager.DocumentSearch";
        public const string DocumentImport = "DocumentManager.DocumentImport";
        public const string DocumentBatchImport = "DocumentManager.DocumentBatchImport";
        public const string DocumentEdit = "DocumentManager.DocumentEdit";
        public const string ErrorPropertyName = "Error";
        public const string DocumentTypePropertyName = "DocumentType";
        public const string DocumentTypeLookupsPropertyName = "DocumentTypeLookups";
        public const string FilePathPropertyName = "FilePath";
        public const string CurrentViewModelPropertyName = "CurrentViewModel";
        public const string ViewModelsPropertyName = "ViewModels";
        public const string NamePropertyName = "Name";
        public const string HasChangesPropertyName = "HasChanges";
        public const string DescriptionPropertyName = "Description";
        public const string SearchCriteriaPropertyName = "SearchCriteria";
        public const string MessagePropertyName = "Message";
        public const string CurrentDevicePropertyName = "CurrentDevice";
        public const string DevicesPropertyName = "Devices";
        public const string IsSelectedPropertyName = "IsSelected";
        public const string ConvertToPdfPropertyName = "ConvertToPdf";

        static App()
        {
            DispatcherHelper.Initialize();
        }

        public static void DisplayErrorMessage(string message)
        {
            MessageBox.Show(Current.MainWindow,
                message,
                "Document Manager",
                MessageBoxButton.OK);
        }
    }
}