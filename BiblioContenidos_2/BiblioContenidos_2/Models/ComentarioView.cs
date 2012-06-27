using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblioContenidos_2.Models
{
    public class ComentarioView
    {
        public string Avatar { set; get; }
        public string Nick { set; get; }
        public int Puntuacion { set; get; }
        public DateTime FechaPublicacion { set; get; }
        public string ContenidoComentario { set; get; }
    }
}