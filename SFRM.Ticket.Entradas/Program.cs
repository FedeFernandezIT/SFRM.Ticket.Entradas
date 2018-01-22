using System;
using System.Windows.Forms;
using SFRM.Ticket.Entradas.Properties;

namespace SFRM.Ticket.Entradas
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var notifyIcon = new NotifyIcon();
            notifyIcon.ContextMenuStrip = GetSincronizadorMenuStrip();
            notifyIcon.Icon = Resources.IconTicket;
            notifyIcon.Visible = true;

            Application.ApplicationExit += (sender, @event) => notifyIcon.Visible = false;
            Application.Run(new TicketApplication());
        }

        private static ContextMenuStrip GetSincronizadorMenuStrip()
        {
            var cms = new ContextMenuStrip();
            cms.Items.Add("Salir", null, (sender, @event) => Application.Exit());
            return cms;
        }
    }
}