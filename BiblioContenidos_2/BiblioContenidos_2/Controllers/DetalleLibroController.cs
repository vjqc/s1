using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;
using System.IO;
//using System.IO;

namespace BiblioContenidos_2.Controllers
{
    public class DetalleLibroController : Controller
    {
        //
        // GET: /DetalleLibro/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index(string id)
        {
            int IdCont = 0;

            if( !String.IsNullOrEmpty(id) ) IdCont = Int32.Parse(id);
            if (IdCont == 0) return Redirect("/Home/Index");

            DataClasses1DataContext db = new DataClasses1DataContext();

            int con = db.Gustas.Count(p => p.IdContenido == IdCont);
            if (con == 0) ViewBag.msg = "Sé la primera persona a quien le gusta ésto";
            //else ViewBag.msg = "";
            else if (con == 1) ViewBag.msg = "A una persona le gusta ésto";
            else ViewBag.msg = "A " + con + " personas les gusta ésto";
             
            
            Contenido cnt = db.Contenidos.Single(c => c.Id == IdCont);
            DetalleLibro Detalle = new DetalleLibro()
            {
                IdContenido = cnt.Id,
                Titulo = cnt.Titulo,
                Descripcion = cnt.Descripcion,
                Autor = cnt.Libros.Single(lb => lb.IdContenido == cnt.Id).Autor,
                Portada = cnt.Libros.Single(lb => lb.IdContenido == cnt.Id).Portada,
                Indice = "",
                AnhoPublicacion = (int)cnt.Libros.Single(lb => lb.IdContenido == cnt.Id).AnhoPublicacion,
                Pdf = cnt.UrlReal,
                
                Nick = cnt.Usuario.aspnet_User.UserName,
                Avatar = cnt.Usuario.Avatar,
         
                Categos = db.RelContenidosCategorias.Where(rc => rc.IdContenido==cnt.Id).Select(p => p.Categoria).ToList()

            };

            ViewBag.Detalle = Detalle;

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Usuario")]
        public ActionResult DescargarLibro(string id)
        {
            if (String.IsNullOrEmpty(id)) id = "";
            //if (id.Length == 0) return HttpNotFound();
            if (id.Length == 0) return RedirectPermanent("/Home/Index");

            return File(Path.Combine(Server.MapPath("~/App_Data/Uploads/Pdf"), id), "application/pdf");
        }
    }
}
