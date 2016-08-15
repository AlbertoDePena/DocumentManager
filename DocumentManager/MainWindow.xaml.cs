namespace DocumentManager
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Forms;
    using GalaSoft.MvvmLight.Messaging;

    public partial class MainWindow
    {
        private readonly string _fileName = string.Format(@"{0}\not-a-pdf.pdf", Application.StartupPath);

        public MainWindow()
        {
            InitializeComponent();

            Messenger.Default.Register<KeyValuePair<bool, string>>(this, App.DocumentView, OnDocumentChanged);
            Messenger.Default.Register<NotificationMessageAction>(this, App.DocumentDelete, OnDeleteDocument);
        }

        private void OnDeleteDocument(NotificationMessageAction data)
        {
            PdfViewer.Navigate("about:blank");

            Thread.Sleep(1000);

            data.Execute();
        }

        private void OnDocumentChanged(KeyValuePair<bool, string> data)
        {
            var isPdf = data.Key;
            var filePath = data.Value;

            PdfViewer.Navigate(isPdf ? new Uri(filePath) : new Uri(_fileName));
        }
    }
}