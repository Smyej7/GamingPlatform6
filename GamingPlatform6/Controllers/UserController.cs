using GamingPlatform6.Data;
using GamingPlatform6.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GamingPlatform6.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Affiche le formulaire
        [HttpGet]
        public IActionResult AddUser()
        {
            return View();
        }

        // POST: Ajoute un utilisateur
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                // Vérifier si le nom d'utilisateur existe déjà
                bool isNameTaken = _context.Users.Any(u => u.UserName == user.UserName);
                if (isNameTaken)
                {
                    ViewBag.ErrorMessage = "Ce nom d'utilisateur est déjà pris. Veuillez en choisir un autre.";
                    return View();
                }

                // Ajouter l'utilisateur à la base de données
                _context.Users.Add(user);
                _context.SaveChanges();

                // Stocker le nom d'utilisateur dans un cookie
                Response.Cookies.Append("username", user.UserName, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30), // Le cookie expire après 30 jours
                    HttpOnly = false, // Le cookie ne sera accessible que par le serveur
                    Secure = true // Le cookie est envoyé uniquement via HTTPS
                });

                // Rediriger vers une autre page (par exemple, liste des utilisateurs)
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Action de déconnexion
        [HttpPost]
        public IActionResult Logout()
        {
            // Supprimer le cookie 'username' pour déconnecter l'utilisateur
            Response.Cookies.Delete("username");

            // Rediriger l'utilisateur vers la page d'accueil ou la page de connexion
            return RedirectToAction("Index", "Home");
        }

        // Exemple d'action pour afficher la liste des utilisateurs
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    var users = _context.Users.ToList();
        //    return View(users);
        //}
    }
}
