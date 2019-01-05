using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
namespace DatingApp.API.helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultsContext = await next();
            var userId = int.Parse(resultsContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo = resultsContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            var user = await repo.GetUser(userId, false);
            if (user != null) {
                user.LastActive = DateTime.Now;
                await repo.SaveAll();
            }
        }
    }
}