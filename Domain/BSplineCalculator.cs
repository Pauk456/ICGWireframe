using System.Collections.Generic;
using Avalonia;


namespace ICGFrame.Domain;

public static class BSplineCalculator
{
    private static readonly double[,] BasisMatrix = new double[,]
    {
        { -1.0/6,  3.0/6, -3.0/6, 1.0/6 },
        {  3.0/6, -6.0/6,  3.0/6, 0.0/6 },
        { -3.0/6,  0.0/6,  3.0/6, 0.0/6 },
        {  1.0/6,  4.0/6,  1.0/6, 0.0/6 }
    };

    public static List<Point> GenerateBSpline(IList<Point> controlPoints, int segmentsPerSpline = 100)
    {
        var result = new List<Point>();
        
        if (controlPoints.Count < 4)
        {
            return result;
        }

        for (int i = 1; i < controlPoints.Count - 2; i++)
        {
            var p0 = controlPoints[i - 1];
            var p1 = controlPoints[i];
            var p2 = controlPoints[i + 1];
            var p3 = controlPoints[i + 2];

            for (int step = 0; step <= segmentsPerSpline; step++)
            {
                double t = (double)step / segmentsPerSpline;
                var point = CalculatePoint(p0, p1, p2, p3, t);
                result.Add(point);
            }
        }

        return result;
    }

    private static Point CalculatePoint(Point p0, Point p1, Point p2, Point p3, double t)
    {
        double[] tVector = { t * t * t, t * t, t, 1 };
        
        double[] coefficients = new double[4];
        for (int i = 0; i < 4; i++)
        {
            coefficients[i] = tVector[0] * BasisMatrix[0, i] +
                              tVector[1] * BasisMatrix[1, i] +
                              tVector[2] * BasisMatrix[2, i] +
                              tVector[3] * BasisMatrix[3, i];
        }
        double x = coefficients[0] * p0.X +
                  coefficients[1] * p1.X +
                  coefficients[2] * p2.X +
                  coefficients[3] * p3.X;

        double y = coefficients[0] * p0.Y +
                  coefficients[1] * p1.Y +
                  coefficients[2] * p2.Y +
                  coefficients[3] * p3.Y;

        return new Point(x, y);
    }
}