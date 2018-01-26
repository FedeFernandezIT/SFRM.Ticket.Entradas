using System.Deployment.Application;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;
using SFRM.Ticket.ClickOnce;
using SFRM.Ticket.Entradas.Services;

namespace SFRM.Ticket.Entradas
{
    public class TicketApplication : ApplicationContext
    {
        private readonly TicketService _ticketService = new TicketService();
        private System.Timers.Timer _ticketPrinterTimer = new System.Timers.Timer(1000);

        public TicketApplication()
        {
            var clickOnce = new ClickOnceHelper(Globals.PublisherName, Globals.ProductName);
            clickOnce.UpdateUninstallParameters();
            RegisterStartup(clickOnce.ProductName);
            _ticketService.InitializeConfiguration();
            InitializeTicketTimer();
        }

        public void RegisterStartup(string productName)
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
                return;
            RegistryKey reg = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            reg.SetValue(productName, Assembly.GetExecutingAssembly().Location);
        }


        private void InitializeTicketTimer()
        {            
            _ticketPrinterTimer = new System.Timers.Timer(500);
            //_ticketPrinterTimer.AutoReset = false;
            _ticketPrinterTimer.Elapsed += (o, e) =>
            {
                _ticketPrinterTimer.Stop();
                _ticketService.PrintInside();
                _ticketPrinterTimer.Start();
            };
            _ticketPrinterTimer.Start();
        }
    }
}