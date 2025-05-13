using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
public class CoordinateSystemDrawer
{
    private readonly Canvas _canvas;

    public double ScaleFactor { get; set; } = 20;
    public int TickStepUnits { get; set; } = 1;
    public int LabelEveryNTicks { get; set; } = 2;

    private readonly Line _axisX;
    private readonly Line _axisY;
    private readonly TextBlock _labelX;
    private readonly TextBlock _labelY;

    private readonly List<Line> _ticks = new();
    private readonly List<TextBlock> _labels = new();

    private double _axisXPos;
    private double _axisYPos;

    public CoordinateSystemDrawer(Canvas canvas)
    {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));

        // Инициализация осей и подписей
        _axisX = new Line { Stroke = Brushes.Black, StrokeThickness = 1 };
        _axisY = new Line { Stroke = Brushes.Black, StrokeThickness = 1 };

        _labelX = new TextBlock { Text = "X", Foreground = Brushes.Black };
        _labelY = new TextBlock { Text = "Y", Foreground = Brushes.Black };

        _canvas.Children.Add(_axisX);
        _canvas.Children.Add(_axisY);
        _canvas.Children.Add(_labelX);
        _canvas.Children.Add(_labelY);
    }

    public void Update(double axisYPos, double axisXPos)
    {
        ClearTemporaryElements();

        _axisXPos = axisXPos;
        _axisYPos = axisYPos;

        double canvasWidth = _canvas.ActualWidth;
        double canvasHeight = _canvas.ActualHeight;

        if (double.IsNaN(canvasWidth) || double.IsNaN(canvasHeight) || canvasWidth <= 0 || canvasHeight <= 0)
            return;

        // --- Ось X ---
        _axisX.X1 = 0;
        _axisX.Y1 = _axisXPos;
        _axisX.X2 = canvasWidth;
        _axisX.Y2 = _axisXPos;

        // --- Ось Y ---
        _axisY.X1 = _axisYPos;
        _axisY.Y1 = 0;
        _axisY.X2 = _axisYPos;
        _axisY.Y2 = canvasHeight;

        // --- Подписи ---
        Canvas.SetLeft(_labelX, canvasWidth - 20);
        Canvas.SetTop(_labelX, _axisXPos + 5);

        Canvas.SetLeft(_labelY, _axisYPos + 5);
        Canvas.SetTop(_labelY, 5);

        DrawGridTicks();
    }

    private void DrawGridTicks()
    {
        int unitsToShow = 20;

        // Горизонтальные тики (ось Y)
        for (int xUnit = -unitsToShow; xUnit <= unitsToShow; xUnit++)
        {
            if (xUnit == 0) continue;

            double screenX = _axisYPos + xUnit * ScaleFactor;

            var tick = new Line
            {
                X1 = screenX,
                Y1 = _axisXPos - 5,
                X2 = screenX,
                Y2 = _axisXPos + 5,
                Stroke = Brushes.Gray,
                StrokeThickness = 1
            };
            _canvas.Children.Add(tick);
            _ticks.Add(tick);

            if (Math.Abs(xUnit) % LabelEveryNTicks == 0)
            {
                var label = new TextBlock
                {
                    Text = xUnit.ToString(),
                    FontSize = 10,
                    Foreground = Brushes.Gray
                };
                Canvas.SetLeft(label, screenX - 5);
                Canvas.SetTop(label, _axisXPos + 10);
                _canvas.Children.Add(label);
                _labels.Add(label);
            }
        }

        // Вертикальные тики (ось X)
        for (int yUnit = -unitsToShow; yUnit <= unitsToShow; yUnit++)
        {
            if (yUnit == 0) continue;

            double screenY = _axisXPos - yUnit * ScaleFactor;

            var tick = new Line
            {
                X1 = _axisYPos - 5,
                Y1 = screenY,
                X2 = _axisYPos + 5,
                Y2 = screenY,
                Stroke = Brushes.Gray,
                StrokeThickness = 1
            };
            _canvas.Children.Add(tick);
            _ticks.Add(tick);

            if (Math.Abs(yUnit) % LabelEveryNTicks == 0)
            {
                var label = new TextBlock
                {
                    Text = yUnit.ToString(),
                    FontSize = 10,
                    Foreground = Brushes.Gray
                };
                Canvas.SetLeft(label, _axisYPos - 20);
                Canvas.SetTop(label, screenY - 5);
                _canvas.Children.Add(label);
                _labels.Add(label);
            }
        }
    }

    private void ClearTemporaryElements()
    {
        foreach (var tick in _ticks)
            _canvas.Children.Remove(tick);
        _ticks.Clear();

        foreach (var label in _labels)
            _canvas.Children.Remove(label);
        _labels.Clear();
    }

    // <-- Новый метод -->
    public Point GetAbsolutePosition()
    {
        return _canvas.TranslatePoint(new Point(0, 0), null);
    }
}
