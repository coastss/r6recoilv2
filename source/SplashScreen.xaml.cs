
using r6recoilv2.Utilities;
using System.Diagnostics;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;

namespace r6recoilv2
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public static string R6RVersion = DateTime.Now.ToString("MM/dd/yyyy");
        public SplashScreen()
        {
            InitializeComponent();
            this.Title = Helper.RandomString(16);
            
            DoSplashScreen();
        }

        private async void DoSplashScreen()
        {
            bool DisableSplash = false;

            if (!DisableSplash)
            {
                SplashScreenProgressBar.IsIndeterminate = true;

                // Check for any updates
                await Task.Delay(1000);
                SplashScreenProgressBar.IsIndeterminate = false;
                SplashScreenProgressBar.Value = 50;

                SplashScreenStatus.Content = "Checking for updates...";

                // github checka
                HttpClient _HttpClient = new HttpClient();
                var VersionRequest = await _HttpClient.GetAsync("https://raw.githubusercontent.com/coastss/r6recoilv2/refs/heads/main/ver.sion");
                string Output = await VersionRequest.Content.ReadAsStringAsync();
                Output = Regex.Replace(Output, @"\t|\n|\r", ""); // ooo im the http request ghost and i like to add random fucking \n lines

                if (R6RVersion != Output)
                {
                    if (MessageBox.Show("A newer version of r6recoilv2 is available! Press 'Yes' to be taken to the GitHub page.", "r6recoilv2", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                       Helper.OpenURL("https://github.com/coastss/r6recoilv2");
                       Close();
                    }
                };
                // aww its over

                SplashScreenProgressBar.Value = 100;
                SplashScreenStatus.Content = ("Up to date! (" + R6RVersion + ")");

                // Close SplashScreen
                // Open MainWindow
                await Task.Delay(1000);
            }

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

    }
}
