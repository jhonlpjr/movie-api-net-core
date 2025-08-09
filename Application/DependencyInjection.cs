using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Use cases
        services.AddScoped<GetPopularMoviesUseCase>();
        // services.AddScoped<GetMovieByIdUseCase>();
        // services.AddScoped<SearchMoviesUseCase>();
        // + validators, mappers, etc.

        return services;
    }
}
