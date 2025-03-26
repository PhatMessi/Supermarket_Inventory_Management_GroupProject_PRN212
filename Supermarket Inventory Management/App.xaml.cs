using System.Configuration;
using System.Data;
using System.Windows;

namespace Supermarket_Inventory_Management
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
