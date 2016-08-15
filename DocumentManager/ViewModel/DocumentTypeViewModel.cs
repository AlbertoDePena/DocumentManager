namespace DocumentManager.ViewModel
{
    using GalaSoft.MvvmLight;
    using Model;

    public class DocumentTypeViewModel : ObservableObject
    {
        private bool _hasChanges;
        private string _name;

        public DocumentTypeViewModel(DocumentType model)
        {
            Model = model;

            _name = model.Name;

            _hasChanges = string.IsNullOrEmpty(_name);
        }

        public bool HasChanges
        {
            get { return _hasChanges; }
            private set
            {
                if (_hasChanges == value) return;

                _hasChanges = value;
                RaisePropertyChanged(App.HasChangesPropertyName);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;

                _name = value;

                HasChanges = true;

                RaisePropertyChanged(App.NamePropertyName);
            }
        }

        public DocumentType Model { get; private set; }

        public void OnPreSave()
        {
            Model.Name = Name;
        }
    }
}