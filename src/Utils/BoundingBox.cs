using Amazon.Rekognition.Model;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace ObjectAnalysis.Utils
{
    public static class BoundingBox
    {
        public static void Draw(SixLabors.ImageSharp.Image image, DetectLabelsResponse detectResponse, string boundingBoxHex, string labelHex)
        {
            boundingBoxHex ??= "#ff0000";
            labelHex ??= "#ffffff";

            foreach (var label in detectResponse.Labels)
            {
                foreach (var instance in label.Instances)
                {
                    var bound = instance.BoundingBox;
                    int x = (int)(image.Width * bound.Left);
                    int y = (int)(image.Height * bound.Top);
                    int width = (int)(image.Width * bound.Width);
                    int height = (int)(image.Height * bound.Height);
                    var rectangle = new Rectangle(x, y, width, height);
                    var points = new PointF(x, y);
                    var labelColor = Rgba32.ParseHex(labelHex.Replace("#", string.Empty));
                    var boundingBoxColor = Rgba32.ParseHex(boundingBoxHex.Replace("#", string.Empty));
                    DrawBoundingRectangle(image, label, rectangle, points, boundingBoxColor, labelColor);
                }
            }
        }

        private static void DrawBoundingRectangle(SixLabors.ImageSharp.Image image, Label label, Rectangle rectangle, PointF points, Rgba32 boundingBoxColor, Rgba32 labelColor)
        {
            var labelName = label.Name;
            var confidence = Math.Round(label.Confidence, 2);
            var text = $"{labelName} ({confidence * 100}%)";
            var pen = new Pen(boundingBoxColor, 5);
            var font = SystemFonts.CreateFont("DejaVu Sans", 20, FontStyle.Bold);
            var color = labelColor;
            var brush = Brushes.Solid(color);
            image.Mutate(ctx => ctx
                .Draw(pen, rectangle)
                .DrawText(label.Name, font, brush, points)
            );
        }
    }
}
