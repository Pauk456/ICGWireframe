using System.Collections.Generic;
using Avalonia;

namespace ICGFrame.Domain.DTO;

public class FPoint
{
    private readonly double _x;
    private readonly double _y;
    public double X { get; set; }
    public double Y { get; set; }

    public FPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static List<Point> ToPoints(List<FPoint> fPoints)
    {
        var points = new List<Point>();
        foreach (var fpoint in fPoints)
        {
            points.Add(new Point(fpoint.X, fpoint.Y));
        }
        return points;
    }

    public static List<FPoint> ToFPoints(List<Point> points)
    {
        var fpoints = new List<FPoint>();
        foreach (var point in points)
        {
            fpoints.Add(new FPoint(point.X, point.Y));
        }
        return fpoints;
    }
}