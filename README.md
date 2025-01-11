# Chatroom Example

This repository demonstrates a simple, real-time chatroom application built using Sidub.Messaging and its related Sidub.* packages. The example includes both a client application (console-based) and a server-side Azure Functions project, showing how to leverage Sidub’s messaging and licensing capabilities for seamless communication through SignalR.

## Features

- Real-time messaging: Clients can send and receive messages instantly.  
- Azure Functions service: A lightweight server using Sidub.Messaging.Host.SignalR for message handling.  
- Licensing integration: Uses Sidub.Licensing to register licenses in the Sidub Platform.  
- Authentication options: Supports Azure credentials, client credentials, or an interactive browser login.  

## Azure Infrastructure Requirements

This chatroom application relies on Azure SignalR Service to enable real-time messaging in the Azure Functions environment. When running locally, you can either use the official [Azure SignalR Service Emulator (asrs-emulator)](https://github.com/Azure/azure-signalr-emulator) or configure an Azure SignalR Service instance in the cloud.

For production or more robust development scenarios, the Sidub.Messaging Azure Marketplace offer provides a one-click deployment that includes:
- An Azure SignalR instance
- Other required Azure services
- Preconfigured infrastructure for Sidub.Messaging

## Licensing

A valid license is required for the Sidub.Messaging components to function correctly. You must specify licensing information in the client configuration (e.g., in the local.settings.json for Chatroom.Client). If you do not currently have a license, you can request a trial at [https://sidub.ca/](https://sidub.ca/).

## Repository Structure

1. **Chatroom.Client/Program.cs**  
   - Implements a console-based chat client.  
   - Establishes a connection to the messaging service through Sidub.Messaging and Sidub.Messaging.Connectors.SignalR.  
   - Prompts the user for credentials, listens for signals, and broadcasts messages.

2. **Chatroom.Domain/ChatMessage.cs**  
   - Defines the `ChatMessage` data model with the `[Entity]` attribute.  
   - Includes a simple property (`Message`) to store the text content.

3. **Chatroom.Domain/ChatOptions.cs**  
   - Holds connection details (e.g., `ChatServiceUri`, Azure `TenantId`) for the chatroom service.

4. **Chatroom.Service**  
   - Hosts an Azure Functions application (see `Program.cs` and `ChatroomFunction.cs`) that configures Sidub.Messaging.Host.SignalR.  
   - Manages all inbound/outbound chat messages using Azure SignalR as the real-time transport.  
   - Provides a sample configuration file named `local.settings.template.json` for environment variables.

## Service-Side Configuration

In the Chatroom.Service project, you can find `local.settings.template.json`. It contains default environment variables for local development:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "MessageServerOptions__ConnectionString": "Endpoint=http://localhost;Port=8888;AccessKey=ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ABCDEFGH;Version=1.0;",
    "MessageServerOptions__UserIdentifierClaimType": "preferred_username",
    "AuthenticationOptions__ValidIssuer": "https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000/v2.0",
    "AuthenticationOptions__ValidAudience": "35a21cb0-bf6e-4de8-a7ad-736afc400ae8"
  }
}