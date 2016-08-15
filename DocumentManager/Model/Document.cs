namespace DocumentManager.Model
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Document
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int DocumentTypeId { get; set; }

        [DataMember]
        public string FilePath { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}