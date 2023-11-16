using System;

using System.IO;
using System.Drawing.Printing;
 using Microsoft.Reporting.WebForms;

namespace MMS2
{
    public class modul_print_reports 
    {
                          private static FileStream m_streams;
        private static  PrintDocument printdoc;
        public static bool print_microsoft_report(ref LocalReport report, int page_width, int page_height,
                                      bool islandscap = false, string printer_name = "")
        {
            printdoc = new PrintDocument();
            if (printer_name != "")
            {
                printdoc.PrinterSettings.PrinterName = printer_name;
            }

            if (printdoc.PrinterSettings.IsValid == false)
            { throw new Exception("Cannot Find the specified printer"); }
            else
            {
                PaperSize ps = new PaperSize("Custom", page_width, page_height);
                printdoc.DefaultPageSettings.PaperSize = ps;
                printdoc.DefaultPageSettings.Landscape = islandscap;
                Export(report);
                                 printdoc.Print();

            }
            return true;
        }

        public static bool print_microsoft_report(ref LocalReport report, string paperkind = "A4",
                          bool islandscap = false, string printer_name = "")
        {
            printdoc = new PrintDocument();
            if (printer_name != "")
            {
                printdoc.PrinterSettings.PrinterName = printer_name;
            }

            if (printdoc.PrinterSettings.IsValid == false)
            { throw new Exception("Cannot Find the specified printer"); }
            else
            {
                PaperSize ps;
                bool pagekind_found = false;
                for (int i = 0; i < printdoc.PrinterSettings.PaperSizes.Count - 1; i++)
                {
                    if (printdoc.PrinterSettings.PaperSizes[i].Kind.ToString() == paperkind)
                    {
                        ps = printdoc.PrinterSettings.PaperSizes[i];
                        printdoc.DefaultPageSettings.PaperSize = ps;
                        pagekind_found = true;
                    }
                    if (pagekind_found == false)
                    { throw new Exception("Paper size is invalid"); }
                    else
                    {
                        printdoc.DefaultPageSettings.Landscape = islandscap;
                        Export(report);
                                                 printdoc.Print();

                    }


                }

            }
            return true;
        }


                                                                        
                                                                                          
                           
                                                                                                                                                
                                                                                                                                                                                                      
         

         
                           
                                                               
         
                 private static void Export(LocalReport report)
        {
            int w;
            int h;
            if (printdoc.DefaultPageSettings.Landscape == true)
            {
                w = printdoc.DefaultPageSettings.PaperSize.Height;
                h = printdoc.DefaultPageSettings.PaperSize.Width;
            }
            else
            {
                w = printdoc.DefaultPageSettings.PaperSize.Width;
                h = printdoc.DefaultPageSettings.PaperSize.Height;
            }
            string deviceInfo = "<DeviceInfo>" +
            "<OutputFormat>EMF</OutputFormat>" +
            "<PageWidth>" + w / 100 + "in</PageWidth>" +
            "<PageHeight>" + h / 100 + "in</PageHeight>" +
            "<MarginTop>0.0in</MarginTop>" +
            "<MarginLeft>0.0in</MarginLeft>" +
            "<MarginRight>0.0in</MarginRight>" +
            "<MarginBottom>0.0in</MarginBottom>" +
            "</DeviceInfo>";
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;


            byte[] bytes = report.Render(
                "Image", deviceInfo, out mimeType, out encoding, out filenameExtension,
                out streamids, out warnings);

            using (m_streams = new FileStream("output.pdf", FileMode.Create))
            {
                m_streams.Write(bytes, 0, bytes.Length);
            }

                                                                                           
                                                                              
                                       
                                                    
        }

       
    }
}
