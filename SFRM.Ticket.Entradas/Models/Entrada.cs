using System;
using System.Drawing;

namespace SFRM.Ticket.Entradas.Models
{
    public class Entrada
    {
        public int TicketNro { get; set; }
        public string CodigoQr { get; set; }
        public DateTime Fecha { get; set; }        
        public string Evento { get; set; }
        public string Hora { get; set; }
        public string Jornada { get; set; }
        public string Temporada { get; set; }
        public string Zona { get; set; }
        public string Puerta { get; set; }
        public int Fila { get; set; }
        public int Asiento { get; set; }
        public decimal Precio { get; set; }
        public string Tipo { get; set; }
    }
}