using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BiblioContenidos_2.Models
{
    public static class Util
    {
        public static int NSub(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                int n = str.Length;

                return (n <= 20 ? n : 20);
            }

            return 0;
        }
    }
}