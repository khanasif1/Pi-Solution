using pi.job.worker.driveAssist;


/*
 * https://swimburger.net/blog/dotnet/how-to-run-a-dotnet-core-console-app-as-a-service-using-systemd-on-linux
 * https://docubear.com/run-a-net-core-application-on-a-raspberry-pi/
 * https://docubear.com/run-a-net-core-application-on-a-raspberry-pi/
  */

IHost host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
