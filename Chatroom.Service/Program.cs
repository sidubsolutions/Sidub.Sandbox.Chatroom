using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sidub.Messaging.Host.SignalR;
using Sidub.Platform.Authentication.IsolatedFunction;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults((context, builder) =>
    {
        builder.Services.Configure<AuthenticationOptions>(context.Configuration.GetSection(nameof(AuthenticationOptions)));
        builder.Services.Configure<MessageServerOptions>(context.Configuration.GetSection(nameof(MessageServerOptions)));
        builder.AddSidubSignalRServer();

    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();