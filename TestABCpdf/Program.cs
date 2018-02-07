using System;
using System.IO;
using System.Reflection;
using WebSupergoo.ABCpdf10;

namespace TestABCpdf
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!XSettings.InstallLicense("<license_key>"))
            {
                throw new Exception("License installation failed.");
            }

            Console.WriteLine("1. Export PDF to HTML. (default)");
            Console.WriteLine("2. Export HTML to PDF.");
            var result = Console.ReadLine();

            switch (result)
            {
                case "2":
                    ExportHtmlToPdf();
                    break;
                default:
                    ExportPdfToHtml();
                    break;
            }

            Console.ReadLine();
        }

        private static void ExportHtmlToPdf()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var htmlPath = Path.Combine(currentPath, "template.html");
            var logoPath = Path.Combine(currentPath, "logo.png");
            var outputPath = Path.Combine(currentPath, "output\\result.pdf");
            var outputFolder = Path.GetDirectoryName(outputPath);

            var tempFolder = CreateTempFolder();
            var tempLogoPath = Path.Combine(tempFolder, "logo.png");
            File.Copy(logoPath, tempLogoPath);

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            var content = File.ReadAllText(htmlPath).Replace("$image_path$", new Uri(tempLogoPath).ToString());

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

                doc.Save(outputPath);
            }

            Console.WriteLine($"PDF path : {outputPath}");
            Directory.Delete(tempFolder, true);
        }

        private static void ExportPdfToHtml()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var pdfPath = Path.Combine(currentPath, "doc.pdf");
            var outputPath = Path.Combine(currentPath, "output\\result.html");
            var outputFolder = Path.GetDirectoryName(outputPath);

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            using (var doc = new Doc())
            {
                doc.Read(pdfPath);
                doc.SaveOptions.Folder = "subfolder";
                doc.Save(outputPath);
            }

            Console.WriteLine($"HTML path : {outputPath}");
        }

        private static string CreateTempFolder()
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
