using GrainInterfaces;

var builder = WebApplication.CreateBuilder(args);


builder.UseOrleansClient(client =>
{
    client.UseLocalhostClustering();
});


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/todos", async (IClusterClient clusterClient) =>
{
    var managementGrain = clusterClient.GetGrain<IManagementGrain>(0);
    var hosts = await managementGrain.GetHosts();

    var result = new Dictionary<string, IEnumerable<string>>();
    foreach (var host in hosts)
    {
        var activations = await managementGrain.GetDetailedGrainStatistics(null, new[]
        {
            host.Key
        });

        result.Add(host.Key.ToString(), activations.Select(a => a.ToString()));
    }
    return Results.Ok(result);
});

app.MapPost("/todos", async (IClusterClient clusterClient, CreateTodo payload) =>
{
    var newGuid = Guid.NewGuid();
    var newTodoGrain = clusterClient.GetGrain<ITodoGrain>(newGuid);

    await newTodoGrain.AddTitle(payload.Title);

    return Results.Created();
});

app.MapGet("/todos/:id", async (IClusterClient clusterClient, Guid id) =>
{
    var grain = clusterClient.GetGrain<ITodoGrain>(id);

    var state = await grain.GetState();

    return Results.Ok(state);
});

app.MapPatch("/todos/:id", async (IClusterClient clusterClient, Guid id, string newTitle) =>
{
    var grain = clusterClient.GetGrain<ITodoGrain>(id);

    await grain.UpdateTitle(newTitle);
    return Results.Ok();
});

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record CreateTodo
{
    public string? Title { get; init; }
}