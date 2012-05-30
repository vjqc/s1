using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblioContenidos_2.Models
{
    public class DetalleLibro
    {
        public int IdContenido { set; get; }
        public string Autor { set; get; }
        public string Portada { set; get; }
        public string Indice { set; get; }
        public int AnhoPublicacion { set; get; }
        public string Pdf { set; get; }

        public string Titulo { set; get; }
        public string Descripcion { set; get; }

        public string Nick { set; get; }
        public string Avatar { set; get; }

        public List<Categoria> Categos { set; get; }
    }
}