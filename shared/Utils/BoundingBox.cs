using Amazon.Rekognition.Model;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace DetectifyLambdaServices.Utils
{
    public static class BoundingBox
    {
        public static void Draw(
            SixLabors.ImageSharp.Image image,
            DetectLabelsResponse recognitionResponse,
            string boundingBoxColorHex,
            string labelColorHex)
        {
            foreach (var foundObject in recognitionResponse.Labels)
            {
                foreach (var instance in foundObject.Instances)
                {
                    var bound = instance.BoundingBox;
                    var label = foundObject.Name;
                    RenderCurrentInstance(image, bound, label, boundingBoxColorHex, labelColorHex);
                }
            }
        }

        public static void Draw(
            SixLabors.ImageSharp.Image image,
            RecognizeCelebritiesResponse recognitionResponse,
            string boundingBoxColorHex,
            string labelColorHex)
        {
            foreach (var instance in recognitionResponse.CelebrityFaces)
            {
                var bound = instance.Face.BoundingBox;
                var label = instance.Name;
                RenderCurrentInstance(image, bound, label, boundingBoxColorHex, labelColorHex);
            }
        }

        private static void RenderCurrentInstance(
            SixLabors.ImageSharp.Image image,
            Amazon.Rekognition.Model.BoundingBox bound,
            string label,
            string boundingBoxHex,
            string labelHex)
        {
            boundingBoxHex ??= "#ff0000";
            labelHex ??= "#ffffff";

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

        private static void DrawBoundingRectangle(
            SixLabors.ImageSharp.Image image,
            string label,
            Rectangle rectangle,
            PointF points,
            Rgba32 boundingBoxColor,
            Rgba32 labelColor)
        {
            var pen = new Pen(boundingBoxColor, 5);
            var font = SystemFonts.CreateFont("DejaVu Sans", 20, FontStyle.Bold);
            var color = labelColor;
            var brush = Brushes.Solid(color);
            image.Mutate(ctx => ctx
                .Draw(pen, rectangle)
                .DrawText(label, font, brush, points)
            );
        }
    }
}
