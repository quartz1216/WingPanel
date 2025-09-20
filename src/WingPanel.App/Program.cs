using System;
using Microsoft.UI.Xaml;

namespace WingPanel.App;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.Start(_ => new App());
    }
}
