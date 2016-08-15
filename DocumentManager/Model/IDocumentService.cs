namespace DocumentManager.Model
{
    using System.Collections.Generic;

    public interface IDocumentService
    {
        string AddDocument(Document document, bool convertToPdf);

        List<Document> BatchImport(IEnumerable<string> fileNames);

        bool CanDeleteDocumentType(int documentTypeId);

        void DeleteDocument(int id);

        void DeleteDocumentType(int id);

        void EditDocument(Document document);

        void ExportDocument(Document document, string directory);

        int GetDocumentId();

        List<ItemLookup> GetDocumentTypeLookup();

        IEnumerable<DocumentType> GetDocumentTypes();

        bool HasDocuments();

        void SaveDocumentTypes(IEnumerable<DocumentType> items);

        List<Document> Search(string searchBy, int? documentTypeId = null);
    }
}