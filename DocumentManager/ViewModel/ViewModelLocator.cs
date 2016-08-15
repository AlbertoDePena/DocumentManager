namespace DocumentManager.ViewModel
{
    using System.Diagnostics.CodeAnalysis;
    using Design;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;
    using Model;

    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
                SimpleIoc.Default.Register<IDocumentService, DesignDocumentService>();
            else
                SimpleIoc.Default.Register<IDocumentService, DocumentService>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        [SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public static void Cleanup()
        {
            ServiceLocator.Current.GetInstance<MainViewModel>().Cleanup();

            SimpleIoc.Default.Unregister<MainViewModel>();
            SimpleIoc.Default.Unregister<IDocumentService>();
        }
    }
}