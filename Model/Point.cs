using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ICGWireframe.Model
{
    public class DotModel
    {
        public Ellipse Ellipse { get; }
        public Point Position { get; private set; }

        public DotModel(double x, double y)
        {
            Position = new Point(x, y);

            Ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Blue,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(Ellipse, x - Ellipse.Width / 2);
            Canvas.SetTop(Ellipse, y - Ellipse.Height / 2);
        }

        public void UpdatePosition(double x, double y)
        {
            Position = new Point(x, y);
            Canvas.SetLeft(Ellipse, x - Ellipse.Width / 2);
            Canvas.SetTop(Ellipse, y - Ellipse.Height / 2);
        }
    }
}