using System;
using System.Collections.Generic;
using System.Windows;

namespace ICGWireframe.Controller
{
    public class BSpline
    {
        public IEnumerable<Point> GetBSpline(IEnumerable<Point> controlPoints, int CountApproachingLines)
        {
            var points = controlPoints as List<Point> ?? new List<Point>(controlPoints);
            if (points.Count < 4)
                yield break;

            for (int i = 0; i < points.Count - 3; i++)
            {
                Point p0 = points[i];
                Point p1 = points[i + 1];
                Point p2 = points[i + 2];
                Point p3 = points[i + 3];

                for (int j = 0; j <= CountApproachingLines; j++)
                {
                    double t = j / (double)CountApproachingLines;
                    Point point = CalculateBSplinePoint(t, p0, p1, p2, p3);
                    yield return point;
                }
            }
        }

        private Point CalculateBSplinePoint(double t, Point p0, Point p1, Point p2, Point p3)
        {
            double t2 = t * t;
            double t3 = t2 * t;

            double x = (
                (-p0.X + 3 * p1.X - 3 * p2.X + p3.X) * t3 +
                (3 * p0.X - 6 * p1.X + 3 * p2.X) * t2 +
                (-3 * p0.X + 3 * p2.X) * t +
                (p0.X + 4 * p1.X + p2.X)) / 6.0;

            double y = (
                (-p0.Y + 3 * p1.Y - 3 * p2.Y + p3.Y) * t3 +
                (3 * p0.Y - 6 * p1.Y + 3 * p2.Y) * t2 +
                (-3 * p0.Y + 3 * p2.Y) * t +
                (p0.Y + 4 * p1.Y + p2.Y)) / 6.0;

            return new Point(x, y);
        }
    }
}