using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Sekka.PL.Hubs
{
    [Authorize(Roles = "Driver,Passenger")]
    public class TripHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User?.IsInRole("Driver") == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Drivers");
                Console.WriteLine($"Driver connected: {Context.UserIdentifier}");
            }
            else if (Context.User?.IsInRole("Passenger") == true)
                Console.WriteLine($"Passenger connected: {Context.UserIdentifier}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User?.IsInRole("Driver") == true)
            {
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId,
                    "Drivers");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}