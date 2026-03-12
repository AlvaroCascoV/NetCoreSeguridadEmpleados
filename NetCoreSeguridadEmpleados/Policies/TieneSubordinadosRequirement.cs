using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NetCoreSeguridadEmpleados.Models;
using NetCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NetCoreSeguridadEmpleados.Policies
{
    public class TieneSubordinadosRequirement : AuthorizationHandler<TieneSubordinadosRequirement>, IAuthorizationRequirement
    {
        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, TieneSubordinadosRequirement requirement)
        {
            var filterContext = context.Resource as AuthorizationFilterContext;
            var httpContext = filterContext.HttpContext;
            //necesito el repo
            RepositoryEmpleados repo = httpContext.RequestServices.GetService<RepositoryEmpleados>();

            if (context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier) == false)
            {
                context.Fail();
            }
            else
            {
                string idString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                int id = int.Parse(idString);
                List<Empleado> subordinados = await repo.FindSubordinadosAsync(id);
                if(subordinados.Count > 0)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }
    }
}
