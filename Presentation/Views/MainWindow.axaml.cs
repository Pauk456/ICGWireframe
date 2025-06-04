using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.ReactiveUI;
using ICGFrame.Domain;
using ICGFrame.Domain.DTO;
using ICGFrame.Presentation.ViewModels;
using ReactiveUI;

namespace ICGFrame.Presentation.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private DPanel _dpanel;
    public MainWindow(ParamContainer container)
    {
        InitializeComponent();
        _dpanel = new(container);
        DGrid.Children.Add(_dpanel);

        DataContextChanged += OpenBWindow;
    }

    private void OpenBWindow(object? sender, EventArgs e)
    {
        this.WhenActivated(action =>
                action(ViewModel!.BInteraction.RegisterHandler(OpenBWindow)));
        this.WhenActivated(action =>
                action(ViewModel!.SaveInteraction.RegisterHandler(OpenSaveWindow)));
        this.WhenActivated(action =>
                action(ViewModel!.LoadInteraction.RegisterHandler(OpenLoadWindow)));
        this.WhenActivated(action =>
                action(ViewModel!.AboutInteraction.RegisterHandler(OpenAboutWindow)));
    }

    private async Task OpenAboutWindow(IInteractionContext<Unit, Unit> context)
    {
        var window = new AboutWindow();
        window.DataContext = new AboutWindowViewModel();
        await window.ShowDialog(this);
        context.SetOutput(Unit.Default);
    }

    private async Task OpenBWindow(IInteractionContext<ParamContainer, Unit> context)
    {
        var window = new BWindow(context.Input);
        window.DataContext = new BWindowViewModel(context.Input);
        await window.ShowDialog(this);
        context.SetOutput(Unit.Default);
    }
    private async Task OpenSaveWindow(IInteractionContext<Unit, IStorageFile?> context)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel is not null)
        {
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Сохранить Файл",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } }
                },
                DefaultExtension = ".json",
                ShowOverwritePrompt = true
            });
            context.SetOutput(file);
        }
        else
        {
            context.SetOutput(null);
        }
        
    }

    private async Task OpenLoadWindow(IInteractionContext<Unit, ParamContainer?> context)
    {
        var topLevel = GetTopLevel(this);
        if (topLevel is not null)
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Загрузить Файл",
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("JSON") { Patterns = new[] { "*.json" } }
                },
            });
            var selectedFile = files?.FirstOrDefault();
            if (selectedFile != null)
            {
                using var stream = await selectedFile.OpenReadAsync();
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                var container = JsonSerializer.Deserialize<ParamContainer>(json);
                if (container is not null)
                {
                    _dpanel.PContainer = container;
                    context.SetOutput(container);
                }
                else
                {
                    context.SetOutput(null);
                }
            }
            else
            {
                context.SetOutput(null);
            }
        }
        else
        {
            context.SetOutput(null);
        }
        
    }
}