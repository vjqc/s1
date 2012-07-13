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

        [HttpGet]
        public ActionResult VerCurso(string id)
        {
            //string i = id;
            //int a = 0;
            if (!String.IsNullOrEmpty(id) && EsNro(id) )
            {
                int IdContenido = Convert.ToInt32(id);
                DataClasses1DataContext db = new DataClasses1DataContext();
                Contenido contenido = db.Contenidos.Single(c => c.Id == IdContenido);

                ViewBag.Curso = contenido;
                
                //int NroMeGustas = db.Gustas.Count(p => p.IdContenido == IdContenido);
                //if (NroMeGustas == 0) ViewBag.msg = "Se la primera persona a quien le gusta ésto";
                //else if (NroMeGustas == 1) ViewBag.msg = "A una persona le gusta ésto";
                //else ViewBag.msg = "A " + NroMeGustas + " personas les gusta ésto";

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

                if (NombreUsuario == "Admin") return "";
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

            int CantidadComentarios = db.Comentarios.Count(a => a.IdContenido == IdContenido && a.Estado=="Aceptado");
            List<ComentarioView> ListaCom = db.Comentarios.Where(a => a.IdContenido == IdContenido && a.Estado == "Aceptado").Select(c => new ComentarioView()
            {
                Avatar = c.Usuario.Avatar,
                Nick = c.Usuario.aspnet_User.UserName,
                Puntuacion = (int)c.Usuario.Karma,
                FechaPublicacion = c.Fecha,
                ContenidoComentario = c.Comentario1
            }).ToList();
            
            string str = (CantidadComentarios == 0) ? "No hay comentarios" : "Hay " + CantidadComentarios + " comentario(s)";
             //string str = (CantidadComentarios == 0) ? "No hay comentarios" : "<div id='comments-block'><div class='n-comments'>"+CantidadComentarios+"</div> <div class='n-comments-text'>comments</div>";
            

            str += "<table>";
            foreach (var item in ListaCom)
            {
                str += "<tr><td><img src='" + Url.Action("ObtenerUrlImagen", "Home", new { id = item.Avatar }) + "' alt='' height='50' width='50'/>" + item.Nick + "("+ item.Puntuacion + ")</td><td>" + item.FechaPublicacion + "</td></tr>";
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

        public ActionResult VerCurso2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Buscar(string s)
        {
            string str = s;
            ViewBag.cad = str;
            
            if (!String.IsNullOrEmpty(str))
                return Redirect("../Contenido/Busqueda/" + str);

            return Redirect("../Home/Index");
        }

        [HttpGet]
        public ActionResult Busqueda(string id)
        {
            string str = id;
            DataClasses1DataContext db = new DataClasses1DataContext();

            var res = db.Contenidos.Where(c => c.Estado=="Aceptado" && (
                                               c.Tipo.Contains(str) ||
                                               c.Titulo.Contains(str) ||
                                               c.Descripcion.Contains(str)
                                            )
                                         );
            ViewBag.data = res;
            return View(res);
        }

        [HttpGet]
        public ActionResult VerContenido(string id)
        {
            if (EsNro(id))
            {
                int IdContenido = Int32.Parse(id);
                DataClasses1DataContext db = new DataClasses1DataContext();

                string Tipo = db.Contenidos.Single(c => c.Id == IdContenido).Tipo;
                switch (Tipo)
                {
                    case "Libro": return Redirect("../../DetalleLibro/Index/" + IdContenido);
                    case "Articulo": return Redirect("../../Contenido/VerArticulo/" + IdContenido);
                    case "Curso": return Redirect("../../Contenido/VerCurso/" + IdContenido);
                }
            }

            return Redirect("../../Home/Index/");
        }

        [HttpGet]
        [Authorize(Roles = "Usuario")]
        public ActionResult ReEdicion(string id)
        {
            if (EsNro(id))
            {
                int IdContenido = Int32.Parse(id);
                DataClasses1DataContext db = new DataClasses1DataContext();
                Contenido cnt = db.Contenidos.Single(c => c.Id == IdContenido);
                ViewBag.Contenido = cnt;
                //ViewBag.Contenido.Descripcion = VicoCode(cnt.Descripcion);
            }
            return View();
        }

        public ActionResult UltimosLibros()
        {
            return View();
        }
    }
}
