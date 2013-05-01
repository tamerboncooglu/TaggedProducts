namespace TaggedProducts.Utils
{
    using System.Web;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    public static class FileHelper
    {
        public static bool SaveImageAs250(HttpPostedFileBase file, string imagePath)
        {
            if (file != null &&
                file.ContentLength > 100 &&
                file.ContentLength < 10048576) // 10mb
            {
                try
                {
                    using (var image = Image.FromStream(file.InputStream))
                    {
                        var newHeight = (int)(image.Height / ((double)image.Width / 250));
                        var thumbnailImg = new Bitmap(250, newHeight);
                        var thumbGraph = Graphics.FromImage(thumbnailImg);
                        thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                        thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                        thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        var imageRectangle = new Rectangle(0, 0, 250, newHeight);
                        thumbGraph.DrawImage(image, imageRectangle);
                        thumbnailImg.Save(imagePath, image.RawFormat);
                    }
                }
                catch { }
            }

            return false;
        }

        public static bool SaveImageAs960(HttpPostedFileBase file, string imagePath)
        {
            if (file != null &&
                file.ContentLength > 100 &&
                file.ContentLength < 10048576) // 10mb
            {
                try
                {
                    using (var image = Image.FromStream(file.InputStream))
                    {
                        var newHeight = (int)(image.Height / ((double)image.Width / 960));
                        var thumbnailImg = new Bitmap(960, newHeight);
                        var thumbGraph = Graphics.FromImage(thumbnailImg);
                        thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                        thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                        thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        var imageRectangle = new Rectangle(0, 0, 960, newHeight);
                        thumbGraph.DrawImage(image, imageRectangle);
                        thumbnailImg.Save(imagePath, image.RawFormat);
                    }
                }
                catch { }
            }

            return false;
        }
    }
}
