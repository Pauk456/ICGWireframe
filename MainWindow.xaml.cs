using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace ICGWireframe;

public partial class MainWindow : Window
{
    private BSpline bSpline = new BSpline();
    private Polyline splineLine = new Polyline()
    {
        Stroke = Brushes.Red,
        StrokeThickness = 2
    };

    //private Polyline ConnectingLine = new Polyline() 
    //{
    //    Stroke = Brushes.Gray,
    //    StrokeThickness = 2
    //};

    public MainWindow()
    {
        InitializeComponent();
        MainCanvas.Children.Add(splineLine);
        bSpline.CountApproachingLines = int.Parse(SegmentsTextBox.Text);
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(SegmentsTextBox.Text, out int segments) && segments > 0)
        {
            bSpline.CountApproachingLines = segments;
            UpdateBSpline();
        }
        else
        {
            MessageBox.Show("Please enter a positive integer value", "Invalid input",
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            SegmentsTextBox.Text = bSpline.CountApproachingLines.ToString();
        }
    }

    private void UpdateBSpline()
    {
        bSpline.points.Clear();

        foreach (var child in MainCanvas.Children)
        {
            if (child is Ellipse ellipse)
            {
                double x = Canvas.GetLeft(ellipse) + ellipse.Width / 2;
                double y = Canvas.GetTop(ellipse) + ellipse.Height / 2;
                bSpline.AddPoint(x, y);
            }
        }

        splineLine.Points.Clear();
        foreach (var point in bSpline.GetBSpline())
        {
            splineLine.Points.Add(point);
        }
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Ellipse ellipse)
        {
            ellipse.CaptureMouse();
            ellipse.Fill = Brushes.Red;
        }
    }

    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Ellipse ellipse && ellipse.IsMouseCaptured)
        {
            ellipse.ReleaseMouseCapture();
            ellipse.Fill = Brushes.Blue;
            UpdateBSpline();
        }
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Source is Ellipse ellipse && ellipse.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
        {
            var canvas = sender as Canvas;
            var position = e.GetPosition(canvas);
            Canvas.SetLeft(ellipse, position.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, position.Y - ellipse.Height / 2);
            UpdateBSpline();
        }
    }

    private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        var canvas = sender as Canvas;
        var position = e.GetPosition(canvas);

        if (e.Source is Ellipse ellipse)
        {
            canvas.Children.Remove(ellipse);
        }
        else
        {
            var dot = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Blue,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(dot, position.X - dot.Width / 2);
            Canvas.SetTop(dot, position.Y - dot.Height / 2);
            canvas.Children.Add(dot);
        }

        UpdateBSpline();
    }
}
