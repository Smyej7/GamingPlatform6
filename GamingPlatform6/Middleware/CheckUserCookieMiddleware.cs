using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GamingPlatform6.Middleware
{
    public class CheckUserCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckUserCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Vérifier si le cookie "username" existe
            if (!context.Request.Cookies.ContainsKey("username"))
            {
                // Si le cookie n'existe pas, rediriger vers la page d'ajout de l'utilisateur
                if (context.Request.Path != "/User/AddUser")
                {
                    context.Response.Redirect("/User/AddUser");
                    return;
                }
            }

            // Passer au middleware suivant
            await _next(context);
        }
    }
}
