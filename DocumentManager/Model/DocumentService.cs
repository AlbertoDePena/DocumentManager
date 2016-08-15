namespace DocumentManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Data;
    using GalaSoft.MvvmLight.Threading;
    using Helper;
    using iTextSharp.text.pdf;
    using Rectangle = iTextSharp.text.Rectangle;

    public class DocumentService : IDocumentService
    {
        private DocumentData _data;

        public DocumentService()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { _data = DocumentRepository.Load(); });
        }

        public string AddDocument(Document document, bool convertToPdf)
        {
            CreateDestinationDirectory();

            var destinationFileName = CreateDocument(document, convertToPdf, false);

            Save();

            return destinationFileName;
        }

        public List<Document> BatchImport(IEnumerable<string> fileNames)
        {
            CreateDestinationDirectory();

            var result = new List<Document>();

            var count = 1;

            foreach (var document in
                fileNames.Select(
                    fileName =>
                        new Document
                        {
                            Id = GetDocumentId() + count++,
                            DocumentTypeId = (int) DocumentTypeEnum.Imported,
                            FilePath = fileName,
                            Name = Path.GetFileNameWithoutExtension(fileName)
                        }))
            {
                CreateDocument(document, deleteSource: false);

                result.Add(document);
            }

            Save();

            return result;
        }

        public bool CanDeleteDocumentType(int documentTypeId)
        {
            return _data.Documents.All(d => d.DocumentTypeId != documentTypeId);
        }

        public void DeleteDocument(int id)
        {
            var document = _data.Documents.FirstOrDefault(x => x.Id == id);

            if (document == null) return;

            File.Delete(document.FilePath);

            _data.Documents.Remove(document);

            Save();
        }

        public void DeleteDocumentType(int id)
        {
            var item = _data.DocumentTypes.FirstOrDefault(x => x.Id == id);

            if (item == null) return;

            _data.DocumentTypes.Remove(item);

            Save();
        }

        public void EditDocument(Document document)
        {
            var item = _data.Documents.FirstOrDefault(x => x.Id == document.Id);

            if (item != null) _data.Documents.Remove(item);

            _data.Documents.Add(document);

            Save();
        }

        public void ExportDocument(Document document, string directory)
        {
            File.Copy(document.FilePath,
                string.Format(@"{0}\{1}{2}", directory, document.Name, new FileInfo(document.FilePath).Extension));
        }

        public int GetDocumentId()
        {
            if (!_data.Documents.Any()) return 1;

            return _data.Documents.Max(d => d.Id) + 1;
        }

        public List<ItemLookup> GetDocumentTypeLookup()
        {
            var items = new List<ItemLookup>();

            foreach (var documentType in GetDocumentTypes())
            {
                var item = new ItemLookup();

                item.WithId(documentType.Id).WithValue(documentType.Name);

                items.Add(item);
            }

            return items;
        }

        public IEnumerable<DocumentType> GetDocumentTypes()
        {
            return _data.DocumentTypes;
        }

        public bool HasDocuments()
        {
            return _data.Documents != null && _data.Documents.Any();
        }

        public void SaveDocumentTypes(IEnumerable<DocumentType> items)
        {
            foreach (var item in items)
            {
                var type = _data.DocumentTypes.FirstOrDefault(x => x.Id == item.Id);

                if (type != null) _data.DocumentTypes.Remove(type);

                _data.DocumentTypes.Add(item);
            }

            Save();
        }

        public List<Document> Search(string searchBy, int? documentTypeId = null)
        {
            searchBy = searchBy.ToUpper();

            var query =
                _data.Documents.Where(
                    d =>
                        (!string.IsNullOrEmpty(d.Name) && d.Name.ToUpper().Contains(searchBy)) ||
                        (!string.IsNullOrEmpty(d.Description) && d.Description.ToUpper().Contains(searchBy)));

            if (documentTypeId.HasValue) query = query.Where(d => d.DocumentTypeId == documentTypeId);

            return query.OrderBy(d => d.Name).ToList();
        }

        private static bool CanConvertToPdf(string extension)
        {
            extension = extension.ToLower();

            return extension.Equals(".jpeg") || extension.Equals(".jpg") || extension.Equals(".tiff") ||
                   extension.Equals(".tif") || extension.Equals(".gif") || extension.Equals(".bmp") ||
                   extension.Equals(".png");
        }

        private static void ConvertToPdf(string sourceFilePath, string destinationFileName, bool deleteSource)
        {
            var document = new iTextSharp.text.Document();

            var writer = PdfWriter.GetInstance(document, new FileStream(destinationFileName, FileMode.Create));

            var bm = new Bitmap(sourceFilePath);

            var total = bm.GetFrameCount(FrameDimension.Page);

            document.Open();

            for (var k = 0; k < total; ++k)
            {
                bm.SelectActiveFrame(FrameDimension.Page, k);

                var img = iTextSharp.text.Image.GetInstance(bm, ImageFormat.Bmp);

                var height = bm.Height*(72f/bm.VerticalResolution);

                var width = bm.Width*(72f/bm.HorizontalResolution);

                img.ScaleAbsolute(width, height);

                img.SetAbsolutePosition(0, 0);

                document.SetPageSize(new Rectangle(width, height));

                document.NewPage();

                writer.DirectContent.AddImage(img);
            }

            document.Close();

            if (deleteSource) File.Delete(sourceFilePath);
        }

        private static void CreateDestinationDirectory()
        {
            if (!File.Exists(GetDestinationDirectory())) Directory.CreateDirectory(GetDestinationDirectory());
        }

        private string CreateDocument(Document document, bool convertToPdf = true, bool deleteSource = true)
        {
            var info = new FileInfo(document.FilePath);

            if (string.IsNullOrEmpty(document.Name)) document.Name = info.Name;

            string destinationFileName;

            if (CanConvertToPdf(info.Extension) && convertToPdf)
            {
                destinationFileName = string.Format(@"{0}\{1}.{2}", GetDestinationDirectory(), Guid.NewGuid(), "pdf");

                ConvertToPdf(document.FilePath, destinationFileName, deleteSource);
            }
            else
            {
                destinationFileName = string.Format(@"{0}\{1}{2}", GetDestinationDirectory(), Guid.NewGuid(),
                    info.Extension);

                File.Copy(document.FilePath, destinationFileName);

                if (deleteSource) File.Delete(document.FilePath);
            }

            document.FilePath = destinationFileName;

            _data.Documents.Add(document);

            return destinationFileName;
        }

        private static string GetDestinationDirectory()
        {
            return string.Format(@"{0}\Binary", Application.StartupPath);
        }

        private void Save()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { DocumentRepository.Save(_data); });
        }
    }
}