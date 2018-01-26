using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using SFRM.Ticket.Entradas.Properties;

namespace SFRM.Ticket.Entradas
{
    static class Program
    {
        private static Mutex instanceMutex;

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                bool createdNew;
                instanceMutex = new Mutex(true, @"Local\" + Assembly.GetExecutingAssembly().GetType().GUID, out createdNew);
                if (!createdNew)
                {
                    instanceMutex = null;
                    return;
                }
                var notifyIcon = new NotifyIcon();
                notifyIcon.ContextMenuStrip = GetSincronizadorMenuStrip();
                notifyIcon.Icon = Resources.IconTicket;
                notifyIcon.Visible = true;

                Application.ApplicationExit += (sender, @event) => notifyIcon.Visible = false;
                Application.Run(new TicketApplication());
                instanceMutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);                
            }
        }

        private static ContextMenuStrip GetSincronizadorMenuStrip()
        {
            var cms = new ContextMenuStrip();
            cms.Items.Add("Salir", null, (sender, @event) => Application.Exit());
            return cms;
        }
    }
}