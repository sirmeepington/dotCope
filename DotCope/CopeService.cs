using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Net.Http;
using System.Text;

namespace DotCope
{
    public class CopeService
    {
        private readonly Random _random = new Random();
        private readonly IWebHostEnvironment webHostEnvironment;

        private static string[] allWords;

        public CopeService(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<Stream> CreateRandomCope()
        {
            if (allWords == null)
            {
                await GatherWords();
            }

            IEnumerable<string> words = await GatherRandomAdjectives(_random.Next(5,15));
            StringBuilder sb = new StringBuilder();

            sb.Append("cope + seethe + ");
            foreach(var word in words)
            {
                sb.Append(word);
                sb.Append(" + ");
            }
            sb.Length -= 3;

            return await GenerateImage(sb.ToString());
        }

        private async Task<Stream> GenerateImage(string text)
        {
            Image image = Image.Load(Path.Combine(webHostEnvironment.WebRootPath, "cope.gif"));

            FontCollection coll = new FontCollection();
            FontFamily fam = coll.Install(Path.Combine(webHostEnvironment.WebRootPath, "font.ttf"));

            TextOptions options = new TextOptions()
            {
                ApplyKerning = true,
                HorizontalAlignment = HorizontalAlignment. Center,
                WrapTextWidth = image.Width - (image.Width/20)                
            };

            Font draw = fam.CreateFont(20);
            FontRectangle textFont = TextMeasurer.Measure(text, new RendererOptions(draw));
            int lines = (int)Math.Ceiling(textFont.Width / options.WrapTextWidth);
            var oldHeight = image.Height;
            int newHeight = oldHeight + (int)(textFont.Height * lines);
            var boxHeight = newHeight - oldHeight;
            image.Mutate(x => {
                x.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Manual,
                    Size = new Size(image.Width, newHeight),
                    TargetRectangle = new Rectangle(0, (newHeight-oldHeight), image.Width, oldHeight)
                });
                x.Fill(Color.White, new RectangleF(0, 0, textFont.Width, boxHeight + (boxHeight/10)));
                x.SetTextOptions(options);
                x.DrawText(text, draw, Color.Black, new PointF((image.Width/20),0));
            });

            Stream outStream = new MemoryStream();
            await image.SaveAsGifAsync(outStream);
            outStream.Seek(0, SeekOrigin.Begin);
            return outStream;
        }

        private Task<IEnumerable<string>> GatherRandomAdjectives(int amount) 
        {
            List<string> words = new List<string>();
            for(int i = 0; i < amount; i++)
            {
                words.Add(allWords[_random.Next(allWords.Length)]);
            } 
            return Task.FromResult(words.AsEnumerable());
        }

        private async Task GatherWords()
        {
            var wordFile = File.OpenText(Path.Combine(webHostEnvironment.WebRootPath, "words.txt"));
            var inStr = await wordFile.ReadToEndAsync();
            string[] all = inStr.Split("\n");
            allWords = all;
        }
    }
}
