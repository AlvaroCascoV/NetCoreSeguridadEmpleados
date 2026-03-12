using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace NetCoreSeguridadEmpleados.Filters
{
    public class AuthorizeEmpleadosAttribute :  AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //POR AHORA, SOLAMENTE NOS INTERESA
            //VALIDAR SI EXISTE O NO EL EMPLEADO
            var user = context.HttpContext.User;

            //queremos que al hacer login, devuelva a la pagina donde quiere acceder, no al perfil siempre
            //NECESITAMOS EL ACTION Y EL CONTROLLER DE DONDE
            //EL USUARIO HA PULSADO.
            //PARA ELLO, TENEMOS RouteValues QUE CONTIENE 
            //LA INFORMACION
            //RouteData["contoller"]
            //RouteData["action"]
            //RouteData["idalgo"] (si lo hubiera)
            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            var id = context.RouteData.Values["id"];

            ITempDataProvider provider = context.HttpContext.RequestServices.GetService<ITempDataProvider>();
            //ESTA CLASE CONTIENE EL TEMPDATA DE NUESTRA APP
            var tempData = provider.LoadTempData(context.HttpContext);
            //ALMACENAMOS LA INFORMACION
            tempData["controller"] = controller;
            tempData["action"] = action;
            //DEBEMOS PREGUNTAR POR EL ID
            if (id != null)
            {
                tempData["id"] = id.ToString();
            }
            else
            {
                //ELIMINAMOS LA CLAVE PARA QUE NO SE QUEDE ENTRE PETICIONES BUSCANDO UN ID QUE NO EXISTE
                tempData.Remove("id");
            }
            //REASIGNAMOS EL TEMPDATA PARA NUESTRA APP
            provider.SaveTempData(context.HttpContext, tempData);


            if (user.Identity.IsAuthenticated == false)
            {
                context.Result = GetRoute("Managed", "Login");
            }


            //else
            //{
            //    //COMPROBAMOS LOS ROLES.
            //    //TENEMOS EN CUENTA MAYUSCULAS/MINUSCULAS
            //    if(user.IsInRole("PRESIDENTE") == false
            //        && user.IsInRole("DIRECTOR") == false
            //        && user.IsInRole("ANALISTA") == false)
            //    {
            //        context.Result = GetRoute("Managed", "ErrorAcceso");
            //    }
            //}
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
