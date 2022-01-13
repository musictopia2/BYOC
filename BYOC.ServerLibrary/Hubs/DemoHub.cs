namespace BYOC.ServerLibrary.Hubs;
public class DemoHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}