namespace DocumentManager.Design
{
    using System.Collections.Generic;
    using Helper;
    using Model;

    public class DesignDocumentService : IDocumentService
    {
        public string AddDocument(Document document, bool convertToPdf)
        {
            return string.Empty;
        }

        public List<Document> BatchImport(IEnumerable<string> fileNames)
        {
            return new List<Document>();
        }

        public bool CanDeleteDocumentType(int documentTypeId)
        {
            return true;
        }

        public void DeleteDocument(int id)
        {
        }

        public void DeleteDocumentType(int id)
        {
        }

        public void EditDocument(Document document)
        {
        }

        public void ExportDocument(Document document, string directory)
        {
        }

        public int GetDocumentId()
        {
            return 0;
        }

        public List<ItemLookup> GetDocumentTypeLookup()
        {
            var itemLookups = new List<ItemLookup>();

            foreach (var documentType in GetDocumentTypes())
            {
                var lookup = new ItemLookup();

                lookup.WithId(documentType.Id).WithValue(documentType.Name);

                itemLookups.Add(lookup);
            }

            return itemLookups;
        }

        public IEnumerable<DocumentType> GetDocumentTypes()
        {
            return new List<DocumentType>
            {
                new DocumentType {Id = (int) DocumentTypeEnum.Imported, Name = DocumentTypeNameEnum.Imported}
            };
        }

        public bool HasDocuments()
        {
            return true;
        }

        public void SaveDocumentTypes(IEnumerable<DocumentType> items)
        {
        }

        public List<Document> Search(string searchBy, int? documentTypeId = null)
        {
            return new List<Document>
            {
                new Document
                {
                    Id = 0,
                    Name = "Test Document",
                    Description = "This is a test",
                    DocumentTypeId = (int) DocumentTypeEnum.Imported
                }
            };
        }
    }
}