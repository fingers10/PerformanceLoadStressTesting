using System.Data;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Data;
using NBomber.Data.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;
using NBomber.Sinks.InfluxDB;

using var httpClient = new HttpClient();
var _globalJwt = string.Empty;
IDataFeed<string> locations = DataFeed.Random(new[] { "India", "New York", "Paris" });
InfluxDBSink _influxDbSink = new();

var getScenario = Scenario.Create("get_weather_forecast", async context =>
{
    var location = locations.GetNextItem(context.ScenarioInfo);
    var request = Http.CreateRequest("GET", $"https://localhost:7116/weatherforecast?location={location}");
    return await Http.Send(httpClient, request);
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.RampingInject(rate: 300,
        interval: TimeSpan.FromSeconds(1),
        during: TimeSpan.FromSeconds(30))
);

var getWithAuthScenario = Scenario.Create("get_weather_forecast_with_auth", async context =>
{
    var location = locations.GetNextItem(context.ScenarioInfo);
    var request = Http.CreateRequest("GET", $"https://localhost:7116/weatherforecast?location={location}")
                      .WithHeader("Authorization", $"Bearer {_globalJwt}");
    return await Http.Send(httpClient, request);
})
.WithInit(async context => 
{
    var token = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = "https://demo.duendesoftware.com/connect/token",
        ClientId = "m2m",
        ClientSecret = "secret",
        Scope = "api",
    });
    _globalJwt = token.AccessToken!;
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.RampingInject(rate: 300,
        interval: TimeSpan.FromSeconds(1),
        during: TimeSpan.FromSeconds(30))
);

NBomberRunner
    .RegisterScenarios(getScenario, getWithAuthScenario)
    .WithWorkerPlugins(new HttpMetricsPlugin())
    // .WithWorkerPlugin(new ApdexScorePlugin())
    .LoadInfraConfig("infra-config.json")
    .WithReportingInterval(TimeSpan.FromSeconds(5))
    .WithReportingSinks(_influxDbSink)
    .WithTestSuite("ILoveDotNetPerformanceTest")
    .WithTestName("Get_WeatherForecast_Requests")
    .Run();

public class ApdexScorePlugin : IWorkerPlugin
{
    public Task Init(IBaseContext context, IConfiguration infraConfig)
    {
        throw new NotImplementedException();
    }

    public Task Start(SessionStartInfo sessionInfo)
    {
        throw new NotImplementedException();
    }

    public Task<DataSet> GetStats(NodeStats stats)
    {
        throw new NotImplementedException();
    }

    public string[] GetHints()
    {
        throw new NotImplementedException();
    }

    public Task Stop()
    {
        throw new NotImplementedException();
    }

    public string PluginName { get; }
    
    public void Dispose()
    {
    }
}