using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;

namespace BiblioContenidos_2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Message = "Welcome to ASP.NET MVC!";
            DataClasses1DataContext db = new DataClasses1DataContext();
            
            List<UltimoLibroPub> ListaLibros = db.Contenidos.Where(p=>p.Tipo=="Libro").Select(a => 
                new UltimoLibroPub {
                    Nick = a.Usuario.aspnet_User.UserName,
                    Descripcion = a.Titulo,
                    Fecha = a.FechaPublicacion
            }).OrderByDescending(f=>f.Fecha).Take(10).ToList();

            ViewBag.ListaLibros = ListaLibros;
            
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
