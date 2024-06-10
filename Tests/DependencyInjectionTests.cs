using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests;

public static class DependencyInjectionTests
{
    [Fact]
    public static async Task ScopedFactory()
    {
        await using var serviceProvider = new ServiceCollection()
           .AddScoped<FactoryUsingServiceProvider>(sp => new FactoryUsingServiceProvider(sp))
           .AddScoped<SomeService>()
           .BuildServiceProvider();

        await using var scope = serviceProvider.CreateAsyncScope();
        var factory = scope.ServiceProvider.GetRequiredService<FactoryUsingServiceProvider>();
        var instanceA = factory.ResolveService();
        var instanceB = factory.ResolveService();

        factory.ServiceProvider.Should().NotBeSameAs(serviceProvider);
        instanceA.Should().BeSameAs(instanceB);
    }
    
    private sealed class FactoryUsingServiceProvider
    {
        public FactoryUsingServiceProvider(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;
        public IServiceProvider ServiceProvider { get; }

        public SomeService ResolveService() => ServiceProvider.GetRequiredService<SomeService>();
    }

    private sealed class SomeService;
}