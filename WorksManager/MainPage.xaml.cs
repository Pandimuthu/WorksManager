using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.AppService;
using Windows.UI.Core;
using Windows.Foundation.Collections;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WorksManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            {
                App.AppServiceConnected += MainPage_AppServiceConnected;
                App.AppServiceDisconnected += MainPage_AppServiceDisconnected;
                await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if (ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0))
            //{
            //    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            //}
            //Frame.Navigate(typeof(StartPage), null);
        }

        private async void MainPage_AppServiceConnected(object sender, AppServiceTriggerDetails e)
        {
            App.Connection.RequestReceived += AppServiceConnection_RequestReceived;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // enable UI to access  the connection
                //btnRegKey.IsEnabled = true;
            });
        }

        private async void MainPage_AppServiceDisconnected(object sender, EventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // disable UI to access the connection
                //btnRegKey.IsEnabled = false;

                // ask user if they want to reconnect
                Reconnect();
            });
        }

        private async void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            double d1 = (double)args.Request.Message["D1"];
            double d2 = (double)args.Request.Message["D2"];
            double result = d1 + d2;

            ValueSet response = new ValueSet();
            response.Add("RESULT", result);
            await args.Request.SendResponseAsync(response);

            // log the request in the UI for demo purposes
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //tbRequests.Text += string.Format("Request: {0} + {1} --> Response = {2}\r\n", d1, d2, result);
            });
        }

        /// <summary>
        /// Ask user if they want to reconnect to the desktop process
        /// </summary>
        private async void Reconnect()
        {
            //if (App.IsForeground)
            //{
            //    MessageDialog dlg = new MessageDialog("Connection to desktop process lost. Reconnect?");
            //    UICommand yesCommand = new UICommand("Yes", async (r) =>
            //    {
            //        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            //    });
            //    dlg.Commands.Add(yesCommand);
            //    UICommand noCommand = new UICommand("No", (r) => { });
            //    dlg.Commands.Add(noCommand);
            //    await dlg.ShowAsync();
            //}
        }

        private async void Test_Click(object sender, RoutedEventArgs e)
        {
            ValueSet request = new ValueSet();
            request.Add("KEY", "Muthu");
            AppServiceResponse response = await App.Connection.SendMessageAsync(request);
        }
    }
}
