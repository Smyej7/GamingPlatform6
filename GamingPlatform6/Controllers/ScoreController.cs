using GamingPlatform6.Data;
using GamingPlatform6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GamingPlatform6.Controllers
{
    public class ScoreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScoreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action pour afficher le formulaire d'ajout de score
        [HttpGet]
        public IActionResult AddScore()
        {
            // Récupérer les jeux disponibles pour les afficher dans le formulaire
            var games = _context.Games.ToList();
            ViewBag.Games = games;

            return View();
        }

        [HttpPost]
        public IActionResult AddScore(Score score)
        {
            // Affichage des données envoyées depuis la vue dans la console avant de vérifier ModelState
            Console.WriteLine("Données envoyées depuis la vue :");

            // Afficher chaque propriété du modèle Score
            Console.WriteLine($"GameId: {score.GameId}");
            Console.WriteLine($"Points: {score.Points}");
            Console.WriteLine($"UserId: {score.UserId}");

            // Vérification des erreurs dans ModelState
            Console.WriteLine("Erreurs de ModelState (si présentes) :");
            foreach (var entry in ModelState)
            {
                Console.WriteLine($"Clé: {entry.Key}");
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Erreur: {error.ErrorMessage}");
                }
            }

            // Vérification de la validité du modèle
            if (ModelState.IsValid)
            {
                // Récupérer l'objet Game à partir de GameId
                var game = _context.Games.FirstOrDefault(g => g.GameName == score.GameId);
                if (game == null)
                {
                    ViewBag.ErrorMessage = "Le jeu sélectionné n'existe pas.";
                    return View(score);
                }

                // Récupérer l'objet User à partir de UserId
                var user = _context.Users.FirstOrDefault(u => u.UserName == score.UserId);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "Utilisateur non trouvé.";
                    return View(score);
                }

                // Ajouter le score à la base de données
                _context.Scores.Add(score);
                _context.SaveChanges(); // Sauvegarder les changements

                Console.WriteLine("Score ajouté à la base de données.");

                return RedirectToAction("Index", "Home"); // Rediriger vers la page d'accueil après l'ajout du score
            }

            // Si le modèle n'est pas valide, renvoyer la vue avec les erreurs
            return View(score);
        }

        // Action pour afficher tous les scores
        public IActionResult ListScores()
        {
            // Récupérer tous les scores de la base de données et les trier par ordre décroissant des points
            var scores = _context.Scores
                .OrderByDescending(s => s.Points)  // Tri par points décroissants
                .ToList();

            return View(scores);  // Passer les scores à la vue
        }
    }
}
