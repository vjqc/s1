using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Globalization;

using System.Web.Helpers;
using BiblioContenidos_2.Models;

namespace BiblioContenidos_2.Controllers
{
    public static class Tema
    {
        public const string Vanilla3D =
        @"<Chart BackColor=""#555"" BackGradientStyle=""TopBottom"" BorderColor=""181, 64, 1"" BorderWidth=""2"" BorderlineDashStyle=""Solid"" Palette=""SemiTransparent"" AntiAliasing=""All"">
	        <ChartAreas>
            <ChartArea Name=""Default"" _Template_=""All"" BackColor=""Transparent"" BackSecondaryColor=""White"" BorderColor=""64, 64, 64, 64"" BorderDashStyle=""Solid"" ShadowColor=""Transparent"">
	            <Area3DStyle LightStyle=""Simplistic"" Enable3D=""True"" Inclination=""30"" IsClustered=""False"" IsRightAngleAxes=""False"" Perspective=""10"" Rotation=""-30"" WallWidth=""0"" />
	        </ChartArea>
	    </ChartAreas>
        </Chart>";
    }

    [Authorize(Roles = "Administrador")]
    public class ModeracionController : Controller
    {
        //
        // GET: /Moderacion/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ModeracionContenidos()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            List<Contenido> ListaPend = db.Contenidos.Where(c => c.Estado == "Pendiente").ToList();

            List<Contenido> lista_con = db.Contenidos.ToList();

            ViewBag.NP = ListaPend.Count();
            ViewBag.ListaPend = ListaPend;

            return View(lista_con);
        }

        public ActionResult ModeracionCategorias()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            List<Categoria> lista_cat = db.Categorias.ToList();

