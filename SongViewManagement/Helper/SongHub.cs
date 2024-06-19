using Microsoft.AspNetCore.SignalR;

namespace SongViewManagement.Helper
{
    public class SongHub : Hub
    {
        public async Task SendSongUpdate(string action, int songId)
        {
            await Clients.All.SendAsync("ReceiveSongUpdate", action, songId);
        }
    }
}
