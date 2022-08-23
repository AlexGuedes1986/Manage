using IronPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Helpers
{
    public static class PdfHelper
    {
        public static HtmlToPdf GeneratePdfRendererFormat()
        {
            var rendererFormat = new HtmlToPdf();
            rendererFormat.PrintOptions.PaperOrientation = PdfPrintOptions.PdfPaperOrientation.Portrait;
            rendererFormat.PrintOptions.SetCustomPaperSizeInInches(10.50, 12);
            rendererFormat.PrintOptions.MarginLeft = 0;
            rendererFormat.PrintOptions.MarginRight = 0;
            rendererFormat.PrintOptions.MarginTop = 0;
            rendererFormat.PrintOptions.MarginBottom = 0;
            return rendererFormat;
        }
    }
}
