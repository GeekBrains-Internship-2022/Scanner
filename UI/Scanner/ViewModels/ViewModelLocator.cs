using Microsoft.Extensions.DependencyInjection;

namespace Scanner.ViewModels
{
    class ViewModelLocator
    {
        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();

        public ViewModelTestDB TestDBWindowModel => App.Services.GetRequiredService<ViewModelTestDB>();
        public SettingsWindowViewModel SettingsWindowModel => App.Services.GetRequiredService<SettingsWindowViewModel>();
    }
}
