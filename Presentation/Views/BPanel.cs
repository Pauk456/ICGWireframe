using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using ICGFrame.Domain;
using ICGFrame.Domain.DTO;
using ICGFrame.Infrastruction;
using ICGFrame.Presentation.ViewModels;

namespace ICGFrame.Presentation.Views;

public class BPanel : UserControl
{
    private double _scale = 1.0;
    private Vector _offset = new Vector(0, 0);
    private Point? _lastPanPosition = null;
    public double SplineThickness => 2.0;
    private int _selectedIndex = -1;
    private Point _startDragPosition;
    public List<Point> Points => _container.Points;
    private ParamContainer _container;

    public static double Radius => 10;

    public BPanel(ParamContainer container)
    {
        AddHandler(PointerPressedEvent, OnPointerPressed, RoutingStrategies.Tunnel);
        AddHandler(PointerReleasedEvent, OnPointerReleased, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, OnPointerMoved, RoutingStrategies.Tunnel);
        // AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
        _container = container;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var screenPos = e.GetPosition(this);
        var position = ScreenToLogicalPoint(screenPos);

        if (e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
        {
            int index = PointExtension.FindSelectedPointIndex(Points, position, Radius);

            if (index != -1)
            {
                _container.Points.RemoveAt(index);
            }
            else
            {
                AddNewCircle(position);
            }

            e.Handled = true;
            InvalidateVisual();
            return;
        }


        _selectedIndex = PointExtension.FindSelectedPointIndex(Points, position, Radius);
        _startDragPosition = position;

        if (_selectedIndex != -1 && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            e.Pointer.Capture(this);
            e.Handled = true;
            _lastPanPosition = null;
        }
        //else if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        //{
        //    _lastPanPosition = ScreenToLogicalPoint(e.GetPosition(this));
        //}
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_selectedIndex != -1 && e.Pointer.Captured == this)
        {
            var screenPos = e.GetPosition(this);
            var newPosition = ScreenToLogicalPoint(screenPos);

            var offset = newPosition - _startDragPosition;

            var point = Points[_selectedIndex];
            Points[_selectedIndex] = new Point(
                point.X + offset.X,
                point.Y + offset.Y
            );

            _startDragPosition = newPosition;
            InvalidateVisual();
            e.Handled = true;
        }
        else if (_lastPanPosition.HasValue && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var screenPos = e.GetPosition(this);
            var current = ScreenToLogicalPoint(screenPos);

            var delta = current - _lastPanPosition.Value;
            _offset += delta;
            _lastPanPosition = ScreenToLogicalPoint(e.GetPosition(this));
            InvalidateVisual();
            e.Handled = true;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _selectedIndex = -1;
        _lastPanPosition = null;

        e.Pointer.Capture(null);
        e.Handled = true;
    }

    public override void Render(DrawingContext context)
    {
        var translateMatrix = Matrix.CreateTranslation(_offset);
        context.PushTransform(translateMatrix);

        base.Render(context);

        context.FillRectangle(Brushes.White, Bounds);

        if (Points == null || Points.Count == 0) return;

        var diameter = Radius * 2;
        var newPoints = LogicalToScreenPoints(Points);

        foreach (var point in newPoints)
        {
            var rect = new Rect(
                point.X - Radius,
                point.Y - Radius,
                diameter,
                diameter);

            context.DrawEllipse(Brushes.Black, null, rect.Center, Radius, Radius);
        }

        if (newPoints != null && newPoints.Count >= 4)
        {
            var splinePoints = BSplineCalculator.GenerateBSpline(newPoints.ToList(), _container.N);

            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(newPoints.First(), false);
                foreach (var point in newPoints.Skip(1))
                {
                    ctx.LineTo(point);
                }
            }
            context.DrawGeometry(
                Brushes.Black,
                new Pen(Brushes.Black, SplineThickness),
                geometry);

            geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(splinePoints.First(), false);
                foreach (var point in splinePoints.Skip(1))
                {
                    ctx.LineTo(point);
                }
            }
            context.DrawGeometry(
                Brushes.Gray,
                new Pen(Brushes.Gray, SplineThickness),
                geometry);
        }

        var axisPen = new Pen(Brushes.LightGray, 1);
        context.DrawLine(axisPen, new Point(-10000, Bounds.Height / 2), new Point(10000, Bounds.Height / 2));
        context.DrawLine(axisPen, new Point(Bounds.Width / 2, -10000), new Point(Bounds.Width / 2, 10000));
    }

    private void AddNewCircle(Point position)
    {
        if (Points == null) return;

        if (!Points.Any(p => PointExtension.GetDistance(p - position) < Radius))
        {
            Points.Add(position);
        }
    }
    public List<Point> LogicalToScreenPoints(List<Point> points)
    {
        var newPoints = new List<Point>();
        foreach (var point in points)
        {
            newPoints.Add(new Point((point.X + Bounds.Width / 2), (-point.Y + Bounds.Height / 2)));
        }
        return newPoints;
    }

    public Point ScreenToLogicalPoint(Point screenPoint)
    {
        double logicalX = (screenPoint.X - Bounds.Width / 2 - _offset.X * _scale) / _scale;
        double logicalY = -(screenPoint.Y - Bounds.Height / 2 + _offset.Y * _scale) / _scale;
        return new Point(logicalX, logicalY);
    }

    public void Validate()
    {
        _container.K = _container.Points.Count;
    }
}
