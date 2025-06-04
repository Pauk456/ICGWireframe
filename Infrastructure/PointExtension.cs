using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia;

namespace ICGFrame.Infrastruction;

public class PointExtension
{
    public static int FindSelectedPointIndex(
            List<Point> points, 
            Point position, double radius)
    {
        for (int i = 0; i < points.Count; i++)
        {
            var point = points[i];
            var vec = position - point;
            var distance = Math.Sqrt(vec.X * vec.X + vec.Y + vec.Y);
            
            if (distance <= radius)
            {
                return i;
            }
        }
        return -1;
    }

    public static double GetDistance(Point point)
    {
        return Math.Sqrt(point.X * point.X + point.Y * point.Y);
    }
}