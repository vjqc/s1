using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Trirand.Web.Mvc;
using System.Web.UI.WebControls;

namespace BiblioContenidos_2.Models
{
    public class ModeracionCategorias
    {
        public JQGrid CategoriasGrid { get; set; }

        public ModeracionCategorias()
        {
            CategoriasGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                {
                    new JQGridColumn {
                        DataField = "Id",
                        PrimaryKey = true,
                        Editable = false,
                        Width = 50
                    },
   
                    new JQGridColumn {
                        DataField = "Descripcion",
                        Editable = true,
                        Width = 200
                    },

                    new JQGridColumn {
                        DataField = "Estado",
                        Editable = true,
                        Width = 100
                    }
                },
                Width = Unit.Pixel(600)
            };

            CategoriasGrid.ToolBarSettings.ShowRefreshButton = true;
            //CategoriasGrid.ToolBarSettings.ShowViewRowDetailsButton = true;
            CategoriasGrid.PagerSettings.PageSize = 6;
            //CategoriasGrid.PagerSettings.

        }

    }
}