using System;
using Gdk;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

namespace ToonBoomExportGUI
{
	public class ImageCropper
	{
		public ImageCropper ()
		{

		}


		public void CropImage (string imagePath, int x1, int y1, int x2, int y2)
		{

			int extIndex = imagePath.LastIndexOf (".", StringComparison.CurrentCulture) + 1;
			string ext = imagePath.Substring (extIndex, imagePath.Length - extIndex);

			Console.WriteLine ("{0} type is {1}", imagePath, ext);
			int width = x2 - x1;
			int height = y2 - y1;

			Console.WriteLine ("{0}x{1}", width, height);

			var buffer = System.IO.File.ReadAllBytes (imagePath);
			var pixbuf = new Gdk.Pixbuf (buffer);
			buffer = System.IO.File.ReadAllBytes (imagePath);
			var cropped = new Pixbuf (pixbuf, x1, y1, width, height);

			//pixbuf.CopyArea (x1, y1, width, height, cropped, 0, 0);

			
			cropped.Save (imagePath, ext);

		}

        public void CropPdf(string pdfPath, float u1, float v1, float u2, float v2)
        {
            PdfDocument doc = PdfReader.Open(pdfPath);
            PdfPage page = doc.Pages[0];
            page.CropBox = new PdfRectangle(new XPoint(u1 * page.Width, v1*page.Height), new XPoint(u2*page.Width, v2*page.Height));
            doc.Save(pdfPath);

            Console.WriteLine("UV1 {0}, {1}\nUV2 {2}, {3}", u1, v1, u2, v2);
            Console.WriteLine("CB1 {0}, {1}\nCB2 {2}, {3}", u1 * page.Width, v1 * page.Height, u2 * page.Width, v2 * page.Height);
        }

		public void Composite (string toppath, string bottompath, string outputPath)
		{
			int extIndex = outputPath.LastIndexOf (".", StringComparison.CurrentCulture) + 1;
			string ext = outputPath.Substring (extIndex, outputPath.Length - extIndex);

			var buffer = System.IO.File.ReadAllBytes (toppath);
			var top = new Gdk.Pixbuf (buffer);
			buffer = System.IO.File.ReadAllBytes (bottompath);
			var bottom = new Gdk.Pixbuf (buffer);

			top.Composite (bottom, 0, 0, top.Width, top.Height, 0, 0, 1, 1, InterpType.Nearest,255);
			bottom.Save (outputPath, ext);
		}

	}
}

