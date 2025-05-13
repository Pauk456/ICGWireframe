using ICGWireframe.Model;
using System.Windows.Controls;
using System.Windows;
namespace ICGWireframe.Controller;

public class DragHandler
{
    private DotModel _draggedDot;

    private readonly Canvas _canvas;
    private readonly Action _onUpdateSpline;
    private readonly Action _onUpdateConnectingLine;

    public DragHandler(Canvas canvas, Action onUpdateSpline, Action onUpdateConnectingLine)
    {
        _canvas = canvas;
        _onUpdateSpline = onUpdateSpline;
        _onUpdateConnectingLine = onUpdateConnectingLine;
    }

    public void StartDragging(Point position)
    {
        // Здесь можно реализовать получение точки через DotManager
    }

    public void UpdateDotPosition(Point position)
    {
        if (_draggedDot != null)
        {
            _draggedDot.UpdatePosition(position.X, position.Y);
            _onUpdateConnectingLine?.Invoke();
            _onUpdateSpline?.Invoke();
        }
    }

    public void StopDragging()
    {
        _draggedDot = null;
    }
}