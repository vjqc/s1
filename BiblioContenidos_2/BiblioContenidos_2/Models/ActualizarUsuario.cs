using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BiblioContenidos_2.Models
{
    public class ActualizarUsuario
    {
        public string Email { set; get; }

        public string Avatar { set; get; }
        
        [RegularExpression("[A-Za-z]{3,30}" , ErrorMessage="[Ap. Paterno] no válido")]
        public string ApPaterno { set; get; }

        [RegularExpression("[A-Za-z]{3,30}", ErrorMessage = "[Ap. Materno] no válido")]
	    public string ApMaterno { set; get; }

        [RegularExpression("[A-Za-z A-Za-z]{3,30}", ErrorMessage = "[Nombres] no válido")]
        public string Nombres { set; get; }

        [RegularExpression("[A-Za-z A-Za-z]{3,30}", ErrorMessage = "[Ubicación] no válida")] //Modif
	    public string Ubicacion { set; get; }
    }
}