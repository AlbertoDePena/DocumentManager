namespace DocumentManager.ViewModel
{
    public interface IDocumentViewModel
    {
        string Description { get; set; }

        string Error { get; set; }

        string Name { get; set; }
    }
}