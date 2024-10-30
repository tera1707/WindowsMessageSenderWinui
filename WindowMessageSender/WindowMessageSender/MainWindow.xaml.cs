using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;

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
    }

    private void myButton_Click(object sender, RoutedEventArgs e)
    {
        WindowTitle.Clear();

        string windowName;
        string windoClass;

        PInvoke.EnumWindows(((hWnd, param) =>
        {
            int bufferSize = PInvoke.GetWindowTextLength(hWnd) + 1;
            unsafe
            {
                var isVisible = PInvoke.IsWindowVisible(hWnd);

                fixed (char* windowNameChars = new char[bufferSize])
                {
                    if (PInvoke.GetWindowText(hWnd, windowNameChars, bufferSize) == 0)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        if (errorCode != 0)
                            throw new Win32Exception(errorCode);

                        return true;
                    }

                    windowName = new string(windowNameChars);
                }

                fixed (char* windoClassChars = new char[256])
                {
                    if (PInvoke.GetClassName(hWnd, windoClassChars, 256) == 0)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        if (errorCode != 0)
                            throw new Win32Exception(errorCode);

                        return true;
                    }

                    windoClass = new string(windoClassChars);
                }

                // 検索ワードが入力されてて、ヒットしなかったら追加しない
                if (!string.IsNullOrEmpty(tbWindowTitleSearch.Text) && !windowName.Contains(tbWindowTitleSearch.Text))
                    return true;
                if (!string.IsNullOrEmpty(tbWindowClassSearch.Text) && !windoClass.Contains(tbWindowClassSearch.Text))
                    return true;

                if (VisibleOnly.IsOn)
                {
                    WindowTitle.Add(new WindowData(hWnd, windowName, windoClass, isVisible));
                }
                else if (isVisible == true)
                {
                    WindowTitle.Add(new WindowData(hWnd, windowName, windoClass, isVisible));
                }

                return true;
            }
        }), (LPARAM)0);

        WindowList.ItemsSource = WindowTitle;
    }

    private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        Button_Click(null, null);
    }

    // Msg送信ボタン押下
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (WindowList.SelectedItem is WindowData wd)
        {
            var msg = UInt32.Parse(tbMsgNumber.Text, System.Globalization.NumberStyles.HexNumber);
            var wp = UInt32.Parse(tbWpNumber.Text, System.Globalization.NumberStyles.HexNumber);
            var lp = Int32.Parse(tbLpNumber.Text, System.Globalization.NumberStyles.HexNumber);
            PInvoke.PostMessage(wd.hWnd, msg, (WPARAM)wp, (LPARAM)lp);
        }
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        tbWindowTitleSearch.Text = "";
        tbWindowClassSearch.Text = "";
    }
}
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool v)
        {
            return v ? "可視" : "不可視";
        }
        return "不可視";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

internal class WindowData
{
    public HWND hWnd {  get; set; }
    public string Title { get; set; }
    public string ClassName { get; set; }
    public bool IsVisible { get; set; }

    public WindowData(HWND hwnd, string title, string className, bool isVisible)
    {
        hWnd = hwnd;
        Title = title;
        ClassName = className;
        IsVisible = isVisible;
    }
}
