using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ICGFrame.Domain.DTO;
using ReactiveUI;

namespace ICGFrame.Presentation.ViewModels;

public class BWindowViewModel : ReactiveObject
{
    private ParamContainer _container;
    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    public Interaction<Unit, Unit> ApplyInteraction = new();
    public Interaction<Unit, Unit> CloseInteraction = new();
    public ParamContainer Params
    {
        get => _container;
        set => this.RaiseAndSetIfChanged(ref _container, value);
    }

    public BWindowViewModel(ParamContainer container)
    {
        _container = container;
        ApplyCommand = ReactiveCommand.CreateFromTask(Accept);
    }

    public async Task Accept()
    {
        await ApplyInteraction.Handle(Unit.Default);
    }
}