using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ICGWireframe.Model;

namespace ICGWireframe.Controller
{
    public class CanvasController
    {
        private readonly Canvas _canvas;
        private readonly List<DotModel> _dots = new();
        private readonly BSpline _spline = new();

        private readonly Polyline _splineLine;
        private readonly Polyline _connectingLine; 

        private DotModel _draggedDot;

        public CanvasController(Canvas canvas)
        {
            _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));

            _splineLine = new Polyline
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            _connectingLine = new Polyline
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 5, 3 }
            };

            _canvas.Children.Add(_splineLine);
            _canvas.Children.Add(_connectingLine);

            UpdateConnectingLine();
        }

        public void StartDragging(Point position)
        {
            _draggedDot = GetDotAt(position);
        }

        public void UpdateDotPosition(Point position)
        {
            if (_draggedDot != null)
            {
                _draggedDot.UpdatePosition(position.X, position.Y);
                UpdateConnectingLine();
            }
        }

        public void StopDragging()
        {
            _draggedDot = null;
        }

        public void SetSplineSegments(int count)
        {
            _spline.CountApproachingLines = count;
            UpdateSpline();
        }

        public void AddOrRemoveDot(Point position)
        {
            var hitDot = GetDotAt(position);

            if (hitDot != null)
            {
                RemoveDot(hitDot);
            }
            else
            {
                AddDot(position);
            }

            UpdateConnectingLine();
            UpdateSpline();
        }

        private DotModel GetDotAt(Point position)
        {
            foreach (var dot in _dots)
            {
                double dx = dot.Position.X - position.X;
                double dy = dot.Position.Y - position.Y;

                if (dx * dx + dy * dy < 100)
                {
                    return dot;
                }
            }

            return null;
        }

        private void AddDot(Point position)
        {
            var dot = new DotModel(position.X, position.Y);
            _dots.Add(dot);
            _canvas.Children.Add(dot.Ellipse);
        }

        private void RemoveDot(DotModel dot)
        {
            _canvas.Children.Remove(dot.Ellipse);
            _dots.Remove(dot);
        }

        public void UpdateSpline()
        {
            _splineLine.Points.Clear();

            foreach (var point in _spline.GetBSpline(_dots.Select(d => d.Position)))
            {
                _splineLine.Points.Add(point);
            }
        }
        public void UpdateConnectingLine()
        {
            _connectingLine.Points.Clear();

            foreach (var dot in _dots)
            {
                _connectingLine.Points.Add(dot.Position);
            }
        }
    }
}