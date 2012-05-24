using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BiblioContenidos_2.Models
{
    public class CargarLibro
    {
        [Required(ErrorMessage="Quien es el autor del libro?")]
        public string Autor { set; get; }

        [Required(ErrorMessage = "En qué año se publico?")]
        public int Anho { set; get; }

        [Required(ErrorMessage = "Cuál es su título del libro?")]
        public string Tema { set; get; }

        public string Descripcion { set; get; }

        //[Required(ErrorMessage = "Falta la portada del libro")]
        public string Portada { set; get; }

        //[Required(ErrorMessage = "Falta el pdf del libro")]
        public string PdfLibro { set; get; }
        
        public DateTime Fecha { set; get;  }
    }
}