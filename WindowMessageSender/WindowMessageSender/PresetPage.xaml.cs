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
using Windows.Storage;

namespace WindowMessageSender;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PresetPage : Page
{
    List<PresetData> presets { get; set; } = new List<PresetData>();

    private Action<FromPresetPageToMainPageParameter> navigateActionToMainPage = ((MainWindow)App.CurrentWindow).NavigateToMainPage;
    private Action navigateActionToPresetPage = ((MainWindow)App.CurrentWindow).NavigateToPresetPage;

    public PresetPage()
    {
        this.InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        //DeleteAll();

        //var presets = new List<PresetData>();
        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        if (localSettings is not null)
        {
            // 項目名、メッセージ番号、WParam、LParamで4項目なので、4で割ったら行数になる
            var rowsCount = localSettings.Values.Count;

            foreach (var val in localSettings.Values.OrderBy(x => x.Key))
            {
                var v = (ApplicationDataCompositeValue)localSettings.Values[val.Key];
                presets.Add(new PresetData((string)v["Name"], (int)v["Message"], (int)v["WParam"], (int)v["LParam"]));
            }
            lbPreset.ItemsSource = presets;
        }
    }

    // 「これを使う」ボタン
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = presets[lbPreset.SelectedIndex];
        var p = new FromPresetPageToMainPageParameter(selectedItem.Message, selectedItem.WParam, selectedItem.LParam);
        navigateActionToMainPage.Invoke(p);
    }

    // 「追加」ボタン
    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        var nameToAdd = tbName.Text;
        var msgToAdd = Convert.ToInt32(tbMessage.Text, 16);
        var wpToAdd = Convert.ToInt32(tbWParam.Text, 16);
        var lpToAdd = Convert.ToInt32(tbLParam.Text, 16);

        var localSettings = ApplicationData.Current.LocalSettings;
        var rowsCount = localSettings.Values.Count;// 設定の件数

        var v = new ApplicationDataCompositeValue();
        v["Name"] = nameToAdd;
        v["Message"] = msgToAdd;
        v["WParam"] = wpToAdd;
        v["LParam"] = lpToAdd;

        // 現在の最大Index+1をキーにした項目を追加してやる（追加した項目の並び順が一番最後になるように）
        var maxIndex = ApplicationData.Current.LocalSettings.Values.Count > 0 ? ApplicationData.Current.LocalSettings.Values.Select(x => int.Parse(x.Key)).Max() : 0;
        localSettings.Values.Add((maxIndex + 1).ToString(), v);

        presets.Add(new PresetData(nameToAdd, msgToAdd, wpToAdd, lpToAdd));
        lbPreset.ItemsSource = new List<PresetData>(presets);// ListをnewしなおさないとListBoxが更新してくれない
    }

    // 「削除」ボタン
    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        var selectedIndex = lbPreset.SelectedIndex;

        var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        var target = GetKeyAt(selectedIndex);

        if (target is not null)
        {
            localSettings.Values.Remove(target);
        }

        presets.RemoveAt(selectedIndex);
        lbPreset.ItemsSource = new List<PresetData>(presets);
    }

    // 「キャンセル」ボタン
    private void Button_Click_3(object sender, RoutedEventArgs e)
    {
        navigateActionToMainPage.Invoke(null);
    }

    private void DeleteAll()
    {
        foreach (var v in Windows.Storage.ApplicationData.Current.LocalSettings.Values)
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(v.Key);
        }
    }

    private string? GetKeyAt(int index)
    {
        int i = 0;
        foreach (var v in Windows.Storage.ApplicationData.Current.LocalSettings.Values.OrderBy(x => x.Key))
        {
            if (i == index)
            {
                return v.Key;
            }
            i++;
        }
        return null;
    }
}

