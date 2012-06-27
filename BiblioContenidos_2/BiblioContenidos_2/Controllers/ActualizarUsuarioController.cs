using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;
using System.IO;

namespace BiblioContenidos_2.Controllers
{
    [Authorize(Roles="Usuario")]
    public class ActualizarUsuarioController : Controller
    {
        //
        // GET: /ActualizarUsuario/

        public int GetPuntos(string Tipo)
        {
            switch (Tipo)
            {
                case "Articulo": return 10;
                case "Tutorial": return 50;
                case "Curso": return 200;
                case "Libro": return 100;
            }
            return 0;
        }

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
            //ViewBag.Resumen = db.Contenidos.Where(q => q.IdUsuario == usuario.Id); ok sprint 1
            
            List<DetalleActividad> Lista = db.Contenidos.Where(u=>u.IdUsuario==usuario.Id).Select(a => new DetalleActividad()
            {
                FechaPublicacion = a.FechaPublicacion,
                Tipo = a.Tipo,
                Titulo = a.Titulo,
                Estado = a.Estado,
                Puntos = GetPuntos(a.Tipo),
                TotalMeGusta = db.Gustas.Where(g=>g.MeGusta==1 && g.IdContenido==a.Id).Count()
            }).ToList();

            ViewBag.TotalG  = Lista.Sum(l => l.TotalMeGusta);
            ViewBag.Resumen = Lista;
           
                        
            return View();
        }

        public ActionResult MostrarFoto()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            string NombreUsuario = User.Identity.Name.ToString();
            
            string foto = db.Usuarios.Where(u => u.aspnet_User.UserName == NombreUsuario).Select(f => f.Avatar).ToArray()[0];
            if (String.IsNullOrEmpty(foto)) foto = "fotito.jpg";
            //ViewBag.f = foto;

            return File(Path.Combine(Server.MapPath("~/App_Data/Uploads/Img"),foto), "image/jpg");
        }

        [HttpPost]
        public ActionResult Index(ActualizarUsuario model, HttpPostedFileBase file)
        {
            var fileName = "";
            if (file!=null && file.ContentLength > 0)
            {
                fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/Uploads/Img"), fileName);

                file.SaveAs(path); 
            }

            if (ModelState.IsValid && model!=null)
            {
                DataClasses1DataContext db = new DataClasses1DataContext();
                    
                string NombreUsuario = User.Identity.Name.ToString();
                System.Guid IdUs = db.aspnet_Users.Where(a => a.UserName == NombreUsuario).Select(a => a.UserId).ToArray()[0];

                string ce = db.aspnet_Memberships.Where(p => p.UserId == IdUs).Select(p => p.Email).ToArray()[0];
                ViewBag.ce = ce;

                Usuario usuario = db.Usuarios.Single(p => p.UserId == IdUs);
                ViewBag.usuario = usuario;

                if (!String.IsNullOrEmpty(fileName))       usuario.Avatar = fileName;
                    
                if ( !String.IsNullOrEmpty(model.ApPaterno) ) usuario.ApPaterno = model.ApPaterno;
                if ( !String.IsNullOrEmpty(model.ApMaterno) ) usuario.ApMaterno = model.ApMaterno;
                if ( !String.IsNullOrEmpty(model.Nombres) )   usuario.Nombres = model.Nombres;
                if ( !String.IsNullOrEmpty(model.Ubicacion))  usuario.Ubicacion = model.Ubicacion;
                if (!String.IsNullOrEmpty(model.Intereses)) 
                { 
                
                }
                
                db.SubmitChanges();
            }
            return View();
        }
    }
}
