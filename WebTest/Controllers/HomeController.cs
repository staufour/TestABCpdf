namespace WebTest.Controllers
{
    using System;
    using System.IO;
    using System.Web.Mvc;
    using WebSupergoo.ABCpdf11;

    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (!XSettings.InstallLicense("<license_key>"))
            {
                throw new Exception("License installation failed.");
            }

            byte[] pdfContent;

            using (var ms = new MemoryStream())
            {
                ExportHtmlToPdf(ms);
                pdfContent = ms.ToArray();
            }
            
            return File(pdfContent, "application/pdf", "test.pdf");
        }

        private void ExportHtmlToPdf(Stream outputStream)
        {
            var htmlPath = Server.MapPath("~/App_Data/template.html");
            var logoPath = Server.MapPath("~/App_Data/logo.png");

            var tempFolder = CreateTempFolder();
            var tempLogoPath = Path.Combine(tempFolder, "logo.png");
            System.IO.File.Copy(logoPath, tempLogoPath);

            var content = System.IO.File.ReadAllText(htmlPath).Replace("$image_path$", new Uri(tempLogoPath).ToString());

            using (var doc = new Doc())
            {
                doc.Page = doc.AddPage();

                var id = doc.AddImageHtml(content);

                while (true)
                {
                    if (!doc.Chainable(id))
                    {
                        break;
                    }

                    doc.Page = doc.AddPage();
                    id = doc.AddImageToChain(id);
                }

                for (int i = 1; i <= doc.PageCount; i++)
                {
                    doc.PageNumber = i;
                    doc.Flatten();
                }

                doc.Save(outputStream);
            }

            Directory.Delete(tempFolder, true);
        }

        private string CreateTempFolder()
        {
            var fullBaseTempDir = Path.Combine(Path.GetTempPath(), "DocToHtml Temporary Files");

            if (!Directory.Exists(fullBaseTempDir))
            {
                Directory.CreateDirectory(fullBaseTempDir);
            }

            var simpleFileDir = Path.GetRandomFileName();
            var fileDir = Path.Combine(fullBaseTempDir, simpleFileDir);

            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            return fileDir;
        }
    }
}