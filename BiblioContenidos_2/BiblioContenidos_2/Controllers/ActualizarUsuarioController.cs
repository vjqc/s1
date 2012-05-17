using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;

namespace BiblioContenidos_2.Controllers
{
    [Authorize(Roles="Usuario")]
    public class ActualizarUsuarioController : Controller
    {
        //
        // GET: /ActualizarUsuario/

        public ActionResult Index()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            string NombreUsuario = User.Identity.Name.ToString();
            System.Guid IdUs = db.aspnet_Users.Where(a => a.UserName == NombreUsuario).Select(a => a.UserId).ToArray()[0];

            string ce = db.aspnet_Memberships.Where(p => p.UserId == IdUs).Select(p => p.Email).ToArray()[0];
            ViewBag.ce = ce;

            Usuario usuario = db.Usuarios.Single(p => p.UserId == IdUs);
            ViewBag.usuario = usuario;

            //Resumen de actividad (Contenidos Publicados)
            //int IdUsuario = usuario.Id;
            ViewBag.Resumen = db.Contenidos.Where(q => q.IdUsuario == usuario.Id);
            
            return View();
        }

        [HttpPost]
        public ActionResult Index(ActualizarUsuario model)
        {
            if (ModelState.IsValid && model!=null)
            {
                DataClasses1DataContext db = new DataClasses1DataContext();
                    
                //ViewBag.NombreUsuario = User.Identity.Name.ToString();
                string NombreUsuario = User.Identity.Name.ToString();
                System.Guid IdUs = db.aspnet_Users.Where(a => a.UserName == NombreUsuario).Select(a => a.UserId).ToArray()[0];

                string ce = db.aspnet_Memberships.Where(p => p.UserId == IdUs).Select(p => p.Email).ToArray()[0];
                ViewBag.ce = ce;

                Usuario usuario = db.Usuarios.Single(p => p.UserId == IdUs);
                ViewBag.usuario = usuario;

                if ( !String.IsNullOrEmpty(model.Avatar) )    usuario.Avatar = model.Avatar;
                if ( !String.IsNullOrEmpty(model.ApPaterno) ) usuario.ApPaterno = model.ApPaterno;
                if ( !String.IsNullOrEmpty(model.ApMaterno) ) usuario.ApMaterno = model.ApMaterno;
                if ( !String.IsNullOrEmpty(model.Nombres) )   usuario.Nombres = model.Nombres;
                if ( !String.IsNullOrEmpty(model.Ubicacion)) usuario.Ubicacion = model.Ubicacion;
                
                db.SubmitChanges();
            }
            return View();
        }
    }
}
