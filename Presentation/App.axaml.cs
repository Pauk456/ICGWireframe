using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ICGFrame.Domain.DTO;
using ICGFrame.Presentation.ViewModels;
using ICGFrame.Presentation.Views;

namespace ICGFrame.Presentation;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var container = new ParamContainer();
            var window = new MainWindow(container);
            window.DataContext = new MainWindowViewModel(container);
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}