using Microsoft.AspNetCore.Mvc;
using NetCoreSeguridadEmpleados.Filters;
using NetCoreSeguridadEmpleados.Models;
using NetCoreSeguridadEmpleados.Repositories;
using System.Security.Claims;

namespace NetCoreSeguridadEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;

        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> Details(int idEmpleado)
        {
            Empleado empleado = await this.repo.FindEmpleadoAsync(idEmpleado);
            return View(empleado);
        }
        [AuthorizeEmpleados]
        public async Task<IActionResult> PerfilEmpleado()
        {
            
            return View();
        }
        [AuthorizeEmpleados]
        public async Task<IActionResult> Compis()
        {
            //RECUPERAMOS EL CLAIM DEL USUARIO VALIDADO
            string dato = HttpContext.User.FindFirstValue("Departamento");
            int idDepartamento = int.Parse(dato);
            List<Empleado> empleados = await this.repo.GetEmpleadosDepartamentoAsync(idDepartamento);
            return View(empleados);
        }
    }
}
