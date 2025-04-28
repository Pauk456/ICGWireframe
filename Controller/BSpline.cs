using System;
using System.Collections.Generic;
using System.Windows;

public class BSpline
{
    public List<Point> points;
    private static readonly double[,] Ms = new double[4, 4]
    {
        { -1.0/6,  3.0/6, -3.0/6, 1.0/6 },
        {  3.0/6, -6.0/6,  3.0/6, 0.0/6 },
        { -3.0/6,  0.0/6,  3.0/6, 0.0/6 },
        {  1.0/6,  4.0/6,  1.0/6, 0.0/6 }
    };

    public int CountApproachingLines { get; set; } = 20;

    public BSpline()
    {
        points = new List<Point>();
    }

    public void AddPoint(double x, double y)
    {
        points.Add(new Point(x, y));
    }

    public List<Point> GetBSpline()
    {
        if (points.Count < 4)
            return new List<Point>(points);

        var result = new List<Point>();

        for (int i = 1; i < points.Count - 2; i++)
        {
            Point p0 = points[i - 1];
            Point p1 = points[i];
            Point p2 = points[i + 1];
            Point p3 = points[i + 2];

            for (int j = 0; j <= CountApproachingLines; j++)
            {
                double t = (double)j / CountApproachingLines;
                double t2 = t * t;
                double t3 = t2 * t;

                double b0 = (-t3 + 3 * t2 - 3 * t + 1) / 6;
                double b1 = (3 * t3 - 6 * t2 + 4) / 6;
                double b2 = (-3 * t3 + 3 * t2 + 3 * t + 1) / 6;
                double b3 = t3 / 6;

                double x = b0 * p0.X + b1 * p1.X + b2 * p2.X + b3 * p3.X;
                double y = b0 * p0.Y + b1 * p1.Y + b2 * p2.Y + b3 * p3.Y;

                result.Add(new Point(x, y));
            }
        }

        return result;
    }

    public List<Point> Get3DFigure()
    {
       
    }
}