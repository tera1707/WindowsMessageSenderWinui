using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

    private List<Action> navigateActions = new List<Action>();

    public MainWindow()
    {
        this.InitializeComponent();

        Title = "好きなウインドウに好きなメッセージを送るツール";

        // WinUI3のウインドウのハンドルを取得
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        // そのウインドウのIDを取得
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        // そこからAppWindowを取得する
        Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        appWindow.Resize(new SizeInt32(700, 900));

        navigateActions.Add(() => { contentFrame.Navigate(typeof(MainPage), navigateActions, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromBottom }); });
        navigateActions.Add(() => { contentFrame.Navigate(typeof(PresetPage), navigateActions, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromBottom }); });


    }

    private void Grid_Loaded(object sender, RoutedEventArgs e)
    {
        contentFrame.Navigate(typeof(MainPage), navigateActions, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromBottom });
    }
}
