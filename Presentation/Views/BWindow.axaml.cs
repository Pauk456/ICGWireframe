using System;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using ICGFrame.Domain.DTO;
using ICGFrame.Presentation.ViewModels;
using ReactiveUI;

namespace ICGFrame.Presentation.Views;

public partial class BWindow : ReactiveWindow<BWindowViewModel>
{
    private BPanel _bPanel;
    public BWindow(ParamContainer container)
    {
        InitializeComponent();
        _bPanel = new(container);
        BGrid.Children.Add(_bPanel);

        DataContextChanged += RegisterInteraction;
    }

    private void RegisterInteraction(object? sender, EventArgs e)
    {
        this.WhenActivated(action =>
                action(ViewModel!.ApplyInteraction.RegisterHandler(ApplyParams)));
    }

    private void ApplyParams(IInteractionContext<Unit, Unit> context)
    {
        _bPanel.Validate();
        _bPanel.InvalidateVisual();
        context.SetOutput(Unit.Default);
    }
}