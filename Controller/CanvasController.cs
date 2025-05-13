using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace ICGWireframe.Controller;
public class CanvasController
{
    private readonly DotManager _dotManager;
    private readonly SplineRenderer _splineRenderer;
    private readonly ConnectingLineRenderer _connectingLineRenderer;
    private readonly CoordinateSystemDrawer _coordinateSystem;
    private readonly ZoomHandler _zoomHandler;
    private readonly DragHandler _dragHandler;

    public CanvasController(Canvas canvas)
    {
        _coordinateSystem = new CoordinateSystemDrawer(canvas);
        _dotManager = new DotManager(canvas);
        _splineRenderer = new SplineRenderer(canvas);
        _connectingLineRenderer = new ConnectingLineRenderer(canvas);

        _zoomHandler = new ZoomHandler(
            _coordinateSystem,
            _connectingLineRenderer,
            _splineRenderer,
            _dotManager);

        _dragHandler = new DragHandler(canvas, UpdateSpline, UpdateConnectingLine);
    }

    public void SetSplineSegments(int count)
    {
        _splineRenderer.CountApproachingLines = count;
        UpdateSpline();
    }

    public void AddOrRemoveDot(Point position)
    {
        var hitDot = _dotManager.GetDotAt(position);
        if (hitDot != null)
        {
            _dotManager.RemoveDot(hitDot);
        }
        else
        {
            _dotManager.AddDot(position);
        }

        UpdateConnectingLine();
        UpdateSpline();
    }

    public void StartDragging(Point position)
    {
        _dragHandler.StartDragging(position);
    }

    public void UpdateDotPosition(Point position)
    {
        _dragHandler.UpdateDotPosition(position);
    }

    public void StopDragging()
    {
        _dragHandler.StopDragging();
    }

    public void OnMouseWheel(Point mousePosition, int delta, ModifierKeys modifiers)
    {
        _zoomHandler.HandleMouseWheel(mousePosition, delta, modifiers);
    }

    private void UpdateConnectingLine()
    {
        _connectingLineRenderer.Update(_dotManager.Dots.Select(d => d.Position));
    }

    public void UpdateSpline()
    {
        _splineRenderer.Update(_dotManager.Dots.Select(d => d.Position));
    }
}