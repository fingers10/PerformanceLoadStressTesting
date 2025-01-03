using NBomber.CSharp;
using NBomber.Http;
using NBomber.Http.CSharp;

using var httpClient = new HttpClient();

var getScenario = Scenario.Create("get_weather_forecast", async context =>
{
    var request = Http.CreateRequest("GET", "https://localhost:7116/weatherforecast");
    return await Http.Send(httpClient, request);

    // var response = await httpClient.GetAsync("https://localhost:7116/weatherforecast");

    // return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
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