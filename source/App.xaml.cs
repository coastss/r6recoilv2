using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;

namespace r6recoilv2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Process.GetCurrentProcess().Kill();
        }
    }

}
