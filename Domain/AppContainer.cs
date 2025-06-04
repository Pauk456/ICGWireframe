using System.Collections.Generic;
using Avalonia.Controls;
using ICGFrame.Domain.Enums;
using ICGFrame.Presentation.ViewModels;
using ICGFrame.Presentation.Views;
using ReactiveUI;

namespace ICGFrame.Domain;

public class AppContainer
{
    private Dictionary<WindowName, ReactiveObject> _models = [];
    public ReactiveObject GetModel(WindowName name) => _models[name]; 
    public AppContainer()
    {
        CreateMainModel();
    }

    private void CreateMainModel()
    {
        var model = new MainWindowViewModel(new DTO.ParamContainer());
        _models.Add(WindowName.MainWindow, model);
    }

}