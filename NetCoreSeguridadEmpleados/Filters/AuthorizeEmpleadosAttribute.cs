using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCoreSeguridadEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //POR AHORA, SOLAMENTE NOS INTERESA
            //VALIDAR SI EXISTE O NO EL EMPLEADO
            var user = context.HttpContext.User;
            if (user.Identity.IsAuthenticated == false)
            {
                context.Result = GetRoute("Managed", "Login");
            }
        }
        
        //EN ALGUN MOMENTO TENDREMOS MAS REDIRECCIONES QUE SOLO
        //A LOGIN, POR LO QUE CREAMOS UN METODO PARA REDIRECCIONAR
        private RedirectToRouteResult GetRoute(string controller, string action)
        { 
            RouteValueDictionary ruta = new RouteValueDictionary(new
            {
                controller = controller,
                action = action
                //,id= algo (podemos pasar params asi)
            });
            RedirectToRouteResult result = new RedirectToRouteResult(ruta);
            return result;
        }
    }
}
