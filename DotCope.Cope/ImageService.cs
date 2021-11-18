using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace DotCope.Cope
{
    public class ImageService
    {
        public async Task<Stream> GenerateImage(string resourcePath, string text)
        {
            Image image = Image.Load(Path.Combine(resourcePath, "cope.gif"));

            FontCollection coll = new FontCollection();
            FontFamily fam = coll.Install(Path.Combine(resourcePath, "font.ttf"));

            TextOptions options = new TextOptions()
            {
                ApplyKerning = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                WrapTextWidth = image.Width - (image.Width / 20)
            };

            Font draw = fam.CreateFont(20);
            FontRectangle textFont = TextMeasurer.Measure(text, new RendererOptions(draw));
            var lines = textFont.Width / options.WrapTextWidth;
            var oldHeight = image.Height;
            int newHeight = oldHeight + (int)(textFont.Height * lines);
            var boxHeight = newHeight - oldHeight;
            image.Mutate(x => {
                x.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Manual,
                    Size = new Size(image.Width, newHeight),
                    TargetRectangle = new Rectangle(0, (newHeight - oldHeight), image.Width, oldHeight)
                });
                x.Fill(Color.White, new RectangleF(0, 0, textFont.Width, boxHeight + (boxHeight / 10)));
                x.SetTextOptions(options);
                x.DrawText(text, draw, Color.Black, new PointF((image.Width / 20), 0));
            });

            Stream outStream = new MemoryStream();
            await image.SaveAsGifAsync(outStream);
            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
        }
    }
}