using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;
using System.IO;

namespace BiblioContenidos_2.Controllers
{
    [Authorize(Roles = "Usuario")]
    
    public class UpLoadLibroController : Controller
    {
        //
        // GET: /UpLoadLibro/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(CargarLibro model, HttpPostedFileBase file, HttpPostedFileBase file2)
        {
            var fileName = "";
            if (file != null && file.ContentLength > 0)
            {
                fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/Uploads/Pdf"), fileName);

                file.SaveAs(path);
            }

            var fileName2 = "";
            if (file2 != null && file2.ContentLength > 0)
            {
                fileName2 = Path.GetFileName(file2.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/Uploads/Img"), fileName2);

                file2.SaveAs(path);
            }
            model.Portada = fileName2;
            model.PdfLibro = fileName;


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
                    //IdCategoria = 1,
                    UrlReal = model.PdfLibro,
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
                    AnhoPublicacion = (int)model.Anho,
                    IdContenido = IdConte
                };
                db.Libros.InsertOnSubmit(libro);
                db.SubmitChanges();

                //Categorias
                char[] separadores = { ',' };
                string[] categorias = model.Categorias.Split(separadores);
                char[] t = { ' ' };
                List<string> CatRechazadas = new List<string>();

                foreach (string str in categorias)
                {
                    string str2 = str.Trim(t);
                    int esta = db.Categorias.Count(c => c.Descripcion == str2);
                    
                    if (esta == 0)
                    {
                        Categoria categoria = new Categoria()
                        {
                            Descripcion = str2,
                            Estado = "Pendiente"
                        };
                        db.Categorias.InsertOnSubmit(categoria);
                        db.SubmitChanges();
                    }

                    int IdCat = db.Categorias.Single(c => c.Descripcion == str2).Id;
                    string estado = db.Categorias.Single(c => c.Descripcion==str2).Estado;
                    if (estado != "Rechazado")
                    {
                        RelContenidosCategoria rel = new RelContenidosCategoria()
                        {
                            IdContenido = IdConte,
                            IdCategoria = IdCat
                        };
                        db.RelContenidosCategorias.InsertOnSubmit(rel);
                    }
                    else
                        CatRechazadas.Add(str2);
                        
                   
                }
                ViewBag.CatRecha = CatRechazadas;
                
                db.SubmitChanges();
            }
            return View();
            
        }
    }
}
