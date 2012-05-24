using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BiblioContenidos_2.Models;

namespace BiblioContenidos_2.Controllers
{
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

            ViewBag.ListaPend = ListaPend;
            return View();
        }

    }
}
