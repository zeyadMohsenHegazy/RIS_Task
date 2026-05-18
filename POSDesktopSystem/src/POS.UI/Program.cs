using Microsoft.Extensions.DependencyInjection;
using POS.UI.Helpers;

namespace POS.UI;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        GlobalExceptionHandler.Register();

        var serviceProvider = DependencyInjection.CompositionRoot.Build();

        while (true)
        {
            using (var loginScope = serviceProvider.CreateScope())
            {
                var loginForm = loginScope.ServiceProvider.GetRequiredService<Forms.LoginForm>();
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    break;
                }
            }

            DialogResult dashboardResult;
            using (var dashboardScope = serviceProvider.CreateScope())
            {
                var dashboard = dashboardScope.ServiceProvider.GetRequiredService<Forms.DashboardForm>();
                dashboardResult = dashboard.ShowDialog();
            }

            if (dashboardResult != DialogResult.Retry)
            {
                break;
            }
        }
    }
}
