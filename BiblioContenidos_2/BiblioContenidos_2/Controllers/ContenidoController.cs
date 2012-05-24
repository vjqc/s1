using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;

namespace BiblioContenidos_2.Controllers
{
    public class ContenidoController : Controller
    {
        //
        // GET: /Contenido/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Usuario")]
        public ActionResult MeGusta(string id)
        {
            int IdContenido = Convert.ToInt32(id);
            DataClasses1DataContext db = new DataClasses1DataContext();
            string NombreUsuario = User.Identity.Name.ToString();
            int IdUsuario = db.Usuarios.Single(u => u.aspnet_User.UserName == NombreUsuario).Id;

            //Verificar si ya le gusta a pepito el contenido x
            int con = db.Gustas.Count(p => p.IdContenido == IdContenido && p.IdUsuario == IdUsuario);

            if (con == 0)
            {
                Gusta g = new Gusta()
                {
                    IdContenido = IdContenido,
                    IdUsuario = IdUsuario,
                    MeGusta = 1,
                    Fecha = DateTime.Now
                };
                db.Gustas.InsertOnSubmit(g);

                int IdPublicador = db.Contenidos.Single(c => c.Id == IdContenido).IdUsuario;
                Usuario usr = db.Usuarios.Single(q => q.Id == IdPublicador);
                usr.Karma++;

                db.SubmitChanges();
            }

            return Redirect("/DetalleLibro/Index/"+id);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult AprobarContenido(string id)
        {
            int IdContenido = Convert.ToInt32(id);
            DataClasses1DataContext db = new DataClasses1DataContext();
            Contenido contenido = db.Contenidos.Single(c => c.Id == IdContenido);
            contenido.Estado = "Aceptado";
            db.SubmitChanges();

            return Redirect("/Moderacion/ModeracionContenidos");
        }

    }
}
