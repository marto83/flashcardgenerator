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

                GeneratePages(doc, words);
                MemoryStream stream = new MemoryStream();
                doc.Save(stream, closeStream: false);
                return File(stream, "application/pdf");
            }
        }

        

        private void GeneratePages(PdfDocument doc, string[] words)
        {
            int pairs = words.Length / 2 + words.Length % 2;
            for (int i = 0; i < pairs; i++)
            {
                var firstWord = words[i * 2];
                int secondWordIndex = i * 2 + 1;
                var secondWord = secondWordIndex < words.Length ? words[secondWordIndex] : null;
                CreatePage(doc, firstWord, secondWord);
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
            var number = 0;
            if (int.TryParse(word, out number))
            {
                var newRectX = number < 10 ? rect.Right - 120 : rect.Right - 220;
                var newTopLeft = new XPoint(newRectX, rect.Top);
                var newBottomRight = new XPoint(rect.Right, rect.Top + rect.Height / 2);
                XRect numberRect = new XRect(newTopLeft, newBottomRight);
                XRect iconsRect = new XRect(rect.Left, rect.Top, rect.Width * 2 / 5, rect.Height);
                int fontsize = 200;
                RenderWord(word, gfx, numberRect, fontsize);
                RenderIcons(gfx, iconsRect);
            }
            else
            {
                int fontsize = word.Length > 12 ? 80 : 100;
                fontsize = word.Length < 18 ? fontsize : 60;
                RenderWord(word, gfx, rect, fontsize);
            }
        }
        private void RenderIcons(XGraphics gfx, XRect iconsRect)
        {
            for (int i = 0; i < 3; i++)
            {
                XPoint position = new XPoint(20 + iconsRect.Left + iconsRect.Width * i / 3, iconsRect.Top + 20);
                DrawIcon(gfx, "airplane.png", position);
            }
        }

        void DrawIcon(XGraphics gfx, string iconName, XPoint position)
        {
            XImage image = XImage.FromFile(Server.MapPath("~/Content/icons/" + iconName));
            var state = gfx.Save();
            double width = image.PixelWidth * 72 / image.HorizontalResolution;
            double height = image.PixelHeight * 72 / image.HorizontalResolution;
            gfx.DrawImage(image, position.X, position.Y, 64, 64);
            gfx.Restore(state);
        }

        private static void RenderWord(string word, XGraphics gfx, XRect rect, int fontsize)
        {
            XFont font = new XFont("Garamond", fontsize);
            XBrush brush = XBrushes.Black;

            XStringFormat format = new XStringFormat();

            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Center;
            gfx.DrawString(word.ToLower(), font, brush, rect, format);
        }
    }
}
