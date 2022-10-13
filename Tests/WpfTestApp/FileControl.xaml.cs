using System.Windows;
using System.Windows.Controls;
namespace WpfTestApp;

public partial class FileControl : UserControl
{
    public FileControl()
    {
        InitializeComponent();
    }

    public string File
    {
        get { return (string)GetValue(FileProperty); }
        set { SetValue(FileProperty, value); }
    }

    public static readonly DependencyProperty FileProperty =
        DependencyProperty.Register("File", typeof(string), typeof(FileControl), new PropertyMetadata(""));

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        MainWindow.GetSetFilter();
        Target.Text = MainWindow.Get(File);
        MainWindow.GetSetFilter("");
    }
}
