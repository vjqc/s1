using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblioContenidos_2.Models
{
    public class DetalleActividad
    {
        public DateTime FechaPublicacion{set; get;}
        public string Tipo{set; get;}
        public string Titulo { set; get; }
        public string Estado { set; get; }
        public int Puntos{set; get;}
        public int TotalMeGusta{set; get;}
        public int IdContenido { set; get; }

        public string Descripcion { set; get; }
    }
}