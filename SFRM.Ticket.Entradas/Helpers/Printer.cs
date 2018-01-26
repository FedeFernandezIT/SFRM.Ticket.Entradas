using System.Drawing;
using SFRM.Ticket.Entradas.Models;
using SFRM.Ticket.Entradas.Properties;

namespace SFRM.Ticket.Entradas.Helpers
{
    using System;
    using System.IO;
    using System.Data;
    using System.Text;
    using System.Drawing.Imaging;
    using System.Drawing.Printing;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Microsoft.Reporting.WinForms;

    public class Printer : IDisposable
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;

        // Routine to provide to the report renderer, in order to
        //    save an image for each page of the report.
        private Stream CreateStream(string name,
          string fileNameExtension, Encoding encoding,
          string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        // Export the given report as an EMF (Enhanced Metafile) file.
        private void Export(LocalReport report)
        {
            string deviceInfo =
              @"<DeviceInfo>
                <OutputFormat>EMF</OutputFormat>
                <PageWidth>5.98425</PageWidth>
                <PageHeight>2.75591</PageHeight>
                <MarginTop>0.00in</MarginTop>
                <MarginLeft>0.00in</MarginLeft>
                <MarginRight>0.00in</MarginRight>
                <MarginBottom>0.00in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            try
            {
                report.Render("Image", deviceInfo, CreateStream,
                    out warnings);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error in Render: " + ex.InnerException?.Message ?? ex.Message);
            }
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }
        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);
            ev.Graphics.PageUnit = GraphicsUnit.Millimeter;
            

            Rectangle adjustedRect = new Rectangle(-152, 0, 152, 70);                

            ev.Graphics.RotateTransform(270);            

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);            

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        private void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                //MessageBox.Show("Error: cannot find the default printer.");
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }
        // Create a local report for Report.rdlc, load the data,
        //    export the report to an .emf file, and print it.
        public void Init(Entrada data)
        {            
            LocalReport report = new LocalReport();            
            TextReader reader = new StringReader(Resources.TicketEntrada);
            report.LoadReportDefinition(reader);
            IEnumerable<Entrada> info = new List<Entrada> {data};
            report.DataSources.Add(
               new ReportDataSource("DataSet1", info));
            report.EnableExternalImages = true;
            Export(report);
            Print();
        }

        public void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }        
    }

}