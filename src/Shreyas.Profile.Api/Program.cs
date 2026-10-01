using Shreyas.Profile.Api.Endpoints;
using Shreyas.Profile.Api.Middleware;
using Shreyas.Profile.Application.Quotes;
using Shreyas.Profile.Infrastructure.Quotes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IQuoteProvider, CuratedQuoteProvider>();
builder.Services.AddSingleton<GetQuoteOfTheDay>();

var app = builder.Build();

app.UseExceptionHandler();
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Shreyas Profile API v1"));
}

app.MapHealthEndpoints();
app.MapQuoteEndpoints();

app.Logger.LogInformation("Shreyas Profile API started in {Environment} environment.", app.Environment.EnvironmentName);

app.Run();

public partial class Program;
