using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace WindowMessageSender;

public sealed partial class MainPage : Page
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

    private Action navigateActionToMainPage;
    private Action navigateActionToPresetPage;

    public MainPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is List<Action> navActions)
        {
            navigateActionToMainPage = navActions[0];
            navigateActionToPresetPage = navActions[1];
        }
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

    // クリアボタン
    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        tbWindowTitleSearch.Text = "";
        tbWindowClassSearch.Text = "";
    }

    // プリセットボタン
    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        navigateActionToPresetPage.Invoke();
    }
}
