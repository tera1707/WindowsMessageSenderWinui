using Windows.Win32.Foundation;

namespace WindowMessageSender;


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
