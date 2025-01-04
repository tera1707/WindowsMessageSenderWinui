using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace ConsoleApp1;

internal class Program
{
    internal static List<WindowData> WindowTitle = new List<WindowData>();

    static void Main(string[] args)
    {
        PInvoke.EnumWindows(OnFindWindows, (LPARAM)0);

        foreach (var item in WindowTitle)
        {
            var v = item.IsVisible ? "〇" : "×";
            Debug.WriteLine($"{v}, {item.Title}, {item.ClassName}");
        }
    }

    private static Windows.Win32.Foundation.BOOL OnFindWindows(Windows.Win32.Foundation.HWND hWnd, Windows.Win32.Foundation.LPARAM lp)
    {
        int lpInt = (int)lp;
        string space = "";

        string windowName;
        string windoClass;
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

            //if (!isVisible)
            //    return true;

            for (int i = 0; i < lpInt; i++)
            {
                space += "　";
            }
            windowName = space + windowName;
            windoClass = space + windoClass;

            WindowTitle.Add(new WindowData(hWnd, windowName, windoClass, isVisible));

            PInvoke.EnumChildWindows(hWnd, OnFindWindows, lpInt + 1);

            return true;
        }
    }
}


internal class WindowData
{
    public HWND hWnd { get; set; }
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