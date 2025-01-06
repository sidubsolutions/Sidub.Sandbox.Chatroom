using Chatroom.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sidub.Licensing.StorageProvider;
using Sidub.Licensing.StorageProvider.Extensions;
using Sidub.Messaging;
using Sidub.Messaging.Connectors.SignalR;
using Sidub.Messaging.Services;
using Sidub.Platform.Authentication;
using Sidub.Platform.Authentication.Credentials;
using Sidub.Platform.Core;
using Sidub.Platform.Core.Services;

namespace Chatroom.Client
{

    public class Program
    {

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: false);
                    config.AddUserSecrets<Program>();
                });

        //public static void Main(string[] args)
        //{
        //    MainAsync(args).GetAwaiter().GetResult();

        //    Console.ReadKey();
        //}

        private static MessagingServiceReference MessagingService = new MessagingServiceReference("Chatroom");

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Enter your email address: ");
            var email = Console.ReadLine();

            Console.WriteLine("Enter your name: ");
            var name = Console.ReadLine();

            var hostBuilder = CreateHostBuilder(args);
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.Configure<LicensingServiceOptions>(hostContext.Configuration.GetSection(nameof(LicensingServiceOptions)));
                services.Configure<ChatOptions>(hostContext.Configuration.GetSection(nameof(ChatOptions)));
                services.AddSidubPlatform(provider =>
                {
                    var licensingOptions = provider.GetRequiredService<IOptionsSnapshot<LicensingServiceOptions>>().Value
                        ?? throw new Exception("Failed to retrieve LicensingServiceOptions instance.");

                    var chatOptions = provider.GetRequiredService<IOptionsSnapshot<ChatOptions>>().Value
                        ?? throw new Exception("Failed to retrieve ChatOptions instance.");

                    var metadata = new InMemoryServiceRegistry();
                    metadata.RegisterLicense(licensingOptions);

                    var serviceUri = new Uri(new Uri(chatOptions.ChatServiceUri), "api").AbsoluteUri;

                    var messageServiceConnector = new SignalRConnector(serviceUri, "Chatroom");
                    metadata.RegisterServiceReference(MessagingService, messageServiceConnector);

                    var messageServiceIdentityReference = new AuthenticationServiceReference("Chatroom-Identity");

                    if (chatOptions.TenantId is not null && chatOptions.ClientId is not null)
                    {
                        if (chatOptions.ClientSecret is null)
                        {
                            var azureCredential = new Azure.Identity.InteractiveBrowserCredential();
                            var userCredential = new UserTokenCredential(azureCredential, @$"{chatOptions.ClientId}/.default", chatOptions.ClientId.Value, chatOptions.TenantId.Value, "https://localhost");

                            metadata.RegisterServiceReference(messageServiceIdentityReference, userCredential, MessagingService);
                        }
                        else
                        {
                            var messageServiceIdentity = new ClientSecretCredential(chatOptions.TenantId.Value.ToString("D"), chatOptions.ClientId.Value.ToString("D"), chatOptions.ClientSecret);

                            metadata.RegisterServiceReference(messageServiceIdentityReference, messageServiceIdentity, MessagingService);
                        }
                    }

                    return metadata;
                });

                services.AddSidubAuthenticationForSignalR();
                services.AddSidubLicensing();
                services.AddSidubMessaging();
            });

            using var host = hostBuilder.Build();

            var messagingService = host.Services.GetService<IMessagingService>()
                ?? throw new Exception("Failed to retrieve IMessagingService.");

            await messagingService.Subscribe<ChatMessage>(MessagingService, async envelope =>
            {
                await Task.Run(() => Console.WriteLine($"{envelope.Author}@{envelope.CreatedOn}: {envelope.Message?.Message}"));
            });

            while (true)
            {
                var msg = await GetInputAsync();

                if (msg == null)
                {
                    break;
                }

                var message = new ChatMessage()
                {
                    Message = msg
                };
                await messagingService.Broadcast(MessagingService, message);
            }

            await host.RunAsync();
        }

        private static Task<string?> GetInputAsync()
        {
            return Task.Run(() => Console.ReadLine());
        }

    }

}