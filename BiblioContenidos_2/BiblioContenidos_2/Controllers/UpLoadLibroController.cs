using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;

namespace BiblioContenidos_2.Controllers
{
    public class UpLoadLibroController : Controller
    {
        //
        // GET: /UpLoadLibro/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(CargarLibro model)
        {
            if (ModelState.IsValid)
            {
                DataClasses1DataContext db = new DataClasses1DataContext();

                string NombreUsuario = User.Identity.Name.ToString();
                System.Guid IdUs = db.aspnet_Users.Where(a => a.UserName == NombreUsuario).Select(a => a.UserId).ToArray()[0];
                int IdUsuario = db.Usuarios.Where(a => a.UserId == IdUs).Select(a => a.Id).ToArray()[0];

                model.Fecha = DateTime.Now;

                Contenido contenido = new Contenido() { 
                    FechaPublicacion = model.Fecha,
                    IdUsuario = IdUsuario,
                    Tipo = "Libro",
                    Titulo = model.Tema,
                    Descripcion = model.Descripcion,
                    IdCategoria = 1,
                    UrlReal = "",
                    UrlVirtual = "",
                    Estado = "Pendiente"
                };
                db.Contenidos.InsertOnSubmit(contenido);
                db.SubmitChanges();

                //Insertar en la tabla Libros
                int IdConte = db.Contenidos.Where(p => p.IdUsuario==IdUsuario).Where(q => q.FechaPublicacion==model.Fecha).Select(r => r.Id).ToArray()[0];
                Libro libro = new Libro()
                {
                    Autor = model.Autor,
                    Portada = model.Portada,
                    Indice = "",
                    AnhoPublicacion = model.Anho.ToString(),
                    IdContenido = IdConte
                };
                db.Libros.InsertOnSubmit(libro);
                db.SubmitChanges();
                //ViewBag.id = IdConte;
                
            }
            return View();
        }
    }
}
