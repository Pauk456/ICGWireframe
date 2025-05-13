using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ICGWireframe.Controller;

namespace ICGWireframe
{
    public partial class MainWindow : Window
    {
        private readonly CanvasController _controller;

        public MainWindow()
        {
            InitializeComponent();
            _controller = new CanvasController(MainCanvas);
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SegmentsTextBox.Text, out int segments) && segments > 0)
            {
                _controller.SetSplineSegments(segments);
            }
            else
            {
                MessageBox.Show("Please enter a positive integer value", "Invalid input",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                SegmentsTextBox.Text = "10";
            }
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            var position = e.GetPosition(canvas);
            _controller.AddOrRemoveDot(position);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Ellipse ellipse)
            {
                var canvas = sender as Canvas;
                var position = e.GetPosition(canvas);

                ellipse.CaptureMouse();
                ellipse.Fill = Brushes.Red;

                _controller.StartDragging(position);
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is Ellipse ellipse && ellipse.IsMouseCaptured)
            {
                ellipse.ReleaseMouseCapture();
                ellipse.Fill = Brushes.Blue;

                var canvas = sender as Canvas;
                var position = e.GetPosition(canvas);

                _controller.StopDragging();
                _controller.UpdateSpline();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Source is Ellipse ellipse && ellipse.IsMouseCaptured && e.LeftButton == MouseButtonState.Pressed)
            {
                var canvas = sender as Canvas;
                var position = e.GetPosition(canvas);

                _controller.UpdateDotPosition(position);
                _controller.UpdateSpline();
            }
        }
    }
}