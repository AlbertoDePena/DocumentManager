namespace DocumentManager.Data
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Model;

    [DataContract]
    public class DocumentData
    {
        [DataMember(Name = "DocumentTypes")]
        public List<DocumentType> DocumentTypes { get; set; }

        [DataMember(Name = "Documents")]
        public List<Document> Documents { get; set; }
    }
}