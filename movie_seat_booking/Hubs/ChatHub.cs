using Microsoft.AspNet.SignalR;


namespace movie_seat_booking.Hubs
{
      public class ChatHub : Hub
        {
            // Broadcasts a message to all connected clients
            public void SendMessage(string user, string message)
            {
                // Send message to all clients
                Clients.All.broadcastMessage(user, message);
            }
        }
    
}
