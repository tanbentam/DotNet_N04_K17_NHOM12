using CommunityToolkit.Mvvm.ComponentModel;

namespace TravelApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _windowTitle = "Digital Travel Application";

        [ObservableProperty]
        private ObservableObject _currentViewModel;
    }
}
