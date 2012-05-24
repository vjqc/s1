using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;
using System.IO;

namespace BiblioContenidos_2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ViewBag.Message = "Welcome to ASP.NET MVC!";
            DataClasses1DataContext db = new DataClasses1DataContext();
            
            List<UltimoLibroPub> ListaLibros = db.Contenidos.Where(p=>p.Tipo=="Libro" && p.Estado=="Aceptado").Select(a => 
                new UltimoLibroPub {
                    //Portada = Path.Combine(Server.MapPath("~/App_Data/Uploads/Img"), db.Libros.Single( t => t.IdContenido==a.Id).Portada ),
                    Portada = db.Libros.Single(t => t.IdContenido == a.Id).Portada,
                    Nick = a.Usuario.aspnet_User.UserName,
                    Karma = (int)a.Usuario.Karma,
                    Descripcion = a.Titulo,
                    Fecha = a.FechaPublicacion,
                    IdContenido = a.Id,
                    IdUsuario = a.IdUsuario
            }).OrderByDescending(f=>f.Fecha).Take(10).ToList();

            ViewBag.ListaLibros = ListaLibros;
            
            return View();
        }
        
        public ActionResult ObtenerUrlImagen(string id)
        {
            if (String.IsNullOrEmpty(id)) id = "";
            return File(Path.Combine(Server.MapPath("~/App_Data/Uploads/Img"), id), "image/jpg");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
