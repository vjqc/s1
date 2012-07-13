using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;
using System.IO;

//using CodeKicker.BBCode;
///using djsolid.BBCodeHelper;

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

            List<UltimoArticuloPub> ListaArticulos = db.Contenidos.Where(p => p.Tipo == "Articulo" && p.Estado == "Aceptado").Select(a =>
                new UltimoArticuloPub
                {
                    Nick = a.Usuario.aspnet_User.UserName,
                    Karma = (int)a.Usuario.Karma,
                    Titulo = a.Titulo,
                    Descripcion = a.Descripcion.Substring(0, 25) + " ... ",
                    Fecha = a.FechaPublicacion,
                    IdContenido = a.Id,
                    IdUsuario = a.IdUsuario
                }).OrderByDescending(f => f.Fecha).Take(10).ToList();
            ViewBag.ListaArticulos = ListaArticulos;


            List<UltimoCursoPub> ListaCursos = db.Contenidos.Where(p => p.Tipo == "Curso" && p.Estado == "Aceptado").Select(a =>
                new UltimoCursoPub
                {
                    Nick = a.Usuario.aspnet_User.UserName,
                    Karma = (int)a.Usuario.Karma,
                    Titulo = a.Titulo,
                    Descripcion = a.Descripcion.Substring(0, 25) + " ... ",
                    Fecha = a.FechaPublicacion,
                    IdContenido = a.Id,
                    IdUsuario = a.IdUsuario
                }).OrderByDescending(f => f.Fecha).Take(10).ToList();
            ViewBag.ListaCursos = ListaCursos;

            return View();
        }
        
        public ActionResult ObtenerUrlImagen(string id)
        {
            if (String.IsNullOrEmpty(id)) id = "";
            return File(Path.Combine(Server.MapPath("~/App_Data/Uploads/Img"), id), "image/jpg");
        }

        public ActionResult ObtenerUrlImagen2(string id)
        {
            if (String.IsNullOrEmpty(id)) id = "";
            //return File(Path.Combine(Server.MapPath("~/Content/Uploads/"), id), "image/jpg");
            return File(Path.Combine(Server.MapPath(""), id), "image/jpg");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
