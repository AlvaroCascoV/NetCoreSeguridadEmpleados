using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using NetCoreSeguridadEmpleados.Models;
using NetCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCoreSeguridadEmpleados.Controllers
{
    public class ManagedController : Controller
    {
        private RepositoryEmpleados repo;
        public ManagedController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            int idEmpleado = int.Parse(password);
            Empleado empleado = await this.repo.LogInEmpleadoAsync(username, idEmpleado);
            if(empleado != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, 
                    ClaimTypes.Role
                    );

                //EMPLEADO ARROYO: 7499 SERA EL ADMIN
                if(empleado.IdEmpleado == 7499)
                {
                    Claim claimAdmin = new Claim("Admin", "Soy el amo de la empresa");
                    identity.AddClaim(claimAdmin);
                }

                Claim claimName = new Claim(ClaimTypes.Name, username);
                identity.AddClaim(claimName);

                //podemos añadir claims personalizados o usar predefinidos para lo que queramos
                Claim claimId = new Claim(ClaimTypes.NameIdentifier, empleado.IdEmpleado.ToString());
                identity.AddClaim(claimId);
                //COMO ROLE, UTILIZAMOS EL OFICIO
                Claim claimRole = new Claim(ClaimTypes.Role, empleado.Oficio);
                identity.AddClaim(claimRole);
                //claim personalizado para el salario
                Claim claimSalario = new Claim("Salario", empleado.Salario.ToString());
                identity.AddClaim(claimSalario);
                Claim claimDepartamento = new Claim("Departamento", empleado.IdDepartamento.ToString());
                identity.AddClaim(claimDepartamento);

                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal
                    );

                //redirigimos a donde quiere el user despues del login
                string controller = TempData["controller"].ToString();
                string action = TempData["action"].ToString();

                return RedirectToAction(action, controller);
            }
            else
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
