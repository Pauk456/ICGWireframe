using System.Collections.Generic;
using System.Text.Json.Serialization;
using Avalonia;
using ICGFrame.Presentation.ViewModels;
using ReactiveUI;


namespace ICGFrame.Domain.DTO;

public class ParamContainer : ReactiveObject
{
    private List<Point> _points;
    public List<FPoint> FPoints { get; set; }
    public List<Point> Points => _points;

    private int _k = 4;
    public int K
    {
        get => _points.Count;
        set
        {
            if (value >= 4)
            {
                if (value < _points.Count)
                {
                    _points.RemoveAt(_points.Count - 1);
                }
                else if (value > _points.Count)
                {
                    var lastPoint = _points[_points.Count - 1];
                    _points.Add(new Point(lastPoint.X + 20, lastPoint.Y));
                }
                this.RaiseAndSetIfChanged(ref _k, value);
            }
        }
    }

    private int _m = 5;

    public int M
    {
        get => _m;
        set
        {
            if (value >= 10)
            {
                this.RaiseAndSetIfChanged(ref _m, 10);
            }
            if (value <= 0)
            {
                this.RaiseAndSetIfChanged(ref _m, 1);
            }
            if (value >= 2)
            {
                this.RaiseAndSetIfChanged(ref _m, value);
            }
        }
    }

    private int _n = 1;
    public int N
    {
        get => _n;
        set
        {
            if (value >= 10)
            {
                this.RaiseAndSetIfChanged(ref _m, 10);
            }
            if (value <= 0)
            {
                this.RaiseAndSetIfChanged(ref _m, 1);
            }
            if (value >= 1)
            {
                this.RaiseAndSetIfChanged(ref _n, value);
            }

        }
    }

    private int _m1 = 1;
    public int M1
    {
        get => _m1;
        set
        {
            if (value >= 1)
            {
                this.RaiseAndSetIfChanged(ref _m1, value);
            }
        }
    }

    public ParamContainer()
    {
        FPoints = new();
        _points = new()
        {
                 new Point(100, 200),
                new Point(200, 150),
                new Point(300, 200),
                new Point(400, 150),  
                new Point(500, 200)
        };
    }

    [JsonConstructor]
    public ParamContainer(List<FPoint> fPoints, int k, int m, int n, int m1)
    {
        FPoints = fPoints;
        _points = FPoint.ToPoints(fPoints);
        K = k;
        N = n;
        M = m;
        M1 = m1;
    }
}