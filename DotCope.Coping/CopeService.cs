using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing.Processing.Processors.Text;
using SixLabors.ImageSharp.Processing;
using System.Text;

namespace DotCope.Coping
{
    public class CopeService
    {
        private Random _random = new Random();
        private readonly string webFilePath;
        private readonly Image copeImage;
        private readonly TextOptions options;

        private readonly string[] allWords;

        public CopeService(string webFilePath)
        {
            this.webFilePath = webFilePath;
            allWords = GatherWords().GetAwaiter().GetResult();

            copeImage = Image.Load(Path.Combine(webFilePath, "cope.gif"));
            FontFamily fontFamily = new FontCollection().Add(Path.Combine(webFilePath, "font.ttf"));

            options = new TextOptions(fontFamily.CreateFont(20))
            {
                KerningMode = KerningMode.Normal,
                HorizontalAlignment = HorizontalAlignment.Center,
                WrappingLength = copeImage.Width - (copeImage.Width / 20),
                Origin = new PointF((copeImage.Width / 2), 0)
            };
        }

        public async Task<Stream> CreateRandomCope(int? seed)
        {
            if (seed != null) {
                _random = new Random(seed.Value);
            }
            IEnumerable<string> words = await GatherRandomAdjectives(_random.Next(5,15));
            StringBuilder sb = new StringBuilder("cope + seethe + ");
            foreach(var word in words)
            {
                sb.Append(word.Replace("\r",""));
                sb.Append(" + ");
            }
            sb.Length -= 3;
            
            return await GenerateImage(sb.ToString());
        }

        private async Task<Stream> GenerateImage(string text)
        {
            FontRectangle textFont = TextMeasurer.Measure(text, options);
            int lines = (int)Math.Ceiling(textFont.Width / options.WrappingLength);
            var oldHeight = copeImage.Height;
            int newHeight = oldHeight + (int)(textFont.Height * lines);
            var boxHeight = newHeight - oldHeight;
            Image outImage = copeImage.Clone(x => {
                x.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Manual,
                    Size = new Size(copeImage.Width, newHeight),
                    TargetRectangle = new Rectangle(0, (newHeight - oldHeight), copeImage.Width, oldHeight)
                })
                .Fill(Color.White, new RectangleF(0, 0, copeImage.Width, boxHeight + (boxHeight / 10)))
                .DrawText(options, text, Color.Black);
            });
            Stream outStream = new MemoryStream();
            await outImage.SaveAsGifAsync(outStream);
            outImage.Dispose();
            outStream.Seek(0, SeekOrigin.Begin);
            GC.Collect();
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

        private async Task<string[]> GatherWords()
        {
            var wordFile = File.OpenText(Path.Combine(webFilePath, "words.txt"));
            var inStr = await wordFile.ReadToEndAsync();
            string[] all = inStr.Split("\n");
            return all;
        }
    }
}
