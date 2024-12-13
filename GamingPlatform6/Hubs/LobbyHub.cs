using GamingPlatform6.Data;
using GamingPlatform6.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace GamingPlatform6.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public LobbyHub(ApplicationDbContext context)
        {
            _context = context; // Injection de dépendance
        }

#region LobbyHub

        // Utiliser un dictionnaire concurrent pour stocker les informations des lobbies
        private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> Lobbies = new();
        private static readonly int MaxPlayersInLobby = 2;  // Limite des joueurs dans un lobby
        
        // Méthode appelée lorsqu'un client rejoint un lobby
        public async Task JoinLobby(string lobbyId, string user)
        {
            // Vérifier si le lobby existe déjà, sinon en créer un
            if (!Lobbies.ContainsKey(lobbyId))
            {
                Lobbies[lobbyId] = new ConcurrentBag<string>();
            }

            // Vérifier si le lobby est déjà plein
            if (Lobbies[lobbyId].Count >= MaxPlayersInLobby)
            {
                // Si le lobby est plein, informer uniquement le joueur concerné
                await Clients.Caller.SendAsync("ReceiveMessage", "System", $"*C1*-Le lobby {lobbyId} est déjà complet.");
                return; // Ne pas ajouter le joueur si le lobby est plein
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
            
            //TicTacToe logique
            if (!Games.ContainsKey(lobbyId))
            {
                Games[lobbyId] = new TicTacToeGame();
            }

            var game = Games[lobbyId];

            // Associer le joueur à un symbole (X ou O) s'il reste de la place
            if (!game.Players.ContainsKey(user))
            {
                var symbol = game.Players.Count == 0 ? "X" : (game.Players.Count == 1 ? "O" : null);

                if (symbol == null)
                {
                    await Clients.Caller.SendAsync("ErrorMessage", "Le lobby est complet.");
                    return;
                }

                game.Players[user] = symbol;
                game.Scores[user] = 9; // Initialiser le score à 9

                // Définir le premier joueur comme joueur actif
                if (game.CurrentPlayer == null)
                {
                    game.CurrentPlayer = user;
                }
            }
        }

        // Méthode appelée lorsqu'un client quitte un lobby
        public async Task LeaveLobby(string lobbyId, string user)
        {
            // Vérifier si le lobby existe
            if (Lobbies.ContainsKey(lobbyId))
            {
                // Retirer l'utilisateur du lobby
                var updatedLobby = new ConcurrentBag<string>(Lobbies[lobbyId].Where(u => u != user));
                Lobbies[lobbyId] = updatedLobby;

                // Retirer l'utilisateur du groupe SignalR du lobby
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);

                // Informer les autres membres du lobby que l'utilisateur a quitté
                await Clients.Group(lobbyId).SendAsync("ReceiveMessage", "System", $"{user} a quitté le lobby.");

                // Envoyer à tous les membres du lobby le nombre actuel de personnes en ligne
                var onlineCount = Lobbies[lobbyId].Count;
                await Clients.Group(lobbyId).SendAsync("UpdateOnlineCount", onlineCount);

                // Vérifier s'il reste un joueur dans le lobby
                if (updatedLobby.Count == 1)
                {
                    // L'autre joueur gagne automatiquement
                    var remainingPlayer = updatedLobby.First();
                    var game = Games[lobbyId];

                    // Terminer le jeu et déclarer l'autre joueur comme gagnant
                    game.CurrentPlayer = null;  // Fin du jeu

                    var winnerMessage = $"{remainingPlayer} a gagné car l'autre joueur a quitté la partie.";
                    await Clients.Group(lobbyId).SendAsync("GameOver", winnerMessage);

                    // Sauvegarder le score du gagnant
                    var winnerScore = new Score
                    {
                        UserId = remainingPlayer,
                        GameId = "morpion",
                        Points = game.Scores[remainingPlayer],
                        AchievedAt = DateTime.Now
                    };

                    _context.Scores.Add(winnerScore);
                    await _context.SaveChangesAsync();

                    // Supprimer le jeu après la fin
                    Games.TryRemove(lobbyId, out _);
                }
                
                //TicTacToe logique
                if (Games.ContainsKey(lobbyId))
                {
                    var game = Games[lobbyId];

                    // Supprimer le joueur du jeu
                    if (game.Players.ContainsKey(user))
                    {
                        game.Players.Remove(user, out _);
                    }

                    // Si aucun joueur n'est présent, supprimer le jeu
                    if (game.Players.Count == 0)
                    {
                        Games.TryRemove(lobbyId, out _);
                    }
                    else
                    {
                        // Passer au tour du joueur restant
                        game.CurrentPlayer = game.Players.Keys.FirstOrDefault();
                    }
                }
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
            // Nous ne faisons rien ici pour éviter d'ajouter l'utilisateur deux fois (cela se fait uniquement via JoinLobby)
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

#endregion

#region TicTacToe

        // Gestion de l'état du jeu (par lobby)
        private static readonly ConcurrentDictionary<string, TicTacToeGame> Games = new();

        // Méthode appelée lorsqu'un joueur effectue un mouvement
        public async Task MakeMove(string lobbyId, int row, int col)
        {
            // Récupérer le nom d'utilisateur depuis le cookie
            var user = Context.GetHttpContext().Request.Cookies["username"];

            if (!Games.ContainsKey(lobbyId) || Games[lobbyId] == null)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Aucun jeu actif dans ce lobby.");
                return;
            }

            if (string.IsNullOrEmpty(user))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Utilisateur non authentifié.");
                return;
            }

            // Vérifier si le jeu existe pour ce lobby
            if (!Games.ContainsKey(lobbyId))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Aucun jeu en cours pour ce lobby.");
                return;
            }

            var game = Games[lobbyId];

            // Vérifier si c'est le tour du joueur
            if (game.CurrentPlayer != user)
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Ce n'est pas votre tour.");
                return;
            }

            // Effectuer le mouvement et vérifier si c'est valide
            var result = game.MakeMove(row, col, user);

            if (!result.Success)
            {
                await Clients.Caller.SendAsync("ErrorMessage", result.Message);
                return;
            }

            ActionLog actionLog = new ActionLog{
                GameId = "morpion",
                Action = "click",
                Description = "case (" + row + "," + col + ")",
                Partie = 1,
                UserId = user
            };
            
            _context.ActionLogs.Add(actionLog);
            _context.SaveChanges();

            // Mettre à jour la grille pour tous les joueurs
            await Clients.Group(lobbyId).SendAsync("UpdateBoard", game.Board);

            // Vérifier si un joueur a gagné ou si c'est un match nul
            if (result.GameOver)
            {
                var message = result.Winner != null ? $"{result.Winner} a gagné !" : "Match nul !";
                await Clients.Group(lobbyId).SendAsync("GameOver", message);

                // Sauvegarder le score du gagnant dans la base de données
                if (result.Winner != null)
                {
                    var winnerScore = new Score
                    {
                        UserId = result.Winner,
                        GameId = "morpion",
                        Points = game.Scores[result.Winner],
                        AchievedAt = DateTime.Now
                    };
                    
                    // Ajouter à la base de données
                    _context.Scores.Add(winnerScore);
                    await _context.SaveChangesAsync();
                }
                Games.TryRemove(lobbyId, out _); // Supprimer le jeu après la fin
            }
        }

    // Classe représentant l'état d'un jeu de morpion
    public class TicTacToeGame
    {
        public string[][] Board { get; private set; } = new string[3][]
        {
            new string[3], // Ligne 0
            new string[3], // Ligne 1
            new string[3]  // Ligne 2
        };

        public string CurrentPlayer { get; set; }
        public ConcurrentDictionary<string, string> Players { get; private set; } = new();
        public ConcurrentDictionary<string, int> Scores { get; private set; } = new ConcurrentDictionary<string, int>();


        public (bool Success, string Message, bool GameOver, string Winner) MakeMove(int row, int col, string user)
        {
            if (!Players.ContainsKey(user))
            {
                return (false, "Vous n'êtes pas dans le jeu.", false, null);
            }

            if (Board[row][col] != null)
            {
                return (false, "Case déjà occupée.", false, null);
            }

            if (CurrentPlayer != user)
            {
                return (false, "Ce n'est pas votre tour.", false, null);
            }

            // Associer le symbole au joueur
            var symbol = Players[user];
            Board[row][col] = symbol;

            // Décrémenter le score
            if (Scores.ContainsKey(user))
            {
                Scores[user]--;
            }

            // Vérifier si le joueur a gagné
            if (CheckWin(symbol))
            {
                return (true, null, true, user);
            }

            // Vérifier si la grille est pleine
            if (Board.All(rowData => rowData.All(cell => cell != null)))
            {
                return (true, null, true, null); // Match nul
            }

            // Passer au joueur suivant
            CurrentPlayer = Players.Keys.FirstOrDefault(p => p != user);
            return (true, null, false, null);
        }

        private bool CheckWin(string symbol)
        {
            // Vérifier les lignes, colonnes et diagonales
            return Enumerable.Range(0, 3).Any(i => (Board[i][0] == symbol && Board[i][1] == symbol && Board[i][2] == symbol) || // Lignes
                                                (Board[0][i] == symbol && Board[1][i] == symbol && Board[2][i] == symbol)) || // Colonnes
                (Board[0][0] == symbol && Board[1][1] == symbol && Board[2][2] == symbol) || // Diagonale principale
                (Board[0][2] == symbol && Board[1][1] == symbol && Board[2][0] == symbol);   // Diagonale secondaire
        }
    }

#endregion
    }
}
