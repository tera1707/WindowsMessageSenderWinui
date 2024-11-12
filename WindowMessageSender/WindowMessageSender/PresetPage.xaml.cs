using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowMessageSender;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PresetPage : Page
{
    List<PresetData> presets { get; set; } = new List<PresetData>();

    private Action navigateActionToMainPage;
    private Action navigateActionToPresetPage;

    public PresetPage()
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

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        //DeleteAll();

        //var presets = new List<PresetData>();
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        if (localSettings is not null)
        {
            // 項目名、メッセージ番号、WParam、LParamで4項目なので、4で割ったら行数になる
            var rowsCount = localSettings.Values.Count / 4;

            for (int i = 0; i < rowsCount; i++)
            {
                presets.Add(new PresetData((string)localSettings.Values[$"Name{i}"], (int)localSettings.Values[$"Message{i}"], (int)localSettings.Values[$"WParam{i}"], (int)localSettings.Values[$"LParam{i}"]));
            }
            lbPreset.ItemsSource = presets;
        }
    }

    // 「これを使う」ボタン
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

    }

    // 「追加」ボタン
    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        var nameToAdd = tbName.Text;
        var msgToAdd = Convert.ToInt32(tbMessage.Text, 16);
        var wpToAdd = Convert.ToInt32(tbWParam.Text, 16);
        var lpToAdd = Convert.ToInt32(tbLParam.Text, 16);

        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        var rowsCount = localSettings.Values.Count / 4;// 設定の件数

        localSettings.Values[$"Name{rowsCount}"] = nameToAdd;
        localSettings.Values[$"Message{rowsCount}"] = msgToAdd;
        localSettings.Values[$"WParam{rowsCount}"] = wpToAdd;
        localSettings.Values[$"LParam{rowsCount}"] = lpToAdd;

        presets.Add(new PresetData(nameToAdd, msgToAdd, wpToAdd, lpToAdd));
        lbPreset.ItemsSource = new List<PresetData>(presets);// ListをnewしなおさないとListBoxが更新してくれない
    }

    // 「削除」ボタン
    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        var selectedIndex = lbPreset.SelectedIndex;

        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        localSettings.Values.Remove($"Name{selectedIndex}");
        localSettings.Values.Remove($"Message{selectedIndex}");
        localSettings.Values.Remove($"WParam{selectedIndex}");
        localSettings.Values.Remove($"LParam{selectedIndex}");

        presets.RemoveAt(selectedIndex);
        lbPreset.ItemsSource = new List<PresetData>(presets);
    }

    // 「キャンセル」ボタン
    private void Button_Click_3(object sender, RoutedEventArgs e)
    {
        navigateActionToMainPage.Invoke();
    }

    private void DeleteAll()
    {
        foreach (var v in Windows.Storage.ApplicationData.Current.LocalSettings.Values)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(v.Key);
        }
    }
}

public record PresetData(string Name, int Message, int WParam, int LParam);
//{
//    public string Name;
//    public int Message;
//    public int WParam;
//    public int LParam;

//    public PresetData(string name, int msg, int wp, int lp) => (Name, Message, WParam, LParam) = (name, msg, wp, lp);
//}
