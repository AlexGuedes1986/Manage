using BioTech.Models;
using BioTech.ViewModels;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Helpers
{
    public static class BiotechCsvHelper
    {
        public static byte[] WriteCsvToMemory(IEnumerable<InvoiceCsv> invoicesFormatted)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(invoicesFormatted);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
        public static List<InvoiceCsv> FormatInvoiceDataToCSV(IEnumerable<InvoiceProject> invoices)
        {
            List<InvoiceCsv> invoicesFormatted = new List<InvoiceCsv>();
            foreach (var invoice in invoices)
            {
                invoicesFormatted.Add(new InvoiceCsv
                {
                    ProjectNumber = invoice.ProjectNumber,
                    ContractNumber = invoice.ContractNumber,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    CurrentCharges = invoice.CurrentCharges,
                    Terms = "Net 30"
                });
            }
            return invoicesFormatted;
        }
    }
}
