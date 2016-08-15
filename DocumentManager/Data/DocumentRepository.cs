namespace DocumentManager.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Forms;
    using System.Xml;
    using Helper;
    using Model;

    public static class DocumentRepository
    {
        private const string FileName = @"Binary\Documents.xml";

        public static DocumentData Load()
        {
            var fileName = string.Format(@"{0}\{1}", Application.StartupPath, FileName);

            using (
                var reader = XmlDictionaryReader.CreateTextReader(new FileStream(fileName, FileMode.Open),
                    new XmlDictionaryReaderQuotas()))
            {
                var serializer = new DataContractSerializer(typeof (DocumentData));

                var documentData = (DocumentData) serializer.ReadObject(reader, true);

                var hasDocumentTypes = documentData.DocumentTypes != null && documentData.DocumentTypes.Any();

                if (!hasDocumentTypes)
                {
                    documentData.DocumentTypes = new List<DocumentType>
                    {
                        new DocumentType
                        {
                            Id = (int) DocumentTypeEnum.Imported,
                            Name = DocumentTypeNameEnum.Imported
                        }
                    };
                }

                var hasDocuments = documentData.Documents != null && documentData.Documents.Any();

                if (!hasDocuments)
                    documentData.Documents = new List<Document>();

                return documentData;
            }
        }

        public static void Save(DocumentData data)
        {
            var fileName = string.Format(@"{0}\{1}", Application.StartupPath, FileName);

            using (var writer = XmlDictionaryWriter.CreateTextWriter(new FileStream(fileName, FileMode.Create)))
            {
                var serializer = new DataContractSerializer(typeof (DocumentData));

                serializer.WriteObject(writer, data);
            }
        }
    }
}