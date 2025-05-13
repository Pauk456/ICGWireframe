using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace ICGWireframe.Controller;

public class ConnectingLineRenderer
{
    private readonly Polyline _connectingLine;
    public ConnectingLineRenderer(Canvas canvas)
    {
        _connectingLine = new Polyline
        {
            Stroke = Brushes.Gray,
            StrokeThickness = 1,
            StrokeDashArray = new DoubleCollection { 5, 3 }
        };
        canvas.Children.Add(_connectingLine);
    }

    public void Update(IEnumerable<Point> points)
    {
        _connectingLine.Points.Clear();
        foreach (var point in points)
        {
            _connectingLine.Points.Add(point);
        }
    }
}