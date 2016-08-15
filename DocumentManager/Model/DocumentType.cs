namespace DocumentManager.Model
{
    using System.Runtime.Serialization;

    [DataContract]
    public class DocumentType
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}