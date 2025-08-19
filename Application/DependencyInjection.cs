
using Microsoft.Extensions.DependencyInjection;
using Application.UseCases;
namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Use cases
        services.AddScoped<GetPopularMoviesUseCase>();
        services.AddScoped<GetAllMoviesUseCase>();
        services.AddScoped<GetMovieByIdUseCase>();
        services.AddScoped<GetPopularMoviesUseCase>();
        services.AddScoped<GetRecommendedMoviesUseCase>();
        services.AddScoped<SearchMoviesUseCase>();
        services.AddScoped<CreateMovieUseCase>();

        return services;
    }
}
