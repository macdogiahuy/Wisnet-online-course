using CourseHub.API.Realtime.Services;
using CourseHub.API.Realtime.Services.Stream;
using Microsoft.AspNetCore.SignalR;

namespace CourseHub.API.Realtime.Hubs;

public partial class AppHub : Hub
{
    public override Task OnConnectedAsync()
    {
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
}
