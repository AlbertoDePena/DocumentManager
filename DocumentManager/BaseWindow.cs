namespace DocumentManager
{
    using System.Windows;
    using System.Windows.Input;

    public abstract class BaseWindow : Window
    {
        protected BaseWindow()
        {
            MouseDown += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Left) DragMove();
            };
        }
    }
}