            return View(lista_cat);
        }

        public ActionResult ModeracionComentarios()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            List<Comentario> lista_comen = db.Comentarios.Where(c => c.Estado!="Eliminado").ToList();

            List<NroComentariosXContenido> lista = db.Contenidos.Where(c => c.Estado == "Aceptado").Select(a => new NroComentariosXContenido()
            {
                Conte = db.Contenidos.Single(s => s.Id == a.Id),
                N = db.Comentarios.Count(q => q.IdContenido == a.Id && q.Estado=="Aceptado")

            }).ToList();

            lista = lista.OrderByDescending(u => u.N).Take(5).ToList();
            ViewBag.lista = lista;

            return View(lista_comen);
        }


        public ActionResult ModeracionUsuarios()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            List<Usuario> lista_users = db.Usuarios.ToList();

            return View(lista_users);
        }

        [HttpGet]
        public ActionResult BanearUsuario(string id)
        {
            return Redirect(Url.Content("~/Moderacion/ModeracionUsuarios#cuerpo"));
        }

        public ActionResult RUsuarios()
        {
            return View();
        }

        public ActionResult TopUsuarios()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();
            List<Usuario> TopUsers = db.Usuarios.OrderByDescending(u => u.Karma).Take(3).ToList();

            string[] usuarios = TopUsers.Select(t => t.aspnet_User.UserName).ToArray();
            string[] karmas = TopUsers.Select(t => Convert.ToInt32(t.Karma).ToString()).ToArray();

            var key = new Chart(width: 600, height: 400, theme: Tema.Vanilla3D)
                .AddSeries(
                    chartType: "bar",
                    legend: "Usuarios",
                    
                    xValue: usuarios,
                    yValues: karmas)
                .Write();

            return null;
        }
        public ActionResult ContenidosConMasComentarios()
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            List<NroComentariosXContenido> lista = db.Contenidos.Where(c => c.Estado == "Aceptado").Select(a => new NroComentariosXContenido()
            {
                Conte = db.Contenidos.Single(s => s.Id==a.Id),
                N = db.Comentarios.Count(q => q.IdContenido == a.Id && q.Estado == "Aceptado")

            }).ToList();
            
            lista = lista.OrderByDescending(u => u.N).Take(5).ToList();
            //ViewBag.lista = 555;

            string[] contenidos = lista.Select(q => q.Conte.Descripcion.Substring(0, Util.NSub(q.Conte.Descripcion)) + ".. (" + q.N + ")").ToArray();
            string[] NCom = lista.Select(q => q.N.ToString()).ToArray();

            var key = new Chart(width: 600, height: 400, theme: Tema.Vanilla3D)
                .AddSeries(
                    chartType: "pie",
                    legend: "Contenidos",

                    xValue: contenidos,
                    yValues: NCom)
                .Write();

            return null;
        }

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

        [HttpGet]
        public ActionResult VerActividadUsuario(string id)
        {
            if (!String.IsNullOrEmpty(id) && EsNro(id))
            {
                int IdUs = Convert.ToInt32(id);
                DataClasses1DataContext db = new DataClasses1DataContext();
                Usuario usuario = db.Usuarios.Single(p => p.Id == IdUs);
                ViewBag.usuario = usuario;

                List<DetalleActividad> Lista = db.Contenidos.Where(u => u.IdUsuario == usuario.Id).Select(a => new DetalleActividad()
                {
                    FechaPublicacion = a.FechaPublicacion,
                    Tipo = a.Tipo,
                    Titulo = a.Titulo,
                    Estado = a.Estado,
                    Puntos = GetPuntos(a.Tipo),
                    TotalMeGusta = db.Gustas.Where(g => g.MeGusta == 1 && g.IdContenido == a.Id).Count(),
                    IdContenido = a.Id,

                    Descripcion = a.Descripcion.Substring(0,10)
                }).ToList();

                ViewBag.TotalG = Lista.Sum(l => l.TotalMeGusta);

                return View(Lista);
            }
            return Redirect(Url.Content("~/Moderacion/ModeracionUsuarios#cuerpo"));
        }


        public ActionResult BuscarPalabrasOfensivas()
        {
            List<string> Malas = new List<string>{"mierda","joder","carajo","puta","cochino","huevada","pario","cagada","chingada","mames"};
            //en una nueva version alamacenar en una tabla

            DataClasses1DataContext db = new DataClasses1DataContext();

            List<Comentario> Comentarios = db.Comentarios.ToList();
            List<Comentario> Observados = new List<Comentario>();
            foreach (var item in Comentarios)
            {
                foreach (var mp in Malas)
                {
                    if (item.Comentario1.Contains(mp))
                    {
                        Observados.Add(item);
                        break;
                    }
                }
            }

            //Observados.ElementAt(k).Usuario.aspnet_User.UserName
            //Observados.ElementAt(k)
            ViewBag.Observados = Observados;

            return View(Observados);
            //return Redirect("../Moderacion/ModeracionComentarios#cuerpo");
        }

        public bool EsNro(string str)
        {
            for (int k = 0; k < str.Length; k++)
                if (str[k] < '0' || str[k] > '9')
                    return false;
            return true;
        }

        [HttpGet]
        public ActionResult EliminarComentario(string id)
        {
            if (!String.IsNullOrEmpty(id) && EsNro(id))
            {
                int IdComentario = Convert.ToInt32(id);
                DataClasses1DataContext db = new DataClasses1DataContext();

                Comentario comen = db.Comentarios.Single(c => c.Id == IdComentario);
                comen.Estado = "Eliminado";
                db.SubmitChanges();              
            }
            return Redirect(Url.Content("~/Moderacion/ModeracionComentarios#"));
        }


        public ActionResult ReemplazarCategoria(string fuente, string destino)
        {
            DataClasses1DataContext db = new DataClasses1DataContext();

            int ExisteFuente = db.Categorias.Count(c => c.Descripcion==fuente);
            if (ExisteFuente == 1)
            {
                int codF = db.Categorias.Single(c => c.Descripcion == fuente).Id;
                int ExisteDestino = db.Categorias.Count(c => c.Descripcion == destino && c.Estado=="Aceptado");

                if (ExisteDestino == 0)
                {
                    Categoria NuevaCat = new Categoria
                    {
                        Descripcion = destino,
                        Estado = "Aceptado"
                    };
                    db.Categorias.InsertOnSubmit(NuevaCat);
                    db.SubmitChanges();
                }

                int codD = db.Categorias.Single(c => c.Descripcion == destino).Id; 
                List<RelContenidosCategoria> lista = db.RelContenidosCategorias.Where(r => r.IdCategoria == codF).ToList();

                foreach (var item in lista)
                {
                    //item.IdCategoria = codD;
                    RelContenidosCategoria NuevaRel = new RelContenidosCategoria
                    {
                        IdCategoria = codD,
                        IdContenido = item.IdContenido
                    };
                    RelContenidosCategoria ViejaRel = db.RelContenidosCategorias.Single(r => r.IdCategoria == codF);
                    
                    db.RelContenidosCategorias.InsertOnSubmit(NuevaRel);
                    db.RelContenidosCategorias.DeleteOnSubmit(ViejaRel);
                }
                Categoria CatElim = db.Categorias.Single(c => c.Descripcion == fuente);
                CatElim.Estado = "Eliminado";

                db.SubmitChanges();
            }

            return Redirect("../Moderacion/ModeracionCategorias#cuerpo");
        }
    }
}
