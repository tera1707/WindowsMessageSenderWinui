using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;

namespace WindowMessageSender
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var actEventArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            
            if (actEventArgs.Kind == ExtendedActivationKind.Protocol)
            {
                var d = actEventArgs.Data as IProtocolActivatedEventArgs;
                if (d != null)
                {
                    var uri = d.Uri;
                    var uriString = uri.AbsoluteUri;

                    Debug.WriteLine(uri);
                    Debug.WriteLine(uriString);
                }
            }

            m_window = new MainWindow();
            m_window.Activate();
        }

        private Window m_window;
    }
}
