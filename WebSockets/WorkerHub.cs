using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;

namespace autenticacion.WebSockets
{
    public class WorkerHub : Hub
    {
        public WorkerHub()
        {
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Init();
        }

        [HubMethodName("Init")]
        public async Task Init()
        {

        }

        public async Task NotifyLogin(string username)
        {
            await Clients.Others.SendAsync("UserLoggedIn", $"{username} has connected.");
            await Clients.Caller.SendAsync("WelcomeMessage", $"Hello {username}, welcome!");
        }

        public async Task Logout(string codice)
        {
            await Clients.All.SendAsync("Logout", codice);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var idSocket = Context.ConnectionId;
            Console.WriteLine(idSocket);
        }

        public async Task NotifySingIn(string codice)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("NotifySingIn", codice);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
