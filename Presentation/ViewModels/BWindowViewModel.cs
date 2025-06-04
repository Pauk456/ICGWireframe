using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ICGFrame.Domain.DTO;
using ReactiveUI;

namespace ICGFrame.Presentation.ViewModels;

public class BWindowViewModel : ReactiveObject
{
    private ParamContainer _container;
    private string _n;
    public string N
    {
        get => _n;
        set => this.RaiseAndSetIfChanged(ref _n, value);
    }

    private string _m;
    public string M
    {
        get => _m;
        set => this.RaiseAndSetIfChanged(ref _m, value);
    }
    public ReactiveCommand<Unit, Unit> ApplyCommand { get; }
    public Interaction<(string, string), Unit> ApplyInteraction = new();
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
        await ApplyInteraction.Handle((N, M));
    }
}
