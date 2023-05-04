using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Default;
using CommunityToolkit.Mvvm.ComponentModel;
using Material.Colors;
using Material.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;
using SimpleLogViewer.ViewModels;
using SimpleLogViewer.Views;

namespace SimpleLogViewer
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            Services = ConfigureServices();
            AvaloniaXamlLoader.Load(this);
        }
        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App? Current => Application.Current as App;

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);

                var mainWindow = Current?.Services.GetService<MainWindow>() ?? throw new NullReferenceException("");
                mainWindow.DataContext = Current?.Services.GetRequiredService<MainWindowViewModel>() ??
                                         throw new NullReferenceException("Could not resolve MainWindowViewModel");
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
         
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; set; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private  IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.Scan(scan => scan.FromAssemblies(GetType().Assembly).AddClasses(classes=> classes.AssignableTo<Window>()).AsSelf().AsImplementedInterfaces().WithSingletonLifetime());
            services.Scan(scan => scan.FromAssemblies(GetType().Assembly).AddClasses(classes=> classes.AssignableTo<ObservableObject>()).AsSelf().AsImplementedInterfaces().WithScopedLifetime());
            services.Scan(scan => scan.FromAssemblies(GetType().Assembly).AddClasses(classes=> classes.AssignableTo<UserControl>()).AsSelf().AsImplementedInterfaces().WithScopedLifetime());

            return services.BuildServiceProvider();
        }
       
    }
}