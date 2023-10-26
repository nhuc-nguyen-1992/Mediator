using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nick.Mediator.Common.Grid.Processor;

namespace Nick.Mediator.Application;

public static class Setup
{
    /// <summary>
    /// Global setup for the application layer
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddOptions();

        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddScoped<CommonGridHandler>();

        return services;
    }

}