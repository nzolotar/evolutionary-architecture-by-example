namespace EvolutionaryArchitecture.Fitnet.Common.IntegrationTests.TestEngine.Configuration;

using System.Reflection;
using Infrastructure.Events.EventBus;
using Infrastructure.Mediator;
using EvolutionaryArchitecture.Fitnet.Common.Infrastructure.SystemClock;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SystemClock;

public static class ConfigurationExtensions
{
    public static WebApplicationFactory<T> WithContainerDatabaseConfigured<T>(
        this WebApplicationFactory<T> webApplicationFactory, Dictionary<string, string?> settings)
        where T : class
    {
        return webApplicationFactory.UseSettings(settings);
    }

    private static WebApplicationFactory<T> UseSettings<T>(this WebApplicationFactory<T> webApplicationFactory,
        Dictionary<string, string?> settings)
        where T : class =>
        webApplicationFactory.WithWebHostBuilder(webHostBuilder =>
        {
            foreach (var setting in settings)
                webHostBuilder.UseSetting(setting.Key, setting.Value);
        });

    public static WebApplicationFactory<T> SetFakeSystemClock<T>(this WebApplicationFactory<T> webApplicationFactory,
        DateTimeOffset fakeDateTimeOffset)
        where T : class =>
        webApplicationFactory.WithWebHostBuilder(webHostBuilder => webHostBuilder.ConfigureTestServices(services =>
            services.AddSingleton<ISystemClock>(new FakeSystemClock(fakeDateTimeOffset))));

    public static WebApplicationFactory<T> WithFakeEventBus<T>(this WebApplicationFactory<T> webApplicationFactory,
        IMock<IEventBus> eventBusMock)
        where T : class =>
        webApplicationFactory.WithWebHostBuilder(webHostBuilder => webHostBuilder.ConfigureTestServices(services =>
            services.AddSingleton(eventBusMock.Object)));

    public static WebApplicationFactory<T> WithFakeConsumers<T>(this WebApplicationFactory<T> webApplicationFactory,
        Assembly executingAssembly)
        where T : class =>
        webApplicationFactory.WithWebHostBuilder(webHostBuilder => webHostBuilder.ConfigureTestServices(services =>
            services.AddMediator(executingAssembly)));

}