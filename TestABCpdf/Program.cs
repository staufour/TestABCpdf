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
                doc.SetInfo(0, "MSHtmlBootstrap", 0);

                doc.Read(pdfPath);
                doc.SaveOptions.Folder = "subfolder";
                doc.Save(outputPath);
            }
        }
    }
}
