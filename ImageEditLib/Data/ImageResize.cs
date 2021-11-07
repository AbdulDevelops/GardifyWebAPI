using ImageEditLib.Extensions;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageEditLib.Data
{
    public class ImageResize
    {
        public static async Task<string> ResizeAsync(IFormFile input, string filePath, int? width = null)
        {
            return await ResizeAsync(input.OpenReadStream(), filePath, width);
        }

        public static async Task<string> ResizeAsync(string fullFilePath, string newFilePath, int? width = null)
        {
            var stream = System.IO.File.OpenRead(fullFilePath);
            return await ResizeAsync(stream, newFilePath, width);
        }

        public static async Task<string> ResizeAsync(Stream input, string filePath, int? width = null)
        {
            IImageFormat format;
            FileStream outputStream;
            string fullFileName = "";

            using (var image = Image.Load(input, out format))
            {
                if (width != null)
                {
                    float heightFactor = (float)width / image.Width;
                    outputStream = File.Create(filePath);
                    image.Mutate(i => i.Resize((int)width, (int)(image.Height * (float)heightFactor)));
                }
                else
                {
                    outputStream = File.Create(filePath);
                    image.Mutate(c => c.Resize(image.Width / 2, image.Height / 2));
                }
                image.Save(outputStream, format);
                fullFileName = outputStream.Name;
            }
            await outputStream.FlushAsync();
            outputStream.Close();
            return fullFileName;
        }
    }
}
