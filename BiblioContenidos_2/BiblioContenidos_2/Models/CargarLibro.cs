using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblioContenidos_2.Models
{
    public class CargarLibro
    {
        public string Autor { set; get; }
        public int Anho { set; get; }
        public string Tema { set; get; }
        public string Descripcion { set; get; }
        public string Portada { set; get; }
        public DateTime Fecha { set; get;  }
    }
}