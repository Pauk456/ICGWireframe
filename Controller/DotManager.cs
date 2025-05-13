using ICGWireframe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace ICGWireframe.Controller;

public class DotManager
{
    private readonly Canvas _canvas;
    private readonly List<DotModel> _dots = new();

    public IEnumerable<DotModel> Dots => _dots.AsReadOnly();

    public DotManager(Canvas canvas)
    {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
    }

    public void AddDot(Point position)
    {
        var dot = new DotModel(position.X, position.Y);
        _dots.Add(dot);
        _canvas.Children.Add(dot.Ellipse);
    }

    public void RemoveDot(DotModel dot)
    {
        if (dot != null)
        {
            _canvas.Children.Remove(dot.Ellipse);
            _dots.Remove(dot);
        }
    }

    public DotModel GetDotAt(Point position)
    {
        foreach (var dot in _dots)
        {
            double dx = dot.Position.X - position.X;
            double dy = dot.Position.Y - position.Y;

            if (dx * dx + dy * dy < 100)
                return dot;
        }

        return null;
    }

    public void UpdateDotPositions()
    {
        // Можно использовать если нужно обновить все точки сразу
    }
}