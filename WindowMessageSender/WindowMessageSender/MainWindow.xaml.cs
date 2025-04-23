using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Graphics;

namespace WindowMessageSender;

public sealed partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged(string propertyName)
        => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    internal ObservableCollection<WindowData> WindowTitle
    {
        get { return _windowTitle; }
        set { _windowTitle = value; OnPropertyChanged(nameof(WindowTitle)); }
    }
    private ObservableCollection<WindowData> _windowTitle = new();

    public void NavigateToMainPage(FromPresetPageToMainPageParameter p)
    {
        contentFrame.Navigate(typeof(MainPage), p, new DrillInNavigationTransitionInfo());
    }
    public void NavigateToPresetPage()
    {
        contentFrame.Navigate(typeof(PresetPage), null, new DrillInNavigationTransitionInfo());
    }

    public MainWindow()
    {
        this.InitializeComponent();

        Title = "好きなウインドウに好きなメッセージを送るツール";

        this.AppWindow.SetIcon("icon.ico");
        this.AppWindow.Resize(new SizeInt32(700, 900));
    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        contentFrame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromBottom });
    }
}


