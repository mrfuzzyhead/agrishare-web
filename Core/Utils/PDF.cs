using Agrishare.Core.Entities;
using ExpertPdf.HtmlToPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Agrishare.Core.Utils
{
    /// <summary>
    /// Generate PDF documents from a URL or HTML string
    /// </summary>
    public class PDF
    {
        /// <summary>
        /// Convert a URL to JPG
        /// </summary>
        /// <param name="Url">The URL of the page to convert</param>
        /// <returns>JPEG bytes</returns>
        public static byte[] ConvertUrlToJpg(string Url)
        {
            // Create converter
            var imgConverter = new ImgConverter
            {
                LicenseKey = Config.Find(Key: "PDF License Key").Value
            };

            // Get HTML
            StringWriter sw = new StringWriter();
            HttpContext.Current.Server.Execute(Url, sw);
            String htmlCodeToConvert = sw.GetStringBuilder().ToString();
            
            // Get Base URL
            Uri originalUri = new Uri(Url);
            String baseUrl = String.Format("{0}{1}", originalUri.Scheme, originalUri.Host);

            // Return bytes
            return imgConverter.GetImageBytesFromHtmlString(htmlCodeToConvert, System.Drawing.Imaging.ImageFormat.Jpeg, baseUrl);
        }

        /// <summary>
        /// Convert a URL to PDF
        /// </summary>
        /// <param name="Url">The URL of the page to convert</param>
        /// <returns>PDF bytes</returns>
        public static byte[] ConvertUrlToPdf(string Url)
        {
            // Create converter
            var pdfConverter = new PdfConverter
            {
                LicenseKey = Config.Find(Key: "PDF License Key").Value
            };

            // Return bytes
            return pdfConverter.GetPdfFromUrlBytes(Url);
        }

        /// <summary>
        /// Convert HTML to PDF with a fixed header and footer
        /// </summary>
        /// <param name="HeaderHtml">HTML source for the header</param>
        /// <param name="HeaderHeight">Height of the header section</param>
        /// <param name="ContentHtml">HTML source for the body of the PDF</param>
        /// <param name="FooterHtml">HTML source for the footer</param>
        /// <param name="FooterHeight">Height of the footer section</param>
        /// <param name="BaseURL">The Base URL for images in the source</param>
        /// <returns>PDF bytes</returns>
        public static byte[] ConvertHtmlToPdf(string ContentHtml, string BaseURL, string HeaderHtml = "", int HeaderHeight = 0, string FooterHtml = "", int FooterHeight = 0, decimal Width = 0, decimal Height = 0, bool ShowPageNumbers = false)
        {
            // Create converter
            var pdfConverter = new PdfConverter
            {
                LicenseKey = Config.Find(Key: "PDF License Key").Value
            };

            // Header
            if (!string.IsNullOrEmpty(HeaderHtml))
            {
                pdfConverter.PdfDocumentOptions.ShowHeader = true;
                pdfConverter.PdfHeaderOptions.DrawHeaderLine = false;
                pdfConverter.PdfHeaderOptions.HeaderHeight = HeaderHeight;
                pdfConverter.PdfHeaderOptions.HtmlToPdfArea = new HtmlToPdfArea(HeaderHtml, BaseURL);
            }

            // Footer
            if (!string.IsNullOrEmpty(FooterHtml))
            {
                pdfConverter.PdfDocumentOptions.ShowFooter = true;
                pdfConverter.PdfFooterOptions.DrawFooterLine = false;                
                pdfConverter.PdfFooterOptions.FooterHeight = FooterHeight;
                pdfConverter.PdfFooterOptions.HtmlToPdfArea = new HtmlToPdfArea(FooterHtml, BaseURL);
            }

            if (ShowPageNumbers)
            {
                // DO NOT DELETE the spaces - this fakes the margin
                pdfConverter.PdfFooterOptions.TextArea = new TextArea(0, 19, "Page &p;/&P;                 ", 
                    new System.Drawing.Font(new System.Drawing.FontFamily("Arial"), 6, System.Drawing.GraphicsUnit.Point))
                    {
                        EmbedTextFont = true,
                        TextAlign = HorizontalTextAlign.Right
                    };
            }

            pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;

            if (Width == 0 && Height == 0)
            {
                pdfConverter.PdfDocumentOptions.FitWidth = true;
                pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
            }
            else
            {
                pdfConverter.PageWidth = 0;
                pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Custom;
                pdfConverter.PdfDocumentOptions.CustomPdfPageSize = new System.Drawing.SizeF
                {
                    Height = (float)Height,
                    Width = (float)Width
                };
            }

            if (string.IsNullOrEmpty(ContentHtml))
                ContentHtml = "There was nothing to print!";

            // Document
            return pdfConverter.GetPdfBytesFromHtmlString(ContentHtml, BaseURL);
        }
    }
}
