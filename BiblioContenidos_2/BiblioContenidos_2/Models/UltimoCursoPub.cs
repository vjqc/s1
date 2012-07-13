using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblioContenidos_2.Models
{
    public class UltimoCursoPub
    {
        public int IdUsuario { set; get; }
        public int IdContenido { set; get; }
        public string Nick { set; get; }
        public int Karma { set; get; }

        public string Titulo { set; get; }
        public string Descripcion { set; get; }
        public DateTime Fecha { set; get; }
        public List<Categoria> ListaCategorias { set; get; }
    }
}