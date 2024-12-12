using GamingPlatform6.Data;
using GamingPlatform6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GamingPlatform6.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Affiche le formulaire
        [HttpGet]
        public IActionResult AddGame()
        {
            return View();
        }

        // POST: Ajoute un utilisateur
        [HttpPost]
        public IActionResult AddGame(Game game)
        {
            if (ModelState.IsValid)
            {
                // Vérifier si le nom d'utilisateur existe déjà
                bool isNameTaken = _context.Games.Any(u => u.GameName == game.GameName);
                if (isNameTaken)
                {
                    ViewBag.ErrorMessage = "Ce nom de jeu est déjà pris. Veuillez en choisir un autre.";
                    return View();
                }

                // Ajouter l'utilisateur à la base de données
                _context.Games.Add(game);
                _context.SaveChanges();

                // Rediriger vers une autre page (par exemple, liste des utilisateurs)
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Action pour afficher le formulaire
        [HttpGet]
        public IActionResult LogAction()
        {
            // Charger les jeux disponibles pour la liste déroulante
            ViewBag.Games = _context.Games.ToList();

            return View();
        }

        // Action pour enregistrer l'action
        [HttpPost]
        public IActionResult LogAction(ActionLog actionLog)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Veuillez remplir tous les champs correctement.";
                ViewBag.Games = _context.Games.ToList(); // Recharger les jeux si erreur
                return View(actionLog);
            }

            // Vérification de l'existence des entités associées
            var gameExists = _context.Games.Any(g => g.GameName == actionLog.GameId);
            var userExists = _context.Users.Any(u => u.UserName == actionLog.UserId);

            if (!gameExists)
            {
                ViewBag.ErrorMessage = "Le jeu sélectionné n'existe pas.";
                ViewBag.Games = _context.Games.ToList();
                return View(actionLog);
            }

            if (!userExists)
            {
                ViewBag.ErrorMessage = "L'utilisateur sélectionné n'existe pas.";
                ViewBag.Games = _context.Games.ToList();
                return View(actionLog);
            }

            // Ajouter le log d'action dans la base de données
            _context.ActionLogs.Add(actionLog);
            _context.SaveChanges();

            return RedirectToAction("LogAction"); // Rediriger vers la même page après succès
        }

        // Exemple d'action pour afficher la liste des utilisateurs
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    var games = _context.Games.ToList();
        //    return View(games);
        //}
    }
}
