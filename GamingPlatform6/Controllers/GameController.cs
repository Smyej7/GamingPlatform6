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

        // Exemple d'action pour afficher la liste des utilisateurs
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    var games = _context.Games.ToList();
        //    return View(games);
        //}
    }
}
