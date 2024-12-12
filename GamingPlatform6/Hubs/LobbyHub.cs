using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace GamingPlatform6.Hubs
{
    public class LobbyHub : Hub
    {
        // Utiliser un dictionnaire concurrent pour stocker les informations des lobbies
        private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> Lobbies = new();

        // Méthode appelée lorsqu'un client rejoint un lobby
        public async Task JoinLobby(string lobbyId, string user)
        {
            // Vérifier si le lobby existe déjà, sinon en créer un
            if (!Lobbies.ContainsKey(lobbyId))
            {
                Lobbies[lobbyId] = new ConcurrentBag<string>();
            }

            // Vérifier si l'utilisateur est déjà dans le lobby
            if (Lobbies[lobbyId].Contains(user))
            {
                return; // Si l'utilisateur est déjà dans le lobby, on ne fait rien
            }

            // Ajouter l'utilisateur au lobby
            Lobbies[lobbyId].Add(user);

            // Ajouter l'utilisateur au groupe SignalR du lobby
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);

            // Informer les autres membres du lobby que l'utilisateur a rejoint
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage", "System", $"{user} a rejoint le lobby.");

            // Envoyer à tous les membres du lobby le nombre actuel de personnes en ligne
            var onlineCount = Lobbies[lobbyId].Count;
            await Clients.Group(lobbyId).SendAsync("UpdateOnlineCount", onlineCount);
        }

        // Méthode appelée lorsqu'un client quitte un lobby
        public async Task LeaveLobby(string lobbyId, string user)
        {
            // Vérifier si le lobby existe
            if (Lobbies.ContainsKey(lobbyId))
            {
                // Retirer l'utilisateur du lobby
                Lobbies[lobbyId] = new ConcurrentBag<string>(Lobbies[lobbyId].Where(u => u != user));

                // Retirer l'utilisateur du groupe SignalR du lobby
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);

                // Informer les autres membres du lobby que l'utilisateur a quitté
                await Clients.Group(lobbyId).SendAsync("ReceiveMessage", "System", $"{user} a quitté le lobby.");

                // Envoyer à tous les membres du lobby le nombre actuel de personnes en ligne
                var onlineCount = Lobbies[lobbyId].Count;
                await Clients.Group(lobbyId).SendAsync("UpdateOnlineCount", onlineCount);
            }
        }

        // Méthode pour envoyer un message général à tous les membres du lobby
        public async Task SendMessage(string lobbyId, string user, string message)
        {
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage", user, message);
        }

        // Méthode appelée lorsqu'un client se connecte
        public override async Task OnConnectedAsync()
        {
            // Ne pas ajouter l'utilisateur ici, car il sera ajouté depuis le frontend
            await base.OnConnectedAsync();
        }

        // Méthode appelée lorsqu'un client se déconnecte
        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var lobbyId = Context.GetHttpContext().Request.Query["lobbyId"];
            var user = Context.GetHttpContext().Request.Cookies["username"] ?? "Anonyme"; // Récupérer le nom depuis le cookie

            if (!string.IsNullOrEmpty(lobbyId))
            {
                await LeaveLobby(lobbyId, user);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
