using CourseHub.API.Realtime.Services;
using CourseHub.API.Realtime.Services.Messaging;
using CourseHub.API.Realtime.Services.Stream;
using CourseHub.Core.Interfaces.Logging;
using CourseHub.Core.Services.Domain.SocialServices.Contracts;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CourseHub.API.Realtime.Hubs;

public partial class AppHub : Hub
{
    private readonly IConversationService _conversationService;
    private readonly IChatMessageService _chatMessageService;
    private readonly IAppLogger _logger;

    public AppHub(IConversationService conversationService, IChatMessageService chatMessageService, IAppLogger logger)
    {
        _conversationService = conversationService;
        _chatMessageService = chatMessageService;
        _logger = logger;
    }



    public override Task OnConnectedAsync()
    {
        _logger.Inform(JsonSerializer.Serialize(Context.User,
            options: new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            })
        );

        _logger.Inform("http:" + JsonSerializer.Serialize(Context.GetHttpContext()?.User,
            options: new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            })
        );

        ConnectionsHandler.Connected(Context);

        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        base.OnDisconnectedAsync(exception);

        ConnectionsHandler.Disconnected(Context.ConnectionId);

        Participant? participant = ConnectionsHandler.FindParticipant(Context.ConnectionId);
        if (participant != null)
            // Is participant
            LeaveRoom(participant.RoomId);
    }






    // Send to the client with specified clientId
    public async Task SendClient(string id, Message message)
    {
        await Clients.Client(id).SendAsync(message.Callback, message);
    }

    // Send to the user with specified identifier
    public async Task SendUser(string id, Message message)
    {
        await Clients.User(id).SendAsync(message.Callback, message);
    }

    // Send to clients with specified clientIds
    public async Task SendClientGroup(string[] ids, Message message)
    {
        await Clients.Clients(ids).SendAsync(message.Callback, message);
    }

    // Send to users with specified identifiers
    public async Task SendUserGroup(string[] ids, Message message)
    {
        await Clients.Users(ids).SendAsync(message.Callback, message);
    }

    // Send to all others
    public async Task SendOthers(Message message)
    {
        await Clients.Others.SendAsync(message.Callback, message);
    }



    private Guid GetClientId()
    {
        /*_logger.Inform(JsonSerializer.Serialize(Context.User,
            options: new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            })
        );*/

        /*var contextFeature = (IHttpContextFeature)Context.Features[typeof(IHttpContextFeature)];
        if (contextFeature is null)
            return default;
        var httpContext = contextFeature.HttpContext;
        if (httpContext is null)
            return default;

        _logger.Inform(JsonSerializer.Serialize(httpContext.User,
            options: new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            })
        );
        _logger.Inform(JsonSerializer.Serialize(httpContext.Request.Headers,
            options: new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            })
        );
        _logger.Inform(httpContext.Request.Cookies["Bearer"]);
        */

        /*foreach (Claim claim in Context.User.Claims)
            if (claim.Type == ClaimTypes.NameIdentifier)
                if (Guid.TryParse(claim.Value, out Guid clientId))
                    return clientId;*/

        _logger.Inform("Connected: " + Context.UserIdentifier);
        if (Guid.TryParse(Context.UserIdentifier, out var clientId))
            return clientId;
        return default;
    }
}
