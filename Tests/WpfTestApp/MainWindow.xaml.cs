using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace WpfTestApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public static string Url => "http://localhost:5065";

    public static HttpClient GetHttpClient()
    {
        return new HttpClient() { BaseAddress = new System.Uri(Url) };
    }

    public static void GetSetFilter(string filter = "*")
    {
        var http = GetHttpClient();
        var response = http.GetAsync("/set-filter/" + filter).Result;
        var str = response.Content.ReadAsStringAsync().Result;
    }

    public static string Get(string url)
    {
        var http = GetHttpClient();
        var response = http.GetAsync(url).Result;
        return response.Content.ReadAsStringAsync().Result;
    }

    public static void Hint(string hint)
    {
        var http = GetHttpClient();
        var response = http.GetAsync("/--hint--/" + hint).Result;
        var str = response.Content.ReadAsStringAsync().Result;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        System.Threading.Thread.Sleep(2000);

        try
        {
            using var http = GetHttpClient();
            var r = http.GetAsync("list").Result;
            var names = System.Text.Json.JsonSerializer.Deserialize<List<string>>(r.Content.ReadAsStringAsync().Result);
            names.Sort();
            DataContext = names;
        }
        catch (Exception ex)
        {
        }
    }
}
