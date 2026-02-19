using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MDS.Reports
{
    public enum AddToSession_PurgeFromSession_No
    {
        No=0,
        AddToSession=1,
        PurgeFromSession=2
    }
    public class ReportGlobal
    {
        public static HttpResponse CreateDownloadFile(HttpServerUtility Server, Microsoft.Reporting.WebForms.ReportViewer ReportViewer1, HttpResponse Response, AddToSession_PurgeFromSession_No APN = AddToSession_PurgeFromSession_No.No, System.Web.SessionState.HttpSessionState Session=null)
        {
            try
            {
                string MimeType; string Encoding; string[] StreamIDs; Microsoft.Reporting.WebForms.Warning[] Warnings; string filenameextnesion;
                byte[] bytespdf;
                try
                {
                    bytespdf = ReportViewer1.LocalReport.Render("pdf", null, out MimeType, out Encoding, out filenameextnesion, out StreamIDs, out Warnings);
                }
                catch (Exception ex)
                {
                    return null;
                }
                if (APN == AddToSession_PurgeFromSession_No.No)
                {
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = MimeType;
                    Response.AddHeader("content-length", bytespdf.Length.ToString());

                    Response.BinaryWrite(bytespdf);
                    return Response;

                }
                else if (Session["bytespdf"] == null)
                {
                    Session["bytespdf"] = bytespdf;
                    return null;
                }
                else
                {
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = MimeType;
                    Response.AddHeader("content-length", bytespdf.Length.ToString());

                    byte[] Prevbytespdf = (byte[])Session["bytespdf"];
                    byte[] concatpdf = concatAndAddContent(new List<byte[]> { bytespdf, Prevbytespdf });
                    Response.BinaryWrite(concatpdf);
                    Session["bytespdf"] = null;
                    return Response;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static byte[] concatAndAddContent(List<byte[]> pdfByteContent)
        {

            using (var ms = new MemoryStream())
            {
                using (var doc = new Document())
                {
                    using (var copy = new PdfSmartCopy(doc, ms))
                    {
                        doc.Open();

                        //Loop through each byte array
                        foreach (var p in pdfByteContent)
                        {

                            //Create a PdfReader bound to that byte array
                            using (var reader = new PdfReader(p))
                            {

                                //Add the entire document instead of page-by-page
                                copy.AddDocument(reader);
                            }
                        }

                        doc.Close();
                    }
                }

                //Return just before disposing
                return ms.ToArray();
            }
        }

        public static string CreateDownloadExcelOLD(string filename, HttpServerUtility Server, Microsoft.Reporting.WebForms.ReportViewer ReportViewer1, HttpResponse Response)
        {
            try
            {
                string file = Server.MapPath("temppdfs");
                if (!System.IO.Directory.Exists(file))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(file);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                file = file + @"\" + filename;
                string MimeType; string Encoding; string[] StreamIDs; Microsoft.Reporting.WebForms.Warning[] Warnings; string filenameextnesion;
                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
                byte[] bytespdf = ReportViewer1.LocalReport.Render("Excel", null, out MimeType, out Encoding, out filenameextnesion, out StreamIDs, out Warnings);
                System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Create);
                fs.Write(bytespdf, 0, bytespdf.Length);
                fs.Close();
                fs.Dispose();
                return file;
            }
            catch (Exception ex)
            {
                //  data.LogError(ex, "Statements.aspx.cs.pdfToFolder");
                return "";
            }
        }

        public static void CreateDownloadExcel(HttpServerUtility Server, Microsoft.Reporting.WebForms.ReportViewer ReportViewer1, HttpResponse Response)
        {
            try
            {
                //string file = Server.MapPath("temppdfs");
                //if (!System.IO.Directory.Exists(file))
                //{
                //    try
                //    {
                //        System.IO.Directory.CreateDirectory(file);
                //    }
                //    catch (Exception ex)
                //    {
                //        throw ex;
                //    }
                //}
                //file = file + @"\" + filename;
                string MimeType; string Encoding; string[] StreamIDs; Microsoft.Reporting.WebForms.Warning[] Warnings; string filenameextnesion;
                //if (System.IO.File.Exists(file))
                //    System.IO.File.Delete(file);
                byte[] bytespdf = ReportViewer1.LocalReport.Render("Excel", null, out MimeType, out Encoding, out filenameextnesion, out StreamIDs, out Warnings);

                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = MimeType;
                Response.AddHeader("content-length", bytespdf.Length.ToString());
                Response.BinaryWrite(bytespdf);

                //System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Create);
                //fs.Write(bytespdf, 0, bytespdf.Length);
                //fs.Close();
                //fs.Dispose();
                //return file;
            }
            catch (Exception ex)
            {
                //  data.LogError(ex, "Statements.aspx.cs.pdfToFolder");
                return;
            }
        }

        static void openfile(string FilePath, HttpResponse Response)
        {
            Response.Buffer = true;
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();


            char dot = char.Parse(".");
            int dotspot = FilePath.LastIndexOf(dot);

            char slash = char.Parse(@"\");
            int slashspot = FilePath.LastIndexOf(slash);

            string filename = FilePath.Substring(slashspot + 1, (FilePath.Length - slashspot - 1)); ;
            Response.AddHeader("content-disposition", @"attachment;filename=""" + filename + @"""");

            string ext = FilePath.Substring(dotspot + 1, (FilePath.Length - dotspot - 1));
            switch (ext.ToLower())
            {
                case "pdf":
                    Response.ContentType = "application/pdf";
                    break;
                case "doc":
                    Response.ContentType = "application/msword";
                    break;
                case "docx":
                    Response.ContentType = "application/vnd.ms-word.document.12";
                    break;
                case "csv":
                    Response.ContentType = "application/vnd.ms-excel.12";
                    break;
                case "xls":
                    Response.ContentType = "application/vnd.ms-excel";
                    break;
                case "xlsx":
                    Response.ContentType = "application/vnd.ms-excel.12";
                    break;
                case "tif":
                    Response.ContentType = "application/tif";
                    break;
                default:
                    Response.ContentType = "image/jpeg";
                    break;
            }
            Response.WriteFile(FilePath);
            Response.End();
        }

    }
}