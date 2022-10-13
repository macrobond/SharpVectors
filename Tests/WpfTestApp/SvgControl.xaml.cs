using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Dom;
using SharpVectors.Renderers.Wpf;

namespace WpfTestApp;

public partial class SvgControl : UserControl
{
    public SvgControl()
    {
        InitializeComponent();
    }

    public string File
    {
        get { return (string)GetValue(FileProperty); }
        set { SetValue(FileProperty, value); }
    }

    public static readonly DependencyProperty FileProperty =
        DependencyProperty.Register("File", typeof(string), typeof(SvgControl), new PropertyMetadata(""));

    public AccessExternalResourcesMode DocumentMode
    {
        get { return (AccessExternalResourcesMode)GetValue(DocumentModeProperty); }
        set { SetValue(DocumentModeProperty, value); }
    }

    public static readonly DependencyProperty DocumentModeProperty =
        DependencyProperty.Register("DocumentMode", typeof(AccessExternalResourcesMode), typeof(SvgControl), new PropertyMetadata(AccessExternalResourcesMode.Allow));

    public bool CanUseBitmap
    {
        get { return (bool)GetValue(CanUseBitmapProperty); }
        set { SetValue(CanUseBitmapProperty, value); }
    }

    public static readonly DependencyProperty CanUseBitmapProperty =
        DependencyProperty.Register("CanUseBitmap", typeof(bool), typeof(SvgControl), new PropertyMetadata(true));

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        //if (DocumentMode != DocumentMode.None)
        //    return;
        if(DocumentMode == AccessExternalResourcesMode.ThrowError)
        {
            BorderTarget.BorderThickness = new Thickness(3);
        }

        try
        {
            MainWindow.Hint(File + " " + DocumentMode);

            if (DocumentMode == AccessExternalResourcesMode.Allow)
                MainWindow.GetSetFilter();
            else
                MainWindow.GetSetFilter(File);

            var settings = new WpfDrawingSettings() {
                AccessExternalResourcesMode = DocumentMode,
                //IncludeRuntime = false,
                CanUseBitmap = CanUseBitmap,
            };
            using var reader = new FileSvgReader(settings);
            var drawGroup = reader.Read(new Uri(MainWindow.Url + "/" + File));
            ImageTarget.Source = new DrawingImage(drawGroup);
            ImageTarget.Visibility = Visibility.Visible;

            if (DocumentMode == AccessExternalResourcesMode.ThrowError)
            {
                BorderTarget.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
        }
        catch (Exception ex)
        {
            LabelTarget.Content = ex.GetType().FullName + " Message: " + ex.Message;
            LabelTarget.Visibility = Visibility.Visible;

            if (DocumentMode == AccessExternalResourcesMode.ThrowError)
            {
                BorderTarget.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
            }
        }
        finally
        {
            MainWindow.GetSetFilter("");
        }
    }
}
