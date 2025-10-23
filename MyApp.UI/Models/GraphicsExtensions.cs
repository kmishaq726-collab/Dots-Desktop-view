using System.Drawing.Drawing2D;

namespace MyApp.Models
{
   public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(bounds.Left, bounds.Top, radius, radius, 180, 90);
                path.AddArc(bounds.Right - radius, bounds.Top, radius, radius, 270, 90);
                path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(bounds.Left, bounds.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }
    }
}