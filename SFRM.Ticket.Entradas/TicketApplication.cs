using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using SFRM.Ticket.Entradas.Services;

namespace SFRM.Ticket.Entradas
{
    public class TicketApplication : ApplicationContext
    {
        private readonly TicketService _ticketService = new TicketService();
        private System.Timers.Timer _ticketPrinterTimer = new System.Timers.Timer(1000);

        public TicketApplication()
        {
            _ticketService.InitializeConfiguration();
            InitializeTicketTimer();
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