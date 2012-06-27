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

        /*
        [HttpGet] ///Sin Ajax
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
        */

        [HttpPost]
        [Authorize(Roles = "Usuario")]
        public string MeGustaPost(string IdConte)
        {
            int IdContenido = Convert.ToInt32(IdConte);
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

            ViewBag.msg = "";
            int NroMeGustas = db.Gustas.Count(p => p.IdContenido == IdContenido);
            
            return (NroMeGustas == 1)? "A una persona le gusta ésto": "A " + NroMeGustas + " personas les gusta ésto";
            
            
        }

        public bool EsNro(string str)
        {
            for (int k = 0; k < str.Length; k++)
                if (str[k] < '0' || str[k] > '9')
                    return false;
            return true;
        }

       
        [HttpGet]
        public ActionResult VerArticulo(string id)
        {
            //string i = id;
            //int a = 0;
            if (!String.IsNullOrEmpty(id) && EsNro(id) )
            {
                int IdContenido = Convert.ToInt32(id);
                DataClasses1DataContext db = new DataClasses1DataContext();
                Contenido contenido = db.Contenidos.Single(c => c.Id == IdContenido);

                ViewBag.Articulo = contenido;
                
                int NroMeGustas = db.Gustas.Count(p => p.IdContenido == IdContenido);
                if (NroMeGustas == 0) ViewBag.msg = "Se la primera persona a quien le gusta ésto";
                else if (NroMeGustas == 1) ViewBag.msg = "A una persona le gusta ésto";
                else ViewBag.msg = "A " + NroMeGustas + " personas les gusta ésto";

                ViewBag.Comentarios = GetComentarios(IdContenido);
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Usuario")]
        public string Comentar(Comentario com, string conte)
        {
            if (EsNro(conte))
            {
                int IdContenido = Int32.Parse(conte);
                DataClasses1DataContext db = new DataClasses1DataContext();
                string NombreUsuario = User.Identity.Name.ToString();
                System.Guid IdUs = db.aspnet_Users.Single(a => a.UserName == NombreUsuario).UserId;
                int IdUsuario = db.Usuarios.Single(a => a.UserId == IdUs).Id;

                Comentario coment = new Comentario()
                {
                    Comentario1 = com.Comentario1,
                    IdUsuario = IdUsuario,
                    Fecha = DateTime.Now,
                    IdContenido = IdContenido,
                    Estado = "Aceptado"
                };
                db.Comentarios.InsertOnSubmit(coment);

                Usuario user = db.Usuarios.Single(u => u.Id==IdUsuario);
                user.Karma += 0.5;

                db.SubmitChanges();

                ViewBag.Comentarios = "";
                return GetComentarios(IdContenido);
            }
            return "";
        }

        public string GetComentarios(int IdContenido)
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            int CantidadComentarios = db.Comentarios.Count(a => a.IdContenido == IdContenido);
            List<ComentarioView> ListaCom = db.Comentarios.Where(a => a.IdContenido == IdContenido && a.Estado == "Aceptado").Select(c => new ComentarioView()
            {
                Avatar = c.Usuario.Avatar,
                Nick = c.Usuario.aspnet_User.UserName,
                Puntuacion = (int)c.Usuario.Karma,
                FechaPublicacion = c.Fecha,
                ContenidoComentario = c.Comentario1
            }).ToList();
            
            string str = (CantidadComentarios == 0) ? "No hay comentarios" : "Hay " + CantidadComentarios + " comentario(s)";

            str += "<table>";
            foreach (var item in ListaCom)
            {
                str += "<tr><td>" + item.Avatar + "</td><td>" + item.Nick + "</td><td>" + item.Puntuacion + "</td><td>" + item.FechaPublicacion + "</td></tr>";
                str += "<tr><td>" + item.ContenidoComentario + "</td></tr><tr><td></td></tr>";
            }
            str += "</table>";

            return str;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult AprobarContenido(string id)
        {
            int IdContenido = Convert.ToInt32(id);
            DataClasses1DataContext db = new DataClasses1DataContext();
            Contenido contenido = db.Contenidos.Single(c => c.Id == IdContenido);
            contenido.Estado = "Aceptado";

            Usuario publicador = contenido.Usuario;
            switch (contenido.Tipo)
            {
                case "Articulo": publicador.Karma += 10; break;
                case "Tutorial": publicador.Karma += 50; break;
                case "Curso":    publicador.Karma += 200; break;
                case "Libro":    publicador.Karma += 100; break;
            }
            db.SubmitChanges();

            List<RelContenidosCategoria> ListaRelCC = db.RelContenidosCategorias.Where(r=>r.IdContenido==IdContenido).ToList();
            foreach (var item in ListaRelCC)
            {
                Categoria cat = db.Categorias.Single(p => p.Id == item.IdCategoria);
                cat.Estado = "Aceptado";
            }
            db.SubmitChanges();

            return Redirect("/Moderacion/ModeracionContenidos");
        }

    }
}
