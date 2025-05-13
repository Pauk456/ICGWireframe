using ICGWireframe.Controller;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace ICGWireframe.Controller;

public class SplineRenderer
{
    private readonly Polyline _splineLine;
    private readonly BSpline _spline = new();
    public int CountApproachingLines { get; set; } = 10;

    public SplineRenderer(Canvas canvas)
    {
        _splineLine = new Polyline
        {
            Stroke = Brushes.Red,
            StrokeThickness = 2
        };
        canvas.Children.Add(_splineLine);
    }

    public void Update(IEnumerable<Point> controlPoints)
    {
        _splineLine.Points.Clear();
        foreach (var point in _spline.GetBSpline(controlPoints, CountApproachingLines))
        {
            _splineLine.Points.Add(point);
        }
    }
}