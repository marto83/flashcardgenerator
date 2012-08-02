using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;

namespace FlashCardGenerator.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to the Snowdrop flashcard generator.";
            return View();
        }

        [HttpPost]
        public ActionResult Generate(string font, string wordLines)
        {
            using (PdfDocument doc = new PdfDocument())
            {
                doc.PageLayout = PdfPageLayout.SinglePage;
                doc.Info.Author = "Snowdrop";
                var words = wordLines.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                
                if(words.Length == 0)
                {
                    TempData["alert"] = "Please add some words.";
                    return Redirect("/");
                }

                int pairs = words.Length / 2 + words.Length % 2;
                for (int i = 0; i < pairs; i++)
                {
                    var firstWord = words[i * 2];
                    int secondWordIndex = i * 2 + 1;
                    var secondWord = secondWordIndex < words.Length ? words[secondWordIndex] : null;
                    CreatePage(doc, firstWord, secondWord);
                }
                MemoryStream stream = new MemoryStream();
                doc.Save(stream, closeStream: false);
                return File(stream, "application/pdf");

            }
        }

        private void CreatePage(PdfDocument doc, string firstWord, string secondWord)
        {
            var page = doc.AddPage();
            page.Size = PdfSharp.PageSize.A4;
            XRect topHalf = new XRect(20, 20, page.Width - 40, (page.Height / 2) - 30);
            XRect bottomHalf = new XRect(topHalf.TopLeft, topHalf.BottomRight);
            bottomHalf.Offset(0, page.Height / 2 - 15);

            using (XGraphics gfx = XGraphics.FromPdfPage(page))
            {
                if (string.IsNullOrWhiteSpace(firstWord) == false)
                {
                    DrawCorners(topHalf, gfx);
                    DrawWord(firstWord, gfx, topHalf);
                }

                if (string.IsNullOrWhiteSpace(secondWord) == false)
                {
                    DrawCorners(bottomHalf, gfx);
                    DrawWord(secondWord, gfx, bottomHalf);
                }
            }
        }

        private static void DrawCorners(XRect rect, XGraphics gfx)
        {
            var pen = new XPen(XColors.Black, 0.5);
            pen.DashStyle = XDashStyle.Dash;

            //top left
            gfx.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Top + 20);
            gfx.DrawLine(pen, rect.Left, rect.Top, rect.Left + 20, rect.Top);

            //top right
            gfx.DrawLine(pen, rect.Right - 20, rect.Top, rect.Right, rect.Top);
            gfx.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Top + 20);

            //bottom left
            gfx.DrawLine(pen, rect.Left, rect.Bottom, rect.Left + 20, rect.Bottom);
            gfx.DrawLine(pen, rect.Left, rect.Bottom, rect.Left, rect.Bottom - 20);

            //bottom right
            gfx.DrawLine(pen, rect.Right, rect.Bottom, rect.Right, rect.Bottom - 20);
            gfx.DrawLine(pen, rect.Right, rect.Bottom, rect.Right - 20, rect.Bottom);
        }


        private void DrawWord(string word, XGraphics gfx, XRect rect)
        {
            int fontsize = word.Length > 12 ? 80 : 100;
            fontsize = word.Length < 18 ? fontsize : 60;
            XFont font = new XFont("Garamond", fontsize);
            XBrush brush = XBrushes.Black;

            XStringFormat format = new XStringFormat();

            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Center;
            gfx.DrawString(word.ToLower(), font, brush, rect, format);

        }
    }
}
