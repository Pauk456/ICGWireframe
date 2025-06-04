using System;
using System.Collections.Generic;
using System.Linq;
using ICGFrame.Domain.DTO;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using Avalonia.Input;
using ICGFrame.Domain;

namespace ICGFrame.Presentation.Views;

public class DPanel : UserControl
{
    private Matrix4x4 _rotationMatrix = Matrix4x4.Identity;
    private Point? _lastMousePosition;
    private float _sensitivity = 0.005f;
    private float _zn = 100.0f;
    private float _zf = 1000.0f;
    private ParamContainer _container;
    public ParamContainer PContainer
    {
        get => _container;
        set
        {
            _container = value;
            InvalidateVisual();
        }
    }
    public DPanel(ParamContainer container)
    {
        _container = container;
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
        PointerWheelChanged += OnPointerWheelChanged;
    }
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.FillRectangle(Brushes.White, Bounds);
        var BFigure = BSplineCalculator.GenerateBSpline(_container.Points, _container.N);
        var figure = GenerateFigure(_container.M, _container.K, _container.M1, BFigure);
        NormalizeFigure(figure);

        var viewMatrix = CreateViewMatrix();
        var projMatrix = CreateProjectionMatrix(_zn, _zf, (float)Bounds.Width, (float)Bounds.Height);

        var transformMatrix = _rotationMatrix * viewMatrix * projMatrix;

        foreach (var line in figure)
        {
            var transformedPoints = line.Points
                .Select(p => TransformPoint(p, transformMatrix))
                .ToList();

            DrawLine(context, transformedPoints);
        }
        DrawAxesGizmo(context, transformMatrix);
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        float delta = e.Delta.Y > 0 ? -1f : 1f;
        _zn = Math.Clamp(_zn + delta, 1.0f, _zf - 1.0f);

        InvalidateVisual();
        e.Handled = true;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _lastMousePosition = e.GetPosition(this);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_lastMousePosition.HasValue &&
            e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var currentPosition = e.GetPosition(this);
            var delta = currentPosition - _lastMousePosition.Value;

            UpdateRotationMatrix(delta.X * _sensitivity, delta.Y * _sensitivity);

