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

        public bool EsNro(string str)
        {
            for (int k = 0; k < str.Length; k++)
                if (str[k] < '0' || str[k] > '9')
                    return false;
            return true;
        } 

        public ActionResult Ver()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            ViewBag.lista = db.Contenidos;
            return View();
        }


        public ActionResult TinyEditor()
        {

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TinyEditor(string editor)
        {
            //ViewBag.curso = editor;
            DataClasses1DataContext db = new DataClasses1DataContext();

            Contenido curso = new Contenido()
            {
                FechaPublicacion = DateTime.Now,
                IdUsuario = IdUsuarioActual(),
                Tipo = "Curso",
                Titulo = "",
                Descripcion = editor,
                UrlReal = "",
                UrlVirtual = "",
                Estado = "Pendiente"
            };

            db.Contenidos.InsertOnSubmit(curso);
            db.SubmitChanges();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Usuario")]
        [ValidateInput(false)]
        public ActionResult GuardarReEdicionCurso(string IdContenido, string editor)
        {
            //Contenido c = info;
            if (!String.IsNullOrEmpty(editor) && EsNro(IdContenido))
            {
                int Id = Int32.Parse(IdContenido);

                DataClasses1DataContext db = new DataClasses1DataContext();
                Contenido cont = db.Contenidos.Single(c => c.Id == Id);

                cont.FechaPublicacion = DateTime.Now;
                cont.Descripcion = editor;
                cont.Estado = "Aceptado";

                //db.Contenidos.InsertOnSubmit(info);
                db.SubmitChanges();
            }
            return Redirect("../Home/Index");
        }
    }
}
