using NBomber.CSharp;

using var httpClient = new HttpClient();

var listScenario = Scenario.Create("get_weather_forecast", async context =>
{
    var response = await httpClient.GetAsync("https://localhost:5001/weatherforecast");

    return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
})
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.RampingInject(rate: 100,
        interval: TimeSpan.FromSeconds(1),
        during: TimeSpan.FromSeconds(30))
);

NBomberRunner
    .RegisterScenarios(listScenario)
    .Run();