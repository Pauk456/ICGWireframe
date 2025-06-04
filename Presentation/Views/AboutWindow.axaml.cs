using Avalonia.ReactiveUI;
using ICGFrame.Presentation.ViewModels;

namespace ICGFrame.Presentation.Views;

public partial class AboutWindow : ReactiveWindow<AboutWindowViewModel>
{
    public AboutWindow()
    {
        InitializeComponent();
    }
}