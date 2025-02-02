using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args)
  .UseOrleans(builder =>
  {
    builder.UseLocalhostClustering()
      .ConfigureLogging(configure => configure.AddConsole())
      .AddMemoryGrainStorage("todoStore");
    builder.UseDashboard();
  });

  using var host = builder.Build();

  await host.RunAsync();

  Console.WriteLine("Silo running. Press enter to stop...");
  Console.ReadLine();

  await host.StopAsync();