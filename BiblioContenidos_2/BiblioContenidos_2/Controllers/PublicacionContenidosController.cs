using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;

namespace BiblioContenidos_2.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class PublicacionContenidosController : Controller
    {
        //
        // GET: /PublicacionContenidos/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Editor()
        {
            return View();
        }

        public int IdUsuarioActual()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            string NombreUsuario = User.Identity.Name.ToString();
            System.Guid IdUs = db.aspnet_Users.Single(a => a.UserName == NombreUsuario).UserId;
            int IdUsuario = db.Usuarios.Single(a => a.UserId == IdUs).Id;

            return IdUsuario;
        }

        [HttpPost]
        public ActionResult Editor(Contenido info)
        {
            //Contenido c = info;
            if (!String.IsNullOrEmpty(info.Titulo) && !String.IsNullOrEmpty(info.Descripcion))
            {
                DataClasses1DataContext db = new DataClasses1DataContext();
                info.IdUsuario = IdUsuarioActual();
                info.FechaPublicacion = DateTime.Now;
                info.Tipo = "Articulo";
                info.UrlReal = "";
                info.UrlVirtual = "";
                info.Estado = "Pendiente";

                db.Contenidos.InsertOnSubmit(info);
                db.SubmitChanges();
            }
            return View();
        }

        public ActionResult Ver()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            ViewBag.lista = db.Contenidos;
            return View();
        }
    }
}
