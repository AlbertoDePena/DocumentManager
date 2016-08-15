namespace DocumentManager.ViewModel
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GalaSoft.MvvmLight;
    using Helper;
    using Model;

    public class DocumentViewModel : ObservableObject
    {
        private bool _isSelected;

        public DocumentViewModel(Document document, IEnumerable<ItemLookup> itemLookups)
        {
            Model = document;

            var fileInfo = new FileInfo(document.FilePath);

            Extension = fileInfo.Extension;
            Size = fileInfo.Length.ToFileSize();
            IsPdf = fileInfo.Extension.ToLower().EndsWith("pdf");

            var item = itemLookups.FirstOrDefault(i => i.Id == document.DocumentTypeId);

            if (item != null) DocumentTypeName = item.Value;
        }

        public string Description
        {
            get
            {
                return Model == null ? null : Model.Description;
            }
        }

        public string DocumentTypeName { get; private set; }

        public string Extension { get; private set; }

        public string FilePath
        {
            get
            {
                return Model == null ? null : Model.FilePath;
            }
        }

        public int Id
        {
            get
            {
                return Model == null ? 0 : Model.Id;
            }
        }

        public string Size { get; private set; }

        public bool IsPdf { get; private set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected == value) return;

                _isSelected = value;
                RaisePropertyChanged(App.IsSelectedPropertyName);
            }
        }

        public Document Model { get; private set; }

        public string Name
        {
            get
            {
                return Model == null ? null : Model.Name;
            }
        }
    }
}