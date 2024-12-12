using GamingPlatform6.Data;
using GamingPlatform6.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GamingPlatform6.Controllers
{
    public class LobbyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LobbyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Méthode GET pour afficher la page de création du lobby
        [HttpGet]
        public IActionResult CreateLobby()
        {
            return View();
        }

        // Méthode POST pour créer un lobby
        [HttpPost]
        public IActionResult CreateLobby([FromForm] string hostUserId)
        {
            if (string.IsNullOrEmpty(hostUserId))
            {
                return BadRequest("Nom d'utilisateur requis.");
            }

            // Afficher les informations du formulaire dans la console (pour le test)
            Console.WriteLine($"Nom de l'hôte : {hostUserId}");

            // Générer un identifiant unique pour le lobby
            var lobbyId = Guid.NewGuid().ToString();

            // Créer le lobby
            var lobby = new Lobby
            {
                LobbyId = lobbyId,
                HostUserId = hostUserId
            };

            _context.Lobbies.Add(lobby);
            _context.SaveChanges();

            // Générer le lien du lobby
            var lobbyLink = Url.Action("JoinLobby", "Lobby", new { lobbyId }, Request.Scheme);

            // Afficher le lien du lobby dans la console (pour le test)
            Console.WriteLine($"Lien du lobby : {lobbyLink}");

            // Retourner le lien du lobby à la vue
            ViewData["LobbyLink"] = lobbyLink;
            return View();
        }

        // Méthode pour rejoindre un lobby
        [HttpGet]
        public IActionResult JoinLobby(string lobbyId)
        {
            var lobby = _context.Lobbies.FirstOrDefault(l => l.LobbyId == lobbyId);
            if (lobby == null)
            {
                return NotFound("Lobby non trouvé.");
            }

            return View("LobbyRoom", lobby);
        }
    }
}
