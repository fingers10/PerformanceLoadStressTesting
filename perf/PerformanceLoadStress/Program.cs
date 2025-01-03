using NBomber.CSharp;
using NBomber.Data;
using NBomber.Data.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

using var httpClient = new HttpClient();

IDataFeed<string> locations = DataFeed.Random(new[] { "India", "New York", "Paris" });

var getScenario = Scenario.Create("get_weather_forecast", async context =>
{
    var location = locations.GetNextItem(context.ScenarioInfo);
    var request = Http.CreateRequest("GET", $"https://localhost:7116/weatherforecast?location={location}");
    return await Http.Send(httpClient, request);
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.RampingInject(rate: 100,
        interval: TimeSpan.FromSeconds(1),
        during: TimeSpan.FromSeconds(30))
);

NBomberRunner
    .RegisterScenarios(getScenario)
    .WithWorkerPlugins(new HttpMetricsPlugin())
    .Run();