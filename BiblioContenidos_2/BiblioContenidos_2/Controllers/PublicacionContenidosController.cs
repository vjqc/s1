using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

    }
}
