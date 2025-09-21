using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using System.Threading.Tasks;

namespace CourseHub.Core.Services.Storage.Utils
{
    internal class FileConverter
    {
        public const string EXTENSION_JPG = ".jpg";

        public async Task<Stream> ToJpg(IFormFile file)
        {
            if (file.ContentType == "image/jpeg")
                return file.OpenReadStream();

            using var imgStream = new MemoryStream();
            await file.CopyToAsync(imgStream);
            imgStream.Seek(0, SeekOrigin.Begin);

            using var image = await Image.LoadAsync(imgStream);

            // not closing the stream (let it be closed by ServerStorage)
            var jpgStream = new MemoryStream();
            await image.SaveAsJpegAsync(jpgStream);
            jpgStream.Position = 0;
            return jpgStream;
        }
    }
}
