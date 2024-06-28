using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace WindowMessageSender
{
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
                PInvoke.PostMessage(wd.hWnd, msg, (WPARAM)0, (LPARAM)0);
            }
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
}
