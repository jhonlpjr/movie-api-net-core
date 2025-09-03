using Application;
using Infrastructure;
using MovieApi.Middleware;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers()
       .AddJsonOptions(options =>
       {
           options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
       });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.UseInlineDefinitionsForEnums();
});
// Orden sugerido: primero Application (usa solo Domain), luego Infrastructure (depende de config/externos)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddTransient<ErrorHandlingMiddleware>();

builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

app.UseCors();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();
app.Run();
