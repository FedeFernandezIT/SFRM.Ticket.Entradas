using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SFRM.Ticket.Entradas.Helpers;
using SFRM.Ticket.Entradas.Models;

namespace SFRM.Ticket.Entradas.Services
{    
    public class TicketService
    {
        public string TicketTerminal { get; set; }
        public string TicketServer { get; set; }
        public string TicketDatabase { get; set; }

        public void InitializeConfiguration()
        {
            try
            {
                var dir = ConfigurationManager.AppSettings["Directory.Setup"];

                var pathTicketTerminal = Path.Combine(dir, ConfigurationManager.AppSettings["Ticket.Terminal"]);
                TicketTerminal = new StreamReader(pathTicketTerminal).ReadLine();

                var pathTicketServer = Path.Combine(dir, ConfigurationManager.AppSettings["Ticket.Server"]);
                TicketServer = new StreamReader(pathTicketServer).ReadLine();

                var pathTicketDatabase = Path.Combine(dir, ConfigurationManager.AppSettings["Ticket.Database"]);
                TicketDatabase = new StreamReader(pathTicketDatabase).ReadLine();
            }
            catch (IOException ex)
            {
                throw new IOException("Error al leer archivos de configuración");
            }
        }
        

        public void PrintInside()
        {
            try
            {
                using (var db = new TicketContext(TicketServer, TicketDatabase))
                {
                    List<Entrada> entradas;
                    try
                    {
                        var sql =
                            @"SELECT t.tickets_id AS TicketNro, s.season_name AS Temporada, e.event_name AS Evento, e.event_date AS Fecha, e.event_jornada AS Jornada, e.event_hora AS Hora, z.zone_name AS Zona, z.zone_puerta AS Puerta, t.ticket_row AS Fila, t.ticket_seat_id AS Asiento, t.ticket_price AS Precio, tt.ticket_type_name AS Tipo, t.ticket_qr AS CodigoQr FROM tickets AS t " +
                            @"INNER JOIN `events` AS e ON e.event_id = t.ticket_event_id " +
                            @"INNER JOIN seasons AS s ON s.season_id = e.event_season_id " +
                            @"INNER JOIN zones AS z ON z.zone_id = t.ticket_zone_id " +
                            @"INNER JOIN tickets_types AS tt ON tt.ticket_type_id = t.ticket_type_id " +
                            @"WHERE t.ticket_state = 1 AND t.terminal = @terminal";
                        entradas = db.Database.SqlQuery<Entrada>(sql,
                                new MySqlParameter("terminal", TicketTerminal))
                            .ToList();
                        if (entradas.Count > 0)
                        {
                            foreach (var entrada in entradas)
                            {

                                try
                                {
                                    entrada.CodigoQr = new Uri(entrada.CodigoQr).AbsoluteUri;
                                    Printer printer = new Printer();
                                    printer.Init(entrada);

                                    var sqlUpdate = @"UPDATE tickets SET  ticket_state = 2 WHERE tickets_id = @ticket";
                                    db.Database.ExecuteSqlCommand(sqlUpdate,
                                        new MySqlParameter("ticket", entrada.TicketNro));
                                }
                                catch (Exception)
                                {
                                    // Nathing
                                    //MessageBox.Show("Level3");
                                }
                            }                            
                        }                        
                    }
                    catch(Exception)
                    {
                        // Nothing
                        //MessageBox.Show("Level2");
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("Level1");
                Task.Delay(500).Wait();
            }            
        }

    }    
}