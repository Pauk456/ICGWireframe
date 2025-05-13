using System.Windows.Input;
using System.Windows;


namespace ICGWireframe.Controller;
public class ZoomHandler
{
    private readonly CoordinateSystemDrawer _coordinateSystem;
    private readonly ConnectingLineRenderer _connectingLineRenderer;
    private readonly SplineRenderer _splineRenderer;
    private readonly DotManager _dotManager;

    private const double MinScale = 5;
    private const double MaxScale = 50;

    public ZoomHandler(
        CoordinateSystemDrawer coordinateSystem,
        ConnectingLineRenderer connectingLineRenderer,
        SplineRenderer splineRenderer,
        DotManager dotManager)
    {
        _coordinateSystem = coordinateSystem ?? throw new ArgumentNullException(nameof(coordinateSystem));
        _connectingLineRenderer = connectingLineRenderer ?? throw new ArgumentNullException(nameof(connectingLineRenderer));
        _splineRenderer = splineRenderer ?? throw new ArgumentNullException(nameof(splineRenderer));
        _dotManager = dotManager ?? throw new ArgumentNullException(nameof(dotManager));
    }

    public void HandleMouseWheel(Point mousePosition, int delta, ModifierKeys modifiers)
    {
        if ((modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            return;

        // Получаем центр экрана
        double centerX = SystemParameters.PrimaryScreenWidth / 2;
        double centerY = SystemParameters.PrimaryScreenHeight / 2;

        Point canvasPosition = _coordinateSystem.GetAbsolutePosition();
        double axisXInCanvas = centerY - canvasPosition.Y;
        double axisYInCanvas = centerX - canvasPosition.X;

        // Переводим позицию курсора в логические координаты
        double cursorLogicalX = (mousePosition.X - axisYInCanvas) / _coordinateSystem.ScaleFactor;
        double cursorLogicalY = (axisXInCanvas - mousePosition.Y) / _coordinateSystem.ScaleFactor;

        // Изменяем масштаб
        double newScale = _coordinateSystem.ScaleFactor + (delta > 0 ? 1 : -1) * (_coordinateSystem.ScaleFactor / 10);
        newScale = Math.Max(MinScale, Math.Min(MaxScale, newScale));

        // Рассчитываем новые координаты осей так, чтобы курсор остался на месте
        double newAxisY = mousePosition.X - cursorLogicalX * newScale;
        double newAxisX = mousePosition.Y + cursorLogicalY * newScale;

        // Обновляем всё
        _coordinateSystem.ScaleFactor = newScale;
        _coordinateSystem.Update(newAxisY, newAxisX);

        _connectingLineRenderer.Update(_dotManager.Dots.Select(d => d.Position));
        _splineRenderer.Update(_dotManager.Dots.Select(d => d.Position));
    }
}