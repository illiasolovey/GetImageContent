using SixLabors.ImageSharp.Formats;

namespace ObjectAnalysis.Utils
{
    public static class ImageFormat
    {
        public static IImageFormat GetObjectImageFormat(string contentType)
        {
            switch (contentType)
            {
                case "image/jpg":
                case "image/jpeg":
                    return SixLabors.ImageSharp.Formats.Jpeg.JpegFormat.Instance;
                case "image/png":
                    return SixLabors.ImageSharp.Formats.Png.PngFormat.Instance;
                case "image/gif":
                    return SixLabors.ImageSharp.Formats.Gif.GifFormat.Instance;
                case "image/bmp":
                    return SixLabors.ImageSharp.Formats.Bmp.BmpFormat.Instance;
                default:
                    throw new ArgumentException($"{contentType} is not supported.");
            }
        }
    }
}