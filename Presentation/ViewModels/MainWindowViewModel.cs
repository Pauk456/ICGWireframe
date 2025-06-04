using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ICGFrame.Domain.DTO;
using ReactiveUI;

namespace ICGFrame.Presentation.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    private ParamContainer _container;

    public ParamContainer Params
    {
        get => _container;
        set => this.RaiseAndSetIfChanged(ref _container, value);
    }

    public ReactiveCommand<Unit, Unit> ChangeCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveFileCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadFileCommand { get; }
    public ReactiveCommand<Unit, Unit> AboutCommand { get; }
    public Interaction<ParamContainer, Unit> BInteraction = new();
    public Interaction<Unit, IStorageFile?> SaveInteraction = new();
    public Interaction<Unit, ParamContainer?> LoadInteraction = new();
    public Interaction<Unit, Unit> AboutInteraction = new();

    public MainWindowViewModel(ParamContainer container)
    {
        _container = container;
        ChangeCommand = ReactiveCommand.CreateFromTask(ShowBWindow);
        SaveFileCommand = ReactiveCommand.CreateFromTask(SaveContainerToJson);
        LoadFileCommand = ReactiveCommand.CreateFromTask(LoadContainerInJson);
        AboutCommand = ReactiveCommand.CreateFromTask(ShowAboutWindow);
    }

    public async Task ShowAboutWindow()
    {
        await AboutInteraction.Handle(Unit.Default);
    }

    public async Task ShowBWindow()
    {
        await BInteraction.Handle(_container);
    }

    public async Task SaveContainerToJson()
    {
        var file = await SaveInteraction.Handle(Unit.Default);
        if (file == null)
        {
            return;
        }

        using var stream = await file.OpenWriteAsync();
        using var writer = new StreamWriter(stream);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        _container.FPoints = FPoint.ToFPoints(_container.Points);
        var json = JsonSerializer.Serialize(_container, options);
        await writer.WriteAsync(json);
    }

    public async Task LoadContainerInJson()
    {
        var container = await LoadInteraction.Handle(Unit.Default);
        if (container != null)
        {
            Params = container;
        }
    }


}