            _lastMousePosition = currentPosition;
            InvalidateVisual();
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _lastMousePosition = null;
    }

    private void UpdateRotationMatrix(double deltaX, double deltaY)
    {
        var yRotation = Matrix4x4.CreateRotationY((float)deltaX);
        var xRotation = Matrix4x4.CreateRotationX((float)deltaY);
        _rotationMatrix = yRotation * xRotation * _rotationMatrix;
    }

    private Point TransformPoint(Point3D p, Matrix4x4 transform)
    {
        var v = new Vector4(p.X, p.Y, p.Z, 1);
        v = Vector4.Transform(v, transform);

        if (v.W != 0) v /= v.W;

        return new Point(
            (v.X + 1) * Bounds.Width / 2,
            (1 - v.Y) * Bounds.Height / 2
        );
    }

    private void DrawLine(DrawingContext context, List<Point> points)
    {
        for (int i = 1; i < points.Count; i++)
        {
            context.DrawLine(
                new Pen(Brushes.Black, 1),
                points[i - 1],
                points[i]
            );
        }
    }

    private Matrix4x4 CreateViewMatrix()
    {
        var cameraPos = new Vector3(-10, 0, 0);
        var target = new Vector3(10, 0, 0);
        var up = Vector3.UnitY;

        return Matrix4x4.CreateLookAt(cameraPos, target, up);
    }

    private Matrix4x4 CreateProjectionMatrix(float zn, float zf, float sw, float sh)
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(
            MathF.PI / 4,
            sw / sh,
            zn,
            zf
        );
    }

    private void NormalizeFigure(List<Line3D> figure)
    {
        float minX = figure.Min(l => l.Points.Min(p => p.X));
        float maxX = figure.Max(l => l.Points.Max(p => p.X));
        float minY = figure.Min(l => l.Points.Min(p => p.Y));
        float maxY = figure.Max(l => l.Points.Max(p => p.Y));
        float minZ = figure.Min(l => l.Points.Min(p => p.Z));
        float maxZ = figure.Max(l => l.Points.Max(p => p.Z));

        float scale = 2 / Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));

        foreach (var line in figure)
        {
            foreach (var point in line.Points)
            {
                point.X = (point.X - (maxX + minX) / 2) * scale;
                point.Y = (point.Y - (maxY + minY) / 2) * scale;
                point.Z = (point.Z - (maxZ + minZ) / 2) * scale;
            }
        }
    }


    List<Line3D> GenerateFigure(int M, int K, int M1, List<Point> generatingLine)
    {
        var figure = new List<Line3D>();
        int N = (generatingLine.Count - 1) / (K - 3);

        // 1. Генерация M образующих
        for (int j = 0; j < M; j++)
        {
            var line = new Line3D();
            float angle = (float)(j * 360.0 / M * Math.PI / 180.0);

            foreach (Point point in generatingLine)
            {
                float u = (float)point.X;
                float v = (float)point.Y;

                float x = v * (float)Math.Cos(angle);
                float y = v * (float)Math.Sin(angle);
                float z = u;

                line.Points.Add(new Point3D { X = x, Y = y, Z = z });
            }

            figure.Add(line);
        }

        // 2. Генерация окружностей по опорным точкам
        for (int i = 0; i <= K - 3; i++)
        {
            int index = i * N;
            if (index >= generatingLine.Count) break;

            var circle = new Line3D();
            for (int j = 0; j < M; j++)
            {
                for (int m = 0; m < M1 + 1; m++)
                {
                    float phi = (float)(j + (float)m / M1) * 360.0f / M;
                    float angle = phi * (float)(Math.PI / 180.0);

                    float v = (float)generatingLine[index].Y;
                    float u = (float)generatingLine[index].X;

                    float x = v * (float)Math.Cos(angle);
                    float y = v * (float)Math.Sin(angle);
                    float z = u;

                    circle.Points.Add(new Point3D { X = x, Y = y, Z = z });
                }
            }

            figure.Add(circle);
        }
        var lastPoint = generatingLine[generatingLine.Count - 1];
        var closingCircle = new Line3D();

        for (int j = 0; j < M + 1; j++)
        {
            for (int m = 0; m < M1; m++)
            {
                float phi = (float)(j + (float)m / M1) * 360.0f / M;
                float angle = phi * (float)(Math.PI / 180.0);

                float v = (float)lastPoint.Y;
                float u = (float)lastPoint.X;

                float x = v * (float)Math.Cos(angle);
                float y = v * (float)Math.Sin(angle);
                float z = u;

                var pt = new Point3D { X = x, Y = y, Z = z };

                closingCircle.Points.Add(pt);
            }
        }
        figure.Add(closingCircle);

        return figure;
    }
    
    void DrawAxesGizmo(DrawingContext context, Matrix4x4 matrix)
    {
        var origin = new Point3D { X = 0, Y = 0, Z = 0 };
        var axisX = new Point3D { X = 1, Y = 0, Z = 0 };
        var axisY = new Point3D { X = 0, Y = 1, Z = 0 };
        var axisZ = new Point3D { X = 0, Y = 0, Z = 1 };

        // Применим текущую матрицу поворота
        var rotOrigin = TransformPoint(origin, matrix);
        var rotX = TransformPoint(axisX, matrix);
        var rotY = TransformPoint(axisY, matrix);
        var rotZ = TransformPoint(axisZ, matrix);

        var centerX = Bounds.Width / 3;
        var centerY = Bounds.Height / 3;

        // Спроецируем в 2D (на маленькой области, скажем, 80x80 в левом верхнем углу)
        Point Project(Point p) => new Point(p.X + centerX, p.Y - centerY); // масштаб + смещение

        var pOrigin = Project(rotOrigin);
        var pX = Project(rotX);
        var pY = Project(rotY);
        var pZ = Project(rotZ);

        // Рисуем стрелки
        var penX = new Pen(Brushes.Red, 2);
        var penY = new Pen(Brushes.Green, 2);
        var penZ = new Pen(Brushes.Blue, 2);

        context.DrawLine(penX, pOrigin, pX);
        context.DrawLine(penY, pOrigin, pY);
        context.DrawLine(penZ, pOrigin, pZ);

        // Можно добавить подписи "X", "Y", "Z"
        var textBrush = Brushes.Black;
        var textX = new FormattedText(
            "X",
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            12,
            Brushes.Red
        );
        context.DrawText(textX, pX + new Avalonia.Vector(4, -4));

        var textY = new FormattedText(
            "Y",
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            12,
            Brushes.Green
        );
        context.DrawText(textY, pY + new Avalonia.Vector(4, -4));

        var textZ = new FormattedText(
            "Z",
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface("Arial"),
            12,
            Brushes.Blue
        );
        context.DrawText(textZ, pZ + new Avalonia.Vector(4, -4));
    }
    
}